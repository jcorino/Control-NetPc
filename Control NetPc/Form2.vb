'------------------------------------------------------------------------------
' Prueba de la clase ConfigXml                                      (21/Feb/06)
' para guardar datos de configuración en formato Xml
'
' ©Guillermo 'guille' Som, 2006
'------------------------------------------------------------------------------
Option Strict On

Public Class Form2
    Private mCfg As jmc.Util.ConfigXml

    Private Sub BtnLeer_Click(ByVal sender As System.Object,
                              ByVal e As System.EventArgs) _
                              Handles btnLeer.Click
        ' Leer el fichero
        Dim openFD As New OpenFileDialog With {
            .Title = "Selecciona el fichero de configuración",
            .Filter = "Configuración|*.cfg;*.config;*.configuration|Todos los ficheros|*.*",
            .FileName = txtFic.Text,
            .Multiselect = False,
            .CheckFileExists = False
        }
        If openFD.ShowDialog = Windows.Forms.DialogResult.OK Then
            mCfg = New jmc.Util.ConfigXml(openFD.FileName, True)
            ActualizarListView()

            Dim b As Boolean = True
            Me.btnAdd.Enabled = b
            Me.btnEliminarSec.Enabled = b
            Me.btnGuardar.Enabled = b
        End If
    End Sub

    Private Sub ActualizarListView()
        ListView1.Items.Clear()
        For Each s As String In mCfg.Secciones
            For Each s1 As KeyValuePair(Of String, String) In mCfg.Claves(s)
                Dim lvi As ListViewItem = ListView1.Items.Add(s)
                lvi.SubItems.Add(s1.Key)
                lvi.SubItems.Add(s1.Value)
            Next
        Next
    End Sub

    Private Sub BtnEliminarSec_Click(ByVal sender As System.Object,
                                     ByVal e As System.EventArgs) _
                                     Handles btnEliminarSec.Click
        If MessageBox.Show("¿Quieres eliminar la sección " & txtSec.Text & "?",
                           "Eliminar sección",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
            mCfg.RemoveSection(txtSec.Text)
            ActualizarListView()
        End If
    End Sub

    Private Sub BtnAdd_Click(ByVal sender As System.Object,
                             ByVal e As System.EventArgs) Handles btnAdd.Click
        ' Añadirlo / actualizarlo y rellenar el ListView
        If mCfg Is Nothing Then
            MessageBox.Show("Debes indicar el ficehero en el que guardar los datos",
                            "Añadir", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If
        mCfg.SetValue(txtSec.Text, txtClave.Text, txtValor.Text)
        ActualizarListView()
    End Sub

    Private Sub ListView1_SelectedIndexChanged(ByVal sender As System.Object,
                                               ByVal e As System.EventArgs) _
                                               Handles ListView1.SelectedIndexChanged
        Dim lvi As ListViewItem
        If ListView1.SelectedItems.Count > 0 Then
            lvi = ListView1.SelectedItems(0)
            Me.txtSec.Text = lvi.SubItems(0).Text
            Me.txtClave.Text = lvi.SubItems(1).Text
            Me.txtValor.Text = lvi.SubItems(2).Text
        End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Deshabilitar los botones y controles
        ' hasta que se indique el fichero
        Dim b As Boolean = False
        Me.btnAdd.Enabled = b
        Me.btnEliminarSec.Enabled = b
        Me.btnGuardar.Enabled = b
        txtFic.Text = System.IO.Directory.GetCurrentDirectory() & "\prueba.cfg"
    End Sub

    Private Sub BtnGuardar_Click(ByVal sender As System.Object,
                                 ByVal e As System.EventArgs) Handles btnGuardar.Click
        ' No hace falta si se indica "guardar automáticamente" en el constructor
        mCfg.Save()
    End Sub

    Private Sub BtnAdd_Click_1(sender As Object, e As EventArgs) Handles btnAdd.Click

    End Sub

    Private Sub ListView1_SelectedIndexChanged_1(sender As Object, e As EventArgs)

    End Sub

    Private Sub BtnGuardar_Click_1(sender As Object, e As EventArgs) Handles btnGuardar.Click

    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub TxtFic_TextChanged(sender As Object, e As EventArgs) Handles txtFic.TextChanged

    End Sub

    Private Sub TxtSec_TextChanged(sender As Object, e As EventArgs) Handles txtSec.TextChanged

    End Sub

    Private Sub TxtClave_TextChanged(sender As Object, e As EventArgs) Handles txtClave.TextChanged

    End Sub

    Private Sub TxtValor_TextChanged(sender As Object, e As EventArgs) Handles txtValor.TextChanged

    End Sub

    Private Sub ListView1_SelectedIndexChanged_2(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged

    End Sub
End Class