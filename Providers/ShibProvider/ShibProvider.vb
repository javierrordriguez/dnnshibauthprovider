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

'Imports System.DirectoryServices

Imports UF.Research.Authentication.Shibboleth
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Entities.Portals
Imports System.Collections.Generic
Imports System.Collections
'Imports System.Reflection
Imports DotNetNuke.Entities.Profile
Imports DotNetNuke.Entities.Users

Namespace UF.Research.Authentication.Shibboleth.SHIB

    Public Class ShibProvider
        Inherits ShibAuthenticationProvider

        Private _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
        Private _ShibConfiguration As ShibConfiguration '= ShibConfiguration.GetConfig()

#Region "Private Methods"

        'Private Function GetSimplyUser(ByVal UserName As String) As ShibUserInfo
        '    Dim objAuthUser As New ShibUserInfo

        '    With objAuthUser
        '        .PortalID = _portalSettings.PortalId
        '        .IsNotSimplyUser = False
        '        '.GUID = ""
        '        .Username = UserName
        '        '.FirstName = Utilities.TrimUserDomainName(UserName)
        '        '.LastName = Utilities.GetUserDomainName(UserName)
        '        .IsSuperUser = False
        '        '.Location = _adsiConfig.ConfigDomainPath
        '        '.PrincipalName = Utilities.TrimUserDomainName(UserName) & "@" & .Location
        '        '.DistinguishedName = Utilities.ConvertToDistinguished(UserName)

        '        Dim strEmail As String = _shibConfig.DefaultEmailDomain
        '        If Not strEmail.Length = 0 Then
        '            If strEmail.IndexOf("@") = -1 Then
        '                strEmail = "@" & strEmail
        '            End If
        '            strEmail = .FirstName & strEmail
        '        Else
        '            strEmail = .FirstName & "@" & .LastName & ".com" ' confusing?
        '        End If
        '        ' Membership properties
        '        .Username = UserName
        '        .Email = strEmail
        '        .Membership.Approved = True
        '        .Membership.LastLoginDate = Date.Now
        '        .Membership.Password = Utilities.GetRandomPassword() 'Membership.GeneratePassword(6)
        '        .AuthenticationExists = False
        '    End With

        '    Return objAuthUser

        'End Function


#End Region

        Private Sub FillShibUserInfo(ByVal UserInfo As ShibUserInfo)

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

                For Each UIPropertiesIN In sh.UserInfoProperties

                    'Select Case sh.UserFieldType   'will be UI or UIP
                    Select Case UIPropertiesIN.FieldType   'will be UI or UIM or UIP
                        Case "UI"
                            Select Case UIPropertiesIN.FieldName.ToLower 'for typeUI 

                                Case "displayname"
                                    If .DisplayName Is Nothing Or .DisplayName Is "" Or UIPropertiesIN.blnOverwrite Then
                                        .DisplayName = UIPropertiesIN.FieldSource.ToString
                                    End If
                                Case "email"
                                    If .Email Is Nothing Or .Email Is "" Or UIPropertiesIN.blnOverwrite Then
                                        .Email = UIPropertiesIN.FieldSource.ToString
                                    End If
                                Case "firstname"
                                    If .FirstName Is Nothing Or .FirstName Is "" Or UIPropertiesIN.blnOverwrite Then
                                        .FirstName = UIPropertiesIN.FieldSource.ToString
                                    End If
                                Case "lastname"
                                    If .LastName Is Nothing Or .LastName Is "" Or UIPropertiesIN.blnOverwrite Then
                                        .LastName = UIPropertiesIN.FieldSource.ToString
                                    End If
                                Case "username"
                                    'the first time in before you've logged in, username will be nothing.

                                    If UIPropertiesIN.FieldSource Is Nothing Then
                                        Exit Sub
                                    End If
                                    If .Email Is Nothing Or .Email = "" Or UIPropertiesIN.blnOverwrite Then
                                        .Username = UIPropertiesIN.FieldSource.ToString
                                    End If
                            End Select

                        Case "UIP" 'for type "UIP"

                            Dim strPPName As String = UIPropertiesIN.FieldName
                            Dim definition As ProfilePropertyDefinition = _
                                ProfileController.GetPropertyDefinitionByName(_portalSettings.PortalId, strPPName)

                            If definition IsNot Nothing Then 'the profile property was found
                                If definition.PropertyValue Is Nothing Or UIPropertiesIN.blnOverwrite Then
                                    definition.PropertyValue = UIPropertiesIN.FieldSource.ToString
                                End If
                            End If

                    End Select
                Next
            End With

        End Sub

        Public Sub New()
            _ShibConfiguration = ShibConfiguration.GetConfig()
        End Sub

        Public Overloads Overrides Function GetUser(ByVal LoggedOnUserName As String) As ShibUserInfo

            Dim objAuthUser As ShibUserInfo
            Dim objUser As DotNetNuke.Entities.Users.UserInfo

            ' Return authenticated if no error 
            objAuthUser = New ShibUserInfo
            'ACD-6760
            InitializeUser(objAuthUser)

            objUser = DotNetNuke.Entities.Users.UserController.GetUserByName(_portalSettings.PortalId, LoggedOnUserName)
            With objAuthUser

                .PortalID = _portalSettings.PortalId
                .Username = LoggedOnUserName
                If objUser IsNot Nothing Then
                    If objUser.DisplayName IsNot Nothing Then
                        .DisplayName = objUser.DisplayName
                    End If

                    If objUser.Email IsNot Nothing Then
                        .Email = objUser.Email
                    End If
                    If objUser.FirstName IsNot Nothing Then
                        .FirstName = objUser.FirstName
                    End If
                    If objUser.LastName IsNot Nothing Then
                        .LastName = objUser.LastName
                    End If
                End If
            End With

            FillShibUserInfo(objAuthUser)

            Return objAuthUser
        End Function
       
        'Public Overloads Function GetShibGroups() As ArrayList
        '    ' Normally number of roles in DNN less than groups in Authentication,
        '    ' so start from DNN roles to get better performance
        '    Try
        '        Dim colGroup As New ArrayList
        '        Dim objRoleController As New DotNetNuke.Security.Roles.RoleController
        '        Dim lstRoles As ArrayList = objRoleController.GetPortalRoles(_portalSettings.PortalId)
        '        Dim objRole As DotNetNuke.Security.Roles.RoleInfo
        '        'Dim AllAdGroupNames As ArrayList = Utilities.GetAllSHIBGroupnames

        '        For Each objRole In lstRoles
        '            ' Auto assignment roles have been added by DNN, so don't need to get them
        '            If Not objRole.AutoAssignment Then

        '                ' It's possible in multiple domains network that search result return more than one group with the same name (i.e Administrators)
        '                ' We better check them all
        '                If AllAdGroupNames.Contains(objRole.RoleName) Then
        '                    Dim group As New GroupInfo

        '                    With group
        '                        .PortalID = objRole.PortalID
        '                        .RoleID = objRole.RoleID
        '                         .RoleName = objRole.RoleName
        '                        .Description = objRole.Description
        '                        .ServiceFee = objRole.ServiceFee
        '                        .BillingFrequency = objRole.BillingFrequency
        '                        .TrialPeriod = objRole.TrialPeriod
        '                        .TrialFrequency = objRole.TrialFrequency
        '                        .BillingPeriod = objRole.BillingPeriod
        '                        .TrialFee = objRole.TrialFee
        '                        .IsPublic = objRole.IsPublic
        '                        .AutoAssignment = objRole.AutoAssignment
        '                    End With

        '                    colGroup.Add(group)
        '                End If
        '            End If
        '        Next

        '        Return colGroup

        '    Catch exc As System.Runtime.InteropServices.COMException
        '        LogException(exc)
        '        Return Nothing
        '    End Try
        'End Function

        Public Overrides Function GetGroups(ByVal ShibHeaderArrayInRow As Object) As ArrayList

            Try

                Dim colGroup As New ArrayList

                Dim objRoleController As New DotNetNuke.Security.Roles.RoleController

                Dim PortalID As Integer
                PortalID = _portalSettings.PortalId

                Dim lstRoles As ArrayList = objRoleController.GetPortalRoles(PortalID)

                Dim AllAdGroupRoleNames As ArrayList = ShibHeaderArrayInRow.ShibHeaderVariables

                Dim objRole As DotNetNuke.Security.Roles.RoleInfo = New DotNetNuke.Security.Roles.RoleInfo

                Dim mappedDNNRoles As ArrayList = New ArrayList
                Dim mappedRoles As ArrayList = New ArrayList

                'get a list of role mappings first. Role mappings have a Setting Name of 'Shib_RM_'.
                Dim psDict As System.Collections.Generic.Dictionary(Of String, String) = _
                New System.Collections.Generic.Dictionary(Of String, String)

                'you need to reset the cache before reading portal settings to make sure you 
                'get what is really current.

                ShibConfiguration.ResetConfig()

                psDict = PortalController.GetPortalSettingsDictionary(PortalID)

                Dim rmCount As Integer

                'Go thru loop once for each role mapping

                rmCount = 0

                Dim strRole As String = ""
                Dim strDNNRole As String = ""
                Dim strIn As String = ""
                Dim strArray As Array
                Dim strType As String = ""

                mappedDNNRoles.Clear()
                mappedRoles.Clear()

                For Each kvp As KeyValuePair(Of String, String) In psDict

                    If InStr(kvp.Key, "Shib_RM_") > 0 Then

                        strIn = kvp.Value

                        strArray = strIn.Split(New Char() {_ShibConfiguration.Delimiter})

                        strType = strArray(0)
                        strDNNRole = strArray(1)
                        strRole = strArray(2)

                        If strType.ToLower = ShibHeaderArrayInRow.ShibHeaderName.ToLower Then

                            For Each strShibRole As String In AllAdGroupRoleNames

                                'Get a list of DNN roles paired with this shib role... there may be > 1. 
                                'for each of these dnn roles store in array mappedDNNRoles. 

                                If strRole = strShibRole Then
                                    mappedDNNRoles.Add(strDNNRole)
                                    Exit For
                                End If
                            Next
                        End If

                    End If

                Next kvp

                For Each objRole In lstRoles

                    If mappedDNNRoles.Contains(objRole.RoleName) Then

                        mappedRoles.Add(objRole.RoleName)

                        'Dim group As New GroupInfo

                        'With group
                        '    .PortalID = objRole.PortalID
                        '    .RoleID = objRole.RoleID
                        '    .RoleName = objRole.RoleName
                        '    .Description = objRole.Description
                        '    .ServiceFee = objRole.ServiceFee
                        '    .BillingFrequency = objRole.BillingFrequency
                        '    .TrialPeriod = objRole.TrialPeriod
                        '    .TrialFrequency = objRole.TrialFrequency
                        '    .BillingPeriod = objRole.BillingPeriod
                        '    .TrialFee = objRole.TrialFee
                        '    .IsPublic = objRole.IsPublic
                        '    .AutoAssignment = objRole.AutoAssignment
                        'End With

                        'colGroup.Add(group)
                    End If
                Next

                'Return colGroup
                Return mappedRoles

            Catch exc As System.Runtime.InteropServices.COMException
                LogException(exc)
                Return Nothing
            End Try

        End Function

        Public Overrides Function GetAllGroups(ByVal ShibHeaderArrayInRow As Object) As ArrayList

            'Dim config As ShibConfiguration = ShibConfiguration.GetConfig()

            Try

                Dim colGroup As New ArrayList

                Dim objRoleController As New DotNetNuke.Security.Roles.RoleController

                Dim PortalID As Integer
                PortalID = _portalSettings.PortalId

                Dim lstRoles As ArrayList = objRoleController.GetPortalRoles(PortalID)

                Dim AllAdGroupRoleNames As ArrayList = ShibHeaderArrayInRow.ShibHeaderVariables

                Dim objRole As DotNetNuke.Security.Roles.RoleInfo = New DotNetNuke.Security.Roles.RoleInfo

                Dim mappedDNNRoles As ArrayList = New ArrayList

                'get a list of role mappings first. Role mappings have a Setting Name of 'Shib_RM_'.
                Dim psDict As System.Collections.Generic.Dictionary(Of String, String) = _
                New System.Collections.Generic.Dictionary(Of String, String)

                'you need to reset the cache before reading portal settings to make sure you 
                'get what is really current.

                ShibConfiguration.ResetConfig()

                psDict = PortalController.GetPortalSettingsDictionary(PortalID)

                Dim rmCount As Integer

                'Go thru loop once for each role mapping

                rmCount = 0

                Dim strRole As String = ""
                Dim strDNNRole As String = ""
                Dim strIn As String = ""
                Dim strArray As Array
                Dim strType As String = ""

                mappedDNNRoles.Clear()

                For Each kvp As KeyValuePair(Of String, String) In psDict

                    If InStr(kvp.Key, "Shib_RM_") > 0 Then

                        strIn = kvp.Value

                        'cb_0314
                        'strArray = strIn.Split(New Char() {";"c})
                        strArray = strIn.Split(New Char() {_ShibConfiguration.Delimiter})
                        'cb_0314

                        strType = strArray(0)
                        strDNNRole = strArray(1)
                        strRole = strArray(2)

                        If strType.ToLower = ShibHeaderArrayInRow.ShibHeaderName.ToLower Then

                            'For Each strShibRole As String In AllAdGroupRoleNames

                            'Get a list of DNN roles paired with this shib role... there may be > 1. 
                            'for each of these dnn roles store in array mappedDNNRoles. 

                            'If strRole = strShibRole Then
                            mappedDNNRoles.Add(strDNNRole)
                            'Exit For
                            'End If
                            'Next
                        End If

                    End If

                Next kvp

                For Each objRole In lstRoles

                    If mappedDNNRoles.Contains(objRole.RoleName) Then

                        Dim group As New GroupInfo

                        With group
                            .PortalID = objRole.PortalID
                            .RoleID = objRole.RoleID
                            .RoleName = objRole.RoleName
                            .Description = objRole.Description
                            .ServiceFee = objRole.ServiceFee
                            .BillingFrequency = objRole.BillingFrequency
                            .TrialPeriod = objRole.TrialPeriod
                            .TrialFrequency = objRole.TrialFrequency
                            .BillingPeriod = objRole.BillingPeriod
                            .TrialFee = objRole.TrialFee
                            .IsPublic = objRole.IsPublic
                            .AutoAssignment = objRole.AutoAssignment
                        End With

                        colGroup.Add(group)
                    End If
                Next

                Return colGroup

            Catch exc As System.Runtime.InteropServices.COMException
                LogException(exc)
                Return Nothing
            End Try

        End Function


        Private Sub InitializeUser(ByVal objUser As ShibUserInfo)
            If _portalSettings IsNot Nothing Then
                objUser.Profile.InitialiseProfile(_portalSettings.PortalId)

                objUser.Profile.PreferredLocale = _portalSettings.DefaultLanguage
                objUser.Profile.TimeZone = _portalSettings.TimeZoneOffset
                
            End If
        End Sub

    End Class
End Namespace
