Public Class IO
    Public Shared Function ImportSquareBracketList(ByVal pathname As String) As Dictionary(Of String, List(Of String))
        Dim total As New Dictionary(Of String, List(Of String))
        Using sr As New System.IO.StreamReader(pathname)
            Dim currentHeader As String = ""
            Dim currentList As New List(Of String)
            While sr.Peek <> -1
                Dim c As String = sr.ReadLine.Trim
                If c = "" Then Continue While

                If c.StartsWith("[") AndAlso c.EndsWith("]") Then
                    'if previous list has already been populated, add to total
                    If currentList.Count > 0 AndAlso currentHeader <> "" Then total.Add(currentHeader, currentList)

                    'now get new headertitle and currentlist
                    currentHeader = c.Replace("[", "")
                    currentHeader = currentHeader.Replace("]", "")
                    currentList = New List(Of String)
                    Continue While
                Else
                    currentList.Add(c)
                End If
            End While

            'add last list
            If currentList.Count > 0 AndAlso currentHeader <> "" Then total.Add(currentHeader, currentList)
        End Using
        Return total
    End Function
    Public Shared Sub SaveSquareBracketList(ByVal pathname As String, ByVal raw As Dictionary(Of String, List(Of String)))
        Using sr As New System.IO.StreamWriter(pathname)
            For Each k In raw.Keys
                Dim rawList As List(Of String) = raw(k)
                sr.WriteLine("[" & k & "]")
                For Each ln In rawList
                    sr.WriteLine(ln)
                Next
                sr.WriteLine()
            Next
        End Using
    End Sub

End Class
