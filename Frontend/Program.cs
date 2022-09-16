var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
ConfigurationManager Configuration = builder.Configuration;
IConfigurationRefresher _refresher = null;

#region App Configuration
// App Configuration with managed Identity
builder.Host.ConfigureAppConfiguration(builder => {
    builder.AddAzureAppConfiguration(options =>
    {
        // Add Environment Variables
        // options.Connect(new Uri(Configuration.GetValue<string>("AppConfig:Uri")), 
        //     new DefaultAzureCredential())
        options.Connect(Configuration.GetValue<String>("AppConfig:ConnectionString"))
        .UseFeatureFlags(options => {
            options.CacheExpirationInterval = TimeSpan.FromSeconds(Configuration.GetValue<int>("AppConfig:FeatureCacheExpirationInSeconds"));
            options.Select(KeyFilter.Any, LabelFilter.Null);
        })
        .Select(KeyFilter.Any, "Production") // Any Key, any label DO NOT PUT LABELFILTER.NULL if you labeled your keys. THIS TOOK ME HOURS TO FIGURE OUT!
        .ConfigureRefresh(refresh => { // Configure sentinel refresh
            refresh.Register("Sentinel:RefreshKey", "Production", refreshAll: true)
                .SetCacheExpiration(TimeSpan.FromSeconds(Configuration.GetValue<int>("AppConfig:SentinelRefreshTimeInSeconds")));
        })
        .ConfigureKeyVault(kv =>
            {
                kv.SetCredential(
                    new DefaultAzureCredential());
            });
        _refresher = options.GetRefresher();
    }).AddEnvironmentVariables();
});
builder.Services.AddSingleton<IConfigurationRefresher>(_refresher);

builder.Services.AddAzureAppConfiguration();

#endregion

#region Authentication / Authorization
    string[] initialScopes = Configuration.GetValue<string>("DownstreamApi:Scopes").Split(' ');

    // Authentication
    builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(Configuration.GetSection("Authentication")) // Fetch Auth Data from appsettings.json
        .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
        .AddMicrosoftGraph(Configuration.GetSection("DownstreamApi"))
        // .AddSessionTokenCaches();
        .AddDistributedTokenCaches();
        // .AddInMemoryTokenCaches();

    // Authorization
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
#endregion

#region Redis caching
    // Distributed Token Cache
    builder.Services.Configure<MsalDistributedTokenCacheAdapterOptions>(options => 
    {
        options.DisableL1Cache = false;
        // Or limit the memory (by default, this is 500 MB)
        // options.L1CacheOptions.SizeLimit = 1024 * 1024 * 1024; // 1 GB

        // You can choose if you encrypt or not encrypt the cache
        options.Encrypt = false;

        // And you can set eviction policies for the distributed cache.
        options.SlidingExpiration = TimeSpan.FromHours(1);
    });


    // REDIS
    builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = Configuration.GetValue<string>("Redis:ConnectionString");
            // options.InstanceName = Configuration.GetValue<string>("Redis:Instance");
        });

    /* 
        Reconnect Redis on error.
        Not implemented (yet?)
    */ 
    builder.Services.Configure<MsalDistributedTokenCacheAdapterOptions>(options => 
    {
        options.OnL2CacheFailure = (ex) =>
        {
            if (ex is StackExchange.Redis.RedisConnectionException)
            {
                return true; //try to do the cache operation again
            }
            return false;
        };
    });
#endregion

// Adding a singleton (example)
builder.Services.AddSingleton<IDataSingleton, DataSingleton>();

builder.Services.AddHttpContextAccessor();

// Adding signalR for Event-Handling on /EventsUpdates
builder.Services.AddSignalR();

#region Application Insights
    /* 
        Include Application Insights with config from appsettings.json
        https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core#using-applicationinsightsserviceoptions
        Must be ConnectionString now as InstrumentaionKey is deprecated.
    */ 
    builder.Services.AddApplicationInsightsTelemetry(options => {
        options.ConnectionString = Configuration.GetSection("ApplicationInsights").GetValue<string>("ConnectionString");
    });
#endregion

// Adding feature management to toggle functionality on and off
builder.Services.AddFeatureManagement();

#region OverWriteSign-Out
    // Configure SignOut redirect
    builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.Events.OnRemoteSignOut = async context =>
        {
            await Task.Run(() => {
                context.Response.Redirect("/");
            });
        };
    });
#endregion

#region Build and run
    var app = builder.Build();
    app.UseAzureAppConfiguration();

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
        endpoints.MapHub<GridEventsHub>("/hubs/gridevents");
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        endpoints.MapRazorPages();
    });

    app.Run();
#endregion