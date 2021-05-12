using LSTY.Sdtd.PatronsMod.WebApi.Gzip;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Swagger.Services;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using Swagger.ObjectModel;
using Swagger.ObjectModel.Builders;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Response = Nancy.Response;

namespace LSTY.Sdtd.PatronsMod.WebApi
{
    public class CustomBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            pipelines.OnError += OnApplicationError;
            pipelines.BeforeRequest += OnBeforeRequest;

            SwaggerMetadataProvider.SetInfo(
                title: "LSTY.Sdtd.PatronsMod API Document",
                version: "v0",
                desc: "7daystodie lsty mod open api",
                contact: new Contact()
                {
                    EmailAddress = "1249993110@qq.com",
                    Name = "IceCoffee",
                    Url = "https://lsty.top"
                },
                termsOfService: "https://lsty.top/termsOfService");

            var securitySchemeBuilder = new ApiKeySecuritySchemeBuilder()
                .Name(WebConfig.AuthHeader)
                .IsInHeader()
                .Description("Authentication with apikey");
            // The scheme name only can be 'ApiKey', because "security":[{"ApiKey":[]}]} in '/api-docs'
            SwaggerMetadataProvider.AddSecuritySchemeBuilder(securitySchemeBuilder, "ApiKey");

            if (FunctionManager.CommonConfig.WebConfig.EnableGzip)
            {
                // Enable Compression with Default Settings
                pipelines.EnableGzipCompression();
            }

            // SwaggerTypeMapping.AddTypeMapping(typeof(DateTime), typeof(string));

            base.ApplicationStartup(container, pipelines);
        }

        private Response OnBeforeRequest(NancyContext context)
        {
            CustomLogger.Info("Web request from: " + context.Request.UserHostAddress + " path: " + context.Request.Path);

            string path = context.Request.Path;
            if (path.StartsWith("/api/"))
            {
                var header = context.Request.Headers[WebConfig.AuthHeader];

                if (header.Any() == false
                    || header.First() != FunctionManager.CommonConfig.WebConfig.AccessToken)
                {
                    return new Response()
                    {
                        StatusCode = HttpStatusCode.Unauthorized
                    };
                }
            }

            return null;
        }

        private dynamic OnApplicationError(NancyContext context, Exception ex)
        {
            CustomLogger.Error(ex, "Application Error in WebApi");

            ResponseResult responseResult = new ResponseResult()
            {
                Code = StatusCode.Failed,
                Data = null,
                Message = ex.Message,
                Title = "Something went horribly, horribly wrong while servicing your request, please check the log file",
            };

            return responseResult.ToResponse(HttpStatusCode.InternalServerError);
        }

        protected override IRootPathProvider RootPathProvider
        {
            get { return new CustomRootPathProvider(); }
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/itemicons",
               FunctionManager.CommonConfig.WebConfig.ItemIconsPath));

            nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/", 
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\wwwroot"));
           
            // Root path should put last
            //nancyConventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("/",
            //    FunctionManager.CommonConfig.WebConfig.ContentRootPath));
        }

        /// <summary>
        /// Initialise the request - can be used for adding pre/post hooks and
        /// any other per-request initialisation tasks that aren't specifically container setup related
        /// </summary>
        /// <param name="container">Container</param><param name="pipelines">Current pipelines</param><param name="context">Current context</param>
        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
        {
            pipelines.AfterRequest.AddItemToEndOfPipeline(_context =>
            {
                var headers = _context.Response.Headers;
                headers.Add("Access-Control-Allow-Origin", "*");
                headers.Add("Access-Control-Allow-Headers", "*");
                headers.Add("Access-Control-Allow-Methods", "*");
            });
        }
    }
}
