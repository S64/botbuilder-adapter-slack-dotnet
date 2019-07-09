using System;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace S64.Bot.Builder.Adapters.Slack.AspNetCore
{

    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddSlackBot<TBot>(
            this IServiceCollection services,
            Action<SlackBotOptions> setupAction = null
        ) where TBot : class, IBot
        {
            services.AddTransient<IBot, TBot>();

            services.Configure(setupAction);

            return services;
        }

    }

}
