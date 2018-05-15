<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnCerrar = New System.Windows.Forms.Button()
        Me.TextBox1_2 = New System.Windows.Forms.TextBox()
        Me.Label1_2 = New System.Windows.Forms.Label()
        Me.TextBox1_1 = New System.Windows.Forms.TextBox()
        Me.Label1_1 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.RadioButton1_1 = New System.Windows.Forms.RadioButton()
        Me.RadioButton1_0 = New System.Windows.Forms.RadioButton()
        Me.TextBox1_0 = New System.Windows.Forms.TextBox()
        Me.Label1_0 = New System.Windows.Forms.Label()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnCerrar
        '
        Me.btnCerrar.Location = New System.Drawing.Point(460, 322)
        Me.btnCerrar.Name = "btnCerrar"
        Me.btnCerrar.Size = New System.Drawing.Size(75, 23)
        Me.btnCerrar.TabIndex = 23
        Me.btnCerrar.Text = "Cerrar"
        '
        'TextBox1_2
        '
        Me.TextBox1_2.Location = New System.Drawing.Point(376, 162)
        Me.TextBox1_2.Name = "TextBox1_2"
        Me.TextBox1_2.Size = New System.Drawing.Size(160, 20)
        Me.TextBox1_2.TabIndex = 22
        Me.TextBox1_2.Text = "TextBox3"
        '
        'Label1_2
        '
        Me.Label1_2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label1_2.Location = New System.Drawing.Point(268, 162)
        Me.Label1_2.Name = "Label1_2"
        Me.Label1_2.Size = New System.Drawing.Size(100, 23)
        Me.Label1_2.TabIndex = 21
        Me.Label1_2.Text = "Label3"
        '
        'TextBox1_1
        '
        Me.TextBox1_1.Location = New System.Drawing.Point(376, 134)
        Me.TextBox1_1.Name = "TextBox1_1"
        Me.TextBox1_1.Size = New System.Drawing.Size(160, 20)
        Me.TextBox1_1.TabIndex = 20
        Me.TextBox1_1.Text = "TextBox2"
        '
        'Label1_1
        '
        Me.Label1_1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label1_1.Location = New System.Drawing.Point(268, 134)
        Me.Label1_1.Name = "Label1_1"
        Me.Label1_1.Size = New System.Drawing.Size(100, 23)
        Me.Label1_1.TabIndex = 19
        Me.Label1_1.Text = "Label2"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.RadioButton1_1)
        Me.Panel1.Controls.Add(Me.RadioButton1_0)
        Me.Panel1.Location = New System.Drawing.Point(264, 194)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(124, 68)
        Me.Panel1.TabIndex = 18
        '
        'RadioButton1_1
        '
        Me.RadioButton1_1.Location = New System.Drawing.Point(12, 36)
        Me.RadioButton1_1.Name = "RadioButton1_1"
        Me.RadioButton1_1.Size = New System.Drawing.Size(104, 24)
        Me.RadioButton1_1.TabIndex = 1
        Me.RadioButton1_1.Text = "RadioButton2"
        '
        'RadioButton1_0
        '
        Me.RadioButton1_0.Location = New System.Drawing.Point(12, 8)
        Me.RadioButton1_0.Name = "RadioButton1_0"
        Me.RadioButton1_0.Size = New System.Drawing.Size(104, 24)
        Me.RadioButton1_0.TabIndex = 0
        Me.RadioButton1_0.Text = "RadioButton1"
        '
        'TextBox1_0
        '
        Me.TextBox1_0.Location = New System.Drawing.Point(376, 106)
        Me.TextBox1_0.Name = "TextBox1_0"
        Me.TextBox1_0.Size = New System.Drawing.Size(160, 20)
        Me.TextBox1_0.TabIndex = 17
        Me.TextBox1_0.Text = "TextBox1"
        '
        'Label1_0
        '
        Me.Label1_0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.Label1_0.Location = New System.Drawing.Point(268, 106)
        Me.Label1_0.Name = "Label1_0"
        Me.Label1_0.Size = New System.Drawing.Size(100, 23)
        Me.Label1_0.TabIndex = 16
        Me.Label1_0.Text = "Label1"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.btnCerrar)
        Me.Controls.Add(Me.TextBox1_2)
        Me.Controls.Add(Me.Label1_2)
        Me.Controls.Add(Me.TextBox1_1)
        Me.Controls.Add(Me.Label1_1)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.TextBox1_0)
        Me.Controls.Add(Me.Label1_0)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnCerrar As Button
    Friend WithEvents TextBox1_2 As TextBox
    Friend WithEvents Label1_2 As Label
    Friend WithEvents TextBox1_1 As TextBox
    Friend WithEvents Label1_1 As Label
    Friend WithEvents Panel1 As Panel
    Friend WithEvents RadioButton1_1 As RadioButton
    Friend WithEvents RadioButton1_0 As RadioButton
    Friend WithEvents TextBox1_0 As TextBox
    Friend WithEvents Label1_0 As Label
End Class
