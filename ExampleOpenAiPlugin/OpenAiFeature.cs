using System.Runtime.Serialization;
using ServiceStack.Api.OpenApi;

namespace ExampleOpenAiPlugin;

public class OpenAiFeature : IPlugin, IPreInitPlugin
{
    public OpenAiManifest OpenAiManifest { get; set; } = new OpenAiManifest();
    
    public void Register(IAppHost appHost)
    {
        appHost.Routes.Add<GetOpenAiPluginManifest>("/.well-known/ai-plugin");
        appHost.RegisterService<OpenAiServices>();
    }

    public void BeforePluginsLoaded(IAppHost appHost)
    {
        var openApiSpecReq = new OpenApiSpecification();
        appHost.ConfigurePlugin<OpenApiFeature>(feature =>
        {
            this.OpenAiManifest.LogoUrl = feature.LogoUrl;
            this.OpenAiManifest.Api ??= new OpenAiManifestApi
            {
                Type = "openapi",
                Url = "https://localhost:5001/openapi.json",
                IsUserAuthenticated = false
            };
            if (feature.SecurityDefinitions != null && feature.SecurityDefinitions.ContainsKey("Bearer"))
            {
                this.OpenAiManifest.HttpAuthorizationType = OpenAiHttpAuthorizationType.Bearer;
                this.OpenAiManifest.Auth = new OpenAiManifestAuth
                {
                    Type = "bearer"
                };
            }
            
            if (feature.SecurityDefinitions != null && feature.SecurityDefinitions.ContainsKey("basic"))
            {
                this.OpenAiManifest.HttpAuthorizationType = OpenAiHttpAuthorizationType.Basic;
                this.OpenAiManifest.Auth = new OpenAiManifestAuth
                {
                    Type = "basic"
                };
            }
        });
    }
}

public class GetOpenAiPluginManifest
{
    
}

public class OpenAiServices : Service
{
    public object Get(GetOpenAiPluginManifest request)
    {
        var feature = this.AssertPlugin<OpenAiFeature>();
        return feature.OpenAiManifest;
    }
}

[DataContract]
public class OpenAiManifest
{
    [DataMember(Name = "schema_version")]
    public string SchemaVersion { get; set; }
    [DataMember(Name = "name_for_model")]
    public string NameForModel { get; set; }
    [DataMember(Name = "name_for_human")]
    public string NameForHuman { get; set; }
    
    [DataMember(Name = "description_for_model")]
    public string DescriptionForModel { get; set; }
    [DataMember(Name = "description_for_human")]
    public string DescriptionForHuman { get; set; }
    [DataMember]
    public OpenAiManifestAuth Auth { get; set; }
    [DataMember]
    public OpenAiManifestApi? Api { get; set; }
    
    [DataMember(Name = "logo_url")]
    public string LogoUrl { get; set; }
    [DataMember(Name = "contact_email")]
    public string ContactEmail { get; set; }
    [DataMember(Name = "legal_info_url")]
    public string LegalInfoUrl { get; set; }
    
    [DataMember(Name = "HttpAuthorizationType")]
    public OpenAiHttpAuthorizationType? HttpAuthorizationType { get; set; }
    
    [DataMember(Name = "ManifestAuthType")]
    public OpenAiManifestAuthType? ManifestAuthType { get; set; }
}

public enum OpenAiManifestAuthType
{
    None,
    UserHttp,
    ServiceHttp,
    Oauth
}

public enum OpenAiHttpAuthorizationType
{
    Bearer,
    Basic
}

public class OpenAiManifestAuth
{
    public string Type { get; set; }
}

public class OpenAiManifestApi
{
    public string Type { get; set; }
    public string Url { get; set; }
    public bool IsUserAuthenticated { get; set; }
}