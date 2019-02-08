using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using JHipsterNet.Config;
using JHipsterNetSampleApplication.Data;
using JHipsterNetSampleApplication.Models;
using JHipsterNetSampleApplication.Security;
using JHipsterNetSampleApplication.Security.Jwt;
using JHipsterNetSampleApplication.Service;
using JHipsterNetSampleApplication.Service.Mapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using AuthenticationService = JHipsterNetSampleApplication.Service.AuthenticationService;
using IAuthenticationService = JHipsterNetSampleApplication.Service.IAuthenticationService;

namespace JHipsterNetSampleApplication.Infrastructure {
    public static class SecurityConfiguration {
        public static IServiceCollection AddSecurityModule(this IServiceCollection @this)
        {
            @this.AddIdentity<User, Role>(options => { options.SignIn.RequireConfirmedEmail = true; })
                .AddEntityFrameworkStores<ApplicationDatabaseContext>()
                .AddUserStore<UserStore<User, Role, ApplicationDatabaseContext, string, IdentityUserClaim<string>,
                    UserRole, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>>()
                .AddRoleStore<RoleStore<Role, ApplicationDatabaseContext, string, UserRole, IdentityRoleClaim<string>>
                >()
                .AddDefaultTokenProviders();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            @this
                .AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg => {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                "my-secret-key-which-should-be-changed-in-production-and-be-base64-encoded")),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });

            @this.AddScoped<IAuthenticationService, AuthenticationService>();
            @this.AddScoped<ITokenProvider, TokenProvider>();
            @this.AddScoped<IUserService, UserService>();
            @this.AddScoped<UserMapper>();
            @this.AddScoped<IPasswordHasher<User>, BCryptPasswordHasher>();
            @this.AddScoped<JHipsterSettings, JHipsterSettings>();
            @this.AddScoped<IClaimsTransformation, RoleClaimsTransformation>();
            @this.AddScoped<IPasswordHasher<User>, BCryptPasswordHasher>();
            @this.AddSingleton<IMailService, MailService>();

            return @this;
        }

        public static IApplicationBuilder UseApplicationSecurity(this IApplicationBuilder @this,
            IHostingEnvironment env)
        {
            @this.UseAuthentication();
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            @this.UseHsts();
            @this.UseHttpsRedirection();
            return @this;
        }
    }
}
