﻿Imports System.IO.Ports
Imports System.Threading

Public Class PuertoCom

    Public myPoolThread As New Threading.Thread(AddressOf SendSERIAL)

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
    Public CantidadMotores As Byte
    Public BufferTXplaca As New List(Of List(Of String))
    Public PlacasMotores() As InfoMotor
    Public BufferRecepcion As String
    Public BufferTransmision As New List(Of String)
    Private ReadOnly BloqueoAcceso As New Object
    Private WithEvents mySerialPort As New SerialPort

    Public Sub New()

        'Creo un arraylist con la cantidad de motores maxima disponible.
        'Esto es un List de list. Es como un Array de 2 dimensiones
        'pero con las propiedades de lista.
        For i As Byte = 0 To 15
            BufferTXplaca.Add(New List(Of String))
        Next

        'Defino la cantidad de placas que reciben y transmiten info remota
        'Redimensiono array a esa cantidad

        CantidadMotores = 12
        ReDim PlacasMotores(CantidadMotores)

    End Sub
    Public Sub InitSerial(ByVal port As String,
                   ByVal baurate As Integer,
                   ByVal parity As Parity,
                   ByVal databit As Integer,
                   ByVal stopbit As StopBits)
        Try
            mySerialPort = New SerialPort(port, baurate, parity, databit, stopbit)
            mySerialPort.Open()
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
            mySerialPort.Close()
        Catch ex As Exception
            'MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub mySerialPort_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles mySerialPort.DataReceived
        'Cada vez que sucede este evento .NET se dispara un Thread.
        'Antes de trabajar con el buffer bloque el acceso
        'para evitar que otro Thread acceda mientras lo estoy procesando
        'ya que tengo la sospecha que puede haber mas de un Thread.
        'SyncLock BloqueoAcceso
        BufferRecepcion = mySerialPort.ReadLine
        ProcesRxData(BufferRecepcion)
        'End SyncLock

    End Sub

    Private Sub ProcesRxData(ByVal data As String)
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
            PlacasMotores(temp(8)).LimiteSup = ((temp(1) * 256) + temp(2))
            PlacasMotores(temp(8)).LimiteInf = ((temp(3) * 256) + temp(4))
        Else
            PlacasMotores(temp(8)).StatusByte4 = temp(0)
            PlacasMotores(temp(8)).StatusByte2 = temp(2)
            PlacasMotores(temp(8)).StatusByte3 = temp(1)
            PlacasMotores(temp(8)).ActualEncoder = ((temp(3) * 256) + temp(4))
        End If

        PlacasMotores(temp(8)).StatusByte1 = temp(5)
        PlacasMotores(temp(8)).StatusByte = temp(6)
        PlacasMotores(temp(8)).Velocidad = temp(7)
        PlacasMotores(temp(8)).NroMotor = temp(8)


    End Sub

    Public Sub EnviarSerieSimple(ByRef enviar As String)
        Try
            mySerialPort.Write(enviar)
        Catch ex As Exception
            'Ver como capturar error si sucede
        End Try
    End Sub

    Private Sub SendSERIAL()
        'Esta sub se ejecuta en un Thread distinto.

        Dim CantidadPosBuffer As Byte
        Dim tempPrioridad As Byte
        Dim indicePrioridad As Byte

        While 1


            'Bloqueo el acceso de otros Thread al BufferTXplaca para evitar
            'que lo puedan modificar mientras lo estoy cargando


            For i As Byte = 0 To CantidadMotores - 1
                SyncLock BloqueoAcceso
                    If BufferTXplaca(i).Count > 0 Then                              'El motor tiene datos en BufferTx ?
                        CantidadPosBuffer = BufferTXplaca(i).Count / 4

                        For j As Byte = 0 To CantidadPosBuffer - 1
                            If tempPrioridad < BufferTXplaca(i)((j * 4) + 3) Then   'Busco el numero mas bajo de prioridad
                                tempPrioridad = BufferTXplaca(i)((j * 4) + 3)       'que corresponde a la maxima prioridad
                                indicePrioridad = (j * 4)
                            End If
                        Next
                        mySerialPort.Write(BufferTXplaca(i)(indicePrioridad))
                            Debug.Print(BufferTXplaca(i)(indicePrioridad) & "  " & (BufferTXplaca(i)(indicePrioridad + 1)) & "  " & (BufferTXplaca(i)(indicePrioridad + 2)) & "  " & (BufferTXplaca(i)(indicePrioridad + 3)))

                            BufferTXplaca(i).RemoveAt(indicePrioridad)                      'Trama
                            BufferTXplaca(i).RemoveAt(indicePrioridad)
                            BufferTXplaca(i).RemoveAt(indicePrioridad)
                            BufferTXplaca(i).RemoveAt(indicePrioridad)
                            ' Debug.Print(BufferTXplaca(i)(indicePrioridad))


                            Else

                        mySerialPort.Write("@" + CStr(i) + "F")                 'Transmito pedido reporte generico
                        Debug.Print("@" + CStr(i) + "F")

                    End If
                End SyncLock
                Thread.Sleep(500)
            Next



        End While

    End Sub

    Public Sub PoolPlacas(ByVal OnOff As Boolean)
        If OnOff Then
            If myPoolThread.ThreadState = Threading.ThreadState.Unstarted Or myPoolThread.ThreadState = Threading.ThreadState.Aborted Then
                myPoolThread = New Threading.Thread(AddressOf SendSERIAL)
                myPoolThread.Start()
            Else
                MsgBox("Instancia de Transmision ya iniciada")
            End If
        Else
            myPoolThread.Abort() 'Abort Thread
        End If
    End Sub

    Public Sub EnviaToBufferTX(ByVal datos As String, ByVal nroMotor As Byte, ByVal prioridad As Byte)
        Dim CantidadPosBuffer As Byte
        Dim tempRespuesta As Byte

        'Bloqueo el acceso de otros Thread al BufferTXplaca para evitar
        'que lo puedan modificar mientras lo estoy cargando
        SyncLock BloqueoAcceso
            'Las datos a guardar en el Buffer seran
            'La trama (que llega en datos) + Nro de respuesta 
            '+ cantidad de retransmisiones sin respuesta + Prioridad
            'Cada dato en un nivel de la lista. Es decir consume 4 niveles por datos
            'por cada nive de buffer por motor.
            '
            'Ej:
            'BufferTXplaca(motorX).Add("@1F")
            'Trama = "@1F" ----> Trama a enviar con la que llaman a esta rutina.

            'BufferTXplaca(motorX).Add("1")
            'Respuesta = 1 ----> Valor que se envia a placa para que responda con ese mismo y 
            '                   asegurarme que lo recibio. La recepcion la chequea rutina de rx.

            'BufferTXplaca(motorX).Add("0")
            'Cantidad Retrasmisiones = 0 ------> Aca se carga en 0 y rutina de TX se encarga de sumarla.

            'Prioridad = 3 ------> Prioridad con que se quiere enviar esta trama 1 mas alta, 5 mas baja
            'BufferTXplaca(motorX).Add("3")
            '
            'Esto se repetiria por cada nivel de stack.
            '
            'Primero chequeo en la lista que no tenga tramas en el Buffer para ese motor
            'y eventualmente conocer el indice para poder leer que numero correlativo
            'corresponderia de Nro de respuesta.

            If BufferTXplaca(nroMotor).Count > 0 Then                               'El motor tiene datos en BufferTx ?
                CantidadPosBuffer = BufferTXplaca(nroMotor).Count / 4
                For j As Byte = 0 To CantidadPosBuffer - 1
                    If tempRespuesta < BufferTXplaca(nroMotor)((j * 4) + 1) Then    'Busco el numero mas alto de NroRespuesta que ya este en el buffer
                        tempRespuesta = BufferTXplaca(nroMotor)((j * 4) + 1)
                    End If
                Next
                BufferTXplaca(nroMotor).Add(datos)                      'Trama
                BufferTXplaca(nroMotor).Add(CStr(tempRespuesta + 1))    'Nro Respuesta Esperado
                BufferTXplaca(nroMotor).Add("0")                        'Cantidad Retrasmisiones = 0
                BufferTXplaca(nroMotor).Add(prioridad)                  'Prioridad

            Else
                BufferTXplaca(nroMotor).Add(datos)      'Trama
                BufferTXplaca(nroMotor).Add("1")        'Nro Respuesta Esperado
                BufferTXplaca(nroMotor).Add("0")        'Cantidad Retrasmisiones = 0
                BufferTXplaca(nroMotor).Add(prioridad)  'Prioridad
            End If

        End SyncLock

    End Sub

End Class
