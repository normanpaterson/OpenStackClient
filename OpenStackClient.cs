using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OpenStackClient
{
    /// <summary>
    /// Copyright (c) 2015 University of Chicago CRI. All rights reserved.

    /// Developed by: Center for Research Informatics
    /// University of Chicago
    /// http://cri.uchicago.edu/

    /// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal with the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

    /// Redistributions of source code must retain this copyright notice, this list of conditions and the following disclaimers.
    /// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimers in the documentation and/or other materials provided with the distribution.
    /// Neither the names of Center for Research Informatics, University of Chicago, nor the names of its contributors may be used to endorse or promote products derived from this Software without specific prior written permission.

    /// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE CONTRIBUTORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS WITH THE SOFTWARE.

    /// </summary>
    public class OpenStackClient
    {
        private Token token = new Token();
        private List<Image> images = new List<Image>();
        private List<Flavor> flavors = new List<Flavor>();
        private string computeUrl = "";
        private string tenantId = "";
        private string serverId = "";
        private Server server = new Server();
        private Addresses addresses = new Addresses();

        public string Username { get; set; }

        public string Password { get; set; }

        public string IdentityUrl { get; set; }

        public string TenantName { get; set; }

        public Token Token { get { return token; } }

        public Server Server { get { return server; } }

        public List<Image> Images { get { return images; } }

        public Addresses Addresses { get { return addresses; } }

        public string ServerId
        {
            get { return serverId; }
        }
        /// <summary>
        /// Authenticate against an OpenStack server
        /// </summary>
        /// <returns></returns>
        /// <remarks>Sample Usage:
        /// OpenStackClient.OpenStackClient osc = new OpenStackClient.OpenStackClient();
        /// osc.Username = "OpenStackUserName";
        /// osc.Password = "OpenStackPassword";
        /// osc.TenantName = "TenantName";
        /// osc.IdentityUrl = identityURI;
        /// </remarks>
        public async Task<OpenStackAuthResponse> Authenticate(string ServiceCatalogName)
        {
            OpenStackAuthResponse resp = new OpenStackAuthResponse();
            resp = await OpenStackAuthenticate(IdentityUrl, TenantName, Username, Password);
            this.token = resp.access.token;

            //find the public endpoint for the compute API (needed to spin up VM)
            foreach (ServiceCatalog sc in resp.access.serviceCatalog)
            {
                if (sc.name == ServiceCatalogName)
                {
                    resp.endPointUrl = sc.endpoints[0].publicURL;
                }
            }
            resp.tenantId = resp.access.token.tenant.id;

            return resp;
        }

        #region Public Methods

        #region Image Methods

        public async Task<OpenStackImageListResponse> GetImages(string endPointUrl, string tenantId, string userName, string token)
        {
            this.computeUrl = endPointUrl;
            this.tenantId = tenantId;
            this.Username = userName;
            this.token.id = token;
            OpenStackImageListResponse resp = new OpenStackImageListResponse();
            resp = await OpenStackListImages(endPointUrl, tenantId, userName, token);
            this.images = resp.images;
            return resp;
        }

        public async Task<Image> GetImageByName(string name)
        {
             var obj = await GetImages(this.computeUrl, this.tenantId, this.Username, this.token.id);
            Image image = null;

            foreach (Image img in obj.images)
            {
                if (img.name == name)
                {
                    image = img;
                }
            }
            return image;
        }

        public string GetImageRefByImageName(string name)
        {
            GetImages(this.computeUrl, this.tenantId, this.Username, this.token.id);
            string imageRef = "";

            foreach (Image img in images)
            {
                if (img.name == name)
                {
                    foreach (Link lnk in img.links)
                    {
                        if (lnk.rel == "self")
                        {
                            imageRef = lnk.href;
                        }
                    }
                }
            }
            return imageRef;
        }

        public Image GetImageByKeyword(string keyword)
        {
            GetImages(this.computeUrl, this.tenantId, this.Username, this.token.id);
            Image image = null;

            foreach (Image img in images)
            {
                if (img.name.Contains(keyword))
                {
                    image = img;
                    break;
                }
            }
            return image;
        }

        public async Task<string> GetImageRefByImageKeyword(string keyword)
        {
            var obj = await GetImages(this.computeUrl, this.tenantId, this.Username, this.token.id);
            string imageRef = "";

            foreach (Image img in images)
            {
                if (img.name.Contains(keyword))
                {
                    foreach (Link lnk in img.links)
                    {
                        if (lnk.rel == "self")
                        {
                            imageRef = lnk.href;
                            break;
                        }
                    }
                    break;
                }
            }
            return imageRef;
        }

        #endregion Image Methods

        #region Flavor Methods

        public async Task<OpenStackFlavorsListResponse> GetFlavors()
        {
            OpenStackFlavorsListResponse resp = new OpenStackFlavorsListResponse();
            resp = await OpenStackListFlavors(this.computeUrl, this.tenantId, this.Username, this.Token.id);
            this.flavors = resp.flavors;
            return resp;
        }

        public async Task<string> GetFlavorRefByFlavorId(string flavorId)
        {
            OpenStackFlavorsListResponse obj = await GetFlavors();
            string flavorRef = "";

            foreach (Flavor flv in flavors)
            {
                if (flv.id == flavorId)
                {
                    foreach (Link lnk in flv.links)
                    {
                        if (lnk.rel == "self")
                        {
                            flavorRef = lnk.href;
                            break;
                        }
                    }
                    break;
                }
            }
            return flavorRef;
        }

        #endregion Flavor Methods

        #region Server Methods
        /// <summary>
        /// Creates a VM on the OpenStack Server
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="imageRef"></param>
        /// <param name="flavorRef"></param>
        /// <param name="user_data"></param>
        /// <param name="key_name"></param>
        /// <returns>OpenStackServerResponse</returns>
        public async Task<OpenStackServerResponse> CreateServer(string serverName, string imageRef, string flavorRef, string user_data, string key_name)
        {
            OpenStackServerResponse resp = new OpenStackServerResponse();
            string encodedUserData = Convert.ToBase64String(Encoding.Default.GetBytes(user_data));
            resp = await OpenStackCreateServer(this.computeUrl, this.tenantId, this.Username, this.token.id, serverName, user_data, imageRef, flavorRef, key_name);
            this.serverId = resp.server.id;
            this.server = resp.server;
            return resp;
        }

        public async Task<OpenStackServerResponse> GetServerStatus(string serverId)
        {
            OpenStackServerResponse resp = new OpenStackServerResponse();

            resp = await OpenStackGetServer(this.computeUrl, this.tenantId, this.Username, this.token.id, serverId);

            return resp;
        }

        public async Task<OpenStackServerResponse> GetServerStatus(string serverId, string computeUrl, string tenantId, string Username, string tokenID)
        {
            OpenStackServerResponse resp = new OpenStackServerResponse();

            resp = await OpenStackGetServer(computeUrl, tenantId, Username, tokenID, serverId);

            return resp;
        }

        public async Task<OpenStackServerAddressResponse> GetServerAddresses(string serverId)
        {
            OpenStackServerAddressResponse resp = new OpenStackServerAddressResponse();

            resp = await OpenStackGetServerAddresses(this.computeUrl, this.tenantId, this.Username, this.token.id, serverId);

            return resp;
        }

        public async Task<OpenStackServerAddressResponse> GetServerAddresses(string serverId, string computeUrl, string tenantId, string Username, string tokenID)
        {
            OpenStackServerAddressResponse resp = new OpenStackServerAddressResponse();

            resp = await OpenStackGetServerAddresses(computeUrl, tenantId, this.Username, tokenID, serverId);

            return resp;
        }

        #endregion Server Methods

        #endregion Public Methods

        #region Private Methods

        private async Task<OpenStackAuthResponse> OpenStackAuthenticate(string uri = null, string tenantName = null, string username = null, string password = null)
        {
            OpenStackAuthResponse authResponse = new OpenStackAuthResponse();

            using (var client = new HttpClient())
            {
                //create an object with the necessary TENANT, USERNAME, and PASSWORD parameters
                OpenStackAuthRequest myAuth = new OpenStackAuthRequest(tenantName, username, password);

                //initialize the HttpClient object with the auth endpoing URI
                client.BaseAddress = new Uri(uri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //POST the OpenStackAuthHeader to the tokens method of the auth endpoint
                HttpResponseMessage response = await client.PostAsJsonAsync("tokens", myAuth);

                if (response.IsSuccessStatusCode)
                {
                    //get the response from the OpenStack auth service
                    authResponse = response.Content.ReadAsAsync<OpenStackAuthResponse>().Result;
                }
            }
            return authResponse;
        }

        private async Task<OpenStackImageListResponse> OpenStackListImages(string uri = null, string tenantId = null, string username = null, string tokenId = null)
        {
            OpenStackImageListResponse images = new OpenStackImageListResponse();

            using (var clientImages = new HttpClient())
            {
                clientImages.BaseAddress = new Uri(uri);
                clientImages.DefaultRequestHeaders.Accept.Clear();
                clientImages.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //add token to header
                clientImages.DefaultRequestHeaders.Add("X-Auth-Token", tokenId);

                HttpResponseMessage response = await clientImages.GetAsync(tenantId + "/images");
                if (response.IsSuccessStatusCode)
                {
                    images = response.Content.ReadAsAsync<OpenStackImageListResponse>().Result;
                }
            }

            return images;
        }

        private async Task<OpenStackFlavorsListResponse> OpenStackListFlavors(string uri = null, string tenantId = null, string username = null, string tokenId = null)
        {
            OpenStackFlavorsListResponse flavors = new OpenStackFlavorsListResponse();

            using (var clientFlavors = new HttpClient())
            {
                clientFlavors.BaseAddress = new Uri(uri);
                clientFlavors.DefaultRequestHeaders.Accept.Clear();
                clientFlavors.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //add token to header
                clientFlavors.DefaultRequestHeaders.Add("X-Auth-Token", tokenId);

                HttpResponseMessage response = await clientFlavors.GetAsync(tenantId + "/flavors");
                if (response.IsSuccessStatusCode)
                {
                    flavors = response.Content.ReadAsAsync<OpenStackFlavorsListResponse>().Result;
                }
            }
            return flavors;
        }

        public async Task<OpenStackServerResponse> OpenStackCreateServer(string uri = null, string tenantId = null, string username = null, string tokenId = null, string serverName = null, string user_data = null, string imageRef = null, string flavorRef = null, string key_name = null)
        {
            OpenStackServerResponse ossr = new OpenStackServerResponse();

            using (var clientServer = new HttpClient())
            {
                clientServer.BaseAddress = new Uri(uri);
                clientServer.DefaultRequestHeaders.Accept.Clear();
                clientServer.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //add token to header
                clientServer.DefaultRequestHeaders.Add("X-Auth-Token", tokenId);

                //prepare server object
                OpenStackCreateServerRequest oscsr = new OpenStackCreateServerRequest(serverName, imageRef, flavorRef, Convert.ToBase64String(Encoding.Default.GetBytes(user_data)), key_name);
                HttpResponseMessage response = await clientServer.PostAsJsonAsync(tenantId + "/servers", oscsr);
                if (response.IsSuccessStatusCode)
                {
                    ossr = response.Content.ReadAsAsync<OpenStackServerResponse>().Result;
                }
            }
            return ossr;
        }

        private async Task<OpenStackServerResponse> OpenStackGetServer(string uri = null, string tenantId = null, string username = null, string tokenId = null, string serverId = null)
        {
            OpenStackServerResponse ossr = new OpenStackServerResponse();

            using (var clientServer = new HttpClient())
            {
                clientServer.BaseAddress = new Uri(uri);
                clientServer.DefaultRequestHeaders.Accept.Clear();
                clientServer.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //add token to header
                clientServer.DefaultRequestHeaders.Add("X-Auth-Token", tokenId);

                HttpResponseMessage response = await clientServer.GetAsync(tenantId + "/servers/" + serverId);
                if (response.IsSuccessStatusCode)
                {
                    ossr = response.Content.ReadAsAsync<OpenStackServerResponse>().Result;
                }
            }
            return ossr;
        }

        private async Task<OpenStackServerAddressResponse> OpenStackGetServerAddresses(string uri = null, string tenantId = null, string username = null, string tokenId = null, string serverId = null)
        {
            OpenStackServerAddressResponse ossar = new OpenStackServerAddressResponse();

            using (var clientServer = new HttpClient())
            {
                clientServer.BaseAddress = new Uri(uri);
                clientServer.DefaultRequestHeaders.Accept.Clear();
                clientServer.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //add token to header
                clientServer.DefaultRequestHeaders.Add("X-Auth-Token", tokenId);

                HttpResponseMessage response = await clientServer.GetAsync(tenantId + "/servers/" + serverId + "/ips");
                if (response.IsSuccessStatusCode)
                {
                    ossar = response.Content.ReadAsAsync<OpenStackServerAddressResponse>().Result;
                }
            }
            return ossar;
        }

        private async Task<OpenStackServerNetworkResponse> OpenStackGetServerNetwork(string uri = null, string tenantId = null, string username = null, string tokenId = null, string serverId = null, string networkLabel = null)
        {
            OpenStackServerNetworkResponse ossnr = new OpenStackServerNetworkResponse();

            using (var clientServer = new HttpClient())
            {
                clientServer.BaseAddress = new Uri(uri);
                clientServer.DefaultRequestHeaders.Accept.Clear();
                clientServer.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //add token to header
                clientServer.DefaultRequestHeaders.Add("X-Auth-Token", tokenId);

                HttpResponseMessage response = await clientServer.GetAsync(tenantId + "/servers/" + serverId + "/ips/" + networkLabel);
                if (response.IsSuccessStatusCode)
                {
                    ossnr = response.Content.ReadAsAsync<OpenStackServerNetworkResponse>().Result;
                }
            }
            return ossnr;
        }

        #endregion Private Methods
    }
}