using System.Configuration;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Configuration;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.Text;

[assembly: WebActivator.PreApplicationStartMethod(typeof(CRM.API.Host.App_Start.AppHost), "Start")]


/**
 * Entire ServiceStack Starter Template configured with a 'Hello' Web Service and a 'Todo' Rest Service.
 *
 * Auto-Generated Metadata API page at: /metadata
 * See other complete web service examples at: https://github.com/ServiceStack/ServiceStack.Examples
 */

namespace CRM.API.Host.App_Start
{
	public class AppHost : AppHostBase
	{		
		public AppHost() //Tell ServiceStack the name and where to find your web services
			: base("StarterTemplate ASP.NET Host", typeof(HelloService).Assembly) { }

		public override void Configure(Funq.Container container)
		{
			//Set JSON web services to return idiomatic JSON camelCase properties
            //ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;
            JsConfig.EmitCamelCaseNames = false;
            JsConfig.IncludeNullValues = true;
		
			//Configure User Defined REST Paths
			Routes
			  .Add<Hello>("/hello")
			  .Add<Hello>("/hello/{Name*}");

            const Feature disableFeatures = Feature.Soap | Feature.Csv | Feature.Xml | Feature.Jsv;

            var hostConfig = new HostConfig()
            {
                EnableFeatures = Feature.All.Remove(disableFeatures),
                DefaultContentType = MimeTypes.Json,
                AllowJsonpRequests = true

            };

            SetConfig(hostConfig);

            JsConfig.DateHandler = DateHandler.ISO8601;

            //ServiceExceptionHandlers = new List<HandleServiceExceptionDelegate>
            //{
            //    (HttpRequest, request, exception) =>
            //    {
            //        string dtoJson =string.Empty;
            //        if(request!=null)
            //        {
            //            dtoJson = request.ToJson();
            //        }
            //        //MKCLocalLogger.Error(dtoJson, "WeChatPayment.API.Host", exception);
            //        return new OutServiceErrorResponse

            //            {
            //                response_code = (int)ResponseCode.ApiSystemError,
            //                response_msg = exception.Message,
            //                response_body =null
            //            };
            //    }
            //};
			//Uncomment to change the default ServiceStack configuration
            //SetConfig(new HostConfig {
            //});

			//Enable Authentication
			//ConfigureAuth(container);

			//Register all your dependencies
			container.Register(new TodoRepository());			
		}

		/* Example ServiceStack Authentication and CustomUserSession */
		private void ConfigureAuth(Funq.Container container)
		{
			var appSettings = new AppSettings();

			//Default route: /auth/{provider}
			Plugins.Add(new AuthFeature(() => new CustomUserSession(),
				new IAuthProvider[] {
					new CredentialsAuthProvider(appSettings), 
					new FacebookAuthProvider(appSettings), 
					new TwitterAuthProvider(appSettings), 
					new BasicAuthProvider(appSettings), 
				})); 

			//Default route: /register
			Plugins.Add(new RegistrationFeature()); 

			//Requires ConnectionString configured in Web.Config
			var connectionString = ConfigurationManager.ConnectionStrings["AppDb"].ConnectionString;
			container.Register<IDbConnectionFactory>(c =>
				new OrmLiteConnectionFactory(connectionString, SqlServerDialect.Provider));

			container.Register<IUserAuthRepository>(c =>
				new OrmLiteAuthRepository(c.Resolve<IDbConnectionFactory>()));

            container.Resolve<IUserAuthRepository>().InitSchema();
		}

		public static void Start()
		{
			new AppHost().Init();
		}
	}
}
