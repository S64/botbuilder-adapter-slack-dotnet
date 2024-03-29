﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace S64.Bot.Builder.Adapters.Slack.AspNetCore.Example
{

    public class Startup
    {

        static string MySlackBotToken
        {
            get
            {
                return Environment.GetEnvironmentVariable("YOUR_SLACK_BOT_TOKEN");
            }
        }

        static string MySlackVerificationToken
        {
            get
            {
                return Environment.GetEnvironmentVariable("YOUR_SLACK_VERIFICATION_TOKEN");
            }
        }

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSlackBot<MyBot>(options =>
            {
                options.Middleware = new List<IMiddleware> { new SlackMessageTypeMiddleware() };
                options.SlackOptions = new SlackOptions
                {
                    BotUserToken = MySlackBotToken,
                };
                options.Paths = new SlackBotPaths
                {
                    BasePath = "/api",
                    RequestPath = "events",
                };
                options.VerificationToken = MySlackVerificationToken;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSlack();
        }
    }

}
