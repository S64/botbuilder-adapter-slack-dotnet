using System;
using SlackNet.Events;

namespace S64.Bot.Builder.Adapters.Slack
{

    public class SlackChannelData
    {

        public MessageEvent Message { get; set; }
        public AppMention AppMention { get; set; }

        public bool? IsMention { get; set; } = null;

    }

}
