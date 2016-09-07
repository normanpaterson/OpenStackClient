using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStackClient
{
    public class OpenStackServerNetworkResponse
    {
        public Network network { get; set; }
    }

    public class Network
    {
        public string id { get; set; }
        public List<IpAddress> ip { get; set; }
    }
}
