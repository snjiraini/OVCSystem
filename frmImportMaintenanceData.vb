Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Math
Imports System.Data.SqlClient
Imports System.Xml
Imports System.IO
Imports System.Text
Imports Ionic.Zip
Imports Ionic.Zlib
Imports SharpCompress.Common
Imports SharpCompress.Archive
Imports System.Configuration
Imports OVCSystem.functions
Public Class frmImportMaintenanceData


    Private Sub btnImportMaintenanceData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImportMaintenanceData.Click
        Try

            'If you click the button when worker is still fetching results, cancel the click event
            If worker.IsBusy = True Then
                MsgBox("Importing Data. Please be patient.", MsgBoxStyle.Exclamation)
            Else
                Progresspanel.Visible = True
                'run the report queries on a different thread to avoid freezing of application
                worker.RunWorkerAsync()
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub worker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles worker.DoWork
        Try
            ' For i = 1 To 10

            If worker.CancellationPending = True Then
                e.Cancel = True
                'Exit For
            Else
                importdata()

            End If
            'Next
        Catch ex As Exception

        End Try

    End Sub

    Private Sub importdata()
        Dim connectionstring As String = ConnectionStrings(SelectedConnectionString).ToString
        Try
            'first, clean up the exports folder
            Dim di As New IO.DirectoryInfo(Application.StartupPath & "\Exports")
            Deletefiles(di)

            'then, we unzip the file sent from partners
            If ExtractZip(txtimportdirectory.Text.ToString, Application.StartupPath & "\Exports") = False Then
                MsgBox("something wrong with Zip file.", MsgBoxStyle.Exclamation)
                Exit Sub
            End If

            ''Delete data from all maintenance tables in preparation for new updated data from HQ
            Dim conn As New SqlConnection(connectionstring)
            Dim cmd As SqlCommand
            'cmd = New SqlCommand("dbo.ClearMaintenanceData")
            'cmd.CommandType = CommandType.StoredProcedure
            'cmd.CommandTimeout = 300
            'conn.Open()
            'cmd.Connection = conn
            'cmd.ExecuteNonQuery()
            'conn.Close()


            'first create stored procedures to create temporary tables
            Dim issuccessful As Boolean = runSQLScriptFiles(Application.StartupPath & "\CreateTempMaintenanceTables.sql")

            'Execute the procedure and create the tables as indicated in CreateTempTables.sql
            If issuccessful = True Then
                cmd = New SqlCommand("dbo.CreateTempMaintenanceTables")
                cmd.CommandType = CommandType.StoredProcedure
                cmd.CommandTimeout = 300
                conn.Open()
                cmd.Connection = conn
                cmd.ExecuteNonQuery()
                conn.Close()
            End If

            'file import
            If ImportfromDirectory(Application.StartupPath & "\Exports") = True Then
                'Actual synchronising from temp tables into main tables
                cmd = New SqlCommand("dbo.SYNCMAINTENANCEDATA")
                cmd.CommandType = CommandType.StoredProcedure
                cmd.CommandTimeout = 3600
                conn.Open()
                cmd.Connection = conn
                cmd.ExecuteNonQuery()
                conn.Close()

                MsgBox("Maintenance Data update SUCCESSFUL.", MsgBoxStyle.Information)
            Else
                MsgBox("Maintenance Data update NOT Successful.")
            End If


        Catch ex As Exception
            MsgBox("Maintenance Data update NOT Successful.")
        End Try
    End Sub

    Private Function runSQLScriptFiles(ByVal scriptfilename As String) As Boolean
        Dim connectionstring As String = ConnectionStrings(SelectedConnectionString).ToString
        Try

            'first create stored procedures to create temporary tables
            Dim cmd As New System.Data.SqlClient.SqlCommand()
            Dim conn As New System.Data.SqlClient.SqlConnection(connectionstring)
            cmd.Connection = conn
            cmd.CommandTimeout = 1800 '20 minutes just incase
            cmd.CommandText = Readscript(scriptfilename)
            conn.Open()
            cmd.ExecuteNonQuery()
            Return True
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation)
            Return False
        End Try
    End Function

    Private Function Readscript(ByVal scriptfile As String) As String
        Try
            Using sr As New StreamReader(scriptfile)
                Return sr.ReadToEnd()
            End Using
        Catch e As Exception
            MsgBox("The file could not be read:" & e.Message, MsgBoxStyle.Critical)
        End Try
    End Function

    Private Sub worker_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles worker.ProgressChanged
        Me.lblProgress.Text = e.ProgressPercentage.ToString() & "%"
    End Sub

    Private Sub worker_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles worker.RunWorkerCompleted
        Try
            Progresspanel.Visible = False
            If e.Cancelled = True Then
                MsgBox("Import Canceled.", MsgBoxStyle.Exclamation)
            ElseIf e.Error IsNot Nothing Then
                MsgBox("Error: " & e.Error.Message, MsgBoxStyle.Critical)
            Else
                MsgBox("Import Successul.", MsgBoxStyle.Information)
            End If

        Catch ex As Exception

        End Try


    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Try
            If worker.WorkerSupportsCancellation = True Then
                worker.CancelAsync()
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub frmImportMaintenanceData_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.WebBrowser1.Url = New System.Uri(Application.StartupPath & "\images\381.gif", System.UriKind.Absolute)
    End Sub

    Private Function ImportfromDirectory(ByVal importdirectory As String) As Boolean
        Dim isautoincreament As Boolean = False
        Try
            'loop through files in the directory
            ' Make a reference to a directory.
            Dim di As New DirectoryInfo(importdirectory)
            ' Get a reference to each file in that directory.
            Dim fiArr As FileInfo() = di.GetFiles("*.xml")
            ' Display the names of the files.
            Dim fri As FileInfo
            Dim filenamebilaextension, destinationxmltable As String
            For Each fri In fiArr
                filenamebilaextension = System.IO.Path.GetFileNameWithoutExtension(fri.Name)
                'remove the digits at the end e.g Clientdetails000 becomes Clientdetails
                destinationxmltable = filenamebilaextension.Substring(0, filenamebilaextension.Length - 12)

                ''if the destination table has an autoincrement field, turn it off to allow identity insert. 
                ''The sqlbulkcopy.keepidentity does not seem to work
                'Dim MyDBAction As New functions
                'Dim SqlAction As String = " Select count(*) from sys.identity_columns where object_name(object_id) = '" & destinationxmltable.ToString & "'"
                'isautoincreament = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)
                'If isautoincreament = True Then
                '    Dim mySqlAction As String = "SET IDENTITY_INSERT [dbo]." & destinationxmltable.ToString & "  OFF"
                '    MyDBAction.DBAction(mySqlAction, functions.DBActionType.Execute)
                'End If

                'import xml data into database
                If bulkcopyxml(importdirectory, filenamebilaextension, destinationxmltable) = False Then
                    MsgBox("Import of " & fri.Name & " FAILED", MsgBoxStyle.Exclamation)
                End If

                ''if the destination table has an autoincrement field, turn it back ON. 
                'If isautoincreament = True Then
                '    Dim mySqlAction As String = "SET IDENTITY_INSERT [dbo]." & destinationxmltable.ToString & "  ON"
                '    MyDBAction.DBAction(mySqlAction, functions.DBActionType.Execute)
                '    isautoincreament = False
                'End If
            Next fri

            Return True

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation)
            Return False
        End Try
    End Function

    Private Function bulkcopyxml(ByVal importdirectory As String, ByVal strfilename As String, _
                                ByVal xmltablename As String) As Boolean
        Dim connectionstring As String = ConnectionStrings(SelectedConnectionString).ToString
        Try
            Using sqlconn As New SqlConnection(connectionstring)
                Dim ds As New DataSet()
                Dim sourcedata As New DataTable()

                '' ''try to enforce table schema to enforce datatypes
                '' '' Set the file path and name. Modify this for your purposes.
                ' ''Dim filename As String = "I:\APHIA\OVCSystem Nakuru\OVCSystem\OVCSystem\bin\Debug\Schema\ParentDetails.xsd"

                '' '' Create a StreamReader object with the file path and name.
                ' ''Dim readStream As New System.IO.StreamReader(filename)

                '' '' Invoke the ReadXmlSchema method with the StreamReader object.
                ' ''ds.ReadXmlSchema(readStream)


                ds.ReadXml(importdirectory & "\" & strfilename & ".xml")

                sourcedata = ds.Tables(0)

                '' '' '' ''For numeric fields, clone the datatable and convert datatype to numeric before bulkcopy to avoid type mismatch errors
                ' '' '' ''Dim dtCloned As DataTable = sourcedata.Clone()

                '' '' '' ''make first column to be primary key
                ' '' '' ''dtCloned.PrimaryKey = New DataColumn() {dtCloned.Columns(0)}

                ' '' '' ''Dim c = 0
                ' '' '' ''For c = 0 To dtCloned.Columns.Count - 1
                ' '' '' ''    Dim field_isstring As Boolean = False

                ' '' '' ''    For r = 0 To sourcedata.Rows.Count - 1

                ' '' '' ''        If IsNumeric(sourcedata.Rows(r).Item(c)) = True Or IsDBNull(sourcedata.Rows(r).Item(c)) = True Then
                ' '' '' ''            If field_isstring = True Then
                ' '' '' ''                field_isstring = field_isstring 'type string remains as the column type
                ' '' '' ''            ElseIf dtCloned.Columns(c).ColumnName.ToLower = "clustercbos" Then 'for some reason, isnumeric("45,25") returns true. Had to enforce column name to never be see as numeric
                ' '' '' ''                field_isstring = True
                ' '' '' ''            Else
                ' '' '' ''                field_isstring = False
                ' '' '' ''            End If
                ' '' '' ''        Else
                ' '' '' ''            field_isstring = True
                ' '' '' ''        End If

                ' '' '' ''        'r = r + 1
                ' '' '' ''    Next r

                ' '' '' ''    'After going thru all items in this column, we finally set the datatype coz we now know if it has strings or numerics
                ' '' '' ''    If field_isstring = True Then
                ' '' '' ''        dtCloned.Columns(c).DataType = GetType(String)
                ' '' '' ''    Else
                ' '' '' ''        dtCloned.Columns(c).DataType = GetType(Int32)
                ' '' '' ''    End If
                ' '' '' ''    'c = c + 1
                ' '' '' ''Next c



                '' '' '' ''import sourcedata to the cloned table with correct datatypes
                ' '' '' ''For Each row As DataRow In sourcedata.Rows
                ' '' '' ''    dtCloned.ImportRow(row)
                ' '' '' ''Next

                '' '' '' ''clear the original sourcedata table
                ' '' '' ''sourcedata.Clear()

                '' '' '' ''put the corrected cloned table back into the original source table
                ' '' '' ''sourcedata = dtCloned

                '' '' '' ''For i = 0 To sourcedata.Columns.Count - 1
                '' '' '' ''    MsgBox(xmltablename & " " & sourcedata.Columns(i).ColumnName + " " + sourcedata.Columns(i).DataType.Name)
                '' '' '' ''    i = i + 1
                '' '' '' ''Next


                sqlconn.Open()
                Using bulkcopy As New SqlBulkCopy(connectionstring, SqlBulkCopyOptions.TableLock _
                                                  And SqlBulkCopyOptions.UseInternalTransaction _
                                                  And SqlBulkCopyOptions.KeepNulls And SqlBulkCopyOptions.KeepIdentity)
                    bulkcopy.BulkCopyTimeout = 600
                    bulkcopy.BatchSize = 1000
                    bulkcopy.DestinationTableName = "dbo.temp_" & xmltablename & ""
                    bulkcopy.ColumnMappings.Clear()
                    For Each myCol In sourcedata.Columns
                        'dont map Rowid coz it does not exist in db
                        If myCol.ColumnName.Trim().ToString.ToLower <> "rowid" _
                        AndAlso myCol.ColumnName.Trim().ToString.ToLower <> "syncdate" Then

                            'when a field has same name as table, errors come up. So servicestatus comes as status, but needs to be mapped back to servicestatus
                            'If myCol.ColumnName.Trim().ToString.ToLower = "status" Then
                            '    bulkcopy.ColumnMappings.Add(myCol.ColumnName.Trim(), "ServiceStatus")
                            'ElseIf myCol.ColumnName.Trim().ToString.ToLower = "district_name" Then
                            '    bulkcopy.ColumnMappings.Add(myCol.ColumnName.Trim(), "district")
                            'Else
                            bulkcopy.ColumnMappings.Add(myCol.ColumnName.Trim(), myCol.ColumnName.Trim())
                            'End If


                        End If

                    Next



                    'bulkcopy.ColumnMappings.Add("ParentId", "ParentId")
                    'bulkcopy.ColumnMappings.Add("IDNumber", "IDNumber")
                    bulkcopy.WriteToServer(sourcedata)

                End Using
            End Using


            Return True
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation)
            Return False
        End Try
    End Function

    Private Sub cmdbrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdbrowse.Click
        OpenFileDialog1.Title = "Please Select a File"
        'OpenFileDialog1.InitialDirectory = "C:OVC"

        OpenFileDialog1.ShowDialog()

    End Sub

    Private Sub OpenFileDialog1_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk

        'Dim strm As System.IO.Stream
        'strm = OpenFileDialog1.OpenFile()
        txtimportdirectory.Text = OpenFileDialog1.FileName.ToString()

    End Sub

    Private Function ExtractZip(ByVal zippedfile As String, ByVal extractlocation As String) As Boolean
        Try
            'Dim archive = ArchiveFactory.Open(zippedfile)
            'For Each entry In archive.Entries


            '    If Not entry.IsDirectory Then

            '        entry.WriteToDirectory(extractlocation, ExtractOptions.ExtractFullPath Or ExtractOptions.Overwrite)
            '    End If

            'Next
            Dim ZipToUnpack As String = zippedfile '"C1P3SML.zip"
            Dim UnpackDirectory As String = extractlocation '"Extracted Files"
            Using zip1 As ZipFile = ZipFile.Read(ZipToUnpack)
                Dim e As ZipEntry
                ' here, we extract every entry, but we could extract conditionally,
                ' based on entry name, size, date, checkbox status, etc.   
                For Each e In zip1
                    e.Extract(UnpackDirectory, ExtractExistingFileAction.OverwriteSilently)
                Next
            End Using


            Return True
        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try

    End Function

    Private Sub Deletefiles(ByVal di As IO.DirectoryInfo)
        Dim fi As IO.FileInfo
        Try


            Dim aryFi As IO.FileInfo() = di.GetFiles("*.*")
            For Each fi In aryFi

                fi.Delete()
            Next


        Catch ex As Exception
            MsgBox(ex.Message)

        End Try
    End Sub
End Class