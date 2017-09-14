
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

Imports System
Imports System.Web
Imports System.Web.UI.WebControls
Imports System.Collections.Generic
Imports System.Collections
Imports System.Web.UI
Imports System.Reflection
Imports System.IO
Imports System.Data.Sql
Imports System.Data
Imports System.Data.SqlClient
Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Security
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Services.Authentication
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Tabs
Imports Telerik.Web.UI
Imports Telerik.Web

Imports System.Globalization
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules.Actions
Imports UF.Research.Authentication.Shibboleth.SHIB


Namespace UF.Research.Authentication.Shibboleth

    Partial Public Class Settings
        'Inherits System.Web.UI.UserControl
        Inherits DotNetNuke.Services.Authentication.AuthenticationSettingsBase

        Dim _PortalSettings As PortalSettings

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            ' Obtain PortalSettings from controller
            _PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
        End Sub

        Public Overrides Sub UpdateSettings()

            Dim strLogoutPage As String = Me.ddlLogoutPage.SelectedValue
            Dim strLoginPage As String = Me.ddlLoginPage.SelectedValue
            Try
                If strLogoutPage Is Nothing Then
                    'cb_062711
                    'strLogoutPage = "home.aspx"
                    strLogoutPage = "default.aspx"
                End If
                If strLoginPage Is Nothing Then
                    'cb_062711
                    'strLogoutPage = "home.aspx"
                    strLogoutPage = "default.aspx"
                End If

                'currently we have the delimiter field disabled, so set it to "|" if it has nothing in it. 
                If Me.txtDelimiter.Text = "" Then
                    Me.txtDelimiter.Text = "|"
                End If
               
                ShibConfiguration.UpdateConfig(_portalSettings.PortalId, Me.chkEnabled.Checked, Me.chkAutoCreateUsers.Checked, Me.chkSynchronizeRoles.Checked, Me.txtDelimiter.Text, strLogoutPage, strLoginPage, Me.chkSimulateLogin.Checked)

                'the configuration is cached.  If you change the portal_settings table, the cache
                'will not be rebuilt and your test may fail.  If you use the settings module to 
                'update the portal_settings value, the cache will be rebuilt with the new values.
                ShibConfiguration.ResetConfig()

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Try

                AddShibLogTypes()

                ' Obtain PortalSettings from controller

                ' Reset config

                ShibConfiguration.ResetConfig()
                Dim config As ShibConfiguration = ShibConfiguration.GetConfig()

                Dim psDict As System.Collections.Generic.Dictionary(Of String, String) = _
                New System.Collections.Generic.Dictionary(Of String, String)
                psDict = PortalController.GetPortalSettingsDictionary(PortalId)

                If Not Page.IsPostBack Then

                    If psDict.ContainsKey("Shib_Authentication") Then

                        chkEnabled.Checked = psDict.Item("Shib_Authentication")
                        chkSynchronizeRoles.Checked = psDict.Item("Shib_SynchronizeRoles")
                        chkAutoCreateUsers.Checked = psDict.Item("Shib_AutoCreateUsers")
                        txtDelimiter.Text = psDict.Item("Shib_Delimiter")
                        chkSimulateLogin.Checked = psDict.Item("Shib_SimulateLogin")

                    Else

                        'set current configuration into the form controls for user display
                        chkEnabled.Checked = config.Enabled
                        chkSynchronizeRoles.Checked = config.SynchronizeRoles
                        chkAutoCreateUsers.Checked = config.AutoCreateUsers
                        txtDelimiter.Text = config.Delimiter
                        chkSimulateLogin.Checked = config.SimulateLogin

                    End If
                End If

                Dim objTabController As New TabController
                Dim TabCollection As DotNetNuke.Entities.Tabs.TabCollection = New DotNetNuke.Entities.Tabs.TabCollection
                TabCollection = objTabController.GetTabsByPortal(PortalId)
                Dim lstTabNames As List(Of String) = New List(Of String)

                For Each GenericKeyValuePair In TabCollection
                    Dim TabInfo As DotNetNuke.Entities.Tabs.TabInfo = GenericKeyValuePair.Value
                    If TabInfo.CreatedByUserID <> -1 And Not TabInfo.IsDeleted Then
                        Dim pName As String = TabInfo.TabName & ".aspx"
                        lstTabNames.Add(pName)
                    End If
                Next
                lstTabNames.Sort()
                Me.ddlLogoutPage.DataSource = lstTabNames
                Me.ddlLogoutPage.DataBind()

                Dim txtPageName As String = config.LogoutPage
                ddlLogoutPage.SelectedIndex = (ddlLogoutPage.Items.IndexOf(ddlLogoutPage.Items.FindByText(txtPageName)))

                Me.ddlLoginPage.DataSource = lstTabNames
                Me.ddlLoginPage.DataBind()

                txtPageName = config.LoginPage
                ddlLoginPage.SelectedIndex = (ddlLoginPage.Items.IndexOf(ddlLoginPage.Items.FindByText(txtPageName)))


            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub DisplayMessage(ByVal text As String)
            'Label1.Text = String.Format("<span>{0}</span>", text)
        End Sub

        'Private Sub SetMessage(ByVal message As String)
        '    gridMessage = message
        'End Sub

        'Private gridMessage As String = Nothing

        Protected Sub Button_Click(ByVal sender As Object, ByVal e As EventArgs)

            Dim btn As Button = CType(sender, Button)
            Dim editFormItem As GridEditFormItem = CType(btn.NamingContainer, GridEditFormItem) ' access the EditFormItem
            Dim txtbx As TextBox = CType(editFormItem.FindControl("Textbox"), TextBox)
            Dim upload As FileUpload = CType(editFormItem.FindControl("FileUpload"), FileUpload)
        End Sub

        Private Sub AddShibLogTypes()

            If Not Utilities.LogTypeKeyInstalled(ShibAlert) Then
                ' Perform add operation for custom log type

                'Dim logController = New LogController()
                Dim logController = New DotNetNuke.Services.Log.EventLog.EventLogController()
                'DotNetNuke.Services.Log.EventLog.EventLogController()
                Try

                    Dim logTypeInfo = New DotNetNuke.Services.Log.EventLog.LogTypeInfo

                    logTypeInfo.LogTypeCSSClass = "GeneralAdminOperation"
                    logTypeInfo.LogTypeDescription = "Shibboleth Logging"
                    logTypeInfo.LogTypeFriendlyName = ShibAlert
                    logTypeInfo.LogTypeOwner = "DotNetNuke.Logging.EventLogType"
                    logTypeInfo.LogTypeKey = ShibAlert ' Pick unique key name
                    logController.AddLogType(logTypeInfo)
                Catch ex As Exception
                    'log the exception, but do not abort execution
                    LogException(ex)
                End Try

                'Dim hc As DotNetNuke.Entities.Controllers.HostController

                Dim logTypeConfigInfo = New DotNetNuke.Services.Log.EventLog.LogTypeConfigInfo
                logTypeConfigInfo.LogTypeKey = ShibAlert
                logTypeConfigInfo.LoggingIsActive = False
                'http://www.dotnetnukeru.com/dnndocs/api/html/P_DotNetNuke_Entities_Portals_PortalSettings_HostSettings.htm
                logTypeConfigInfo.MailFromAddress = DotNetNuke.Entities.Host.Host.HostEmail
                logTypeConfigInfo.MailToAddress = DotNetNuke.Entities.Host.Host.HostEmail
                logTypeConfigInfo.EmailNotificationIsActive = False

                logTypeConfigInfo.NotificationThreshold = AcceptRejectRule.None
                logTypeConfigInfo.NotificationThresholdTime = AcceptRejectRule.None
                logTypeConfigInfo.NotificationThresholdTimeType = Services.Log.EventLog.LogTypeConfigInfo.NotificationThresholdTimeTypes.Days

                logTypeConfigInfo.KeepMostRecent = "20"

                logController.AddLogTypeConfigInfo(logTypeConfigInfo)

                DataCache.ClearHostCache(True)
            End If

        End Sub

    End Class

End Namespace