Public Class FrmConfig



    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                            Handles MyBase.Load
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
End Class