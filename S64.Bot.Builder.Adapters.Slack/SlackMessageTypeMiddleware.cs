using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using SlackNet.Events;

namespace S64.Bot.Builder.Adapters.Slack
{

    public class SlackMessageTypeMiddleware : IMiddleware
    {

        public async Task OnTurnAsync(
            ITurnContext turnContext,
            NextDelegate next,
            CancellationToken cancellationToken = default
        )
        {
            if (SlackAdapter.CHANNEL_ID.Equals(turnContext.Activity.ChannelId))
            {
                var adapter = turnContext.Adapter as SlackAdapter;
                var data = turnContext.Activity.ChannelData as SlackChannelData;
                if (turnContext.Activity.Type.Equals(ActivityTypes.Message))
                {
                    if (data.AppMention != null)
                    {
                        data.IsMention = true;
                    }
                    else if (data.Message.Channel == null)
                    {
                        data.IsMention = false;
                    }
                    else
                    {
                        var channel = await adapter.Rest.Conversations.Info(data.Message.Channel);

                        if (data.Message is MessageReplied)
                        {
                            data.IsMention = false;
                        }
                        else if (data.Message.User == null || data.Message.User.Equals("USLACKBOT"))
                        {
                            data.IsMention = false;
                        }
                        else if (data.Message.Subtype != null && !data.Message.Subtype.Equals("thread_broadcast"))
                        {
                            data.IsMention = false;
                        }
                        else if (!(data.Message is MessageEvent) || data.Message is BotMessage)
                        {
                            data.IsMention = false;
                        }
                        else if ((!channel.IsIm || channel.IsMpim) && !data.Message.Text.Contains($"<@{adapter.CurrentUser.UserId}>"))
                        {
                            data.IsMention = false;
                        }
                    }

                    if (data.IsMention == null)
                    {
                        data.IsMention = true;
                    }
                }
            }
            await next(cancellationToken);
        }

    }

}
