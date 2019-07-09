using System;
namespace S64.Bot.Builder.Adapters.Slack.AspNetCore
{

    public class SlackBotPaths
    {

        public SlackPaths()
        {
            this.BasePath = "/api";
            this.RequestPath = "events";
        }

        public string BasePath { get; set; }
        public string RequestPath { get; set; }

    }

}
