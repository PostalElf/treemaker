Imports System.ComponentModel

Public Class Form1
    Dim WithEvents ActiveNodeList As New NodeList
    Private _ActivePath As String
    Private Property ActivePath As String
        Get
            Return _ActivePath
        End Get
        Set(value As String)
            If value = "" Then
                lblParent.Text = ""
                lblParent.Enabled = False
                lblActive.Text = ""
                lblActive.Enabled = False
                lstChildren.Items.Clear()
                lstChildren.Enabled = False
                txtDescription.Enabled = False
                txtChildName.Enabled = False
                txtEffects.Enabled = False
            Else
                lblParent.Enabled = True
                lblActive.Enabled = True
                lstChildren.Enabled = True
                txtDescription.Enabled = True
                txtChildName.Enabled = True
                txtEffects.Enabled = True
            End If
            _ActivePath = value
        End Set
    End Property
    Private Sub ChangeNode(newNode As Node)
        With ActiveNodeList.ActiveNode
            'update UI after a node has been changed
            If .ParentName = "" Then
                lblParent.Text = "<ROOT>"
            Else
                lblParent.Text = .ParentName
            End If
            lblActive.Text = .Name
            lstChildren.Items.Clear()
            For Each childName In .ChildNames
                lstChildren.Items.Add(childName)
            Next
            txtChildName.Text = ""

            txtDescription.Text = .Description
            txtEffects.Text = ""
            For n = 0 To .Effects.Count - 1
                Dim e As String = .Effects(n)
                txtEffects.Text &= e
                If n <> .Effects.Count - 1 Then txtEffects.Text &= vbCrLf
            Next
        End With
    End Sub
    Private Sub SaveNode()
        If ActiveNodeList Is Nothing Then Exit Sub
        If ActiveNodeList.ActiveNode Is Nothing Then Exit Sub

        With ActiveNodeList.ActiveNode
            .LoadRaw("Description|" & txtDescription.Text)
            .Effects.Clear()
            For Each line In txtEffects.Text.Split(vbCrLf)
                .Effects.Add(line)
            Next
        End With
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        ActivePath = ""
    End Sub
    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        SaveNode()
        If MsgBox("Save changes?", MsgBoxStyle.YesNo + MsgBoxStyle.Question, "Save") <> MsgBoxResult.Yes Then Exit Sub

        SaveToolStripMenuItem_Click(Me, Nothing)
    End Sub

    Private Sub lblActive_Click(sender As Object, e As EventArgs) Handles lblActive.Click
        Dim an As Node = ActiveNodeList.ActiveNode
        If an Is Nothing Then Exit Sub
        Dim newName As String = InputBox("Enter new name", "Rename Node", an.Name)
        ActiveNodeList.RenameActiveNode(newName)
        lblActive.Text = newName
    End Sub
    Private Sub lblParent_Click(sender As Object, e As EventArgs) Handles lblParent.Click
        Dim an As Node = ActiveNodeList.ActiveNode
        If an Is Nothing Then Exit Sub
        If an.ParentName = "" Then Exit Sub

        ActiveNodeList.SetActiveNode(an.ParentName)
        SaveNode()
        ChangeNode(an)
    End Sub
    Private Sub lstChildren_DoubleClick(sender As Object, e As EventArgs) Handles lstChildren.MouseDoubleClick
        If lstChildren.SelectedIndex = -1 Then Exit Sub
        SaveNode()
        Dim s As String = lstChildren.SelectedItem.ToString
        ActiveNodeList.SetActiveNode(s)
        ChangeNode(ActiveNodeList.ActiveNode)
    End Sub
    Private Sub btnAddChild_Click(sender As Object, e As EventArgs) Handles btnAddChild.Click
        Dim nodeName As String = txtChildName.Text
        ActiveNodeList.Add(nodeName, ActiveNodeList.ActiveNode)
        ChangeNode(ActiveNodeList.ActiveNode)
    End Sub

    Private Sub NewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewToolStripMenuItem.Click
        If MsgBox("Are you sure?", MsgBoxStyle.YesNo + MsgBoxStyle.Question, "New Tree") = MsgBoxResult.No Then Exit Sub
        Dim path As String = GetActivePathForSave()
        If path = "" Then Exit Sub
        ActivePath = path

        Dim raw As New Dictionary(Of String, List(Of String))
        raw.Add("RootNode", New List(Of String))
        With raw("RootNode")
            .Add("Description|Description goes here")
        End With
        IO.SaveSquareBracketList(ActivePath, raw)

        ActiveNodeList = NodeList.LoadList(ActivePath)
        ChangeNode(ActiveNodeList.ActiveNode)
    End Sub
    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        Dim fd As New OpenFileDialog
        fd.Filter = "Tree files (*.tree)|*.tree"
        Dim result = fd.ShowDialog
        If result <> DialogResult.OK Then Exit Sub

        Dim path As String = fd.FileName
        ActivePath = path
        ActiveNodeList = NodeList.LoadList(path)
        ChangeNode(ActiveNodeList.ActiveNode)
    End Sub
    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        If ActivePath = "" Then
            Dim path As String = GetActivePathForSave()
            If path = "" Then Exit Sub
            ActivePath = path
        End If

        SaveNode()
        ActiveNodeList.SaveList(ActivePath)
    End Sub

    Private Function GetActivePathForSave() As String
        Dim fd As New SaveFileDialog
        fd.Filter = "Tree files (*.tree)|*.tree"
        fd.DefaultExt = ".tree"
        fd.AddExtension = True
        Dim result = fd.ShowDialog
        If result <> DialogResult.OK Then Return ""

        Return fd.FileName
    End Function

End Class
