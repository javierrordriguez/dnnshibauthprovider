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
' of the Software.lI
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports System.Web
Imports System.Web.Security

Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Security.Membership

Imports Configuration = UF.Research.Authentication.Shibboleth.ShibConfiguration
Imports DNNUserController = DotNetNuke.Entities.Users.UserController

Imports DotNetNuke.Entities.Modules.UserUserControlBase
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.Common.Globals
Imports DotNetNuke.Entities.Tabs


Namespace UF.Research.Authentication.Shibboleth

    Public Class ShibAuthController
        Inherits DotNetNuke.Entities.Modules.UserUserControlBase

        Private mProcessLog As String = ""
        Private mProviderTypeName As String = ""
        Private _portalSettings As PortalSettings
        Private _portalID As Integer

        Dim _ShibConfiguration As ShibConfiguration = New ShibConfiguration

        Sub New()

            _ShibConfiguration = ShibConfiguration.GetConfig()
            _portalSettings = PortalController.GetCurrentPortalSettings
            _portalID = _portalSettings.PortalId
            mProviderTypeName = _ShibConfiguration.ProviderTypeName

        End Sub

        Public Sub AuthenticationLogon()
            'authenticationLogon is always called directly from shibHandler so it should have the 
            'correct portal settings since they are created in shibHandler

            Dim objAuthUserController As New UF.Research.Authentication.Shibboleth.ShibUserController
            Dim objUser As DotNetNuke.Entities.Users.UserInfo = Nothing
            Dim objDNNValidateUser As DotNetNuke.Entities.Users.UserInfo = Nothing
            Dim objReturnUser As DotNetNuke.Entities.Users.UserInfo = Nothing
            Dim intUserId As Integer

            Dim blnSimulateShibLogin As Boolean = _ShibConfiguration.SimulateLogin

            Dim sh As ShibHandler = New ShibHandler
            Dim LoggedOnUserName As String

            'If blnSimulateShibLogin Then
            '    LoggedOnUserName = sh.GetTestCaseUserName()
            'Else
            '    LoggedOnUserName = sh.userName
            'End If

            LoggedOnUserName = sh.userName()

            Dim loginStatus As UserLoginStatus = UserLoginStatus.LOGIN_SUCCESS

            ' Get ipAddress for eventLog
            Dim ipAddress As String = ""
            If Not HttpContext.Current.Request.UserHostAddress Is Nothing Then
                ipAddress = HttpContext.Current.Request.UserHostAddress
            End If

            Dim objAuthUser As UF.Research.Authentication.Shibboleth.ShibUserInfo
            objAuthUser = objAuthUserController.GetUser(LoggedOnUserName)
            objUser = DotNetNuke.Entities.Users.UserController.GetUserByName(_portalID, LoggedOnUserName)

            Dim myUserName As String = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo().Username

            If objUser Is Nothing Then
                Exit Sub
            End If

            intUserId = objUser.UserID

            If objUser.Profile.TimeZone <> 0 Then
                objAuthUser.Profile.TimeZone = objUser.Profile.TimeZone
            End If

            objAuthUser.UserID = intUserId
            objUser = CType(objAuthUser, DotNetNuke.Entities.Users.UserInfo)

            objReturnUser = DNNUserController.GetUserByName(_portalID, LoggedOnUserName)

            Dim PersistentCookieTimeout As Integer

            Dim objEventLog As New DotNetNuke.Services.Log.EventLog.EventLogController
            Dim objEventLogInfo As New DotNetNuke.Services.Log.EventLog.LogInfo

            'create cookie

            FormsAuthentication.SetAuthCookie(Convert.ToString(LoggedOnUserName), True)

            'check if user has supplied custom value for expiration

            If Not Config.GetSetting("PersistentCookieTimeout") Is Nothing Then
                PersistentCookieTimeout = Integer.Parse(Config.GetSetting("PersistentCookieTimeout"))
                'only use if non-zero, otherwise leave as asp.net value
                If PersistentCookieTimeout <> 0 Then
                    'locate and update cookie
                    Dim authCookie As String = FormsAuthentication.FormsCookieName
                    For Each cookie As String In HttpContext.Current.Response.Cookies
                        If cookie.Equals(authCookie) Then
                            HttpContext.Current.Response.Cookies(cookie).Expires = DateTime.Now.AddMinutes(PersistentCookieTimeout)
                        End If
                    Next
                End If
            End If

            SetStatus(_portalID, AuthenticationStatus.SHIBLogon)

            objEventLogInfo.AddProperty("IP", ipAddress)
            objEventLogInfo.LogPortalID = _portalSettings.PortalId
            objEventLogInfo.LogPortalName = _portalSettings.PortalName
            objEventLogInfo.LogUserID = intUserId
            objEventLogInfo.LogUserName = LoggedOnUserName
            objEventLogInfo.AddProperty("ShibbolethAuthentication", "True")
            objEventLogInfo.LogTypeKey = "LOGIN_SUCCESS"

            objEventLog.AddLog(objEventLogInfo)

            If Not Config.GetSetting("PersistentCookieTimeout") Is Nothing Then
                PersistentCookieTimeout = Integer.Parse(Config.GetSetting("PersistentCookieTimeout"))
                'only use if non-zero, otherwise leave as asp.net value
                If PersistentCookieTimeout <> 0 Then
                    'locate and update cookie
                    Dim authCookie As String = FormsAuthentication.FormsCookieName
                    For Each cookie As String In HttpContext.Current.Response.Cookies
                        If cookie.Equals(authCookie) Then
                            HttpContext.Current.Response.Cookies(cookie).Expires = DateTime.Now.AddMinutes(PersistentCookieTimeout)
                        End If
                    Next
                End If
            End If

            SetStatus(_portalID, AuthenticationStatus.SHIBLogon)

            objEventLogInfo.AddProperty("IP", ipAddress)
            objEventLogInfo.LogPortalID = _portalSettings.PortalId
            objEventLogInfo.LogPortalName = _portalSettings.PortalName
            objEventLogInfo.LogUserID = intUserId
            objEventLogInfo.LogUserName = LoggedOnUserName
            objEventLogInfo.AddProperty("ShibbolethAuthentication", "True")
            objEventLogInfo.LogTypeKey = "LOGIN_SUCCESS"

            objEventLog.AddLog(objEventLogInfo)

        End Sub

        Public Function ManualLogon(ByVal UserName As String, ByRef loginStatus As UserLoginStatus, ByVal ipAddress As String) As DotNetNuke.Entities.Users.UserInfo
            Dim objShibUser As ShibUserInfo = Nothing
            Dim objUser As DotNetNuke.Entities.Users.UserInfo = Nothing
            Dim objDNNValidateUser As DotNetNuke.Entities.Users.UserInfo = Nothing
            Dim objReturnUser As DotNetNuke.Entities.Users.UserInfo = Nothing
            Dim intUserId As Integer
            Dim strPassword As String = ""

            Try
                objShibUser = ProcessShibAuthentication(UserName)

                If Not String.IsNullOrEmpty(UserName) AndAlso (UserName.Length > 0) And (objShibUser IsNot Nothing) Then


                    objShibUser.Username = UserName
                    objUser = DotNetNuke.Entities.Users.UserController.GetUserByName(_portalID, UserName)

                    'DNN user exists
                    If Not (objUser Is Nothing) Then

                        If (objUser.IsDeleted = False) Then
                            intUserId = objUser.UserID
                            ' Synchronize role membership if it's required in settings
                            'we could go ahead and sychronize the roles...
                            If _ShibConfiguration.SynchronizeRoles Then
                                SynchronizeRoles(objUser)
                            End If
                            SetProfileProperties(objUser)

                            objReturnUser = DNNUserController.GetUserByName(_portalID, objShibUser.Username)

                        Else
                            'if the user is deleted, don't do anything

                            ''Only create user if Allowed to
                            'If _config.AutoCreateUsers = True Then
                            '    'if they've been deleted they won't be able to automatically log back in
                            '    'you must set objUser.Membership.Password to a value to get a loginStatus
                            '    'of success with CreateUser
                            '    'objUser.Membership.Password = RandomizePassword(objUser, strPassword)
                            '    CreateUser(objUser, loginStatus)
                            '    If loginStatus = UserLoginStatus.LOGIN_SUCCESS Then
                            '        objReturnUser = DNNUserController.GetUserByName(_portalSettings.PortalId, objShibUser.Username)
                            '        If _config.SynchronizeRoles Then
                            '            SynchronizeRoles(objUser)
                            '        End If
                            '        SetProfileProperties(objUser)
                            '    End If
                            'End If
                        End If
                    Else 'DNN user doesn't exist
                        If _ShibConfiguration.AutoCreateUsers = True Then
                            'User doesn't exist in this portal. Make sure user doesn't exist on any other portal
                            objUser = DNNUserController.GetUserByName(Null.NullInteger, objShibUser.Username)
                            If objUser Is Nothing Then 'User doesn't exist in any portal
                                'you must set objShibUser.Membership.Password to a value to get a loginStatus
                                'of success with CreateUser
                                'objShibUser.Membership.Password = objShibUser.Username & "12345"
                                'objShibUser.Membership.Password = RandomizePassword(objShibUser, strPassword)
                                objShibUser.Membership.Password = GetRandomPasswordUsingGUID(12)
                                CreateUser(CType(objShibUser, DotNetNuke.Entities.Users.UserInfo), loginStatus)
                                'cb 1029
                                'objShibUser.Membership.Password = RandomizePassword(objUser, strPassword)
                                objShibUser.Membership.UpdatePassword = True
                                DotNetNuke.Services.Exceptions.LogException(New Exception("Shib user created"))
                            Else 'user exists in another portal, then create userportals record.You must set  
                                'the password to be that of the current DNN user password in order for CreateUser to work.  
                                'Otherwise, CreateUser will see that the user exists, but with a different password,
                                'and the loginStatus will not be successful.
                                objShibUser.UserID = objUser.UserID
                                objShibUser.Membership.Password = DNNUserController.GetPassword(objUser, strPassword)
                                CreateUser(CType(objShibUser, DotNetNuke.Entities.Users.UserInfo), loginStatus)
                            End If
                            If loginStatus = UserLoginStatus.LOGIN_SUCCESS Then
                                objReturnUser = DNNUserController.GetUserByName(_portalID, objShibUser.Username)
                                If _ShibConfiguration.SynchronizeRoles Then
                                    SynchronizeRoles(objReturnUser)
                                End If
                                SetProfileProperties(objReturnUser)
                                'Change this to be a random password.  We are defaulting the password here for testing. 
                                'objReturnUser.Membership.Password = RandomizePassword(objReturnUser, strPassword)
                                'Dim blnPasswordChanged As Boolean = DNNUserController.ChangePassword(objReturnUser, objReturnUser.Membership.Password, RandomizePassword(objReturnUser, strPassword))
                            End If
                        End If
                    End If

                End If
            Catch exc As Exception
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exc)
            End Try

            Return objReturnUser

        End Function

        Private Sub CreateUser(ByVal objUser As DotNetNuke.Entities.Users.UserInfo, ByRef loginStatus As UserLoginStatus)
            UpdateDisplayName(objUser)
            objUser.Membership.Approved = True

            Dim createStatus As UserCreateStatus = DNNUserController.CreateUser(objUser)

            Dim args As UserCreatedEventArgs
            If createStatus = UserCreateStatus.Success Then
                args = New UserCreatedEventArgs(objUser)

            Else       ' registration error
                args = New UserCreatedEventArgs(Nothing)
            End If
            args.CreateStatus = createStatus
            OnUserCreated(args)
            OnUserCreateCompleted(args)

            If createStatus = UserCreateStatus.Success Then
                loginStatus = UserLoginStatus.LOGIN_SUCCESS
            Else
                loginStatus = UserLoginStatus.LOGIN_FAILURE
            End If
        End Sub

        '''' -----------------------------------------------------------------------------
        '''' <summary>
        '''' RandomizePassword = Creates a random password to be stored in the database
        '''' </summary>
        '''' <param name="objUser">DNN User Object</param>
        '''' <history>
        ''''     [mhorton]   12/10/2008 - ACD-4158
        '''' </history>
        '''' -----------------------------------------------------------------------------
        Private Function RandomizePassword(ByVal objUser As DotNetNuke.Entities.Users.UserInfo, ByRef strPassword As String) As String
            'ACD-4158 - Make sure password in the DNN database does not match that of the password in the AD.
            Dim aspNetUser As MembershipUser = Membership.GetUser(objUser.Username)
            Dim strStoredPassword As String = aspNetUser.GetPassword()

            'If (strStoredPassword = strPassword) Then
            'Dim strRandomPassword As String = ADSI.Utilities.GetRandomPassword()

            Dim strRandomPassword As String = SHIB.Utilities.GetRandomPassword()
            DNNUserController.ChangePassword(objUser, objUser.Membership.Password, strRandomPassword)
            Return strRandomPassword
            'Else
            'Return strStoredPassword
            'End If

        End Function

        'copied from http://www.4guysfromrolla.com/articles/101205-1.aspx
        Public Function GetRandomPasswordUsingGUID(ByVal length As Integer) As String
            'Get the GUID
            Dim guidResult As String = System.Guid.NewGuid().ToString()

            'Remove the hyphens
            guidResult = guidResult.Replace("-", String.Empty)

            'Make sure length is valid
            If length <= 0 OrElse length > guidResult.Length Then
                Throw New ArgumentException("Length must be between 1 and " & guidResult.Length)
            End If

            'Return the first length bytes
            Return guidResult.Substring(0, length)
        End Function

        Public Sub AuthenticationLogoff()

            Dim authCookies As String = ShibConfiguration.AUTHENTICATION_KEY & "_" & _portalID.ToString

            'set the logout page
            Dim objPortalSettings As PortalSettings = Nothing
            objPortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            If objPortalSettings Is Nothing Then Exit Sub

            Dim config As ShibConfiguration = ShibConfiguration.GetConfig

            Dim loginPage As String = _ShibConfiguration.LoginPage
            Dim logoutPage As String = _ShibConfiguration.LogoutPage

            Dim sh As ShibHandler = New ShibHandler

            Dim psDict As System.Collections.Generic.Dictionary(Of String, String) = _
                New System.Collections.Generic.Dictionary(Of String, String)
            Dim myLogoutPage As String

            psDict = PortalController.GetPortalSettingsDictionary(_portalID)

            ' Log User Off from Cookie Authentication System
            FormsAuthentication.SignOut()
            If GetStatus(_portalSettings.PortalId) = AuthenticationStatus.SHIBLogon Then
                If _ShibConfiguration.Enabled Then
                    SetStatus(_portalSettings.PortalId, AuthenticationStatus.SHIBLogoff)
                Else
                    SetStatus(_portalSettings.PortalId, AuthenticationStatus.Undefined)
                End If
               
            Else
                SetStatus(_portalSettings.PortalId, AuthenticationStatus.DNNLogoff)
            End If

            ' expire cookies
            HttpContext.Current.Response.Cookies("portalaliasid").Value = Nothing
            HttpContext.Current.Response.Cookies("portalaliasid").Path = "/"
            HttpContext.Current.Response.Cookies("portalaliasid").Expires = DateTime.Now.AddYears(-30)

            HttpContext.Current.Response.Cookies("portalroles").Value = Nothing
            HttpContext.Current.Response.Cookies("portalroles").Path = "/"
            HttpContext.Current.Response.Cookies("portalroles").Expires = DateTime.Now.AddYears(-30)

            Dim slnPath As String = ""
            GetSolutionPath(slnPath)

            Dim psDictionary As System.Collections.Generic.Dictionary(Of String, String) = PortalController.GetPortalSettingsDictionary(_portalID)

            If psDictionary.ContainsKey("Shib_Authentication") = False Then ' Then Or sh.userName Is Nothing Then
                myLogoutPage = "default.aspx"
            Else
                myLogoutPage = _ShibConfiguration.LogoutPage
            End If

            Dim objTabController As New TabController
            Dim TabCollection As DotNetNuke.Entities.Tabs.TabCollection = New DotNetNuke.Entities.Tabs.TabCollection

            myLogoutPage = Replace(myLogoutPage, ".aspx", "")

            Dim TabInfo As DotNetNuke.Entities.Tabs.TabInfo = objTabController.GetTabByName(myLogoutPage, _portalID)

            If TabInfo IsNot Nothing Then
                Dim myTabID As Integer = TabInfo.TabID
                HttpContext.Current.Response.Redirect(Globals.NavigateURL(myTabID, False))
            Else
                Dim reDir As String = slnPath ' + "default.aspx"
                HttpContext.Current.Response.Redirect(reDir)
            End If
        End Sub
        Private Sub GetSolutionPath(ByRef slnPath As String)
            Dim prjSettings As ProjectSettings = New ProjectSettings
            slnPath = prjSettings.slnPath
        End Sub

        Public Function ProcessShibAuthentication(ByVal LoggedOnUserName As String) As ShibUserInfo

            Dim objShibUserController As New ShibUserController
            Dim objUsers As New DotNetNuke.Entities.Users.UserController

            Dim UserName As String = LoggedOnUserName

            Dim objShibUser As ShibUserInfo = objShibUserController.GetUser(UserName)

            Return objShibUser

        End Function

        Public Function GetDNNUser(ByVal PortalID As Integer, ByVal LoggedOnUserName As String) As DotNetNuke.Entities.Users.UserInfo

            Dim objUser As DotNetNuke.Entities.Users.UserInfo

            Dim UserName As String = LoggedOnUserName

            objUser = DotNetNuke.Entities.Users.UserController.GetUserByName(Null.NullInteger, UserName)
            If Not objUser Is Nothing Then
                ' Check if user exists in this portal
                If DotNetNuke.Entities.Users.UserController.GetUserByName(PortalID, UserName) Is Nothing Then
                    ' The user does not exist in this portal - add them
                    objUser.PortalID = PortalID
                    DotNetNuke.Entities.Users.UserController.CreateUser(objUser)
                End If
                Return objUser
            Else
                ' the user does not exist
                Return Nothing
            End If

        End Function

        Public Shared Function GetStatus(ByVal PortalID As Integer) As AuthenticationStatus

            Dim authCookies As String = ShibConfiguration.AUTHENTICATION_STATUS_KEY & "." & PortalId.ToString
            Try
                If Not HttpContext.Current.Request.Cookies(authCookies) Is Nothing Then
                    ' get Authentication from cookie
                    Dim AuthenticationTicket As FormsAuthenticationTicket = FormsAuthentication.Decrypt(HttpContext.Current.Request.Cookies(authCookies).Value)
                    Return CType([Enum].Parse(GetType(AuthenticationStatus), AuthenticationTicket.UserData), AuthenticationStatus)
                Else
                    'Return AuthenticationStatus.Undefined
                End If

            Catch ex As Exception
            End Try
        End Function

        Public Shared Sub SetStatus(ByVal PortalID As Integer, ByVal Status As AuthenticationStatus)

            Dim authCookies As String = ShibConfiguration.AUTHENTICATION_STATUS_KEY & "." & PortalID.ToString
            Dim Request As HttpRequest = HttpContext.Current.Request
            Dim Response As HttpResponse = HttpContext.Current.Response

            Dim AuthenticationTicket As New FormsAuthenticationTicket(1, authCookies, DateTime.Now, DateTime.Now.AddHours(1), False, Status.ToString)
            ' encrypt the ticket
            Dim strAuthentication As String = FormsAuthentication.Encrypt(AuthenticationTicket)

            If Not Request.Cookies(authCookies) Is Nothing Then
                ' expire
                Request.Cookies(authCookies).Value = Nothing
                Request.Cookies(authCookies).Path = "/"
                Request.Cookies(authCookies).Expires = DateTime.Now.AddYears(-1)
            End If

            Response.Cookies(authCookies).Value = strAuthentication
            Response.Cookies(authCookies).Path = "/"
            Response.Cookies(authCookies).Expires = DateTime.Now.AddHours(1)

        End Sub
        'Public Shared Sub SetPortalSettings(ByVal PortalID As Integer, ByVal authStatus As AuthenticationStatus)

        '    Dim authCookies As String = ShibConfiguration.AUTHENTICATION_STATUS_KEY & "." & PortalID.ToString
        '    Dim Request As HttpRequest = HttpContext.Current.Request
        '    Dim Response As HttpResponse = HttpContext.Current.Response

        '    Dim AuthenticationTicket As New FormsAuthenticationTicket(1, authCookies, DateTime.Now, DateTime.Now.AddHours(1), False, authStatus.ToString)
        '    ' encrypt the ticket
        '    Dim strAuthentication As String = FormsAuthentication.Encrypt(AuthenticationTicket)

        '    If Not Request.Cookies(authCookies) Is Nothing Then
        '        ' expire
        '        Request.Cookies(authCookies).Value = Nothing
        '        Request.Cookies(authCookies).Path = "/"
        '        Request.Cookies(authCookies).Expires = DateTime.Now.AddYears(-1)
        '    End If

        '    Response.Cookies(authCookies).Value = strAuthentication
        '    Response.Cookies(authCookies).Path = "/"
        '    Response.Cookies(authCookies).Expires = DateTime.Now.AddHours(1)

        'End Sub

        Public Sub SynchronizeRoles(ByVal objUser As DotNetNuke.Entities.Users.UserInfo)

            Dim objAuthUserController As New UF.Research.Authentication.Shibboleth.ShibUserController
            Dim objAuthUser As UF.Research.Authentication.Shibboleth.ShibUserInfo

            Dim intUserId As Integer = objUser.UserID

            objAuthUser = objAuthUserController.GetUser(objUser.Username)

            objAuthUser.UserID = objUser.UserID
            ShibUserController.AddUserRoles(_portalID, objAuthUser)
            'User exists updating user profile
            objAuthUserController.UpdateDNNUser(objAuthUser)

        End Sub

        Public Sub SetProfileProperties(ByVal objUser As DotNetNuke.Entities.Users.UserInfo)
            Dim objAuthUserController As New UF.Research.Authentication.Shibboleth.ShibUserController

            Dim sh As ShibHandler = New ShibHandler
            'Dim strPropertyName As String

            With UserInfo
                'User properties are stored in an arraylist with 
                '  - 1)fieldtype: UI-UserInfo, UIM-Membership, UIP-UserInfoProfile
                '  - 2)fieldname: "UserName", "DistinquishedName", "Department", UIM-"Approved", UIM-"LastLoginDate"
                '  - 3)fieldsource: "HTTP_EPPN', "HTTP_GIVENName", "HTTP_SN"
                '  - 4)fieldOverwrite: "T", "F"
                'these are read in 

                If sh.UserInfoProperties Is Nothing Then
                    Exit Sub
                End If

                objUser.Profile.InitialiseProfile(objUser.PortalID)

                For Each UIPropertiesIN In sh.UserInfoProperties

                    'Select Case sh.UserFieldType   'will be UI or UIP
                    Select Case UIPropertiesIN.FieldType   'will be UI or UIM or UIP

                        Case "UIP" 'for type "UIP"

                            Dim strPPName As String = UIPropertiesIN.FieldName
                            Dim strPPValue As String = UIPropertiesIN.FieldSource.ToString

                            Dim definition As ProfilePropertyDefinition = _
                            ProfileController.GetPropertyDefinitionByName(_portalID, strPPName)

                            If definition IsNot Nothing Then 'the profile property was found

                                If UIPropertiesIN.blnOverwrite Or definition.PropertyValue Is Nothing Then

                                    objUser.Profile.SetProfileProperty(strPPName, strPPValue)
                                End If

                            End If

                    End Select
                Next
            End With

            DotNetNuke.Entities.Profile.ProfileController.UpdateUserProfile(objUser)

        End Sub

        Private Sub UpdateDisplayName(ByVal objDNNUser As DotNetNuke.Entities.Users.UserInfo)

            'Update DisplayName to conform to Format
            Dim setting As Object = DotNetNuke.Entities.Modules.UserModuleBase.GetSetting(_portalSettings.PortalId, "Security_DisplayNameFormat")
            If (Not setting Is Nothing) AndAlso (Not String.IsNullOrEmpty(Convert.ToString(setting))) Then
                objDNNUser.UpdateDisplayName(Convert.ToString(setting))
            End If
        End Sub

    End Class


End Namespace
