Public Class FrmConfig



    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                            Handles MyBase.Load

        'Lista puertos series disponibles
        For Each s As String In My.Computer.Ports.SerialPortNames
            ListBox1.Items.Add(s)
        Next

        For i As Byte = 1 To FrmPrincipal.CantidadMotores
            Me.ComboBox1.Items.Add("Node" & i)
        Next
        Label6.Text = FrmPrincipal.myPuertoSerie.CantidadMotores


    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub Label6_Click(sender As Object, e As EventArgs) Handles Label6.Click

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(PuertoCom.ComandoMotor.cActualizarLimites, 1, 45000, 7, 1532)
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs)

        If CheckBox1.CheckState = CheckState.Checked Then
            FrmPrincipal.myPuertoSerie.UseCheckPacket = True                'Si voy a utilizar chequeo de tramas con las placas
        Else
            FrmPrincipal.myPuertoSerie.UseCheckPacket = False
        End If

    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs)

        If CheckBox2.CheckState = CheckState.Checked Then
            FrmPrincipal.myPuertoSerie.HabilitarPoollingAutomatico = True   'Habilita pooling automatico
        Else
            FrmPrincipal.myPuertoSerie.HabilitarPoollingAutomatico = False
        End If

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        FrmPrincipal.myPuertoSerie.InitSerial(FrmPrincipal.CantidadMotores, "COM5", 115200, "None", 8, "One")  'Inicio Puerto seria

    End Sub

    Private Sub CheckBox2_CheckedChanged_1(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged

    End Sub
End Class