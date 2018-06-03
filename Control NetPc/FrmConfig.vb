
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
        If TxtCmPulsos.Text <> "" Then
            nroNodo = (ComboBox1.SelectedIndex)
            FrmPrincipal.myPuertoSerie.NodeStatus(nroNodo).CmPulse = CUShort(TxtCmPulsos.Text)
            FrmPrincipal.mCfg.SetValue("Nodo" & (nroNodo + 1), "CmX1000", CInt(TxtCmPulsos.Text)) 'Grabo en archivo config XML
            TxtCmPulsos.Text = ""
            LblCmPulsos.Text = FrmPrincipal.myPuertoSerie.NodeStatus(nroNodo).CmPulse
        End If
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
        FrmPrincipal.myPuertoSerie.PollTime = CInt(TextBox7.Text)
        FrmPrincipal.mCfg.SetValue("General", "PollTime", CInt(TextBox7.Text)) 'Grabo en archivo config XML
        TextBox6.Text = CStr(FrmPrincipal.myPuertoSerie.PollTime)
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

        If TxtUpLimit.Text <> "" Then
            nroNodo = (ComboBox1.SelectedIndex)
            limitSup = CUShort(TxtUpLimit.Text)
            limitInf = CUShort(LblDownLimit.Text)
            FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cActualizarLimites, nroNodo, limitSup, 1, limitInf)
            TxtUpLimit.Text = ""
            LblUpLimit.Text = limitSup.ToString
        End If

    End Sub

    Private Sub BtnDownLimit_Click(sender As Object, e As EventArgs) Handles BtnDownLimit.Click
        Dim nroNodo As Byte
        Dim limitSup As UShort
        Dim limitInf As UShort

        If TxtDownLimit.Text <> "" Then
            nroNodo = (ComboBox1.SelectedIndex)
            limitSup = CUShort(LblUpLimit.Text)
            limitInf = CUShort(TxtDownLimit.Text)
            FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cActualizarLimites, nroNodo, limitSup, 1, limitInf)
            TxtDownLimit.Text = ""
            LblDownLimit.Text = limitInf.ToString
        End If
    End Sub
End Class