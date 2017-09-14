
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
Imports DotNetNuke
Imports DotNetNuke.Common
Imports DotNetNuke.Security
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Entities.Profile
Imports System.Collections
Imports System.Web.UI
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Services.Authentication
Imports DotNetNuke.Entities.Portals
Imports System.Reflection
Imports System.IO
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Tabs
Imports Telerik.Web.UI
Imports Telerik.Web

Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient

Imports System.Data.Sql
Imports System.Globalization
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Modules.Actions


Namespace UF.Research.Authentication.Shibboleth

    Partial Public Class uaDetail

        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

        Private _dataItem As Object = Nothing

        Private _RMID As String

        Public Property RMID() As String
            Get
                Return labelRMID.Text
            End Get
            Set(ByVal value As String)
                _RMID = value
            End Set
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        End Sub


#Region "Web Form Designer generated code"

        '/ <summary>
        '/        Required method for Designer support - do not modify
        '/        the contents of this method with the code editor.
        '/ </summary>

#End Region

        Public Property DataItem() As Object
            Get
                Return Me._dataItem
            End Get
            Set(ByVal value As Object)
                Me._dataItem = value
            End Set
        End Property

        Private Sub GetUserProfileProperties()

            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)

            Dim pc As ProfileController = New ProfileController

            Dim properties As ProfilePropertyDefinitionCollection = ProfileController.GetPropertyDefinitionsByPortal(PortalId)
            Dim ppDef As ProfilePropertyDefinition
            Me.ddlDNNProperty.Items.clear()

            Dim i As Integer = 0
            For Each ppDef In properties

                Me.ddlDNNProperty.Items.Add(ppDef.PropertyName)
                Me.ddlDNNProperty.Items(i).Value = ppDef.PropertyDefinitionId
                Me.ddlDNNProperty.Items(i).Text = ppDef.PropertyName
                i = i + 1
            Next

            Me.ddlDNNProperty.DataBind()

        End Sub

        Private Sub GetUserProperties()

            Dim _portalSettings As PortalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)

            Dim i As Integer = 0

            Dim objUser As New UserController

            Me.ddlDNNProperty.Items.Clear()

            Me.ddlDNNProperty.Items.Add("DisplayName")
            Me.ddlDNNProperty.Items(0).Value = 0
            Me.ddlDNNProperty.Items(0).Text = ("DisplayName")

            Me.ddlDNNProperty.Items.Add("Email")
            Me.ddlDNNProperty.Items(1).Value = 1
            Me.ddlDNNProperty.Items(1).Text = "Email"

            Me.ddlDNNProperty.Items.Add("FirstName")
            Me.ddlDNNProperty.Items(2).Value = 2
            Me.ddlDNNProperty.Items(2).Text = "FirstName"

            Me.ddlDNNProperty.Items.Add("LastName")
            Me.ddlDNNProperty.Items(3).Value = 3
            Me.ddlDNNProperty.Items(3).Text = "LastName"

            Me.ddlDNNProperty.Items.Add("UserName")
            Me.ddlDNNProperty.Items(4).Value = 4
            Me.ddlDNNProperty.Items(4).Text = "UserName"

            Me.ddlDNNProperty.DataBind()

        End Sub

        Private Sub GetTypes()

            Dim alType As New ArrayList(New String() {"User", "UserProfile"})
            Me.ddlType.DataSource = alType
            ddlType.DataBind()

            Dim TypeValue As Object = DataBinder.Eval(DataItem, "Type")

            If TypeValue.Equals(DBNull.Value) Then
                TypeValue = "User"

            End If
        End Sub

        Private Sub GetOverwrite()

            Dim OverwriteValue As Object = DataBinder.Eval(DataItem, "Overwrite")

            If OverwriteValue.Equals(DBNull.Value) Then
                OverwriteValue = True
            Else
                Me.chkOverwrite.Checked = OverwriteValue
            End If

            Me.chkOverwrite.Checked = OverwriteValue
        End Sub

        Private Sub Page_DataBinding(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.DataBinding

            'GetUserProperties()

            GetTypes()

            If ddlType.SelectedItem.Text = "User" Then
                GetUserProperties()
            Else
                GetUserProfileProperties()
            End If

            GetOverwrite()

        End Sub

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            If Me.txtDNNProperty.Text <> "" Then

                If Me.ddlDNNProperty.Items.FindByText(Me.txtDNNProperty.Text) IsNot Nothing Then

                    Me.ddlDNNProperty.Items.FindByText(Me.txtDNNProperty.Text).Selected = True

                End If

            End If

            If (Me.ddlType.Items.FindByText("User").Selected = False) And (Me.ddlType.Items.FindByText("UserProfile").Selected = False) Then

                If Me.txtType.Text = "UI" Then

                    GetUserProperties()
                    Me.ddlType.Items.FindByText("User").Selected = True
                Else

                    GetUserProfileProperties()
                    Me.ddlType.Items.FindByText("UserProfile").Selected = True
                End If

                If Me.ddlDNNProperty.Items.FindByText(Me.txtDNNProperty.Text) IsNot Nothing Then

                    Me.ddlDNNProperty.Items.FindByText(Me.txtDNNProperty.Text).Selected = True

                End If

                Me.btnCancel.Text = Localization.GetString("btnCancel", LocalResourceFile)
                Me.btnInsert.Text = Localization.GetString("btnInsert", LocalResourceFile)
                Me.btnUpdate.Text = Localization.GetString("btnUpdate.Header", LocalResourceFile)
            End If
        End Sub

        Protected Sub ddlDNNProperty_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlDNNProperty.SelectedIndexChanged

        End Sub

        Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUpdate.Click

        End Sub

        Protected Sub ddlType_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles ddlType.SelectedIndexChanged

            If ddlType.SelectedItem.Text = "User" Then
                GetUserProperties()
            Else
                GetUserProfileProperties()
            End If

        End Sub
    End Class
End Namespace
