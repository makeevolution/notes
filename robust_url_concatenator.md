[TestFixture]
public class MedidataUtilsTests
{
    private static object[] BuildUriTestCases =
    {
        new object[]
        {
            "https://example.com/",
            new[] { "/api", "v1/", "/users/" },
            null,
            "https://example.com/api/v1/users"
        },
        new object[]
        {
            "https://example.com/",
            Array.Empty<string>(),
            null,
            "https://example.com/"
        },
        new object[]
        {
            "https://example.com/",
            new[] { "/api/", "/products/" },
            new[] { ("category", "electronics"), ("sort", "price") },
            "https://example.com/api/products?category=electronics&sort=price"
        },
        new object[]
        {
            "https://example.com/",
            new[] { "/api/" },
            new[] { ("key with spaces", "value/with/reserved&chars") },
            "https://example.com/api?key%20with%20spaces=value%2Fwith%2Freserved%26chars"
        },
        new object[]
        {
            "https://example.com",
            new[] { "path1", "subpath2" },
            null,
            "https://example.com/path1/subpath2"
        },
        new object[]
        {
            "https://example.com",
            new[] { "api/" },
            null,
            "https://example.com/api"
        },
        new object[]
        {
            "https://example.com/",
            new[] { "/path1/", "/subpath2/", "final/" },
            null,
            "https://example.com/path1/subpath2/final"
        },
        new object[]
        {
            "https://example.com/",
            new[] { "api", "test" },
            new[] { ("key1", ""), ("key2", "value2") },
            "https://example.com/api/test?key1=&key2=value2"
        }
    };

    /// <summary>
    /// Test that the BuildUri method always generates a correct URL regardless of missing trailing or ending forward slashes in paths.
    /// This ensures that in appsettings, we can set urls without having to worry about whether to put ending/trailing slashes.
    /// </summary>
    [Test, TestCaseSource(nameof(BuildUriTestCases))]
    public void BuildUri_ShouldGenerateExpectedUrl(
        string baseUrl,
        string[] paths,
        (string, string)[] queryParameters,
        string expected)
    {
        // ACT
        Uri result = MedidataUtils.BuildUri(baseUrl, paths, queryParameters);

        // ASSERT
        Assert.AreEqual(expected, result.AbsoluteUri);
    }
}



    public static Uri BuildUri(string baseUrl, string[] paths, params (string key, string value)[] queryParameters)
    {
        var fullPath = string.Join("/", paths.Select(x => x.Trim('/')));
        var fullUrl = $"{baseUrl.TrimEnd('/')}/{fullPath}";

        var uriBuilder = new UriBuilder(fullUrl);

        if (queryParameters?.Length > 0)
        {
            uriBuilder.Query = string.Join("&", queryParameters.Select(p => $"{Uri.EscapeDataString(p.key)}={Uri.EscapeDataString(p.value)}"));
        }

        return uriBuilder.Uri;
    }

/// <summary>
/// Factory for creating <see cref="MedidataIntegrationDbContext"/> instances at design time.
/// Required by EF Core CLI tools (e.g., dotnet ef migrations add) because:
/// - The application uses <c>AddPooledDbContextFactory</c> which only registers IDbContextFactory, not the DbContext itself
/// - EF tools cannot resolve DbContextOptions when the DbContext isn't directly registered in DI
/// - The application's Program.Main has conditional logic that prevents EF tools from accessing the IHost
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MedidataIntegrationDbContext>
{
    public MedidataIntegrationDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        const string configFile = "appsettings.json";
        var configPath = Path.Combine(basePath, configFile);

        var configuration = new ConfigurationBuilder()
                            .SetBasePath(basePath)
                            .AddJsonFile(configFile)
                            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Console.WriteLine($"[DesignTimeDbContextFactory] Config source: {configPath}");
        Console.WriteLine($"[DesignTimeDbContextFactory] Connection string: {DbContextExtensions.MaskConnectionString(connectionString)}");
        Console.WriteLine("[Important] This connectionstring should connect to an SQL Server database.");
        
        //  EF Core's design-time tools (like migrations add) work by partially building your host to extract the IServiceProvider and resolve the DbContext. Once it has what it needs, it intentionally throws HostAbortedException to abort the host before it fully starts.
        Console.WriteLine("\n[Important] Please note that a HostAbortedException is expected when adding migrations to hosted services like the medidata integration.\n");

        var optionsBuilder = new DbContextOptionsBuilder<MedidataIntegrationDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new MedidataIntegrationDbContext(optionsBuilder.Options);
    }
}


