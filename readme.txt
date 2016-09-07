This package was developed to access OpenStack servers.  Basic functionality includes Authorization (returns a token used for subsequent calls), List server status 
(including information such as IP address), and create server which creates a VM from an image.

Example to authenticate:

        OpenStackClient.OpenStackClient osc = new OpenStackClient.OpenStackClient();
        osc.Username = "OpenStackUserName";
        osc.Password = "OpenStackPassword";
        osc.TenantName = "TenantName";
        osc.IdentityUrl = identityURI;
		OpenStackAuthResponse osresponse = await osc.Authenticate("nova");

The response object (several different response objects, consult the class diagram) can be inspected when the asynchronous  call is completed

i.e.  string accesstoken = osresponse.access.token.id;