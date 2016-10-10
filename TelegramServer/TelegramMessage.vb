Namespace TelegramMessage
    Public Class From
        Public Property id As Integer
        Public Property first_name As String
        Public Property last_name As String
        Public Property username As String
    End Class

    Public Class Chat
        Public Property id As Integer
        Public Property first_name As String
        Public Property last_name As String
        Public Property username As String
        Public Property type As String
    End Class

    Public Class Entity
        Public Property type As String
        Public Property offset As Integer
        Public Property length As Integer
    End Class
    Public Class Document
        Public Property file_name As String
        Public Property mime_type As String
        Public Property file_id As String
        Public Property file_size As Integer
    End Class
    Public Class Photo
        Public Property file_id As String
        Public Property file_size As Integer
        Public Property width As Integer
        Public Property height As Integer
    End Class
    Public Class Voice
        Public Property duration As Integer
        Public Property mime_type As String
        Public Property file_id As String
        Public Property file_size As Integer
    End Class
    Public Class Location
        Public Property latitude As Double
        Public Property longitude As Double
    End Class
    Public Class Thumb
        Public Property file_id As String
        Public Property file_size As Integer
        Public Property width As Integer
        Public Property height As Integer
    End Class
    Public Class Video
        Public Property duration As Integer
        Public Property width As Integer
        Public Property height As Integer
        Public Property thumb As Thumb
        Public Property file_id As String
        Public Property file_size As Integer
    End Class
    Public Class Audio
        Public Property duration As Integer
        Public Property mime_type As String
        Public Property title As String
        Public Property file_id As String
        Public Property file_size As Integer
    End Class
    Public Class Message
        Public Property message_id As Integer
        Public Property from As From
        Public Property chat As Chat
        Public Property [date] As Integer
        Public Property text As String
        Public Property entities As List(Of Entity)
        Public Property document As Document
        Public Property photo As List(Of Photo)
        Public Property voice As Voice
        Public Property location As Location
        Public Property video As Video
        Public Property audio As Audio
    End Class

    Public Class CallbackQuery
        Public Property id As String
        Public Property from As From
        Public Property message As Message
        Public Property data As String
    End Class

    Public Class TMessage
        Public Property update_id As Integer
        Public Property message As Message
        Public Property callback_query As CallbackQuery
    End Class
End Namespace

