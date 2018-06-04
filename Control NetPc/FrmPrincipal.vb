Option Strict On
Imports System.IO

Public Class FrmPrincipal

    Inherits System.Windows.Forms.Form

    Public mCfg As jmc.Util.ConfigXml

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
    Public m_LblName As New ControlArray("LblName")
    Private m_LblGo As New ControlArray("LblGo")
    Private m_BtnDown As New ControlArray("BtnDown")
    Private m_BtnStop As New ControlArray("BtnStop")
    Private m_ChbEnable As New ControlArray("ChbEnable")
    Private m_BtnGo As New ControlArray("BtnGo")
    Private m_TxtVel As New ControlArray("TxtVel")
    Private m_PnlPause As New ControlArray("PnlPause")

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

        For Each btn In m_BtnGo
            AddHandler btn.Click, AddressOf BtnGo_Click
        Next

        For Each chk In m_ChbEnable
            AddHandler chk.CheckedChanged, AddressOf ChbEnable_CheckedChanged
        Next



    End Sub

    Private Sub Form1_Load(ByVal sender As Object,
                    ByVal e As System.EventArgs) Handles MyBase.Load

        'Asignar los controles y reorganizar los índices
        'Esto es para manejo de colecciones de controles
        m_LblPos.AsignarControles(Me.Controls)
        m_LblLimUP.AsignarControles(Me.Controls)
        m_LblLimDWN.AsignarControles(Me.Controls)
        m_LblGo.AsignarControles(Me.Controls)
        m_LblName.AsignarControles(Me.Controls)
        m_BtnUP.AsignarControles(Me.Controls)
        m_BtnDown.AsignarControles(Me.Controls)
        m_BtnStop.AsignarControles(Me.Controls)
        m_BtnGo.AsignarControles(Me.Controls)
        m_ChbEnable.AsignarControles(Me.Controls)
        m_TxtVel.AsignarControles(Me.Controls)
        m_PnlPause.AsignarControles(Me.Controls)

        AsignarEventos()    ' Asignar sólo los eventos

        'Abro archivo XML de configuracion para cargar parametros
        mCfg = New jmc.Util.ConfigXml(Path.GetFullPath("config.cfg"), True)

        'Tiempo de polling a las placas en ms
        myPuertoSerie.PollTime = CInt(mCfg.GetValue("General", "PollTime"))

        'Si voy a utilizar chequeo de tramas con las placas
        myPuertoSerie.UseCheckPacket = CBool(mCfg.GetValue("General", "UseCheckPacket"))

        'Habilita pooling automatico
        myPuertoSerie.HabilitarPoollingAutomatico = CBool(mCfg.GetValue("General", "HabilitarPoollingAutomatico"))

        'Inhabilita la escritura de paquetes en el puerto serie el resto es igual.
        myPuertoSerie.ActivarComunicacion = CBool(mCfg.GetValue("General", "ActivarComunicacion"))

        myPuertoSerie.ComPort = mCfg.GetValue("General", "ComPort")
        myPuertoSerie.ComBaurate = CInt(mCfg.GetValue("General", "Baudrate"))


        'Esto es una chanchada ya que no logre hacer que funcione el arreglo de
        'CheckBox. Asi que hasta encontrar otra solucion los gestino de a uno

        If (mCfg.GetValue("Nodo0", "Enable")) = "Disable" Then
            ChbEnable_00.CheckState = CheckState.Unchecked
            myPuertoSerie.NodeStatus(0).Enable = False
        Else
            ChbEnable_00.CheckState = CheckState.Checked
            myPuertoSerie.NodeStatus(0).Enable = True
        End If

        If (mCfg.GetValue("Nodo1", "Enable")) = "Disable" Then
            ChbEnable_01.CheckState = CheckState.Unchecked
            myPuertoSerie.NodeStatus(1).Enable = False
        Else
            ChbEnable_01.CheckState = CheckState.Checked
            myPuertoSerie.NodeStatus(1).Enable = True
        End If

        If (mCfg.GetValue("Nodo2", "Enable")) = "Disable" Then
            ChbEnable_02.CheckState = CheckState.Unchecked
            myPuertoSerie.NodeStatus(2).Enable = False
        Else
            ChbEnable_02.CheckState = CheckState.Checked
            myPuertoSerie.NodeStatus(2).Enable = True
        End If

        If (mCfg.GetValue("Nodo3", "Enable")) = "Disable" Then
            ChbEnable_03.CheckState = CheckState.Unchecked
            myPuertoSerie.NodeStatus(3).Enable = False
        Else
            ChbEnable_03.CheckState = CheckState.Checked
            myPuertoSerie.NodeStatus(3).Enable = True
        End If

        If (mCfg.GetValue("Nodo4", "Enable")) = "Disable" Then
            ChbEnable_04.CheckState = CheckState.Unchecked
            myPuertoSerie.NodeStatus(4).Enable = False
        Else
            ChbEnable_04.CheckState = CheckState.Checked
            myPuertoSerie.NodeStatus(4).Enable = True
        End If

        If (mCfg.GetValue("Nodo5", "Enable")) = "Disable" Then
            ChbEnable_05.CheckState = CheckState.Unchecked
            myPuertoSerie.NodeStatus(5).Enable = False
        Else
            ChbEnable_05.CheckState = CheckState.Checked
            myPuertoSerie.NodeStatus(5).Enable = True
        End If

        If (mCfg.GetValue("Nodo6", "Enable")) = "Disable" Then
            ChbEnable_06.CheckState = CheckState.Unchecked
            myPuertoSerie.NodeStatus(6).Enable = False
        Else
            ChbEnable_06.CheckState = CheckState.Checked
            myPuertoSerie.NodeStatus(6).Enable = True
        End If

        If (mCfg.GetValue("Nodo7", "Enable")) = "Disable" Then
            ChbEnable_07.CheckState = CheckState.Unchecked
            myPuertoSerie.NodeStatus(7).Enable = False
        Else
            ChbEnable_07.CheckState = CheckState.Checked
            myPuertoSerie.NodeStatus(7).Enable = True
        End If

        If (mCfg.GetValue("Nodo8", "Enable")) = "Disable" Then
            ChbEnable_08.CheckState = CheckState.Unchecked
            myPuertoSerie.NodeStatus(8).Enable = False
        Else
            ChbEnable_08.CheckState = CheckState.Checked
            myPuertoSerie.NodeStatus(8).Enable = True
        End If

        If (mCfg.GetValue("Nodo9", "Enable")) = "Disable" Then
            ChbEnable_09.CheckState = CheckState.Unchecked
            myPuertoSerie.NodeStatus(9).Enable = False
        Else
            ChbEnable_09.CheckState = CheckState.Checked
            myPuertoSerie.NodeStatus(9).Enable = True
        End If

        If (mCfg.GetValue("Nodo10", "Enable")) = "Disable" Then
            ChbEnable_10.CheckState = CheckState.Unchecked
            myPuertoSerie.NodeStatus(10).Enable = False
        Else
            ChbEnable_10.CheckState = CheckState.Checked
            myPuertoSerie.NodeStatus(10).Enable = True
        End If

        If (mCfg.GetValue("Nodo11", "Enable")) = "Disable" Then
            ChbEnable_11.CheckState = CheckState.Unchecked
            myPuertoSerie.NodeStatus(11).Enable = False
        Else
            ChbEnable_11.CheckState = CheckState.Checked
            myPuertoSerie.NodeStatus(11).Enable = True
        End If

        'Cargo datos de los nodos desde archivo de configuracion
        For w As Byte = 0 To 11
            myPuertoSerie.NodeStatus(w).Nombre = mCfg.GetValue("Nodo" & w + 1, "Name")
            m_LblName(w).Text = myPuertoSerie.NodeStatus(w).Nombre
            myPuertoSerie.NodeStatus(w).CmPulse = CUShort(mCfg.GetValue("Nodo" & w + 1, "CmX1000"))
        Next



        'Inicio Puerto serie
        myPuertoSerie.InitSerial()

        myPuertoSerie.PoolPlacas(True)                      'Inicia comunicacion con placas independientemente
        '                                                   que este HabilitarPoollingAutomatico
        TmrActualizarPrincipal.Enabled = True

    End Sub

    Private Sub BtnUp_Click(ByVal sender As Object,
                    ByVal e As System.EventArgs)
        '
        Dim txt As Button = CType(sender, Button)
        Dim Index As Byte = CByte(m_BtnUP.Index(txt))

        myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cSubir, CByte(Index), 0, 0,,, True)

    End Sub

    Private Sub BtnDown_Click(ByVal sender As Object,
                    ByVal e As System.EventArgs)
        '
        Dim txt As Button = CType(sender, Button)
        Dim Index As Byte = CByte(m_BtnDown.Index(txt))

        myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cBajar, CByte(Index), 0, 0,,, True)

    End Sub

    Private Sub BtnStop_Click(ByVal sender As Object,
                    ByVal e As System.EventArgs)
        '
        Dim txt As Button = CType(sender, Button)
        Dim Index As Byte = CByte(m_BtnStop.Index(txt))

        myPuertoSerie.ClearBufferTX(CByte(Index))
        myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cStop, CByte(Index), 0, 0)

    End Sub

    Private Sub BtnGo_Click(sender As Object, e As EventArgs)
        '
        Dim txt As Button = CType(sender, Button)
        Dim Index As Byte = CByte(m_BtnGo.Index(txt))

        If EnableGo Then
            myPuertoSerie.AccionesMotores(NodeComunication.ComandoMotor.cGoAutomatic, CByte(Index), CUShort(m_LblGo(Index).Text))
        End If
    End Sub

    Private Sub ChbEnable_CheckedChanged(ByVal sender As Object,
                                         ByVal e As EventArgs)

        Dim txt As CheckBox = CType(sender, CheckBox)
        Dim Index As Byte = CByte(m_ChbEnable.Index(txt))

        If txt.CheckState = CheckState.Checked Then
            myPuertoSerie.NodeStatus(CByte(Index)).Enable = True
            mCfg.SetValue("Nodo" & (Index), "Enable", "Enable")
            m_BtnDown(Index).Enabled = True
            m_BtnDown(Index).BackColor = Color.Red
            m_BtnDown(Index).ForeColor = Color.WhiteSmoke
            m_BtnGo(Index).Enabled = True
            m_BtnStop(Index).Enabled = True
            m_BtnStop(Index).ForeColor = Color.WhiteSmoke
            m_BtnUP(Index).Enabled = True
            m_BtnUP(Index).BackColor = Color.Green
            m_BtnUP(Index).ForeColor = Color.WhiteSmoke
            m_LblGo(Index).Enabled = True
            m_LblGo(Index).ForeColor = Color.WhiteSmoke
            m_LblLimDWN(Index).ForeColor = Color.Red
            m_LblLimUP(Index).ForeColor = Color.Red
            m_LblName(Index).ForeColor = Color.WhiteSmoke
            m_LblPos(Index).ForeColor = Color.WhiteSmoke

        Else
            myPuertoSerie.NodeStatus(CByte(Index)).Enable = False
            mCfg.SetValue("Nodo" & (Index), "Enable", "Disable")
            m_BtnDown(Index).BackColor = Color.FromArgb(85, 85, 106)
            m_BtnDown(Index).ForeColor = Color.FromArgb(66, 66, 92)
            m_BtnStop(Index).ForeColor = Color.FromArgb(66, 66, 92)
            m_BtnUP(Index).BackColor = Color.FromArgb(85, 85, 106)
            m_BtnUP(Index).ForeColor = Color.FromArgb(66, 66, 92)
            m_LblGo(Index).Enabled = False
            m_LblGo(Index).ForeColor = Color.DimGray
            m_LblLimDWN(Index).ForeColor = Color.DimGray
            m_LblLimUP(Index).ForeColor = Color.DimGray
            m_LblName(Index).ForeColor = Color.DimGray
            m_LblPos(Index).ForeColor = Color.DimGray

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

    Private Sub Timer4_Tick(sender As Object, e As EventArgs) Handles TmrActualizarPrincipal.Tick
        ActualizarPrincipal()
    End Sub

    Public Sub ActualizarPrincipal()

        'Cantidad de placas mostradas en pantalla 12
        For e As Byte = 0 To 11
            m_LblPos(e).Text = (myPuertoSerie.NodeStatus(e).ActualEncoder).ToString("#####00000")
            m_LblLimUP(e).Text = (myPuertoSerie.NodeStatus(e).LimiteSup).ToString("#####00000")
            m_LblLimDWN(e).Text = (myPuertoSerie.NodeStatus(e).LimiteInf).ToString("#####00000")
            m_TxtVel(e).Text = (myPuertoSerie.NodeStatus(e).Velocidad).ToString("##00")

            'Colores de botones UP segun estado recibido
            If myPuertoSerie.NodeStatus(e).IsUp = True Then
                m_BtnUP(e).BackColor = Color.Yellow
            Else
                If myPuertoSerie.NodeStatus(e).Enable Then
                    m_BtnUP(e).BackColor = Color.Green
                Else
                    m_BtnUP(e).BackColor = Color.FromArgb(85, 85, 106)
                End If
            End If

            'Colores de botones DOWN segun estado recibido
            If myPuertoSerie.NodeStatus(e).IsDown = True Then
                m_BtnDown(e).BackColor = Color.Yellow
            Else
                If myPuertoSerie.NodeStatus(e).Enable Then
                    m_BtnDown(e).BackColor = Color.Red
                Else
                    m_BtnDown(e).BackColor = Color.FromArgb(85, 85, 106)
                End If
            End If

            'Colores de limite Superior de acuerdo a lo recibido
            If myPuertoSerie.NodeStatus(e).IsSuperoLimSup = True Then
                m_LblLimUP(e).BackColor = Color.Yellow
            Else
                m_LblLimUP(e).BackColor = Color.FromArgb(66, 66, 92)
            End If

            'Colores de limite Inferior de acuerdo a lo recibido
            If myPuertoSerie.NodeStatus(e).IsSuperoLimInf = True Then
                m_LblLimDWN(e).BackColor = Color.Yellow
            Else
                m_LblLimDWN(e).BackColor = Color.FromArgb(66, 66, 92)
            End If

            'Colores de Pausa de acuerdo a lo recibido
            If myPuertoSerie.NodeStatus(e).IsInPause = True Then
                m_PnlPause(e).BackColor = Color.Yellow
            Else
                m_PnlPause(e).BackColor = Color.FromArgb(66, 66, 92)
            End If

        Next

    End Sub

    Private Sub BtnConfig_Click(sender As Object, e As EventArgs) Handles BtnConfig.Click
        FrmConfig.Show()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        myPuertoSerie.PoolPlacas(True)
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs)
        myPuertoSerie.ClearBufferTX(1)
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged

        If CheckBox1.CheckState = CheckState.Checked Then
            EnableGo = True                'Habilitar los botones Go
            For i As Byte = 0 To 11
                m_BtnGo(i).ForeColor = Color.WhiteSmoke
            Next
        Else
            EnableGo = False
            For i As Byte = 0 To 11
                m_BtnGo(i).ForeColor = Color.Black
            Next
        End If

    End Sub

    Private Sub Panel17_Paint(sender As Object, e As PaintEventArgs) Handles Panel17.Paint

    End Sub
End Class
