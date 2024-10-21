using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxEssDataAccess.Models
{
    public interface IFoxRequest
    {
        void Validate();

        string RequestUri { get; }

        bool GetRequest { get; }
    }
}
