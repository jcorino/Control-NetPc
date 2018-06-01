
Imports System.IO.Ports

Public Class FrmConfig

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                            Handles MyBase.Load

        'Lista puertos series disponibles
        For Each s As String In My.Computer.Ports.SerialPortNames
            Me.ComboBox2.Items.Add(s)
        Next

        For i As Byte = 1 To FrmPrincipal.myPuertoSerie.QtydMotores
            Me.ComboBox1.Items.Add("Node" & i)
        Next

        TextBox5.Text = FrmPrincipal.myPuertoSerie.ComPort

    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub Label6_Click(sender As Object, e As EventArgs) Handles Label6.Click

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cActualizarLimites, 1, 45000, 7, 1532)
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged

        If CheckBox1.CheckState = CheckState.Checked Then
            FrmPrincipal.myPuertoSerie.UseCheckPacket = True                'Si voy a utilizar chequeo de tramas con las placas
        Else
            FrmPrincipal.myPuertoSerie.UseCheckPacket = False
        End If

    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged

        If CheckBox2.CheckState = CheckState.Checked Then
            FrmPrincipal.myPuertoSerie.HabilitarPoollingAutomatico = True   'Habilita pooling automatico
        Else
            FrmPrincipal.myPuertoSerie.HabilitarPoollingAutomatico = False
        End If

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        FrmPrincipal.myPuertoSerie.ComPort = TextBox5.Text
        FrmPrincipal.myPuertoSerie.InitSerial()  'Inicio Puerto seria

    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.CheckState = CheckState.Checked Then
            FrmPrincipal.myPuertoSerie.ActivarComunicacion = True
        Else
            FrmPrincipal.myPuertoSerie.ActivarComunicacion = False
        End If
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        TextBox5.Text = ComboBox2.SelectedItem.ToString
    End Sub
End Class