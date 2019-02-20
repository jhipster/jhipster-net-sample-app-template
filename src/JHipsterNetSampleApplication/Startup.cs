using System;
using JHipsterNetSampleApplication.Data;
using JHipsterNetSampleApplication.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: ApiController]

namespace JHipsterNetSampleApplication {
    public sealed class Startup {
        private IHostingEnvironment Environment { get; }

        private IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddNhipsterModule(Configuration)
                .AddDatabaseModule(Configuration)
                .AddSecurityModule()
                .AddProblemDetailsModule(Environment)
                .AddAutoMapperModule()
                .AddWebModule()
                .AddSwaggerModule();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider,
            ApplicationDatabaseContext context)
        {
            app
                .UseApplicationSecurity(env)
                .UseApplicationProblemDetails()
                .UseApplicationWeb(env)
                .UseApplicationSwagger()
                .UseApplicationDatabase(serviceProvider, env)
                .UseApplicationIdentity(serviceProvider);
        }
    }
}
