Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Data
Imports System.Data.SqlClient
Imports System.Xml
Imports SQLXMLBULKLOADLib


Namespace ClsXMLBulkCopy
    Class Items

        Private Shared Sub Main(ByVal args As String())
            Try
                Dim objBL As New SQLXMLBULKLOADLib.SQLXMLBulkLoad()
                objBL.ConnectionString = "provider=SQLOLEDB.1;data source=SQL;database=DATABASE;integrated security=SSPI"
                objBL.ErrorLogFile = "error.xml"
                objBL.KeepIdentity = False
                objBL.Execute("F:\temp\Testxml.xsd", "F:\temp\Testxml.xml")

            Catch e As Exception
                Console.WriteLine(e.ToString())
            End Try
        End Sub
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik, @toddanglin
'Facebook: facebook.com/telerik
'=======================================================

