
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
Imports DotNetNuke.Entities.Portals
Imports System.Collections

Namespace UF.Research.Authentication.Shibboleth

    ''' <summary>
    ''' Represents the collection of RoleMappings for a portal
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class RoleMappings
        Inherits System.Collections.Generic.Dictionary(Of String, RoleMappingElement)

#Region "Private Members"

        'This is used to return a sorted List
        Private list As List(Of RoleMappingElement)

#End Region

#Region "Constructors"

        Public Sub New()
            list = New List(Of RoleMappingElement)
        End Sub

        'Required for Serialization
        Public Sub New(ByVal info As SerializationInfo, ByVal context As StreamingContext)
            MyBase.New(info, context)
        End Sub

        Public Sub New(ByVal RMList As List(Of RoleMappingElement))

            Dim _portalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

            Try
                'Cambrian = DNN v5
                Dim CambrianSettings As System.Collections.Generic.Dictionary(Of String, String) = _
                    PortalController.GetPortalSettingsDictionary(_portalSettings.PortalId)

            Catch
            Finally
            End Try

        End Sub

#End Region

        Private Function AddRoleMapping(ByVal RMElement As RoleMappingElement) As Integer

            Dim RMList As RoleMappings = Me
            Dim RMSettingName As String = "ShibRoleMapping"
            'get ShibRoleMappingCount from PortalSettings and update RMCount with the value
            Dim RMCount As Integer = 0
            RMSettingName = RMSettingName & System.Convert.ToString(RMCount)
            RMList.Add(RMSettingName, RMElement)
            Return RMList.Count
        End Function

#Region "Public Methods"

        Public Function AsList() As List(Of RoleMappingElement)
            Return list
        End Function

        Public Function ToArrayList() As ArrayList
            Dim RMList As New ArrayList()
            For Each RMElement As RoleMappingElement In list
                RMList.Add(RMElement)
            Next
            Return RMList
        End Function


#End Region
    End Class

End Namespace

Public Class RoleMappings

End Class
