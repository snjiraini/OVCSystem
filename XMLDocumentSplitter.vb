Imports System.IO
Imports System.Xml

Public Class XMLDocumentSplitter
#Region "Private and Public Declarations"
    Private m_SplitType As SplitTypes
    Private m_SplitSize As Long
    Private m_SplitCount As Long
    Private m_TotalSplitSizes As Long

    Public Enum SplitTypes
        ByDocumentSize
        ByElementCount
    End Enum
#End Region
#Region "Constructors, Destructors and Initialization"
    Public Sub New(Optional ByVal split_type As SplitTypes = SplitTypes.ByDocumentSize, _
                    Optional ByVal split_size As Long = 512000)
        m_SplitType = split_type : m_SplitSize = split_size
    End Sub
#End Region
#Region "Properties"
    Public Property SplitType() As SplitTypes
        Get
            Return m_SplitType
        End Get
        Set(ByVal Value As SplitTypes)
            m_SplitType = Value
        End Set
    End Property

    Public Property SplitSize() As Long
        Get
            Return m_SplitSize
        End Get
        Set(ByVal Value As Long)
            m_SplitSize = Value
        End Set
    End Property

    Public ReadOnly Property SplitCount() As Long
        Get
            Return m_SplitCount
        End Get
    End Property

    Public ReadOnly Property TotalSplitSizes() As Long
        Get
            Return m_TotalSplitSizes
        End Get
    End Property
#End Region
#Region "Public Methods"
    Public Sub Split(ByVal text_reader As TextReader, ByVal handler As XMLDocumentSplitHandler)
        Dim document_header As String, document_footer As String
        Dim element_counter As Long = 0 : m_SplitCount = 0 : m_TotalSplitSizes = 0
        Dim empty_document As Boolean = False ' First time through is never an empty document
        Dim string_builder As New System.Text.StringBuilder
        Dim xml_text_writer As New XmlTextWriter(New StringWriter(string_builder))
        Dim xml_text_reader As New XmlTextReader(text_reader)
        xml_text_reader.Read() ' prime the pump

        ' Capture all of the items before the first element...
        While xml_text_reader.NodeType <> XmlNodeType.Element
            xml_text_writer.WriteNode(xml_text_reader, True)
        End While

        ' Prepare the document header and footer sections for use later...
        xml_text_writer.WriteStartElement(xml_text_reader.Name)
        xml_text_writer.WriteAttributes(xml_text_reader, True)
        document_header = string_builder.ToString & ">" ' Must close this manually...
        document_footer = vbCrLf & "</" & xml_text_reader.Name & ">" ' Create close element manually...
        xml_text_reader.Read() ' Skip past the root node...

        While Not xml_text_reader.EOF
            ' Only count the nodes that interest us...
            If Not IgnorableNodeType(xml_text_reader.NodeType) Then element_counter += 1 : empty_document = False
            xml_text_writer.WriteNode(xml_text_reader, True) ' Copy everything from the reader to the writer

            If (m_SplitType = SplitTypes.ByDocumentSize And (string_builder.Length - document_header.Length) >= m_SplitSize) OrElse _
                (m_SplitType = SplitTypes.ByElementCount And element_counter >= m_SplitSize) Then
                handler(m_SplitCount, string_builder.ToString & document_footer)
                m_TotalSplitSizes += string_builder.Length : m_SplitCount += 1 : element_counter = 0 ' Adjust the counters
                string_builder.Length = 0 : string_builder.Append(document_header) ' Reset the StringBuilder
                empty_document = True ' It is an empty document again...
            End If
        End While

        If Not empty_document Then
            'handler(m_SplitCount, string_builder.ToString & document_footer)
            handler(m_SplitCount, string_builder.ToString) 'For the last file, we dont want 2 closing identical rootnodes
            m_TotalSplitSizes += string_builder.Length : m_SplitCount += 1 ' Adjust the counters
        End If
    End Sub
#End Region
#Region "Private Methods"
    Private Function IgnorableNodeType(ByVal node_type As XmlNodeType) As Boolean
        If node_type = XmlNodeType.Whitespace OrElse _
            node_type = XmlNodeType.SignificantWhitespace OrElse _
            node_type = XmlNodeType.EndEntity OrElse _
            node_type = XmlNodeType.EndElement Then
            Return True
        Else
            Return False
        End If
    End Function
#End Region
End Class

Public Delegate Sub XMLDocumentSplitHandler(ByVal count As Long, ByVal document As String)
