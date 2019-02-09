using System;
using JHipsterNet.Config;
using JHipsterNetSampleApplication.Data;
using JHipsterNetSampleApplication.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

[assembly: ApiController]

namespace JHipsterNetSampleApplication {
    public sealed class Startup {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddNhipsterModule(Configuration)
                .AddDatabaseModule(Configuration)
                .AddSecurityModule()
                .AddProblemDetailsModule()
                .AddAutoMapperModule()
                .AddWebModule()
                .AddSwaggerModule();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider,
            ApplicationDatabaseContext context, IOptions<JHipsterSettings> jhipsterSettingsOptions)
        {
            var jhipsterSettings = jhipsterSettingsOptions.Value;
            app
                .UseApplicationSecurity(jhipsterSettings)
                .UseApplicationProblemDetails()
                .UseApplicationWeb(env)
                .UseApplicationSwagger()
                .UseApplicationDatabase(serviceProvider, env)
                .UseApplicationIdentity(serviceProvider);
        }
    }
}
