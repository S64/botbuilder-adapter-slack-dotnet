using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace S64.Bot.Builder.Adapters.Slack.AspNetCore
{

    public static class ApplicationBuilderExtensions
    {

        public static IApplicationBuilder UseSlack(this IApplicationBuilder applicationBuilder)
        {
            var options = applicationBuilder.ApplicationServices.GetRequiredService<IOptions<SlackBotOptions>>().Value;

            var adapter = new SlackAdapter(options.SlackOptions);

            foreach (var middleware in options.Middleware)
            {
                adapter.Use(middleware);
            }

            var paths = options.Paths;

            if (!paths.BasePath.EndsWith("/"))
            {
                paths.BasePath += "/";
            }

            applicationBuilder.Map(
                $"{paths.BasePath}{paths.RequestPath}",
                botActivitiesAppBuilder => botActivitiesAppBuilder.Run(new SlackRequestHandler(adapter, options).HandleAsync));

            return applicationBuilder;
        }


    }

}
