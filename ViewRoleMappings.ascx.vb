' 061611
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


'
' UF.Resarch 
' Copyright (c) 2010 UF Research
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
' OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.


''' ----------------------------------------------------------------------------- 
''' <summary> 
''' The ViewRoleMapping class displays Role Mappings in a RadGrid
''' </summary> 
''' ----------------------------------------------------------------------------- 
''' 
Namespace UF.Research.Authentication.Shibboleth
    Partial Class ViewRoleMappings
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase
        Implements IActionable

        'Private _portalSettings As PortalSettings

        Dim _portalSettings As PortalSettings
        Dim _psDict As System.Collections.Generic.Dictionary(Of String, String)

        ''' ----------------------------------------------------------------------------- 
        ''' <summary> 
        ''' OnInit runs when the control is initialized 
        ''' </summary> 
        ''' ----------------------------------------------------------------------------- 
        Protected Overloads Overrides Sub OnInit(ByVal e As EventArgs)
            MyBase.OnInit(e)
            AddHandler Load, AddressOf Page_Load
        End Sub


        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            'Dim config As ShibConfiguration = ShibConfiguration.GetConfig()
            _portalSettings = CType(HttpContext.Current.Items("PortalSettings"), PortalSettings)
            If ViewState("psDict") Is Nothing Then
                _psDict = PortalController.GetPortalSettingsDictionary(_portalSettings.PortalId)
            Else
                _psDict = ViewState("psDict")
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)

            If _psDict.ContainsKey("Shib_Authentication") Then

                Try

                    Dim slnPath As String = ""
                    Dim reDir As String = ""
                    GetSolutionPath(slnPath)

                    If Me.RadGrid10.MasterTableView.EditFormSettings.UserControlName = "rmDetail.ascx" Then
                        Dim myURL As String = Globals.NavigateURL
                        Dim lastSlashPos As Integer = myURL.LastIndexOf("/")
                        myURL = Left(myURL, lastSlashPos)
                        reDir = myURL + "/DesktopModules/AuthenticationServices/Shibboleth/" + RadGrid10.MasterTableView.EditFormSettings.UserControlName
                        reDir = "~/DesktopModules/AuthenticationServices/Shibboleth/" + RadGrid10.MasterTableView.EditFormSettings.UserControlName

                        Me.RadGrid10.MasterTableView.EditFormSettings.UserControlName = reDir

                    End If

                    If Me.RadGrid20.MasterTableView.EditFormSettings.UserControlName = "uaDetail.ascx" Then
                        Dim myURL As String = Globals.NavigateURL
                        Dim lastSlashPos As Integer = myURL.LastIndexOf("/")
                        myURL = Left(myURL, lastSlashPos)
                        reDir = myURL + "/DesktopModules/AuthenticationServices/Shibboleth/" + RadGrid20.MasterTableView.EditFormSettings.UserControlName
                        reDir = "~/DesktopModules/AuthenticationServices/Shibboleth/" + RadGrid20.MasterTableView.EditFormSettings.UserControlName

                        Me.RadGrid20.MasterTableView.EditFormSettings.UserControlName = reDir

                    End If

                    If Me.RadGrid30.MasterTableView.EditFormSettings.UserControlName = "hvDetail.ascx" Then
                        Dim myURL As String = Globals.NavigateURL
                        Dim lastSlashPos As Integer = myURL.LastIndexOf("/")
                        myURL = Left(myURL, lastSlashPos)
                        reDir = myURL + "/DesktopModules/AuthenticationServices/Shibboleth/" + RadGrid30.MasterTableView.EditFormSettings.UserControlName
                        reDir = "~/DesktopModules/AuthenticationServices/Shibboleth/" + RadGrid30.MasterTableView.EditFormSettings.UserControlName

                        Me.RadGrid30.MasterTableView.EditFormSettings.UserControlName = reDir

                    End If

                    ' Obtain PortalSettings from controller
                    btnUpdateRoleMappings.Text = Localization.GetString("btnUpdateRoleMappings.Text", LocalResourceFile)
                    btnUpdateAttributes.Text = Localization.GetString("btnUpdateAttributes.Text", LocalResourceFile)

                Catch exc As ModuleLoadException
                    'Module failed to load 
                    Exceptions.ProcessModuleLoadException(Me, exc)
                End Try

            End If

        End Sub


#Region "Optional Interfaces"

        ''' ----------------------------------------------------------------------------- 
        ''' <summary> 
        ''' Registers the module actions required for interfacing with the portal framework 
        ''' </summary> 
        ''' ----------------------------------------------------------------------------- 
        ReadOnly Property ModuleActions() As Actions.ModuleActionCollection Implements IActionable.ModuleActions
            Get
                Dim actions As New ModuleActionCollection()
                actions.Add(GetNextActionID(), Localization.GetString("AddContent.Action", LocalResourceFile), ModuleActionType.AddContent, "", "", EditUrl(), _
                False, SecurityAccessLevel.Edit, True, False)
                Return actions
            End Get
        End Property

#End Region


        Private Function GetRMTable() As DataTable

            Dim config As ShibConfiguration = ShibConfiguration.GetConfig()

            If ViewState("rmDataTable") IsNot Nothing Then
                Return ViewState("rmDataTable")
                Exit Function
            End If

            Dim rmDataTable As DataTable = New DataTable("rmDataTable")
            Dim column As DataColumn
            Dim row As DataRow

            column = New DataColumn()
            column.DataType = System.Type.GetType("System.Int32")
            column.ColumnName = "RMID"
            column.ReadOnly = False
            column.Unique = True
            column.AutoIncrement = True
            column.AutoIncrementSeed = 0
            rmDataTable.Columns.Add(column)

            ' Make the ID column the primary key column.
            Dim PrimaryKeyColumns(0) As DataColumn
            PrimaryKeyColumns(0) = rmDataTable.Columns("RMID")
            rmDataTable.PrimaryKey = PrimaryKeyColumns

            'add a column for the Portal Settings DNN Role field
            column = New DataColumn()
            column.DataType = System.Type.GetType("System.String")
            column.ColumnName = "DNNRoleName"
            column.ReadOnly = False
            column.Unique = False

            rmDataTable.Columns.Add(column)

            'add a column for the Portal Settings DNN Role ID field
            column = New DataColumn()
            column.DataType = System.Type.GetType("System.Int32")
            column.ColumnName = "DNNRoleID"
            column.ReadOnly = False
            column.Unique = False

            rmDataTable.Columns.Add(column)

            'add a column for the Portal Settings SHIB Role Type field
            column = New DataColumn()
            column.DataType = System.Type.GetType("System.String")
            column.ColumnName = "ShibRoleType"
            column.ReadOnly = False
            column.Unique = False

            rmDataTable.Columns.Add(column)

            'add a column for the Portal Settings Shibboleth Role Name field
            column = New DataColumn()
            column.DataType = System.Type.GetType("System.String")
            column.ColumnName = "ShibRoleName"
            column.ReadOnly = False
            column.Unique = False

            rmDataTable.Columns.Add(column)

            'add a column for the Portal Settings Shibboleth Role ID field
            column = New DataColumn()
            column.DataType = System.Type.GetType("System.Int32")
            column.ColumnName = "ShibRoleID"
            column.ReadOnly = False
            column.Unique = False

            rmDataTable.Columns.Add(column)

            Dim strKeyName As String
            Dim strValue As String
            Dim strArray As Array

            '''''''''''''''''''''''''
            ''get all portal settings for PortalID = 0 and read them into a datatable
            '''''''''''''''''''''''''

            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            'get all of the portal settings for the current portal and read them into a dictionary field 

            ShibConfiguration.ResetConfig()

            ViewState("psDict") = _psDict
            'this isn't the actual count of role mappings, it's the count of all portal settings
            'for this portal, which is greater than the count of role mappings,
            'but it will do to give us a starting integer counter for the number of role mappings
            'ViewState("psDictCounter") = psDict.Count

            Dim rmCount As Integer '= psDict("Shib_RoleMappingCount")

            'Go thru loop once for each role mapping
            For i = 1 To _psDict.Count
                strKeyName = "Shib_RM_" & i
                If _psDict.ContainsKey(strKeyName) Then
                    rmCount = i
                Else
                    Exit For
                End If
            Next
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            For i = 1 To rmCount

                strKeyName = "Shib_RM_" & i.ToString
                'read the values from the dictionary into the datatable, row by row
                If _psDict.Item(strKeyName) IsNot Nothing Then
                    strValue = _psDict.Item(strKeyName)
                    row = rmDataTable.NewRow

                    row("DNNRoleID") = 0
                    row("ShibRoleID") = 0

                    '''''''''''''''''''''''''''''''''''''''''''''

                    strArray = strValue.Split(New Char() {config.Delimiter})
                   
                    row("SHIBRoleType") = strArray(0)

                    'get SHIB Role Type
                    row("SHIBRoleType") = strArray(0)
                    'get DNN Role
                    row("DNNRoleName") = strArray(1)
                    'get SHIB Role Name
                    row("SHIBRoleName") = strArray(2)

                    rmDataTable.Rows.Add(row)

                Else
                    Exit For
                End If
            Next

            ViewState("rmDataTable") = rmDataTable

            Return rmDataTable

        End Function


        Private Function GetHVTable() As DataTable

            Dim config As ShibConfiguration = ShibConfiguration.GetConfig()

            If ViewState("hvDataTable") IsNot Nothing Then
                Return ViewState("hvDataTable")
                Exit Function
            End If

            Dim hvDataTable As DataTable = New DataTable("hvDataTable")
            Dim column As DataColumn
            Dim row As DataRow

            column = New DataColumn()
            column.DataType = System.Type.GetType("System.Int32")
            column.ColumnName = "RMID"
            column.ReadOnly = False
            column.Unique = True
            column.AutoIncrement = True
            column.AutoIncrementSeed = 0
            hvDataTable.Columns.Add(column)

            ' Make the ID column the primary key column.
            Dim PrimaryKeyColumns(0) As DataColumn
            PrimaryKeyColumns(0) = hvDataTable.Columns("RMID")
            hvDataTable.PrimaryKey = PrimaryKeyColumns

            'add a column for the Portal Settings Shib Header Variable Name field
            column = New DataColumn()
            column.DataType = System.Type.GetType("System.String")
            column.ColumnName = "ShibHdrVarName"
            column.ReadOnly = False
            column.Unique = False

            hvDataTable.Columns.Add(column)

            'add a column for the Portal Settings DNN Role ID field
            column = New DataColumn()
            column.DataType = System.Type.GetType("System.Char")
            column.ColumnName = "ShibHdrVarDelim"
            column.ReadOnly = False
            column.Unique = False

            hvDataTable.Columns.Add(column)

            Dim strKeyName As String
            Dim strValue As String
            Dim strArray As Array

            '''''''''''''''''''''''''
            ''get all portal settings for PortalID = 0 and read them into a datatable
            '''''''''''''''''''''''''

            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            'get all of the portal settings for the current portal and read them into a dictionary field 

            ShibConfiguration.ResetConfig()

            ViewState("psDict") = _psDict
            'this isn't the actual count of header variables, it's the count of all portal settings
            'for this portal, which is greater than the count of header variables,
            'but it will do to give us a starting integer counter for the number of header variables
           
            Dim rmCount As Integer

            'Go thru loop once for each shib header variable
            For i = 1 To _psDict.Count
                strKeyName = "Shib_HeaderItem_" & i
                If _psDict.ContainsKey(strKeyName) Then
                    rmCount = i
                Else
                    Exit For
                End If
            Next
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            For i = 1 To rmCount

                strKeyName = "Shib_HeaderItem_" & i.ToString
                'read the values from the dictionary into the datatable, row by row
                If _psDict.Item(strKeyName) IsNot Nothing Then
                    strValue = _psDict.Item(strKeyName)
                    row = hvDataTable.NewRow

                    strArray = strValue.Split(config.Delimiter)

                    'get SHIB Header Variable Name
                    row("SHIBHdrVarName") = strArray(0)
                    'get Shib Header Variable delimiter
                    row("SHIBHdrVarDelim") = strArray(1)

                    hvDataTable.Rows.Add(row)

                Else
                    Exit For
                End If
            Next

            ViewState("hvDataTable") = hvDataTable

            Return hvDataTable

        End Function


        Private Function GetUATable() As DataTable

            Dim config As ShibConfiguration = ShibConfiguration.GetConfig()

            If ViewState("uaDataTable") IsNot Nothing Then
                Return ViewState("uaDataTable")
                Exit Function
            End If

            Dim uaDataTable As DataTable = New DataTable("uaDataTable")
            Dim column As DataColumn
            Dim row As DataRow

            column = New DataColumn()
            column.DataType = System.Type.GetType("System.Int32")
            column.ColumnName = "RMID"
            column.ReadOnly = False
            column.Unique = True
            column.AutoIncrement = True
            column.AutoIncrementSeed = 0
            uaDataTable.Columns.Add(column)

            ' Make the ID column the primary key column.
            Dim PrimaryKeyColumns(0) As DataColumn
            PrimaryKeyColumns(0) = uaDataTable.Columns("RMID")
            uaDataTable.PrimaryKey = PrimaryKeyColumns

            'add a column for the attribute Type field
            column = New DataColumn()
            column.DataType = System.Type.GetType("System.String")
            column.ColumnName = "Type"
            column.ReadOnly = False
            column.Unique = False

            uaDataTable.Columns.Add(column)

            ''add a column for the attribute property field
            
            'add a column for the attribute property field
            column = New DataColumn()
            column.DataType = System.Type.GetType("System.String")
            column.ColumnName = "DNNProperty"
            column.ReadOnly = False
            column.Unique = False

            uaDataTable.Columns.Add(column)

            'add a column for the attribute Shibboleth source field
            column = New DataColumn()
            column.DataType = System.Type.GetType("System.String")
            column.ColumnName = "Source"
            column.ReadOnly = False
            column.Unique = False

            uaDataTable.Columns.Add(column)

            'add a column for the overwrite flag
            column = New DataColumn()
            column.DataType = System.Type.GetType("System.Boolean")
            column.ColumnName = "Overwrite"
            column.ReadOnly = False
            column.Unique = False

            uaDataTable.Columns.Add(column)
            ''add a column for the Portal Settings Shibboleth Role ID field

            Dim strKeyName As String
            Dim strValue As String
            Dim strArray As Array

            '''''''''''''''''''''''''
            ''get all portal settings for PortalID = 0 and read them into a datatable
            '''''''''''''''''''''''''

            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            'get all of the portal settings for the current portal and read them into a dictionary field 

            ShibConfiguration.ResetConfig()

            ViewState("psDict") = _psDict
            'this isn't the actual count of role mappings, it's the count of all portal settings
            'for this portal, which is greater than the count of role mappings,
            'but it will do to give us a starting integer counter for the number of role mappings
            'ViewState("psDictCounter") = psDict.Count

            Dim rmCount As Integer '= psDict("Shib_RoleMappingCount")

            'Go thru loop once for each user attribute
            For i = 1 To _psDict.Count
                strKeyName = "Shib_UserMap_" & i
                If _psDict.ContainsKey(strKeyName) Then
                    rmCount = i
                Else
                    Exit For
                End If
            Next
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            For i = 1 To rmCount

                strKeyName = "Shib_UserMap_" & i.ToString
                'read the values from the dictionary into the datatable, row by row
                If _psDict.Item(strKeyName) IsNot Nothing Then
                    strValue = _psDict.Item(strKeyName)
                    row = uaDataTable.NewRow

                    'cb_0314

                    strArray = strValue.Split(New Char() {config.Delimiter})

                    If strArray(0) = "UI" Then
                        row("Type") = "User"
                    Else
                        row("Type") = "UserProfile"
                    End If

                    'get SHIB Role Type
                    row("Type") = strArray(0)
                    'get DNN Role
                    row("DNNProperty") = strArray(1)
                    'get SHIB Role Name
                    row("Source") = strArray(2)
                    'get Overwrite
                    row("Overwrite") = strArray(3)

                    uaDataTable.Rows.Add(row)

                Else
                    Exit For
                End If
            Next

            ViewState("uaDataTable") = uaDataTable

            Return uaDataTable

        End Function


        Private Sub RadGrid1_NeedDataSource(ByVal source As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles RadGrid10.NeedDataSource
            If _psDict.ContainsKey("Shib_Authentication") Then
                Dim dataView As DataView = New DataView(GetRMTable())
                dataView.Sort = "SHIBRoleType ASC, DNNRoleName ASC, SHIBRoleName ASC"
                Me.RadGrid10.DataSource = dataView
            End If
        End Sub

        Private Sub RadGrid2_NeedDataSource(ByVal source As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles RadGrid20.NeedDataSource
            If _psDict.ContainsKey("Shib_Authentication") Then
                Dim dataView As DataView = New DataView(GetUATable())
                dataView.Sort = "Type ASC, DNNProperty ASC, Source ASC"
                Me.RadGrid20.DataSource = dataView
            End If
        End Sub

        Private Sub RadGrid3_NeedDataSource(ByVal source As Object, ByVal e As Telerik.Web.UI.GridNeedDataSourceEventArgs) Handles RadGrid30.NeedDataSource
            If _psDict.ContainsKey("Shib_Authentication") Then
                Dim dataView As DataView = New DataView(GetHVTable())
                dataView.Sort = "SHIBHdrVarName ASC, SHIBHdrVarDelim ASC"
                Me.RadGrid30.DataSource = dataView
            End If
        End Sub

        Protected Sub RadGrid3_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles RadGrid30.ItemDataBound
            If _psDict.ContainsKey("Shib_Authentication") Then
                If TypeOf e.Item Is GridDataItem Then
                    Dim item As GridDataItem = CType(e.Item, GridDataItem)
                    Dim btnDelete As LinkButton = CType(item("DeleteColumn").Controls(0), LinkButton) ' accessing Delete Button
                    Dim strHdrItem As String = DataBinder.Eval(item.DataItem, "SHIBHdrVarName")


                    '    'save the original value of the header variable
                    '    'check to see if there are existing role mappings using this header variable
                    '    'if there are don't allow the update.  Instead the user must add a new header 
                    '    'variable, change the role mappings to use the new header variable, and then
                    '    'delete the original

                    Dim config As ShibConfiguration = ShibConfiguration.GetConfig()

                    Dim strArray As Array
                    Dim strValue As String
                    Dim strHeaderType As String
                    Dim strKeyName As String

                    For i = 1 To _psDict.Count
                        strKeyName = "Shib_RM_" & i
                        If _psDict.ContainsKey(strKeyName) Then
                            strValue = _psDict.Item(strKeyName)
                            strArray = strValue.Split(New Char() {config.Delimiter})
                            strHeaderType = strArray(0)
                            If strHdrItem.ToLower = strHeaderType.ToLower Then
                                btnDelete.Enabled = False
                                Exit For
                            Else
                                btnDelete.Enabled = True
                            End If

                        End If
                    Next

                End If
            End If
        End Sub


        Protected Sub RadGrid2_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles RadGrid20.ItemDataBound

            If _psDict.ContainsKey("Shib_Authentication") Then
                If TypeOf e.Item Is GridDataItem Then
                    Dim item As GridDataItem = CType(e.Item, GridDataItem)

                    Dim strChk As String = DirectCast(item.DataItem, DataRowView).Row.ItemArray(4)

                    Dim chkbx As CheckBox = item.FindControl("uaChkOverwrite")

                    If strChk = True Then
                        chkbx.Checked = True
                    Else
                        chkbx.Checked = False
                    End If

                End If
            End If
        End Sub

        Private Sub GetSolutionPath(ByRef slnPath As String)

            Dim prjSettings As ProjectSettings = New ProjectSettings
            slnPath = prjSettings.slnPath

        End Sub

        Protected Sub RadGrid1_DeleteCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles RadGrid10.DeleteCommand
            If _psDict.ContainsKey("Shib_Authentication") Then
                Dim ID As String = (CType(e.Item, GridDataItem)).OwnerTableView.DataKeyValues(e.Item.ItemIndex)("RMID").ToString

                Dim editedItem As GridEditableItem = CType(e.Item, GridEditableItem)

                Dim key As Integer = editedItem.GetDataKeyValue("RMID")
                Dim index As Integer = editedItem.DataSetIndex

                ' http://www.telerik.com/help/aspnet/grid/grdaccessingcellsandrows.html

                Dim rmDataTable As DataTable = GetRMTable()

                Dim myRow As DataRow = rmDataTable.Rows.Find(key)

                'delete the row
                myRow.Delete()
                rmDataTable.AcceptChanges()

                'save the change in viewstate
                ViewState("rmDataTable") = rmDataTable
            End If
        End Sub

        Protected Sub RadGrid2_DeleteCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles RadGrid20.DeleteCommand

            Dim ID As String = (CType(e.Item, GridDataItem)).OwnerTableView.DataKeyValues(e.Item.ItemIndex)("RMID").ToString

            Dim editedItem As GridEditableItem = CType(e.Item, GridEditableItem)

            Dim key As Integer = editedItem.GetDataKeyValue("RMID")
            Dim index As Integer = editedItem.DataSetIndex

            ' http://www.telerik.com/help/aspnet/grid/grdaccessingcellsandrows.html

            Dim uaDataTable As DataTable = GetUATable()

            Dim myRow As DataRow = uaDataTable.Rows.Find(key)

            'delete the row
            myRow.Delete()
            uaDataTable.AcceptChanges()

            'save the change in viewstate
            ViewState("uaDataTable") = uaDataTable

        End Sub

        Protected Sub RadGrid3_DeleteCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles RadGrid30.DeleteCommand

            Dim ID As String = (CType(e.Item, GridDataItem)).OwnerTableView.DataKeyValues(e.Item.ItemIndex)("RMID").ToString

            Dim editedItem As GridEditableItem = CType(e.Item, GridEditableItem)

            Dim key As Integer = editedItem.GetDataKeyValue("RMID")
            Dim index As Integer = editedItem.DataSetIndex

            Dim hvDataTable As DataTable = GetHVTable()

            Dim myRow As DataRow = hvDataTable.Rows.Find(key)

            'delete the row
            myRow.Delete()
            hvDataTable.AcceptChanges()

            'save the change in viewstate
            ViewState("uaDataTable") = hvDataTable

        End Sub

        Private Sub RadGrid1_InsertCommand(ByVal source As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs) Handles RadGrid10.InsertCommand

            Dim config As ShibConfiguration = ShibConfiguration.GetConfig()

            Dim MyUserControl As UserControl =
                CType(e.Item.FindControl(GridEditFormItem.EditFormUserControlID), UserControl)

            Dim DNNRoleName As String = CType(MyUserControl.FindControl("ddlDNNRoles"), DropDownList).SelectedItem.ToString
            Dim SHIBRoleName As String = CType(MyUserControl.FindControl("txtSHIBRoleName"), TextBox).Text
            Dim SHIBType As String = CType(MyUserControl.FindControl("ddlSHIBType"), DropDownList).SelectedItem.ToString

            Dim strSHIBType As String = SHIBType
            Dim strSHIBRoleName As String = SHIBRoleName
            strSHIBType = SHIBType

            Dim strRM As String = config.Delimiter.ToString & strSHIBType & config.Delimiter.ToString & DNNRoleName.ToString & config.Delimiter.ToString & strSHIBRoleName.ToString
            'cb_0314

            Dim rmDataTable As DataTable = GetRMTable()

            Dim row As DataRow = rmDataTable.NewRow()

            row.SetField(Of String)("DNNRoleName", DNNRoleName)
            row.SetField(Of String)("SHIBRoleName", strSHIBRoleName)
            row.SetField(Of String)("SHIBRoleType", strSHIBType)

            rmDataTable.Rows.Add(row)

            rmDataTable.AcceptChanges()

            'save the change in viestate
            ViewState("rmDataTable") = rmDataTable

            Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)
            item.Selected = True

        End Sub


        Private Sub RadGrid2_InsertCommand(ByVal source As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs) Handles RadGrid20.InsertCommand

            Dim config As ShibConfiguration = ShibConfiguration.GetConfig()

            Dim MyUserControl As UserControl =
                CType(e.Item.FindControl(GridEditFormItem.EditFormUserControlID), UserControl)

            Dim uaType As String = CType(MyUserControl.FindControl("ddlType"), DropDownList).SelectedItem.ToString
            Dim uaSource As String = CType(MyUserControl.FindControl("txtSource"), TextBox).Text
            Dim uaProperty As String = CType(MyUserControl.FindControl("ddlDNNProperty"), DropDownList).SelectedItem.ToString
            Dim chkOverwrite As CheckBox = MyUserControl.FindControl("chkOverwrite")

            Dim strType As String = uaType

            If uaType = "User" Then
                strType = "UI"
            Else
                strType = "UIP"
            End If

            Dim strUA As String = config.Delimiter.ToString & strType & config.Delimiter.ToString & uaProperty & config.Delimiter.ToString & uaSource

            Dim uaDataTable As DataTable = GetUATable()

            Dim row As DataRow = uaDataTable.NewRow()
            row.SetField(Of String)("Type", strType)
            row.SetField(Of String)("DNNProperty", uaProperty)
            row.SetField(Of String)("Source", uaSource)

            If chkOverwrite.Checked Then
                row.SetField(Of String)("Overwrite", "True")
            Else
                row.SetField(Of String)("Overwrite", "False")
            End If


            uaDataTable.Rows.Add(row)

            uaDataTable.AcceptChanges()

            'save the change in viestate
            ViewState("uaDataTable") = uaDataTable

            Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)
            item.Selected = True

        End Sub


        Private Sub RadGrid3_InsertCommand(ByVal source As Object, ByVal e As Telerik.Web.UI.GridCommandEventArgs) Handles RadGrid30.InsertCommand

            Dim config As ShibConfiguration = ShibConfiguration.GetConfig()

            Dim MyUserControl As UserControl =
                CType(e.Item.FindControl(GridEditFormItem.EditFormUserControlID), UserControl)

            Dim hvName As String = CType(MyUserControl.FindControl("txtSHIBHdrVarName"), TextBox).Text
            Dim hvDelim As String = CType(MyUserControl.FindControl("txtSHIBHdrVarDelim"), TextBox).Text

            Dim strHV As String = hvName & config.Delimiter.ToString & hvDelim
            'cb_0314

            Dim hvDataTable As DataTable = GetHVTable()

            Dim row As DataRow = hvDataTable.NewRow()

            row.SetField(Of String)("SHIBHdrVarName", hvName)
            row.SetField(Of String)("SHIBHdrVarDelim", hvDelim)

            hvDataTable.Rows.Add(row)

            hvDataTable.AcceptChanges()

            'save the change in viestate
            ViewState("hvDataTable") = hvDataTable

            Dim item As GridEditableItem = DirectCast(e.Item, GridEditableItem)
            item.Selected = True

        End Sub


        Protected Sub RadGrid1_UpdateCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles RadGrid10.UpdateCommand

            Dim config As ShibConfiguration = ShibConfiguration.GetConfig()

            Dim MyUserControl As UserControl =
               CType(e.Item.FindControl(GridEditFormItem.EditFormUserControlID), UserControl)

            Dim editedItem As GridEditableItem = CType(e.Item, GridEditableItem)
            Dim key As Integer = editedItem.GetDataKeyValue("RMID")
            Dim strSHIBType As String = ""

            Dim lblKey As Label = CType(MyUserControl.FindControl("labelRMID"), Label)
            Dim intID = e.Item.ItemIndex

            Dim DNNRoleName As String = CType(MyUserControl.FindControl("ddlDNNRoles"), DropDownList).SelectedItem.ToString
            Dim SHIBRoleName As String = CType(MyUserControl.FindControl("txtSHIBRoleName"), TextBox).Text
            Dim SHIBType As String = CType(MyUserControl.FindControl("ddlSHIBType"), DropDownList).SelectedItem.ToString

            Dim strSHIBRoleName As String = ""
            strSHIBType = SHIBType

            Dim strRM As String = strSHIBType.ToString & config.Delimiter.ToString & DNNRoleName.ToString & config.Delimiter.ToString & SHIBRoleName.ToString

            Dim rmDataTable As DataTable = GetRMTable()
            Dim myRow As DataRow = rmDataTable.Rows.Find(key)

            'update the row
            myRow.SetField(Of String)("DNNRoleName", DNNRoleName)
            myRow.SetField(Of String)("SHIBRoleName", SHIBRoleName)
            myRow.SetField(Of String)("SHIBRoleType", strSHIBType)

            rmDataTable.AcceptChanges()

            'save the change in viestate
            ViewState("rmDataTable") = rmDataTable
            '------------------------------------------------------------------------------

        End Sub

        Protected Sub RadGrid2_UpdateCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles RadGrid20.UpdateCommand

            Dim config As ShibConfiguration = ShibConfiguration.GetConfig()

            Dim MyUserControl As UserControl =
                CType(e.Item.FindControl(GridEditFormItem.EditFormUserControlID), UserControl)

            'Dim editMan As GridEditManager = editedItem.EditManager
            Dim editedItem As GridEditableItem = CType(e.Item, GridEditableItem)
            Dim key As Integer = editedItem.GetDataKeyValue("RMID")
            Dim strType As String = ""

            Dim lblKey As Label = CType(MyUserControl.FindControl("labelRMID"), Label)
            Dim intID = e.Item.ItemIndex

            Dim uaProperty As String = CType(MyUserControl.FindControl("ddlDNNProperty"), DropDownList).SelectedItem.ToString
            Dim uaSource As String = CType(MyUserControl.FindControl("txtSource"), TextBox).Text
            Dim uaType As String = CType(MyUserControl.FindControl("ddlType"), DropDownList).SelectedItem.ToString
            Dim chkOverwrite As CheckBox = MyUserControl.FindControl("chkOverwrite")

            If uaType = "User" Then
                strType = "UI"
            Else
                strType = "UIP"
            End If

            Dim strUA As String = strType & config.Delimiter.ToString & uaProperty & config.Delimiter.ToString & uaSource

            Dim uaDataTable As DataTable = GetUATable()
            Dim myRow As DataRow = uaDataTable.Rows.Find(key)

            'update the row
            myRow.SetField(Of String)("DNNProperty", uaProperty)
            myRow.SetField(Of String)("Source", uaSource)
            myRow.SetField(Of String)("Type", strType)

            If chkOverwrite.Checked Then
                myRow.SetField(Of String)("Overwrite", "True")
            Else
                myRow.SetField(Of String)("Overwrite", "False")
            End If

            uaDataTable.AcceptChanges()

            'save the change in viewstate
            ViewState("uaDataTable") = uaDataTable
            '------------------------------------------------------------------------------

        End Sub


        Protected Sub RadGrid3_UpdateCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles RadGrid30.UpdateCommand

            Dim MyUserControl As UserControl =
                CType(e.Item.FindControl(GridEditFormItem.EditFormUserControlID), UserControl)

            Dim editedItem As GridEditableItem = CType(e.Item, GridEditableItem)
            Dim key As Integer = editedItem.GetDataKeyValue("RMID")
            Dim strType As String = ""

            Dim lblKey As Label = CType(MyUserControl.FindControl("labelRMID"), Label)
            Dim intID = e.Item.ItemIndex

            Dim hvHdrVarName As String = CType(MyUserControl.FindControl("txtSHIBHdrVarName"), TextBox).Text
            Dim hvHdrVarDelim As String = CType(MyUserControl.FindControl("txtSHIBHdrVarDelim"), TextBox).Text
            hvHdrVarDelim = Left(hvHdrVarDelim, 1)

            Dim hvDataTable As DataTable = GetHVTable()
            Dim myRow As DataRow = hvDataTable.Rows.Find(key)

            'update the row
            myRow.SetField(Of String)("SHIBHdrVarName", hvHdrVarName)
            myRow.SetField(Of String)("SHIBHdrVarDelim", hvHdrVarDelim)

            hvDataTable.AcceptChanges()

            'save the change in viewstate
            ViewState("uaDataTable") = hvDataTable
            '------------------------------------------------------------------------------

        End Sub


        Protected Sub btnUpdateUserName_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateUserName.Click

            Try
                Dim txtBox As TextBox = Me.pnlAddShibHdrVar.FindControl("txtUserName")

                If txtBox.Text <> "" Then
                    ShibConfiguration.UpdateConfigUserName(_portalSettings.PortalId, txtBox.Text, _psDict)
                End If

                'the configuration is cached.  If you change the portal_settings table, the cache
                'will not be rebuilt and your test may fail.  If you use the settings module to 
                'update the portal_settings value, the cache will be rebuilt with the new values.
                ShibConfiguration.ResetConfig()

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub btnUpdateRoleMappings_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateRoleMappings.Click

            Dim rmDataTable As DataTable = GetRMTable()
            
            Try
                ShibConfiguration.UpdateConfigRoleMappings(_portalSettings.PortalId, rmDataTable, _psDict)

                'the configuration is cached.  If you change the portal_settings table, the cache
                'will not be rebuilt and your test may fail.  If you use the settings module to 
                'update the portal_settings value, the cache will be rebuilt with the new values.
                ShibConfiguration.ResetConfig()

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub


        Protected Sub btnUpdateAttributes_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateAttributes.Click

            Dim uaDataTable As DataTable = GetUATable()
            
            Try
                ShibConfiguration.UpdateConfigUserAttributes(_portalSettings.PortalId, uaDataTable, _psDict)

                'the configuration is cached.  If you change the portal_settings table, the cache
                'will not be rebuilt and your test may fail.  If you use the settings module to 
                'update the portal_settings value, the cache will be rebuilt with the new values.
                ShibConfiguration.ResetConfig()

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Protected Sub btnUpdateShibHdrVar_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateShibHdrVar.Click

            Dim hvDataTable As DataTable = GetHVTable()
           
            Try
                ShibConfiguration.UpdateConfigHeaderVariables(_portalSettings.PortalId, hvDataTable, _psDict)

                'the configuration is cached.  If you change the portal_settings table, the cache
                'will not be rebuilt and your test may fail.  If you use the settings module to 
                'update the portal_settings value, the cache will be rebuilt with the new values.
                ShibConfiguration.ResetConfig()

            Catch exc As Exception 'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        'add localization for radgrid headers
        Private Sub RadGrid1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles RadGrid10.Load
            If _psDict.ContainsKey("Shib_Authentication") Then

                'cb_062711
                'RadGrid1.Culture = System.Threading.Thread.CurrentThread.CurrentCulture

                'RadGrid1.CurrentPageIndex = 1

                Dim strHeaderTextName As String

                For Each bc As GridColumn In RadGrid10.MasterTableView.Columns
                    If bc.ColumnType = "GridButtonColumn" Then
                        Dim bcCol As GridButtonColumn = bc
                        strHeaderTextName = bc.UniqueName & ".Header"
                        bcCol.HeaderText = Localization.GetString(strHeaderTextName, LocalResourceFile)
                    End If
                    If bc.ColumnType = "GridEditCommandColumn" Then
                        Dim bcCol As GridEditCommandColumn = bc
                        strHeaderTextName = bc.UniqueName & ".Header"
                        bcCol.HeaderText = Localization.GetString(strHeaderTextName, LocalResourceFile)
                    End If

                    If bc.ColumnType = "GridBoundColumn" Then
                        Dim bcCol As GridBoundColumn = bc
                        strHeaderTextName = bcCol.DataField & ".Header"
                        bcCol.HeaderText = Localization.GetString(strHeaderTextName, LocalResourceFile)
                    End If
                Next
            End If
        End Sub

        'add localization for radgrid headers
        Private Sub RadGrid2_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles RadGrid20.Load
            If _psDict.ContainsKey("Shib_Authentication") Then
                'cb 062711
                'RadGrid2.Culture = System.Threading.Thread.CurrentThread.CurrentCulture

                Dim strHeaderTextName As String

                For Each bc As GridColumn In RadGrid20.MasterTableView.Columns
                    If bc.ColumnType = "GridButtonColumn" Then
                        Dim bcCol As GridButtonColumn = bc
                        strHeaderTextName = bc.UniqueName & ".Header"
                        bcCol.HeaderText = Localization.GetString(strHeaderTextName, LocalResourceFile)
                    End If
                    If bc.ColumnType = "GridEditCommandColumn" Then
                        Dim bcCol As GridEditCommandColumn = bc
                        strHeaderTextName = bc.UniqueName & ".Header"
                        bcCol.HeaderText = Localization.GetString(strHeaderTextName, LocalResourceFile)
                    End If

                    If bc.ColumnType = "GridBoundColumn" Then
                        Dim bcCol As GridBoundColumn = bc
                        strHeaderTextName = bcCol.DataField & ".Header"
                        bcCol.HeaderText = Localization.GetString(strHeaderTextName, LocalResourceFile)
                    End If
                Next
            End If
        End Sub


        'add localization for radgrid headers
        Private Sub RadGrid3_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles RadGrid30.Load
            If _psDict.ContainsKey("Shib_Authentication") Then

                'cb_062711
                'RadGrid3.Culture = System.Threading.Thread.CurrentThread.CurrentCulture

                Dim strHeaderTextName As String

                For Each bc As GridColumn In RadGrid30.MasterTableView.Columns
                    If bc.ColumnType = "GridButtonColumn" Then
                        Dim bcCol As GridButtonColumn = bc
                        strHeaderTextName = bc.UniqueName & ".Header"
                        bcCol.HeaderText = Localization.GetString(strHeaderTextName, LocalResourceFile)
                    End If
                    If bc.ColumnType = "GridEditCommandColumn" Then
                        Dim bcCol As GridEditCommandColumn = bc
                        strHeaderTextName = bc.UniqueName & ".Header"
                        bcCol.HeaderText = Localization.GetString(strHeaderTextName, LocalResourceFile)
                    End If

                    If bc.ColumnType = "GridBoundColumn" Then
                        Dim bcCol As GridBoundColumn = bc
                        strHeaderTextName = bcCol.DataField & ".Header"
                        bcCol.HeaderText = Localization.GetString(strHeaderTextName, LocalResourceFile)
                    End If
                Next
            End If
        End Sub

        Private Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If _psDict.ContainsKey("Shib_Authentication") Then

                'Dim psDict As System.Collections.Generic.Dictionary(Of String, String) = _
                '    New System.Collections.Generic.Dictionary(Of String, String)
                ShibConfiguration.ResetConfig()


                _psDict = PortalController.GetPortalSettingsDictionary(PortalId)

                If _psDict.ContainsKey("Shib_Authentication") Then

                    If _psDict.Item("Shib_Authentication") Then
                        Me.chkShibEnabled.Checked = True
                    Else
                        Me.chkShibEnabled.Checked = False
                    End If

                    If _psDict.Item("Shib_SimulateLogin") Then
                        Me.chkShibSimulation.Checked = True
                    Else
                        Me.chkShibSimulation.Checked = False
                    End If

                    If _psDict.ContainsKey("Shib_UserName") Then

                        If _psDict.Item("Shib_UserName") IsNot Nothing Then
                            Dim txtBox As TextBox = Me.pnlAddShibHdrVar.FindControl("txtUserName")
                            txtBox.Text = _psDict.Item("Shib_UserName")
                        End If
                    End If

                End If
            End If
        End Sub

    End Class

End Namespace