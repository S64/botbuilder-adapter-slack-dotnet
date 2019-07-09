using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using SlackNet;
using SlackNet.Events;

namespace S64.Bot.Builder.Adapters.Slack.AspNetCore
{

    public class SlackRequestHandler
    {

        private readonly SlackAdapter adapter;
        private readonly SlackJsonSettings jsonSettings;
        private readonly SlackBotOptions options;

        public SlackRequestHandler(
            SlackAdapter adapter,
            SlackBotOptions options
        )
        {
            this.adapter = adapter;
            this.jsonSettings = Default.JsonSettings(Default.SlackTypeResolver(Default.AssembliesContainingSlackTypes));
            this.options = options;
        }

        public async Task HandleAsync(HttpContext httpContext)
        {
            var request = httpContext.Request;
            var response = httpContext.Response;

            var requestServices = httpContext.RequestServices;
            var bot = requestServices.GetRequiredService<IBot>();

            if (request.Method != HttpMethods.Post)
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return;
            }

            var body = DeserializeRequestBody(httpContext);

            if (body is UrlVerification urlVerification && urlVerification.Token.Equals(options.VerificationToken))
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "application/x-www-form-urlencoded";

                await response.WriteAsync(urlVerification.Challenge);
                return;
            }

            if (body is EventCallback eventCallback && eventCallback.Token.Equals(options.VerificationToken))
            {
                if (eventCallback.Event is MessageEvent msg)
                {
                    await adapter.ProcessActivityAsync(msg, bot.OnTurnAsync);
                    response.StatusCode = (int)HttpStatusCode.OK;
                    return;
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }
            }
            else
            {
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }
        }

        private Event DeserializeRequestBody(HttpContext context)
        {
            return JsonSerializer.Create(jsonSettings.SerializerSettings)
                .Deserialize<Event>(new JsonTextReader(new StreamReader(context.Request.Body)));
        }

    }

}
