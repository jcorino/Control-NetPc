Imports System.IO.Ports
Imports System.Threading

Public Class PuertoCom
    Public Structure InfoMotor
        Public NroMotor As Byte
        Public StatusByte As Byte
        Public StatusByte1 As Byte
        Public StatusByte2 As Byte
        Public StatusByte3 As Byte
        Public StatusByte4 As Byte
        Public ActualEncoder As UInt16
        Public TargetEncoder As UInt16
        Public LimiteSup As UInt16
        Public LimiteInf As UInt16
        Public Velocidad As Byte
    End Structure
    Public PlacasMotores(16) As InfoMotor
    Public BufferRecepcion As String
    Private ReadOnly BloqueoAcceso As New Object
    Private WithEvents PuertoSerial As New SerialPort

    Public Sub New(ByVal port As String,
                   ByVal baurate As Integer,
                   ByVal parity As Parity,
                   ByVal databit As Integer,
                   ByVal stopbit As StopBits)
        Try
            PuertoSerial = New SerialPort(port, baurate, parity, databit, stopbit)
            PuertoSerial.Open()
        Catch ex As Exception
            'Ver como responder si hay error
        End Try
    End Sub

    Public Sub Disponse()
        'Parece que para cerrar la clase es conveniente llamar a esta Sub porque sino no queda muy
        'claro cuando el sistema la da de baja de memoria y menos aun cuando se le antoja cerrar
        'el puerto serie, aun utilizando el metodo Finalize. Por lo menos asi me aseguro que lo cierre
        'y lo libere. Despues de llamar a esta Sub hay que eliminar la referencia al objeto
        '(Set instancia = Nothing). La famosa programacion orientada a objetos !!!(Objetos de mierda)
        Try
            PuertoSerial.Close()
        Catch ex As Exception
            'MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Puertoserial_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles PuertoSerial.DataReceived
        'Cada vez que sucede este evento .NET se dispara un Thread.
        'Antes de trabajar con el buffer bloque el acceso
        'para evitar que otro Thread acceda mientras lo estoy procesando
        'ya que tengo la sospecha que puede haber mas de un Thread.
        SyncLock BloqueoAcceso
            BufferRecepcion = PuertoSerial.ReadLine
            ProcesRxData(BufferRecepcion)
        End SyncLock

    End Sub

    Public Sub ProcesRxData(ByVal data As String)
        'Esta sub es la llama el Thread que dispara el evento DataReceiver del puerto COM

        Dim temp(10) As Byte
        Dim tempCrc As Byte

        'Check inicio de trama y largo---------------
        If data.Substring(0, 1) <> ":" Then Exit Sub
        If data.Length <> 22 Then Exit Sub
        '--------------------------------------------

        'Check CRC ----------------------------------
        For e As Byte = 0 To 9
            temp(e) = Convert.ToByte(data.Substring((2 * e) + 1, 2), 16)
            If e <= 8 Then
                tempCrc = tempCrc Xor temp(e)
            End If
        Next
        If tempCrc <> temp(9) Then Exit Sub
        '--------------------------------------------

        If temp(0) = 255 Then   'Si esta informando limites
            PlacasMotores(temp(7)).LimiteSup = ((temp(1) * 256) + temp(2))
            PlacasMotores(temp(7)).LimiteInf = ((temp(3) * 256) + temp(4))
        Else
            PlacasMotores(temp(7)).StatusByte4 = temp(0)
            PlacasMotores(temp(7)).StatusByte2 = temp(2)
            PlacasMotores(temp(7)).StatusByte3 = temp(1)
            PlacasMotores(temp(7)).ActualEncoder = ((temp(3) * 255) + temp(4))
        End If

        PlacasMotores(temp(7)).StatusByte1 = temp(5)
        PlacasMotores(temp(7)).StatusByte = temp(6)
        PlacasMotores(temp(7)).Velocidad = temp(7)
        PlacasMotores(temp(7)).NroMotor = temp(8)


    End Sub

    Public Sub EnviarSerie(ByVal Comando As Byte,
                                    ByVal NroMotor As Byte,
                                    ByVal Velocidad As Byte,
                                    ByVal EncoderLow As Byte,
                                    ByVal EncoderHi As Byte,
                                    ByVal SubComando As Byte,
                                    ByVal PKT7 As Byte)
        Try

            PuertoSerial.Write("TRANSMITIR cadena")

        Catch ex As Exception
            'Ver como capturar error si sucede
        End Try
    End Sub

End Class