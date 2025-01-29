using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrbanSystem.Data;
using UrbanSystem.Data.Models;
using static UrbanSystem.Web.Infrastructure.Extensions.ApplicationBuilderExtensions;
using UrbanSystem.Data.Repository.Contracts;
using UrbanSystem.Data.Repository;
using UrbanSystem.Web.Infrastructure.Extensions;
using UrbanSystem.Services.Data.Contracts;
using UrbanSystem.Services.Data;
using UrbanSystem.Services.Interfaces;
using UrbanSystem.Services;

namespace UrbanSystem.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string adminEmail = builder.Configuration.GetValue<string>("Administrator:Email")!;
            string adminUsername = builder.Configuration.GetValue<string>("Administrator:Username")!;
            string adminPassword = builder.Configuration.GetValue<string>("Administrator:Password")!;

            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options => {
                ConfigureIdentity(options, builder);
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddRoles<IdentityRole<Guid>>()
            .AddSignInManager<SignInManager<ApplicationUser>>()
            .AddUserManager<UserManager<ApplicationUser>>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
            });

            // Register repositories
            RegisterUserDefinedRepositories(builder);

            // Register user-defined services
            RegisterUserDefinedServices(builder);

            // Add MVC services
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.SeedAdministrator(adminUsername, adminEmail, adminPassword);

            app.MapControllerRoute(
                name: "Areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            
            app.MapRazorPages();

            app.ApplyMigrations();
            app.Run();
        }

        private static void RegisterUserDefinedRepositories(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped(typeof(IRepository<,>), typeof(BaseRepository<,>));
            builder.Services.AddScoped<IRepository<Meeting, Guid>, BaseRepository<Meeting, Guid>>();
            builder.Services.AddScoped<IRepository<ApplicationUser, Guid>, BaseRepository<ApplicationUser, Guid>>();
            builder.Services.AddScoped<IRepository<ApplicationUserSuggestion, object>, BaseRepository<ApplicationUserSuggestion, object>>();
            builder.Services.AddScoped<IRepository<Data.Models.Project, Guid>, BaseRepository<Data.Models.Project, Guid>>();
            builder.Services.AddScoped<IRepository<Meeting, Guid>, BaseRepository<Meeting, Guid>>();
        }

        private static void RegisterUserDefinedServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IBaseService, BaseService>();
            builder.Services.AddScoped<ILocationService, LocationService>();
            builder.Services.AddScoped<IMeetingService, MeetingService>();
            builder.Services.AddScoped<ISuggestionService, SuggestionService>();
            builder.Services.AddScoped<IMySuggestionService, MySuggestionService>();
            builder.Services.AddScoped<IProjectService, ProjectService>();
            builder.Services.AddScoped<IMeetingService, MeetingService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ISuggestionManagementService, SuggestionManagementService>();
            builder.Services.AddScoped<IProjectManagementService, ProjectManagementService>();
            builder.Services.AddScoped<IMeetingManagementService, MeetingManagementService>();
            builder.Services.AddScoped<ILocationManagementService, LocationManagementService>();
        }

        private static void ConfigureIdentity(IdentityOptions options, WebApplicationBuilder builder)
        {
            options.Password.RequireDigit = builder.Configuration.GetValue<bool>("Identity:Password:RequireDigits");
            options.Password.RequireLowercase = builder.Configuration.GetValue<bool>("Identity:Password:RequireLowercase");
            options.Password.RequireUppercase = builder.Configuration.GetValue<bool>("Identity:Password:RequireUppercase");
            options.Password.RequireNonAlphanumeric = builder.Configuration.GetValue<bool>("Identity:Password:RequireNonAlphanumeric");
            options.Password.RequiredLength = builder.Configuration.GetValue<int>("Identity:Password:RequiredLength");
            options.Password.RequiredUniqueChars = builder.Configuration.GetValue<int>("Identity:Password:RequiredUniqueChars");

            options.SignIn.RequireConfirmedAccount = builder.Configuration.GetValue<bool>("Identity:SignIn:RequireConfirmedAccount");
            options.SignIn.RequireConfirmedEmail = builder.Configuration.GetValue<bool>("Identity:SignIn:RequireConfirmedEmail");
            options.SignIn.RequireConfirmedPhoneNumber = builder.Configuration.GetValue<bool>("Identity:SignIn:RequireConfirmedPhoneNumber");

            options.User.RequireUniqueEmail = builder.Configuration.GetValue<bool>("Identity:User:RequireUniqueEmail");
        }
    }
}
