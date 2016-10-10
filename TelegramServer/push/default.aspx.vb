Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports FireSharp.Config
Imports Newtonsoft.Json

Public Class push
    Inherits System.Web.UI.Page
    Dim Emojis As List(Of Emoji)
    Dim ConfigAdmin As AdminJSON
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        Try
            ConfigAdmin = JsonConvert.DeserializeObject(Of AdminJSON)(My.Computer.FileSystem.ReadAllText(Server.MapPath("../admin.json")))
        Catch ex As Exception
            Exit Sub
        End Try

        Try
            Dim sLog As New StringBuilder
            sLog.AppendLine("Token:" + Request.QueryString("token"))
            sLog.AppendLine("Categoria:" + Request.QueryString("cat"))
            sLog.AppendLine("Testo:" + Request.QueryString("text"))
            SendError(sLog.ToString)
        Catch ex As Exception
            SendError(ex.Message)
        End Try

        Dim documentContents As String
        Try
            Using receiveStream As Stream = Request.InputStream
                Using readStream As New StreamReader(receiveStream, Encoding.UTF8)
                    documentContents = readStream.ReadToEnd()
                End Using
            End Using
            SendError(documentContents)
        Catch ex As Exception
            SendError("Recupero messaggio telegram" + vbCrLf + ex.Message)
            Exit Sub
        End Try




        Dim txtJson As String
        txtJson = My.Computer.FileSystem.ReadAllText(Server.MapPath("../config.json"))

        Dim cBots As New List(Of BotConfig)
        cBots = JsonConvert.DeserializeObject(Of List(Of BotConfig))(txtJson)

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


        Dim sMessage As String = ""
        If Not Request.QueryString("text") Is Nothing Then
            sMessage = Request.QueryString("text")
        End If
        If Not Request.QueryString("textfromurl") Is Nothing Then
            sMessage = TextFromURL(Request.QueryString("textfromurl"))
        End If

        If sMessage = "" Then Exit Sub

        If Not Request.QueryString("token") Is Nothing Then
            For Each cBot In cBots
                If cBot.TOKEN = Request.QueryString("token") Then
                    If Not Request.QueryString("cat") Is Nothing Then

                        Dim configProfile As New FirebaseConfig With {.AuthSecret = ConfigAdmin.FirebaseAuthSecret, .BasePath = ConfigAdmin.FirebaseBasePath}
                        Dim dbProfile As New FireSharp.FirebaseClient(configProfile)
                        Dim sPush As String = dbProfile.Get(cBot.BOT + "/push/" + Request.QueryString("cat")).Body
                        Try
                            Dim items As Linq.JObject = JsonConvert.DeserializeObject(sPush)
                            For Each item In items
                                Try
                                    sMessage = EmojiRead(sMessage)
                                    SendMessageTelegram(item.Key, sMessage, cBot.TOKEN)
                                Catch ex As Exception

                                End Try
                            Next
                        Catch ex As Exception

                        End Try

                    End If
                End If
            Next
        Else
            Exit Sub
        End If
    End Sub
    Function SendMessageTelegram(chat_id As String, message As String, Token As String) As Boolean
        Try
            Dim sMessage As New StringBuilder
            sMessage.Append("https://api.telegram.org/bot" + Token + "/sendMessage?chat_id=").Append(chat_id)
            sMessage.Append("&text=").Append(Server.UrlEncode(message))
            Dim sRisposta As String
            Using w As New WebClient
                w.Encoding = Encoding.UTF8
                sRisposta = w.DownloadString(sMessage.ToString)
            End Using
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Function TextFromURL(URL As String) As String
        Try
            Using w As New WebClient
                w.Encoding = Encoding.UTF8
                Return w.DownloadString(Server.UrlDecode(URL))
            End Using
        Catch ex As Exception
            Return ""
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
        Try
            Dim msg = New MailMessage()
            msg.From = New MailAddress(ConfigAdmin.EmailFrom)
            msg.To.Add(ConfigAdmin.EmailTo)
            msg.Subject = "BOT Telegram - Push"
            msg.IsBodyHtml = False
            msg.Body = sLog
            Dim smtp = New SmtpClient(ConfigAdmin.EmailSMTP)
            smtp.Credentials = New System.Net.NetworkCredential(ConfigAdmin.EmailUserName, ConfigAdmin.EmailPassword)
            smtp.Send(msg)
        Catch ex As Exception

        End Try
    End Sub
End Class