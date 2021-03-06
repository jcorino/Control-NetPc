﻿
Imports System.IO.Ports

Public Class FrmConfig

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                            Handles MyBase.Load

        'Lista puertos series disponibles
        For Each s As String In My.Computer.Ports.SerialPortNames
            Me.ComboBox2.Items.Add(s)
        Next

        For i As Byte = 0 To FrmPrincipal.myPuertoSerie.QtydMotores - 1
            Me.ComboBox1.Items.Add("Node" & i)
        Next

        TextBox5.Text = FrmPrincipal.myPuertoSerie.ComPort
        TextBox6.Text = CStr(FrmPrincipal.myPuertoSerie.PollTime)

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles BtnCmPulsos.Click
        Dim nroNodo As Byte

        If (ComboBox1.SelectedIndex) = -1 Then Exit Sub
        If TxtCmPulsos.Text = "" Then Exit Sub
        If CInt(TxtCmPulsos.Text) < 0 Or CInt(TxtCmPulsos.Text) > 65535 Then Exit Sub

        nroNodo = (ComboBox1.SelectedIndex)
        FrmPrincipal.myPuertoSerie.NodeStatus(nroNodo).CmPulse = CUShort(TxtCmPulsos.Text)
        FrmPrincipal.mCfg.SetValue("Nodo" & (nroNodo + 1), "CmX1000", CInt(TxtCmPulsos.Text)) 'Grabo en archivo config XML
        TxtCmPulsos.Text = ""
        LblCmPulsos.Text = FrmPrincipal.myPuertoSerie.NodeStatus(nroNodo).CmPulse

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        FrmPrincipal.myPuertoSerie.ComPort = TextBox5.Text
        FrmPrincipal.mCfg.SetValue("General", "ComPort", TextBox5.Text) 'Grabo en archivo config XML
        FrmPrincipal.myPuertoSerie.InitSerial()  'Inicio Puerto serie

    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        TextBox5.Text = ComboBox2.SelectedItem.ToString
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        'Filtro para que el valor ingresado se entre 0 y 9999 ms
        If TextBox7.Text = "" Then Exit Sub
        If CInt(TextBox7.Text) > 0 And CInt(TextBox7.Text) < 9999 Then
            FrmPrincipal.myPuertoSerie.PollTime = CInt(TextBox7.Text)
            FrmPrincipal.mCfg.SetValue("General", "PollTime", CInt(TextBox7.Text)) 'Grabo en archivo config XML
            TextBox6.Text = CStr(FrmPrincipal.myPuertoSerie.PollTime)
        End If
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged

        Dim nroNodo As Byte
        nroNodo = CByte(ComboBox1.SelectedIndex)

        lblNodeDisable.Visible = Not (FrmPrincipal.myPuertoSerie.NodeStatus(nroNodo).Enable)

        LblName.Text = FrmPrincipal.myPuertoSerie.NodeStatus(nroNodo).Nombre
        LblUpLimit.Text = FrmPrincipal.myPuertoSerie.NodeStatus(nroNodo).LimiteSup
        LblDownLimit.Text = FrmPrincipal.myPuertoSerie.NodeStatus(nroNodo).LimiteInf
        LblCmPulsos.Text = FrmPrincipal.myPuertoSerie.NodeStatus(nroNodo).CmPulse

    End Sub

    Private Sub BtnName_Click(sender As Object, e As EventArgs) Handles BtnName.Click
        Dim nroNodo As Byte

        If (ComboBox1.SelectedIndex) = -1 Then Exit Sub
        If TxtName.Text <> "" Then
            nroNodo = (ComboBox1.SelectedIndex)
            FrmPrincipal.myPuertoSerie.NodeStatus(nroNodo).Nombre = TxtName.Text
            FrmPrincipal.mCfg.SetValue("Nodo" & (nroNodo + 1), "Name", TxtName.Text) 'Grabo en archivo config XML
            FrmPrincipal.m_LblName(nroNodo).Text = TxtName.Text
            TxtName.Text = ""
            LblName.Text = FrmPrincipal.myPuertoSerie.NodeStatus(nroNodo).Nombre
        End If
    End Sub

    Private Sub BtnUpLimit_Click(sender As Object, e As EventArgs) Handles BtnUpLimit.Click
        Dim nroNodo As Byte
        Dim limitSup As UShort
        Dim limitInf As UShort

        If (ComboBox1.SelectedIndex) = -1 Then Exit Sub
        If TxtUpLimit.Text = "" Then Exit Sub
        If CInt(TxtUpLimit.Text) < 0 Or CInt(TxtUpLimit.Text) > 65535 Then Exit Sub

        nroNodo = (ComboBox1.SelectedIndex)
        limitSup = CUShort(TxtUpLimit.Text)
        limitInf = CUShort(LblDownLimit.Text)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cActualizarLimites, nroNodo, limitSup, 1, limitInf)
        TxtUpLimit.Text = ""
        LblUpLimit.Text = limitSup.ToString

    End Sub

    Private Sub BtnDownLimit_Click(sender As Object, e As EventArgs) Handles BtnDownLimit.Click
        Dim nroNodo As Byte
        Dim limitSup As UShort
        Dim limitInf As UShort

        If (ComboBox1.SelectedIndex) = -1 Then Exit Sub
        If TxtDownLimit.Text = "" Then Exit Sub
        If CInt(TxtDownLimit.Text) < 0 Or CInt(TxtDownLimit.Text) > 65535 Then Exit Sub

        nroNodo = (ComboBox1.SelectedIndex)
        limitSup = CUShort(LblUpLimit.Text)
        limitInf = CUShort(TxtDownLimit.Text)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cActualizarLimites, nroNodo, limitSup, 1, limitInf)
        TxtDownLimit.Text = ""
        LblDownLimit.Text = limitInf.ToString

    End Sub

    Private Sub TxtUpLimit_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtUpLimit.KeyPress
        e.Handled = Not IsNumeric(e.KeyChar) And Not Char.IsControl(e.KeyChar)
    End Sub

    Private Sub TxtDownLimit_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtDownLimit.KeyPress
        e.Handled = Not IsNumeric(e.KeyChar) And Not Char.IsControl(e.KeyChar)
    End Sub

    Private Sub TxtCmPulsos_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TxtCmPulsos.KeyPress
        e.Handled = Not IsNumeric(e.KeyChar) And Not Char.IsControl(e.KeyChar)
    End Sub

    Private Sub TextBox7_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox7.KeyPress
        e.Handled = Not IsNumeric(e.KeyChar) And Not Char.IsControl(e.KeyChar)
    End Sub
End Class