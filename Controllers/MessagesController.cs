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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {

        public readonly string TEXTDLG = "2";
        public readonly string CARDDLG = "3";
        public readonly string MEDIADLG = "4";

        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {

            HttpResponseMessage response;

            // welcome message Ãâ·Â   
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
                Debug.WriteLine("ÇÁ·Î±×·¥ ¼öÇà½Ã°£ : {0}/ms", ((endTime - startTime).Milliseconds));
            }
            else if (activity.Type == ActivityTypes.Message)
            {
                string orgMent = "";
                string orgENGMent_history = "";
                JObject Luis = new JObject();
                DbConnect db = new DbConnect();
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                orgMent = activity.Text;
                Translator translateInfo = await getTranslate(orgMent);

                orgENGMent_history = Regex.Replace(translateInfo.data.translations[0].translatedText, @"[^a-zA-Z0-9¤¡-ÆR-\s-&#39;]", "", RegexOptions.Singleline);

                orgENGMent_history = orgENGMent_history.Replace("&#39;", "'");

                Luis = await GetIntentFromBotLUIS(orgENGMent_history);

                Debug.WriteLine("score : " + (float)Luis["intents"][0]["score"]);
                Debug.WriteLine("score : " + Luis["entities"].Count());
                //Debug.WriteLine("entity : " + Luis["entities"][0]["entity"]);

                float luisScore = (float)Luis["intents"][0]["score"];
                int luisEntityCount = (int)Luis["entities"].Count();

                if(luisScore > 0 && luisEntityCount > 0)
                {
                    string intent = (string)Luis["intents"][0]["intent"];
                    string entity = (string)Luis["entities"][0]["entity"];

                    List<LuisList> LuisDialogID = db.SelectLuis(intent, entity);

                    for(int i = 0; i < LuisDialogID.Count; i++)
                    {
                        Activity reply2 = activity.CreateReply();
                        reply2.Recipient = activity.From;
                        reply2.Type = "message";
                        reply2.Attachments = new List<Attachment>();
                        reply2.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                        int dlgID = LuisDialogID[i].dlgId;

                        List<DialogList> dlg = db.SelectDialog(dlgID);

                        for(int n = 0; n < dlg.Count; n++)
                        {
                            string dlgType = dlg[n].dlgType;

                            if(dlgType == TEXTDLG)
                            {
                                List<TextList> text = db.SelectDialogText(dlg[n].dlgId);

                                for (int j = 0; j < text.Count; j++)
                                {
                                    HeroCard plCard = new HeroCard()
                                    {
                                        Title = text[j].cardTitle,
                                        Subtitle = text[j].cardText
                                    };

                                    Attachment plAttachment = plCard.ToAttachment();
                                    reply2.Attachments.Add(plAttachment);
                                }
                            }
                            else if(dlgType == CARDDLG)
                            {
                                List<CardList> card = db.SelectDialogCard(dlg[n].dlgId);

                                for (int j = 0; j < card.Count; j++)
                                {
                                    List<CardImage> cardImages = new List<CardImage>();
                                    List<CardAction> cardButtons = new List<CardAction>();

                                    if (card[j].imgUrl != null)
                                    {
                                        cardImages.Add(new CardImage(url: card[j].imgUrl));
                                    }

                                    if (card[j].btn1Type != null)
                                    {
                                        CardAction plButton = new CardAction()
                                        {
                                            Value = card[j].btn1Context,
                                            Type = card[j].btn1Type,
                                            Title = card[j].btn1Title
                                        };

                                        cardButtons.Add(plButton);
                                    }

                                    if (card[j].btn2Type != null)
                                    {
                                        CardAction plButton = new CardAction()
                                        {
                                            Value = card[j].btn2Context,
                                            Type = card[j].btn2Type,
                                            Title = card[j].btn2Title
                                        };

                                        cardButtons.Add(plButton);
                                    }

                                    if (card[j].btn3Type != null)
                                    {
                                        CardAction plButton = new CardAction()
                                        {
                                            Value = card[j].btn3Context,
                                            Type = card[j].btn3Type,
                                            Title = card[j].btn3Title
                                        };

                                        cardButtons.Add(plButton);
                                    }

                                    HeroCard plCard = new HeroCard()
                                    {
                                        Title = card[j].cardTitle,
                                        Subtitle = card[j].cardSubTitle,
                                        Images = cardImages,
                                        Buttons = cardButtons
                                    };

                                    Attachment plAttachment = plCard.ToAttachment();
                                    reply2.Attachments.Add(plAttachment);
                                }
                            }
                            else if(dlgType == MEDIADLG)
                            {
                                List<MediaList> media = db.SelectDialogMedia(dlg[n].dlgId);

                                for (int j = 0; j < media.Count; j++)
                                {
                                    List<MediaUrl> mediaURL = new List<MediaUrl>();
                                    List<CardAction> cardButtons = new List<CardAction>();

                                    if (media[j].mediaUrl != null)
                                    {
                                        mediaURL.Add(new MediaUrl(url: media[j].mediaUrl));
                                    }

                                    if (media[j].btn1Type != null)
                                    {
                                        CardAction plButton = new CardAction()
                                        {
                                            Value = media[j].btn1Context,
                                            Type = media[j].btn1Type,
                                            Title = media[j].btn1Title
                                        };

                                        cardButtons.Add(plButton);
                                    }

                                    if (media[j].btn2Type != null)
                                    {
                                        CardAction plButton = new CardAction()
                                        {
                                            Value = media[j].btn2Context,
                                            Type = media[j].btn2Type,
                                            Title = media[j].btn2Title
                                        };

                                        cardButtons.Add(plButton);
                                    }

                                    if (media[j].btn3Type != null)
                                    {
                                        CardAction plButton = new CardAction()
                                        {
                                            Value = media[j].btn3Context,
                                            Type = media[j].btn3Type,
                                            Title = media[j].btn3Title
                                        };

                                        cardButtons.Add(plButton);
                                    }

                                    VideoCard plCard = new VideoCard()
                                    {
                                        Title = media[j].cardTitle,
                                        Text = media[j].cardText,
                                        Media = mediaURL,
                                        Buttons = cardButtons,
                                        Autostart = false
                                    };

                                    Attachment plAttachment = plCard.ToAttachment();
                                    reply2.Attachments.Add(plAttachment);
                                }
                            }
                        }

                        var reply1 = await connector.Conversations.SendToConversationAsync(reply2);

                    }
                }else
                {
                    Activity reply_err = activity.CreateReply();
                    reply_err.Recipient = activity.From;
                    reply_err.Type = "message";
                    reply_err.Text = "ÁË¼ÛÇØ¿ä. ¹«½¼ ¸»ÀÎÁö Àß ¸ð¸£°Ú¾î¿ä.";
                    var reply1 = await connector.Conversations.SendToConversationAsync(reply_err);
                }

            }
            else
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

        private static async Task<JObject> GetIntentFromBotLUIS(string Query)
        {
            Query = Uri.EscapeDataString(Query);
            JObject jsonObj = new JObject();
            string[] RequestURI = new string[2];
            RequestURI[0] = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/81c9ba56-7577-42ae-b0ea-45ef0f2c56ca?subscription-key=7efb093087dd48918b903885b944740c" + "&timezoneOffset=0&verbose=true&q=" + Query;
            RequestURI[1] = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/b1437ec6-3301-4c24-8bcb-1af58ee2c47c?subscription-key=7efb093087dd48918b903885b944740c" + "&timezoneOffset=0&verbose=true&q=" + Query;

            using (HttpClient client = new HttpClient())
            {
                for(int i = 0; i < RequestURI.Length; i++)
                {
                    HttpResponseMessage msg = await client.GetAsync(RequestURI[i]);

                    if (msg.IsSuccessStatusCode)
                    {
                        var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                        jsonObj = JObject.Parse(JsonDataResponse);
                    }
                    msg.Dispose();

                    if(jsonObj["entities"].Count() != 0 && (float)jsonObj["intents"][0]["score"] > 0.5)
                    {
                        break;
                    }
                }
            }
            return jsonObj;
        }

        private static async Task<Translator> getTranslate(string input)
        {
            Translator trans = new Translator();

            using (HttpClient client = new HttpClient())
            {
                string appId = "AIzaSyDr4CH9BVfENdM9uoSK0fANFVWD0gGXlJM";

                string url = string.Format("https://translation.googleapis.com/language/translate/v2/?key={0}&q={1}&source=ko&target=en&model=nmt", appId, input);

                HttpResponseMessage msg = await client.GetAsync(url);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    trans = JsonConvert.DeserializeObject<Translator>(JsonDataResponse);
                }
                return trans;
            }

        }

    }
}