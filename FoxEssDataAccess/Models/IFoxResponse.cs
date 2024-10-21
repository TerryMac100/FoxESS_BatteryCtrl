using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxEssDataAccess.Models
{
    public interface IFoxResponse
    {
        public int Errno { get; set; }

        public string Msg { get; set; }
    }
}
