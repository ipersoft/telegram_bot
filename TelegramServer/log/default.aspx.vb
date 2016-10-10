Imports FireSharp.Config
Imports Newtonsoft.Json

Public Class log
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim ConfigAdmin As AdminJSON
        Try
            ConfigAdmin = JsonConvert.DeserializeObject(Of AdminJSON)(My.Computer.FileSystem.ReadAllText(Server.MapPath("../admin.json")))
        Catch ex As Exception
            Exit Sub
        End Try

        Dim txtJson As String
        txtJson = My.Computer.FileSystem.ReadAllText(Server.MapPath("../config.json"))

        Dim cBots As New List(Of BotConfig)
        cBots = JsonConvert.DeserializeObject(Of List(Of BotConfig))(txtJson)

        If Not Request.QueryString("token") Is Nothing Then
            For Each cBot In cBots
                If cBot.TOKEN = Request.QueryString("token") Then
                    Dim configProfile As New FirebaseConfig With {.AuthSecret = ConfigAdmin.FirebaseAuthSecret, .BasePath = ConfigAdmin.FirebaseBasePath}
                    Dim dbProfile As New FireSharp.FirebaseClient(configProfile)
                    Response.Write(dbProfile.Get(cBot.BOT).Body)
                End If
            Next
        Else
            Exit Sub
        End If
    End Sub

End Class