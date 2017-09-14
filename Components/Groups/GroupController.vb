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
Imports System.Collections

Namespace UF.Research.Authentication.Shibboleth

    Public Class GroupController

        Private mProviderTypeName As String = ""

 
        Sub New()
            Dim _config As ShibConfiguration = ShibConfiguration.GetConfig()
            mProviderTypeName = _config.ProviderTypeName
        End Sub

        Public Function GetGroups(ByVal oRoleGroup As Object) As ArrayList
            Return ShibAuthenticationProvider.Instance(mProviderTypeName).GetGroups(oRoleGroup)
        End Function
        Public Function GetAllGroups(ByVal oRoleGroup As Object) As ArrayList
            Return ShibAuthenticationProvider.Instance(mProviderTypeName).GetAllGroups(oRoleGroup)
        End Function
    End Class

End Namespace
