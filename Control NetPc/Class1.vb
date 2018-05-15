Option Strict On

'<summary>
' Colección de objetos de tipo Control
'</summary>
Public Class ControlArray
    Inherits CollectionBase
    '
    Private mNombre As String
    '
    Public Sub New()
        MyBase.New()
    End Sub
    Public Sub New(ByVal elNombre As String)
        MyBase.New()
        mNombre = elNombre
    End Sub
    '
    Public Function Add(ByVal ctrl As Control) As Integer
        Return MyBase.List.Add(ctrl)
    End Function
    '
    ' Asignar los controles que contendrá esta colección            (14/Nov/02)
    Public Sub AsignarControles(ByVal ctrls As Control.ControlCollection,
                    Optional ByVal elNombre As String = "")
        ' Asignar los controles a los arrays,
        ' para que esto funcione automáticamente los nombres de los controles
        ' deberían tener el formato: nombre_numero
        '   nombre del control seguido de un guión bajo y el índice
        If elNombre = "" Then
            elNombre = mNombre
        End If
        ' si no se indica el nombre de los controles a añadir,
        ' lanzar una excepción
        If elNombre = "" Then
            Throw New ArgumentException("No se ha indicado el nombre base de los controles")
            Exit Sub
        End If
        '
        Me.Clear()
        asignarLosControles(ctrls, elNombre)
        Me.Reorganizar()
    End Sub
    Private Sub asignarLosControles(
                    ByVal ctrls As Control.ControlCollection,
                    ByVal elNombre As String)
        Dim ctr As Control
        '
        For Each ctr In ctrls
            ' Hacer una llamada recursiva por si este control "contiene" otros
            asignarLosControles(ctr.Controls, elNombre)
            '
            If ctr.Name.IndexOf(elNombre) > -1 Then
                Me.Add(ctr)
            End If
        Next
    End Sub
    '
    Public Function Contains(ByVal ctrl As Control) As Boolean
        List.Contains(ctrl)
    End Function
    '
    Public Function IndexOf(ByVal ctrl As Control) As Integer
        Return List.IndexOf(ctrl)
    End Function
    '
    Public Function Index(ByVal name As String) As Integer
        Dim ctrl As Control
        Dim i As Integer
        Dim hallado As Integer = -1
        '
        For i = 0 To List.Count - 1
            ctrl = CType(List(i), Control)
            If StrComp(ctrl.Name, name, CompareMethod.Text) = 0 Then
                hallado = i
                Exit For
            End If
        Next
        Return hallado
    End Function
    Public Function Index(ByVal ctrl As Control) As Integer
        Dim i As Integer
        '
        i = ctrl.Name.LastIndexOf("_")
        ' Si el nombre no tiene el signo _
        If i = -1 Then
            i = List.IndexOf(ctrl)
        Else
            i = CInt(ctrl.Name.Substring(i + 1))
        End If
        Return i
    End Function
    '
    Public Sub Insert(ByVal index As Integer, ByVal ctrl As Control)
        List.Insert(index, ctrl)
    End Sub
    '
    Default Public Property Item(ByVal index As Integer) As Control
        Get
            Return CType(List.Item(index), Control)
        End Get
        Set(ByVal Value As Control)
            List.Item(index) = Value
        End Set
    End Property
    Default Public Property Item(ByVal name As String) As Control
        Get
            Dim index As Integer = Me.Index(name)
            ' Si existe, devolverlo, sino, crear uno nuevo
            If index = -1 Then
                'index = Me.Add(ctrl)
            End If
            Return CType(List.Item(index), Control)
        End Get
        Set(ByVal Value As Control)
            Dim index As Integer = Me.Index(name)
            If index = -1 Then
                index = Me.Add(Value)
            End If
            List.Item(index) = Value
        End Set
    End Property
    Default Public Property Item(ByVal ctrl As Control) As Control
        Get
            Return CType(List(Me.IndexOf(ctrl)), Control)
        End Get
        Set(ByVal Value As Control)
            List(Me.IndexOf(ctrl)) = Value
        End Set
    End Property
    '
    Public Property Nombre() As String
        Get
            Return mNombre
        End Get
        Set(ByVal Value As String)
            mNombre = Value
        End Set
    End Property
    '
    Public Sub Remove(ByVal ctrl As Control)
        List.Remove(ctrl)
    End Sub
    '
    ' Reorganizar el contenido de la colección y ordenar por índice
    Public Sub Reorganizar()
        Dim ca As New ControlArray()
        Dim ctr As Control
        Dim i As Integer
        '
        For i = 0 To Me.Count - 1
            For Each ctr In Me
                If i = Me.Index(ctr) Then
                    ca.Add(ctr)
                    Exit For
                End If
            Next
        Next
        '
        Me.Clear()
        For Each ctr In ca
            Me.Add(ctr)
        Next
    End Sub
End Class
