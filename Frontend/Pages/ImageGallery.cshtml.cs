using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

using Development_Praxisworkshop.Helper;

namespace Development_Praxisworkshop.Pages
{
    //[AllowAnonymous]
    [Authorize(Roles = ("3b166cfb-21ff-4ba7-9460-819d066d9b94"))] // Win10E3License
    public class GalleryModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly IConfiguration _config;
        public List<String> images;
        public GalleryModel(ILogger<PrivacyModel> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        public void OnGet()
        {
            StorageAccountHelper todo = new StorageAccountHelper(_config);
            images = todo.GetImages();
        }
    }
}
