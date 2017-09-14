Imports Microsoft.VisualBasic
'
' UF Office of Research
' Copyright (c) 2010
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

'testing muliple portal notes: 
'open your browser and delete all cookies so you start with no shib cookie, no dnn cookies
'if it's been awhile though, over half an hour and your shib cookie has expired, you will need to relogin again. 


Imports System
Imports System.Web
Imports System.Web.HttpServerUtility
Imports System.Web.UI.Control
Imports System.Web.UI
Imports DotNetNuke
Imports DotNetNuke.Security
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Entities.Portals
Imports UF.Research.Authentication.Shibboleth
Imports System.Net
Imports System.Security
Imports DNNUserInfo = DotNetNuke.Entities.Users.UserInfo
Imports DotNetNuke.Security.Membership
'Imports System.Web.Services
Imports System.Collections.Specialized
Imports System.Web.Security
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DNNUserController = DotNetNuke.Entities.Users.UserController
Imports DotNetNuke.Services.Authentication
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Skins.Controls.ModuleMessage
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Services.Exceptions
Imports System.Data
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.Entities.Users
Imports System.Collections.Generic
Imports System.IO
Imports DotNetNuke.Services.Messaging.Data
Imports DotNetNuke.Services.Mail
Imports DotNetNuke.UI.UserControls
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.Entities.Tabs
Imports UF.Research.Authentication.Shibboleth.SHIB

Namespace UF.Research.Authentication.Shibboleth.HttpModules

    Public Class AuthenticationModule
        Implements IHttpModule

        'can't initialize _portalSettings here because it's too early in the DNN startup process 

        Public ReadOnly Property ModuleName() As String

            Get
                Return "AuthenticationModule"
            End Get
        End Property

        Public Sub Init(ByVal application As HttpApplication) Implements IHttpModule.Init

            Try
                AddHandler application.AuthenticateRequest, AddressOf Me.OnAuthenticateRequest
                'can't set portalSettings here because they don't exist yet
                'it's also too early to set ShibConfiguration
                '_portalSettings = DotNetNuke.Common.GetPortalSettings
                '_shibConfiguration = ShibConfiguration.GetConfig()
            Catch
                Exit Sub
            Finally
            End Try

        End Sub

        Public Sub OnAuthenticateRequest(ByVal s As Object, ByVal e As EventArgs)

            Dim _portalSettings As PortalSettings = DotNetNuke.Common.GetPortalSettings
            Dim objEventLog As New DotNetNuke.Services.Log.EventLog.EventLogController

            objEventLog.AddLog("AuthenticationModule_OnAuthenticateRequest0", "Entering OnAuthenticationRequest0", _portalSettings, -1, ShibAlert)

            Dim Request As HttpRequest = HttpContext.Current.Request
            Dim Response As HttpResponse = HttpContext.Current.Response

            ''check if we are upgrading/installing/using a web service/rss feeds (ACD-7748)
            'Abort if NOT Default.aspx

            'cb_1005'
            'If Request.Url.LocalPath Is Nothing Or Request.RawUrl Is Nothing Then
            If Request.Url.LocalPath Is Nothing Or Request.RawUrl Is Nothing Or Request.RawUrl = "/" Then
                'cb_1005'
                objEventLog.AddLog("AuthenticationModule_OnAuthenticateRequest0.2", "null request values", _portalSettings, -1, ShibAlert)
                Exit Sub
            Else

                If Not Request.Url.LocalPath.ToLower.EndsWith("default.aspx") _
                    OrElse (Request.RawUrl.ToLower.Contains("rssid")) Then
                    Exit Sub
                End If

                objEventLog.AddLog("AuthenticationModule_OnAuthenticateRequest4", Request.Url.LocalPath.ToLower, _portalSettings, -1, ShibAlert)

            End If

            If Request.ServerVariables("HTTP_USER_AGENT").Contains("gsa-crawler") Then
                Exit Sub
            End If


            'comment these out for testing since they cause more iterations thru AuthenticationModule.vb
            'If InStr(Request.RawUrl, "ScriptResource.axd") > 0 Or InStr(Request.RawUrl, "WebResource.axd") > 0 Then
            '    Exit Sub
            'End If

            'get the solution path
            Dim reDir As String
            'Dim slnPath As String = GetSolutionPath()

            If InStr(Request.RawUrl, "ShibHandler") > 0 Then
                Exit Sub
            End If

            objEventLog.AddLog("AuthenticationModule_OnAuthenticateRequest6", Request.RawUrl, _portalSettings, -1, ShibAlert)

            Dim portalID As Integer

            Dim objPortalSettings As PortalSettings = Nothing
            objPortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            If objPortalSettings Is Nothing Then Exit Sub
            portalID = objPortalSettings.PortalId

            Dim objAuthentication As New ShibAuthController
            Dim objShibUserController As ShibUserController = New ShibUserController
            Dim authStatus As AuthenticationStatus = GetCookieStatus(objPortalSettings.PortalId)

            Dim psDictionary As Dictionary(Of String, String) = PortalController.GetPortalSettingsDictionary(portalID)
            Dim myLogoutPage As String
            Dim myLoginPage As String

            Dim config As ShibConfiguration = ShibConfiguration.GetConfig()
            If config Is Nothing Then
                Exit Sub
            End If

            If psDictionary.ContainsKey("Shib_Authentication") = False Then ' Then Or sh.userName Is Nothing Then
                myLogoutPage = "default.aspx"
            Else
                myLogoutPage = config.LogoutPage
                myLoginPage = config.LoginPage
            End If

            If InStr(Request.RawUrl, myLogoutPage) > 0 Then
                Exit Sub
            End If
            ''''''''''''''''''''''''''''''''''''''
            'add code to stop page caching so dnn will display the latest version of a page
            'otherwise, when going from a non-shib site to a shib site, an earlier non-logged in version
            'of the shib site may display causing view mac state errors.

            'If InStr(Request.RawUrl.ToLower, "home.aspx") > 0 Then

            Response.CacheControl = "no-cache"
            Response.AddHeader("Pragma", "no-cache")
            Response.Expires = -1

            ''''''''''''''''''''''''''''''''''''''''''

            Dim blnSimulateShibLogin As Boolean = config.SimulateLogin

            Dim blnShibEnabled As Boolean = config.Enabled

            Dim sh As ShibHandler = New ShibHandler

            ''0. Check 1st special condition: you've logged out, are on the MyLogout page, and clicked login

            'If InStr(Request.RawUrl.ToLower, "login.aspx") > 0 And InStr(Request.RawUrl.ToLower, myLogoutPage.ToLower) > 0 _
            'And CheckForShibCookie() And authStatus = AuthenticationStatus.SHIBLogoff Then

            '    SetDNNReturnToCookieAfterLogout(Request, Response, objPortalSettings)

            '    If psDictionary.ContainsKey("Shib_Authentication") Then

            '        If psDictionary.Item("Shib_Authentication") = "True" Then

            '            If Not (CheckForDNNCookie(portalID) And authStatus = AuthenticationStatus.Undefined) Then

            '                reDir = "~/DesktopModules/AuthenticationServices/Shibboleth/Login/InvokeShibHandler.ashx?" & portalID

            '                ShibLogon(portalID)

            '                Exit Sub

            '            End If

            '        End If

            '    ElseIf CheckForDNNCookie(portalID) And authStatus <> AuthenticationStatus.DNNLogoff Then

            '        If Not (CheckForDNNCookie(portalID) And authStatus = AuthenticationStatus.Undefined) Then

            '            ShibLogon(portalID)
            '            Exit Sub

            '        End If

            '    End If
            'End If

            '1. is this a logoff?

            If InStr(Request.RawUrl.ToLower, "/logoff/") > 0 Then
                'ReSetDNNReturnToCookie(Request, Response, objPortalSettings)
                SetDNNReturnToCookieAfterLogout(Request, Response, objPortalSettings)
                objAuthentication.AuthenticationLogoff()
                Exit Sub
                '2. is this a login?
            ElseIf (InStr(Request.RawUrl.ToLower, "/login.aspx?") > 0) _
             Or InStr(Request.RawUrl.ToLower, "/login/") > 0 Then
                'Or InStr(Request.RawUrl, "/MyLogout.aspx") > 0 Then

                SetDNNReturnToCookie(Request, Response, objPortalSettings)

                'If Utilities.CheckShibEventLogging() Then
                objEventLog.AddLog("AuthenticationModule_OnAuthenticateRequest8", authStatus.ToString, _portalSettings, -1, ShibAlert)
                'End If

                'if you've been on thru shib before, log on thru shib again
                If authStatus = AuthenticationStatus.SHIBLogon Or authStatus = AuthenticationStatus.SHIBLogoff Then
                    ShibLogon(portalID)
                    Exit Sub

                Else

                    If psDictionary.ContainsKey("Shib_Authentication") Then

                        If psDictionary.Item("Shib_Authentication") = "True" Then

                            ShibLogon(portalID)
                            Exit Sub

                        Else
                            reDir = NavigateURL(DotNetNuke.Common.GetPortalSettings().LoginTabId) ' + "login.aspx?"
                            HttpContext.Current.Response.Redirect(reDir)
                            Exit Sub
                        End If

                    Else
                        reDir = NavigateURL(DotNetNuke.Common.GetPortalSettings().LoginTabId)  ' + "login.aspx?"
                        HttpContext.Current.Response.Redirect(reDir)
                        Exit Sub
                    End If

                End If
                '3 is this a transfer in from another url
                'add and track a status for DNN login.  Make them loging to DNN if a shib cookie exists to distinguish
                'between shib signon and dnn signon

            ElseIf CheckForShibCookie() And _
                authStatus = AuthenticationStatus.Undefined Or authStatus = AuthenticationStatus.DNNLogoff _
                And InStr(Request.RawUrl, myLogoutPage) <> 0 And InStr(Request.RawUrl, "default.aspx") <> 0 Then


                If psDictionary.ContainsKey("Shib_Authentication") Then

                    If psDictionary.Item("Shib_Authentication") = "False" Then

                        SetDNNReturnToCookieAfterLogout(Request, Response, objPortalSettings)
                        objAuthentication.AuthenticationLogoff()
                        Exit Sub

                    End If

                Else

                    SetDNNReturnToCookieAfterLogout(Request, Response, objPortalSettings)
                    objAuthentication.AuthenticationLogoff()
                    Exit Sub

                End If

            End If

        End Sub

        Public Sub ShibLogon(ByVal portalID As Integer)

            Dim sh As ShibHandler = New ShibHandler
            Dim eppn As String = sh.userName

            'cb_072811
            Dim _portalSettings As PortalSettings = DotNetNuke.Common.GetPortalSettings
            Dim objEventLog As New DotNetNuke.Services.Log.EventLog.EventLogController
           
            'cb_072811

            Dim config As ShibConfiguration = ShibConfiguration.GetConfig()
            If config Is Nothing Then
                Exit Sub
            End If

            'DNN caching settings: 
            'File, memory, heavy, seP

            objEventLog.AddLog("AuthenticationModule_ShibLogon20", "Redirecting to Shib Handler ", _portalSettings, -1, "SHIB_ALERT")
         
            Dim reDir As String

            reDir = "~/DesktopModules/AuthenticationServices/Shibboleth/Login/InvokeShibHandler.ashx?" & portalID

            'DataCache.ClearHostCache(True)

            HttpContext.Current.Response.Redirect(reDir)

        End Sub

        Public Function GetCookieStatus(ByVal portalID As Integer) As AuthenticationStatus

            Dim cookieName As String = "authentication.status." & portalID.ToString
            Dim strStatus As String

            Dim config As ShibConfiguration = ShibConfiguration.GetConfig()
            If config Is Nothing Then
                Return AuthenticationStatus.Undefined
                Exit Function
            End If

            If HttpContext.Current.Request.Cookies(cookieName) IsNot Nothing Then
                Try
                    strStatus = FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies(cookieName).Value).UserData

                    Return CType([Enum].Parse(GetType(AuthenticationStatus), strStatus), AuthenticationStatus)
                Catch
                    'http://forums.asp.net/p/1608654/4107624.aspx
                    Return AuthenticationStatus.Undefined
                End Try

            Else
                Return 0

            End If

        End Function

        Public Sub Dispose() Implements IHttpModule.Dispose

        End Sub

        Private Function GetRedirectURL(ByVal Request As HttpRequest, ByVal _portalSettings As PortalSettings) As String

            If Request.ApplicationPath = "/" Then
                Return ShibConfiguration.AUTHENTICATION_PATH & ShibConfiguration.AUTHENTICATION_LOGON_PAGE & "?tabid=" & _portalSettings.ActiveTab.TabID.ToString
            Else
                Return Request.ApplicationPath & ShibConfiguration.AUTHENTICATION_PATH & ShibConfiguration.AUTHENTICATION_LOGON_PAGE & "?tabid=" & _portalSettings.ActiveTab.TabID.ToString
            End If
        End Function

        Private Sub SetDNNReturnToCookie(ByVal Request As HttpRequest, ByVal Response As HttpResponse, ByVal _portalSettings As PortalSettings)

            Try
                Dim refUrl As String = Request.RawUrl
                Response.Clear()

                Dim slnPath As String = GetSolutionPath()
               
                refUrl = slnPath + "default.aspx"
                refUrl = refUrl.ToLower

                If InStr(Request.RawUrl.ToLower, "/login.aspx?") > 0 Then
                    refUrl = Replace(refUrl, "default.aspx", "")
                Else
                    refUrl = Replace(refUrl, "login.aspx", "")

                End If

                Response.Cookies("DNNReturnTo" & _portalSettings.PortalId.ToString()).Value = refUrl
                Response.Cookies("DNNReturnTo" & _portalSettings.PortalId.ToString()).Path = "/"
                Response.Cookies("DNNReturnTo" & _portalSettings.PortalId.ToString()).Expires = DateTime.Now.AddMinutes(2)

            Catch ex As Exception
                LogException(ex)
            End Try

        End Sub

        Private Sub SetDNNReturnToCookieAfterLogout(ByVal Request As HttpRequest, ByVal Response As HttpResponse, ByVal _portalSettings As PortalSettings)

            'Dim objPortalSettings As PortalSettings = Nothing
            'objPortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            'If objPortalSettings Is Nothing Then Exit Sub

            'Dim portalID As Integer = objPortalSettings.PortalId

            'Dim psDictionary As Dictionary(Of String, String) = PortalController.GetPortalSettingsDictionary(portalID)

            Dim psDictionary As Dictionary(Of String, String) = PortalController.GetPortalSettingsDictionary(_portalSettings.PortalId)

            Dim sh As ShibHandler = New ShibHandler

            Dim LoggedOnUserName As String = sh.userName

            Try
                Dim refUrl As String = Request.RawUrl
                Response.Clear()

                Dim myLogoutPage As String
                Dim config As ShibConfiguration = ShibConfiguration.GetConfig()

                If psDictionary.ContainsKey("Shib_Authentication") = False Then ' Or sh.userName Is Nothing Then
                    myLogoutPage = "default.aspx"
                Else
                    myLogoutPage = Config.LogoutPage
                End If

                Dim slnPath As String = GetSolutionPath()
                
                refUrl = slnPath + myLogoutPage
                refUrl = "/" + myLogoutPage


                Response.Cookies("DNNReturnTo" & _portalSettings.PortalId.ToString()).Value = refUrl
                Response.Cookies("DNNReturnTo" & _portalSettings.PortalId.ToString()).Path = "/"
                Response.Cookies("DNNReturnTo" & _portalSettings.PortalId.ToString()).Expires = DateTime.Now.AddMinutes(2)

            Catch ex As Exception
                LogException(ex)
            End Try

        End Sub

        Private Function GetSolutionPath() As String
            Dim prjSettings As ProjectSettings = New ProjectSettings
            Return prjSettings.slnPath
        End Function

        Public Function CheckForShibCookie() As Boolean

            Dim Request As HttpRequest = HttpContext.Current.Request
            Dim blnShibCookieFound As Boolean = False
           
            For i As Integer = 0 To Request.Cookies.Count - 1
                If InStr(HttpContext.Current.Request.Cookies.Item(i).Name, "_shibsession_") > 0 Then
                    blnShibCookieFound = True
                    Exit For
                End If
            Next

            Return blnShibCookieFound
        End Function

        Public Function CheckForDNNCookie(ByVal portalID As Integer) As Boolean

            Dim Request As HttpRequest = HttpContext.Current.Request
            Dim blnDNNCookieFound As Boolean = False

            For i As Integer = 0 To Request.Cookies.Count - 1
                If InStr(HttpContext.Current.Request.Cookies.Item(i).Name, "authentication.status." & portalID) > 0 Then

                    blnDNNCookieFound = True

                    Exit For
                End If
            Next

            Return blnDNNCookieFound
        End Function

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub

        'Public Function CheckShibEventLogging() As Boolean
        '    If LogTypeKeyInstalled(ShibAlert) Then
        '        Return False
        '    Else
        '        Dim logTypeConfigInfo = New DotNetNuke.Services.Log.EventLog.LogTypeConfigInfo
        '        logTypeConfigInfo.LogTypeKey = ShibAlert
        '        Return logTypeConfigInfo.LoggingIsActive
        '    End If
        'End Function


        'Private Sub AddLogTypes()

        '    'logging info and code comes from here:
        '    '  http://www.byteblocks.com/post/2011/06/23/DNN-Custom-Event-Log-Entries.aspx

        '    If Not LogTypeKeyInstalled(ShibAlert) Then
        '        ' Perform add operation for custom log type

        '        'Dim logController = New LogController()
        '        Dim logController = New DotNetNuke.Services.Log.EventLog.EventLogController()
        '        'DotNetNuke.Services.Log.EventLog.EventLogController()
        '        Dim logTypeInfo = New DotNetNuke.Services.Log.EventLog.LogTypeInfo

        '        logTypeInfo.LogTypeCSSClass = "GeneralAdminOperation"
        '        logTypeInfo.LogTypeDescription = "Shibboleth Logging"
        '        logTypeInfo.LogTypeFriendlyName = ShibAlert
        '        logTypeInfo.LogTypeOwner = "DotNetNuke.Logging.EventLogType"
        '        logTypeInfo.LogTypeKey = ShibAlert ' Pick unique key name
        '        logController.AddLogType(logTypeInfo)

        '        Dim hc As DotNetNuke.Entities.Controllers.HostController

        '        Dim logTypeConfigInfo = New DotNetNuke.Services.Log.EventLog.LogTypeConfigInfo
        '        logTypeConfigInfo.LogTypeKey = ShibAlert
        '        logTypeConfigInfo.LoggingIsActive = False
        '        logTypeConfigInfo.MailFromAddress = hc.GetSettings("SMTPServer").Value
        '        logTypeConfigInfo.MailToAddress = hc.GetSettings("HostEmail").Value
        '        logTypeConfigInfo.EmailNotificationIsActive = False
        '        logController.AddLogTypeConfigInfo(logTypeConfigInfo)

        '    End If

        'End Sub

        'Public Shared Function LogTypeKeyInstalled(ByVal logTypeKey As String) As Boolean
        '    Dim eventLogController = New DotNetNuke.Services.Log.EventLog.EventLogController()
        '    Dim logTypes = New List(Of DotNetNuke.Services.Log.EventLog.LogTypeInfo)(eventLogController.GetLogTypeInfo().Cast(Of DotNetNuke.Services.Log.EventLog.LogTypeInfo)())
        '    Return logTypes.Any(Function(lt) lt.LogTypeKey = logTypeKey)
        'End Function

    End Class

End Namespace

