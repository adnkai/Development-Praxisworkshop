﻿namespace Development_Praxisworkshop.Pages;

//[AllowAnonymous]
[Authorize(Roles = ("d32ce6ad-d1c1-48e9-ad3d-a30798dcdc16"))] // Diginea
public class PrivacyModel : PageModel
{
    private readonly ILogger<PrivacyModel> _logger;

    public PrivacyModel(ILogger<PrivacyModel> logger)
    {
        _logger = logger;
    }
    
    public void OnGet(){}
}