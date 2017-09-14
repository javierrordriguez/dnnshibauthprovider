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
Imports DotNetNuke.Security.Roles
Imports System.Collections

Namespace UF.Research.Authentication.Shibboleth

    Public Class GroupInfo
        Inherits RoleInfo
        Implements IAuthenticationObjectBase

        'Private mGUID As String = ""
        'Private mLocation As String = ""
        Private mIsPopulated As Boolean = False
        'Private mMembers As New ArrayList ' store members object
        Private mAuthenticationMember As New ArrayList ' store members DistinguishedName string
        'Private mDNNPopulated As Boolean = False
        'Private mDNNMember As New ArrayList  ' store members UserId        

        'Private mProcessLog As String = ""

        Sub New()
            MyBase.New()
        End Sub


        Public ReadOnly Property Name() As String Implements IAuthenticationObjectBase.Name
            Get
                Return RoleName
            End Get
        End Property

 
        Public ReadOnly Property ObjectClass() As ObjectClass Implements IAuthenticationObjectBase.ObjectClass
            Get
                Return ObjectClass.group
            End Get
        End Property

        Public ReadOnly Property AuthenticationMember() As ArrayList
            Get
                Return mAuthenticationMember
            End Get
        End Property

        Public Property IsPopulated() As Boolean
            Get
                Return mIsPopulated
            End Get
            Set(ByVal Value As Boolean)
                mIsPopulated = Value
            End Set
        End Property

    End Class


End Namespace
