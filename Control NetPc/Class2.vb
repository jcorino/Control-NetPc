'------------------------------------------------------------------------------
' Clase para manejar ficheros de configuración                      (15/Nov/05)
'
' Las secciones siempre estarán dentro de <configuration>
' al menos así lo guardará esta clase, aunque permite leer pares key / value.
' Para que se sepa que se lee de configuration,
' en el código se indica explícitamente.
'
' Basada en mi código publicado el 22/Feb/05 en:
' http://www.elguille.info/NET/dotnet/appSettings2.htm
' Pero para usarla de forma independiente de ConfigurationSettings
'
' Revisado para poder guardar automáticamente                       (21/Feb/06)
' Poder leer todas las secciones y las claves de una sección        (21/Feb/06)
' Se puede eliminar una clave (a petición de Juansa)                (15/Ene/07)
'   Si después de eliminar la clave, la sección está vacía, se borra
' Se leen correctamente las claves que están como atributos         (15/Ene/07)
' Permite leer las claves de comentarios                            (15/Ene/07)
' Se filtran más caracteres no permitidos en secciones y claves     (15/Ene/07)
'
' No filtrar el / ya que se usa para niveles de configuración       (01/Ago/07)
'
' Revisado para publicar en mi sitio (como revisión actualizada)    (12/Ene/08)
'
' ©Guillermo 'guille' Som, 2005-2008
'------------------------------------------------------------------------------
Option Explicit On
Option Strict On

Imports Microsoft.VisualBasic
Imports System

Imports System.Collections
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Xml
Imports System.IO

Namespace jmc.Util
    Public Class ConfigXml

        '----------------------------------------------------------------------
        ' Los campos y métodos privados
        '----------------------------------------------------------------------
        Private mGuardarAlAsignar As Boolean = True
        Private Const configuration As String = "configuration/"
        Private ficConfig As String = ""
        Private configXml As New XmlDocument
        ' Los caracteres no permitidos en los nombres               (15/Ene/07)
        ' Se sustituirán por un guión bajo
        Private ReadOnly noPermitidos() As Char = " !|@#$%&()=?¿¡*[]{};.,:<>ç'ºª\-+".ToCharArray

        ' Para hacer las pruebas del tiempo empleado
        'Private mTest As New System.Diagnostics.Stopwatch
        'Private mt1, mt2 As TimeSpan
        '
        ' Mejor usar constantes                                 (15/Ene/07)
        ' y que estén al principio del fichero para tenerlas a mano
        Const revDate As String = "Sat, 12 Jan 2008 18:31:52 GMT"
        Const cfgInfo As String = "Generado con ConfigXml para Visual Basic 2008"
        Const copyGuille As String = "©Guillermo 'guille' Som, 2005-2008"


        ''' <summary>
        ''' Indica si se se guardarán los datos cuando se añadan nuevos.
        ''' </summary>
        ''' <value>
        ''' Indica si se se guardarán los datos cuando se añadan nuevos.
        ''' </value>
        ''' <returns>
        ''' Un valor verdadero o falso según el valor de la propiedad
        ''' </returns>
        ''' <remarks>21/Feb/06</remarks>
        Public Property GuardarAlAsignar() As Boolean
            Get
                Return mGuardarAlAsignar
            End Get
            Set(ByVal value As Boolean)
                mGuardarAlAsignar = value
            End Set
        End Property

        ''' <summary>
        ''' Obtiene un valor de tipo cadena de la sección y clave indicadas.
        ''' </summary>
        ''' <param name="seccion">
        ''' La sección de la que queremos obtener el valor
        ''' </param>
        ''' <param name="clave">
        ''' La clave de la que queremos recuperar el valor
        ''' </param>
        ''' <returns>
        ''' Un valor de tipo cadena con el valor de la sección y clave indicadas
        ''' </returns>
        ''' <remarks>
        ''' Existe otra sobrecarga para indicar un valor predeterminado.
        ''' Tanbién hay otras dos sobrecargas para valores enteros y boolean.
        ''' </remarks>
        Public Function GetValue(ByVal seccion As String, ByVal clave As String) As String
            Return GetValue(seccion, clave, "")
        End Function

        ''' <summary>
        ''' Obtiene un valor de tipo cadena de la sección y clave indicadas.
        ''' </summary>
        ''' <param name="seccion">
        ''' La sección de la que queremos obtener el valor
        ''' </param>
        ''' <param name="clave">
        ''' La clave de la que queremos recuperar el valor
        ''' </param>
        ''' <param name="predeterminado">
        ''' El valor predeterminado para cuando no exista.
        ''' </param>
        ''' <returns>
        ''' Un valor de tipo cadena con el valor de la sección y clave indicadas
        ''' </returns>
        ''' <remarks></remarks>
        Public Function GetValue(ByVal seccion As String,
                                 ByVal clave As String,
                                 ByVal predeterminado As String) As String
            Return CfgGetValue(seccion, clave, predeterminado)
        End Function

        ''' <summary>
        ''' Obtiene un valor de tipo entero de la sección y clave indicadas.
        ''' </summary>
        ''' <param name="seccion">
        ''' La sección de la que queremos obtener el valor
        ''' </param>
        ''' <param name="clave">
        ''' La clave de la que queremos recuperar el valor
        ''' </param>
        ''' <param name="predeterminado">
        ''' El valor predeterminado para cuando no exista.
        ''' </param>
        ''' <returns>
        ''' Un valor de tipo entero con el valor de la sección y clave indicadas
        ''' </returns>
        ''' <remarks></remarks>
        Public Function GetValue(ByVal seccion As String,
                                 ByVal clave As String,
                                 ByVal predeterminado As Integer) As Integer
            Return CInt(CfgGetValue(seccion, clave, predeterminado.ToString))
        End Function

        ''' <summary>
        ''' Obtiene un valor de tipo boolean de la sección y clave indicadas.
        ''' </summary>
        ''' <param name="seccion">
        ''' La sección de la que queremos obtener el valor
        ''' </param>
        ''' <param name="clave">
        ''' La clave de la que queremos recuperar el valor
        ''' </param>
        ''' <param name="predeterminado">
        ''' El valor predeterminado para cuando no exista.
        ''' </param>
        ''' <returns>
        ''' Un valor de tipo boolean con el valor de la sección y clave indicadas
        ''' </returns>
        ''' <remarks>
        ''' Internamente el valor se guarda con un cero para False y uno para True
        ''' </remarks>
        Public Function GetValue(ByVal seccion As String,
                                 ByVal clave As String,
                                 ByVal predeterminado As Boolean) As Boolean
            Dim def As String = "0"
            If predeterminado Then def = "1"
            def = CfgGetValue(seccion, clave, def)
            If def = "1" Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Asignar un valor de tipo cadena en la sección y clave indicadas.
        ''' </summary>
        ''' <param name="seccion">
        ''' La sección de la que queremos obtener el valor
        ''' </param>
        ''' <param name="clave">
        ''' La clave de la que queremos recuperar el valor</param>
        ''' <param name="valor">
        ''' El valor a asignar</param>
        ''' <remarks>
        ''' El valor se guardar como un elemento de la sección indicada.
        ''' <seealso cref="SetKeyValue" />
        ''' </remarks>
        Public Sub SetValue(ByVal seccion As String, ByVal clave As String, ByVal valor As String)
            CfgSetValue(seccion, clave, valor)
        End Sub

        ''' <summary>
        ''' Asignar un valor de tipo entero en la sección y clave indicadas.
        ''' </summary>
        ''' <param name="seccion">
        ''' La sección de la que queremos obtener el valor</param>
        ''' <param name="clave">
        ''' La clave de la que queremos recuperar el valor</param>
        ''' <param name="valor">
        ''' El valor a asignar</param>
        ''' <remarks>
        ''' El valor se guardar como un elemento de la sección indicada.
        ''' El valor siempre se guarda como un valor de cadena.
        ''' <seealso cref="SetKeyValue" />
        ''' </remarks>
        Public Sub SetValue(ByVal seccion As String, ByVal clave As String, ByVal valor As Integer)
            CfgSetValue(seccion, clave, valor.ToString)
        End Sub

        ''' <summary>
        ''' Asignar un valor de tipo boolean en la sección y clave indicadas.
        ''' </summary>
        ''' <param name="seccion">
        ''' La sección de la que queremos obtener el valor</param>
        ''' <param name="clave">
        ''' La clave de la que queremos recuperar el valor</param>
        ''' <param name="valor">
        ''' El valor a asignar</param>
        ''' <remarks>
        ''' El valor se guardar como un elemento de la sección indicada.
        ''' El valor siempre se guarda como un valor de cadena, siendo un 1 para True y 0 para False.
        ''' <seealso cref="SetKeyValue" />
        ''' </remarks>
        Public Sub SetValue(ByVal seccion As String, ByVal clave As String, ByVal valor As Boolean)
            If valor Then
                CfgSetValue(seccion, clave, "1")
            Else
                CfgSetValue(seccion, clave, "0")
            End If
        End Sub

        ''' <summary>
        ''' Asigna un valor de tipo cadena en la sección y clave indicadas.
        ''' </summary>
        ''' <param name="seccion">
        ''' La sección de la que queremos obtener el valor</param>
        ''' <param name="clave">
        ''' La clave de la que queremos recuperar el valor</param>
        ''' <param name="valor">
        ''' El valor a asignar</param>
        ''' <remarks>
        ''' El valor se guarda como un atributo de la sección indicada.
        ''' La clave se guarda con el atributo key y el valor con el atributo value.
        ''' <seealso cref="SetValue" />
        ''' </remarks>
        Public Sub SetKeyValue(ByVal seccion As String, ByVal clave As String, ByVal valor As String)
            CfgSetKeyValue(seccion, clave, valor)
        End Sub

        ''' <summary>
        ''' Asigna un valor de tipo entero en la sección y clave indicadas.
        ''' </summary>
        ''' <param name="seccion">
        ''' La sección de la que queremos obtener el valor</param>
        ''' <param name="clave">
        ''' La clave de la que queremos recuperar el valor</param>
        ''' <param name="valor">
        ''' El valor a asignar</param>
        ''' <remarks>
        ''' El valor se guarda como un atributo de la sección indicada.
        ''' La clave se guarda con el atributo key y el valor con el atributo value.
        ''' El valor siempre se guarda como un valor de cadena.
        ''' <seealso cref="SetValue" />
        ''' </remarks>
        Public Sub SetKeyValue(ByVal seccion As String, ByVal clave As String, ByVal valor As Integer)
            CfgSetKeyValue(seccion, clave, valor.ToString)
        End Sub

        ''' <summary>
        ''' Asigna un valor de tipo boolean en la sección y clave indicadas.
        ''' </summary>
        ''' <param name="seccion">
        ''' La sección de la que queremos obtener el valor</param>
        ''' <param name="clave">
        ''' La clave de la que queremos recuperar el valor</param>
        ''' <param name="valor">
        ''' El valor a asignar</param>
        ''' <remarks>
        ''' El valor se guarda como un atributo de la sección indicada.
        ''' La clave se guarda con el atributo key y el valor con el atributo value.
        ''' El valor siempre se guarda como un valor de cadena, siendo un 1 para True y 0 para False.
        ''' <seealso cref="SetValue" />
        ''' </remarks>
        Public Sub SetKeyValue(ByVal seccion As String,
                               ByVal clave As String,
                               ByVal valor As Boolean)
            If valor Then
                CfgSetKeyValue(seccion, clave, "1")
            Else
                CfgSetKeyValue(seccion, clave, "0")
            End If
        End Sub

        ''' <summary>
        ''' Elimina la sección indicada, aunque en realidad la deja vacía.
        ''' </summary>
        ''' <param name="seccion">
        ''' La sección a eliminar.</param>
        ''' <remarks></remarks>
        Public Sub RemoveSection(ByVal seccion As String)
            Dim n As XmlNode
            ' Filtar la sección para evitar errores                 (15/Ene/07)
            seccion = FiltrarNombre(seccion)

            n = configXml.SelectSingleNode(configuration & seccion)
            If Not n Is Nothing Then
                n.RemoveAll()
                ' Si se ha indicado que se guarde al asignar        (21/Feb/06)
                If mGuardarAlAsignar Then
                    Me.Save()
                End If
            End If
        End Sub

        ''' <summary>
        ''' Función de apoyo para filtrar los caracteres no permitidos
        ''' </summary>
        ''' <param name="nombre">
        ''' El nombre a filtrar</param>
        ''' <returns>
        ''' El nombre filtrado</returns>
        ''' <remarks>
        ''' Usar esta función internamente para que las secciones y claves
        ''' no incluyan los caracteres indicados en la constante noPermitidos
        ''' </remarks>
        Private Function FiltrarNombre(ByVal nombre As String) As String
            ' Solo repetir el bucle si hay caracteres no permitidos
            ' Esto es más rápido si no tiene caracteres "raros",
            ' en caso de que los tenga, será más lento, pero...
            ' se supone que no debe tener caracteres no admitidos.
            'mTest.Reset()
            'mTest.Start()
            If nombre.IndexOfAny(noPermitidos) = -1 Then
                'mt1 = mTest.Elapsed
                Return nombre
            End If
            'mTest.Reset()
            'mTest.Start()
            Dim sb As New System.Text.StringBuilder(nombre)
            For Each c As Char In noPermitidos '.ToCharArray
                sb.Replace(c, "_"c)
            Next
            'mt2 = mTest.Elapsed
            Return sb.ToString
        End Function

        ''' <summary>
        ''' Eliminar la clave indicada
        ''' </summary>
        ''' <param name="seccion">
        ''' La sección en la que está la clave a eliminar
        ''' </param>
        ''' <param name="clave">
        ''' La clave a eliminar
        ''' </param>
        ''' <returns>
        ''' Devuelve True si se borró, en otro caso, devuelve False
        ''' </returns>
        ''' <remarks>
        ''' 15/Ene/07
        ''' Los valores al estilo de appSettings no se borran
        ''' </remarks>
        Public Function RemoveKey(ByVal seccion As String,
                                  ByVal clave As String) As Boolean
            Dim n As XmlNode
            '
            ' Filtrar los caracteres no válidos
            ' en principio solo comprobamos el espacio
            'seccion = seccion.Replace(" ", "_")
            'clave = clave.Replace(" ", "_")
            seccion = FiltrarNombre(seccion)
            clave = FiltrarNombre(clave)
            '
            ' Primero comprobar si están el formato de appSettings: <add key = clave value = valor />
            n = configXml.SelectSingleNode(configuration & seccion &
                                           "/add[@key=""" & clave & """]")
            If n IsNot Nothing Then
                ' Por ahora no se puede eliminar estas claves
                'n.Attributes.RemoveAll()
                Return False
            End If
            '
            ' Después se comprueba si está en el formato <Seccion clave = valor>
            n = configXml.SelectSingleNode(configuration & seccion)
            If n IsNot Nothing Then
                Dim a As XmlAttribute = n.Attributes(clave)
                If a IsNot Nothing Then
                    Dim b As XmlAttribute
                    b = n.Attributes.Remove(a)
                    If b Is Nothing Then
                        Return False
                    Else
                        ' Si en la clave no quedan claves ni atributos,
                        ' borrarla para que no se quede vacía
                        If n.HasChildNodes = False _
                        AndAlso (n.Attributes IsNot Nothing _
                                 AndAlso n.Attributes.Count = 0) Then
                            RemoveKey(seccion, n.Name)
                        End If
                        ' Si se ha indicado que se guarde al asignar
                        If mGuardarAlAsignar Then
                            Me.Save()
                        End If
                        Return True
                    End If
                End If
            End If
            '
            ' Por último se comprueba si es un elemento de seccion:
            '   <seccion><clave>valor</clave></seccion>
            ' Detectar los posibles errores...                      (15/Ene/07)
            Try
                n = configXml.SelectSingleNode(configuration & seccion & "/" & clave)
            Catch ex As Exception
                n = Nothing
            End Try
            'n = configXml.SelectSingleNode(configuration & seccion & "/" & clave)
            If n IsNot Nothing Then
                Dim n1 As XmlNode
                n1 = n.ParentNode.RemoveChild(n)
                If n1 Is Nothing Then
                    Return False
                Else
                    ' Eliminar la clave si no tiene otras claves ni atributos
                    n1 = configXml.SelectSingleNode(configuration & seccion)
                    If n1.HasChildNodes = False _
                    AndAlso (n1.Attributes IsNot Nothing _
                             AndAlso n1.Attributes.Count = 0) Then
                        n1.ParentNode.RemoveChild(n1)
                    End If
                    ' Si se ha indicado que se guarde al asignar    (15/Ene/07)
                    If mGuardarAlAsignar Then
                        Me.Save()
                    End If
                    Return True
                End If
            End If
            '
            Return False
        End Function

        ''' <summary>
        ''' Guardar el fichero de configuración.
        ''' </summary>
        ''' <remarks>
        ''' Si no se llama a este método, no se guardará de forma permanente.
        ''' Para guardar automáticamente al asignar,
        ''' asignar un valor verdadero a la propiedad 
        ''' <see cref="GuardarAlAsignar">GuardarAlAsignar</see>
        ''' </remarks>
        Public Sub Save()
            configXml.Save(ficConfig)
        End Sub

        ''' <summary>
        ''' Lee el fichero de configuración.
        ''' </summary>
        ''' <remarks>
        ''' Si no existe, se crea uno nuevo con los valores predeterminados.
        ''' </remarks>
        Public Sub Read()
            Dim fic As String = ficConfig

            If File.Exists(fic) Then
                configXml.Load(fic)
                ' Actualizar los datos de la información de esta clase
                Dim b As Boolean = mGuardarAlAsignar
                mGuardarAlAsignar = False
                Me.SetValue("configXml_Info", "info", cfgInfo)
                Me.SetValue("configXml_Info", "revision", revDate)
                Me.SetValue("configXml_Info", "formatoUTF8",
                            "El formato de este fichero debe ser UTF-8")
                mGuardarAlAsignar = b
                Me.Save()
            Else
                ' Crear el XML de configuración con la sección General
                ' Aunque no se añadan retornos de carro, al guardarse se añaden y se indenta.
                Dim sb As New System.Text.StringBuilder
                sb.Append("<?xml version=""1.0"" encoding=""utf-8"" ?>")
                sb.Append("<configuration>")
                ' Por si es un fichero appSetting
                sb.Append("<configSections>")
                sb.Append("<section name=""General"" " &
                          "type=""System.Configuration.DictionarySectionHandler"" />")
                sb.Append("</configSections>")
                sb.Append("<General>")
                sb.Append("<!-- Los valores irán dentro del elemento indicado por la clave -->")
                sb.Append("<!-- Aunque también se podrán indicar como pares key / value -->")
                sb.AppendFormat("<add key=""Revisión"" value=""{0}"" />", revDate)
                sb.Append("<!-- La clase siempre los añade como un elemento -->")
                sb.AppendFormat("<Copyright>{0}</Copyright>", copyGuille)
                sb.Append("</General>")
                ' Pero queda más "bonico" con los cambios de línea... en fin... cosas mías...
                sb.AppendFormat("<configXml_Info>{0}", vbCrLf)
                sb.AppendFormat("<info>{0}</info>{1}", cfgInfo, vbCrLf)
                sb.AppendFormat("<copyright>{0}</copyright>{1}", copyGuille, vbCrLf)
                sb.AppendFormat("<revision>{0}</revision>{1}", revDate, vbCrLf)
                sb.AppendFormat("<formatoUTF8>El formato de este fichero debe ser UTF-8" &
                                "</formatoUTF8>{0}", vbCrLf)
                sb.AppendFormat("</configXml_Info>{0}", vbCrLf)
                '
                sb.Append("</configuration>")
                ' Asignamos la cadena al objeto
                configXml.LoadXml(sb.ToString)
                '
                ' Guardamos el contenido de configXml y creamos el fichero
                configXml.Save(ficConfig)
            End If
        End Sub

        ''' <summary>
        ''' El nombre del fichero de configuración.
        ''' </summary>
        ''' <value>
        ''' El path completo con el nombre del fichero de configuración.</value>
        ''' <returns>
        ''' Una cadena con el fichero de configuración.</returns>
        ''' <remarks>
        ''' El nombre del fichero se debe indicar en el constructor.
        ''' </remarks>
        Public Property FileName() As String
            Get
                Return ficConfig
            End Get
            Set(ByVal value As String)
                ' Al asignarlo, NO leemos el contenido del fichero
                ficConfig = value
                'LeerFile()
            End Set
        End Property

        ''' <summary>
        ''' Constructor en el que indicamos el nombre del fichero de configuración.
        ''' </summary>
        ''' <param name="fic">
        ''' El fichero a usar para guardar los datos de configuración.
        ''' </param>
        ''' <remarks>
        ''' Si no existe, se creará.
        ''' Al usar este constructor, por defecto se guardarán los valores al asignarlos.
        ''' </remarks>
        Public Sub New(ByVal fic As String)
            ficConfig = fic
            ' Por defecto se guarda al asignar los valores          (21/Feb/06)
            mGuardarAlAsignar = True
            Read()
        End Sub


        ''' <summary>
        ''' Constructor en el que indicamos el nombre del fichero de configuración
        ''' y si guardamos o no automáticamente.
        ''' </summary>
        ''' <param name="fic">
        ''' El fichero a usar para guardar los datos de configuración.</param>
        ''' <param name="guardarAlAsignar">
        ''' Un valor verdadero o falso para indicar
        ''' si se guardan los datos automáticamente al asignarlos.</param>
        ''' <remarks>21/Feb/06</remarks>
        Public Sub New(ByVal fic As String, ByVal guardarAlAsignar As Boolean)
            ficConfig = fic
            mGuardarAlAsignar = guardarAlAsignar
            Read()
        End Sub

        ''' <summary>
        ''' Devuelve una colección de tipo List con las secciones del fichero de configuración.
        ''' </summary>
        ''' <returns>
        ''' Una colección de tipo List(Of String) con las secciones del fichero de configuración.
        ''' </returns>
        ''' <remarks>
        ''' 21/Feb/06
        ''' Este método solo se puede usar en la .NET 2.0 o superior.
        ''' </remarks>
        Public Function Secciones() As List(Of String)
            Dim d As New List(Of String)
            Dim root As XmlNode
            Dim s As String = "configuration"
            root = configXml.SelectSingleNode(s)
            If root IsNot Nothing Then
                For Each n As XmlNode In root.ChildNodes
                    d.Add(n.Name)
                Next
            End If
            Return d
        End Function

        ''' <summary>
        ''' Devuelve una colección de tipo Dictionary con las claves y valores de la sección indicada.
        ''' </summary>
        ''' <param name="seccion">
        ''' La sección de la que queremos obtener las claves y valores.
        ''' </param>
        ''' <returns>
        ''' Una colección de tipo Dictionary(Of String, String) con las claves y valores.
        ''' </returns>
        ''' <remarks>
        ''' 21/Feb/06
        ''' Este método solo se puede usar en la .NET 2.0 o superior.
        ''' 15/Ene/07: También se devuelven los valores internos:
        ''' <![CDATA[ <Prueba2 Uno="Valor de uno" /> ]]>
        ''' En este caso, Prueba2 es una sección y Uno es una clave.
        ''' 15/Ene/07: También lee los pares valor/clave, por ejemplo:
        ''' <![CDATA[ <section name="General" type="System.Configuration.DictionarySectionHandler" /> ]]>
        ''' <![CDATA[ <add key="Revisión" value="Tue, 21 Feb 2006 19:45:00 GMT" /> ]]>
        ''' Antes solo devolvía section en el primer caso y add en el segundo,
        ''' ahora devuelve como clave el contenido de name/key y como valor type/value
        ''' respectivamente.
        ''' Nota sobre los comentarios insertados en el fichero de configuración:
        ''' Las claves #comment se devuelven numeradas, por tanto el valor devuelto
        ''' por esta colección no será el nombre real de ese nombre.
        ''' </remarks>
        Public Function Claves(ByVal seccion As String) As Dictionary(Of String, String)
            Dim d As New Dictionary(Of String, String)
            Dim root As XmlNode
            ' Para permitir más de un comentario
            Dim nComment As Integer = 0
            Dim sName As String

            seccion = FiltrarNombre(seccion)

            root = configXml.SelectSingleNode(configuration & seccion)
            If root IsNot Nothing Then
                For Each n As XmlNode In root.ChildNodes
                    ' Si no queremos los comentarios                (15/Ene/07)
                    ' como parte de la colección de claves, dejar el Continue For
                    If n.Name = "#comment" Then
                        ' Si queremos tener todos los comentarios
                        nComment += 1
                        sName = n.Name & nComment.ToString("0000")
                    Else
                        sName = n.Name
                    End If
                    If d.ContainsKey(sName) = False Then
                        ' Si el valor es una cadena vacía,          (15/Ene/07)
                        ' es que están como atributos
                        If String.IsNullOrEmpty(n.InnerText) = False Then
                            d.Add(sName, n.InnerText)
                        Else
                            If n.Attributes IsNot Nothing AndAlso n.Attributes.Count = 2 Then
                                ' Deben existir dos atributos,
                                ' el primero es la clave y el segundo el valor
                                d.Add(n.Attributes(0).Value, n.Attributes(1).Value)
                            End If
                        End If
                    End If
                Next
                'TODO: 15/Ene/07
                ' En este caso, se debería asignar mediante SetKeyValue
                If root.Attributes IsNot Nothing AndAlso root.Attributes.Count > 0 Then
                    For Each a As XmlAttribute In root.Attributes
                        If d.ContainsKey(a.Name) = False Then
                            d.Add(a.Name, a.Value)
                        End If
                    Next
                End If
            End If
            Return d
        End Function

        '
        '----------------------------------------------------------------------
        ' Los métodos privados
        '----------------------------------------------------------------------
        '

        ' El método interno para guardar los valores
        ' Este método siempre guardará en el formato <seccion><clave>valor</clave></seccion>
        Private Sub CfgSetValue(ByVal seccion As String,
                                ByVal clave As String,
                                ByVal valor As String)
            Dim n As XmlNode

            ' Filtrar los caracteres no válidos
            seccion = FiltrarNombre(seccion)
            clave = FiltrarNombre(clave)

            ' Se comprueba si es un elemento de la sección:
            '   <seccion><clave>valor</clave></seccion>
            ' Detectar los posibles errores...                      (15/Ene/07)
            Try
                n = configXml.SelectSingleNode(configuration & seccion & "/" & clave)
            Catch ex As Exception
                n = Nothing
            End Try
            If n IsNot Nothing Then
                n.InnerText = valor
            Else
                Dim root As XmlNode
                Dim elem As XmlElement
                root = configXml.SelectSingleNode(configuration & seccion)
                If root Is Nothing Then
                    ' Si no existe el elemento principal,
                    ' lo añadimos a <configuration>
                    elem = configXml.CreateElement(seccion)
                    configXml.DocumentElement.AppendChild(elem)
                    root = configXml.SelectSingleNode(configuration & seccion)
                End If
                If root IsNot Nothing Then
                    ' Crear el elemento
                    elem = configXml.CreateElement(clave)
                    elem.InnerText = valor
                    ' Añadirlo al nodo indicado
                    root.AppendChild(elem)
                End If
            End If

            ' Si se debe guardar al asignar                         (21/Feb/06)
            If mGuardarAlAsignar Then
                Me.Save()
            End If
        End Sub

        ' Asigna un atributo a una sección
        ' Por ejemplo: <Seccion clave=valor>...</Seccion>
        ' También se usará para el formato de appSettings: <add key=clave value=valor />
        '   Aunque en este caso, debe existir el elemento a asignar.
        Private Sub CfgSetKeyValue(ByVal seccion As String,
                                   ByVal clave As String,
                                   ByVal valor As String)
            Dim n As XmlNode

            ' Filtrar los caracteres no válidos
            seccion = FiltrarNombre(seccion)
            clave = FiltrarNombre(clave)

            n = configXml.SelectSingleNode(configuration & seccion & "/add[@key=""" & clave & """]")
            If n IsNot Nothing Then
                n.Attributes("value").InnerText = valor
            Else
                Dim root As XmlNode
                Dim elem As XmlElement
                root = configXml.SelectSingleNode(configuration & seccion)
                If root Is Nothing Then
                    ' Si no existe el elemento principal,
                    ' lo añadimos a <configuration>
                    elem = configXml.CreateElement(seccion)
                    configXml.DocumentElement.AppendChild(elem)
                    root = configXml.SelectSingleNode(configuration & seccion)
                End If
                If root IsNot Nothing Then
                    Dim a As XmlAttribute
                    a = TryCast(configXml.CreateNode(XmlNodeType.Attribute, clave, Nothing),
                                XmlAttribute)
                    If a IsNot Nothing Then
                        a.InnerText = valor
                        root.Attributes.Append(a)
                    End If
                End If
            End If

            ' Si se debe guardar al asignar                         (21/Feb/06)
            If mGuardarAlAsignar Then
                Me.Save()
            End If
        End Sub

        ' Devolver el valor de la clave indicada
        Private Function CfgGetValue(ByVal seccion As String,
                                     ByVal clave As String,
                                     ByVal valor As String) As String
            Dim n As XmlNode

            ' Filtrar los caracteres no válidos
            ' en principio solo comprobamos el espacio
            seccion = FiltrarNombre(seccion)
            clave = FiltrarNombre(clave)

            ' Primero comprobar si están el formato de appSettings: <add key = clave value = valor />
            n = configXml.SelectSingleNode(configuration & seccion & "/add[@key=""" & clave & """]")
            If n IsNot Nothing Then
                Return n.Attributes("value").InnerText
            End If

            ' Después se comprueba si está en el formato <Seccion clave = valor>
            n = configXml.SelectSingleNode(configuration & seccion)
            If n IsNot Nothing Then
                Dim a As XmlAttribute = n.Attributes(clave)
                If a IsNot Nothing Then
                    Return a.InnerText
                End If
            End If

            ' Por último se comprueba si es un elemento de seccion:
            '   <seccion><clave>valor</clave></seccion>
            n = configXml.SelectSingleNode(configuration & seccion & "/" & clave)
            If n IsNot Nothing Then
                Return n.InnerText
            End If

            ' Si no existe, se devuelve el valor predeterminado
            Return valor
        End Function
    End Class
End Namespace