using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStackClient
{
    public class OpenStackServerResponse
    {
        public Server server { get; set; }
    }
    public class SecurityGroup
    {
        public string name { get; set; }
    }

    public class IpAddress
    {
        public string addr { get; set; }
        public int version { get; set; }
    }

    public class Addresses
    {
        public List<IpAddress> @private { get; set; }
        public List<IpAddress> @public { get; set; }
    }

    public class Server
    {
        public List<SecurityGroup> security_groups { get; set; }
        public string id { get; set; }
        public List<Link> links { get; set; }
        public string adminPass { get; set; }
        public string accessIPv4 { get; set; }
        public string accessIPv6 { get; set; }
        public Addresses addresses { get; set; }
        public string created { get; set; }
        public Flavor flavor { get; set; }
        public string hostId { get; set; }
        public Image image { get; set; }
        public Metadata metadata { get; set; }
        public string name { get; set; }
        public int progress { get; set; }
        public string status { get; set; }
        public string tenant_id { get; set; }
        public string updated { get; set; }
        public string user_id { get; set; }
    }

}
