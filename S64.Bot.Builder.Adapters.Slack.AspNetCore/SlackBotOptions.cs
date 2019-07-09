using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder;

namespace S64.Bot.Builder.Adapters.Slack.AspNetCore
{

    public class SlackBotOptions
    {

        public SlackOptions SlackOptions { get; set; }

        public IList<IMiddleware> Middleware { get; set; }

        public SlackBotPaths Paths { get; set; }

        public bool IsValid => this.Paths != null && this.Middleware != null && this.SlackOptions != null;

    }

}
