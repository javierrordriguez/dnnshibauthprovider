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

Imports DotNetNuke.Entities.Portals
Imports System.Collections

Namespace UF.Research.Authentication.Shibboleth

    Public MustInherit Class ShibAuthenticationProvider

#Region "Shared/Static Methods"

        ' singleton reference to the instantiated object 
        Private Shared objProvider As ShibAuthenticationProvider = Nothing


        Shared Sub New()
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Dim _config As ShibConfiguration = ShibConfiguration.GetConfig()
            Dim strKey As String
            If _portalSettings IsNot Nothing Then
                strKey = "ShibAuthenticationProvider" & _portalSettings.PortalId.ToString
            End If
            objProvider = CType(DotNetNuke.Framework.Reflection.CreateObject(_config.ProviderTypeName, strKey), ShibAuthenticationProvider)

        End Sub

        Public Shared Shadows Function Instance(ByVal AuthenticationTypeName As String) As ShibAuthenticationProvider
            'CreateProvider()
            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            Dim strKey As String
            If _portalSettings IsNot Nothing Then
                strKey = "ShibAuthenticationProvider" & _portalSettings.PortalId.ToString
            End If
            objProvider = CType(DotNetNuke.Framework.Reflection.CreateObject(AuthenticationTypeName, strKey), ShibAuthenticationProvider)
            Return objProvider
        End Function

#End Region

#Region "Abstract Methods"

        Public MustOverride Overloads Function GetUser(ByVal LoggedOnUserName As String) As ShibUserInfo

        Public MustOverride Function GetGroups(ByVal oRoleGroup As Object) As ArrayList

        Public MustOverride Function GetAllGroups(ByVal oRoleGroup As Object) As ArrayList

#End Region

    End Class

End Namespace
