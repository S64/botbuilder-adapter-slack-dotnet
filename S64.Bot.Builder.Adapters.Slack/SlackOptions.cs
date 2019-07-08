using System;

namespace S64.Bot.Builder.Adapters.Slack
{

    public class SlackOptions
    {

        public string BotUserToken { get; set; }

        public bool IsValid => !string.IsNullOrEmpty(BotUserToken);

    }

}
