using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace S64.Bot.Builder.Adapters.Slack.Example
{

    public class MyBot : ActivityHandler
    {

        protected override async Task OnMessageActivityAsync(
            ITurnContext<IMessageActivity> turnContext,
            CancellationToken cancellationToken
        )
        {
            if (SlackAdapter.CHANNEL_ID.Equals(turnContext.Activity.ChannelId))
            {
                var data = turnContext.Activity.ChannelData as SlackChannelData;
                if (turnContext.Activity.Type.Equals(ActivityTypes.Message) && data.IsMention == true)
                {
                    await turnContext.SendActivityAsync(
                        MessageFactory.Text($"Hello World! Your message is: `{turnContext.Activity.Text}`."),
                        cancellationToken
                    );
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

    }

}
