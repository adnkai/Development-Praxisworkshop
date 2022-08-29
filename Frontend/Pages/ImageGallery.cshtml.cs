namespace Development_Praxisworkshop.Pages;

//[AllowAnonymous]
[Authorize(Roles = ("3b166cfb-21ff-4ba7-9460-819d066d9b94"))] // Win10E3License
public class GalleryModel : PageModel
{
    #region Private
        private readonly ILogger<PrivacyModel> _logger;
        private readonly IConfiguration _config;
    #endregion

    #region Public
        public List<String> images;
    #endregion

    public GalleryModel(ILogger<PrivacyModel> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
        images = new List<String>();
    }
    public void OnGet()
    {
        StorageAccountHelper todo = new StorageAccountHelper(_config);
        images = todo.GetImages();
    }
}