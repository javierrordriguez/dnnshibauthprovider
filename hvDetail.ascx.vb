
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

    Partial Public Class hvDetail
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

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs)

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


        Private Sub GetSHIBRoles()

        End Sub

        Private Sub Page_DataBinding(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.DataBinding

        End Sub

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            'Me.btnCancel.Text = Localization.GetString("btnCancel", LocalResourceFile)
            'Me.btnInsert.Text = Localization.GetString("btnInsert", LocalResourceFile)
            'Me.btnUpdate.Text = Localization.GetString("btnUpdate.Header", LocalResourceFile)
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            'save the original value of the header variable in a text box
            'check to see if there are existing role mappings using this header variable
            'if there are don't allow the update.  Instead the user must add a new header 
            'variable, change the role mappings to use the new header variable, and then
            'delete the original

            Dim psDict As System.Collections.Generic.Dictionary(Of String, String) = _
                          New System.Collections.Generic.Dictionary(Of String, String)
            Dim config As ShibConfiguration = ShibConfiguration.GetConfig()

            psDict = PortalController.GetPortalSettingsDictionary(PortalId)
            Dim strArray As Array
            Dim strValue As String
            Dim strHeaderType As String
            Dim strKeyName As String

            For i = 1 To psDict.Count
                strKeyName = "Shib_RM_" & i
                If psDict.ContainsKey(strKeyName) Then
                    strValue = psDict.Item(strKeyName)
                    strArray = strValue.Split(New Char() {config.Delimiter})
                    strHeaderType = strArray(0)
                    If Me.txtSHIBHdrVarName.Text.ToLower = strHeaderType.ToLower Then
                        Me.txtSHIBHdrVarName.Enabled = False
                        Exit For
                    Else
                        Me.txtSHIBHdrVarName.Enabled = True
                    End If


                End If
            Next

            Me.btnCancel.Text = Localization.GetString("btnCancel", LocalResourceFile)
            Me.btnInsert.Text = Localization.GetString("btnInsert", LocalResourceFile)
            Me.btnUpdate.Text = Localization.GetString("btnUpdate.Header", LocalResourceFile)

        End Sub

    End Class

End Namespace
