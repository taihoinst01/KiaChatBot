using SimpleEchoBot.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace SimpleEchoBot.DB
{
    public class DbConnect
    {
        string connStr = "Data Source=faxtimedb.database.windows.net;Initial Catalog=taihoLab;User ID=faxtime;Password=test2016!;";
        StringBuilder sb = new StringBuilder();

        public List<DialogList> SelectInitDialog()
        {
            SqlDataReader rdr = null;
            List<DialogList> dialog = new List<DialogList>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT DLG_ID, DLG_NAME, DLG_DESCRIPTION, DLG_LANG FROM TBL_DLG WHERE DLG_TYPE = '1' AND USE_YN = 'Y' AND DLG_ID > 999 ORDER BY DLG_ID";

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    int dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    string dlgName = rdr["DLG_NAME"] as string;
                    string dlgDescription = rdr["DLG_DESCRIPTION"] as string;
                    string dlgLang = rdr["DLG_LANG"] as string;

                    DialogList dlg = new DialogList();
                    dlg.dlgId = dlgId;
                    dlg.dlgName = dlgName;
                    dlg.dlgDescription = dlgDescription;
                    dlg.dlgLang = dlgLang;

                    dialog.Add(dlg);
                }
            }
            return dialog;
        }

        public List<DialogList> SelectDialog(int dlgID)
        {
            SqlDataReader rdr = null;
            List<DialogList> dialog = new List<DialogList>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT DLG_ID, DLG_NAME, DLG_DESCRIPTION, DLG_LANG, DLG_TYPE FROM TBL_DLG WHERE DLG_ID = @dlgID AND USE_YN = 'Y' AND DLG_ID > 999 ORDER BY DLG_ID";

                cmd.Parameters.AddWithValue("@dlgID", dlgID);
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    int dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    string dlgName = rdr["DLG_NAME"] as string;
                    string dlgDescription = rdr["DLG_DESCRIPTION"] as string;
                    string dlgLang = rdr["DLG_LANG"] as string;
                    string dlgType = rdr["DLG_TYPE"] as string;

                    DialogList dlg = new DialogList();
                    dlg.dlgId = dlgId;
                    dlg.dlgName = dlgName;
                    dlg.dlgDescription = dlgDescription;
                    dlg.dlgLang = dlgLang;
                    dlg.dlgType = dlgType;

                    dialog.Add(dlg);
                }
            }
            return dialog;
        }

        public List<CardList> SelectDialogCard(int dlgID)
        {
            SqlDataReader rdr = null;
            List<CardList> dialogCard = new List<CardList>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT CARD_DLG_ID, DLG_ID, CARD_TITLE, CARD_SUBTITLE, CARD_TEXT, IMG_URL," +
                    "BTN_1_TYPE, BTN_1_TITLE, BTN_1_CONTEXT, BTN_2_TYPE, BTN_2_TITLE, BTN_2_CONTEXT, BTN_3_TYPE, BTN_3_TITLE, BTN_3_CONTEXT " +
                    "FROM TBL_DLG_CARD WHERE DLG_ID = @dlgID AND USE_YN = 'Y' AND DLG_ID > 999 ORDER BY CARD_ORDER_NO";

                cmd.Parameters.AddWithValue("@dlgID", dlgID);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    int cardDlgId = Convert.ToInt32(rdr["CARD_DLG_ID"]);
                    int dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    string cardTitle = rdr["CARD_TITLE"] as string;
                    string cardSubTitle = rdr["CARD_SUBTITLE"] as string;
                    string cardText = rdr["CARD_TEXT"] as string;
                    string imgUrl = rdr["IMG_URL"] as string;
                    string btn1Type = rdr["BTN_1_TYPE"] as string;
                    string btn1Title = rdr["BTN_1_TITLE"] as string;
                    string btn1Context = rdr["BTN_1_CONTEXT"] as string;
                    string btn2Type = rdr["BTN_2_TYPE"] as string;
                    string btn2Title = rdr["BTN_2_TITLE"] as string;
                    string btn2Context = rdr["BTN_2_CONTEXT"] as string;
                    string btn3Type = rdr["BTN_3_TYPE"] as string;
                    string btn3Title = rdr["BTN_3_TITLE"] as string;
                    string btn3Context = rdr["BTN_3_CONTEXT"] as string;

                    CardList dlgCard = new CardList();
                    dlgCard.cardDlgId = cardDlgId;
                    dlgCard.dlgId = dlgId;
                    dlgCard.cardTitle = cardTitle;
                    dlgCard.cardSubTitle = cardSubTitle;
                    dlgCard.cardText = cardText;
                    dlgCard.imgUrl = imgUrl;
                    dlgCard.btn1Type = btn1Type;
                    dlgCard.btn1Title = btn1Title;
                    dlgCard.btn1Context = btn1Context;
                    dlgCard.btn2Type = btn2Type;
                    dlgCard.btn2Title = btn2Title;
                    dlgCard.btn2Context = btn2Context;
                    dlgCard.btn3Type = btn3Type;
                    dlgCard.btn3Title = btn3Title;
                    dlgCard.btn3Context = btn3Context;

                    dialogCard.Add(dlgCard);
                }
            }
            return dialogCard;
        }

        public List<TextList> SelectDialogText(int dlgID)
        {
            SqlDataReader rdr = null;
            List<TextList> dialogText = new List<TextList>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT TEXT_DLG_ID, DLG_ID, CARD_TITLE, CARD_TEXT FROM TBL_DLG_TEXT WHERE DLG_ID = @dlgID AND USE_YN = 'Y' AND DLG_ID > 999";

                cmd.Parameters.AddWithValue("@dlgID", dlgID);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    int textDlgId = Convert.ToInt32(rdr["TEXT_DLG_ID"]);
                    int dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    string cardTitle = rdr["CARD_TITLE"] as string;
                    string cardText = rdr["CARD_TEXT"] as string;


                    TextList dlgText = new TextList();
                    dlgText.textDlgId = textDlgId;
                    dlgText.dlgId = dlgId;
                    dlgText.cardTitle = cardTitle;
                    dlgText.cardText = cardText;


                    dialogText.Add(dlgText);
                }
            }
            return dialogText;
        }

        public List<MediaList> SelectDialogMedia(int dlgID)
        {
            SqlDataReader rdr = null;
            List<MediaList> dialogMedia = new List<MediaList>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT MEDIA_DLG_ID, DLG_ID, CARD_TITLE, CARD_TEXT, MEDIA_URL," +
                    "BTN_1_TYPE, BTN_1_TITLE, BTN_1_CONTEXT, BTN_2_TYPE, BTN_2_TITLE, BTN_2_CONTEXT, BTN_3_TYPE, BTN_3_TITLE, BTN_3_CONTEXT " +
                    "FROM TBL_DLG_MEDIA WHERE DLG_ID = @dlgID AND USE_YN = 'Y' AND DLG_ID > 999";

                cmd.Parameters.AddWithValue("@dlgID", dlgID);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    int mediaDlgId = Convert.ToInt32(rdr["MEDIA_DLG_ID"]);
                    int dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    string cardTitle = rdr["CARD_TITLE"] as string;
                    string cardText = rdr["CARD_TEXT"] as string;
                    string mediaUrl = rdr["MEDIA_URL"] as string;
                    string btn1Type = rdr["BTN_1_TYPE"] as string;
                    string btn1Title = rdr["BTN_1_TITLE"] as string;
                    string btn1Context = rdr["BTN_1_CONTEXT"] as string;
                    string btn2Type = rdr["BTN_2_TYPE"] as string;
                    string btn2Title = rdr["BTN_2_TITLE"] as string;
                    string btn2Context = rdr["BTN_2_CONTEXT"] as string;
                    string btn3Type = rdr["BTN_3_TYPE"] as string;
                    string btn3Title = rdr["BTN_3_TITLE"] as string;
                    string btn3Context = rdr["BTN_3_CONTEXT"] as string;

                    MediaList dlgMedia = new MediaList();
                    dlgMedia.mediaDlgId = mediaDlgId;
                    dlgMedia.dlgId = dlgId;
                    dlgMedia.cardTitle = cardTitle;
                    dlgMedia.cardText = cardText;
                    dlgMedia.mediaUrl = mediaUrl;
                    dlgMedia.btn1Type = btn1Type;
                    dlgMedia.btn1Title = btn1Title;
                    dlgMedia.btn1Context = btn1Context;
                    dlgMedia.btn2Type = btn2Type;
                    dlgMedia.btn2Title = btn2Title;
                    dlgMedia.btn2Context = btn2Context;
                    dlgMedia.btn3Type = btn3Type;
                    dlgMedia.btn3Title = btn3Title;
                    dlgMedia.btn3Context = btn3Context;

                    dialogMedia.Add(dlgMedia);
                }
            }
            return dialogMedia;
        }

        public List<LuisList> SelectLuis(string intent, String entities)
        {
            SqlDataReader rdr = null;
            List<LuisList> luisList = new List<LuisList>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = " SELECT RELATION_ID, LUIS_ID, LUIS_INTENT, LUIS_ENTITIES, BEFORE_1_LUIS, BEFORE_1_INTENT, BEFORE_1_ENTITIES," +
                                  " BEFORE_2_LUIS, BEFORE_2_INTENT, BEFORE_2_ENTITIES, BEFORE_3_LUIS, BEFORE_3_INTENT, BEFORE_3_ENTITIES, DLG_ID, DLG_ORDER_NO " +
                                  " FROM TBL_DLG_RELATION_LUIS WHERE LUIS_INTENT = @intent" +
                                  " AND LUIS_ENTITIES LIKE '%" + entities + "%'" +
                                  " AND USE_YN = 'Y' ORDER BY DLG_ORDER_NO";

                cmd.Parameters.AddWithValue("@intent", intent);

                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (rdr.Read())
                {
                    int dlgId = Convert.ToInt32(rdr["DLG_ID"]);
                    string dlgOrderNo = rdr["DLG_ORDER_NO"] as string;

                    LuisList luis = new LuisList();
                    luis.dlgId = dlgId;
                    luis.dlgOrderNo = dlgOrderNo;

                    luisList.Add(luis);
                }
            }
            return luisList;
        }

    }
}