using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace JHipsterNetSampleApplication.Infrastructure {
    public static class AutoMapperStartup {
        public static IServiceCollection AddAutoMapperModule(this IServiceCollection @this)
        {
            @this.AddAutoMapper();
            return @this;
        }
    }
}
