Public Class NodeList
    Private pList As New List(Of Node)
    Public ActiveNode As Node
    Public ActiveParent As Node
    Public ActiveChildren As New List(Of Node)

    Public Event ChangeNode(newNode As Node)

    Public Function SetActiveNode(name As String) As String
        Dim node As Node = GetNode(name)
        If node Is Nothing Then Return "Node '" & name & "' not found."
        Return SetActiveNode(node)
    End Function
    Public Function SetActiveNode(node As Node) As String
        Dim parent As Node = GetNode(node.ParentName)
        ActiveParent = parent

        ActiveChildren.Clear()
        For Each nodeName As String In node.ChildNames
            Dim n As Node = GetNode(nodeName)
            If n Is Nothing Then Return "Child node '" & nodeName & "' not found."
            ActiveChildren.Add(n)
        Next

        ActiveNode = node
        RaiseEvent ChangeNode(node)
        Return Nothing
    End Function
    Public Sub Add(name As String, parent As Node)
        Dim node As New Node
        node.LoadRaw("Name|" & name)
        node.LoadRaw("Parent|" & parent.Name)
        parent.ChildNames.Add(name)
        pList.Add(node)
    End Sub

    Public Sub RenameActiveNode(newName As String)
        If ActiveParent Is Nothing = False Then
            Dim n As Integer = ActiveParent.ChildNames.IndexOf(ActiveNode.Name)
            ActiveParent.ChildNames(n) = newName
        End If
        ActiveNode.LoadRaw("Name|" & newName)
    End Sub
    Private Function GetNode(name As String) As Node
        For Each n In pList
            If n.Name = name Then Return n
        Next
        Return Nothing
    End Function
    Private Function GetRootNode() As Node
        For Each n In pList
            If n.ParentName = "" Then Return n
        Next
        Return Nothing
    End Function

    Public Shared Function LoadList(path As String) As NodeList
        Dim rawText As Dictionary(Of String, List(Of String)) = IO.ImportSquareBracketList(path)
        Dim nl As New NodeList
        For Each key In rawText.Keys
            Dim ln As List(Of String) = rawText(key)
            Dim n As New Node
            n.LoadRaw("Name|" & key)
            For Each line In ln
                n.LoadRaw(line)
            Next
            nl.pList.Add(n)
        Next
        nl.SetActiveNode(nl.GetRootNode)
        Return nl
    End Function
    Public Sub SaveList(path As String)
        Dim total As New Dictionary(Of String, List(Of String))
        For Each n In pList
            Dim kvp As KeyValuePair(Of String, List(Of String)) = n.ExportRaw
            total.Add(kvp.Key, kvp.Value)
        Next
        IO.SaveSquareBracketList(path, total)
    End Sub
End Class
