using System;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;

using System.Security.Claims;

using Development_Praxisworkshop.Helper;

namespace Development_Praxisworkshop.Pages
{
    //[Authorize(Policy="MustHaveOneDrive")] // need onedrive permissions
    [AuthorizeForScopes(Scopes = new[] { "files.readwrite" })]
    public class OneDriveFilesModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly IConfiguration _config;
        public List<String> files;
        readonly ITokenAcquisition _tokenAcquisition;
        public OneDriveFilesModel(ILogger<PrivacyModel> logger, IConfiguration config, ITokenAcquisition tokenAcquisition)
        {
            _logger = logger;
            _config = config;
            _tokenAcquisition = tokenAcquisition;
        }

        public void OnGet()
        {
            ClaimsPrincipal principal = User as ClaimsPrincipal; 
            //TransformAsync(principal);
            files = new List<string>();
            //https://login.microsoftonline.com/common/reprocess
            //?ctx=rQQIARAApZK9TxNxGMd7Pai1iZZgog4OHYhR8Xe99941wVgslCtwFSi0vcXc_V7aq_eWu4OW7iaORAcIiRrd6GScDJMzEzoZ_wQTjXEwhEVLCH-B3-GT55vneZbv82RSPMMLDHuP5hm2OIWwYkHFkgBSiQpExeKAqZgIWAVRQWaBl1hZDCczEyda9_TPRkt78_ZBtv95qzykip04DqJiPo_wFnb8AIeMa8PQj3wSM9B389gDm1G-HZpB55wA9wPHD3H4kaKOKepFMrto2syqH3cemshjEN5Pjo-W1teGySlcUAgRiAR4kZOByEILqFgmQOIJLIiqaKoc_y2ZrZU24w5_Bj-0B_h3Mk1Cs-1iL96nHbxdDYxHmqy5K_1aZcMxKsuDpcaKqNdLsV5ub-vbnK3zK8JSo9ptddd7-uCppNfbYsvWIs3lHFSZs2teZJsNiTWa1U5LWA0sXlrHzVlH6_o2rKg91CzZpMnODOkp9lwCOOM54EV1oQ90ahSN63uHo3nOUlmOQCAiGQORQzKwILGARJBpsTILOVQ4ou_P2w6OmFVsIqbkOLlR0J6NckHok1Ej5xPi2B5-YkKIo-grTR2PUd_HrqeTEzduJnKJO7dZuphOZyYSZ-5kjHo3Pjrmy_Lf17cOviw9m6w2hIO7iaPxfJmHa4swtAKxI08bMFTL_FxtQFqyweeX1xYK80JVng9IlXTbM3yR20lRO6lrR6msG5kO040YK_R7EQ5_pajnlxKHl__jOfYy1DAzzRr1jbmmAvqx0N4SudiOdanRn2VVU9UUX3vck_XlkrVYN2bF9xnq05XEydWfP17tnu7u_Vj4Bw2
            //&sessionid=01b901fc-4d6e-41d6-bcfb-5fdab060c1d7
            //&sso_nonce=AwABAAEAAAACAOz_BAD0_7iUJPjS7I1m4rYHjtkiY4d5vNd1JIVEYvBQIzq928LEiZDNyp7U1nNUo5F2H4qdzDLHLiyyMlskTT4EUblty3UgAA
            //&client-request-id=fa6a49f8-56f6-4959-9fa1-3e0f78d17644
            //&mscrid=fa6a49f8-56f6-4959-9fa1-3e0f78d17644
            // Acquire the access token.
            string[] scopes = new string[]{"files.readwrite"};
            string accessToken = _tokenAcquisition.GetAccessTokenForUserAsync(scopes).Result;

            // Use the access token to call a protected web API.
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();
            var claimType = "Delegated";
            if (!principal.HasClaim(claim => claim.Type == claimType))
            {
                claimsIdentity.AddClaim(new Claim(claimType, "Files.ReadWrite"));
            }

            principal.AddIdentity(claimsIdentity);
            return Task.FromResult(principal);
        }
    }
}
