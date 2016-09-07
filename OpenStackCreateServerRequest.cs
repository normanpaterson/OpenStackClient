using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStackClient
{
    class OpenStackCreateServerRequest
    {
        public server server { get; set; }

        public OpenStackCreateServerRequest(string serverName = null, string imageRef = null, string flavorRef = null, string user_data = null, string key_name = null)
        {
            this.server = new server();
            this.server.flavorRef = flavorRef;
            this.server.imageRef = imageRef;
            this.server.name = serverName;
            this.server.user_data = user_data;
            this.server.key_name = key_name;
        }
    }
    class server
    {
        public string name { get; set; }
        public string imageRef { get; set; }
        public string flavorRef { get; set; }
        public string user_data { get; set; }
        public string key_name { get; set; }
    }
}
