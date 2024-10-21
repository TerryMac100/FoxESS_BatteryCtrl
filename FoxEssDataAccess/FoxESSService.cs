using ApplicationFramework;
using FoxEssDataAccess.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace FoxEssDataAccess
{
    public class FoxESSService
    {
        private readonly ISettings m_settings;
        private readonly IServiceProvider m_service;

        private IMainWindowViewModel m_mainWindowViewModel;

        /// <summary>
        /// Lazy load the main window dependence
        /// </summary>
        private IMainWindowViewModel mainWindowViewModel
        {
            get
            {
                if (m_mainWindowViewModel == null)
                    m_mainWindowViewModel = m_service.GetRequiredService<IMainWindowViewModel>();

                return m_mainWindowViewModel;
            }
        }

        public FoxESSService(ISettings settings, IServiceProvider service)
        {
            m_settings = settings;
            m_service = service;
            m_mainWindowViewModel = mainWindowViewModel;
        }

        private static string GenMd5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            md5.ComputeHash(Encoding.UTF8.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                // change it into 2 hexadecimal digits  
                // for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        private static string ToHexString(string path, string token, string timestamp)
        {
            var myString = $"{path}\\r\\n{token}\\r\\n{timestamp}";

            return GenMd5Hash(myString);
        }

        private RestRequest GetHeader(string path, RestSharp.Method method)
        {
            var request = new RestRequest(domain + path, method);

            long time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var signature = ToHexString(path, m_settings.ApiKey, time.ToString());

            request.AddHeader("token", m_settings.ApiKey);
            request.AddHeader("signature", signature);
            request.AddHeader("timestamp", time.ToString());
            request.AddHeader("lang", lang);
            request.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36");

            return request;
        }

        public async Task<GetTimeSegmentResponse?> GetSchedule()
        {
            try
            {
                m_mainWindowViewModel.SetBusy("Getting Schedule");

                var request = GetHeader("/op/v0/device/scheduler/get", Method.Post);

                request.AddBody(new DeviceSerialNumber(m_settings));

                var client = new RestClient();

                RestResponse response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && response.Content != null)
                {
                    GetTimeSegmentResponse? ret = JsonConvert.DeserializeObject<GetTimeSegmentResponse>(response.Content);

                    mainWindowViewModel.ClearBusy();
                    return ret;
                }

                mainWindowViewModel.SetErrorMessage($"Get Schedule Failed - Requested Rejected");
                return null;
            }
            catch (Exception ex)
            {
                mainWindowViewModel.SetErrorMessage($"Get Schedule Failed - {ex.Message}");
                return null;
            }
        }

        public async Task<SetScheduleResponse?> SetSchedule(SetSchedule setSchedule)
        {
            try
            {
                m_mainWindowViewModel.SetBusy("Setting Schedule");

                var request = GetHeader("/op/v0/device/scheduler/enable", Method.Post);

                request.AddBody(setSchedule);

                var client = new RestClient();

                RestResponse response = await client.ExecuteAsync(request);

                if (response.IsSuccessful && response.Content != null)
                {
                    SetScheduleResponse? ret = JsonConvert.DeserializeObject<SetScheduleResponse>(response.Content);
                    if (ret != null)
                        if (ret.Errno == 0)
                            mainWindowViewModel.ClearBusy();
                        else
                            mainWindowViewModel.SetErrorMessage($"Set Schedule Failed - Error {ret.Errno} - {ret.Msg}");

                    return ret;
                }

                mainWindowViewModel.SetErrorMessage("Set Schedule Failed - Message Rejected");
                return null;
            }
            catch (Exception ex)
            {
                mainWindowViewModel.SetErrorMessage($"Set Schedule Failed - {ex.Message}");
                return null;
            }
        }

        private const string lang = "en";
        private const string domain = "https://www.foxesscloud.com";
    }
}
