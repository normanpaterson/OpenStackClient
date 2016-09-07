using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStackClient
{
    public class OpenStackFlavorsListResponse
    {
        public List<Flavor> flavors { get; set; }
    }

    public class Flavor
    {
        public string id { get; set; }
        public List<Link> links { get; set; }
        public string name { get; set; }
    }

}
