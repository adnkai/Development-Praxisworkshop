namespace Development_Praxisworkshop;
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        string[] initialScopes = Configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' ');
        services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(Configuration.GetSection("Authentication")) // Fetch Auth Data from appsettings.json
            .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
            .AddMicrosoftGraph(Configuration.GetSection("DownstreamApi"))
            .AddInMemoryTokenCaches();

        // Role Based Claims
        services.AddAuthorization(options => {
            options.AddPolicy("ClaimsTest", policy => policy.RequireClaim("Contacts.Read"));
            options.AddPolicy("MustHaveOneDrive", policy => policy.RequireClaim("Files.ReadWrite"));
            //options.AddPolicy("Roles", policy => policy.RequireClaim("Roles"));
        });
        
        // Enable Authentication globally
        services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
        // Add MicrosoftIdentity middleware to basically any page
        services.AddRazorPages()
            .AddMicrosoftIdentityUI();
        
        // Add the UI support to handle claims challenges
        services.AddServerSideBlazor()
            .AddMicrosoftIdentityConsentHandler();

        // Include Application Insights with config from appsettings.json
        // https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core#using-applicationinsightsserviceoptions
        services.AddApplicationInsightsTelemetry(Configuration.GetSection("ApplicationInsights").GetValue<string>("InstrumentationKey"));
        // Configure SignOut redirect (doesn't work though...)
        services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Events.OnRemoteSignOut = async context =>
            {
                context.Response.Redirect("/");
            };
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
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

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
        });
    }
}