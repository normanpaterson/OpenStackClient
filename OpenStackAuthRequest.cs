using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStackClient
{
    class OpenStackAuthRequest
    {
        public auth auth { get; set; }

        public OpenStackAuthRequest(string tenantName = null, string username = null, string password = null)
        {
            this.auth = new auth();
            this.auth.tenantName = tenantName;
            this.auth.passwordCredentials = new passwordCredentials();
            this.auth.passwordCredentials.username = username;
            this.auth.passwordCredentials.password = password;
        }
    }
    class auth
    {
        public string tenantName { get; set; }
        public passwordCredentials passwordCredentials { get; set; }
    }
    class passwordCredentials
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
