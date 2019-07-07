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
            await turnContext.SendActivityAsync(
                MessageFactory.Text($"Hello World! Your message is: `{turnContext.Activity.Text}`."),
                cancellationToken
            );
        }

    }

}
