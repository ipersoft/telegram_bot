'CODICE SVILUPPATO DA PAOLO FISCO @IPERSOFT

Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Net.Mail
Imports FireSharp.Config
Imports Newtonsoft.Json

Public Class hook
    Inherits System.Web.UI.Page
    Dim Token As String
    Dim ConfigJson As String
    Dim ConfigAdmin As AdminJSON
    Dim Emojis As List(Of Emoji)
    Dim tmenu As List(Of TelegramMenu.TMenu)
    Dim sLog As New StringBuilder
    Dim pathProfile As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

#Region "Leggo configurazione bot"
        Try

            Try
                ConfigAdmin = JsonConvert.DeserializeObject(Of AdminJSON)(My.Computer.FileSystem.ReadAllText(Server.MapPath("admin.json")))
            Catch ex As Exception
                SendError("Leggo admin bot" + vbCrLf + ex.Message)
            End Try


            Dim txtJson As String
            txtJson = My.Computer.FileSystem.ReadAllText(Server.MapPath("config.json"))

            Dim cBots As New List(Of BotConfig)
            cBots = JsonConvert.DeserializeObject(Of List(Of BotConfig))(txtJson)

            If Not Request.QueryString("bot") Is Nothing Then
                For Each cBot In cBots
                    If cBot.BOT.ToUpper = Request.QueryString("bot").ToUpper Then
                        Token = cBot.TOKEN
                        ConfigJson = cBot.LINK
                    End If
                Next
            Else
                Exit Sub
            End If
        Catch ex As Exception
            SendError("Leggo configurazione bot" + vbCrLf + ex.Message)
            Exit Sub
        End Try
#End Region

#Region "Recupero Messaggio Telegram"
        Dim documentContents As String
        Try
            Using receiveStream As Stream = Request.InputStream
                Using readStream As New StreamReader(receiveStream, Encoding.UTF8)
                    documentContents = readStream.ReadToEnd()
                End Using
            End Using

        Catch ex As Exception
            SendError("Recupero messaggio telegram" + vbCrLf + ex.Message)
            Exit Sub
        End Try
#End Region


#Region "Recupero trascodifica emoji"
        Try
            Dim sEmoji As String
            Using w As New WebClient
                w.Encoding = Encoding.UTF8
                sEmoji = w.DownloadString("https://raw.githubusercontent.com/ipersoft/telegram_bot/master/emoji.json")
            End Using
            Emojis = JsonConvert.DeserializeObject(Of List(Of Emoji))(sEmoji)
        Catch ex As Exception

        End Try
#End Region




        'Json messaggio
        Dim tMessage As TelegramMessage.TMessage

        tMessage = JsonConvert.DeserializeObject(Of TelegramMessage.TMessage)(documentContents)


#Region "Memorizzazione dati utente"
        Try
            If tMessage.callback_query Is Nothing Then
                'Comando diretto

                sLog.AppendLine("Memorizzo dati su firebase")
                'x debug Firebase(ActionFirebase.PushData, Request.QueryString("bot").ToUpper + "/" + tMessage.message.from.id.ToString, documentContents)

                Dim profileTelegram As New UserProfile
                profileTelegram.ChatID = tMessage.message.from.id
                profileTelegram.LastCommand = tMessage.message.text
                profileTelegram.LastUpdate = Now

                If Not tMessage.message.location Is Nothing Then
                    sLog.AppendLine("Memorizzo posizione")
                    Dim sError As String = ""
                    profileTelegram.Latitude = tMessage.message.location.latitude
                    profileTelegram.Longitude = tMessage.message.location.longitude
                    'SendMessageTelegram(tMessage.message.from.id, "Posizione memorizzata", "", "", sError)
                End If
                Firebase(ActionFirebase.SetData, Request.QueryString("bot").ToUpper + "/users/profile/" + tMessage.message.from.id.ToString, JsonConvert.SerializeObject(profileTelegram))

            End If
        Catch ex As Exception
            sLog.AppendLine.AppendLine(ex.Message)
            sLog.AppendLine(Request.QueryString("bot").ToUpper)
            sLog.AppendLine(documentContents)
            SendError(sLog.ToString)
            Exit Sub
        End Try
#End Region

#Region "Scarico configurazione BOT"
        sLog.AppendLine("Scarico configurazione")
        Dim sWeb As String
        Dim sPagina As String
        sWeb = ConfigJson

        Try
            Using w As New WebClient
                w.Encoding = Encoding.UTF8
                sPagina = w.DownloadString(sWeb)
                tmenu = JsonConvert.DeserializeObject(Of List(Of TelegramMenu.TMenu))(sPagina)
            End Using
        Catch ex As Exception
            sLog.AppendLine.AppendLine(ex.Message)
            sLog.AppendLine(Request.QueryString("bot").ToUpper)
            sLog.AppendLine(documentContents)
            SendError(sLog.ToString)
            Exit Sub
        End Try
#End Region

        Try

            Dim sError As String = ""

            sLog.AppendLine("Controllo tipo messaggio")
            If tMessage.callback_query Is Nothing Then

#Region "Messaggio HOME"

#Region "Invio ID in base al tipo allegato"
                If Not tMessage.message.location Is Nothing Then
                    sLog.AppendLine("Invio Menu LOCATION")
                    RicercaMenu("LOCATION", False, tMessage.message.from.id, "", tMessage.message.message_id)
                    Exit Sub
                End If
                If Not tMessage.message.document Is Nothing Then
                    sLog.AppendLine("Invio ID Documento")
                    SendMessageTelegram(tMessage.message.from.id, "ID Documento: " & tMessage.message.document.file_id, "", "", sError)
                    Exit Sub
                End If
                If Not tMessage.message.voice Is Nothing Then
                    sLog.AppendLine("Invio ID Voce")
                    SendMessageTelegram(tMessage.message.from.id, "ID Voce: " & tMessage.message.voice.file_id, "", "", sError)
                    Exit Sub
                End If
                If Not tMessage.message.audio Is Nothing Then
                    sLog.AppendLine("Invio ID Audio")
                    SendMessageTelegram(tMessage.message.from.id, "ID Audio: " & tMessage.message.audio.file_id, "", "", sError)
                    Exit Sub
                End If
                If Not tMessage.message.video Is Nothing Then
                    sLog.AppendLine("Invio ID Video")
                    SendMessageTelegram(tMessage.message.from.id, "ID Video: " & tMessage.message.video.file_id, "", "", sError)
                    Exit Sub
                End If
                If Not tMessage.message.photo Is Nothing Then
                    sLog.AppendLine("Invio ID Immagine")
                    Dim sFoto As New StringBuilder
                    For Each tPhoto In tMessage.message.photo
                        sFoto.Append("- ID Immagine: ").Append(tPhoto.file_id).Append(" Dimensione: ").Append(tPhoto.file_size)
                        sFoto.Append(" Formato: ").Append(tPhoto.height).Append("x").AppendLine(tPhoto.width)
                    Next
                    SendMessageTelegram(tMessage.message.from.id, sFoto.ToString, "", "", sError)
                    Exit Sub
                End If
#End Region


                sLog.AppendLine("Messaggio normale invio comandi ID=HOME")

                RicercaMenu("HOME", False, tMessage.message.from.id, "", tMessage.message.message_id)

                'Dim sButton As New StringBuilder
                'Dim blnButton As Boolean = False
                'Dim listButton As String = ""
                'For Each smenu In tmenu
                '    If smenu.ID = "HOME" Then

                '        If sError <> "" Then
                '            sLog.AppendLine.AppendLine(sError)
                '            sLog.AppendLine(Request.QueryString("bot").ToUpper)
                '            sLog.AppendLine(documentContents)
                '            SendError(sLog.ToString)
                '        End If

                '    End If
                'Next


#End Region
            Else
#Region "Messaggio callback"


                sLog.AppendLine("Risposta callback")


                'log callback per statistiche
                Dim LogCallBack As New UserProfile
                LogCallBack.LastCommand = tMessage.callback_query.data
                LogCallBack.LastUpdate = Now
                Firebase(ActionFirebase.PushData, Request.QueryString("bot").ToUpper + "/callback/" + Now.ToString("yyyy-MM-dd"), JsonConvert.SerializeObject(LogCallBack))
                pathProfile = Request.QueryString("bot").ToUpper + "/users/profile/" + tMessage.callback_query.from.id.ToString

                RicercaMenu(tMessage.callback_query.data, True, tMessage.callback_query.from.id, tMessage.callback_query.id, tMessage.callback_query.message.message_id)

#End Region




            End If
        Catch ex As Exception
            sLog.AppendLine.AppendLine(ex.Message)
            sLog.AppendLine(Request.QueryString("bot").ToUpper)
            sLog.AppendLine(documentContents)
            SendError(sLog.ToString)
        End Try


    End Sub
    Sub RicercaMenu(IDMenu As String, callback As Boolean, chat_id As Integer, callback_id As String, message_id As Integer)
        For Each smenu In tmenu
            If smenu.ID = IDMenu Then
                ElaboraMenu(smenu, callback, chat_id, callback_id, message_id)
                If Not smenu.IDNext Is Nothing Then
                    RicercaMenu(smenu.IDNext, False, chat_id, callback_id, message_id)
                End If
            End If
        Next
    End Sub
    Sub ElaboraMenu(smenu As TelegramMenu.TMenu, callback As Boolean, chat_id As Integer, callback_id As String, message_id As Integer)
        Dim sError As String = ""
        Dim sLog As New StringBuilder
        Dim listButton As String = ""

        'Se callback invio comando chiusura
        If callback Then
            Dim sAlertText As String = ""
            If Not smenu.AlertText Is Nothing Then
                sAlertText = smenu.AlertText
            End If
            Dim bAlertShow As Boolean = False
            If Not smenu.AlertShow Is Nothing Then
                bAlertShow = smenu.AlertShow
            End If
            sLog.AppendLine("Chiudo callback")
            SendResponseCallBack(chat_id, callback_id, sAlertText, bAlertShow, sError)
        End If

        If Not smenu.POI Is Nothing Then
            If Not smenu.POI.Text Is Nothing Then
                smenu.Text = smenu.POI.Text
            Else
                smenu.Text = ""
            End If

            Dim tempButton As List(Of TelegramMenu.Button)
            tempButton = CreaButtonPOI(smenu.POI)
            If Not smenu.Button Is Nothing Then
                For Each b In smenu.Button
                    tempButton.Add(b)
                Next
            End If
            smenu.Button = tempButton
        End If


        'Creo tasti di callback
        If Not smenu.Button Is Nothing Then
            If smenu.Button.Count > 0 Then
                listButton = CreaCallBack(smenu.Button)
            End If
        End If


        'Imposto parsemode
        Dim sParsemode As String = ""
        If Not smenu.ParseMode Is Nothing Then
            sParsemode = smenu.ParseMode
        End If

        'Recuperto testo in caso si TextFromURL
        If Not smenu.TextFromURL Is Nothing Then
            SendChatAction(chat_id, ActionChat.InvioTesto, sError) 'Invio scrittura in corso
            smenu.Text = TextFromURL(smenu.TextFromURL)
        End If


        'Codifico Emoji
        If Not smenu.Text Is Nothing Then
            smenu.Text = EmojiRead(smenu.Text)
        Else
            smenu.Text = ""
        End If

        'Nasconti tasti callback
        If Not smenu.HidePrevKeyb Is Nothing AndAlso smenu.HidePrevKeyb = True Then
            SendEditKeyboardTelegram(chat_id, message_id, sError)
        End If

        If Not smenu.Image Is Nothing Then
            sLog.AppendLine("Invio Immagine")
            SendImageTelegram(chat_id, smenu.Text, smenu.Image, listButton, sError)
        ElseIf Not smenu.ImageFromURL Is Nothing Then
            sLog.AppendLine("Invio Immagine da URL")
            SendChatAction(chat_id, ActionChat.InvioImmagine, sError)
            SendImageFromURLTelegram(chat_id, smenu.Text, smenu.ImageFromURL, listButton, sError)
        ElseIf Not smenu.Document Is Nothing Then
            sLog.AppendLine("Invio Documento")
            SendDocumentTelegram(chat_id, smenu.Text, smenu.Document, listButton, sError)
        ElseIf Not smenu.Audio Is Nothing Then
            sLog.AppendLine("Invio Audio")
            SendAudioTelegram(chat_id, smenu.Text, smenu.Audio, listButton, sError)
        ElseIf Not smenu.Video Is Nothing Then
            sLog.AppendLine("Invio Video")
            SendVideoTelegram(chat_id, smenu.Text, smenu.Video, listButton, sError)
        ElseIf Not smenu.Voice Is Nothing Then
            sLog.AppendLine("Invio Voice")
            SendVoiceTelegram(chat_id, smenu.Voice, listButton, sError)
        ElseIf Not smenu.Location Is Nothing Then
            sLog.AppendLine("Invio Location")
            SendLocationTelegram(chat_id, smenu.Location, listButton, sError)
        ElseIf Not smenu.Text Is Nothing Then
            sLog.AppendLine("Invio testo")

            If Not smenu.OneMessage Is Nothing AndAlso smenu.OneMessage = True Then
                SendEditMessageTelegram(chat_id, smenu.Text, listButton, sParsemode, message_id, sError)
            Else
                SendMessageTelegram(chat_id, smenu.Text, listButton, sParsemode, sError)
            End If
        End If

        sLog.AppendLine("Registro push")
        If Not smenu.Push Is Nothing Then
            Dim sPercorsoPush2 As String = Request.QueryString("bot").ToUpper + "/users/push/" + chat_id.ToString + "/" + smenu.Push.ID
            Dim sPercorsoPush As String = Request.QueryString("bot").ToUpper + "/push/" + smenu.Push.ID + "/" + chat_id.ToString
            sLog.AppendLine(sPercorsoPush)
            If smenu.Push.Enabled = True Then
                sLog.AppendLine("Registro push abilita")
                Firebase(ActionFirebase.SetData, sPercorsoPush, "{""Enabled"":true}")
                Firebase(ActionFirebase.SetData, sPercorsoPush2, "{""Enabled"":true}")
            Else
                sLog.AppendLine("Registro push disabilita")
                Firebase(ActionFirebase.DeleteData, sPercorsoPush, "")
                Firebase(ActionFirebase.DeleteData, sPercorsoPush2, "")
            End If
        End If

        If sError <> "" Then
            sLog.AppendLine.AppendLine(sError)
            sLog.AppendLine(Request.QueryString("bot").ToUpper)
            SendError(sLog.ToString)
        End If


    End Sub
    Function CreaCallBack(buttons As List(Of TelegramMenu.Button)) As String
        Dim listButton As New StringBuilder
        For Each y In buttons
            If listButton.ToString <> "" Then listButton.Append(",")
            If Not y.ID Is Nothing Then
                listButton.Append("[{""text"":""" & Server.UrlEncode(EmojiRead(y.Text.Replace("""", "\"""))) & """,""callback_data"":""" & Server.UrlEncode(y.ID) & """}]")
            Else
                If Not y.URL Is Nothing Then
                    listButton.Append("[{""text"":""" & Server.UrlEncode(EmojiRead(y.Text.Replace("""", "\"""))) & """,""url"":""" & Server.UrlEncode(y.URL) & """}]")
                End If
            End If
        Next
        Return listButton.ToString
    End Function
    Function TextFromURL(URL As String) As String
        Try
            Using w As New WebClient
                w.Encoding = Encoding.UTF8
                Return w.DownloadString(URL)
            End Using
        Catch ex As Exception
            Return "ERROR URL"
            SendError("TextFromURL" + ex.Message)
        End Try

    End Function
    Function SendResponseCallBack(chat_id As String, IdCallBack As String, sAlert As String, show As Boolean, ByRef sErrore As String) As Boolean
        Try
            sErrore = ""
            Dim sMessage As New StringBuilder
            sMessage.Append("https://api.telegram.org/bot" + Token + "/answerCallbackQuery?chat_id=").Append(chat_id)
            If IdCallBack.Length > 0 Then
                sMessage.Append("&callback_query_id=").Append(IdCallBack)
            End If
            If sAlert.Length > 0 Then
                sMessage.Append("&text=").Append(Server.UrlEncode(sAlert))
                If show = True Then
                    sMessage.Append("&show_alert=").Append("true")
                End If
            End If
            Dim sRisposta As String
            Using w As New WebClient
                w.Encoding = Encoding.UTF8
                sRisposta = w.DownloadString(sMessage.ToString)
            End Using
            Return True
        Catch ex As Exception
            sErrore = "SendResponseCallBack " & ex.Message
            Return False
        End Try
    End Function
    Function SendChatAction(chat_id As String, action As ActionChat, ByRef sErrore As String)
        Try
            sErrore = ""
            Dim sMessage As New StringBuilder
            sMessage.Append("https://api.telegram.org/bot" + Token + "/sendChatAction?chat_id=").Append(chat_id)
            Select Case action
                Case ActionChat.InvioTesto
                    sMessage.Append("&action=typing")
                Case ActionChat.InvioImmagine
                    sMessage.Append("&action=upload_photo")
                Case ActionChat.CercaLocazione
                    sMessage.Append("&action=find_location")
            End Select
            Dim sRisposta As String
            Using w As New WebClient
                w.Encoding = Encoding.UTF8
                sRisposta = w.DownloadString(sMessage.ToString)
            End Using
            Return True
        Catch ex As Exception
            sErrore = "SendChatAction " & ex.Message
            Return False
        End Try
    End Function
    Function SendMessageTelegram(chat_id As String, message As String, button As String, ParseMode As String, ByRef sErrore As String) As Boolean
        Dim sMessage As New StringBuilder
        Try
            sErrore = ""
            sMessage.Append("https://api.telegram.org/bot" + Token + "/sendMessage?chat_id=").Append(chat_id)
            sMessage.Append("&text=").Append(Server.UrlEncode(message))
            If ParseMode.Length > 0 Then
                sMessage.Append("&parse_mode=").Append(ParseMode)
            End If
            If button.Length > 0 Then
                sMessage.Append("&reply_markup={""inline_keyboard"":[")
                sMessage.Append(button).Append("]}")
            End If
            Dim sRisposta As String
            Using w As New WebClient
                w.Encoding = Encoding.UTF8
                'SendError(sMessage.ToString)
                sRisposta = w.DownloadString(sMessage.ToString)
            End Using
            Return True
        Catch ex As Exception
            sErrore = "SendMessageTelegram " + vbCrLf + sMessage.ToString + vbCrLf + ex.Message
            Return False
        End Try
    End Function
    Function SendEditKeyboardTelegram(chat_id As String, MessageID As String, ByRef sErrore As String) As Boolean
        Try
            sErrore = ""
            Dim sMessage As New StringBuilder
            sMessage.Append("https://api.telegram.org/bot" + Token + "/editMessageReplyMarkup?chat_id=").Append(chat_id)
            sMessage.Append("&message_id=").Append(MessageID)
            Dim sRisposta As String
            Using w As New WebClient
                w.Encoding = Encoding.UTF8
                'SendError(sMessage.ToString)
                sRisposta = w.DownloadString(sMessage.ToString)
            End Using
            Return True
        Catch ex As Exception
            sErrore = "SendEditKeyboardTelegram " & ex.Message
            Return False
        End Try
    End Function
    Function SendEditMessageTelegram(chat_id As String, message As String, button As String, ParseMode As String, MessageID As String, ByRef sErrore As String) As Boolean
        Try
            sErrore = ""
            Dim sMessage As New StringBuilder
            sMessage.Append("https://api.telegram.org/bot" + Token + "/editMessageText?chat_id=").Append(chat_id)
            sMessage.Append("&text=").Append(Server.UrlEncode(message))
            sMessage.Append("&message_id=").Append(MessageID)
            If ParseMode.Length > 0 Then
                sMessage.Append("&parse_mode=").Append(ParseMode)
            End If
            If button.Length > 0 Then
                sMessage.Append("&reply_markup={""inline_keyboard"":[")
                sMessage.Append(button).Append("]}")
            End If
            Dim sRisposta As String
            Using w As New WebClient
                w.Encoding = Encoding.UTF8
                'SendError(sMessage.ToString)
                sRisposta = w.DownloadString(sMessage.ToString)
            End Using
            Return True
        Catch ex As Exception
            sErrore = "SendEditMessageTelegram " & ex.Message
            Return False
        End Try
    End Function
    Function SendImageFromURLTelegram(chat_id As String, message As String, urlimage As String, button As String, ByRef sErrore As String) As Boolean

        Dim url = String.Format("https://api.telegram.org/bot{0}/sendPhoto", Token)

        'Download immagine
        Dim memoryimage As MemoryStream
        Using w As New WebClient
            memoryimage = New MemoryStream(w.DownloadData(urlimage))
        End Using

        Using form = New MultipartFormDataContent()
            'Inserimento parametri
            form.Add(New StringContent(chat_id, System.Text.Encoding.UTF8), "chat_id")
            form.Add(New StringContent("true", System.Text.Encoding.UTF8), "disable_notification")
            If message <> "" Then
                form.Add(New StringContent(message, System.Text.Encoding.UTF8), "caption")
            End If
            form.Add(New StreamContent(memoryimage), "photo", "immagine.png")
            If button.Length > 0 Then
                form.Add(New StringContent("{""inline_keyboard"":[" + Server.UrlDecode(button) + "]}", System.Text.Encoding.UTF8), "reply_markup")
            End If

            'invio richiesta
            Using client = New HttpClient()
                Dim result = client.PostAsync(url, form).Result
                Dim s As String = result.Content.ReadAsStringAsync.Result
                If result.IsSuccessStatusCode = False Then
                    sErrore = result.Content.ReadAsStringAsync.Result
                End If
                Return result.IsSuccessStatusCode
            End Using


        End Using

    End Function

    Function SendImageTelegram(chat_id As String, message As String, image As String, button As String, ByRef sErrore As String) As Boolean
        Try
            sErrore = ""
            Dim sMessage As New StringBuilder
            sMessage.Append("https://api.telegram.org/bot" + Token + "/sendPhoto?chat_id=").Append(chat_id)
            If message <> "" Then
                sMessage.Append("&caption=").Append(Server.UrlEncode(message))
            End If
            sMessage.Append("&photo=").Append(image)
            If button.Length > 0 Then
                sMessage.Append("&reply_markup={""inline_keyboard"":[")
                sMessage.Append(button).Append("]}")
            End If
            Dim sRisposta As String
            Using w As New WebClient
                w.Encoding = Encoding.UTF8
                'SendError(sMessage.ToString)
                sRisposta = w.DownloadString(sMessage.ToString)
            End Using
            Return True
        Catch ex As Exception
            sErrore = "SendImageTelegram " & ex.Message
            Return False
        End Try
    End Function
    Function SendVideoTelegram(chat_id As String, message As String, video As String, button As String, ByRef sErrore As String) As Boolean
        Try
            sErrore = ""
            Dim sMessage As New StringBuilder
            sMessage.Append("https://api.telegram.org/bot" + Token + "/sendVideo?chat_id=").Append(chat_id)
            If message <> "" Then
                sMessage.Append("&caption=").Append(Server.UrlEncode(message))
            End If
            sMessage.Append("&video=").Append(video)
            If button.Length > 0 Then
                sMessage.Append("&reply_markup={""inline_keyboard"":[")
                sMessage.Append(button).Append("]}")
            End If
            Dim sRisposta As String
            Using w As New WebClient
                w.Encoding = Encoding.UTF8
                'SendError(sMessage.ToString)
                sRisposta = w.DownloadString(sMessage.ToString)
            End Using
            Return True
        Catch ex As Exception
            sErrore = "SendVideoTelegram " & ex.Message
            Return False
        End Try
    End Function
    Function SendVoiceTelegram(chat_id As String, voice As String, button As String, ByRef sErrore As String) As Boolean
        Try
            sErrore = ""
            Dim sMessage As New StringBuilder
            sMessage.Append("https://api.telegram.org/bot" + Token + "/sendVoice?chat_id=").Append(chat_id)
            sMessage.Append("&voice=").Append(voice)
            If button.Length > 0 Then
                sMessage.Append("&reply_markup={""inline_keyboard"":[")
                sMessage.Append(button).Append("]}")
            End If
            Dim sRisposta As String
            Using w As New WebClient
                w.Encoding = Encoding.UTF8
                'SendError(sMessage.ToString)
                sRisposta = w.DownloadString(sMessage.ToString)
            End Using
            Return True
        Catch ex As Exception
            sErrore = "SendVoiceTelegram " & ex.Message
            Return False
        End Try
    End Function
    Function SendLocationTelegram(chat_id As String, location As TelegramMenu.Location, button As String, ByRef sErrore As String) As Boolean
        Try
            sErrore = ""
            Dim sMessage As New StringBuilder
            sMessage.Append("https://api.telegram.org/bot" + Token + "/sendLocation?chat_id=").Append(chat_id)
            sMessage.Append("&latitude=").Append(location.Latitude)
            sMessage.Append("&longitude=").Append(location.Longitude)
            If button.Length > 0 Then
                sMessage.Append("&reply_markup={""inline_keyboard"":[")
                sMessage.Append(button).Append("]}")
            End If
            Dim sRisposta As String
            Using w As New WebClient
                w.Encoding = Encoding.UTF8
                'SendError(sMessage.ToString)
                sRisposta = w.DownloadString(sMessage.ToString)
            End Using
            Return True
        Catch ex As Exception
            sErrore = "SendVoiceTelegram " & ex.Message
            Return False
        End Try
    End Function
    Function SendDocumentTelegram(chat_id As String, message As String, document As String, button As String, ByRef sErrore As String) As Boolean
        Try
            sErrore = ""
            Dim sMessage As New StringBuilder
            sMessage.Append("https://api.telegram.org/bot" + Token + "/sendDocument?chat_id=").Append(chat_id)
            If message <> "" Then
                sMessage.Append("&caption=").Append(Server.UrlEncode(message))
            End If
            sMessage.Append("&document=").Append(document)
            If button.Length > 0 Then
                sMessage.Append("&reply_markup={""inline_keyboard"":[")
                sMessage.Append(button).Append("]}")
            End If
            Dim sRisposta As String
            Using w As New WebClient
                w.Encoding = Encoding.UTF8
                sRisposta = w.DownloadString(sMessage.ToString)
            End Using
            Return True
        Catch ex As Exception
            sErrore = "SendDocumentTelegram " & ex.Message
            Return False
        End Try
    End Function
    Function SendAudioTelegram(chat_id As String, message As String, audio As String, button As String, ByRef sErrore As String) As Boolean
        Try
            sErrore = ""
            Dim sMessage As New StringBuilder
            sMessage.Append("https://api.telegram.org/bot" + Token + "/sendAudio?chat_id=").Append(chat_id)
            If message <> "" Then
                sMessage.Append("&caption=").Append(Server.UrlEncode(message))
            End If
            sMessage.Append("&audio=").Append(audio)
            If button.Length > 0 Then
                sMessage.Append("&reply_markup={""inline_keyboard"":[")
                sMessage.Append(button).Append("]}")
            End If
            Dim sRisposta As String
            Using w As New WebClient
                w.Encoding = Encoding.UTF8
                sRisposta = w.DownloadString(sMessage.ToString)
            End Using
            Return True
        Catch ex As Exception
            sErrore = "SendAudioTelegram " & ex.Message
            Return False
        End Try
    End Function
    Function EmojiRead(sText As String) As String
        If Not Emojis Is Nothing Then
            For Each e In Emojis
                sText = sText.Replace(e.Shortcode, e.Icon)
            Next
            Return sText
        Else
            Return sText
        End If
    End Function
    Sub SendError(sLog As String)
        Dim msg = New MailMessage()
        msg.From = New MailAddress(ConfigAdmin.EmailFrom)
        msg.To.Add(ConfigAdmin.EmailTo)
        msg.Subject = "BOT Telegram - Error"
        msg.IsBodyHtml = False
        msg.Body = sLog
        Dim smtp = New SmtpClient(ConfigAdmin.EmailSMTP)
        smtp.Credentials = New System.Net.NetworkCredential(ConfigAdmin.EmailUserName, ConfigAdmin.EmailPassword)
        smtp.Send(msg)
    End Sub
    Function Firebase(action As ActionFirebase, path As String, data As String) As String

        Dim configProfile As New FirebaseConfig With {.AuthSecret = ConfigAdmin.FirebaseAuthSecret, .BasePath = ConfigAdmin.FirebaseBasePath}
        Dim dbProfile As New FireSharp.FirebaseClient(configProfile)

        Select Case action
            Case ActionFirebase.PushData
                dbProfile.Push(path, data)
                Return ""
            Case ActionFirebase.SetData
                dbProfile.Set(path, data)
                Return ""
            Case ActionFirebase.DeleteData
                dbProfile.Delete(path)
                Return ""
            Case ActionFirebase.GetData
                Dim s As String = dbProfile.Get(path).Body
                Return s
            Case Else
                Return ""
        End Select
    End Function
    Enum ActionFirebase
        PushData
        SetData
        DeleteData
        GetData
    End Enum
    Enum ActionChat
        InvioTesto
        InvioImmagine
        CercaLocazione
    End Enum
    Function CreaButtonPOI(pois As TelegramMenu.POI) As List(Of TelegramMenu.Button)



        Dim s As String = Firebase(ActionFirebase.GetData, pathProfile, "")
        s = s.Replace("""", "").Replace("\", """")
        Dim pr As UserProfile = JsonConvert.DeserializeObject(Of UserProfile)(s)


        Dim rs As New ResultPOI
        rs.Results = New List(Of Result)
        For Each p In pois.ListPOI
            Dim dist As Double = (New Distance).Calcola(pr.Latitude, pr.Longitude, p.Latitude, p.Longitude, "K")
            If dist * 1000 <= pois.MaxDistance Then
                rs.Results.Add(New Result With {.ID = p.ID, .Text = p.Text, .Distance = dist, .Latitude = p.Latitude, .Longitude = p.Longitude})
            End If

        Next


        Dim sMap As New StringBuilder
        sMap.Append("https://maps.googleapis.com/maps/api/staticmap?size=300x300&maptype=roadmap&markers=color:blue%7C")
        sMap.Append(pr.Latitude).Append(",").Append(pr.Longitude)

        Dim z = From f In rs.Results Order By f.Distance Take pois.MaxResult
        Dim btn As New List(Of TelegramMenu.Button)
        Dim nLabel As Integer = 0
        For Each b In z
            If Not pois.SendMap Is Nothing AndAlso pois.SendMap = True Then
                b.Text = "(" + Chr(65 + nLabel) + ") " + b.Text
            End If

            If Not pois.SendDistance Is Nothing AndAlso pois.SendDistance = True Then
                b.Text = b.Text + " " + StringaDistanza(b.Distance)
            End If
            btn.Add(New TelegramMenu.Button With {.ID = b.ID, .Text = b.Text})
            sMap.Append("&markers=color:red%7Clabel:" + Chr(65 + nLabel) + "%7C").Append(b.Latitude).Append(",").Append(b.Longitude)
            nLabel += 1
        Next

        If Not pois.SendMap Is Nothing AndAlso pois.SendMap = True Then
            Dim sErrore As String = ""
            SendImageFromURLTelegram(pr.ChatID, "", sMap.ToString, "", sErrore)
        End If



        Return btn
    End Function
    Function StringaDistanza(n As Double) As String
        If n > 1 Then
            Return n.ToString("0.0") + " km"
        Else
            Return (n * 1000).ToString(0) + " m"
        End If
    End Function
End Class


