using Funq;
using ExampleOpenAiPlugin.ServiceInterface;
using ServiceStack.Api.OpenApi;
using ServiceStack.DataAnnotations;
using ServiceStack.NativeTypes;

[assembly: HostingStartup(typeof(ExampleOpenAiPlugin.AppHost))]

namespace ExampleOpenAiPlugin;

public class AppHost : AppHostBase, IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services => {
            // Configure ASP.NET Core IOC Dependencies
        });

    public AppHost() : base("ExampleOpenAiPlugin", typeof(MyServices).Assembly) {}

    public override void Configure(Container container)
    {
        SetConfig(new HostConfig {
        });
        
        Plugins.Add(new CorsFeature(new[] {
            "http://localhost:5173", //vite dev
        }, allowCredentials:true));

        Plugins.Add(new OpenApiFeature());
        var openAiFeature = new OpenAiFeature
        {
            OpenAiManifest = new OpenAiManifest
            {
                SchemaVersion = "v1",
                NameForHuman = "TODO Plugin",
                NameForModel = "todo",
                DescriptionForHuman = "Plugin for managing a TODO list. You can add, remove and view your TODOs.",
                DescriptionForModel = "Plugin for managing a TODO list. You can add, remove and view your TODOs.",
                Auth = new OpenAiManifestAuth
                {
                    Type = "none"
                },
                ContactEmail = "darren@reidmail.org",
                LegalInfoUrl = "https://www.example.com/legal"
            }
        };
        if (HostingEnvironment.IsProduction())
        {
            openAiFeature.OpenAiManifest.Api = new OpenAiManifestApi
            {
                Type = "openai",
                Url = "https://todo-openai.reidodon.net/openapi.json",
                IsUserAuthenticated = false
            };
        }
        Plugins.Add(openAiFeature);
        
    }
}
