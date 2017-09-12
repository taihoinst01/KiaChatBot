using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Web.Http.Description;
using System.Net.Http;
using System;
using System.Linq;
using SimpleEchoBot.DB;
using System.Collections.Generic;
using SimpleEchoBot.Models;
using System.Diagnostics;
using System.Net;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {

            HttpResponseMessage response;

            // welcome message 출력   
            if (activity.Type == ActivityTypes.ConversationUpdate && activity.MembersAdded.Any(m => m.Id == activity.Recipient.Id))
            {
                DateTime startTime = DateTime.Now;

                // Db
                DbConnect db = new DbConnect();
                List<DialogList> dlg = db.SelectInitDialog();
                Debug.WriteLine("!!!!!!!!!!! : " + dlg[0].dlgId);

                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                for (int n = 0; n < dlg.Count; n++)
                {

                    Activity reply2 = activity.CreateReply();
                    reply2.Recipient = activity.From;
                    reply2.Type = "message";
                    reply2.Attachments = new List<Attachment>();
                    reply2.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                    List<CardList> card = db.SelectDialogCard(dlg[n].dlgId);
                    List<TextList> text = db.SelectDialogText(dlg[n].dlgId);
                    List<MediaList> media = db.SelectDialogMedia(dlg[n].dlgId);

                    for(int i = 0; i < text.Count; i++)
                    {
                        HeroCard plCard = new HeroCard()
                        {
                            Title = text[i].cardTitle,
                            Subtitle = text[i].cardText
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        reply2.Attachments.Add(plAttachment);
                    }

                    for(int i = 0; i < card.Count; i++)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        List<CardAction> cardButtons = new List<CardAction>();

                        if (card[i].imgUrl != null)
                        {   
                            cardImages.Add(new CardImage(url: card[i].imgUrl));
                        }

                        if(card[i].btn1Type != null)
                        {
                            CardAction plButton = new CardAction()
                            {
                                Value = card[i].btn1Context,
                                Type = card[i].btn1Type,
                                Title = card[i].btn1Title
                            };

                            cardButtons.Add(plButton);
                        }

                        if (card[i].btn2Type != null)
                        {
                            CardAction plButton = new CardAction()
                            {
                                Value = card[i].btn2Context,
                                Type = card[i].btn2Type,
                                Title = card[i].btn2Title
                            };

                            cardButtons.Add(plButton);
                        }

                        if (card[i].btn3Type != null)
                        {
                            CardAction plButton = new CardAction()
                            {
                                Value = card[i].btn3Context,
                                Type = card[i].btn3Type,
                                Title = card[i].btn3Title
                            };

                            cardButtons.Add(plButton);
                        }

                        HeroCard plCard = new HeroCard()
                        {
                            Title = card[i].cardTitle,
                            Subtitle = card[i].cardSubTitle,
                            Images = cardImages,
                            Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        reply2.Attachments.Add(plAttachment);
                    }

                    for(int i = 0; i < media.Count; i++)
                    {
                        List<MediaUrl> mediaURL = new List<MediaUrl>();
                        List<CardAction> cardButtons = new List<CardAction>();

                        if (media[i].mediaUrl != null)
                        {
                            mediaURL.Add(new MediaUrl(url: media[i].mediaUrl));
                        }

                        if (media[i].btn1Type != null)
                        {
                            CardAction plButton = new CardAction()
                            {
                                Value = media[i].btn1Context,
                                Type = media[i].btn1Type,
                                Title = media[i].btn1Title
                            };

                            cardButtons.Add(plButton);
                        }

                        if (media[i].btn2Type != null)
                        {
                            CardAction plButton = new CardAction()
                            {
                                Value = media[i].btn2Context,
                                Type = media[i].btn2Type,
                                Title = media[i].btn2Title
                            };

                            cardButtons.Add(plButton);
                        }

                        if (media[i].btn3Type != null)
                        {
                            CardAction plButton = new CardAction()
                            {
                                Value = media[i].btn3Context,
                                Type = media[i].btn3Type,
                                Title = media[i].btn3Title
                            };

                            cardButtons.Add(plButton);
                        }

                        VideoCard plCard = new VideoCard()
                        {
                            Title = media[i].cardTitle,
                            Text = media[i].cardText,
                            Media = mediaURL,
                            Buttons = cardButtons,
                            Autostart = false
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        reply2.Attachments.Add(plAttachment);
                    }

                    var reply1 = await connector.Conversations.SendToConversationAsync(reply2);
                }
                DateTime endTime = DateTime.Now;
                Debug.WriteLine("프로그램 수행시간 : {0}/ms", ((endTime - startTime).Milliseconds));
            }
            else if (activity.Type == ActivityTypes.Message)
            {
                HandleSystemMessage(activity);
            }

            response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}