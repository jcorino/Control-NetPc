Imports System.Xml
Imports System.IO
Imports System.Data

Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim w As Integer
        Dim Cues As String

        Dim tempDS As DataSet = New DataSet

        tempDS.ReadXml(My.Application.Info.DirectoryPath & "\Cues.xml")

        DataGridView2.DataSource = tempDS
        DataGridView2.DataMember = "rutina_subir"
        DataGridView2.AutoGenerateColumns = False

        w = tempDS.Tables.Count()
        Cues = tempDS.Tables.Item(0).TableName
        Cues = tempDS.Tables.Item(1).ToString

        For i As Integer = 0 To tempDS.Tables.Count - 1
            Debug.WriteLine("Table count: " & tempDS.Tables(i).TableName.ToString)
        Next

    End Sub

    Private Sub DataGridView2_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click

    End Sub
End Class