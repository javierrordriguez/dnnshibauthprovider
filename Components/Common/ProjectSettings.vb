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
Imports DotNetNuke.Entities.Portals


Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities

Imports DotNetNuke.Services.Localization

Imports System.IO


Imports UF.Research.Authentication.Shibboleth.SHIB

Imports DotNetNuke.Common.Globals

Imports DotNetNuke.Entities.Tabs

Namespace UF.Research.Authentication.Shibboleth

    Public Class ProjectSettings

        Public ReadOnly Property slnPath() As String


            Get
                Dim intLastSlash As Integer = DotNetNuke.Common.Globals.NavigateURL.LastIndexOf("/")
                Dim _portalSettings As PortalSettings = PortalController.Instance.GetCurrentPortalSettings
                If _portalSettings IsNot Nothing Then


                    If DotNetNuke.Common.Globals.NavigateURL <> "" Then
                        'Home.aspx()

                        'cb_062711
                        'Return DotNetNuke.Common.Globals.NavigateURL.Replace("Home.aspx", "")
                        'Return DotNetNuke.Common.Globals.NavigateURL.Replace("default.aspx", "")

                        Return Left(DotNetNuke.Common.Globals.NavigateURL, (intLastSlash + 1))

                    Else
                        Return ""
                    End If

                Else : Return ""

                End If

            End Get

        End Property

        Public ReadOnly Property appDefaultPagePath() As String
            Get
                Dim _portalSettings As PortalSettings = PortalSettings.Current
                If _portalSettings IsNot Nothing Then

                    Dim intPos As Integer
                    Dim defaultPagePath As String

                    intPos = InStr(slnPath(), DotNetNuke.Common.Globals.ApplicationPath) + Len(DotNetNuke.Common.Globals.ApplicationPath)
                    defaultPagePath = "~" + Mid(slnPath(), intPos) + "default.aspx"
                    Return defaultPagePath

                Else : Return ""

                End If

            End Get

        End Property

        Public Shared Function CreateNewPortalSettings(ByVal portalId As Integer) As PortalSettings
            'new settings object
            Dim ps As PortalSettings = New PortalSettings()
            'controller instances
            Dim pc As PortalController = New PortalController()
            Dim tc As TabController = New TabController()
            'Dim pac As PortalAliasController = New PortalAliasController()

            'get the first portal alias found to be used as the current portal alias
            Dim portalAlias As PortalAliasInfo = Nothing
            Dim aliases As PortalAliasCollection = PortalAliasController.Instance.GetPortalAliasesByPortalId(portalId)
            Dim aliasKey As String = ""
            If Not aliases Is Nothing AndAlso aliases.Count > 0 Then
                'get the first portal alias in the list and use that
                For Each key As String In aliases.Keys
                    aliasKey = key
                    portalAlias = aliases(key)
                    Exit For
                Next key
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
