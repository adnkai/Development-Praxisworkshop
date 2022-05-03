using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.ApplicationInsights;

namespace Development_Praxisworkshop.Pages
{
    [AllowAnonymous]
    public class _LayoutModel : PageModel
    {
        private readonly ILogger<_LayoutModel> _logger;
        private readonly TelemetryClient _telemetryClient;
        

        public _LayoutModel(ILogger<_LayoutModel> logger, TelemetryClient telemetryClient)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
        }
        public void OnGet()
        {
            _telemetryClient.TrackPageView("Layout");
        }
    }
}
