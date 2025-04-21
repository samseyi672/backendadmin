using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Org.BouncyCastle.Utilities.Collections;
using Retailbanking.BL.ActionFilter;
using Retailbanking.BL.IServices;
using Retailbanking.BL.Services;
using Retailbanking.Common.CustomObj;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PrimeAppAdmin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IGeneric, GenericServices>();
            services.AddSingleton<IBeneficiary, BeneficiaryServices>();
            services.AddSingleton<IUserCacheService,UserCacheService>();
            // Register Redis storage service
            services.AddSingleton<IRedisStorageService, RedisStorageService>();
            services.AddSingleton<IRegistration, RegistrationServices>();
            services.AddSingleton<IPinService,PinService>();
            services.AddSingleton<IPinManagementService,PinManagementService>();
            // services.AddSingleton<IRedisStorageService, RedisStorageService>();
            services.AddSingleton<ISmsBLService, SmsBLService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<INotification, NotificationService>();
            services.AddSingleton<ILdapService, LdapService>();
            services.AddSingleton<IStaffUserService,StaffUserService>();
            services.AddSingleton<IMobileUserService,MobileUserService>();
            services.AddSingleton<IMobileInvestment,MobileInvestmentService>();
            services.AddSingleton<ITransactionReportService,TransactionReportService>();
            services.AddSingleton<IAuthentication, AuthenticationServices>();
            services.AddSingleton<IOfficeTransactionLoader,OfficeTransactionLoader>();
            services.AddSingleton<IPlatformSuspenderService, PlatformSuspenderService>();
            services.AddSingleton<IStaffServiceDbOperationFilter,StaffServiceDbOperationFilter>();
            services.AddSingleton<IDataService,DataService>();
            services.AddScoped<AuthorizerActionFilter>();
            var CustomerChannelLimit = Configuration.GetSection("CustomerChannelLimit");
            services.Configure<CustomerChannelLimit>(CustomerChannelLimit);
            var AccountChannelLimit = Configuration.GetSection("AccountChannelLimit");
            services.Configure<AccountChannelLimit>(AccountChannelLimit);
            var AccountLimitType = Configuration.GetSection("AccountLimitType");
            services.Configure<AccountLimitType>(AccountLimitType);
            var IndemnityType = Configuration.GetSection("IndemnityType");
            services.Configure<IndemnityType>(IndemnityType);
            var Tier1AccountLimitInfo = Configuration.GetSection("Tier1AccountLimitInfo");
            services.Configure<Tier1AccountLimitInfo>(Tier1AccountLimitInfo);
            var Tier2AccountLimitInfo = Configuration.GetSection("Tier2AccountLimitInfo");
            services.Configure<Tier2AccountLimitInfo>(Tier2AccountLimitInfo);
            var Tier3AccountLimitInfo = Configuration.GetSection("Tier3AccountLimitInfo");
            services.Configure<Tier3AccountLimitInfo>(Tier3AccountLimitInfo);
            services.Configure<IndemnityType>(IndemnityType);
            services.AddSingleton<DapperContext>();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            // Bind LdapSettings from appsettings.json
            // var ldapSettings = Configuration.GetSection("LdapSettings").Get<LdapSettings>();
            // Register the LdapSettings object for dependency injection
            // services.AddSingleton(ldapSettings);
            var ldapSettings = Configuration.GetSection("LdapSettings");
            services.Configure<LdapSettings>(ldapSettings);
            var appSetting = Configuration.GetSection("AppSettingConfig");
            services.Configure<AppSettings>(appSetting);    
            var jwtSetting = Configuration.GetSection("JwtSettings");
            services.Configure<JwtSettings>(jwtSetting);
            var smtpDetails = Configuration.GetSection("SMTPDetails");
            services.Configure<SmtpDetails>(smtpDetails);
            var folderPaths = Configuration.GetSection("FolderPaths");
            services.Configure<FolderPaths>(folderPaths);
            var otpMsg = Configuration.GetSection("OtpMessages");
            services.Configure<OtpMessage>(otpMsg);
            // Add LDAP service
            // JWT Authentication Configuration
            // Configure JWT Authentication
            // Bind JWT settings from appsettings.json
            var jwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            Console.WriteLine("start up aud "+jwtSettings.Audience);
            Console.WriteLine("start up issuer " + jwtSettings.Issuer);
            Console.WriteLine("start up secrey key " + jwtSettings.SecretKey);
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379,password=";
                options.InstanceName = "Prime";
            });
            //  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
            };
            });
            services.AddCors(options =>
            {
             // http://localhost:5173
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder
                        .WithOrigins("http://localhost:5173")
                        .SetIsOriginAllowed((host) => "http://localhost:5173".Equals(host, StringComparison.InvariantCultureIgnoreCase))
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                         );
            });
            services.AddControllers();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("approvestaffprofile", policy =>
                policy.RequireClaim("Permission", "approvestaffprofile"));
                options.AddPolicy("customerdeactivation", policy =>
               policy.RequireClaim("Permission", "customerdeactivation"));
                options.AddPolicy("viewreport", policy =>
               policy.RequireClaim("Permission", "viewreport"));
                options.AddPolicy("customeractivation", policy =>
               policy.RequireClaim("Permission", "customeractivation"));
                options.AddPolicy("approvedeletestaff", policy =>
               policy.RequireClaim("Permission", "approvedeletestaff"));
                options.AddPolicy("viewpermissions", policy =>
               policy.RequireClaim("Permission", "viewpermissions"));
                options.AddPolicy("viewroles", policy =>
               policy.RequireClaim("Permission", "viewroles"));
                options.AddPolicy("viewstaff", policy =>
               policy.RequireClaim("Permission", "viewstaff"));
                options.AddPolicy("searchstaff", policy =>
               policy.RequireClaim("Permission", "searchstaff"));
                options.AddPolicy("viewkyc", policy =>
               policy.RequireClaim("Permission", "viewkyc"));
                options.AddPolicy("viewuser", policy =>
               policy.RequireClaim("Permission", "viewuser"));
                options.AddPolicy("fixreport", policy =>
            policy.RequireClaim("Permission", "fixreport"));
                options.AddPolicy("upgradeaccount", policy =>
              policy.RequireClaim("Permission", "upgradeaccount"));               
              options.AddPolicy("initiateupgradeaccount", policy =>
              policy.RequireClaim("Permission", "initiateupgradeaccount"));
                options.AddPolicy("viewtransaction", policy =>
             policy.RequireClaim("Permission", "viewtransaction"));
                options.AddPolicy("acceptkyc", policy =>
                policy.RequireClaim("Permission", "acceptkyc"));
                options.AddPolicy("rejectkyc", policy =>
               policy.RequireClaim("Permission", "rejectkyc"));
                options.AddPolicy("acceptcustomerindemnity", policy =>
               policy.RequireClaim("Permission", "acceptcustomerindemnity"));
                options.AddPolicy("rejectcustomerindemnity", policy =>
               policy.RequireClaim("Permission", "rejectcustomerindemnity"));
                options.AddPolicy("turnonloginoroff", policy =>
               policy.RequireClaim("Permission", "turnonloginoroff"));
               options.AddPolicy("turnontransactionoroff", policy =>
                policy.RequireClaim("Permission", "turnontransactionoroff"));
                options.AddPolicy("turnonbilloroff", policy =>
                policy.RequireClaim("Permission", "turnonbilloroff"));
                options.AddPolicy("initiatepinapproval", policy =>
           policy.RequireClaim("Permission", "initiatepinapproval"));
                options.AddPolicy("pinapproval", policy =>
           policy.RequireClaim("Permission", "pinapproval"));
                options.AddPolicy("acceptaccountindemnitylimit", policy =>
          policy.RequireClaim("Permission", "acceptaccountindemnitylimit"));
                options.AddPolicy("rejectaccountindemnitylimit", policy =>
          policy.RequireClaim("Permission", "rejectaccountindemnitylimit"));    
            options.AddPolicy("initiateaccountindemnitylimit", policy =>
            policy.RequireClaim("Permission", "initiateaccountindemnitylimit"));              
           options.AddPolicy("initiatecustomerindemnity", policy =>
            policy.RequireClaim("Permission", "initiatecustomerindemnity"));
           options.AddPolicy("Initiatestaffdelete", policy =>
           policy.RequireClaim("Permission", "Initiatestaffdelete"));
                options.AddPolicy("initiatecustomeractivation", policy =>
         policy.RequireClaim("Permission", "initiatecustomeractivation"));
                options.AddPolicy("initiatecustomerdeactivation", policy =>
             policy.RequireClaim("Permission", "initiatecustomerdeactivation"));
                options.AddPolicy("approvekyc", policy =>
               policy.RequireClaim("Permission", "approvekyc"));
                options.AddPolicy("denykyc", policy =>
             policy.RequireClaim("Permission", "denykyc"));
            options.AddPolicy("approveaccountupgrade", policy =>
            policy.RequireClaim("Permission", "approveaccountupgrade"));
           options.AddPolicy("approvekyc", policy =>
          policy.RequireClaim("Permission", "approvekyc"));
                options.AddPolicy("initiatetranscappedlimit", policy =>
               policy.RequireClaim("Permission", "initiatetranscappedlimit"));
                options.AddPolicy("approvetranscappedlimit", policy =>
         policy.RequireClaim("Permission", "approvetranscappedlimit"));
                options.AddPolicy("indemnityformentryofficer", policy =>
          policy.RequireClaim("Permission", "indemnityformentryofficer"));
          options.AddPolicy("fixreportedtransaction", policy =>
          policy.RequireClaim("Permission", "fixreportedtransaction"));
            });
            
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Internal Mobile App (PrimeApp 2.0) Admin Office API", Version = "v1" });
                //  c.OperationFilter<SwaggerFileOperationFilter>();
                c.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                new OpenApiSecurityScheme{
                    Reference = new OpenApiReference{
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                //new List<string>()
                new string[] {}
                   }
                });
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                };
                c.AddSecurityDefinition("Bearer", securityScheme);
                c.SchemaFilter<Retailbanking.Common.CustomObj.SwaggerIgnoreFilter>();
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
        });
        
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            // Add your custom exception handling middleware
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseRouting();
            app.UseCors("AllowSpecificOrigin");

            app.UseAuthentication();

            app.UseAuthorization();
            /*
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            */
            //allow access to static files in security 
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Prime Admin Back office Management Office v1");
            });
        }
    }
}
