Public Class Node
    Public ReadOnly Property Name As String
    Public ReadOnly Property ParentName As String
    Public ReadOnly Property ChildNames As New List(Of String)
    Public ReadOnly Property Description As String
    Public ReadOnly Property Effects As New List(Of String)

    Public Sub LoadRaw(line As String)
        Dim ln As String() = line.Split("|")
        Dim key As String = ln(0)
        Dim value As String = ln(1)
        Select Case key
            Case "Name" : _Name = value
            Case "Parent" : _ParentName = value
            Case "Child" : _ChildNames.Add(value)
            Case "Description" : _Description = value
            Case "Effect" : _Effects.Add(value)
        End Select
    End Sub
    Public Function ExportRaw() As KeyValuePair(Of String, List(Of String))
        Dim kvp As New KeyValuePair(Of String, List(Of String))(Name, New List(Of String))
        If ParentName <> "" Then kvp.Value.Add("Parent|" & ParentName)
        For Each c In ChildNames
            kvp.Value.Add("Child|" & c)
        Next
        kvp.Value.Add("Description|" & Description)
        For Each e In Effects
            kvp.Value.Add("Effect|" & e)
        Next
        Return kvp
    End Function
End Class
