
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

Imports System.Collections.Generic
Imports System.Runtime.Serialization

'Namespace UF.Research.Shibboleth.Authentication
Namespace UF.Research.Authentication.Shibboleth


    Public Class RoleMappingElement

#Region "Private Members"

        Private _ShibConfiguration As ShibConfiguration
        Private _ShibRoleName As String
        Private _Delimiter As String
        Private _DNNRoleName As String

#End Region

#Region "Constructors"

        Public Sub New()
            _ShibConfiguration = ShibConfiguration.GetConfig()
            _ShibRoleName = ""
            _Delimiter = ""
            _DNNRoleName = ""
        End Sub

#End Region

#Region "Public Properties"

        Public Property ShibRoleName() As String
            Get
                Return GetRoleName("SHIB")
            End Get

            Set(ByVal value As String)
                _ShibRoleName = value
            End Set
        End Property

        Public Property Delimiter() As String
            Get
                Return _Delimiter
            End Get
            Set(ByVal value As String)
                _Delimiter = value
            End Set
        End Property

        Public Property DNNRoleName() As String
            Get
                _DNNRoleName = GetRoleName("DNN")
                Return _DNNRoleName
            End Get
            Set(ByVal value As String)
                _DNNRoleName = value
            End Set
        End Property

#End Region

#Region "Private Methods"

        ''this function will return the Shibboleth or DNN Role Name based on the value of strRoleType passed in
        Private Function GetRoleName(ByVal strRoleType As String) As String

            Dim shibRoleName As String = String.Empty

            Dim sArray As Array = Split(Me.ToString, _ShibConfiguration.Delimiter)
            shibRoleName = sArray(0)
            DNNRoleName = sArray(1)

            If strRoleType.ToUpper = "SHIB" Then
                Return sArray(0)
            Else
                Return sArray(1)
            End If

        End Function

#End Region

    End Class

End Namespace

