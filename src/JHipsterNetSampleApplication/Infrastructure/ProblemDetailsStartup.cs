using Hellang.Middleware.ProblemDetails;
using JHipsterNetSampleApplication.Web.Rest.Problems;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace JHipsterNetSampleApplication.Infrastructure {
    public static class ProblemDetailsStartup {
        //TODO Understand difference between UI and non-ui Exceptions
        //https://github.com/christianacca/ProblemDetailsDemo/blob/master/src/ProblemDetailsDemo.Api/Startup.cs

        public static IServiceCollection AddProblemDetailsModule(this IServiceCollection @this, IHostingEnvironment env)
        {
            @this.AddProblemDetails(options => {
                // This is the default behavior; only include exception details in a development environment.
                options.IncludeExceptionDetails = ctx => env.IsDevelopment();

                // This will map NotImplementedException to the 501 Not Implemented status code.
                options.Map<NotImplementedException>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status501NotImplemented));

                // This will map HttpRequestException to the 503 Service Unavailable status code.
                options.Map<HttpRequestException>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status503ServiceUnavailable));

                // Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
                // If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
                options.Map<Exception>(ex => new ExceptionProblemDetails(ex, StatusCodes.Status500InternalServerError));
            });

            @this.AddProblemDetails();

            return @this;
        }

        public static IApplicationBuilder UseApplicationProblemDetails(this IApplicationBuilder @this)
        {
            @this.UseProblemDetails();
            return @this;
        }
    }
}
