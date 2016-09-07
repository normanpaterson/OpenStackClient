using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStackClient
{
    public class Link
    {
        public string href { get; set; }
        public string rel { get; set; }
        public string type { get; set; }
    }

    public class Image
    {
        public string id { get; set; }
        public List<Link> links { get; set; }
        public string name { get; set; }
    }

    public class OpenStackImageListResponse
    {
        public List<Image> images { get; set; }
    }
}
