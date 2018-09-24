Public Class Form2
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 31954, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 31954, 0)
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32106, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32106, 0)
    End Sub

    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 4, 32025, 0)
    End Sub

    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32670, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32670, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 4, 32594, 0)
    End Sub

    Private Sub Button16_Click(sender As Object, e As EventArgs) Handles Button16.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 4, 32100, 0)
    End Sub

    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 3, 32106, 0)
        FrmPrincipal.myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 5, 32100, 0)
    End Sub
End Class