﻿Imports System.Xml
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
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32000, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 4, 32000, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32000, 0)


    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32308, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 4, 32308, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32308, 0)

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32475, 3)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 4, 32475, 3)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32475, 3)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32025, 5)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 4, 32025, 2)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32025, 5)

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32123, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 4, 32123, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32123, 0)

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32540, 4)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 4, 32540, 4)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32540, 4)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32024, 5)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 4, 32015, 2)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32024, 5)
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32560, 6)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 4, 32560, 6)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32560, 6)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32640, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 4, 32640, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32640, 0)
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32024, 4)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 4, 32024, 4)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32024, 4)
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32340, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 4, 32340, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32340, 0)
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 31954, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 31954, 0)
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32106, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32106, 0)
    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32110, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32110, 0)
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32670, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32360, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32670, 1)
    End Sub

    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 4, 32016, 0)
    End Sub

    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32670, 1)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32670, 1)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 4, 32585, 1)
    End Sub

    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 4, 32100, 7)
    End Sub

    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32106, 3)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32100, 3)
    End Sub

    Private Sub Button19_Click(sender As Object, e As EventArgs) Handles Button19.Click
        For Each r As DataGridViewRow In DataGridView2.SelectedRows
            DataGridView2.Rows.Remove(r)
        Next
    End Sub

    Private Sub Button20_Click(sender As Object, e As EventArgs) Handles Button20.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 31954, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 31954, 0)
    End Sub

    Private Sub Button18_Click(sender As Object, e As EventArgs) Handles Button18.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32110, 3)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32110, 3)
    End Sub
End Class