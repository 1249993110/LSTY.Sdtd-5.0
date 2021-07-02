using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using Serilog;
using System;
using System.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using IceCoffee.AspNetCore.Resources;
using IceCoffee.AspNetCore.Models;
using IceCoffee.AspNetCore.Middlewares;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IceCoffee.AspNetCore.Extensions;
using LSTY.Sdtd.WebApi.Data;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IceCoffee.DbCore;
using IceCoffee.AspNetCore.Permission;
using LSTY.Sdtd.WebApi.Permission;
using IceCoffee.AspNetCore.Models.Results;
using Dapper;
using LSTY.Sdtd.WebApi.Data.Primitives;
using Hei.Captcha;

[assembly: ApiController]
namespace LSTY.Sdtd.WebApi
{
    public class Startup
    {
        private const string _corsPolicyName = "CorsPolicy_AllowSpecificOrigins";
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            WebHostEnvironment = env;

            // 将数据库连接信息添加到全局缓存池
            ConnectionInfoManager.AddDbConnectionInfoToCache(configuration, DatabaseType.SQLServer, "DefaultConnection");
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment WebHostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        string messages = string.Join(Environment.NewLine, 
                            context.ModelState.Values.SelectMany(s => s.Errors).Select(s => s.ErrorMessage));

                        var result = new RespResultBase()
                        {
                            Code = CustomStatusCode.Failed,
                            Title = "One or more model validation errors occurred",
                            Message = messages
                        };

                        return (result as IConvertToActionResult).Convert();
                    };
                    // 不需要的理应不要
                    options.SuppressConsumesConstraintForFormFileParameters = true;
                    // options.SuppressInferBindingSourcesForParameters = true;
                    // options.SuppressModelStateInvalidFilter = true;
                    // options.SuppressMapClientErrors = true;
                })
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        return factory.Create(typeof(DataAnnotationsResource));
                    };
                });

            services.AddJwtAuthentication(Configuration.GetSection(nameof(JwtOptions)));
            services.AddJwtAuthorization();

            if (WebHostEnvironment.IsDevelopment())
            {
                // Register the Swagger services
                services.AddOpenApiDocument(config =>
                {
                    config.GenerateEnumMappingDescription = true;
                    config.PostProcess = document =>
                    {
                        document.Info.Version = "v1";
                        document.Info.Title = "LSTY 7daytodie API";
                        document.Info.Description = "A simple ASP.NET Core web API";
                        //document.Info.TermsOfService = "None";
                        document.Info.Contact = new OpenApiContact
                        {
                            Name = "IceCoffee",
                            Email = "1249993110@qq.com",
                            Url = "https://github.com/1249993110"
                        };
                        //document.Info.License = new OpenApiLicense
                        //{
                        //    Name = "Use under LICX",
                        //    Url = "https://github.com/1249993110"
                        //};
                    };

                    // 可以设置从注释文件加载，但是加载的内容可被 OpenApiTagAttribute 特性覆盖
                    config.UseControllerSummaryAsTagDescription = true;
                });
            }
            else
            {
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.All;

                    // 在 ForwardedHeadersMiddleware 中间件代码第268行 CheckKnownAddress 方法
                    // 会检查访问的IP是否在 ForwardedHeadersOptions.KnownProxies 或者 ForwardedHeadersOptions.KnownNetworks 之中
                    // 通过清空 KnownNetworks 和 KnownProxies 的默认值来不执行严格匹配，这样做有可能受到 IP欺骗 攻击
                    options.KnownNetworks.Clear();
                    options.KnownProxies.Clear();
                });
            }

            services.AddLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new CultureInfo[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("zh")
                };

                // State what the default culture for your application is. This will be used if no specific culture
                // can be determined for a given request.
                options.DefaultRequestCulture = new RequestCulture("zh");

                // You must explicitly state which cultures your application supports.
                // These are the cultures the app supports for formatting numbers, dates, etc.
                options.SupportedCultures = supportedCultures;

                // These are the cultures the app supports for UI strings, i.e. we have localized resources for.
                options.SupportedUICultures = supportedCultures;
            });

            SqlMapper.AddTypeHandler(new StringGuidHandler());

            foreach (var type in typeof(ConnectionInfoManager).Assembly.GetExportedTypes())
            {
                if (type.FullName.StartsWith("LSTY.Sdtd.WebApi.Data.Repositories"))
                {
                    var interfaceType = type.GetInterfaces().First(p => p.FullName.StartsWith("LSTY.Sdtd.WebApi.Data.IRepositories"));
                    services.TryAddSingleton(interfaceType, type);
                }
            }

            services.AddSingleton<IPermissionValidator, PermissionValidator>();

            services.AddHeiCaptcha();

            services.AddMemoryCache();

            services.AddEmailService(Configuration.GetSection(nameof(SmtpOptions)));

            if (WebHostEnvironment.IsDevelopment())
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(_corsPolicyName, builder =>
                    {
                        builder.WithOrigins("http://localhost:8080");
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                        builder.AllowCredentials();
                    });
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            // Write streamlined request completion events, instead of the more verbose ones from the framework.
            // To use the default framework request logging instead, remove this line and set the "Microsoft"
            // level in appsettings.json to "Information".
            
            app.UseMiddleware<GlobalExceptionHandleMiddleware>();

            app.UseSerilogRequestLogging();

            if (WebHostEnvironment.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage();

                // Register the Swagger generator and the Swagger UI middlewares
                app.UseOpenApi();
                app.UseSwaggerUi3();
            }
            else
            {
                app.UseForwardedHeaders();
            }

            app.UseRequestLocalization();

            app.UseRouting();

            // UseCors 必须在 UseAuthorization 之前在 UseRouting 之后调用
            if (WebHostEnvironment.IsDevelopment())
            {
                app.UseCors(_corsPolicyName);
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
