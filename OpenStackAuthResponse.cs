using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStackClient
{
    public class Tenant
    {
        public object description { get; set; }
        public bool enabled { get; set; }
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Token
    {
        public string issued_at { get; set; }
        public string expires { get; set; }
        public string id { get; set; }
        public Tenant tenant { get; set; }
        public List<string> audit_ids { get; set; }
    }

    public class Endpoint
    {
        public string adminURL { get; set; }
        public string region { get; set; }
        public string internalURL { get; set; }
        public string id { get; set; }
        public string publicURL { get; set; }
    }

    public class ServiceCatalog
    {
        public List<Endpoint> endpoints { get; set; }
        public List<object> endpoints_links { get; set; }
        public string type { get; set; }
        public string name { get; set; }
    }

    public class Role
    {
        public string name { get; set; }
    }

    public class User
    {
        public string username { get; set; }
        public List<object> roles_links { get; set; }
        public string id { get; set; }
        public List<Role> roles { get; set; }
        public string name { get; set; }
    }

    public class Metadata
    {
        public int is_admin { get; set; }
        public List<string> roles { get; set; }
    }

    public class Access
    {
        public Token token { get; set; }
        public List<ServiceCatalog> serviceCatalog { get; set; }
        public User user { get; set; }
        public Metadata metadata { get; set; }
    }

    public class OpenStackAuthResponse
    {
        public Access access { get; set; }
        public string endPointUrl { get; set; }
        public string tenantId { get; set; }
    }
}
