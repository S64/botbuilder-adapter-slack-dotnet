using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Reactive.Linq;
using SlackNet;
using SlackNet.Events;
using SlackNet.WebApi;

namespace S64.Bot.Builder.Adapters.Slack
{

    public class SlackAdapter : BotAdapter
    {

        public const string CHANNEL_ID = "slack";

        private readonly SlackOptions options;
        public readonly SlackApiClient Rest;

        public AuthTestResponse CurrentUser { get; set; }

        public SlackAdapter(SlackOptions options) : base()
        {
            this.options = options;
            Rest = new SlackApiClient(options.BotUserToken);
        }

        public new SlackAdapter Use(IMiddleware middleware)
        {
            base.Use(middleware);
            return this;
        }

        private async Task InitUserIfNeeded()
        {
            if (CurrentUser == null)
            {
                CurrentUser = await Rest.Auth.Test();
            }
        }

        public async Task ProcessActivityBySocketAsync(
            BotCallbackHandler callback = null
        )
        {
            await InitUserIfNeeded();

            var socket = new SlackRtmClient(options.BotUserToken);
            await socket.Connect().ConfigureAwait(false);

            var sub = socket.Messages.Subscribe(async (msg) =>
            {
                switch (msg.Type)
                {
                    case "message":
                        await OnMessageReceived(msg, callback);
                        break;
                    default:
                        // TODO: Implement non-message type
                        break;
                }
            });

            await socket.Events;
            sub.Dispose();
        }

        private async Task OnMessageReceived(MessageEvent message, BotCallbackHandler callback)
        {
            var activity = new Activity
            {
                Id = message.ThreadTs,
                Timestamp = DateTimeOffset.Now,
                Type = ActivityTypes.Message,
                Text = message.Text,
                ChannelId = CHANNEL_ID,
                From = new ChannelAccount
                {
                    Id = message.User
                },
                Recipient = null,
                Conversation = new ConversationAccount
                {
                    Id = message.Channel != null ? message.Channel : null,
                },
                ChannelData = new SlackChannelData
                {
                    Message = message
                },
            };

            using (var context = new TurnContext(this, activity))
            {
                await this.RunPipelineAsync(
                    context,
                    callback,
                    default(CancellationToken)
                );
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

                            await Rest.Chat.PostMessage(
                                new Message
                                {
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
