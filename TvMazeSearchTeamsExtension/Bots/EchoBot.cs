// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.10.3

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema.Teams;
using System;
using System.IO;
using AdaptiveCards;
using AdaptiveCards.Templating;
using Html2Markdown;
using TvMazeSearchTeamsExtension.Services;
using TvMazeSearchTeamsExtension.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TvMazeSearchTeamsExtension.Bots
{
    public class EchoBot : TeamsActivityHandler
    {
        protected override Task<MessagingExtensionResponse> OnTeamsMessagingExtensionQueryAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionQuery query, CancellationToken cancellationToken)
        {
            switch (query.CommandId)
            {
                case "searchQuery":
                    var text = query?.Parameters?[0]?.Value as string ?? string.Empty;

                    //TODO: Search tv show or movie by name
                    var searchResult = TvMazeService.GetShowsByNameAsync(text).Result;

                    List<MessagingExtensionAttachment> attachments = new List<MessagingExtensionAttachment>();

                    foreach (var item in searchResult)
                    {
                        string imageUrl = item.show.image != null ? item.show.image.medium : "https://i.vippng.com/png/small/365-3650886_esta-oportunidad-ya-no-est-disponible-not-available.png";

                        ThumbnailCard preview = new ThumbnailCard()
                        {
                            Title = item.show.name,
                            Subtitle = item.score.ToString(),
                            Images = new List<CardImage> { new CardImage(imageUrl) },
                        };

                        HeroCard card = new HeroCard()
                        {
                            Title = item.show.name,
                            Subtitle = item.score.ToString(),
                            Text = item.show.summary,
                            Images = new List<CardImage> { new CardImage(imageUrl) },
                        };

                        MessagingExtensionAttachment attachment = new MessagingExtensionAttachment()
                        {
                            Content = card,
                            Preview = card.ToAttachment(),
                            ContentType = HeroCard.ContentType
                        };

                        attachments.Add(attachment);
                    }

                    MessagingExtensionResult composeExtension = new MessagingExtensionResult()
                    {
                        AttachmentLayout = "list",
                        Type = "result",
                        Attachments = attachments
                    };

                    return Task.FromResult(new MessagingExtensionResponse(composeExtension));
                default:
                    throw new NotImplementedException($"Invalid CommandId: {query.CommandId}");
            }
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            switch (action.CommandId)
            {
                case "addToWatchList":
                    return await AddToWatchListCommandAsync (turnContext, action);
            }
            return new MessagingExtensionActionResponse();
        }
       
        protected override Task<MessagingExtensionResponse> OnTeamsAppBasedLinkQueryAsync(ITurnContext<IInvokeActivity> turnContext, AppBasedLinkQuery query, CancellationToken cancellationToken)
        {


            var data = TvMazeService.GetShowDataFromUrlAsync(query.Url).Result;

            var heroCard = new ThumbnailCard
            {
                Title = data.name,
                Subtitle = data.status,
                Text = "Some custom text",
                Images = new List<CardImage> { new CardImage(data.image.medium) },
            };

            var card = CreateAdaptiveCardAttachment(data);

            card.Preview = new Attachment(HeroCard.ContentType, null, heroCard);

            var result = new MessagingExtensionResult(AttachmentLayoutTypes.List, "result", new[] { card });

            return Task.FromResult(new MessagingExtensionResponse(result));
        }

        private static MessagingExtensionAttachment CreateAdaptiveCardAttachment(ShowData data)
        {
            // combine path for cross platform support
            string[] paths = { ".", "Resources", "adaptiveCard.json" };
            var templateJson = File.ReadAllText(Path.Combine(paths));
            AdaptiveCardTemplate template = new AdaptiveCardTemplate(templateJson);

            var converter = new Converter();
            data.summary = converter.Convert(data.summary);

            string adaptiveCardJson = template.Expand(data);

            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return adaptiveCardAttachment.ToMessagingExtensionAttachment();
        }

        private async Task<MessagingExtensionActionResponse> AddToWatchListCommandAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action)
        {
            // The user has chosen to create a card by choosing the 'Create Card' context menu command.
            CardResponse createCardData = ((JObject)action.Data).ToObject<CardResponse>();

            var card = new HeroCard
            {
                Title = "Movie added to Watch List",
                Subtitle = createCardData.Title,
                Text = createCardData.Rating.ToString(),
            };

            await TvMazeService.AddToWatchList(createCardData);

            var attachments = new List<MessagingExtensionAttachment>();
            attachments.Add(new MessagingExtensionAttachment
            {
                Content = card,
                ContentType = HeroCard.ContentType,
                Preview = card.ToAttachment(),
            });

            return new MessagingExtensionActionResponse()
            {
                ComposeExtension = new MessagingExtensionResult
                {
                    AttachmentLayout = "list",
                    Type = "result",
                    Attachments = attachments,
                },
            };
        }
    }
}
