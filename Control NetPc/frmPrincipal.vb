Option Strict On

Imports System.IO.Ports

Public Class FrmPrincipal

    Inherits System.Windows.Forms.Form
    Dim myPuertoSerie As New PuertoCom()

    ' Arrays para contener los controles
    ' (definir los arrays que vamos a usar)
    Private m_BtnUP As New ControlArray("BtnUP")
    Private m_LblPos As New ControlArray("LblPos")
    Private m_BtnDown As New ControlArray("BtnDown")
    Private m_BtnStop As New ControlArray("BtnStop")

    ' Asignar los eventos a los controles
    Private Sub AsignarEventos()
            Dim btn As Button
            '
            ' Aquí estarán los procedimientos a asignar a cada array de controles

            For Each btn In m_BtnUP
                AddHandler btn.Click, AddressOf BtnUp_Click
            Next

            For Each btn In m_BtnDown
                AddHandler btn.Click, AddressOf BtnDown_Click
            Next

            For Each btn In m_BtnStop
                AddHandler btn.Click, AddressOf BtnStop_Click
            Next

        End Sub

    Private Sub Form1_Load(ByVal sender As Object,
                    ByVal e As System.EventArgs) Handles MyBase.Load

        myPuertoSerie.InitSerial("COM5", 115200, Parity.None, 8, StopBits.One)  'Inicio Puerto seria
        myPuertoSerie.UseCheckPacket = False        'Si voy a utilizar chequeo de tramas con las placas
        myPuertoSerie.PoollTime = 50                'Tiempo de pooleo a las placas en ms
        myPuertoSerie.CantidadMotores = 12          'Set cantidad de placas a utilizar

        'Lista puertos series disponibles
        For Each s As String In My.Computer.Ports.SerialPortNames
            ListBox1.Items.Add(s)
        Next

        ' Asignar los controles y reorganizar los índices
        m_LblPos.AsignarControles(Me.Controls)
        m_BtnUP.AsignarControles(Me.Controls)
        m_BtnDown.AsignarControles(Me.Controls)
        m_BtnStop.AsignarControles(Me.Controls)
        ' Asignar sólo los eventos
        AsignarEventos()

    End Sub

        Private Sub BtnUp_Click(ByVal sender As Object,
                    ByVal e As System.EventArgs)
            '
            Dim txt As Button = CType(sender, Button)
            Dim Index As Integer = m_BtnUP.Index(txt)

            'm_BtnUP(Index).Text = "asdfsdf"

        End Sub

        Private Sub BtnDown_Click(ByVal sender As Object,
                    ByVal e As System.EventArgs)
            '
            Dim txt As Button = CType(sender, Button)
            Dim Index As Integer = m_BtnDown.Index(txt)

            'm_BtnDown(Index).Text = "12345"

        End Sub

        Private Sub BtnStop_Click(ByVal sender As Object,
                    ByVal e As System.EventArgs)
            '
            Dim txt As Button = CType(sender, Button)
            Dim Index As Integer = m_BtnStop.Index(txt)

            'm_BtnStop(Index).Text = "6789"

        End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        myPuertoSerie.PoolPlacas(False)
        Me.Close()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
            Static Dim a As Single
            Static Dim seg As Single
            Static Dim min As Single
            Static Dim hor As Single

            a = a + 1

            If a = 25 Then
                a = 0
                seg += 1

                If seg = 60 Then
                    seg = 0
                    min += 1
                End If

                If min = 60 Then
                    min = 0
                    hor += 1
                End If

                If hor = 100 Then
                    hor = 0
                End If

                Label27.Text = hor.ToString(“##00”) & ":" & min.ToString(“##00”) & ":" & seg.ToString(“##00”)
            End If

            Label21.Text = a.ToString(“##00”)

        End Sub

        Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
            Static Dim seg As Single
            Static Dim min As Single
            Static Dim hor As Single

            seg += 1

            If seg = 60 Then
                seg = 0
                min += 1
            End If

            If min = 60 Then
                min = 0
                hor += 1
            End If

            If hor = 100 Then
                hor = 0
            End If

            Label12.Text = hor.ToString(“##00”) & ":" & min.ToString(“##00”) & ":" & seg.ToString(“##00”)

        End Sub

        Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
            Timer1.Enabled = True
            Timer2.Enabled = True
        End Sub

        Private Function DisplayValue(values As Byte()) As String
            Dim result As String = String.Empty

            For Each item As Byte In values
                result += String.Format("{0:0}", item)
                result += ", "
            Next
            Return result
        End Function

        Private Function DisplayValueRegister(values As Byte()) As String
            Dim result As String = String.Empty
            Dim datu As Integer

            If values.Length > 2 Then
                For i As Integer = 1 To (values.Length - 2) Step 2
                    datu = (CInt(values(i)) << 8)
                    datu += values(i + 1)
                    result += String.Format("{0:00000}", datu)
                    result += ", "
                Next
                Return result
                Exit Function
            Else
                For Each item As Byte In values
                    'result += String.Format("{0:X2}", item)
                    result += String.Format("{0:000}", item)
                    result += ", "
                Next
                Return result
                Exit Function
            End If

        End Function


        Private Sub Timer4_Tick(sender As Object, e As EventArgs) Handles Timer4.Tick
            ActualizarPrincipal()
        End Sub

        Public Sub ActualizarPrincipal()

        'Cantidad de placas mostradas en pantalla 12
        For e As Byte = 1 To 12
            m_LblPos(e - 1).Text = (myPuertoSerie.PlacasMotores(e).ActualEncoder).ToString("#####00000")
        Next

    End Sub

    Private Sub BtnConfig_Click(sender As Object, e As EventArgs) Handles BtnConfig.Click
        Form2.Show()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        myPuertoSerie.PoolPlacas(True)
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click

    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        myPuertoSerie.AccionesMotores(PuertoCom.ComandoMotor.cGoAutomatic, 2, 65535)

    End Sub

End Class
