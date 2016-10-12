
Namespace TelegramMenu
    Public Class Button
        Public Property Text As String
        Public Property ID As String
        Public Property URL As String
    End Class
    Public Class Push
        Public Property ID As String
        Public Property Enabled As Boolean
    End Class
    Public Class Location
        Public Property Latitude As Double
        Public Property Longitude As Double
    End Class
    Public Class ListPOI
        Public Property ID As String
        Public Property Text As String
        Public Property Latitude As Double
        Public Property Longitude As Double
    End Class

    Public Class POI
        Public Property Text As String
        Public Property TextNoResult As String
        Public Property MaxResult As Integer
        Public Property MaxDistance As Integer
        Public Property DistanceOnText As Boolean?
        Public Property DistanceOnButton As Boolean?
        Public Property POITextOnText As Boolean?
        Public Property POITextOnButton As Boolean?
        Public Property LabelOnText As Boolean?
        Public Property LabelOnButton As Boolean?
        Public Property SendMap As Boolean?
        Public Property ListPOI As List(Of ListPOI)
    End Class
    Public Class TMenu
        Public Property ID As String
        Public Property IDNext As String
        Public Property ParseMode As String
        Public Property Text As String
        Public Property TextFromURL As String
        Public Property Image As String
        Public Property ImageFromURL As String
        Public Property Document As String
        Public Property Audio As String
        Public Property Video As String
        Public Property Voice As String
        Public Property Button As List(Of Button)
        Public Property AlertText As String
        Public Property AlertShow As Boolean?
        Public Property Push As Push
        Public Property OneMessage As Boolean?
        Public Property HidePrevKeyb As Boolean?
        Public Property Location As Location
        Public Property POI As POI
    End Class
End Namespace
