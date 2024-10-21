using FoxEssDataAccess.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxEssDataAccess
{
    public class Register
    {
        public static IServiceCollection RegisterItems(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<FoxESSService, FoxESSService>();

            return serviceCollection;
        }
    }
}
