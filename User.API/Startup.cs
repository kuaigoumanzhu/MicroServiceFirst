﻿using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using User.API.Data;

namespace User.API
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
            services.AddDbContext<UserContext>(options=>
            {
                options.UseMySQL(Configuration.GetConnectionString("MysqlUser"));
            });
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            InitUserDatabase(app);
        }
        // 默认用户
        public void InitUserDatabase(IApplicationBuilder app)
        {
            using (var scope= app.ApplicationServices.CreateScope())
            {
                var userContext = scope.ServiceProvider.GetRequiredService<UserContext>();
                userContext.Database.Migrate();
                if (!userContext.Users.Any())
                {
                    userContext.Users.Add(new Models.AppUser
                    {
                        Name = "han"
                    });
                    userContext.SaveChanges();
                }
            }
        }
    }
}
