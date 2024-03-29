﻿using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder;

namespace S64.Bot.Builder.Adapters.Slack.AspNetCore
{

    public class SlackBotOptions
    {

        public SlackOptions SlackOptions { get; set; }

        public IList<IMiddleware> Middleware { get; set; } = new List<IMiddleware>();

        public SlackBotPaths Paths { get; set; }

        public string VerificationToken { get; set; }

        public bool IsValid => !string.IsNullOrEmpty(VerificationToken) && this.Paths != null && this.Middleware != null && this.SlackOptions != null;

    }

}
