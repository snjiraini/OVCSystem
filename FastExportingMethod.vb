Imports System.Text
Imports Microsoft.Office.Interop.Excel

Module FastExportingMethod

    Public Sub ExportToExcel(ByVal dataSet As DataSet, ByVal outputPath As String, ByVal templatePath As String)
        Try

            ' Create the Excel Application object
            Dim excelApp As New ApplicationClass()

            ' Create a new Excel Workbook

            'Dim excelWorkbook As Workbook = excelApp.Workbooks.Add(Type.Missing)

            Dim sheetIndex As Double = 0
            Dim col As Integer
            Dim row As Decimal
            Dim excelSheet As Worksheet

            Dim excelWorkbook As Workbook = excelApp.Workbooks.Open(templatePath)
            excelSheet = excelWorkbook.Worksheets(1)

            ' Copy each DataTable as a new Sheet
            For Each dt As System.Data.DataTable In dataSet.Tables

                sheetIndex += 1

                ' Copy the DataTable to an object array
                ''Dim rawData(dt.Rows.Count, dt.Columns.Count - 1) As Object
                Dim rawData(dt.Rows.Count, dt.Columns.Count - 1) As Object

                ' Copy the column names to the first row of the object array
                For col = 0 To dt.Columns.Count - 1
                    rawData(0, col) = dt.Columns(col).ColumnName
                Next

                ' Copy the values to the object array
                'modified to also remove any special characters which give an error when presenting the data on excel

                For col = 0 To dt.Columns.Count - 1
                    For row = 0 To dt.Rows.Count - 1
                        'rawData(row + 1, col) = RegularExpressions.Regex.Replace(dt.Rows(row).ItemArray(col).ToString, "[^0-9a-zA-Z]", "") 'dt.Rows(row).ItemArray(col)
                        rawData(row + 1, col) = dt.Rows(row).ItemArray(col)
                    Next
                Next

                ' Calculate the final column letter
                Dim finalColLetter As String = String.Empty
                Dim colCharset As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
                Dim colCharsetLen As Integer = colCharset.Length

                If dt.Columns.Count > colCharsetLen Then
                    finalColLetter = colCharset.Substring( _
                     (dt.Columns.Count - 1) \ colCharsetLen - 1, 1)
                End If

                finalColLetter += colCharset.Substring( _
                  (dt.Columns.Count - 1) Mod colCharsetLen, 1)

                '' Create a new Sheet
                'excelSheet = CType( _
                '    excelWorkbook.Sheets.Add(excelWorkbook.Sheets(sheetIndex), _
                '    Type.Missing, 1, XlSheetType.xlWorksheet), Worksheet)

                'excelSheet.Name = dt.TableName

                ' Fast data export to Excel
                Dim excelRange As String = String.Format("A1:{0}{1}", finalColLetter, dt.Rows.Count + 1)
                excelSheet.Range(excelRange, Type.Missing).Value2 = rawData


                ' Mark the first row as BOLD
                CType(excelSheet.Rows(1, Type.Missing), Range).Font.Bold = True

                excelSheet = Nothing
            Next

            ' Save and Close the Workbook
            'excelWorkbook.SaveAs(outputPath, XlFileFormat.xlIntlMacro, Type.Missing, _
            ' Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, _
            ' Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing)
            excelWorkbook.SaveAs(outputPath, XlFileFormat.xlOpenXMLWorkbookMacroEnabled, Type.Missing, _
           Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange, _
           Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing)

            excelWorkbook.Close(True, Type.Missing, Type.Missing)

            excelWorkbook = Nothing

            ' Release the Application object
            excelApp.Quit()
            excelApp = Nothing

            ' Collect the unreferenced objects
            GC.Collect()
            GC.WaitForPendingFinalizers()

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

End Module
