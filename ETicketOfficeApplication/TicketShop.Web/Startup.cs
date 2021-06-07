using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketShop.Domain;
using TicketShop.Domain.Domain;
using TicketShop.Domain.DTO;
using TicketShop.Domain.Identity;
using TicketShop.Repository;
using TicketShop.Repository.Implementation;
using TicketShop.Repository.Interface;
using TicketShop.Service;
using TicketShop.Service.Implementation;
using TicketShop.Service.Interface;

namespace TicketShop.Web
{
    public class Startup
    {
        public EmailSettings _emailSettings;

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;

            this._emailSettings = new EmailSettings();
            Configuration.GetSection("EmailSettings").Bind(_emailSettings);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<EShopUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
            services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));

            services.AddTransient<ITicketService, TicketService>();
            services.AddTransient<IShoppingCartService, ShoppingCartService>();
            services.AddTransient<IOrderService, TicketShop.Service.Implementation.OrderService>();

            // E-mail configurations
            services.AddScoped<EmailSettings>(es => _emailSettings);
            services.AddScoped<IEmailService, EmailService>(email => new EmailService(_emailSettings));
            services.AddScoped<IBackgroundEmailSender, BackgroundEmailSender>();
            services.AddHostedService<ConsumeScopedHostedService>();

            // Payment provider (Stripe) settings configuration
            services.Configure<StripeSettingsDTO>(Configuration.GetSection("Stripe"));

            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Payment provider (Stripe) API key configuration
            StripeConfiguration.SetApiKey(Configuration.GetSection("Stripe")["SecretKey"]);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
