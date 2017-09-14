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

Imports System.Web


Imports DNNUserInfo = DotNetNuke.Entities.Users.UserInfo
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals

Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Security
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Services.Authentication







Namespace UF.Research.Authentication.Shibboleth

    Public Class ShibbolethLogin
        Inherits DotNetNuke.Services.Authentication.AuthenticationLoginBase


        Protected Property LoginStatus() As UserLoginStatus
            Get
                Dim _LoginStatus As UserLoginStatus = UserLoginStatus.LOGIN_FAILURE
                If Not ViewState("LoginStatus") Is Nothing Then
                    _LoginStatus = CType(ViewState("LoginStatus"), UserLoginStatus)
                End If
                Return _LoginStatus
            End Get
            Set(ByVal value As UserLoginStatus)
                ViewState("LoginStatus") = value
            End Set
        End Property



        Private Shared Sub AddEventLog(ByVal portalId As Integer, ByVal username As String, ByVal userId As Integer, ByVal portalName As String, ByVal Ip As String, ByVal loginStatus As UserLoginStatus)

            Dim objEventLog As New DotNetNuke.Services.Log.EventLog.EventLogController

            ' initialize log record
            Dim objEventLogInfo As New DotNetNuke.Services.Log.EventLog.LogInfo
            Dim objSecurity As New PortalSecurity
            objEventLogInfo.AddProperty("IP", Ip)
            objEventLogInfo.LogPortalID = portalId
            objEventLogInfo.LogPortalName = portalName
            objEventLogInfo.LogUserName = objSecurity.InputFilter(username, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoAngleBrackets Or PortalSecurity.FilterFlag.NoMarkup)
            objEventLogInfo.LogUserID = userId

            ' create log record
            objEventLogInfo.LogTypeKey = loginStatus.ToString
            objEventLog.AddLog(objEventLogInfo)

        End Sub

#Region "Public Properties"

        Public Overrides ReadOnly Property Enabled() As Boolean
            Get
                Try
                    'Make sure app is running at full trust
                    Dim HostingPermissions As New AspNetHostingPermission(System.Security.Permissions.PermissionState.Unrestricted)
                    HostingPermissions.Demand()

                    'Check if Windows Auth is enabled for the portal
                    Return ShibConfiguration.GetConfig().Enabled
                Catch ex As Exception
                    Return False
                End Try
            End Get
        End Property

#End Region

        Public Sub Login()

            Dim LoginStatus As AuthenticationStatus

            Dim Request As HttpRequest = HttpContext.Current.Request
            Dim Response As HttpResponse = HttpContext.Current.Response

            Dim portalID As Integer

            Dim objPortalSettings As PortalSettings = Nothing
            objPortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            If objPortalSettings Is Nothing Then Exit Sub
            portalID = objPortalSettings.PortalId

            'Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

            Dim config As ShibConfiguration = ShibConfiguration.GetConfig()
            If config Is Nothing Then
                Exit Sub
            End If

            Dim blnSimulateLogin As Boolean = config.SimulateLogin

            Dim prjSettings As UF.Research.Authentication.Shibboleth.ProjectSettings = New ProjectSettings
            Dim slnPath As String = prjSettings.slnPath

            Dim ipAddress As String = ""

            Dim sh As ShibHandler = New ShibHandler
            Dim UserName As String = sh.userName

            If UserName Is Nothing Or UserName = "" Then

            Else

                Dim objAuthentication As ShibAuthController = New ShibAuthController
                Dim objUser As DNNUserInfo = objAuthentication.ManualLogon(UserName, LoginStatus, ipAddress)

                'cb_050411 - don't allow login if doing simulation and user is superuser
                If Not blnSimulateLogin Or blnSimulateLogin And objUser.IsSuperUser = False Then

                    LoginStatus = UserLoginStatus.LOGIN_SUCCESS
                    'Dim testUserName As String = UserName + CType(DateTime.Now, String)

                    Dim authenticated As Boolean = Null.NullBoolean
                    Dim message As String = Null.NullString
                    authenticated = (LoginStatus <> UserLoginStatus.LOGIN_FAILURE)

                    If objUser Is Nothing Then
                        AddEventLog(portalID, UserName, Null.NullInteger, objPortalSettings.PortalName, ipAddress, LoginStatus)
                    Else

                        objAuthentication.AuthenticationLogon()
                        Dim eventArgs As UserAuthenticatedEventArgs = New UserAuthenticatedEventArgs(objUser, UserName, LoginStatus, "Shibboleth")
                        eventArgs.Authenticated = authenticated
                        eventArgs.Message = message
                        OnUserAuthenticated(eventArgs)

                    End If

                End If
                'cb_050411

            End If

        End Sub

    End Class

End Namespace

