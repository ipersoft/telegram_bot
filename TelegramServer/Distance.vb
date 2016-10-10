Public Class Distance
    ':::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    ':::                                                                         :::
    ':::  This routine calculates the distance between two points (given the     :::
    ':::  latitude/longitude of those points). It is being used to calculate     :::
    ':::  the distance between two locations using GeoDataSource (TM) prodducts  :::
    ':::                                                                         :::
    ':::  Definitions:                                                           :::
    ':::    South latitudes are negative, east longitudes are positive           :::
    ':::                                                                         :::
    ':::  Passed to function:                                                    :::
    ':::    lat1, lon1 = Latitude and Longitude of point 1 (in decimal degrees)  :::
    ':::    lat2, lon2 = Latitude and Longitude of point 2 (in decimal degrees)  :::
    ':::    unit = the unit you desire for results                               :::
    ':::           where: 'M' is statute miles (default)                         :::
    ':::                  'K' is kilometers                                      :::
    ':::                  'N' is nautical miles                                  :::
    ':::                                                                         :::
    ':::  Worldwide cities and other features databases with latitude longitude  :::
    ':::  are available at http://www.geodatasource.com                          :::
    ':::                                                                         :::
    ':::  For enquiries, please contact sales@geodatasource.com                  :::
    ':::                                                                         :::
    ':::  Official Web site: http://www.geodatasource.com                        :::
    ':::                                                                         :::
    ':::              GeoDataSource.com (C) All Rights Reserved 2015             :::
    ':::                                                                         :::
    ':::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    Public Function Calcola(ByVal lat1 As Double, ByVal lon1 As Double, ByVal lat2 As Double, ByVal lon2 As Double, ByVal unit As Char) As Double
        Dim theta As Double = lon1 - lon2
        Dim dist As Double = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta))
        dist = Math.Acos(dist)
        dist = rad2deg(dist)
        dist = dist * 60 * 1.1515
        If unit = "K" Then
            dist = dist * 1.609344
        ElseIf unit = "N" Then
            dist = dist * 0.8684
        End If
        Return dist
    End Function

    Private Function deg2rad(ByVal deg As Double) As Double
        Return (deg * Math.PI / 180.0)
    End Function

    Private Function rad2deg(ByVal rad As Double) As Double
        Return rad / Math.PI * 180.0
    End Function
End Class
