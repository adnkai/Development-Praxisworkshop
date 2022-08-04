
var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;


// Add services to the container.
builder.Services.AddRazorPages();

string[] initialScopes = Configuration.GetValue<string>("DownstreamApi:Scopes").Split(' ');
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(Configuration.GetSection("Authentication")) // Fetch Auth Data from appsettings.json
    .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
    .AddMicrosoftGraph(Configuration.GetSection("DownstreamApi"))
    .AddInMemoryTokenCaches();

builder.Services.AddAuthorization(options => {
    options.AddPolicy("ClaimsTest", policy => policy.RequireClaim("Contacts.Read"));
    options.AddPolicy("MustHaveOneDrive", policy => policy.RequireClaim("Files.ReadWrite"));
    //options.AddPolicy("Roles", policy => policy.RequireClaim("Roles"));
});
        
        // Enable Authentication globally
builder.Services.AddControllersWithViews(options =>
    {
        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
        options.Filters.Add(new AuthorizeFilter(policy));
    });
// Add MicrosoftIdentity middleware to basically any page
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

// Add the UI support to handle claims challenges
builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();

builder.Services.AddSingleton<IUserDataSingleton, UserDataSingleton>();

// Include Application Insights with config from appsettings.json
// https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core#using-applicationinsightsserviceoptions
builder.Services.AddApplicationInsightsTelemetry(Configuration.GetSection("ApplicationInsights").GetValue<string>("InstrumentationKey"));
// Configure SignOut redirect (doesn't work though...)
builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Events.OnRemoteSignOut = async context =>
    {
        await Task.Run(() => {
            context.Response.Redirect("/");
        });
    };
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Use Authentication
app.UseAuthorization();

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});

app.Run();