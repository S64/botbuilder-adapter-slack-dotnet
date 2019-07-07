﻿using System;

namespace S64.Bot.Builder.Adapter.Slack.Example
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
            var adapter = new SlackAdapter(MySlackBotToken);

            var myBot = new MyBot();

            adapter.ProcessActivityAsync(async (turnContext, cancellationToken) =>
            {
                await myBot.OnTurnAsync(turnContext, cancellationToken);
            }).Wait();
        }

    }

}
