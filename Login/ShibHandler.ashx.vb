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
Imports System.Web.Services

Imports System.Collections.Specialized
Imports System.Web.Security

Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DNNUserController = DotNetNuke.Entities.Users.UserController
Imports DNNUserInfo = DotNetNuke.Entities.Users.UserInfo

Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Services.Authentication
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Skins.Controls.ModuleMessage
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Security
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
Imports UF.Research.Authentication.Shibboleth

Imports UF.Research.Authentication.Shibboleth.SHIB
Imports System
Imports System.Web.UI.Control

Imports System.Web.UI
Imports DotNetNuke
Imports DotNetNuke.Common.Globals
Imports System.Net
Imports System.Security
Imports System.Collections

Imports DotNetNuke.Entities.Tabs


Namespace UF.Research.Authentication.Shibboleth

    Public Class ShibHandler
        Inherits DotNetNuke.Services.Authentication.AuthenticationLoginBase
        Implements System.Web.IHttpHandler


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

        'Private config As ShibConfiguration = ShibConfiguration.GetConfig()

        Private _portalSettings As PortalSettings ' = PortalController.GetCurrentPortalSettings

        Private mDistinguishedName As String = ""
        ' '' Additional properties which are not provided by MemberRole

        Private alUIProperties As ArrayList
        Private alShibTestDataUserProperties As ArrayList
        Private alShibHeaders As ArrayList
        Private alShibHeaderArrays As ArrayList

        Private ShibTestDataUserProperties As System.Collections.Generic.Dictionary(Of String, String)

        Private mblnSimulateLogin As Boolean = False
        Private mUserName As String = ""
        Private mCurrentPortalID As Integer = 0

        Private Const RD_BUFFER_SIZE As Integer = 4 * 1024

        Public Property currentPortalID() As Integer
            Get
                Dim Request As HttpRequest = HttpContext.Current.Request
                Return Request.QueryString.Item(0)

            End Get
            Set(ByVal value As Integer)
                mCurrentPortalID = value
            End Set
        End Property

        Public Property blnSimulateLogin() As Boolean
            Get
                Dim config As ShibConfiguration = ShibConfiguration.GetConfig()
                Dim currentProjectSettings As ProjectSettings = New ProjectSettings
                If config Is Nothing Then
                    Dim portalID As Integer = Request.QueryString.Item(0)
                    Dim ps As PortalSettings = New PortalSettings
                    ps = ProjectSettings.CreateNewPortalSettings(portalID)
                    config = ShibConfiguration.GetConfig()
                    Return config.SimulateLogin
                End If
                Return config.SimulateLogin
            End Get
            Set(ByVal value As Boolean)
                mblnSimulateLogin = value
            End Set
        End Property

        Public Property userName() As String
            Get
                If Not blnSimulateLogin Then
                    Return GetShibUserName()

                Else
                    'Return GetTestCaseUserName()
                    Return GetShibUserName()
                End If

            End Get
            Set(ByVal value As String)
                mUserName = value
            End Set
        End Property

        Public Structure ShibUserAttributesIn
            Dim ShibHeaderVariableName As String
            Dim ShibHeaderVariableValue As String
        End Structure

        Public Structure ShibUserRolesIn
            Dim RoleName As String
        End Structure

        Public Structure UIPropertiesIN
            Dim FieldType As String
            Dim FieldName As String
            Dim FieldSource As String
            Dim blnOverWrite As Boolean
        End Structure

        Public Structure ShibHeaderIn
            Dim HeaderName As String
            Dim HeaderDelimiter As Char
        End Structure

        Public Structure ShibHeaderArraysIn
            Dim ShibHeaderName As String
            Dim ShibHeaderVariables As ArrayList
        End Structure

        Public Property UserInfoProperties() As ArrayList

            Get
                Return BuildUserInfo()
            End Get
            Set(ByVal value As ArrayList)
                alUIProperties = value
            End Set

        End Property

        Public Property ShibHeaders() As ArrayList
            Get
                Return GetShibHeaders()

            End Get
            Set(ByVal value As ArrayList)
                alShibHeaders = value
            End Set
        End Property

        Public Property ShibHeaderArrays() As ArrayList
            Get
                ProcessShibHeaders()
                Return alShibHeaderArrays

            End Get
            Set(ByVal value As ArrayList)
                alShibHeaderArrays = value
            End Set
        End Property


#Region "Private Members"

        Private memberProvider As DotNetNuke.Security.Membership.MembershipProvider = DotNetNuke.Security.Membership.MembershipProvider.Instance()

#End Region

        Private Shared Sub AddEventLog(ByVal portalId As Integer, ByVal username As String, ByVal userId As Integer, ByVal portalName As String, ByVal Ip As String, ByVal loginStatus As UserLoginStatus)

            Dim objEventLog As New DotNetNuke.Services.Log.EventLog.EventLogController
            'Dim objEventLog As DotNetNuke.Services.Log.EventLog.EventLogController

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
                    'Did require full trust previously.  Medium trust is ok now
                    'Dim HostingPermissions As New AspNetHostingPermission(System.Security.Permissions.PermissionState.Unrestricted)
                    'HostingPermissions.Demand()

                    'Check if Shib Auth is enabled for the portal
                    Return ShibConfiguration.GetConfig().Enabled
                Catch ex As Exception
                    Return False
                End Try
            End Get
        End Property

#End Region

        Public Sub New()
            'moved to Private variable so it is accessible throughout class
            'but initialized here
            _portalSettings = PortalController.GetCurrentPortalSettings
        End Sub

        ' TODO - break this method into a series of method/subroutine calls.  too much logic is placed inline
        Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

            Dim objEventLog As New DotNetNuke.Services.Log.EventLog.EventLogController

            'If Utilities.CheckShibEventLogging() Then
            'objEventLog.AddLog("ShibHandler_ProcessRequest1", "Entering Process Request - ShibHandler_ProcessRequest1", PortalSettings, -1, ShibAlert)
            'End If

            Dim Request As HttpRequest = HttpContext.Current.Request
            Dim Response As HttpResponse = HttpContext.Current.Response ' sra - Why are we creating an  instance of the response if we'e not using it.

            ' TODO - have this use the key name, not the index.  what if it changes?
            Dim portalID As Integer = Request.QueryString.Item(0)

            ' TODO - first method refactor - create method to build out settings.
            Dim ps As PortalSettings = New PortalSettings
            ps = CreateNewPortalSettings(portalID)

            _portalSettings = ps

            Dim config As ShibConfiguration = ShibConfiguration.GetConfig()
            If config Is Nothing Then
                Exit Sub
            End If

            objEventLog.AddLog("ShibHandler_ProcessRequest3", "Portal Settings Created. - ShibHandler_ProcessRequest1", PortalSettings, -1, ShibAlert)

            blnSimulateLogin = config.SimulateLogin ' TODO - move this to a singleton instance.

            ' TODO - why are we creating a second instance of the request?
            Dim objRequest As HttpRequest = context.Request


            Dim prjSettings As UF.Research.Authentication.Shibboleth.ProjectSettings = New ProjectSettings
            Dim slnPath As String = prjSettings.slnPath
            ' END FIRST METHOD

            Dim strURL As String

            Dim ipAddress As String

            'cb - 020812 If userName is empty then return to the default page

            ' TODO - because the first if block forces a redirect, separate it from the other if blocks.
            If userName Is "UserName NotFound" And Not blnSimulateLogin Then
                'If userName not found And Not blnSimulateLogin Then
                'if there is nothing returned from Shibboleth, then you probably
                'don't have the Login directory secured by Shibboleth and you're 
                'getting here without logging in first. 

                'cb_062711
                'strURL = slnPath + "Home.aspx"
                strURL = slnPath + "default.aspx"

                HttpContext.Current.Response.Redirect(strURL, True)

            ElseIf blnSimulateLogin Then
                ' TODO - if the first if block is separated, then we can take the ProcessShibHeaders and BuildUserInfo method out of the if statements and proces the simulation separately

                'BuildUserInfo loads up the UserInfoProperties array list for either Shibboleth logins or Shibboleth simulation testing
                'and the ShibTestDataUserProperties dictionary for Shibboleth simulation testing

                objEventLog.AddLog("ShibHandler_ProcessRequest5", "Simulating Login - ShibHandler_ProcessRequest1.5", PortalSettings, -1, ShibAlert)

                ProcessShibHeaders()
                BuildUserInfo()

                ' regardless of condition, this exits the sub anyway.  we should refactor that.
                If alShibTestDataUserProperties Is Nothing Then
                    'no data was read in so exit sub
                    HttpContext.Current.Response.Write("TestCase.txt File Not Found or incorrect data file.")
                    Exit Sub
                ElseIf ShibTestDataUserProperties.Count = 0 Then
                    HttpContext.Current.Response.Write("TestCase.txt File Not Found or incorrect data file.")
                    Exit Sub
                End If


            Else 'this is a normal Shibboleth login

                'Process Headers assigns values to alADGroups and alPSRoles obtained from Server Variables returned after signing
                'in through Shibboleth

                ProcessShibHeaders()
                BuildUserInfo()

            End If

            'executing BuildUserInfo will load up the alUserInfo array list.  If this arraylist has a 
            'count of 0, you're not pulling shibboleth data in so maybe you meant to be simulating a 
            'shib login but forgot to set the simulation flag.  Only Login if BuildUserInfo().count > 0.'

            'cb_020812
            'If BuildUserInfo().Count > 0 Then
            If BuildUserInfo().Count >= 0 Then ' TODO  - we need something better than a count to determine if the user info was built.  maybe check for null
                'at this point we should have userName either by signing in through Shibboleth
                'or by signing in through the siumlation Test Case. 

                Dim objAuthentication As ShibAuthController = New ShibAuthController

                'Services.Exceptions.LogException(New Exception("shibtrace username:" + userName + "-LoginStatus:" + LoginStatus.ToString))

                Dim objUser As DNNUserInfo = objAuthentication.ManualLogon(userName, LoginStatus, ipAddress)

                'cb_050411 - don't allow login if doing simulation and user is superuser
                ' TODO - group the conditions.  right now it doesn't matter if it's a simulation as long as the user is not a super user
                If objUser Is Nothing OrElse objUser.IsSuperUser = False Then

                    LoginStatus = UserLoginStatus.LOGIN_SUCCESS

                    Dim authenticated As Boolean = Null.NullBoolean
                    Dim message As String = Null.NullString
                    authenticated = (LoginStatus <> UserLoginStatus.LOGIN_FAILURE)

                    'If objUser is nothing then there must've been a problem logging in. Write to the eventlog.
                    'objUser will be nothing if you are running a simulation and forget to include the property in the text file
                    'that is mapped to the userName.  (For UF, this property is HTTP_EPPN)  This property is mapped via the settingName
                    ''Shib_UserName' in portal Settings.

                    If objUser Is Nothing Then
                        AddEventLog(portalID, userName, Null.NullInteger, PortalSettings.PortalName, ipAddress, LoginStatus)
                    Else

                        objAuthentication.AuthenticationLogon()
                        Dim eventArgs As UserAuthenticatedEventArgs = New UserAuthenticatedEventArgs(objUser, userName, LoginStatus, "Shibboleth")
                        eventArgs.Authenticated = authenticated
                        eventArgs.Message = message
                        OnUserAuthenticated(eventArgs)
                    End If

                End If

            End If

            If Not HttpContext.Current.Request.Cookies("DNNReturnTo" + _portalSettings.PortalId.ToString()) Is Nothing Then
                strURL = HttpContext.Current.Request.Cookies("DNNReturnTo" + _portalSettings.PortalId.ToString()).Value

            Else
                strURL = slnPath + "Default.aspx"

            End If

            HttpContext.Current.Response.Redirect(strURL, True)

        End Sub
        'Public Function GetShibUserName() As String

        '    Dim psDict As System.Collections.Generic.Dictionary(Of String, String) = _
        '      New System.Collections.Generic.Dictionary(Of String, String)

        '    psDict = PortalController.GetPortalSettingsDictionary(PortalId)

        '    Dim strKeyName As String = "Shib_UserName"
        '    Dim strKeyValue As String

        '    If psDict.ContainsKey(strKeyName) Then
        '        strKeyValue = psDict.Item(strKeyName)
        '        Return Context.Request.ServerVariables(strKeyValue)
        '    Else
        '        Return "UserName NotFound"
        '    End If

        'End Function

        Public Function GetShibUserName() As String

            Dim psDict As System.Collections.Generic.Dictionary(Of String, String) = _
              New System.Collections.Generic.Dictionary(Of String, String)

            psDict = PortalController.GetPortalSettingsDictionary(PortalId)

            Dim strKeyName As String = "Shib_UserName"
            Dim strKeyValue As String

            If psDict.ContainsKey(strKeyName) Then
                strKeyValue = psDict.Item(strKeyName)

                'DotNetNuke.Services.Exceptions.LogException(New Exception("strKeyValue:" + strKeyValue + "-serverVariable:" + Context.Request.ServerVariables(strKeyValue)))

                If Not blnSimulateLogin Then
                    Return Context.Request.ServerVariables(strKeyValue)
                Else

                    If ShibTestDataUserProperties IsNot Nothing Then
                        If ShibTestDataUserProperties.ContainsKey(strKeyValue) Then
                            Return ShibTestDataUserProperties.Item(strKeyValue)
                            'strValue = psDict.Item(strKeyName)
                        Else
                            Return ""
                        End If
                    Else
                        ReadInShibbolethSimData()
                        If ShibTestDataUserProperties.ContainsKey(strKeyValue) Then
                            Return ShibTestDataUserProperties.Item(strKeyValue)
                        Else : Return ""
                        End If
                    End If
                End If

            Else
                Return "UserName NotFound"
            End If


        End Function

        Public Function GetTestCaseUserName() As String

            'read thru dictionary ShibTestDataUserProperties 
            'which has been built from calling sub ReadInShibbolethSimulationData()
            'ShibTestUserProperties is a dictionary of (string, string) where 
            'each item contains key,value pair of key: Header Variable Name, value: Header Variable Value

            If ShibTestDataUserProperties IsNot Nothing Then
                If ShibTestDataUserProperties.ContainsKey("HTTP_EPPN") Then
                    Return ShibTestDataUserProperties.Item("HTTP_EPPN")
                    'strValue = psDict.Item(strKeyName)
                Else
                    Return ""
                End If
            Else
                ReadInShibbolethSimData()
                If ShibTestDataUserProperties.ContainsKey("HTTP_EPPN") Then
                    Return ShibTestDataUserProperties.Item("HTTP_EPPN")
                Else : Return ""
                End If
            End If

        End Function

        Public Sub ProcessShibHeaders()

            Dim alShibHeaders As ArrayList = GetShibHeaders()

            Dim objRequest As HttpRequest = Context.Request
            Dim ShibHeaderInRow As ShibHeaderIn = New ShibHeaderIn
            Dim sArray As String()
            Dim sArrayList As ArrayList = New ArrayList

            Dim hdrArray As ArrayList = New ArrayList

            If Not blnSimulateLogin Then 'process as regular Shibboleth signon

                'For Each Header As String In objRequest.Headers
                For Each Header As String In objRequest.ServerVariables
                    For Each ShibHeaderInRow In alShibHeaders

                        If ShibHeaderInRow.HeaderName.ToUpper = Header.ToUpper Then

                            Dim ShibHeaderArrayInRow As ShibHeaderArraysIn = New ShibHeaderArraysIn

                            sArray = Split(objRequest.ServerVariables.Item(ShibHeaderInRow.HeaderName), ShibHeaderInRow.HeaderDelimiter)

                            'sArray = Split(Header, ShibHeaderInRow.HeaderDelimiter)


                            For Each sItem As String In sArray
                                sArrayList.Add(sItem)
                            Next

                            ShibHeaderArrayInRow.ShibHeaderName = Header
                            ShibHeaderArrayInRow.ShibHeaderVariables = sArrayList

                            hdrArray.Add(ShibHeaderArrayInRow)

                        End If

                    Next

                Next Header

                alShibHeaderArrays = hdrArray

            Else
                'process as Shib login simulation'
                'ReadInShibbolethSimulationData will load up alADGroups, alPSRoles and alShibTestDataUserProperties arraylists
                'and also load up ShibTestDataUserProperties dictionary

                ReadInShibbolethSimData()
            End If

        End Sub

        'Public Sub ProcessHeaders()

        '    If Not blnSimulateLogin Then 'process as regular Shibboleth signon

        '        Dim objRequest As HttpRequest = Context.Request

        '        For Each Header As String In objRequest.Headers

        '            Select Case Header.ToUpper
        '                Case "UFAD_GROUPS"
        '                    'alADGroups = OutputArray(Header, ";")
        '                Case "UFAD_PSROLES"
        '                    'alPSRoles = OutputArray(Header, "$")
        '                Case "HTTPS_SERVER_ISSUER", "HTTPS_SERVER_SUBJECT", "CERT_SERVER_SUBJECT", "CERT_SERVER_ISSUER"

        '                Case "ALL_HTTP", "ALL_RAW"

        '                Case Else

        '                    If objRequest.Headers.Item(Header) = "" Then
        '                        'Me.Page.Response.Write(" (blank)<br><br>")
        '                    Else
        '                        'Me.Page.Response.Write("<br>--> " & objRequest.Headers.Item(Header) & "<br><br>")
        '                    End If

        '            End Select

        '        Next Header
        '    Else 'process as Shib login simulation'
        '        'ReadInShibbolethSimulationData will load up alADGroups, alPSRoles and alShibTestDataUserProperties arraylists
        '        'and also load up ShibTestDataUserProperties dictionary
        '        ReadInShibbolethSimData()

        '    End If
        'End Sub

        Public Function BuildUserInfo() As ArrayList

            Dim config As ShibConfiguration = ShibConfiguration.GetConfig()

            If ShibTestDataUserProperties Is Nothing And blnSimulateLogin Then
                'ReadInShibbolethSimulationData()
                ReadInShibbolethSimData()
            End If
            'read portal settings and read in each UserInfo value, value by value and add it to the UserInfoProperties array.

            'get all of the portal settings for the current portal and read them into a dictionary field 

            Dim alUserInfo As ArrayList = New ArrayList

            Dim psDict As System.Collections.Generic.Dictionary(Of String, String) = _
              New System.Collections.Generic.Dictionary(Of String, String)
            ShibConfiguration.ResetConfig()
            psDict = PortalController.GetPortalSettingsDictionary(PortalId)

            Dim strKeyName As String 'portal settings key field
            Dim strArray As Array
            Dim strValue As String   'portal settings value field
            Dim strSource As String 'portal settings source field
            Dim uiCount As Integer 'counter for portal settings user Information values
            Dim UIPropertiesRow As UIPropertiesIN = New UIPropertiesIN


            'Go thru loop once for each role mapping
            For i = 1 To psDict.Count
                strKeyName = "Shib_UserMap_" & i.ToString
                If psDict.ContainsKey(strKeyName) Then
                    uiCount = i
                Else
                    Exit For
                End If
            Next
            For i = 1 To uiCount

                strKeyName = "Shib_UserMap_" & i.ToString

                'read the values from the dictionary into the datatable, row by row
                If psDict.Item(strKeyName) IsNot Nothing Then
                    strValue = psDict.Item(strKeyName)

                    strArray = strValue.Split(New Char() {config.Delimiter})

                    UIPropertiesRow.FieldType = strArray(0)
                    UIPropertiesRow.FieldName = strArray(1)
                    strSource = strArray(2)
                    If blnSimulateLogin Then
                        UIPropertiesRow.FieldSource = GetUserAttributesFromTestData(strSource)
                    Else
                        UIPropertiesRow.FieldSource = Context.Request.ServerVariables(strSource)
                    End If

                    If UIPropertiesRow.FieldSource <> "" Then
                        UIPropertiesRow.blnOverWrite = strArray(3)
                        alUserInfo.Add(UIPropertiesRow)
                    End If
                Else
                    Exit For
                End If
            Next

            Return alUserInfo

        End Function

        Public Function GetUserAttributesFromTestData(ByVal strSource As String) As String

            'read thru dictionary ShibTestDataUserProperties 
            'which has been built from calling sub ReadInShibbolethSimulationData()
            'ShibTestUserProperties is a dictionary of (string, string) where 
            'each item contains key,value pair of key: Header Variable Name, value: Header Variable Value
            If ShibTestDataUserProperties IsNot Nothing Then
                If ShibTestDataUserProperties.ContainsKey(strSource) Then
                    Return ShibTestDataUserProperties.Item(strSource)
                Else : Return ""
                End If
            Else
                ReadInShibbolethSimData()
                If ShibTestDataUserProperties.ContainsKey(strSource) Then
                    Return ShibTestDataUserProperties.Item(strSource)
                Else : Return ""
                End If
            End If

        End Function

        Public Sub ReadInShibbolethSimData()

            Dim alShibHeaderItems As ArrayList = GetShibHeaders()

            Try

                Dim TextLine As String = ""
                Dim File_Name As String = DotNetNuke.Common.Globals.ApplicationMapPath + "\DesktopModules\AuthenticationServices\Shibboleth\" + "TestCase.txt"
                Dim objReader As New System.IO.StreamReader(File_Name)
                If System.IO.File.Exists(File_Name) = True Then

                    ''''''''''''''''''''''''
                    Dim strFileNameIn As String = objReader.ReadLine()
                    Dim DataFileName As String = DotNetNuke.Common.Globals.ApplicationMapPath + "\DesktopModules\AuthenticationServices\Shibboleth\" + strFileNameIn
                    Dim objReaderFileIn As New System.IO.StreamReader(DataFileName)

                    ''''''''''''''''''''''''

                    If System.IO.File.Exists(DataFileName) = True Then

                        Dim i As Integer = 0
                        Dim UIPropertiesRow As UIPropertiesIN = New UIPropertiesIN
                        Dim strArray As Array

                        Dim shHeaderArrays As ArrayList = New ArrayList

                        Dim userAttributesIn As ArrayList = New ArrayList
                        Dim userAttributesInRow As ShibUserAttributesIn = New ShibUserAttributesIn
                        Dim ShibUserRolesInRow As ShibUserRolesIn = New ShibUserRolesIn
                        ShibTestDataUserProperties = New System.Collections.Generic.Dictionary(Of String, String)
                        Dim shibHeaderInRow As ShibHeaderIn = New ShibHeaderIn
                        Dim ShibHeaderArraysInRow As ShibHeaderArraysIn = New ShibHeaderArraysIn

                        Do While objReaderFileIn.Peek() <> -1

                            'Dim RolesRow As RolesIN = New RolesIN
                            Dim myUserName As String = ""

                            Dim strIn As String = objReaderFileIn.ReadLine()
                            If strIn = "" Then
                                Exit Do
                            End If
                            Dim strInNext As String

                            Dim strColPosn As Integer = strIn.IndexOf(":")
                            Dim strHeaderItem As String = Left(strIn, strColPosn)
                            Dim blnHeaderItemFound As Boolean = False

                            ''''''''''''''''''''''''''''''
                            For Each shibHeaderInRow In alShibHeaderItems

                                Dim ShibHeaderArraysInRoles As ArrayList = New ArrayList
                                ShibHeaderArraysInRow.ShibHeaderVariables = ShibHeaderArraysInRoles

                                If shibHeaderInRow.HeaderName = strHeaderItem Then
                                    blnHeaderItemFound = True
                                    ShibHeaderArraysInRow.ShibHeaderName = shibHeaderInRow.HeaderName
                                    strInNext = strIn.Replace(shibHeaderInRow.HeaderName & ":", "")
                                    Dim strDel As String = shibHeaderInRow.HeaderDelimiter
                                    strArray = strInNext.Split(strDel)
                                    For Each strGroup As String In strArray
                                        ShibHeaderArraysInRow.ShibHeaderVariables.Add(strGroup)
                                    Next
                                    Exit For
                                End If
                            Next
                            If blnHeaderItemFound Then
                                shHeaderArrays.Add(ShibHeaderArraysInRow)
                            Else

                                'if line wasn't a HeaderItem type then you're processing a user attribute

                                strArray = strIn.Split(New Char() {":"c})
                                userAttributesInRow.ShibHeaderVariableName = strArray(0)
                                userAttributesInRow.ShibHeaderVariableValue = strArray(1)
                                userAttributesIn.Add(userAttributesInRow)
                                ShibTestDataUserProperties.Add(strArray(0), strArray(1))
                            End If
                        Loop

                        alShibHeaderArrays = shHeaderArrays
                        alShibTestDataUserProperties = userAttributesIn
                        objReaderFileIn.Close()

                    Else
                        objReader.Close()
                        HttpContext.Current.Response.Write("Shibboleth Simulation requires file which was not found. File specified in TestCase.txt Not Found.")
                    End If
                Else
                    HttpContext.Current.Response.Write("Shibboleth Simulation requires file which was not found. TestCase.txt File Not Found.")
                End If

            Catch ex As Exception
                HttpContext.Current.Response.Write("Error processing Shibboleth Simulation data.")
            End Try

        End Sub

        Public Function GetShibHeaders() As ArrayList

            Dim config As ShibConfiguration = ShibConfiguration.GetConfig()

            Dim strKeyName As String
            Dim hdrCount As Integer
            Dim strValue As String
            Dim strArray As Array

            Dim alShHeaders As ArrayList = New ArrayList
            alShHeaders.Clear()

            Dim shibHeaderInRow As ShibHeaderIn = New ShibHeaderIn

            Dim psDict As System.Collections.Generic.Dictionary(Of String, String) = _
              New System.Collections.Generic.Dictionary(Of String, String)
            ShibConfiguration.ResetConfig()
            psDict = PortalController.GetPortalSettingsDictionary(PortalId)

            'Go thru loop once for each shib header item
            For i = 1 To psDict.Count
                strKeyName = "Shib_HeaderItem_" & i.ToString
                If psDict.ContainsKey(strKeyName) Then
                    hdrCount = i
                Else
                    Exit For
                End If
            Next
            For i = 1 To hdrCount

                strKeyName = "Shib_HeaderItem_" & i.ToString

                'read the values from the dictionary into the datatable, row by row
                If psDict.Item(strKeyName) IsNot Nothing Then
                    strValue = psDict.Item(strKeyName)

                    strArray = strValue.Split(config.Delimiter)

                    shibHeaderInRow.HeaderName = strArray(0)
                    shibHeaderInRow.HeaderDelimiter = strArray(1)
                    alShHeaders.Add(shibHeaderInRow)
                Else
                    Exit For
                End If
            Next

            Return alShHeaders

        End Function


        'Public Function OutputArray(ByVal sHeader As String, ByVal sDelimiter As String) As ArrayList

        '    Dim al As ArrayList
        '    Dim objRequest As HttpRequest = Context.Request
        '    Dim sArray As String()
        '    Dim sArrayList As ArrayList = New ArrayList

        '    sArray = Split(objRequest.Headers.Item(sHeader), sDelimiter)
        '    For Each sItem As String In sArray
        '        sArrayList.Add(sItem)
        '    Next
        '    If sHeader.ToUpper = "UFAD_GROUPS" Then
        '        al = sArrayList
        '        Return al
        '    Else
        '        If sHeader.ToUpper = "UFAD_PSROLES" Then
        '            al = sArrayList
        '            Return al
        '        Else
        '            Return Nothing
        '        End If
        '    End If

        'End Function

        ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
            Get
                Return False
            End Get
        End Property

        ''' <summary>
        ''' Code to get portal settings
        ''' </summary>
        ''' <param name="portalId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Shared Function CreateNewPortalSettings(ByVal portalId As Integer) As DotNetNuke.Entities.Portals.PortalSettings
            'new settings object
            Dim ps As PortalSettings = New PortalSettings()
            'controller instances
            Dim pc As PortalController = New PortalController()
            Dim tc As TabController = New TabController()
            Dim pac As PortalAliasController = New PortalAliasController()

            'get the first portal alias found to be used as the current portal alias
            Dim portalAlias As PortalAliasInfo = Nothing
            Dim aliases As PortalAliasCollection = pac.GetPortalAliasByPortalID(portalId)
            Dim aliasKey As String = ""
            If Not aliases Is Nothing AndAlso aliases.Count > 0 Then

                Dim currentUrl = HttpContext.Current.Request.Url.AbsoluteUri
                For Each key As String In aliases.Keys
                    If currentUrl.Contains(key) Then
                        portalAlias = aliases(key)
                    End If
                Next
            End If
            'get the portal and copy across the settings
            Dim portal As PortalInfo = pc.GetPortal(portalId)
            If Not portal Is Nothing Then

                ps.PortalAlias = portalAlias
                ps.PortalId = portal.PortalID
                ps.PortalName = portal.PortalName
                ps.LogoFile = portal.LogoFile
                ps.FooterText = portal.FooterText
                ps.ExpiryDate = portal.ExpiryDate
                ps.UserRegistration = portal.UserRegistration
                ps.BannerAdvertising = portal.BannerAdvertising
                ps.Currency = portal.Currency
                ps.AdministratorId = portal.AdministratorId
                ps.Email = portal.Email
                ps.HostFee = portal.HostFee
                ps.HostSpace = portal.HostSpace
                ps.PageQuota = portal.PageQuota
                ps.UserQuota = portal.UserQuota
                ps.AdministratorRoleId = portal.AdministratorRoleId
                ps.AdministratorRoleName = portal.AdministratorRoleName
                ps.RegisteredRoleId = portal.RegisteredRoleId
                ps.RegisteredRoleName = portal.RegisteredRoleName
                ps.Description = portal.Description
                ps.KeyWords = portal.KeyWords
                ps.BackgroundFile = portal.BackgroundFile
                ps.GUID = portal.GUID
                ps.SiteLogHistory = portal.SiteLogHistory
                ps.AdminTabId = portal.AdminTabId
                ps.SuperTabId = portal.SuperTabId
                ps.SplashTabId = portal.SplashTabId
                ps.HomeTabId = portal.HomeTabId
                ps.LoginTabId = portal.LoginTabId
                ps.UserTabId = portal.UserTabId
                ps.DefaultLanguage = portal.DefaultLanguage
                ps.TimeZoneOffset = portal.TimeZoneOffset
                ps.HomeDirectory = portal.HomeDirectory
                'ps.Version = portal.Version
                'ps.Application.Version = portal.Version

                '' ''ps.AdminSkin = SkinController.GetSkin(SkinInfo.RootSkin, portalId, SkinType.Admin)
                ' ''ps.DefaultAdminSkin = SkinController.GetSkin(SkinInfo.RootSkin, portalId, SkinType.Admin)


                '' ''If ps.AdminSkin Is Nothing Then
                '' ''    ps.AdminSkin = SkinController.GetSkin(SkinInfo.RootSkin, DotNetNuke.Common.Utilities.Null.NullInteger, SkinType.Admin)
                '' ''End If

                ' ''If ps.DefaultAdminSkin Is Nothing Then
                ' ''    ps.DefaultAdminSkin = SkinController.GetSkin(SkinInfo.RootSkin, DotNetNuke.Common.Utilities.Null.NullInteger, SkinType.Admin)
                ' ''End If

                '' ''ps.PortalSkin = SkinController.GetSkin(SkinInfo.RootSkin, portalId, SkinType.Portal)
                ' ''ps.DefaultPortalSkin = SkinController.GetSkin(SkinInfo.RootSkin, portalId, SkinType.Portal)

                '' ''If ps.PortalSkin Is Nothing Then
                '' ''    ps.PortalSkin = SkinController.GetSkin(SkinInfo.RootSkin, DotNetNuke.Common.Utilities.Null.NullInteger, SkinType.Portal)
                '' ''End If
                ' ''If ps.DefaultPortalSkin Is Nothing Then
                ' ''    ps.DefaultPortalSkin = SkinController.GetSkin(SkinInfo.RootSkin, DotNetNuke.Common.Utilities.Null.NullInteger, SkinType.Portal)
                ' ''End If

                '' ''ps.AdminContainer = SkinController.GetSkin(SkinInfo.RootContainer, portalId, SkinType.Admin)
                ' ''ps.DefaultAdminContainer = SkinController.GetSkin(SkinInfo.RootContainer, portalId, SkinType.Admin)

                '' ''If ps.AdminContainer Is Nothing Then
                '' ''    ps.AdminContainer = SkinController.GetSkin(SkinInfo.RootContainer, DotNetNuke.Common.Utilities.Null.NullInteger, SkinType.Admin)
                '' ''End If

                ' ''If ps.DefaultAdminContainer Is Nothing Then
                ' ''    ps.DefaultAdminContainer = SkinController.GetSkin(SkinInfo.RootContainer, DotNetNuke.Common.Utilities.Null.NullInteger, SkinType.Admin)
                ' ''End If

                '' ''ps.PortalContainer = SkinController.GetSkin(SkinInfo.RootContainer, portalId, SkinType.Portal)
                ' ''ps.DefaultPortalContainer = SkinController.GetSkin(SkinInfo.RootContainer, portalId, SkinType.Portal)

                '' ''If ps.PortalContainer Is Nothing Then
                '' ''    ps.PortalContainer = SkinController.GetSkin(SkinInfo.RootContainer, DotNetNuke.Common.Utilities.Null.NullInteger, SkinType.Portal)
                '' ''End If

                ' ''If ps.DefaultPortalContainer Is Nothing Then
                ' ''    ps.DefaultPortalContainer = SkinController.GetSkin(SkinInfo.RootContainer, DotNetNuke.Common.Utilities.Null.NullInteger, SkinType.Portal)
                ' ''End If

                ps.Pages = portal.Pages
                ps.Users = portal.Users
                ' set custom properties
                If DotNetNuke.Common.Utilities.Null.IsNull(ps.HostSpace) Then
                    ps.HostSpace = 0
                End If
                If DotNetNuke.Common.Utilities.Null.IsNull(ps.DefaultLanguage) Then
                    ps.DefaultLanguage = DotNetNuke.Services.Localization.Localization.SystemLocale
                End If
                If DotNetNuke.Common.Utilities.Null.IsNull(ps.TimeZoneOffset) Then
                    ps.TimeZoneOffset = DotNetNuke.Services.Localization.Localization.SystemTimeZoneOffset
                End If
                Dim prjSettings As UF.Research.Authentication.Shibboleth.ProjectSettings = New ProjectSettings
                Dim slnPath As String = prjSettings.slnPath
                ps.HomeDirectory = DotNetNuke.Common.Globals.ApplicationPath & "/" & portal.HomeDirectory & "/"
                ps.HomeDirectory = DotNetNuke.Common.Globals.ApplicationPath & "/" & portal.HomeDirectory & "/"

                ' get application version
                Dim arrVersion As String() = DotNetNuke.Common.Assembly.glbAppVersion.Split("."c)
                Dim intMajor As Integer = 0
                Dim intMinor As Integer = 0
                Dim intBuild As Integer = 0
                Int32.TryParse(arrVersion(0), intMajor)
                Int32.TryParse(arrVersion(1), intMinor)
                Int32.TryParse(arrVersion(2), intBuild)
                'ps.Version = intMajor.ToString() & "." & intMinor.ToString() & "." & intBuild.ToString()
                'ps.Application.Version = intMajor.ToString() & "." & intMinor.ToString() & "." & intBuild.ToString()

            End If

            'Add each portal Tab to DekstopTabs
            Dim portalTab As TabInfo = Nothing
            'ps.DesktopTabs = New ArrayList()
            Dim first As Boolean = True
            For Each tabPair As KeyValuePair(Of Integer, TabInfo) In tc.GetTabsByPortal(ps.PortalId)
                ' clone the tab object ( to avoid creating an object reference to the data cache )
                portalTab = tabPair.Value.Clone()
                ' set custom properties
                If portalTab.TabOrder = 0 Then
                    portalTab.TabOrder = 999
                End If
                If DotNetNuke.Common.Utilities.Null.IsNull(portalTab.StartDate) Then
                    portalTab.StartDate = System.DateTime.MinValue
                End If
                If DotNetNuke.Common.Utilities.Null.IsNull(portalTab.EndDate) Then
                    portalTab.EndDate = System.DateTime.MaxValue
                End If
                'ps.DesktopTabs.Add(portalTab)

                'assign the first 'normal' tab as the active tab - could be the home tab, if it 
                'still exists, or it will be after the admin tab(s)
                If first AndAlso (portalTab.TabID = portal.HomeTabId OrElse portalTab.TabID > portal.AdminTabId) Then
                    ps.ActiveTab = portalTab
                    first = False
                End If
            Next tabPair
            'last gasp chance in case active tab was not set
            If ps.ActiveTab Is Nothing Then
                ps.ActiveTab = portalTab
            End If
            'Add each host Tab to DesktopTabs
            Dim hostTab As TabInfo = Nothing
            For Each tabPair As KeyValuePair(Of Integer, TabInfo) In tc.GetTabsByPortal(DotNetNuke.Common.Utilities.Null.NullInteger)
                ' clone the tab object ( to avoid creating an object reference to the data cache )
                hostTab = tabPair.Value.Clone()
                hostTab.PortalID = ps.PortalId
                hostTab.StartDate = System.DateTime.MinValue
                hostTab.EndDate = System.DateTime.MaxValue
                'ps.DesktopTabs.Add(hostTab)
            Next tabPair

            'now add the portal settings to the httpContext
            If System.Web.HttpContext.Current Is Nothing Then
                'if there is no HttpContext, then mock one up by creating a fake WorkerRequest
                Dim appVirtualDir As String = DotNetNuke.Common.Globals.ApplicationPath
                Dim appPhysicalDir As String = AppDomain.CurrentDomain.BaseDirectory
                Dim page As String = ps.PortalAlias.HTTPAlias
                Dim query As String = String.Empty
                Dim output As System.IO.TextWriter = Nothing
                'create a dummy simple worker request
                Dim workerRequest As System.Web.Hosting.SimpleWorkerRequest = New System.Web.Hosting.SimpleWorkerRequest(page, query, output)
                System.Web.HttpContext.Current = New System.Web.HttpContext(workerRequest)
            End If

            'stash the portalSettings in the Context Items, where the rest of the DNN Code expects it to be
            'always remove the old portal settings and put in the current one in case a portal setting from a 
            'different portal is still being stored. 
            'then, you can probably take out the portalID specification used everywhere. 

            If System.Web.HttpContext.Current.Items("PortalSettings") IsNot Nothing Then
                System.Web.HttpContext.Current.Items.Remove("PortalSettings")
            End If

            System.Web.HttpContext.Current.Items.Add("PortalSettings", ps)

            Return ps
        End Function

    End Class

End Namespace