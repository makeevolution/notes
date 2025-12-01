ConfigureWebHostDefaults(webBuilder =>
                   {
                       webBuilder.UseWebRoot(Path.Combine(exePath, "html"));
                       webBuilder.UseStartup<Startup>();
                       webBuilder.ConfigureKestrel((context, options) =>
                       {
                           var applicationOptions = context.Configuration.GetSection(BasicApplicationOptions.SectionName).Get<ViewerApplicationOptions>();
                           options.ListenAnyIP(applicationOptions.EndPoints);
                       });
                   })
public class Startup : IWebServiceStartup

## Before Host.Build()

ConfigureWebHostDefaults
AddHttpContextAccessor
AddApplicationPart
ExtendedEmbeddedFileProvider
RegisterAuthorizationService
        var authOptions = configuration.GetSection(AuthenticationOptions.SectionName).Get<AuthenticationOptions>();

        services.AddDataProtection()
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo($@"{applicationOptions.DataFolder}\{authOptions.KeyPath}"))
                .SetApplicationName(authOptions.KeyApplication);

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = c => true;
            options.Secure = CookieSecurePolicy.SameAsRequest;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddScoped<RemoteCookieAuthenticationEvents>();
        services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddCookie(IdentityConstants.ApplicationScheme, options =>
                {
                    options.SetCookieOptions(authOptions, true);
                    options.EventsType = typeof(RemoteCookieAuthenticationEvents);
                });
        
        services.AddAuthorization(options => AuthenticationExtensions.AddDefaultPolicy(options, IdentityConstants.ApplicationScheme));

        services.AddSingleton<ITicketStore, HttpClientTicketStore>();
        services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, ConfigureCookieAuthenticationOptions>();
        services.AddScoped<ClientCertificateAuthenticationEvents>();
        services.AddScoped<ClientCertificateAuthorizationFilter>();

        if (applicationOptions.EndPoints.BackEnd?.ClientCertificate != null &&
            applicationOptions.EndPoints.BackEnd.ClientCertificate.IsConfigured)
        {
            services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
                    .AddCertificate(options =>
                    {
                        options.AllowedCertificateTypes = CertificateTypes.All;
                        options.EventsType = typeof(ClientCertificateAuthenticationEvents);
                    });
        }

        services.AddCertificateAuthPolicy(applicationOptions.EndPoints.BackEnd?.ClientCertificate);

services.AddHealthChecks()
                    .AddCheck<HealthCheck>("ready");

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | 
                                       ForwardedHeaders.XForwardedProto;
            // Only loopback proxies are allowed by default.
            // Clear that restriction because forwarders are enabled by explicit 
            // configuration.
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });
    }

## After host.build()

\\
