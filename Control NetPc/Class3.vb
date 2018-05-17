Imports System.IO.Ports
Imports System.Threading

Public Class SerialCom

    Private serialPort As SerialPort = Nothing

    Public Sub New(ByVal port As String,
                   ByVal baurate As Integer,
                   ByVal parity As Parity,
                   ByVal databit As Integer,
                   ByVal stopbit As StopBits)
        Try
            serialPort = New SerialPort(port, baurate, parity, databit, stopbit)
            serialPort.Open()
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
            serialPort.Close()
        Catch ex As Exception
            'MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function CreateReadTxFrame(ByVal slaveAddress As Byte,
                                       ByVal functionCode As Byte,
                                       ByVal startAddress As UShort,
                                       ByVal numbersOfPoints As UShort) As Byte()

        Dim frame As Byte() = New Byte(7) {}
        Dim toCRC As Byte() = New Byte(5) {}

        frame(0) = slaveAddress
        frame(1) = functionCode
        frame(2) = CByte(Math.Truncate(startAddress / 256))
        frame(3) = CByte(startAddress - (frame(2) * 256))
        frame(4) = CByte(numbersOfPoints >> 8)
        frame(5) = CByte(numbersOfPoints)
        Array.Copy(frame, 0, toCRC, 0, 6)
        Dim crc As Byte() = ModCRC(toCRC)
        frame(6) = crc(0)
        frame(7) = crc(1)
        Return frame

    End Function

    Private Function CreateWriteTxFrame(ByVal slaveAddress As Byte,
                                        ByVal functionCode As Byte,
                                        ByVal Address As UShort,
                                        ByVal AddressVal As UShort) As Byte()

        Dim frame As Byte() = New Byte(7) {}
        Dim toCRC As Byte() = New Byte(5) {}

        frame(0) = slaveAddress
        frame(1) = functionCode
        frame(2) = CByte(Math.Truncate(Address / 256))
        frame(3) = CByte(Address - (frame(2) * 256))
        If functionCode = 6 Then
            frame(4) = CByte(Math.Truncate(AddressVal / 256))
            frame(5) = CByte(AddressVal - (frame(4) * 256))
        End If
        If functionCode = 5 Then
            frame(5) = 0
            frame(4) = CByte(AddressVal)
        End If

        Array.Copy(frame, 0, toCRC, 0, 6)
        Dim crc As Byte() = ModCRC(toCRC)
        frame(6) = crc(0)
        frame(7) = crc(1)
        Return frame

    End Function

    Public Function ReadFC1234_ModbusRTU(ByVal slaveAddress As Byte,
                                  ByVal FC As Byte,
                                  ByVal startAddress As UShort,
                                  ByVal numbersOfPoints As Byte,
                                  ByVal Optional rtuTimeOut As Int32 = 100) As Byte()

        'Funcion que ejecuta MODBUS Function 1, 2, 3 o 4 ( 1 Read Coil Status - 2 Read Input Status 
        '3 Read Holding Registers – 4 Read Input Registers)
        'Devuelve un Array de bytes con el estado de las bobinas o registros solicitados.
        'El primer byte del array(0) si esta en 1 es que la lectura fue ok y a continuacion estan los datos
        'Si el array(0) esta en 0 es que hubo un error que se expresa en array(1).

        'Errores posibles generados localmente:
        'array(1)= 252 Error de parametro de lectura FC <> 1,2,3 o 4.
        'array(1)= 253 timeout. No llego la respuesta.
        'array(1)= 254 error de CRC en recepcion.
        'array(1)= 255 Otro error no gestionado.

        'Errores posibles generados por el dispositivo remoto:
        'array(1)= 1 Función Ilegal
        'array(1)= 2 Dirección de datos no válido
        'array(1)= 3 Datos con valor no válido
        'array(1)= 4 Fallo en el dispositivo esclavo
        'array(1)= 5 Ack
        'array(1)= 6 Dispositivo esclavo ocupado
        'array(1)= 7 Nack
        'array(1)= 8 Error de paridad en memoria
        'array(1)= 10 Puerta de enlace Ruta No disponible
        'array(1)= 11 Dispositivo de puerta de enlace de destino no respondió

        Dim bufferRecei As Byte()
        Dim frame = CreateReadTxFrame(slaveAddress, FC, startAddress, numbersOfPoints)

        If (FC < 1) Or (FC > 4) Then              'Error de parametro Function lectura RTU.
            Return New Byte() {0, 252}
        End If

        serialPort.Write(frame, 0, frame.Length)
        Thread.Sleep(rtuTimeOut)

        If serialPort.BytesToRead > 4 Then
            bufferRecei = New Byte(serialPort.BytesToRead - 1) {}
            serialPort.Read(bufferRecei, 0, bufferRecei.Length)

            If bufferRecei(1) = (FC + &H80) Then    'Si respuesta tiene el bit mas significativo en 1 con lo cual hay un error
                'Devuelvo 0 para informar error
                'Devuelvo codigo de error
                Return New Byte() {0, bufferRecei(2)}
                Exit Function
            End If

            If Not CheckCRC(bufferRecei) Then       'Chequeo si esta ok CRC16 del paquete recibido
                'Devuelvo 0 para informar error
                'Devuelvo codigo de error
                Return New Byte() {0, 254}
                Exit Function
            End If

            Select Case FC
                Case 1, 2

                    Dim result As Byte() = New Byte(bufferRecei(2) * 8) {}
                    For i As Integer = 0 To (bufferRecei(2) - 1)
                        For j As Integer = 0 To 7
                            result(1 + ((i * 8) + j)) = (bufferRecei(3 + i) >> j) And 1
                        Next
                    Next
                    result(0) = 1               'Para informar que el resultado es correcto
                    Return result
                    Exit Function

                Case 3, 4

                    Dim result As Byte() = New Byte(bufferRecei(2)) {}
                    For i As Integer = 0 To (bufferRecei(2) - 1)
                        result(i + 1) = bufferRecei(3 + i)
                    Next
                    result(0) = 1               'Para informar que el resultado es correcto
                    Return result
                    Exit Function

            End Select

        Else

            'Error timeout. Se espero la respuesta y no llego
            'Devuelvo 0 para informar error
            'Devuelvo codigo de error
            Return New Byte() {0, 253}
            Exit Function

        End If

        Return New Byte() {0, 255}

    End Function

    Private Function ModCRC(ByVal Buffer() As Byte) As Byte()

        Dim CRCHigh As Byte
        Dim CRCLow As Byte
        Dim CRC1 As Long
        Dim I As Integer
        Dim J As Integer
        Dim K As Long

        CRC1 = &HFFFF
        For I = 0 To Buffer.Length - 1 ' Para cada byte
            CRC1 = CRC1 Xor Buffer(I)
            For J = 0 To 7 ' Para cada Bit dentro del Byte
                K = CRC1 And 1
                CRC1 = ((CRC1 And &HFFFE) / 2) And &H7FFF ' Shift right
                If K > 0 Then CRC1 = CRC1 Xor &HA001
            Next J
        Next I
        CRCHigh = CByte((CRC1 >> 8) And &HFF)
        CRCLow = CByte(CRC1 And &HFF)
        Return New Byte(1) {CRCLow, CRCHigh}

    End Function

    Private Function CheckCRC(ByVal Buffer() As Byte) As Boolean

        Dim CRCHigh As Byte
        Dim CRCLow As Byte
        Dim CRC1 As Long
        Dim I As Integer
        Dim J As Integer
        Dim K As Long

        CRC1 = &HFFFF
        For I = 0 To Buffer.Length - 3 ' Para cada byte
            CRC1 = CRC1 Xor Buffer(I)
            For J = 0 To 7 ' Para cada Bit dentro del Byte
                K = CRC1 And 1
                CRC1 = ((CRC1 And &HFFFE) / 2) And &H7FFF ' Shift right
                If K > 0 Then CRC1 = CRC1 Xor &HA001
            Next J
        Next I
        CRCHigh = CByte((CRC1 >> 8) And &HFF)
        CRCLow = CByte(CRC1 And &HFF)
        If CRCHigh = Buffer(Buffer.Length - 1) And CRCLow = Buffer(Buffer.Length - 2) Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function WriteFC5_ModbusRTU(ByVal slaveAddress As Byte,
                                    ByVal Address As UShort,
                                    ByVal AddresVal As Byte,
                                    ByVal Optional rtuTimeOut As Int32 = 100) As Byte()

        'Funcion que ejecuta MODBUS Function 5 ( 5-Preset Single Coil)
        'Devuelve un Array de bytes con el estado de las bobinas o registros solicitados.
        'El primer byte del array(0) si esta en 1 es que la escritura fue ok.
        'Si el array(0) esta en 0 es que hubo un error que se expresa en array(1).

        'Errores posibles generados localmente:
        'array(1)= 252 Error de parametro de lectura FC <> 6.
        'array(1)= 253 timeout. No llego la respuesta.
        'array(1)= 254 error de CRC en recepcion.
        'array(1)= 255 Otro error no gestionado.

        'Errores posibles generados por el dispositivo remoto:
        'array(1)= 1 Función Ilegal
        'array(1)= 2 Dirección de datos no válido
        'array(1)= 3 Datos con valor no válido
        'array(1)= 4 Fallo en el dispositivo esclavo
        'array(1)= 5 Ack
        'array(1)= 6 Dispositivo esclavo ocupado
        'array(1)= 7 Nack
        'array(1)= 8 Error de paridad en memoria
        'array(1)= 10 Puerta de enlace Ruta No disponible
        'array(1)= 11 Dispositivo de puerta de enlace de destino no respondió

        Dim bufferRecei As Byte()
        Dim frame = CreateWriteTxFrame(slaveAddress, 5, Address, AddresVal)

        serialPort.Write(frame, 0, frame.Length)
        Thread.Sleep(rtuTimeOut)

        If serialPort.BytesToRead > 4 Then
            bufferRecei = New Byte(serialPort.BytesToRead - 1) {}
            serialPort.Read(bufferRecei, 0, bufferRecei.Length)

            If bufferRecei(1) = (5 + &H80) Then    'Si respuesta tiene el bit mas significativo en 1 con lo cual hay un error
                'Devuelvo 0 para informar error
                'Devuelvo codigo de error
                Return New Byte() {0, bufferRecei(2)}
                Exit Function
            End If

            If Not CheckCRC(bufferRecei) Then       'Chequeo si esta ok CRC16 del paquete recibido
                'Devuelvo 0 para informar error
                'Devuelvo codigo de error
                Return New Byte() {0, 254}
                Exit Function
            End If

            Dim result As Byte() = New Byte(bufferRecei(2) * 8) {}
            For i As Integer = 0 To (bufferRecei(2) - 1)
                For j As Integer = 0 To 7
                    result(1 + ((i * 8) + j)) = (bufferRecei(3 + i) >> j) And 1
                Next
            Next
            result(0) = 1               'Para informar que el resultado es correcto
            Return result
            Exit Function

        Else

            'Error timeout. Se espero la respuesta y no llego
            'Devuelvo 0 para informar error
            'Devuelvo codigo de error
            Return New Byte() {0, 253}
            Exit Function

        End If

        Return New Byte() {0, 255}

    End Function

    Public Function WriteFC6_ModbusRTU(ByVal slaveAddress As Byte,
                                   ByVal Address As UShort,
                                   ByVal AddresVal As UShort,
                                   ByVal Optional rtuTimeOut As Int32 = 100) As Byte()

        'Funcion que ejecuta MODBUS Function 6 ( 6-Preset Single Register)
        'Devuelve un Array de bytes con el estado de las bobinas o registros solicitados.
        'El primer byte del array(0) si esta en 1 es que la escritura fue ok.
        'Si el array(0) esta en 0 es que hubo un error que se expresa en array(1).

        'Errores posibles generados localmente:
        'array(1)= 252 Error de parametro de lectura FC <> 6.
        'array(1)= 253 timeout. No llego la respuesta.
        'array(1)= 254 error de CRC en recepcion.
        'array(1)= 255 Otro error no gestionado.

        'Errores posibles generados por el dispositivo remoto:
        'array(1)= 1 Función Ilegal
        'array(1)= 2 Dirección de datos no válido
        'array(1)= 3 Datos con valor no válido
        'array(1)= 4 Fallo en el dispositivo esclavo
        'array(1)= 5 Ack
        'array(1)= 6 Dispositivo esclavo ocupado
        'array(1)= 7 Nack
        'array(1)= 8 Error de paridad en memoria
        'array(1)= 10 Puerta de enlace Ruta No disponible
        'array(1)= 11 Dispositivo de puerta de enlace de destino no respondió

        Dim bufferRecei As Byte()
        Dim frame = CreateWriteTxFrame(slaveAddress, 6, Address, AddresVal)

        serialPort.Write(frame, 0, frame.Length)
        Thread.Sleep(rtuTimeOut)

        If serialPort.BytesToRead > 4 Then
            bufferRecei = New Byte(serialPort.BytesToRead - 1) {}
            serialPort.Read(bufferRecei, 0, bufferRecei.Length)

            If bufferRecei(1) = (6 + &H80) Then    'Si respuesta tiene el bit mas significativo en 1 con lo cual hay un error
                'Devuelvo 0 para informar error
                'Devuelvo codigo de error
                Return New Byte() {0, bufferRecei(2)}
                Exit Function
            End If

            If Not CheckCRC(bufferRecei) Then       'Chequeo si esta ok CRC16 del paquete recibido
                'Devuelvo 0 para informar error
                'Devuelvo codigo de error
                Return New Byte() {0, 254}
                Exit Function
            End If

            Dim result As Byte() = New Byte(bufferRecei(2)) {}
            For i As Integer = 0 To (bufferRecei(2) - 1)
                result(i + 1) = bufferRecei(3 + i)
            Next
            result(0) = 1               'Para informar que el resultado es correcto
            Return result
            Exit Function


        Else

            'Error timeout. Se espero la respuesta y no llego
            'Devuelvo 0 para informar error
            'Devuelvo codigo de error
            Return New Byte() {0, 253}
            Exit Function

        End If

        Return New Byte() {0, 255}

    End Function

    Public Function WriteFC15_ModbusRTU(ByVal slaveAddress As Byte,
                                   ByVal Address As UShort,
                                   ByVal WriteQty As UShort,
                                   ByVal Values As Byte(),
                                   ByVal Optional rtuTimeOut As Int32 = 100) As Byte()

        'Funcion que ejecuta MODBUS Function 15 ( 15-Write multiple Coil)
        'El primer byte del array(0) si esta en 1 es que la escritura fue ok.
        'Si el array(0) esta en 0 es que hubo un error que se expresa en array(1).

        'Errores posibles generados localmente:
        'array(1)= 252 Error de parametro de lectura FC <> 6.
        'array(1)= 253 timeout. No llego la respuesta.
        'array(1)= 254 error de CRC en recepcion.
        'array(1)= 255 Otro error no gestionado.

        'Errores posibles generados por el dispositivo remoto:
        'array(1)= 1 Función Ilegal
        'array(1)= 2 Dirección  de datos no válido
        'array(1)= 3 Datos con valor no válido
        'array(1)= 4 Fallo en el dispositivo esclavo
        'array(1)= 5 Ack
        'array(1)= 6 Dispositivo esclavo ocupado
        'array(1)= 7 Nack
        'array(1)= 8 Error de paridad en memoria
        'array(1)= 10 Puerta de enlace Ruta No disponible
        'array(1)= 11 Dispositivo de puerta de enlace de destino no respondió

        Dim bufferRecei As Byte()
        Dim frame As Byte() = New Byte(8 + Values.Length) {}
        Dim i As Int16
        Dim result As Byte() = New Byte(1) {}

        frame(0) = slaveAddress
        frame(1) = 15
        frame(2) = CByte(Address >> 8)
        frame(3) = CByte(Address)
        frame(4) = CByte(WriteQty >> 8)
        frame(5) = CByte(WriteQty)
        frame(6) = Values.Length
        For i = 0 To (Values.Length - 1)
            frame(7 + i) = Values(i)
        Next

        Dim toCRC As Byte() = New Byte(7 + (Values.Length - 1)) {}
        Array.Copy(frame, 0, toCRC, 0, frame.Length - 2)
        Dim crc As Byte() = ModCRC(toCRC)
        frame(7 + Values.Length) = crc(0)
        frame(8 + Values.Length) = crc(1)

        serialPort.Write(frame, 0, frame.Length)
        Thread.Sleep(rtuTimeOut)

        If serialPort.BytesToRead > 4 Then
            bufferRecei = New Byte(serialPort.BytesToRead - 1) {}
            serialPort.Read(bufferRecei, 0, bufferRecei.Length)

            If bufferRecei(1) = (15 + &H80) Then    'Si respuesta tiene el bit mas significativo en 1 con lo cual hay un error
                'Devuelvo 0 para informar error
                'Devuelvo codigo de error
                Return New Byte() {0, bufferRecei(2)}
                Exit Function
            End If

            If Not CheckCRC(bufferRecei) Then       'Chequeo si esta ok CRC16 del paquete recibido
                'Devuelvo 0 para informar error
                'Devuelvo codigo de error
                Return New Byte() {0, 254}
                Exit Function
            End If

            result(0) = 1               'Para informar que el resultado es correcto
            Return result
            Exit Function

        Else

            'Error timeout. Se espero la respuesta y no llego
            'Devuelvo 0 para informar error
            'Devuelvo codigo de error
            Return New Byte() {0, 253}
            Exit Function

        End If

        Return New Byte() {0, 255}

    End Function

    Public Function WriteFC16_ModbusRTU(ByVal slaveAddress As Byte,
                                   ByVal Address As UShort,
                                   ByVal WriteQty As UShort,
                                   ByVal Values As Byte(),
                                   ByVal Optional rtuTimeOut As Int32 = 100) As Byte()

        'Funcion que ejecuta MODBUS Function 16 ( 16-Write multiple Register)
        'El primer byte del array(0) si esta en 1 es que la escritura fue ok.
        'Si el array(0) esta en 0 es que hubo un error que se expresa en array(1).

        'Errores posibles generados localmente:
        'array(1)= 252 Error de parametro de lectura FC <> 6.
        'array(1)= 253 timeout. No llego la respuesta.
        'array(1)= 254 error de CRC en recepcion.
        'array(1)= 255 Otro error no gestionado.

        'Errores posibles generados por el dispositivo remoto:
        'array(1)= 1 Función Ilegal
        'array(1)= 2 Dirección  de datos no válido
        'array(1)= 3 Datos con valor no válido
        'array(1)= 4 Fallo en el dispositivo esclavo
        'array(1)= 5 Ack
        'array(1)= 6 Dispositivo esclavo ocupado
        'array(1)= 7 Nack
        'array(1)= 8 Error de paridad en memoria
        'array(1)= 10 Puerta de enlace Ruta No disponible
        'array(1)= 11 Dispositivo de puerta de enlace de destino no respondió

        Dim bufferRecei As Byte()
        Dim frame As Byte() = New Byte(8 + Values.Length) {}
        Dim i As Int16
        Dim result As Byte() = New Byte(1) {}

        frame(0) = slaveAddress
        frame(1) = 16
        frame(2) = CByte(Address >> 8)
        frame(3) = CByte(Address)
        frame(4) = CByte(CUShort(Values.Length / 2) >> 8)
        frame(5) = CByte(CUShort(Values.Length / 2))
        frame(6) = Values.Length
        For i = 0 To (Values.Length - 1)
            frame(7 + i) = Values(i)
        Next

        Dim toCRC As Byte() = New Byte(7 + (Values.Length - 1)) {}
        Array.Copy(frame, 0, toCRC, 0, frame.Length - 2)
        Dim crc As Byte() = ModCRC(toCRC)
        frame(7 + Values.Length) = crc(0)
        frame(8 + Values.Length) = crc(1)

        serialPort.Write(frame, 0, frame.Length)
        Thread.Sleep(rtuTimeOut)

        If serialPort.BytesToRead > 4 Then
            bufferRecei = New Byte(serialPort.BytesToRead - 1) {}
            serialPort.Read(bufferRecei, 0, bufferRecei.Length)

            If bufferRecei(1) = (16 + &H80) Then    'Si respuesta tiene el bit mas significativo en 1 con lo cual hay un error
                'Devuelvo 0 para informar error
                'Devuelvo codigo de error
                Return New Byte() {0, bufferRecei(2)}
                Exit Function
            End If

            If Not CheckCRC(bufferRecei) Then       'Chequeo si esta ok CRC16 del paquete recibido
                'Devuelvo 0 para informar error
                'Devuelvo codigo de error
                Return New Byte() {0, 254}
                Exit Function
            End If

            result(0) = 1               'Para informar que el resultado es correcto
            Return result
            Exit Function

        Else

            'Error timeout. Se espero la respuesta y no llego
            'Devuelvo 0 para informar error
            'Devuelvo codigo de error
            Return New Byte() {0, 253}
            Exit Function

        End If

        Return New Byte() {0, 255}

    End Function

End Class