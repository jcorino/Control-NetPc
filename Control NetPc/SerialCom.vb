﻿Imports System.IO.Ports
Imports System.Threading

Public Class NodeComunication

    Const bd_Conexion As String = "Data Source=data.db; Version=3;"
    Private db_objCon As SQLite.SQLiteConnection
    Private db_objComand As SQLite.SQLiteCommand


    Public myPoolThread As New Threading.Thread(AddressOf SendSERIAL)

    '(port, baurate, parity, databit, stopbit)
    Public Property ComPort As String = "COM5"
    Public Property ComBaurate As Integer = 115200
    Public Property ComParity As Parity = Parity.None
    Public Property ComDatabit As Integer = 8
    Public Property ComStopbit As StopBits = StopBits.One

    'Tiempo en ms entre pedido de reporte motores
    Public Property PollTime As Integer = 20

    'Si utilizo envio y recepcion de chequeo packetes
    'Se deberia utilizar pero puede haber situaciones que 
    'requieran no utilizarlo.
    Public Property UseCheckPacket As Boolean = True

    'Deshabilita el poolling automatico a las placas
    'Esto puede ser util para pasar a un modo programacion
    'donde no se requiere un monitoreo online de los encoders
    'pero si se siguen enviando el resto de las tramas.
    'De requerirlo se puede pedir reporte a placas individuales
    'con el comando cReporte al numero de placa que sea.
    'No es tan veloz quizas ya que le suma confirmacion de trama
    Public Property HabilitarPoollingAutomatico As Boolean = True

    Public Structure InfoMotor
        Public NroMotor As Byte
        Public ConfirmByte As Byte
        Public StatusByte As Byte
        Public StatusByte2 As Byte
        Public StatusByte3 As Byte
        Public StatusByte4 As Byte
        Public ActualEncoder As UInt16
        Public TargetEncoder As UInt16
        Public LimiteSup As UInt16
        Public LimiteInf As UInt16
        Public Velocidad As Byte
        Public Nombre As String
        Public Enable As Boolean
        Public CmPulse As UInt16
        Public IsInPause As Boolean
        Public IsRepro As Boolean
        Public IsUp As Boolean
        Public IsDown As Boolean
        Public IsSuperoLimSup As Boolean
        Public IsSuperoLimInf As Boolean
    End Structure

    Public Enum ComandoMotor As Byte

        ' "p"       Reset equipo - Utilizada en PICs con reset por soft (18F)
        ' "g"       Transmitir a soft PC limites, posicion, etc, etc - Es la que utilizo para reportar
        ' "a"       Comando Subir
        ' "z"       Comando Bajar
        ' "x"       Comando Stop
        ' "j"       Detener GoAutomatic DEBO UTILIZARLA - IMPLEMENTAR CODIGO
        ' "k"       Comando ir a automaticamente
        ' "s"       Comando actualizar final de carrera minEncoder
        ' "d"       Comando actualizar final de carrera maxEncoder

        ' "r"       Comando Reproducir Rutina grabada en EEPROM - NO UTILIZADA
        ' "b"       Comenzar a Grabar Datos PC a SD - NO UTILIZADA
        ' "c"       Comenzar a Grabar de PC a SD - NO UTILIZADA	
        ' "r"       Comando Reproducir Rutina grabada en EEPROM - NO UTILIZADA
        ' "l"       Comando Comenzar Grabacion Rutina en EEPROM   - NO UTILIZADA
        ' "q"       Comando Enviar Rutina a PC - NO UTILIZADA
        ' "n"       Comando Finalizar Grabacion o Reproduccion Rutina en EEPROM - NO UTILIZADA

        cReset = 1
        cReporte = 2
        cSubir = 3
        cBajar = 4
        cStop = 5
        cStopGoAutomatic = 6
        cGoAutomatic = 7
        cActualizarLimites = 8
    End Enum

    Private BufferTX As New List(Of List(Of String))
    Private BufferRX As String
    Public NodeStatus() As InfoMotor
    Private ReadOnly BloqueoAcceso As New Object
    Private WithEvents MySerialPort As New SerialPort
    Private ActivarCom As Boolean = True
    Private ReadOnly CantidadMotores As Byte = 12

    Public Sub New(ByVal QTyMotores As Byte)


        CantidadMotores = QTyMotores

        'Redimensiono array a cantidad de placas remotas a 
        'consultar y/o comandar
        ReDim NodeStatus(QTyMotores)

        'Creo un arraylist con la cantidad de motores 
        'maxima disponible. Esto es un List de list. 
        'Es como un Array de 2 dimensiones pero con 
        'las propiedades de lista.
        For i As Byte = 0 To QTyMotores
            BufferTX.Add(New List(Of String))
        Next



    End Sub

    Public Sub InitSerial()

        Try
            ActivarComunicacion = False
            If MySerialPort.IsOpen Then
                MySerialPort.Close()
            End If
            MySerialPort.PortName = ComPort
            MySerialPort.BaudRate = ComBaurate
            MySerialPort.Parity = ComParity
            MySerialPort.DataBits = ComDatabit
            MySerialPort.StopBits = ComStopbit

            MySerialPort.Open()
            ActivarComunicacion = True
        Catch ex As Exception

        End Try

    End Sub

    Public Sub Disponse()
        'Parece que para cerrar la clase es conveniente 
        'llamar a esta Sub porque sino no queda muy claro 
        'cuando el sistema la da de baja de memoria y menos 
        'aun cuando se le antoja cerrar el puerto serie, aun 
        'utilizando el metodo Finalize. Por lo menos asi me 
        'aseguro que lo cierre y lo libere. Despues de llamar 
        'a esta Sub hay que eliminar la referencia al objeto
        '(Set instancia = Nothing). La famosa programacion 
        'orientada a objetos !!!(Objetos de mierda)
        Try
            If MySerialPort.IsOpen Then
                MySerialPort.Close()
            End If
        Catch ex As Exception
            'MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub MySerialPort_DataReceived(ByVal sender As Object,
                                          ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles MySerialPort.DataReceived
        'Cada vez que sucede este evento .NET dispara un Thread.
        BufferRX = MySerialPort.ReadLine
        ProcesRxData(BufferRX)
    End Sub

    Private Sub ProcesRxData(ByVal data As String)
        'Esta sub es la que llama el Thread que dispara el 
        'evento DataReceiver del puerto COM. Se encarga de 
        'procesar los datos que llegan por el puerto serial
        'y cargarlos en PlacasMotores.

        Dim temp(10) As Byte
        Dim tempCrc As Byte
        Dim CantidadPosBuffer As Byte
        Dim indiceRespuesta As Byte

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

        With NodeStatus(temp(8))

            If temp(0) = 255 Then   'Si esta informando limites
                .LimiteSup = ((temp(1) * 256) + temp(2))
                .LimiteInf = ((temp(3) * 256) + temp(4))
            Else
                .StatusByte4 = temp(0)
                .StatusByte2 = temp(2)
                .StatusByte3 = temp(1)
                .ActualEncoder = ((temp(3) * 256) + temp(4))
            End If

            .ConfirmByte = temp(5)
            .StatusByte = temp(6)
            .Velocidad = temp(7)
            .NroMotor = temp(8)

            .IsInPause = CBool(temp(6) And 1)
            .IsUp = CBool((temp(6) >> 1) And 1)
            .IsDown = CBool((temp(6) >> 2) And 1)
            .IsRepro = CBool((temp(6) >> 3) And 1)
            .IsSuperoLimSup = CBool((temp(6) >> 4) And 1)
            .IsSuperoLimInf = CBool((temp(6) >> 5) And 1)

        End With

        'Busco en el BufferTX a que posicion corresponde el ConfirmByte
        'para eliminar ese nivel de BufferTX ya que se recibio la confirmacion
        SyncLock BloqueoAcceso

            With BufferTX(temp(8))

                If .Count > 0 Then                              'El motor tiene datos en BufferTx ?
                    CantidadPosBuffer = CByte(.Count) / 6       'Determino cuantos niveles

                    For j As Byte = 0 To CantidadPosBuffer - 1
                        'Busco en que indice esta el ConfirmByte para eliminar esa entrada del BufferTX
                        'y chequeo que no sea una trama marcada como repetir. Es asi no debo eliminarla
                        'aun cuando reciba correctamente el ConfirmByte.
                        If (NodeStatus(temp(8)).ConfirmByte = BufferTX(temp(8))((j * 6) + 1)) And (BufferTX(temp(8))((j * 6) + 5) = "0") Then
                            indiceRespuesta = (j * 6)
                            .RemoveAt(indiceRespuesta)  'Elimino los 6 registros
                            .RemoveAt(indiceRespuesta)  'del nivel de bufferTX recibido ok
                            .RemoveAt(indiceRespuesta)
                            .RemoveAt(indiceRespuesta)
                            .RemoveAt(indiceRespuesta)
                            .RemoveAt(indiceRespuesta)
                            Exit For
                        End If

                    Next

                End If

            End With

        End SyncLock

    End Sub

    Public Sub EnviarSerieSimple(ByVal enviar As String)
        'Lo que recibe lo escribe en el puerto serial.

        Try
            If ActivarCom Then     'Si esta activa la propiedad ActivarComunicion
                MySerialPort.Write(enviar)
            End If
        Catch ex As Exception
            'Ver como capturar error si sucede
        End Try

    End Sub

    Private Sub SendSERIAL()
        'Esta sub se ejecuta en un Thread distinto. Es la 
        'encargada de recorrer el BufferTX y escribir el 
        'puerto Serial en forma correlativa por motor dando 
        'prioridad a las tramas. Lo hace cada PoollTime

        Dim CantidadPosBuffer As Byte
        Dim tempPrioridad As Byte
        Dim indicePrioridad As Byte
        Dim byteTrama As Byte()

        'Genero un array para armar las tramas de pedido reporte
        'generico. Los campos son solo como Dummy solo importa el
        'numero de motor que lo modifico en el momento de utilizar
        'este array y el byte Accion que sera 02 (Reportar)
        ReDim byteTrama(10)
        byteTrama(0) = 64   '"@" Inicio de trama
        byteTrama(1) = 1    'Numero de motor
        byteTrama(2) = 2    'Accion
        byteTrama(3) = 0    'Posicion MSB
        byteTrama(4) = 0    'Posicion LSB
        byteTrama(5) = 0    'TargetPos MSB
        byteTrama(6) = 0    'TargetPos LSB
        byteTrama(7) = 0    'Velocidad
        byteTrama(8) = 0    'Numero confirmacion

        While 1
            For i As Byte = 0 To CantidadMotores - 1

                'Bloqueo el acceso de otros Thread al BufferTXplaca 
                'para evitar que lo puedan modificar mientras lo 
                'estoy manipulando
                SyncLock BloqueoAcceso

                    If BufferTX(i).Count > 0 Then                      'El motor tiene datos en BufferTx ?
                        CantidadPosBuffer = BufferTX(i).Count / 6      'Determino cuantos niveles
                        tempPrioridad = 255

                        'Si la trama requiere respuesta
                        For j As Byte = 0 To CantidadPosBuffer - 1          'Sumo 1 al acumulador de retries.
                            If BufferTX(i)((j * 6) + 4) = "1" Then
                                If BufferTX(i)((j * 6) + 2) = 255 Then 'Para que no pase de contar 255 Retries.
                                    BufferTX(i)((j * 6) + 2) = 254
                                End If
                                BufferTX(i)((j * 6) + 2) = BufferTX(i)((j * 6) + 2) + 1
                            End If
                        Next


                        For j As Byte = 0 To CantidadPosBuffer - 1

                            If tempPrioridad > BufferTX(i)((j * 6) + 3) Then   'Busco el numero mas bajo de prioridad
                                tempPrioridad = BufferTX(i)((j * 6) + 3)       'que corresponde a la maxima prioridad
                                indicePrioridad = (j * 6)
                            End If

                        Next

                        tempPrioridad = 255
                        If ActivarCom Then 'Si esta activa la propiedad ActivarComunicacion
                            Try
                                MySerialPort.Write(BufferTX(i)(indicePrioridad))
                                Debug.Print(BufferTX(i)(indicePrioridad) & "  " & (BufferTX(i)(indicePrioridad + 1)) & "  " & (BufferTX(i)(indicePrioridad + 2)) & "  " & (BufferTX(i)(indicePrioridad + 3)))

                            Catch es As Exception

                            End Try
                        End If
                        'Si esta ultima trama tiene atributo repeat tengo que ver si
                        'hay mas tramas despues de ella. Si hay la tengo que eliminar
                        'aun que sea repeat. Si no hay mas tramas en el buffer si la
                        'dejo como repeat. Si la trama no era repeat y no es una trama
                        'con pedido de confirmacion debo eliminarla ya que significa
                        'que solo se transmite 1 vez.
                        If (BufferTX(i)(indicePrioridad + 5)) = "1" Then   'Tiene atributo repeat?
                            If CantidadPosBuffer > 1 Then                       'Hay mas tramas que ella en el buffer?
                                BufferTX(i).RemoveAt(indicePrioridad)      'Elimino los 6 registros
                                BufferTX(i).RemoveAt(indicePrioridad)      'del nivel de bufferTX recibido ok
                                BufferTX(i).RemoveAt(indicePrioridad)
                                BufferTX(i).RemoveAt(indicePrioridad)
                                BufferTX(i).RemoveAt(indicePrioridad)
                                BufferTX(i).RemoveAt(indicePrioridad)
                            End If
                        Else
                            If (BufferTX(i)(indicePrioridad + 4)) = "0" Then   'Trama sin pedido de confirmacion
                                BufferTX(i).RemoveAt(indicePrioridad)          'Elimino los 6 registros
                                BufferTX(i).RemoveAt(indicePrioridad)          'del nivel de bufferTX recibido ok
                                BufferTX(i).RemoveAt(indicePrioridad)
                                BufferTX(i).RemoveAt(indicePrioridad)
                                BufferTX(i).RemoveAt(indicePrioridad)
                                BufferTX(i).RemoveAt(indicePrioridad)
                            End If
                        End If

                    Else

                        If HabilitarPoollingAutomatico Then
                            byteTrama(1) = i        'Numero de motor
                            If ActivarCom Then      'Si esta activa la propiedad ActivarComunicion
                                Try
                                    MySerialPort.Write(GenerarTramaYcRc(byteTrama))         'Transmito pedido reporte generico
                                    Debug.Print(GenerarTramaYcRc(byteTrama))

                                Catch es As Exception

                                End Try

                            End If
                        End If

                    End If

                End SyncLock

                Thread.Sleep(PollTime)

            Next

        End While

    End Sub

    Public Sub PoolPlacas(ByVal OnOff As Boolean)
        'Sub que es llamada para comenzar con el pooleo de placas motores
        'ejecutando SendSERIAL en un Thread
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

    Public Sub AccionesMotores(ByVal Action As ComandoMotor,
                               ByVal numMotor As Byte,
                               ByVal Posicion As UInt16,
                               ByVal Optional Velocidad As Byte = 1,
                               ByVal Optional TargetPosicion As UInt16 = 0,
                               ByVal Optional prioridad As Byte = 1,
                               ByVal Optional Repeat As Boolean = 0)

        'Sub que se encarga de recibir los pedidos de acciones
        'y los coloca en el BufferTX. Antes de generar la trama
        'chequea el numero esperado de respuesta de ser necesario
        Dim sql As String
        Dim CantidadPosBuffer As Byte
        Dim tempRespuesta As Byte
        Dim byteTrama As Byte()

        'Si el motor esta desactivado para utilizarce salgo de la rutina
        If NodeStatus(numMotor).Enable = False Then
            Exit Sub
        End If

        'Los posibles valores que puedo recibir en Accion son los
        'determinados en el enum ComandoMotor:
        'cReset = 1
        'cReporte = 2
        'cSubir = 3
        'cBajar = 4
        'cStop = 5
        'cStopGoAutomatic = 6
        'cGoAutomatic = 7
        'cActualizarLimites = 8

        'Trama Modelo:
        '@ + NroMotor + Action + PosH + PosL + TargetPosH + TargetPosL + 
        'Vel + ConfirmNum + CRC

        ReDim byteTrama(10)

        byteTrama(0) = 64                        '"@" Inicio de trama
        byteTrama(1) = numMotor                  'Numero de motor
        byteTrama(2) = CByte(Action)             'Accion
        byteTrama(3) = Posicion >> 8             'Posicion MSB
        byteTrama(4) = Posicion And &HFF         'Posicion LSB
        byteTrama(5) = TargetPosicion >> 8       'TargetPos MSB
        byteTrama(6) = TargetPosicion And &HFF   'TargetPos LSB
        byteTrama(7) = Velocidad                 'Velocidad

        'Bloqueo el acceso de otros Thread al BufferTXplaca
        'para evitar que lo puedan modificar mientras lo 
        'estoy cargando.
        SyncLock BloqueoAcceso
            'Las datos a guardar en el Buffer seran
            'La trama + Nro de respuesta 
            '+ cantidad de retransmisiones sin respuesta 
            '+ Prioridad + RequiereRespuesta + Repeat.
            'Cada dato en un nivel de la lista.
            'Es decir consume 6 niveles por datos
            'por cada nivel de buffer por motor.
            '
            'Ej:
            'BufferTXplaca(motorX).Add("@1F")
            'Trama = "@1F" ----> Trama a enviar.

            'BufferTXplaca(motorX).Add("1")
            'Respuesta = 1 ----> Valor que se envia a placa
            'para que responda con ese mismo y asegurarme 
            'que lo recibio. La recepcion la chequea rutina 
            'de rx.

            'BufferTXplaca(motorX).Add("0")
            'Cantidad Retrasmisiones = 0 ------> Aca se carga 
            'en 0 y rutina de TX se encarga de sumarla.

            'Prioridad = 3 ------> Prioridad con que se quiere 
            'enviar esta trama 1 mas alta, 5 mas baja
            'BufferTXplaca(motorX).Add("3")

            'RequiereRespuesta = True ------> Si la trama
            'requiere de respuesta por parte de la placa
            'True = "1"  False = "0"
            'BufferTXplaca(motorX).Add("1")

            'Repeat = True ------> Si la trama se va repetir
            'hasta que haya otra en el BufferdeTX.
            'True = "1"  False = "0"
            'BufferTXplaca(motorX).Add("1")

            'Esto se repetiria por cada nivel de stack.
            '
            'Primero chequeo en la lista que no tenga tramas en 
            'el Buffer para ese motor y eventualmente conocer el
            'indice para poder leer que numero correlativo
            'corresponderia de Nro de respuesta.
            With BufferTX(numMotor)
                If .Count > 0 Then   'El motor tiene datos en BufferTx
                    CantidadPosBuffer = .Count / 6

                    For j As Byte = 0 To CantidadPosBuffer - 1
                        If tempRespuesta < BufferTX(numMotor)((j * 6) + 1) Then    'Busco el numero mas alto de NroRespuesta que ya este en el buffer
                            tempRespuesta = BufferTX(numMotor)((j * 6) + 1)
                        End If
                    Next

                    If tempRespuesta = 255 Then         'Esto es para que no se produzca un eventual
                        tempRespuesta = 0               'desbordamiento de la variable que suma el 
                    End If                              'numero de confirmacion.

                    byteTrama(8) = tempRespuesta + 1    'Numero confirmacion

                    .Add(GenerarTramaYcRc(byteTrama))   'Agrego al BufferTX Trama
                    .Add(CStr(byteTrama(8)))            'Agrego al BufferTX Nro Respuesta Esperado
                    .Add("0")                           'Agrego al BufferTX Cantidad Retrasmisiones = 0
                    .Add(prioridad)                     'Agrego al BufferTX Prioridad

                    If UseCheckPacket Then              'Agrego si la trama requiere confirmacion
                        .Add("1")
                    Else
                        .Add("0")
                    End If

                    'Si la trama tiene el atributo Repeat hay que repetir
                    'la transmision mientras no haya otra trama en el buffer.
                    If Repeat Then                      'Agrego si la trama requiere repeat
                        .Add("1")
                    Else
                        .Add("0")

                    End If

                Else

                    byteTrama(8) = 1
                    .Add(GenerarTramaYcRc(byteTrama))   'Agrego al BufferTX Trama
                    .Add("1")                           'Agrego al BufferTX Nro Respuesta Esperado. 255 no requiere conf.
                    .Add("0")                           'Agrego al BufferTX Cantidad Retrasmisiones = 0
                    .Add(prioridad)                     'Agrego al BufferTX Prioridad
                    If UseCheckPacket Then              'Agrego si la trama requiere confirmacion
                        .Add("1")
                    Else
                        .Add("0")
                    End If

                    'Si la trama tiene el atributo Repeat hay que repetir
                    'la transmision mientras no haya otra trama en el buffer.
                    If Repeat Then                      'Agrego si la trama requiere repeat
                        .Add("1")
                    Else
                        .Add("0")

                    End If

                End If

                'Grabo Trama en db
                'AGREGAR:
                'ByVal Action As ComandoMotor,
                '              ByVal numMotor As Byte,
                '             ByVal Posicion As UInt16,
                '            ByVal Optional Velocidad As Byte = 1,
                '           ByVal Optional TargetPosicion As UInt16 = 0,
                '          ByVal Optional prioridad As Byte = 1,
                '         ByVal Optional Repeat As Boolean = 0)

                Using conn As New SQLite.SQLiteConnection(bd_Conexion)
                    conn.Open()
                    sql = "INSERT INTO Audit (Date, Who, NroNodo, Trama,ac_Posicion, ac_Velocidad, ac_TargetPosicion,ac_Prioridad, ac_Repeat ) VALUES(@param1, @param2, @param3,@param4,@param5,@param6,@param7,@param8,@param9)"
                    Dim cmdGuardar As SQLite.SQLiteCommand = New SQLite.SQLiteCommand(Sql, conn)
                    cmdGuardar.Parameters.AddWithValue("@param1", DateTime.UtcNow.Ticks)
                    cmdGuardar.Parameters.AddWithValue("@param2", "AccionMotores")
                    cmdGuardar.Parameters.AddWithValue("@param3", numMotor)
                    cmdGuardar.Parameters.AddWithValue("@param4", GenerarTramaYcRc(byteTrama))
                    cmdGuardar.Parameters.AddWithValue("@param5", Posicion)
                    cmdGuardar.Parameters.AddWithValue("@param6", Velocidad)
                    cmdGuardar.Parameters.AddWithValue("@param7", TargetPosicion)
                    cmdGuardar.Parameters.AddWithValue("@param8", prioridad)
                    cmdGuardar.Parameters.AddWithValue("@param9", Repeat)
                    cmdGuardar.ExecuteNonQuery()
                    conn.Close()
                End Using

            End With

        End SyncLock

    End Sub

    Private Function GenerarTramaYcRc(ByVal trama As Byte()) As String
        'Esta Sub se encarga de generar el CRC de la trama de TX.
        'Tambien devuelve la trama ya conformada lista para agregar
        'al BufferTX

        Dim CRC As Byte
        Dim DevTrama As String

        For i As Byte = 0 To 8          'Calcula CRC
            CRC = CRC Xor trama(i)
        Next
        trama(9) = CRC

        DevTrama = "@"                  'Inicio de trama
        For i As Byte = 1 To 9          'Armo trama a devolver
            DevTrama = DevTrama & trama(i).ToString("X2")
        Next

        Return DevTrama

    End Function

    Public Sub ClearBufferTX(ByVal nroMotor As Byte)
        'Esta sub elimina todos los datos del BufferTX del motor
        'solicitado. Sirve para cancelar el reenvio de tramas
        'que puedan estar marcadas como repeat o con confirmacion
        'que se quieran cancelar.
        If nroMotor > CantidadMotores Then  'Se esta intentando vaciar buffer de un motor que no existe
            Exit Sub
        End If

        'Bloqueo el acceso de otros Thread al BufferTXplaca 
        'para evitar que lo puedan modificar mientras lo 
        'estoy manipulando
        SyncLock BloqueoAcceso

            If BufferTX(nroMotor).Count > 0 Then                      'El motor tiene datos en BufferTx ?
                For r As Byte = 0 To (BufferTX(nroMotor).Count - 1)
                    BufferTX(nroMotor).RemoveAt(0)
                Next
            End If

        End SyncLock

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Public Property ActivarComunicacion As Boolean

        Get
            ActivarComunicacion = ActivarCom
        End Get
        Set(value As Boolean)
            Try
                If MySerialPort.IsOpen Then
                    MySerialPort.Close()
                End If
                ActivarCom = value
                MySerialPort.Open()
            Catch ex As Exception

            End Try

        End Set
    End Property

    Public Property QtydMotores As Byte
        'Defino la cantidad de placas que reciben y transmiten 
        'info remota. Redimensiono array a esa cantidad

        Get
            QtydMotores = CantidadMotores
        End Get
        Set(value As Byte)

        End Set
    End Property

End Class
