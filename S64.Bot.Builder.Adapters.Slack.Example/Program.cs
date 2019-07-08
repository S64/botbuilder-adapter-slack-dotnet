using System;

namespace S64.Bot.Builder.Adapters.Slack.Example
{

    class Program
    {

        static string MySlackBotToken
        {
            get
            {
                return Environment.GetEnvironmentVariable("YOUR_SLACK_BOT_TOKEN");
            }
        }

        static void Main(string[] args)
        {
            var options = new SlackOptions
            {
                BotUserToken = MySlackBotToken
            };

            var adapter = new SlackAdapter(options)
                .Use(new SlackMessageTypeMiddleware());

            var myBot = new MyBot();

            adapter.ProcessActivityAsync(async (turnContext, cancellationToken) =>
            {
                await myBot.OnTurnAsync(turnContext, cancellationToken);
            }).Wait();
        }

    }

}
