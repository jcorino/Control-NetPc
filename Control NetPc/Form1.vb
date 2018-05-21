
Option Strict On

Imports System.IO.Ports

Public Class Form1
    Inherits System.Windows.Forms.Form
    Dim myPuertoSerie As New PuertoCom("COM6", 115200, Parity.None, 8, StopBits.One)
    '
    ' Arrays para contener los controles
    ' (definir los arrays que vamos a usar)
    Private m_Label1 As New ControlArray("Label1")
    Private m_TextBox1 As New ControlArray("TextBox1")
    Private m_RadioButton1 As New ControlArray("RadioButton1")

    ' Asignar los eventos a los controles
    Private Sub AsignarEventos()
        Dim txt As TextBox
        Dim opt As RadioButton
        '
        ' Aquí estarán los procedimientos a asignar a cada array de controles
        '
        For Each opt In m_RadioButton1
            AddHandler opt.KeyPress, AddressOf RadioButton1_KeyPress
            'AddHandler opt.CheckedChanged, AddressOf RadioButton1_CheckedChanged
        Next
        For Each txt In m_TextBox1
            AddHandler txt.Enter, AddressOf TextBox1_Enter
            AddHandler txt.KeyPress, AddressOf TextBox1_KeyPress
            AddHandler txt.Leave, AddressOf TextBox1_Leave
            'AddHandler txt.TextChanged, AddressOf TextBox1_TextChanged
        Next
        '
    End Sub
    '
    Private Sub Form1_Load(ByVal sender As Object,
                    ByVal e As System.EventArgs) Handles MyBase.Load

        'Lista puertos series disponibles
        For Each s As String In My.Computer.Ports.SerialPortNames
            ListBox1.Items.Add(s)
        Next

        ' Asignar los controles y reorganizar los índices
        m_Label1.AsignarControles(Me.Controls)
        m_TextBox1.AsignarControles(Me.Controls)
        m_RadioButton1.AsignarControles(Me.Controls)

        'Asignar sólo los eventos
        AsignarEventos()
    End Sub
    '
    Private Sub TextBox1_Enter(ByVal sender As Object,
                    ByVal e As System.EventArgs)
        '
        Dim txt As TextBox = CType(sender, TextBox)
        Dim Index As Integer = m_TextBox1.Index(txt)
        '
        txt.SelectAll()
        ' resaltar la etiqueta
        m_Label1(Index).BackColor = Color.FromKnownColor(KnownColor.ControlDark)
        '
    End Sub

    Private Sub TextBox1_Leave(ByVal sender As Object, ByVal e As System.EventArgs)
        '
        Dim txt As TextBox = CType(sender, TextBox)
        Dim Index As Integer = m_TextBox1.Index(txt)
        '
        ' poner la etiqueta con el color normal
        m_Label1(Index).BackColor = Color.FromKnownColor(KnownColor.Control)
    End Sub

    Private Sub TextBox1_KeyPress(ByVal sender As Object,
                    ByVal e As System.Windows.Forms.KeyPressEventArgs)
        '
        If e.KeyChar = ChrW(Keys.Return) Then
            Dim txt As TextBox = CType(sender, TextBox)
            Dim Index As Integer = m_TextBox1.Index(txt)
            '
            If Index = 2 Then
                m_RadioButton1(0).Focus()
            Else
                m_TextBox1(Index + 1).Focus()
            End If
        End If
    End Sub

    'Private Sub TextBox1_TextChanged(ByVal sender As Object, _
    '                ByVal e As System.EventArgs)
    '    '
    'End Sub
    '
    'Private Sub RadioButton1_CheckedChanged(ByVal sender As Object, _
    '                ByVal e As System.EventArgs)
    '    '
    'End Sub
    '

    Private Sub RadioButton1_KeyPress(ByVal sender As Object,
                    ByVal e As System.Windows.Forms.KeyPressEventArgs)
        If e.KeyChar = ChrW(Keys.Return) Then
            Dim opt As RadioButton = CType(sender, RadioButton)
            Dim Index As Integer = m_TextBox1.Index(opt)
            '
            If Index = 0 Then
                m_RadioButton1(Index + 1).Focus()
            Else
                m_TextBox1(0).Focus()
            End If
        End If
    End Sub
    '
    Private Sub BtnCerrar_Click(ByVal sender As Object,
                    ByVal e As System.EventArgs)

        Me.Close()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form2.Show()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub Label12_Click(sender As Object, e As EventArgs) Handles Label12.Click

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
            'result += String.Format("{0:X2}", item)
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

    Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
        'myPuertoSerie.EnviarSerie(1, 2, 3, 3, 4, 5, 6)
        If String.IsNullOrEmpty(myPuertoSerie.BufferRecepcion) Then

        Else
            Me.TextBox1.Text = myPuertoSerie.BufferRecepcion
            myPuertoSerie.BufferRecepcion = ""
        End If
    End Sub

    Private Sub Timer4_Tick(sender As Object, e As EventArgs) Handles Timer4.Tick
        TextBox2.Text = "Motor: " & CStr(myPuertoSerie.PlacasMotores(1).NroMotor) & vbCrLf
        TextBox2.Text = TextBox2.Text & "Status: " & CStr(myPuertoSerie.PlacasMotores(1).StatusByte) & vbCrLf
        TextBox2.Text = TextBox2.Text & "Status1: " & CStr(myPuertoSerie.PlacasMotores(1).StatusByte1) & vbCrLf
        TextBox2.Text = TextBox2.Text & "Status2: " & CStr(myPuertoSerie.PlacasMotores(1).StatusByte2) & vbCrLf
        TextBox2.Text = TextBox2.Text & "Status3: " & CStr(myPuertoSerie.PlacasMotores(1).StatusByte3) & vbCrLf
        TextBox2.Text = TextBox2.Text & "Status4: " & CStr(myPuertoSerie.PlacasMotores(1).StatusByte4) & vbCrLf
        TextBox2.Text = TextBox2.Text & "Limite Inf: " & CStr(myPuertoSerie.PlacasMotores(1).LimiteInf) & vbCrLf
        TextBox2.Text = TextBox2.Text & "Limite Sup: " & CStr(myPuertoSerie.PlacasMotores(1).LimiteSup) & vbCrLf
        TextBox2.Text = TextBox2.Text & "Target Encoder: " & CStr(myPuertoSerie.PlacasMotores(1).TargetEncoder) & vbCrLf
        TextBox2.Text = TextBox2.Text & "Velocidad: " & CStr(myPuertoSerie.PlacasMotores(1).Velocidad) & vbCrLf
        TextBox2.Text = TextBox2.Text & "Encoder: " & CStr(myPuertoSerie.PlacasMotores(1).ActualEncoder) & vbCrLf

    End Sub
End Class
