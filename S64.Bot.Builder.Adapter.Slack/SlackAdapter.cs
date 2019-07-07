using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using SlackAPI;
using SlackAPI.WebSocketMessages;

namespace S64.Bot.Builder.Adapter.Slack
{

    public class SlackAdapter : BotAdapter
    {

        private readonly SlackSocketClient client;
        private readonly ManualResetEventSlim initialized;
        private readonly ManualResetEventSlim disconnected;

        public SlackAdapter(string token) : base()
        {
            client = new SlackSocketClient(token);
            initialized = new ManualResetEventSlim(false);
            disconnected = new ManualResetEventSlim(false);
        }

        public new SlackAdapter Use(IMiddleware middleware)
        {
            base.Use(middleware);
            return this;
        }

        public void ProcessActivity(
            BotCallbackHandler callback = null
        )
        {
            if (client.IsConnected)
            {
                throw new NotImplementedException();
            }

            client.OnMessageReceived += (message) =>
            {
                switch (message.type) {
                    case "message":
                        OnMessageReceived(message, callback);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            };

            client.Connect((connected) =>
            {
                if (!initialized.IsSet)
                {
                    initialized.Set();
                }
            });

            disconnected.Wait();
        }

        private void OnMessageReceived(NewMessage message, BotCallbackHandler callback)
        {
            var activity = new Activity
            {
                Id = message.thread_ts.ToLongDateString(),
                Timestamp = DateTimeOffset.Now,
                Type = ActivityTypes.Message,
                Text = message.text,
                ChannelId = "slack",
                From = new ChannelAccount
                {
                    Id = message.user
                },
                Recipient = null,
                Conversation = new ConversationAccount
                {
                    Id = message.channel,
                },
                ChannelData = message,
            };

            using (var context = new TurnContext(this, activity))
            {
                this.RunPipelineAsync(
                    context,
                    callback,
                    default(CancellationToken)
                ).Wait();
            }
        }

        public override async Task<ResourceResponse[]> SendActivitiesAsync(
            ITurnContext turnContext,
            Activity[] activities,
            CancellationToken cancellationToken
        )
        {
            var responses = new ResourceResponse[activities.Length];

            for (int idx = 0; idx < activities.Length; idx++)
            {
                var activity = activities[idx];

                switch (activity.Type)
                {
                    case ActivityTypes.Message:
                        {
                            IMessageActivity msg = activity.AsMessageActivity();

                            client.PostMessage(
                                null,
                                msg.Conversation.Id,
                                msg.Text
                            );
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }

                responses[idx] = new ResourceResponse(activity.Id);
            }

            return responses;
        }

        public override Task<ResourceResponse> UpdateActivityAsync(
            ITurnContext turnContext,
            Activity activity,
            CancellationToken cancellationToken
        )
        {
            throw new NotImplementedException();
        }

        public override Task DeleteActivityAsync(
            ITurnContext turnContext,
            ConversationReference reference,
            CancellationToken cancellationToken
        )
        {
            throw new NotImplementedException();
        }

    }

}
