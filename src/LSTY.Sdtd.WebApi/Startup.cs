﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSwag;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using LSTY.Sdtd.WebApi.Resources;
using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Resources;
using System.ComponentModel.DataAnnotations;
using IceCoffee.AspNetCore.Resources;
using System.Text.Json.Serialization;

[assembly: ApiController]
namespace LSTY.Sdtd.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            WebHostEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment WebHostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    // 不需要的理应不要
                    options.SuppressConsumesConstraintForFormFileParameters = true;
                    // options.SuppressInferBindingSourcesForParameters = true;
                    // options.SuppressModelStateInvalidFilter = true;
                    // options.SuppressMapClientErrors = true;
                }).AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        return factory.Create(typeof(DataAnnotationsResource));
                    };
                });

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
                options.DefaultRequestCulture = new RequestCulture("en");

                // You must explicitly state which cultures your application supports.
                // These are the cultures the app supports for formatting numbers, dates, etc.
                options.SupportedCultures = supportedCultures;

                // These are the cultures the app supports for UI strings, i.e. we have localized resources for.
                options.SupportedUICultures = supportedCultures;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (WebHostEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Write streamlined request completion events, instead of the more verbose ones from the framework.
                // To use the default framework request logging instead, remove this line and set the "Microsoft"
                // level in appsettings.json to "Information".
                app.UseSerilogRequestLogging();

                // Register the Swagger generator and the Swagger UI middlewares
                app.UseOpenApi();
                app.UseSwaggerUi3();
            }
            else
            {
                app.UseSerilogRequestLogging();

                app.UseForwardedHeaders();
            }

            app.UseRequestLocalization();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}