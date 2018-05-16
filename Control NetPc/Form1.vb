'------------------------------------------------------------------------------
' Prueba para usar la clase ControlArray con VB .NET                (14/Nov/02)
'
' ©Guillermo 'guille' Som, 2002
'------------------------------------------------------------------------------
Option Strict On

Public Class Form1
    Inherits System.Windows.Forms.Form

    '
    ' Arrays para contener los controles
    ' (definir los arrays que vamos a usar)
    Private m_Label1 As New ControlArray("Label1")
    Private m_TextBox1 As New ControlArray("TextBox1")
    Private m_RadioButton1 As New ControlArray("RadioButton1")
    '
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
        '
        ' Asignar los controles y reorganizar los índices
        m_Label1.AsignarControles(Me.Controls)
        m_TextBox1.AsignarControles(Me.Controls)
        m_RadioButton1.AsignarControles(Me.Controls)

        ' Asignar sólo los eventos
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
        If a = 25 Then
            a = 0
        Else
            a = a + 1
        End If
        Label21.Text = a.ToString(“##00”)

    End Sub
End Class
