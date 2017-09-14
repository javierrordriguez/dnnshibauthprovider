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

Namespace UF.Research.Authentication.Shibboleth

    Public Class ShibUserInfo
        Inherits DotNetNuke.Entities.Users.UserInfo
        Implements IAuthenticationObjectBase

        Private mCName As String = ""
        Private mDistinguishedName As String = ""
        Private mGLID As String = ""
        Private mDepartment As String

        Private mAuthenticationExists As Boolean = False


        Sub New()
            MyBase.New()
        End Sub


        Public Property CName() As String
            Get
                Return mCName
            End Get
            Set(ByVal Value As String)
                mCName = Value
            End Set
        End Property


        Public Property DistinguishedName() As String
            Get
                Return mDistinguishedName
            End Get
            Set(ByVal Value As String)
                mDistinguishedName = Value
            End Set
        End Property

        Public Property Department() As String
            Get
                Return mDepartment
            End Get
            Set(ByVal Value As String)
                mDepartment = Value
            End Set
        End Property

        Public ReadOnly Property Name() As String Implements IAuthenticationObjectBase.Name
            Get
                Return mGLID
            End Get
            'Set(ByVal Value As String)
            '    mGLID = Value
            'End Set
        End Property

        Public ReadOnly Property ObjectClass() As ObjectClass Implements IAuthenticationObjectBase.ObjectClass
            Get
                Return ObjectClass.person
            End Get
        End Property

        Public Property AuthenticationExists() As Boolean
            Get
                Return mAuthenticationExists
            End Get
            Set(ByVal Value As Boolean)
                mAuthenticationExists = Value
            End Set
        End Property

    End Class

End Namespace

