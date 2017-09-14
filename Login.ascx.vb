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

Imports System.Web
Imports System.Web.UI.WebControls
Imports System.Collections.Generic
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Entities.Host
Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Security
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Users
Imports System.Collections
Imports System.Web.UI
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Services.Authentication
Imports DotNetNuke.Entities.Portals
Imports System.Reflection
Imports System.IO
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Security.Permissions

Imports DotNetNuke.Common.Utilities
Imports DNNUserController = DotNetNuke.Entities.Users.UserController
Imports DNNUserInfo = DotNetNuke.Entities.Users.UserInfo
Imports DotNetNuke.UI.Skins.Controls.ModuleMessage
Imports DotNetNuke.UI.WebControls

Imports UF.Research.Authentication.Shibboleth
Imports System

Namespace UF.Research.Authentication.Shibboleth

    Partial Public Class Login
        Inherits DotNetNuke.Services.Authentication.AuthenticationLoginBase
        'Inherits System.Web.UI.UserControl

#Region "Protected Properties"

        Protected ReadOnly Property UseCaptcha() As Boolean
            Get
                Return AuthenticationConfig.GetConfig(PortalId).UseCaptcha
            End Get
        End Property

#End Region

#Region "Public Properties"


        Public Overrides ReadOnly Property Enabled() As Boolean
            Get
                'Return AuthenticationConfig.GetConfig(PortalId).Enabled
                Dim _ShibConfiguration As ShibConfiguration = ShibConfiguration.GetConfig()
                Return _ShibConfiguration.Enabled
            End Get
        End Property

#End Region

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Dim _ShibConfiguration As ShibConfiguration = ShibConfiguration.GetConfig()
            If _ShibConfiguration Is Nothing Then
                Exit Sub
            End If

            Dim objPortalSettings As PortalSettings = Nothing
            objPortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            If objPortalSettings Is Nothing Then Exit Sub

            Dim portalID As Integer = objPortalSettings.PortalId

            Dim psDictionary As Dictionary(Of String, String) = _
              PortalController.GetPortalSettingsDictionary(portalID)

            If psDictionary.ContainsKey("Shib_Authentication") Then

                If psDictionary.Item("Shib_Authentication") = "True" Then
                    Me.cmdLogin.Enabled = True
                    Me.lblShibEnabled.Visible = True
                    Me.lblShibNotEnabled.Visible = False
                Else
                    Me.cmdLogin.Enabled = False
                    Me.lblShibEnabled.Visible = False
                    Me.lblShibNotEnabled.Visible = True
                End If
            Else
                Me.cmdLogin.Enabled = False
                Me.lblShibEnabled.Visible = False
                Me.lblShibNotEnabled.Visible = True

            End If

        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load

            'DotNetNuke.Services.Exceptions.LogException(New Exception("Page Load"))

            'DotNetNuke.UI.Utilities.ClientAPI.RegisterKeyCapture(Me.Parent, Me.cmdLogin, Asc(vbCr))
            Dim _ShibConfiguration As ShibConfiguration = ShibConfiguration.GetConfig()
            If _ShibConfiguration Is Nothing Then
                Exit Sub
            End If

            Dim psDictionary As Dictionary(Of String, String) =
             PortalController.GetPortalSettingsDictionary(PortalId)

            'DotNetNuke.Services.Exceptions.LogException(New Exception("shib auth is in dictionary" + psDictionary.ContainsKey("Shib_Authentication").ToString()))
            If psDictionary.ContainsKey("Shib_Authentication") Then
                'If Request.QueryString("NoShib") IsNot Nothing AndAlso Request.QueryString("NoShib") <> "" Then
                If Not Request.QueryString.AllKeys.Contains("noshib") Then
                    If psDictionary.Item("Shib_Authentication") = "True" Then
                        Dim redir As String = "~/DesktopModules/AuthenticationServices/Shibboleth/Login/ShibHandler.ashx?" & PortalId
                        Response.Redirect(redir)

                    End If
                End If

            End If

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

        End Sub


        Private Sub cmdLogin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdLogin.Click

            'System.Diagnostics.Debugger.Break()
            Dim _ShibConfiguration As ShibConfiguration = ShibConfiguration.GetConfig()
            If _ShibConfiguration Is Nothing Then
                Exit Sub
            End If

            Dim psDictionary As Dictionary(Of String, String) =
             PortalController.GetPortalSettingsDictionary(PortalId)

            'DotNetNuke.Services.Exceptions.LogException(New Exception("shib auth is in dictionary" + psDictionary.ContainsKey("Shib_Authentication").ToString()))
            If psDictionary.ContainsKey("Shib_Authentication") Then
                'If Request.QueryString("NoShib") IsNot Nothing AndAlso Request.QueryString("NoShib") <> "" Then
                If psDictionary.Item("Shib_Authentication") = "True" Then
                    Dim redir As String = "~/DesktopModules/AuthenticationServices/Shibboleth/Login/ShibHandler.ashx?" & PortalId
                    Response.Redirect(redir)

                End If

            End If

        End Sub

#End Region

    End Class

End Namespace
