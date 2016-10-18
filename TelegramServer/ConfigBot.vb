Imports Newtonsoft.Json

Public Class BotConfig
    Public Property BOT As String
    Public Property LINK As String
    Public Property TOKEN As String
End Class
Public Class UserProfile
    Public Property ChatID As Integer
    Public Property LastCommand As String
    Public Property LastUpdate As Date
    Public Property Latitude As Double
    Public Property Longitude As Double
End Class
Public Class PushMessage
    Public Property Item As String
    Public Property Enabled As Boolean
End Class
Public Class StatCallBack

End Class
Public Class AdminJSON
    Public Property FirebaseAuthSecret As String
    Public Property FirebaseBasePath As String
    Public Property EmailFrom As String
    Public Property EmailTo As String
    Public Property EmailSMTP As String
    Public Property EmailUserName As String
    Public Property EmailPassword As String
End Class
Public Class Emoji
    Public Property Descrizione As String
    Public Property Shortcode As String
    Public Property Icon As String
End Class
Public Class ResultPOI
    Public Results As List(Of Result)
End Class
Public Class Result
    Public ID As String
    Public Text As String
    Public Distance As Double
    Public Property Latitude As Double
    Public Property Longitude As Double
End Class
Public Class ButtonCallback
    Public Property text As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property callback_data As String
    <JsonProperty(NullValueHandling:=NullValueHandling.Ignore)>
    Public Property url As String
End Class
Public Class ConfigBOT
    Property TOKEN As String
    Property PATH As String
End Class