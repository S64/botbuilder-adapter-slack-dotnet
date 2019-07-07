using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Reactive.Linq;
using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;

namespace S64.Bot.Builder.Adapter.Slack
{

    public class SlackAdapter : BotAdapter
    {

        private readonly SlackRtmClient client;
        private readonly SlackApiClient api;
        private readonly ManualResetEventSlim initialized;
        private readonly ManualResetEventSlim disconnected;

        private AuthTestResponse currentUser { get; set; }

        public SlackAdapter(string token) : base()
        {
            client = new SlackRtmClient(token);
            api = new SlackApiClient(token);
            initialized = new ManualResetEventSlim(false);
            disconnected = new ManualResetEventSlim(false);
        }

        public new SlackAdapter Use(IMiddleware middleware)
        {
            base.Use(middleware);
            return this;
        }

        public async Task ProcessActivityAsync(
            BotCallbackHandler callback = null
        )
        {

            currentUser = await api.Auth.Test();

            await client.Connect().ConfigureAwait(false);
            if (!initialized.IsSet)
            {
                initialized.Set();
            }

            client.Messages.Subscribe((msg) =>
            {
                switch (msg.Type)
                {
                    case "message":
                        OnMessageReceived(msg, callback);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            });

            await client.Events;
        }

        private void OnMessageReceived(MessageEvent message, BotCallbackHandler callback)
        {
            if (!message.Text.Contains($"<@{currentUser.UserId}>")) {
                return;
            }

            var activity = new Activity
            {
                Id = message.ThreadTs,
                Timestamp = DateTimeOffset.Now,
                Type = ActivityTypes.Message,
                Text = message.Text,
                ChannelId = "slack",
                From = new ChannelAccount
                {
                    Id = message.User
                },
                Recipient = null,
                Conversation = new ConversationAccount
                {
                    Id = message.Channel,
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
                            
                            await api.Chat.PostMessage(
                                new Message {
                                    Channel = msg.Conversation.Id,
                                    Text = msg.Text
                                }
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
