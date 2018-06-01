Option Strict On

Public Class FrmPrincipal

    Inherits System.Windows.Forms.Form

    Private EnableGo As Boolean = False

    'Instacio la Clase. Hay que llamarla con la cantidad de
    'Nodos a utilizar.
    Public myPuertoSerie As New NodeComunication(12)

    'Arrays para contener los controles. Esto para tener 
    'arreglos de controles que se puedan direccionar desde
    'un indice. (definir los arrays que vamos a usar)
    Private m_BtnUP As New ControlArray("BtnUP")
    Private m_LblPos As New ControlArray("LblPos")
    Private m_LblLimUP As New ControlArray("LblLimUP")
    Private m_LblLimDWN As New ControlArray("LblLimDWN")
    Private m_BtnDown As New ControlArray("BtnDown")
    Private m_BtnStop As New ControlArray("BtnStop")
    Private m_ChbEnable As New ControlArray("ChbEnable")

    Private Sub AsignarEventos()
        'Asignar los eventos a los controles
        'Esto para tener arreglos de controles que
        'se puedan direccionar desde un indice.
        Dim btn As Button
        Dim chk As CheckBox

        'Aquí estarán los procedimientos a
        'asignar a cada array de controles

        For Each btn In m_BtnUP
            AddHandler btn.Click, AddressOf BtnUp_Click
        Next

        For Each btn In m_BtnDown
            AddHandler btn.Click, AddressOf BtnDown_Click
        Next

        For Each btn In m_BtnStop
            AddHandler btn.Click, AddressOf BtnStop_Click
        Next

        For Each chk In m_ChbEnable
            AddHandler chk.CheckedChanged, AddressOf ChbEnable_CheckedChanged
        Next

    End Sub

    Private Sub Form1_Load(ByVal sender As Object,
                    ByVal e As System.EventArgs) Handles MyBase.Load


        myPuertoSerie.InitSerial()                          'Inicio Puerto serie
        myPuertoSerie.PoollTime = 5                         'Tiempo de pooleo a las placas en ms
        myPuertoSerie.UseCheckPacket = True                 'Si voy a utilizar chequeo de tramas con las placas
        myPuertoSerie.HabilitarPoollingAutomatico = True    'Habilita pooling automatico
        myPuertoSerie.ActivarComunicacion = True            'Inhabilita la escritura de paquetes en el puerto serie el resto es igual.
        myPuertoSerie.PoolPlacas(True)                      'Inicia comunicacion con placas independientemente
        '                                                   que este HabilitarPoollingAutomatico

        'Asignar los controles y reorganizar los índices
        'Esto es para manejo de colecciones de controles
        m_LblPos.AsignarControles(Me.Controls)
        m_LblLimUP.AsignarControles(Me.Controls)
        m_LblLimDWN.AsignarControles(Me.Controls)
        m_BtnUP.AsignarControles(Me.Controls)
        m_BtnDown.AsignarControles(Me.Controls)
        m_BtnStop.AsignarControles(Me.Controls)
        m_ChbEnable.AsignarControles(Me.Controls)
        AsignarEventos()    ' Asignar sólo los eventos

    End Sub

    Private Sub BtnUp_Click(ByVal sender As Object,
                    ByVal e As System.EventArgs)
        '
        Dim txt As Button = CType(sender, Button)
        Dim Index As Byte = CByte(m_BtnUP.Index(txt))

        myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cSubir, CByte(Index + 1), 0, 0,,, True)

    End Sub

    Private Sub BtnDown_Click(ByVal sender As Object,
                    ByVal e As System.EventArgs)
        '
        Dim txt As Button = CType(sender, Button)
        Dim Index As Byte = CByte(m_BtnDown.Index(txt))

        myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cBajar, CByte(Index + 1), 0, 0,,, True)

    End Sub

    Private Sub BtnStop_Click(ByVal sender As Object,
                    ByVal e As System.EventArgs)
        '
        Dim txt As Button = CType(sender, Button)
        Dim Index As Byte = CByte(m_BtnStop.Index(txt))

        myPuertoSerie.ClearBufferTX(CByte(Index + 1))
        myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cStop, CByte(Index + 1), 0, 0)

    End Sub

    Private Sub ChbEnable_CheckedChanged(sender As Object, e As EventArgs)
        Dim txt As CheckBox = CType(sender, CheckBox)
        Dim Index As Byte = CByte(m_ChbEnable.Index(txt))

        If txt.CheckState = CheckState.Checked Then
            myPuertoSerie.NodeStatus(CByte(Index + 1)).Enable = True
        Else
            myPuertoSerie.NodeStatus(CByte(Index + 1)).Enable = False
        End If

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
            m_LblPos(e - 1).Text = (myPuertoSerie.NodeStatus(e).ActualEncoder).ToString("#####00000")
        Next
        For e As Byte = 1 To 12
            m_LblLimUP(e - 1).Text = (myPuertoSerie.NodeStatus(e).LimiteSup).ToString("#####00000")
        Next
        For e As Byte = 1 To 12
            m_LblLimDWN(e - 1).Text = (myPuertoSerie.NodeStatus(e).LimiteInf).ToString("#####00000")
        Next

    End Sub

    Private Sub BtnConfig_Click(sender As Object, e As EventArgs) Handles BtnConfig.Click
        FrmConfig.Show()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        myPuertoSerie.PoolPlacas(True)
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        myPuertoSerie.ClearBufferTX(1)
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        If EnableGo Then
            myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, 1, CUShort(LblGo_00.Text))
        End If
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged

        If CheckBox1.CheckState = CheckState.Checked Then
            EnableGo = True                'Habilitar los botones Go
        Else
            EnableGo = False
        End If

    End Sub

End Class
