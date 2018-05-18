Imports System.IO.Ports
Imports System.Threading

Public Class PuertoCom
    Private WithEvents PuertoSerial As New SerialPort
    'Private PuertoSerial As SerialPort = Nothing

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

    Public Sub Puertoserial_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles PuertoSerial.DataReceived
        Dim s As String = PuertoSerial.ReadExisting
        ProcesRxData(s)
    End Sub

    Public Sub ProcesRxData(ByVal d As String)
        'Esta sub es la llama el Thret que dispara el evento DataReceiver del puerto COM
        'Ver como tratar los datos aca.
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