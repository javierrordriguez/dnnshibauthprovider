'
'
' UF Office of Research
' Copyright (c) 2010
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'


Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Log.EventLog

Imports System.Web




Namespace UF.Research.Authentication.Shibboleth.SHIB

    Public Module Globals

#Region "Public Constants"
        Public Const ShibAlert As String = "SHIB_ALERT"
#End Region

    End Module

    Public Class Utilities


        Sub New()
        End Sub
        Public Shared Function LogTypeKeyInstalled(ByVal logTypeKey As String) As Boolean
            Dim eventLogController = New DotNetNuke.Services.Log.EventLog.EventLogController()
            'Dim logTypes = New List(Of DotNetNuke.Services.Log.EventLog.LogTypeInfo)(eventLogController.GetLogTypeConfigInfo().Cast(Of DotNetNuke.Services.Log.EventLog.LogTypeInfo)())
            Dim result As Boolean = False
            Try
                Dim logType As LogTypeConfigInfo = eventLogController.GetLogTypeConfigInfoByID(logTypeKey)
                If logType IsNot Nothing Then
                    result = True
                End If
            Catch
                result = False
            End Try

            Return result
        End Function
        Public Shared Function CheckShibEventLogging() As Boolean

            'http://msdn.microsoft.com/en-us/library/x0b5b5bc.aspx

            If Not LogTypeKeyInstalled(ShibAlert) Then
                Return False
            Else
                Dim logTypeConfigInfo As DotNetNuke.Services.Log.EventLog.LogTypeConfigInfo
                Dim eventLogController = New DotNetNuke.Services.Log.EventLog.EventLogController()
                Dim logConfigTypes = New List(Of DotNetNuke.Services.Log.EventLog.LogTypeConfigInfo)(eventLogController.GetLogTypeConfigInfo().Cast(Of DotNetNuke.Services.Log.EventLog.LogTypeConfigInfo)())
                logTypeConfigInfo = logConfigTypes.Find(AddressOf FindLogConfigID)
                Return logTypeConfigInfo.LoggingIsActive
            End If
        End Function

        Public Shared Function FindLogConfigID(ByVal logInfo As DotNetNuke.Services.Log.EventLog.LogTypeConfigInfo) As Boolean
            If logInfo.LogTypeKey = ShibAlert Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Shared Function GetLoggedOnUserName() As String

            Dim _config As ShibConfiguration = ShibConfiguration.GetConfig()

            Dim blnSimulateShibLogin As Boolean
            blnSimulateShibLogin = _config.SimulateLogin

            Dim sh As ShibHandler = New ShibHandler

            Return sh.userName

        End Function


        Public Shared Function GetUser() As ShibUserInfo
            Dim objUserInfo As ShibUserInfo = New ShibUserInfo

            Return objUserInfo

        End Function

        Public Shared Function CheckNullString(ByVal value As Object) As String
            If value Is Nothing Then
                Return ""
            Else
                Return value.ToString
            End If
        End Function

        Public Shared Function GetRandomPassword() As String
            Dim rd As New System.Random
            Return Convert.ToString(rd.Next)
        End Function


        'Public Shared Function GetIP4Address(ByVal strPassedIP As String) As String
        '    Dim IP4Address As String = String.Empty

        '    For Each IPA As IPAddress In Dns.GetHostAddresses(strPassedIP)
        '        If IPA.AddressFamily.ToString() = "InterNetwork" Then
        '            IP4Address = IPA.ToString()
        '            Exit For
        '        End If
        '    Next

        '    If IP4Address <> String.Empty Then
        '        Return IP4Address
        '    End If

        '    For Each IPA As IPAddress In Dns.GetHostAddresses(Dns.GetHostName())
        '        If IPA.AddressFamily.ToString() = "InterNetwork" Then
        '            IP4Address = IPA.ToString()
        '            Exit For
        '        End If
        '    Next

        '    Return IP4Address
        'End Function

        Public Shared Function GetCurrentTrustLevel() As AspNetHostingPermissionLevel
            For Each trustLevel As AspNetHostingPermissionLevel In New AspNetHostingPermissionLevel() {AspNetHostingPermissionLevel.Unrestricted, AspNetHostingPermissionLevel.High, AspNetHostingPermissionLevel.Medium, AspNetHostingPermissionLevel.Low, AspNetHostingPermissionLevel.Minimal}
                Try
                    Dim perm As New AspNetHostingPermission(trustLevel)
                    perm.Demand()
                Catch generatedExceptionName As System.Security.SecurityException
                    Continue For
                End Try

                Return trustLevel
            Next

            Return AspNetHostingPermissionLevel.None
        End Function

    End Class
End Namespace
