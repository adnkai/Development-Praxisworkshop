using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

using Microsoft.Identity.Web.UI;

using Development_Praxisworkshop.Helper;

namespace Development_Praxisworkshop.Pages
{
    //[AllowAnonymous]
    [Authorize]
    public class LogoutModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly IConfiguration _config;

        public LogoutModel(ILogger<PrivacyModel> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        public void OnGet()
        {
            if (1==1)
            {

            }
        }

    }
    
}
