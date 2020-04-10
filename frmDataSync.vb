Imports OVCSystem.functions
Imports System.Data.Odbc
Imports System.Configuration.ConfigurationManager
Imports System.Math
Imports System.Data.SqlClient
Imports System.Xml
'Imports SQLXMLBULKLOADLib
Imports System.IO
Imports System.Text
Imports Ionic.Zip
Imports Ionic.Zlib
'Imports SharpCompress.Common
'Imports SharpCompress.Archive
Imports System.Configuration
Imports System.ComponentModel
Imports Excel
Imports System.Reflection
Imports System.Text.RegularExpressions
Imports CPIMS.Net.OLMIS.Core.Services

Public Class frmDataSync

    Private m_XMLSplitter As New XMLDocumentSplitter()
    Private Sub btnExportData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExportData.Click


        'If you click the button when worker is still fetching results, cancel the click event
        If Exportworker.IsBusy = True Then
            MsgBox("Fetching results. Please be patient.", MsgBoxStyle.Exclamation)
        Else

            Me.WebBrowser1.Url = New System.Uri(Application.StartupPath & "\images\289.gif", System.UriKind.Absolute)
            ProgressPanel.Visible = True
            'run the report queries on a different thread to avoid freezing of application
            Exportworker.RunWorkerAsync()
        End If




    End Sub

    Private Sub worker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles Exportworker.DoWork
        Dim connectionstring As String = ConnectionStrings(SelectedConnectionString).ToString

        'if we are redoing exports, nullify syncdates
        If Panel2.Visible = True Then
            strReportProgress = "Undoing SyncDate Timestamps"
            Exportworker.ReportProgress(0)

            'Identical to the normal export, only that we first run procedure to undo syncdates updated within a given period to allow re-exporting
            'Execute the procedure and create the tables as indicated in CreateTempTables.sql
            Dim conn As New SqlConnection(connectionstring)
            Dim cmd As SqlCommand

            cmd = New SqlCommand("dbo.NullifySyncDates")
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandTimeout = 3600000
            cmd.Parameters.Add(New SqlParameter("@startdate", dtpFrom.Value))
            cmd.Parameters.Add(New SqlParameter("@enddate", dtpTo.Value))
            conn.Open()
            cmd.Connection = conn
            'cmd.ExecuteReader()
            cmd.ExecuteNonQuery()
            conn.Close()
        End If


        ExportData()
    End Sub

    Private Sub worker_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles Exportworker.ProgressChanged
        Label2.Text = strReportProgress
    End Sub

    Private Sub worker_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles Exportworker.RunWorkerCompleted
        ProgressPanel.Visible = False
        Panel1.Visible = False
        Panel2.Visible = False
        Label2.Text = "----------"
        Label4.Text = "----------"
    End Sub

    Private Sub ExportData()
        Try
            'Check to see if Export has NEVER been done before. 
            Dim MyDBAction As New functions

            If NormalExport() = True Then
                ZipExports(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"), My.Computer.Name)
            End If

        Catch ex As Exception
            MsgBox("Data Export FAILED.", MsgBoxStyle.Exclamation)
        End Try
    End Sub

    Private Sub ZipExports(ByVal ExportsDirectory As String, ByVal zipname As String)
        Try
            ''Split file incase it is big
            Using zip As New ZipFile()
                ' add all those files to the  folder in the zip file
                zip.AddDirectory(ExportsDirectory)

                ''we want to extract straight into the /Exports folder and ditch the dated-folder
                'Dim di As New DirectoryInfo(ExportsDirectory)
                'Dim strFileSize As String = ""
                'Dim fi As IO.FileInfo

                'Dim aryFi As IO.FileInfo() = di.GetFiles("*.*")
                'For Each fi In aryFi
                '    'add the individual files into the zip
                '    zip.AddFile(fi.FullName)
                'Next

                zip.Comment = "This zip was created at " & System.DateTime.Now.ToString("G")
                zip.MaxOutputSegmentSize = 2 * 1024 * 1024   '' 2mb
                zip.Save(zipname & "-" & Format(Date.Now, "dd-MMM-yyyy") & ".zip")
                MsgBox(zip.NumberOfSegmentsForMostRecentSave & " Export files generated, all starting with the names: " &
                    My.Computer.Name & "-" & Format(Date.Now, "dd-MMM-yyyy") & ".zip,.z01,.z02 e.t.c")
            End Using


            'hyperlink to new folder with exported files
            'Label4.Text = "Files exported to: " & My.Computer.Name & "-" & Format(Date.Now, "dd-MMM-yyyy") & ".zip"

            'MsgBox("Data Export Zipping Successful.", MsgBoxStyle.Information)

        Catch ex As Exception
            MsgBox("Data Exports Zipping NOT successful.", MsgBoxStyle.Exclamation)
        End Try

    End Sub

    'Private Function HQExports(ByVal cbolist As String) As Boolean
    '    Try

    '        'create a folder for today's exports
    '        ' Determine whether the directory exists.
    '        If Directory.Exists(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy")) = False Then
    '            Dim di As DirectoryInfo = Directory.CreateDirectory(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"))
    '        Else
    '            'delete it
    '            My.Computer.FileSystem.DeleteDirectory(Application.StartupPath & "\Exports\" & _
    '            Format(Date.Now, "dd-MMM-yyyy"), FileIO.DeleteDirectoryOption.DeleteAllContents)

    '            'then create it
    '            Dim di As DirectoryInfo = Directory.CreateDirectory(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"))
    '        End If

    '        '1. export clientdetails
    '        Dim mySqlAction As String = "select * from clientdetails where syncdate between ' " & _
    '        Format(dtpFrom.Value, "dd-MMM-yyyy") & "' and '" & Format(dtpTo.Value, "dd-MMM-yyyy") & "' " & _
    '        " and cbo in (" & cbolist & ")" & _
    '                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('ClientDetails')"
    '        Dim MyDBAction As New functions
    '        Dim MyDatable As New Data.DataTable
    '        MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
    '        'only write data if the characters can be counted. sio blank space
    '        If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '            ' convert string to stream
    '            Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '            Dim stream As New MemoryStream(byteArray)


    '            m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '            m_XMLSplitter.SplitSize = CType("7000", Long)

    '            m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '            renamesplitfiles("clientdetails")
    '            'WriteToFile(MyDatable.Rows(0).Item(0).ToString, Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"), _
    '            '"\clientdetails.xml")
    '        End If

    '        '2. export clientlongitudinaldetails
    '        mySqlAction = "select * from clientlongitudinaldetails where syncdate between ' " & _
    '        Format(dtpFrom.Value, "dd-MMM-yyyy") & "' and '" & Format(dtpTo.Value, "dd-MMM-yyyy") & "' " & _
    '        " and cbo in (" & cbolist & ")" & _
    '                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('clientlongitudinaldetails')"
    '        MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
    '        'only write data if the characters can be counted. sio blank space
    '        If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '            ' convert string to stream
    '            Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '            Dim stream As New MemoryStream(byteArray)


    '            m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '            m_XMLSplitter.SplitSize = CType("7000", Long)

    '            m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '            renamesplitfiles("clientlongitudinaldetails")
    '            'WriteToFile(MyDatable.Rows(0).Item(0).ToString, Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"), _
    '            '"\clientlongitudinaldetails.xml")
    '        End If

    '        '3. export NeedsAssessment
    '        mySqlAction = "SELECT NeedsAssessment.NeedsRowID, NeedsAssessment.NeedsAssessmentID, NeedsAssessment.OVCID, " & _
    '        " NeedsAssessment.DateofAssessment,NeedsAssessment.DomainID, NeedsAssessment.SubdomainID, NeedsAssessment.GoalID, " & _
    '        " NeedsAssessment.SyncDate FROM NeedsAssessment INNER JOIN " & _
    '        " Clientdetails ON NeedsAssessment.OVCID = Clientdetails.OVCID " & _
    '        " where NeedsAssessment.syncdate between ' " & _
    '        Format(dtpFrom.Value, "dd-MMM-yyyy") & "' and '" & Format(dtpTo.Value, "dd-MMM-yyyy") & "' " & _
    '        " and Clientdetails.cbo in (" & cbolist & ")" & _
    '                                   " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('NeedsAssessment')"
    '        MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
    '        'only write data if the characters can be counted. sio blank space
    '        If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '            ' convert string to stream
    '            Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '            Dim stream As New MemoryStream(byteArray)


    '            m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '            m_XMLSplitter.SplitSize = CType("7000", Long)

    '            m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '            renamesplitfiles("NeedsAssessment")
    '            'WriteToFile(MyDatable.Rows(0).Item(0).ToString, Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"), _
    '            '"\NeedsAssessment.xml")
    '        End If

    '        '4. export NeedsAssessmentMain
    '        mySqlAction = "SELECT NeedsAssessmentMain.NeedsAssessmentID, NeedsAssessmentMain.OVCID, " & _
    '        " NeedsAssessmentMain.DateofAssessment, NeedsAssessmentMain.SyncDate" & _
    '        " FROM NeedsAssessmentMain INNER JOIN " & _
    '        " Clientdetails ON NeedsAssessmentMain.OVCID = Clientdetails.OVCID " & _
    '        " where NeedsAssessmentMain.syncdate between ' " & _
    '        Format(dtpFrom.Value, "dd-MMM-yyyy") & "' and '" & Format(dtpTo.Value, "dd-MMM-yyyy") & "' " & _
    '        " and Clientdetails.cbo in (" & cbolist & ")" & _
    '                                  " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('NeedsAssessmentMain')"
    '        MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
    '        'only write data if the characters can be counted. sio blank space
    '        If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '            ' convert string to stream
    '            Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '            Dim stream As New MemoryStream(byteArray)


    '            m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '            m_XMLSplitter.SplitSize = CType("7000", Long)

    '            m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '            renamesplitfiles("needsassessmentmain")
    '            'WriteToFile(MyDatable.Rows(0).Item(0).ToString, Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"), _
    '            '"\NeedsAssessmentMain.xml")
    '        End If

    '        '5. export PriorityMonitoring
    '        mySqlAction = "SELECT PriorityMonitoring.PriorityMonitoringid, PriorityMonitoring.SSVID, " & _
    '        " PriorityMonitoring.PriorityID, PriorityMonitoring.SyncDate " & _
    '        " FROM  PriorityMonitoring INNER JOIN " & _
    '        " StatusAndServiceVisit ON PriorityMonitoring.SSVID = StatusAndServiceVisit.SSVID INNER JOIN " & _
    '        " Clientdetails ON StatusAndServiceVisit.OVCID = Clientdetails.OVCID " & _
    '        " where PriorityMonitoring.syncdate between ' " & _
    '        Format(dtpFrom.Value, "dd-MMM-yyyy") & "' and '" & Format(dtpTo.Value, "dd-MMM-yyyy") & "' " & _
    '        " and Clientdetails.cbo in (" & cbolist & ")" & _
    '                                 " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('PriorityMonitoring')"
    '        MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
    '        'only write data if the characters can be counted. sio blank space
    '        If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '            ' convert string to stream
    '            Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '            Dim stream As New MemoryStream(byteArray)


    '            m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '            m_XMLSplitter.SplitSize = CType("7000", Long)

    '            m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '            renamesplitfiles("prioritymonitoring")
    '            'WriteToFile(MyDatable.Rows(0).Item(0).ToString, Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"), _
    '            '"\PriorityMonitoring.xml")
    '        End If

    '        '6. export PriorityNeeds
    '        mySqlAction = "SELECT PriorityNeeds.PriorityNeedsID, PriorityNeeds.NeedsAssessmentID, " & _
    '        " PriorityNeeds.PriorityID, PriorityNeeds.SyncDate " & _
    '        " FROM PriorityNeeds INNER JOIN NeedsAssessmentMain " & _
    '        " ON PriorityNeeds.NeedsAssessmentID = NeedsAssessmentMain.NeedsAssessmentID INNER JOIN " & _
    '        " Clientdetails ON NeedsAssessmentMain.OVCID = Clientdetails.OVCID " & _
    '        " where PriorityNeeds.syncdate between ' " & _
    '        Format(dtpFrom.Value, "dd-MMM-yyyy") & "' and '" & Format(dtpTo.Value, "dd-MMM-yyyy") & "' " & _
    '        " and Clientdetails.cbo in (" & cbolist & ")" & _
    '                               " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('PriorityNeeds')"
    '        MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
    '        'only write data if the characters can be counted. sio blank space
    '        If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '            ' convert string to stream
    '            Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '            Dim stream As New MemoryStream(byteArray)


    '            m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '            m_XMLSplitter.SplitSize = CType("7000", Long)

    '            m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '            renamesplitfiles("priorityneeds")
    '            'WriteToFile(MyDatable.Rows(0).Item(0).ToString, Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"), _
    '            '"\PriorityNeeds.xml")
    '        End If


    '        '7. export ServicesProvided
    '        mySqlAction = "SELECT ServicesProvided.SPID, ServicesProvided.NeedsAssessmentId, " & _
    '        " ServicesProvided.Domainid, ServicesProvided.SSID, ServicesProvided.Providerid, " & _
    '        " ServicesProvided.SyncDate FROM ServicesProvided INNER JOIN NeedsAssessmentMain " & _
    '        " ON ServicesProvided.NeedsAssessmentId = NeedsAssessmentMain.NeedsAssessmentID INNER JOIN " & _
    '        " Clientdetails ON NeedsAssessmentMain.OVCID = Clientdetails.OVCID" & _
    '        " where ServicesProvided.syncdate between ' " & _
    '        Format(dtpFrom.Value, "dd-MMM-yyyy") & "' and '" & Format(dtpTo.Value, "dd-MMM-yyyy") & "' " & _
    '        " and Clientdetails.cbo in (" & cbolist & ")" & _
    '                              " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('ServicesProvided')"
    '        MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
    '        'only write data if the characters can be counted. sio blank space
    '        If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '            ' convert string to stream
    '            Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '            Dim stream As New MemoryStream(byteArray)


    '            m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '            m_XMLSplitter.SplitSize = CType("7000", Long)

    '            m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '            renamesplitfiles("servicesprovided")
    '            'WriteToFile(MyDatable.Rows(0).Item(0).ToString, Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"), _
    '            '"\ServicesProvided.xml")
    '        End If

    '        '8. export StatusandServiceMonitoring
    '        mySqlAction = "SELECT StatusAndServiceMonitoring.SSMID, StatusAndServiceMonitoring.SSVID, " & _
    '        " StatusAndServiceMonitoring.Month, StatusAndServiceMonitoring.SSID,  " & _
    '        " StatusAndServiceMonitoring.SyncDate  " & _
    '        " FROM StatusAndServiceMonitoring INNER JOIN StatusAndServiceVisit  " & _
    '        " ON StatusAndServiceMonitoring.SSVID = StatusAndServiceVisit.SSVID INNER JOIN  " & _
    '        " Clientdetails ON StatusAndServiceVisit.OVCID = Clientdetails.OVCID " & _
    '        " where StatusAndServiceMonitoring.syncdate between ' " & _
    '        Format(dtpFrom.Value, "dd-MMM-yyyy") & "' and '" & Format(dtpTo.Value, "dd-MMM-yyyy") & "' " & _
    '        " and Clientdetails.cbo in (" & cbolist & ")" & _
    '                            " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('StatusandServiceMonitoring')"
    '        MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
    '        'only write data if the characters can be counted. sio blank space
    '        If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '            ' convert string to stream
    '            Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '            Dim stream As New MemoryStream(byteArray)


    '            m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '            m_XMLSplitter.SplitSize = CType("7000", Long)

    '            m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '            renamesplitfiles("statusandservicemonitoring")
    '            'WriteToFile(MyDatable.Rows(0).Item(0).ToString, Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"), _
    '            '"\StatusandServiceMonitoring.xml")
    '        End If

    '        '9. export StatusandServiceVisit
    '        mySqlAction = "SELECT StatusAndServiceVisit.SSVID, StatusAndServiceVisit.OVCID, " & _
    '        " StatusAndServiceVisit.Guardianid, StatusAndServiceVisit.Parentid,  " & _
    '        " StatusAndServiceVisit.CHW, StatusAndServiceVisit.DateofVisit, " & _
    '        " StatusAndServiceVisit.VisitType, StatusAndServiceVisit.HouseHoldVisits, " & _
    '        " StatusAndServiceVisit.SyncDate FROM StatusAndServiceVisit INNER JOIN " & _
    '        " Clientdetails ON StatusAndServiceVisit.OVCID = Clientdetails.OVCID" & _
    '          " where StatusAndServiceVisit.syncdate between ' " & _
    '        Format(dtpFrom.Value, "dd-MMM-yyyy") & "' and '" & Format(dtpTo.Value, "dd-MMM-yyyy") & "' " & _
    '        " and Clientdetails.cbo in (" & cbolist & ")" & _
    '                           " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('StatusandServiceVisit')"
    '        MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
    '        'only write data if the characters can be counted. sio blank space
    '        If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '            ' convert string to stream
    '            Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '            Dim stream As New MemoryStream(byteArray)


    '            m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '            m_XMLSplitter.SplitSize = CType("7000", Long)

    '            m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '            renamesplitfiles("statusandservicevisit")
    '            'WriteToFile(MyDatable.Rows(0).Item(0).ToString, Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"), _
    '            '"\StatusandServiceVisit.xml")
    '        End If

    '        '' ''10. export ParentDetails
    '        ' ''mySqlAction = "select  * from ParentDetails where Parentid not in " & _
    '        ' ''                            " (select Parentid from tempKeys_parentid) " & _
    '        ' ''                   " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('ParentDetails')"
    '        ' ''MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
    '        '' ''only write data if the characters can be counted. sio blank space
    '        ' ''If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '        ' ''    ' convert string to stream
    '        ' ''    Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '        ' ''    Dim stream As New MemoryStream(byteArray)


    '        ' ''    m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '        ' ''    m_XMLSplitter.SplitSize = CType("7000", Long)

    '        ' ''    m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '        ' ''    renamesplitfiles("parentdetails")
    '        ' ''    'WriteToFile(MyDatable.Rows(0).Item(0).ToString, Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"), _
    '        ' ''    '"\ParentDetails.xml")
    '        ' ''End If

    '        '11. export CHW
    '        mySqlAction = "SELECT CHW.CHWID, CHW.FirstName, CHW.MiddleName, CHW.Surname, CHW.ID, " & _
    '        " CHW.CBOID, CHW.IsActive, CHW.createdon, CHW.createdsession, CHW.lastmodifiedon, " & _
    '        " CHW.lastmodifiedsession, CHW.syncdate " & _
    '        " FROM  CHW INNER JOIN " & _
    '        " Clientdetails ON CHW.CHWID = Clientdetails.VolunteerId " & _
    '         " where chw.syncdate between ' " & _
    '        Format(dtpFrom.Value, "dd-MMM-yyyy") & "' and '" & Format(dtpTo.Value, "dd-MMM-yyyy") & "' " & _
    '        " and Clientdetails.cbo in (" & cbolist & ")" & _
    '                           " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('CHW')"
    '        MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
    '        'only write data if the characters can be counted. sio blank space
    '        If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '            ' convert string to stream
    '            Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '            Dim stream As New MemoryStream(byteArray)


    '            m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '            m_XMLSplitter.SplitSize = CType("7000", Long)

    '            m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '            renamesplitfiles("chw")
    '            'WriteToFile(MyDatable.Rows(0).Item(0).ToString, Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"), _
    '            '"\CHW.xml")
    '        End If



    '        MsgBox("Partner Data Export Successful.", MsgBoxStyle.Information)

    '        Return True

    '    Catch ex As Exception
    '        MsgBox("Data Export NOT Successful.", MsgBoxStyle.Exclamation)
    '        Return False

    '    End Try
    'End Function

    'Private Function FirstTimeExport() As Boolean
    '    Try

    '        'create a folder for today's exports
    '        ' Determine whether the directory exists.
    '        If Directory.Exists(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy")) = False Then
    '            Dim di As DirectoryInfo = Directory.CreateDirectory(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"))
    '        Else
    '            'delete it
    '            My.Computer.FileSystem.DeleteDirectory(Application.StartupPath & "\Exports\" & _
    '            Format(Date.Now, "dd-MMM-yyyy"), FileIO.DeleteDirectoryOption.DeleteAllContents)

    '            'then create it
    '            Dim di As DirectoryInfo = Directory.CreateDirectory(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"))
    '        End If


    '        Dim loop_counter As Integer = 0
    '        Dim MyDBAction As New functions
    '        Dim MyDatable As New Data.DataTable
    '        Dim mySqlAction As String

    '        '1. export clientdetails
    '        Dim sqlscalarAction As String = ""
    '        Dim count_scalar, num_loops As Double
    '        Dim record_range_count As Double = 200000

    '        'a. Count number of records to export
    '        sqlscalarAction = " select count (*) from " & _
    '                        " (select * from clientdetails where (SyncDate IS NULL " & _
    '                        " OR  Lastmodifiedon > isnull(syncdate,'1900-01-01 00:00:00.000')) " & _
    '                        " union " & _
    '                        " select * from clientdetails where " & _
    '                        " ovcid not in (select ovcid from tempKeys_OVCID)) tbl"
    '        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

    '        'b. Decide how many times to loop
    '        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

    '        For i = 0 To num_loops - 1
    '            'with every loop, increment the loop counter by 1
    '            loop_counter = loop_counter + 1

    '            'calculate range 
    '            Dim start_record As Double
    '            Dim last_record As Double
    '            If loop_counter = 1 Then
    '                start_record = 0
    '                last_record = record_range_count
    '            Else
    '                start_record = (record_range_count * i) + 1
    '                last_record = (record_range_count * loop_counter)
    '            End If

    '            'fetch records within specific range
    '            mySqlAction = "select * from (select *, ROW_NUMBER() over (order by ovcId asc) as RowId " & _
    '                                         " from clientdetails where (SyncDate IS NULL  " & _
    '                                        " OR  Lastmodifiedon > isnull(syncdate,'1900-01-01 00:00:00.000'))  " & _
    '                                        "union" & _
    '                                " select *, ROW_NUMBER() over (order by ovcId asc) as RowId  " & _
    '                                             " from clientdetails where   " & _
    '                                " ovcid not in (select ovcid from tempKeys_OVCID)) clientdetails " & _
    '                              " where RowId between " & start_record & " and " & last_record & " " & _
    '                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('ClientDetails')"
    '            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

    '            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '                ' convert string to stream
    '                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '                Dim stream As New MemoryStream(byteArray)


    '                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '                m_XMLSplitter.SplitSize = CType("1000", Long)

    '                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '                renamesplitfiles("clientdetails")

    '            End If
    '        Next i
    '        loop_counter = 0 'reinitialize counter before next table


    '        '2. export clientlongitudinaldetails
    '        'a. Count number of records to export
    '        sqlscalarAction = " select count (*) from " & _
    '                        " (select * from clientlongitudinaldetails where (SyncDate IS NULL " & _
    '                        " OR  Lastmodifiedon > isnull(syncdate,'1900-01-01 00:00:00.000')) " & _
    '                        " union " & _
    '                        " select * from clientlongitudinaldetails where " & _
    '                        " ovcid not in (select ovcid from tempKeys_OVCID)) tbl"
    '        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

    '        'b. Decide how many times to loop
    '        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

    '        For i = 0 To num_loops - 1
    '            'with every loop, increment the loop counter by 1
    '            loop_counter = loop_counter + 1

    '            'calculate range 
    '            Dim start_record As Double
    '            Dim last_record As Double
    '            If loop_counter = 1 Then
    '                start_record = 0
    '                last_record = record_range_count
    '            Else
    '                start_record = (record_range_count * i) + 1
    '                last_record = (record_range_count * loop_counter)
    '            End If

    '            'fetch records within specific range
    '            mySqlAction = "select * from (select *, ROW_NUMBER() over (order by ovcId asc) as RowId " & _
    '                                         " from clientlongitudinaldetails where (SyncDate IS NULL  " & _
    '                                        " OR  Lastmodifiedon > isnull(syncdate,'1900-01-01 00:00:00.000'))  " & _
    '                                        "union" & _
    '                                " select *, ROW_NUMBER() over (order by ovcId asc) as RowId  " & _
    '                                             " from clientlongitudinaldetails where   " & _
    '                                " ovcid not in (select ovcid from tempKeys_OVCID)) clientlongitudinaldetails " & _
    '                              " where RowId between " & start_record & " and " & last_record & " " & _
    '                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('clientlongitudinaldetails')"
    '            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

    '            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '                ' convert string to stream
    '                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '                Dim stream As New MemoryStream(byteArray)


    '                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '                m_XMLSplitter.SplitSize = CType("1000", Long)

    '                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '                renamesplitfiles("clientlongitudinaldetails")

    '            End If
    '        Next i
    '        loop_counter = 0

    '        '3. export NeedsAssessment
    '        'a. Count number of records to export
    '        sqlscalarAction = "select  count(*) from NeedsAssessment where NeedsAssessmentID not in " & _
    '        " (select NeedsAssessmentID from tempKeys_NeedsAssessmentID)"
    '        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

    '        'b. Decide how many times to loop
    '        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

    '        For i = 0 To num_loops - 1
    '            'with every loop, increment the loop counter by 1
    '            loop_counter = loop_counter + 1

    '            'calculate range 
    '            Dim start_record As Double
    '            Dim last_record As Double
    '            If loop_counter = 1 Then
    '                start_record = 0
    '                last_record = record_range_count
    '            Else
    '                start_record = (record_range_count * i) + 1
    '                last_record = (record_range_count * loop_counter)
    '            End If

    '            'fetch records within specific range
    '            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by NeedsAssessmentID asc) as RowId  " & _
    '                            " from NeedsAssessment where NeedsAssessmentID not in " & _
    '                            " (select NeedsAssessmentID from tempKeys_NeedsAssessmentID)) NeedsAssessment " & _
    '                            " where RowId between " & start_record & " and " & last_record & " " & _
    '                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('NeedsAssessment')"
    '            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

    '            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '                ' convert string to stream
    '                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '                Dim stream As New MemoryStream(byteArray)


    '                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '                m_XMLSplitter.SplitSize = CType("7000", Long)

    '                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '                renamesplitfiles("NeedsAssessment")

    '            End If
    '        Next i
    '        loop_counter = 0 'reinitialize counter before next table

    '        '4. export NeedsAssessmentMain
    '        'a. Count number of records to export
    '        sqlscalarAction = "select  count(*) from NeedsAssessmentmain where NeedsAssessmentID not in " & _
    '        " (select NeedsAssessmentID from tempKeys_NeedsAssessmentID)"
    '        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

    '        'b. Decide how many times to loop
    '        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

    '        For i = 0 To num_loops - 1
    '            'with every loop, increment the loop counter by 1
    '            loop_counter = loop_counter + 1

    '            'calculate range 
    '            Dim start_record As Double
    '            Dim last_record As Double
    '            If loop_counter = 1 Then
    '                start_record = 0
    '                last_record = record_range_count
    '            Else
    '                start_record = (record_range_count * i) + 1
    '                last_record = (record_range_count * loop_counter)
    '            End If

    '            'fetch records within specific range
    '            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by NeedsAssessmentID asc) as RowId  " & _
    '                            " from NeedsAssessmentmain where NeedsAssessmentID not in " & _
    '                            " (select NeedsAssessmentID from tempKeys_NeedsAssessmentID)) NeedsAssessmentmain " & _
    '                            " where RowId between " & start_record & " and " & last_record & " " & _
    '                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('NeedsAssessmentmain')"
    '            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

    '            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '                ' convert string to stream
    '                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '                Dim stream As New MemoryStream(byteArray)


    '                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '                m_XMLSplitter.SplitSize = CType("7000", Long)

    '                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '                renamesplitfiles("NeedsAssessmentmain")

    '            End If
    '        Next i
    '        loop_counter = 0 'reinitialize counter before next table

    '        '5. export PriorityMonitoring
    '        'a. Count number of records to export
    '        sqlscalarAction = "select  count(*) from PriorityMonitoring where ssvID not in " & _
    '        " (select ssvID from tempKeys_ssvID)"
    '        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

    '        'b. Decide how many times to loop
    '        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

    '        For i = 0 To num_loops - 1
    '            'with every loop, increment the loop counter by 1
    '            loop_counter = loop_counter + 1

    '            'calculate range 
    '            Dim start_record As Double
    '            Dim last_record As Double
    '            If loop_counter = 1 Then
    '                start_record = 0
    '                last_record = record_range_count
    '            Else
    '                start_record = (record_range_count * i) + 1
    '                last_record = (record_range_count * loop_counter)
    '            End If

    '            'fetch records within specific range
    '            mySqlAction = "select * from (select *, ROW_NUMBER() over (order by ssvid asc) as RowId " & _
    '                                 " from PriorityMonitoring where ssvid not in " & _
    '                                " (select ssvid from tempKeys_ssvid)) PriorityMonitoring " & _
    '                                " where RowId between " & start_record & " and " & last_record & "" & _
    '                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('PriorityMonitoring')"
    '            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

    '            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '                ' convert string to stream
    '                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '                Dim stream As New MemoryStream(byteArray)


    '                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '                m_XMLSplitter.SplitSize = CType("7000", Long)

    '                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '                renamesplitfiles("PriorityMonitoring")

    '            End If
    '        Next i
    '        loop_counter = 0 'reinitialize counter before next table

    '        '6. export PriorityNeeds
    '        'a. Count number of records to export
    '        sqlscalarAction = "select  count(*) from PriorityNeeds where NeedsAssessmentID not in " & _
    '                        " (select NeedsAssessmentID from tempKeys_NeedsAssessmentID)"
    '        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

    '        'b. Decide how many times to loop
    '        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

    '        For i = 0 To num_loops - 1
    '            'with every loop, increment the loop counter by 1
    '            loop_counter = loop_counter + 1

    '            'calculate range 
    '            Dim start_record As Double
    '            Dim last_record As Double
    '            If loop_counter = 1 Then
    '                start_record = 0
    '                last_record = record_range_count
    '            Else
    '                start_record = (record_range_count * i) + 1
    '                last_record = (record_range_count * loop_counter)
    '            End If

    '            'fetch records within specific range
    '            mySqlAction = "select * from (select  * , ROW_NUMBER() over (order by NeedsAssessmentID asc) as RowId " & _
    '                            " from PriorityNeeds where NeedsAssessmentID not in " & _
    '                            " (select NeedsAssessmentID from tempKeys_NeedsAssessmentID)) PriorityNeeds " & _
    '                                " where RowId between " & start_record & " and " & last_record & "" & _
    '                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('PriorityNeeds')"
    '            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

    '            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '                ' convert string to stream
    '                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '                Dim stream As New MemoryStream(byteArray)


    '                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '                m_XMLSplitter.SplitSize = CType("7000", Long)

    '                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '                renamesplitfiles("PriorityNeeds")

    '            End If
    '        Next i
    '        loop_counter = 0 'reinitialize counter before next table


    '        '7. export ServicesProvided
    '        'a. Count number of records to export
    '        sqlscalarAction = "select count(*) from ServicesProvided where NeedsAssessmentID not in " & _
    '                        " (select NeedsAssessmentID from tempKeys_NeedsAssessmentID)"
    '        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

    '        'b. Decide how many times to loop
    '        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

    '        For i = 0 To num_loops - 1
    '            'with every loop, increment the loop counter by 1
    '            loop_counter = loop_counter + 1

    '            'calculate range 
    '            Dim start_record As Double
    '            Dim last_record As Double
    '            If loop_counter = 1 Then
    '                start_record = 0
    '                last_record = record_range_count
    '            Else
    '                start_record = (record_range_count * i) + 1
    '                last_record = (record_range_count * loop_counter)
    '            End If

    '            'fetch records within specific range
    '            mySqlAction = "select * from (select * , ROW_NUMBER() over (order by NeedsAssessmentID asc) as RowId " & _
    '                                " from ServicesProvided where NeedsAssessmentID not in " & _
    '                                " (select NeedsAssessmentID from tempKeys_NeedsAssessmentID)) ServicesProvided " & _
    '                                " where RowId between " & start_record & " and " & last_record & "" & _
    '                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('ServicesProvided')"
    '            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

    '            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '                ' convert string to stream
    '                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '                Dim stream As New MemoryStream(byteArray)


    '                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '                m_XMLSplitter.SplitSize = CType("7000", Long)

    '                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '                renamesplitfiles("ServicesProvided")

    '            End If
    '        Next i
    '        loop_counter = 0 'reinitialize counter before next table

    '        '8. export StatusandServiceMonitoring
    '        'a. Count number of records to export
    '        sqlscalarAction = "select  count(*) from StatusandServiceMonitoring where ssvid not in " & _
    '                        " (select ssvid from tempKeys_ssvid)"
    '        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

    '        'b. Decide how many times to loop
    '        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

    '        For i = 0 To num_loops - 1
    '            'with every loop, increment the loop counter by 1
    '            loop_counter = loop_counter + 1

    '            'calculate range 
    '            Dim start_record As Double
    '            Dim last_record As Double
    '            If loop_counter = 1 Then
    '                start_record = 0
    '                last_record = record_range_count
    '            Else
    '                start_record = (record_range_count * i) + 1
    '                last_record = (record_range_count * loop_counter)
    '            End If

    '            'fetch records within specific range
    '            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by ssvid asc) as RowId  " & _
    '                           " from StatusandServiceMonitoring where ssvid not in  " & _
    '                           " (select ssvid from tempKeys_ssvid)) StatusandServiceMonitoring " & _
    '                                " where RowId between " & start_record & " and " & last_record & "" & _
    '                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('StatusandServiceMonitoring')"
    '            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

    '            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '                ' convert string to stream
    '                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '                Dim stream As New MemoryStream(byteArray)


    '                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '                m_XMLSplitter.SplitSize = CType("7000", Long)

    '                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '                renamesplitfiles("StatusandServiceMonitoring")

    '            End If
    '        Next i
    '        loop_counter = 0 'reinitialize counter before next table

    '        '9. export StatusandServiceVisit
    '        'a. Count number of records to export
    '        sqlscalarAction = "select  count(*) from StatusandServiceVisit where ssvid not in " & _
    '                            "(select ssvid from tempKeys_ssvid)"
    '        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

    '        'b. Decide how many times to loop
    '        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

    '        For i = 0 To num_loops - 1
    '            'with every loop, increment the loop counter by 1
    '            loop_counter = loop_counter + 1

    '            'calculate range 
    '            Dim start_record As Double
    '            Dim last_record As Double
    '            If loop_counter = 1 Then
    '                start_record = 0
    '                last_record = record_range_count
    '            Else
    '                start_record = (record_range_count * i) + 1
    '                last_record = (record_range_count * loop_counter)
    '            End If

    '            'fetch records within specific range
    '            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by ssvid asc) as RowId " & _
    '                           " from StatusandServiceVisit where ssvid not in " & _
    '                           " (select ssvid from tempKeys_ssvid)) StatusandServiceVisit " & _
    '                                " where RowId between " & start_record & " and " & last_record & "" & _
    '                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('StatusandServiceVisit')"
    '            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

    '            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '                ' convert string to stream
    '                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '                Dim stream As New MemoryStream(byteArray)


    '                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '                m_XMLSplitter.SplitSize = CType("7000", Long)

    '                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '                renamesplitfiles("StatusandServiceVisit")

    '            End If
    '        Next i
    '        loop_counter = 0 'reinitialize counter before next table


    '        '10. export ParentDetails
    '        'a. Count number of records to export
    '        sqlscalarAction = "select  count(*) from ParentDetails where Parentid not in " & _
    '                                    " (select Parentid from tempKeys_parentid) "
    '        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

    '        'b. Decide how many times to loop
    '        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

    '        For i = 0 To num_loops - 1
    '            'with every loop, increment the loop counter by 1
    '            loop_counter = loop_counter + 1

    '            'calculate range 
    '            Dim start_record As Double
    '            Dim last_record As Double
    '            If loop_counter = 1 Then
    '                start_record = 0
    '                last_record = record_range_count
    '            Else
    '                start_record = (record_range_count * i) + 1
    '                last_record = (record_range_count * loop_counter)
    '            End If

    '            'fetch records within specific range
    '            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by parentid asc) as RowId " & _
    '                            " from ParentDetails where Parentid not in " & _
    '                            " (select Parentid from tempKeys_parentid)) ParentDetails " & _
    '                                " where RowId between " & start_record & " and " & last_record & "" & _
    '                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('ParentDetails')"
    '            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

    '            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '                ' convert string to stream
    '                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '                Dim stream As New MemoryStream(byteArray)


    '                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '                m_XMLSplitter.SplitSize = CType("1000", Long)

    '                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '                renamesplitfiles("ParentDetails")

    '            End If
    '        Next i
    '        loop_counter = 0 'reinitialize counter before next table

    '        '11. export CHW
    '        'a. Count number of records to export
    '        sqlscalarAction = "select  count(*) from CHW where CHWid not in " & _
    '                                   " (select CHWid from tempKeys_CHWid)"
    '        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

    '        'b. Decide how many times to loop
    '        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

    '        For i = 0 To num_loops - 1
    '            'with every loop, increment the loop counter by 1
    '            loop_counter = loop_counter + 1

    '            'calculate range 
    '            Dim start_record As Double
    '            Dim last_record As Double
    '            If loop_counter = 1 Then
    '                start_record = 0
    '                last_record = record_range_count
    '            Else
    '                start_record = (record_range_count * i) + 1
    '                last_record = (record_range_count * loop_counter)
    '            End If

    '            'fetch records within specific range
    '            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by chwid asc) as RowId " & _
    '                            "  from CHW where CHWid not in " & _
    '                                   " (select CHWid from tempKeys_CHWid)) CHW " & _
    '                                " where RowId between " & start_record & " and " & last_record & "" & _
    '                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('CHW')"
    '            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

    '            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '                ' convert string to stream
    '                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '                Dim stream As New MemoryStream(byteArray)


    '                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '                m_XMLSplitter.SplitSize = CType("1000", Long)

    '                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '                renamesplitfiles("CHW")

    '            End If
    '        Next i
    '        loop_counter = 0 'reinitialize counter before next table

    '        '12. export clientcriteria
    '        'a. Count number of records to export
    '        sqlscalarAction = "SELECT count(*) FROM [ClientCriteria] " & _
    '                            " where eligibilitycriteria in " & _
    '                            " ( " & _
    '                            " select clienttype from  " & _
    '                            " (select * from clientdetails where (SyncDate IS NULL " & _
    '                            " OR  Lastmodifiedon > isnull(syncdate,'1900-01-01 00:00:00.000')) " & _
    '                            " union " & _
    '                            " select * from clientdetails where " & _
    '                            " ovcid not in (select ovcid from tempKeys_OVCID)) tbl " & _
    '                            " )"
    '        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

    '        'b. Decide how many times to loop
    '        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

    '        For i = 0 To num_loops - 1
    '            'with every loop, increment the loop counter by 1
    '            loop_counter = loop_counter + 1

    '            'calculate range 
    '            Dim start_record As Double
    '            Dim last_record As Double
    '            If loop_counter = 1 Then
    '                start_record = 0
    '                last_record = record_range_count
    '            Else
    '                start_record = (record_range_count * i) + 1
    '                last_record = (record_range_count * loop_counter)
    '            End If

    '            'fetch records within specific range
    '            mySqlAction = "select * from (SELECT *,ROW_NUMBER() over (order by clientcriteriaid asc) as RowId " & _
    '                            " FROM [ClientCriteria] where eligibilitycriteria in " & _
    '                            " ( " & _
    '                            " select clienttype from  " & _
    '                            " (select * from clientdetails where (SyncDate IS NULL  " & _
    '                            " OR  Lastmodifiedon > isnull(syncdate,'1900-01-01 00:00:00.000')) " & _
    '                            " union " & _
    '                            " select * from clientdetails where " & _
    '                            " ovcid not in (select ovcid from tempKeys_OVCID)) tbl " & _
    '                            " )) ClientCriteria " & _
    '                              " where RowId between " & start_record & " and " & last_record & " " & _
    '                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('clientcriteria')"
    '            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

    '            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
    '                ' convert string to stream
    '                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
    '                Dim stream As New MemoryStream(byteArray)


    '                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
    '                m_XMLSplitter.SplitSize = CType("1000", Long)

    '                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

    '                renamesplitfiles("clientcriteria")

    '            End If
    '        Next i
    '        loop_counter = 0 'reinitialize counter before next table


    '        'show count of exported records---------------
    '        Dim SqlAction As String = ""
    '        Dim clientdetailsnewcount As Double = 0
    '        Dim Clientdetailsupdatecount As Double = 0
    '        Dim ClientdetailsLongitudinalcount As Double = 0
    '        Dim CSIrecordcount As Double = 0
    '        Dim Form1Acount As Double = 0
    '        Dim ParentsCount As Double = 0
    '        Dim ChwCount As Double = 0

    '        'new clients
    '        SqlAction = " select count(ovcid) from clientdetails where  " & _
    '                                    " ovcid not in (select ovcid from tempKeys_OVCID)"
    '        clientdetailsnewcount = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

    '        'updated clients
    '        SqlAction = "select count(ovcid) from clientdetails where Lastmodifiedon > isnull(syncdate,'1900-01-01 00:00:00.000') "
    '        Clientdetailsupdatecount = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

    '        'new CSI entries
    '        SqlAction = "select  count(distinct(NeedsAssessmentID)) from NeedsAssessment where NeedsAssessmentID not in " & _
    '                                    " (select NeedsAssessmentID from tempKeys_NeedsAssessmentID) "
    '        CSIrecordcount = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

    '        'new form1a
    '        SqlAction = "select  count(distinct(ssvid)) from StatusandServiceMonitoring where ssvid not in " & _
    '                                    " (select ssvid from tempKeys_ssvid) "
    '        Form1Acount = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

    '        'new parents
    '        SqlAction = "select  count(parentid) from Parentdetails where parentid not in " & _
    '                                    " (select parentid from tempKeys_parentid) "
    '        ParentsCount = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

    '        'new chw
    '        SqlAction = "select  count(chwid) from chw where chwid not in " & _
    '                                    " (select chwid from tempKeys_chwid) "
    '        ChwCount = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

    '        Label4.Text = "New Children:" & clientdetailsnewcount & "  Updated children:" & Clientdetailsupdatecount & vbCr & _
    '                      "New CSI:" & CSIrecordcount & "   New Form1A:" & Form1Acount & vbCr & _
    '                      "New Parents:" & ParentsCount & "  New CHWs:" & ChwCount

    '        '-----show count of exported records---------------




    '        'hyperlink to new folder with exported files
    '        Label2.Text = "Files exported to: " & Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy")

    '        MsgBox("Data Export Successful." & vbCrLf & " Click [OK] and Wait for Data Zipping...", _
    '               MsgBoxStyle.Information)

    '        Return True

    '    Catch ex As Exception
    '        MsgBox("Data Export NOT Successful.", MsgBoxStyle.Exclamation)
    '        Return False

    '    End Try
    'End Function

    Private Function NormalExport() As Boolean
        'Try
        Dim i As Integer ' simple counter to update progress for background worker

        'create a folder for today's exports
        ' Determine whether the directory exists.
        If Directory.Exists(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy")) = False Then
            Dim di As DirectoryInfo = Directory.CreateDirectory(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"))
        Else
            'delete it
            My.Computer.FileSystem.DeleteDirectory(Application.StartupPath & "\Exports\" &
            Format(Date.Now, "dd-MMM-yyyy"), FileIO.DeleteDirectoryOption.DeleteAllContents)

            'then create it
            Dim di As DirectoryInfo = Directory.CreateDirectory(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"))
        End If

        strReportProgress = "Prepare Exports folder"
        Exportworker.ReportProgress(i + 1)

        Dim loop_counter As Integer = 0
        Dim MyDBAction As New functions
        Dim MyDatable As New Data.DataTable
        Dim mySqlAction As String

        '1. export clientdetails
        Dim sqlscalarAction As String = ""
        Dim count_scalar, num_loops As Double
        Dim record_range_count As Double = 200000


        strReportProgress = "Exporting Clientdetals"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = " select count (*) from " &
                        " (select * from clientdetails) tbl"
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select *, ROW_NUMBER() over (order by ovcId asc) as RowId " &
                                         " from clientdetails ) clientdetails " &
                              " where RowId between " & start_record & " and " & last_record & " " &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('ClientDetails')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("clientdetails")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table



        '2. export clientlongitudinaldetails
        strReportProgress = "Exporting Client Longitudinal details"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = " select count (*) from " &
                        " (select * from clientlongitudinaldetails) tbl"
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select *, ROW_NUMBER() over (order by ovcId asc) as RowId " &
                                         " from clientlongitudinaldetails) clientlongitudinaldetails " &
                              " where RowId between " & start_record & " and " & last_record & " " &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('clientlongitudinaldetails')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("clientlongitudinaldetails")

            End If
        Next i
        loop_counter = 0

        '3. export NeedsAssessment
        strReportProgress = "Exporting CSI Assessemnts"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = "select  count(*) from NeedsAssessment where  SyncDate IS NULL "
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by NeedsAssessmentID asc) as RowId  " &
                            " from NeedsAssessment where SyncDate IS NULL ) NeedsAssessment " &
                            " where RowId between " & start_record & " and " & last_record & " " &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('NeedsAssessment')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("7000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("NeedsAssessment")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        '4. export NeedsAssessmentMain
        'a. Count number of records to export
        sqlscalarAction = "select  count(*) from NeedsAssessmentmain where SyncDate IS NULL"
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by NeedsAssessmentID asc) as RowId  " &
                            " from NeedsAssessmentmain where SyncDate IS NULL ) NeedsAssessmentmain " &
                            " where RowId between " & start_record & " and " & last_record & " " &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('NeedsAssessmentmain')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("7000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("NeedsAssessmentmain")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table



        '6. export PriorityNeeds
        strReportProgress = "Exporting CSI Priority Needs"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = "select  count(*) from PriorityNeeds where SyncDate IS NULL"
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select  * , ROW_NUMBER() over (order by NeedsAssessmentID asc) as RowId " &
                            " from PriorityNeeds where SyncDate IS NULL ) PriorityNeeds " &
                                " where RowId between " & start_record & " and " & last_record & "" &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('PriorityNeeds')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("7000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("PriorityNeeds")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table


        '7. export ServicesProvided
        strReportProgress = "Exporting CSI Services Provided"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = "select count(*) from ServicesProvided where SyncDate IS NULL "
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select * , ROW_NUMBER() over (order by NeedsAssessmentID asc) as RowId " &
                                " from ServicesProvided where SyncDate IS NULL ) ServicesProvided " &
                                " where RowId between " & start_record & " and " & last_record & "" &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('ServicesProvided')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("7000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("ServicesProvided")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        '===========OVC FORM1A ITEMS============
        '5. export PriorityMonitoring
        strReportProgress = "Exporting Form1A Priority Needs"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = "select  count(*) from PriorityMonitoring where  SyncDate IS NULL "
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select *, ROW_NUMBER() over (order by ssvid asc) as RowId " &
                                 " from PriorityMonitoring where  SyncDate IS NULL ) PriorityMonitoring " &
                                " where RowId between " & start_record & " and " & last_record & "" &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('PriorityMonitoring')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("7000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("PriorityMonitoring")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        '8. export StatusandServiceMonitoring
        strReportProgress = "Exporting Form1A Assessments"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = "select  count(*) from StatusandServiceMonitoring_assessment where SyncDate IS NULL "
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by ssvid asc) as RowId  " &
                           " from StatusandServiceMonitoring_assessment where SyncDate IS NULL ) StatusandServiceMonitoring_assessment " &
                                " where RowId between " & start_record & " and " & last_record & "" &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('StatusandServiceMonitoring_assessment')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("7000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("StatusandServiceMonitoring_assessment")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        'a. Count number of records to export
        sqlscalarAction = "select  count(*) from StatusandServiceMonitoring_Service where SyncDate IS NULL "
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by ssvid asc) as RowId  " &
                           " from StatusandServiceMonitoring_Service where SyncDate IS NULL ) StatusandServiceMonitoring_Service " &
                                " where RowId between " & start_record & " and " & last_record & "" &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('StatusandServiceMonitoring_service')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("7000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("StatusandServiceMonitoring_service")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        '8. export OVCVisitsCriticalEvents
        strReportProgress = "Exporting Form1A Critical Events"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = "select  count(ceid) from OVCVisitsCriticalEvents where SyncDate IS NULL "
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by ssvid asc) as RowId  " &
                           " from OVCVisitsCriticalEvents where SyncDate IS NULL ) OVCVisitsCriticalEvents " &
                                " where RowId between " & start_record & " and " & last_record & "" &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('OVCVisitsCriticalEvents')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("7000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("OVCVisitsCriticalEvents")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        '9. export StatusandServiceVisit
        strReportProgress = "Exporting Form1A Visits"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = "select  count(*) from StatusandServiceVisit where SyncDate IS NULL "
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by ssvid asc) as RowId " &
                           " from StatusandServiceVisit where SyncDate IS NULL ) StatusandServiceVisit " &
                                " where RowId between " & start_record & " and " & last_record & "" &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('StatusandServiceVisit')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("7000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("StatusandServiceVisit")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        '===========Caregiver FORM1A ITEMS============
        '5. export PriorityMonitoring
        strReportProgress = "Exporting Form1A Caregiver Priority Needs"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = "select  count(PriorityMonitoringid) from CAREGIVER_PriorityMonitoring where  SyncDate IS NULL"
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select *, ROW_NUMBER() over (order by ssvid asc) as RowId " &
                                 " from CAREGIVER_PriorityMonitoring where  SyncDate IS NULL ) PriorityMonitoring " &
                                " where RowId between " & start_record & " and " & last_record & "" &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('CAREGIVER_PriorityMonitoring')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("7000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("CAREGIVER_PriorityMonitoring")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        '8. export StatusandServiceMonitoring
        strReportProgress = "Exporting Form1A Caregiver Assessemnts"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = "select  count(SSMID) from CAREGIVER_StatusAndServiceMonitoring_Assessment where SyncDate IS NULL "
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by ssvid asc) as RowId  " &
                           " from CAREGIVER_StatusAndServiceMonitoring_assessment where SyncDate IS NULL ) CAREGIVER_StatusAndServiceMonitoring_assessment " &
                                " where RowId between " & start_record & " and " & last_record & "" &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('CAREGIVER_StatusAndServiceMonitoring_assessment')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("7000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("CAREGIVER_StatusAndServiceMonitoring_assessment")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        'a. Count number of records to export
        sqlscalarAction = "select  count(SSMID) from CAREGIVER_StatusAndServiceMonitoring_service where SyncDate IS NULL "
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by ssvid asc) as RowId  " &
                           " from CAREGIVER_StatusAndServiceMonitoring_service where SyncDate IS NULL ) CAREGIVER_StatusAndServiceMonitoring_service " &
                                " where RowId between " & start_record & " and " & last_record & "" &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('CAREGIVER_StatusAndServiceMonitoring_service')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("7000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("CAREGIVER_StatusAndServiceMonitoring_service")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        '8. export OVCVisitsCriticalEvents
        strReportProgress = "Exporting Form1A Caregiver Critical Events"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = "select  count(ceid) from CareGiverVisitsCriticalEvents where SyncDate IS NULL "
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by ssvid asc) as RowId  " &
                           " from CareGiverVisitsCriticalEvents where SyncDate IS NULL ) CareGiverVisitsCriticalEvents " &
                                " where RowId between " & start_record & " and " & last_record & "" &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('CareGiverVisitsCriticalEvents')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("7000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("CareGiverVisitsCriticalEvents")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        '9. export StatusandServiceVisit
        strReportProgress = "Exporting Form1A Caregiver Visits"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = "select  count(ssvid) from CAREGIVER_StatusAndServiceVisit where SyncDate IS NULL "
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by ssvid asc) as RowId " &
                           " from CAREGIVER_StatusAndServiceVisit where SyncDate IS NULL ) CAREGIVER_StatusAndServiceVisit " &
                                " where RowId between " & start_record & " and " & last_record & "" &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('CAREGIVER_StatusAndServiceVisit')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("7000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("CAREGIVER_StatusAndServiceVisit")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        '===========End of Caregiver Form1As========================

        '10. export ParentDetails
        strReportProgress = "Exporting Caregiver/parents Details"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export

        sqlscalarAction = "select  count(*) from ParentDetails where (SyncDate IS NULL  " &
                                       " OR  isnull(Lastmodifiedon,'1900-01-01 00:00:00.000') > isnull(syncdate,'1900-01-01 00:00:00.000'))"
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by parentid asc) as RowId " &
                            " from ParentDetails where (SyncDate IS NULL  " &
                                       " OR  isnull(Lastmodifiedon,'1900-01-01 00:00:00.000') > isnull(syncdate,'1900-01-01 00:00:00.000'))) ParentDetails " &
                                " where RowId between " & start_record & " and " & last_record & "" &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('ParentDetails')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("ParentDetails")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        '11. export CHW
        strReportProgress = "Exporting CHW Details"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = "select  count(*) from CHW where (SyncDate IS NULL " &
                        " OR  Lastmodifiedon > isnull(syncdate,'1900-01-01 00:00:00.000')) "
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by chwid asc) as RowId " &
                            "  from CHW where (SyncDate IS NULL " &
           " OR  isnull(Lastmodifiedon,'1900-01-01 00:00:00.000') > isnull(syncdate,'1900-01-01 00:00:00.000')) ) CHW " &
                                " where RowId between " & start_record & " and " & last_record & "" &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('CHW')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("CHW")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        '12. export clientcriteria
        strReportProgress = "Exporting OVC Vulnerability Criteria"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = " select count(*) from " &
                            " (select * from clientdetails where (SyncDate IS NULL " &
                            " OR  isnull(Lastmodifiedon,'1900-01-01 00:00:00.000') > isnull(syncdate,'1900-01-01 00:00:00.000')) ) tbl "
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (SELECT *,ROW_NUMBER() over (order by clientcriteriaid asc) as RowId " &
                             " FROM [ClientCriteria] where eligibilitycriteria in " &
                             " (  " &
                             " select clienttype from  " &
                            " (select * from clientdetails where (SyncDate IS NULL " &
                            " OR  isnull(Lastmodifiedon,'1900-01-01 00:00:00.000') > isnull(syncdate,'1900-01-01 00:00:00.000')) " &
                            " ) tbl " &
                            " )) ClientCriteria   " &
                              " where RowId between " & start_record & " and " & last_record & " " &
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('clientcriteria')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("ClientCriteria")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        '12. export Household assessments
        strReportProgress = "Exporting OVC Household Assessments"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = " select count(*) from " &
                            " (select * from HHAssessmentMain where SyncDate IS NULL ) tbl "
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by hhassessmentid asc) as RowId " &
                         " from hhassessmentmain where SyncDate IS NULL ) hhassessmentmain " &
                              " where RowId between " & start_record & " and " & last_record & "" &
                                  " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('hhassessmentmain')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("hhassessmentmain")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        '12. export Household assessments
        strReportProgress = "Exporting OVC Household Assessments details"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = " select count(*) from " &
                            " (select * from HHAssessment where SyncDate IS NULL ) tbl "
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by hhassessmentid asc) as RowId " &
                         " from hhassessment where SyncDate IS NULL ) hhassessment " &
                              " where RowId between " & start_record & " and " & last_record & "" &
                                  " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('hhassessment')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("hhassessment")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        '=========Start ViralLoad export============
        '13. export Viral Load
        strReportProgress = "Exporting ViralLoad Testing details"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = " select count(*) from " &
                            " (select * from ViralLoad where SyncDate IS NULL ) tbl "
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by vlid asc) as RowId " &
                         " from ViralLoad where SyncDate IS NULL ) ViralLoad " &
                              " where RowId between " & start_record & " and " & last_record & "" &
                                  " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('viralload')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("viralload")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table

        '13. export Viral Load Longitudinal
        strReportProgress = "Exporting ViralLoadLongitudinal Testing details"
        Exportworker.ReportProgress(i + 1)

        'a. Count number of records to export
        sqlscalarAction = " select count(*) from " &
                            " (select * from ViralLoadLongitudinal where SyncDate IS NULL ) tbl "
        count_scalar = MyDBAction.DBAction(sqlscalarAction, DBActionType.Scalar)

        'b. Decide how many times to loop
        num_loops = FormatNumber(count_scalar / record_range_count, 0) + 1 'round off to nearest 10

        For i = 0 To num_loops - 1
            'with every loop, increment the loop counter by 1
            loop_counter = loop_counter + 1

            'calculate range 
            Dim start_record As Double
            Dim last_record As Double
            If loop_counter = 1 Then
                start_record = 0
                last_record = record_range_count
            Else
                start_record = (record_range_count * i) + 1
                last_record = (record_range_count * loop_counter)
            End If

            'fetch records within specific range
            mySqlAction = "select * from (select  *, ROW_NUMBER() over (order by vlid asc) as RowId " &
                         " from ViralLoadLongitudinal where SyncDate IS NULL ) viralloadlongitudinal " &
                              " where RowId between " & start_record & " and " & last_record & "" &
                                  " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('viralloadlongitudinal')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("viralloadlongitudinal")

            End If
        Next i
        loop_counter = 0 'reinitialize counter before next table
        '=========End ViralLoad export============


        'Execute the procedure to update SyncDates 
        strReportProgress = "Updating Timestamps for Exported Data"
        Exportworker.ReportProgress(i + 1)

        Dim connectionstring As String = ConnectionStrings(SelectedConnectionString).ToString
        Dim conn As New SqlConnection(connectionstring)
        Dim cmd As SqlCommand

        cmd = New SqlCommand("dbo.UpdateSyncDates")
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandTimeout = 3600000
        ''cmd.Parameters.Add(New SqlParameter("@OVCID", myOVCID.ToString))
        ''cmd.Parameters.Add(New SqlParameter("@DateofVisit", dtpDateofVisit.Value))
        conn.Open()
        cmd.Connection = conn
        cmd.ExecuteReader()
        conn.Close()





        ''show count of exported records---------------
        'Dim SqlAction As String = ""
        'Dim clientdetailsnewcount As Double = 0
        'Dim Clientdetailsupdatecount As Double = 0
        'Dim ClientdetailsLongitudinalcount As Double = 0
        'Dim CSIrecordcount As Double = 0
        'Dim Form1Acount As Double = 0
        'Dim ParentsCount As Double = 0
        'Dim ChwCount As Double = 0

        ''new clients
        'SqlAction = " select count(ovcid) from clientdetails where SyncDate IS NULL " & _
        '" and ovcid not in (select ovcid from tempKeys_OVCID)"
        'clientdetailsnewcount = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

        ''updated clients
        'SqlAction = "select count(ovcid) from clientdetails where isnull(Lastmodifiedon,'1900-01-01 00:00:00.000') > isnull(syncdate,'1900-01-01 00:00:00.000')  " & _
        '" and ovcid not in (select ovcid from tempKeys_OVCID)"
        'Clientdetailsupdatecount = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

        ''new CSI entries
        'SqlAction = "select  count(distinct(NeedsAssessmentID)) from NeedsAssessment where SyncDate IS NULL " & _
        '                            " AND NeedsAssessmentID not in " & _
        '                            " (select NeedsAssessmentID from tempKeys_NeedsAssessmentID) "
        'CSIrecordcount = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

        ''new form1a
        'SqlAction = "select  count(distinct(ssvid)) from StatusandServiceMonitoring where SyncDate IS NULL " & _
        '                            " AND ssvid not in " & _
        '                            " (select ssvid from tempKeys_ssvid) "
        'Form1Acount = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

        ''new parents
        'SqlAction = "select  count(parentid) from Parentdetails where SyncDate IS NULL"
        'ParentsCount = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

        ''new chw
        'SqlAction = "select  count(chwid) from chw where SyncDate IS NULL"
        'ChwCount = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

        'Label4.Text = "New Children:" & clientdetailsnewcount & "  Updated children:" & Clientdetailsupdatecount & vbCr & _
        '              "New CSI:" & CSIrecordcount & "   New Form1A:" & Form1Acount & vbCr & _
        '              "New Parents:" & ParentsCount & "  New CHWs:" & ChwCount

        '-----show count of exported records---------------

        ''---update syncdates so that data is not exportable again
        ''1.  clientdetails
        ''update syncdates of just exported data
        'mySqlAction = "UPDATE Clientdetails set Syncdate = getdate() where ovcid in " & _
        '                        "(select ovcid from clientdetails where (SyncDate IS NULL " & _
        '                        " OR  Lastmodifiedon > isnull(syncdate,'1900-01-01 00:00:00.000'))) "
        'UpdateSyncDates(mySqlAction)


        ''2.  clientlongitudinaldetails
        ''update syncdates of just exported data
        'mySqlAction = "UPDATE clientlongitudinaldetails set Syncdate = getdate() where ovcid in " & _
        '                        "(select ovcid from clientlongitudinaldetails where (SyncDate IS NULL " & _
        '                        " OR  Lastmodifiedon > isnull(syncdate,'1900-01-01 00:00:00.000'))) "
        'UpdateSyncDates(mySqlAction)


        ''3.  NeedsAssessment
        ''update syncdates of just exported data
        'mySqlAction = "UPDATE NeedsAssessment set Syncdate = getdate() where SyncDate IS NULL "
        'UpdateSyncDates(mySqlAction)


        ''4.  NeedsAssessmentMain
        ''update syncdates of just exported data
        'mySqlAction = "UPDATE NeedsAssessmentMain set Syncdate = getdate() where SyncDate IS NULL "
        'UpdateSyncDates(mySqlAction)

        ''5.  PriorityMonitoring
        ''update syncdates of just exported data
        'mySqlAction = "UPDATE PriorityMonitoring set Syncdate = getdate() where SyncDate IS NULL "
        'UpdateSyncDates(mySqlAction)


        ''6.  PriorityNeeds
        ''update syncdates of just exported data
        'mySqlAction = "UPDATE PriorityNeeds set Syncdate = getdate() where SyncDate IS NULL "
        'UpdateSyncDates(mySqlAction)

        ''7.  ServicesProvided
        ''update syncdates of just exported data
        'mySqlAction = "UPDATE ServicesProvided set Syncdate = getdate() where SyncDate IS NULL "
        'UpdateSyncDates(mySqlAction)

        ''8.  StatusandServiceMonitoring
        ''update syncdates of just exported data
        'mySqlAction = "UPDATE StatusandServiceMonitoring set Syncdate = getdate() where SyncDate IS NULL "
        'UpdateSyncDates(mySqlAction)

        ''9.  StatusandServiceVisit
        ''update syncdates of just exported data
        'mySqlAction = "UPDATE StatusandServiceVisit set Syncdate = getdate() where SyncDate IS NULL "
        'UpdateSyncDates(mySqlAction)
        ''------------


        'hyperlink to new folder with exported files
        'Label2.Text = "Files exported to: " & Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy")

        'show how many files were exported.


        MsgBox("Data Export Successful." & vbCrLf & " Click [OK] and Wait for Data Zipping...",
              MsgBoxStyle.Information)

        'Return True
        'Catch ex As Exception
        '    MsgBox("Data Export NOT Successful." & ex.Message & "---" & strReportProgress.ToString, MsgBoxStyle.Exclamation)
        '    Return False
        'End Try

    End Function

    Private Sub SplitHandler(ByVal count As Long, ByVal document As String)
        Dim stream_writer As New System.IO.StreamWriter(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy") & "\" &
        String.Format("Splitter{0:D8}.xml", count))
        stream_writer.Write(document)
        stream_writer.Close()
    End Sub

    Private Sub renamesplitfiles(ByVal newprefix As String)
        Dim di As New IO.DirectoryInfo(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"))
        RecurseDirectories(di, newprefix)
    End Sub

    Private Sub RecurseDirectories(ByVal di As DirectoryInfo, ByVal newprefix As String)
        Try

            'For Each d In di.GetDirectories()
            Dim strFileSize As String = ""
            Dim fi As IO.FileInfo

            Dim aryFi As IO.FileInfo() = di.GetFiles("*.*")
            For Each fi In aryFi

                If fi.Name.ToString.Contains("Splitter") = True Then
                    Dim Myrandomkey As New Random
                    Dim oldname As String = fi.Name
                    Dim newname As String = Replace(oldname, "Splitter", newprefix.ToString & Myrandomkey.Next(1111, 9999))
                    My.Computer.FileSystem.RenameFile(fi.FullName, newname)

                End If

            Next

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        'Next
        'Catch
        'End Try
    End Sub

    Private Function ExportExistingKeys() As Boolean
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        ' Specify the directory you want to manipulate.
        Dim path As String = Application.StartupPath & "\UniqueKeys\"

        Try
            '1. Create the directory to hold uniquekeys
            ' Determine whether the directory exists.
            If Not Directory.Exists(path) Then
                'create a new folder with new exports
                ' Try to create the directory.
                Dim dirExports As DirectoryInfo = Directory.CreateDirectory(path)
            End If

            '2. Create textfile and loop through keys and write them to file
            'a. NeedsAssessmentid
            If File.Exists(path & "NeedsAssessmentid") Then File.Delete(path & "NeedsAssessmentid")
            Dim sqlAction As String = "select NeedsAssessmentid from NeedsAssessmentMain order by NeedsAssessmentid " &
                                      " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('NeedsAssessmentMain')"
            Dim cmd As New SqlCommand(sqlAction, conn)
            conn.Open()
            Dim myreader As SqlDataReader = cmd.ExecuteReader()

            Do While myreader.Read
                WriteToFile(myreader(0).ToString, path, "NeedsAssessmentid.xml")
            Loop
            conn.Close()

            'b. Status Visit id SSVID
            If File.Exists(path & "SSVID") Then File.Delete(path & "SSVID")
            sqlAction = "select SSVID from StatusAndServiceVisit order by SSVID " &
                                      " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('SSVID')"
            cmd = New SqlCommand(sqlAction, conn)
            conn.Open()
            myreader = cmd.ExecuteReader()


            Do While myreader.Read
                WriteToFile(myreader(0).ToString, path, "SSVID.xml")
            Loop
            conn.Close()

            'C. OVCID
            If File.Exists(path & "OVCID") Then File.Delete(path & "OVCID")
            sqlAction = "select OVCID from Clientdetails order by OVCID " &
                                      " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('OVCID')"
            cmd = New SqlCommand(sqlAction, conn)
            conn.Open()
            myreader = cmd.ExecuteReader()


            Do While myreader.Read
                WriteToFile(myreader(0).ToString, path, "OVCID.xml")
            Loop
            conn.Close()

            'd. Parentid
            If File.Exists(path & "ParentID") Then File.Delete(path & "ParentID")
            sqlAction = "select ParentID from Parentdetails order by ParentID " &
                                      " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('ParentID')"
            cmd = New SqlCommand(sqlAction, conn)
            conn.Open()
            myreader = cmd.ExecuteReader()


            Do While myreader.Read
                WriteToFile(myreader(0).ToString, path, "ParentID.xml")
            Loop
            conn.Close()

            'e. Chwid
            If File.Exists(path & "CHWID") Then File.Delete(path & "CHWID")
            sqlAction = "select CHWID from CHW order by CHWID " &
                                      " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('CHWID')"
            cmd = New SqlCommand(sqlAction, conn)
            conn.Open()
            myreader = cmd.ExecuteReader()


            Do While myreader.Read
                WriteToFile(myreader(0).ToString, path, "CHWID.xml")
            Loop
            conn.Close()

            Return True
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation)
            Return False
        End Try
    End Function

    Public Shared Sub WriteToFile(ByVal strcontent As String, ByVal todayspath As String, ByVal filename As String)

        ' Specify the directory you want to manipulate.
        Dim path As String = Application.StartupPath & "\Exports"


        Try
            ' Determine whether the directory exists.
            If Not Directory.Exists(path) Then
                'create a new folder with new exports
                ' Try to create the directory.
                Dim dirExports As DirectoryInfo = Directory.CreateDirectory(path)

            End If


            If Not Directory.Exists(todayspath) Then
                'create a new folder with new exports
                ' Try to create the directory.
                Dim dirTodaysExports As DirectoryInfo = Directory.CreateDirectory(todayspath)

            End If

            '' Delete the directory.
            'di.Delete()
            'Console.WriteLine("The directory was deleted successfully.")

            If File.Exists(todayspath & filename) Then File.Delete(todayspath & filename)

            Using sw As StreamWriter = File.CreateText(todayspath & filename)
                sw.WriteLine(strcontent)
                sw.Close()
            End Using

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation)
        End Try
    End Sub

    Private Function Readscript(ByVal scriptfile As String) As String
        Try
            Using sr As New StreamReader(scriptfile)
                Return sr.ReadToEnd()
            End Using
        Catch e As Exception
            MsgBox("The file could not be read:" & e.Message, MsgBoxStyle.Critical)
        End Try
    End Function

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


    Private Function UpdateSyncDates(ByVal mysqlaction As String) As Boolean
        Dim connectionstring As String = ConnectionStrings(SelectedConnectionString).ToString
        Try

            Dim cmd As New System.Data.SqlClient.SqlCommand()
            Dim conn As New System.Data.SqlClient.SqlConnection(connectionstring)
            cmd.Connection = conn
            cmd.CommandTimeout = 3600 '60 minutes just incase
            cmd.CommandText = mysqlaction
            conn.Open()
            cmd.ExecuteNonQuery()
            Return True
        Catch ex As Exception
            MsgBox(ex.Message & vbCrLf & mysqlaction & vbCrLf & "[updatesyncdates]", MsgBoxStyle.Exclamation)
            Return False
        End Try
    End Function

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

    Private Sub btnImport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImport.Click

        'If you click the button when worker is still fetching results, cancel the click event
        If ImportWorker.IsBusy = True Then
            MsgBox("Data import in progress. Please be patient.", MsgBoxStyle.Exclamation)
        Else

            Me.WebBrowser2.Url = New System.Uri(Application.StartupPath & "\images\289.gif", System.UriKind.Absolute)
            ProgressPanel2.Visible = True
            'run the report queries on a different thread to avoid freezing of application
            ImportWorker.RunWorkerAsync()
        End If

    End Sub

    Private Sub Importworker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles ImportWorker.DoWork
        importdata()
    End Sub

    Private Sub Importworker_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles ImportWorker.ProgressChanged
        Label6.Text = strReportProgress
    End Sub

    Private Sub Importworker_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles ImportWorker.RunWorkerCompleted
        ProgressPanel2.Visible = False
    End Sub

    Private Sub importdata()
        Dim connectionstring As String = ConnectionStrings(SelectedConnectionString).ToString
        Try
            Dim i As Integer = 0



            'first, clean up the exports folder
            strReportProgress = "Opening Excel File"
            ImportWorker.ReportProgress(i + 1)

            'Dim di As New IO.DirectoryInfo(Application.StartupPath & "\Exports")
            'Deletefiles(di)

            '''then, we unzip the file sent from partners
            ''strReportProgress = "Unzipping partner data"
            ''ImportWorker.ReportProgress(i + 1)

            ''If ExtractZip(txtimportdirectory.Text.ToString, Application.StartupPath & "\Exports") = False Then
            ''    MsgBox("something wrong with Zip file.", MsgBoxStyle.Exclamation)
            ''    Exit Sub
            ''End If



            'first create stored procedures to create temporary tables
            strReportProgress = "Preparing temporary tables"
            ImportWorker.ReportProgress(i + 1)

            Dim issuccessful As Boolean = runSQLScriptFiles(Application.StartupPath & "\CreateTempTables.sql")

            'Execute the procedure and create the tables as indicated in CreateTempTables.sql
            Dim conn As New SqlConnection(connectionstring)
            Dim cmd As SqlCommand
            If issuccessful = True Then
                cmd = New SqlCommand("dbo.CreateTempTables")
                cmd.CommandType = CommandType.StoredProcedure
                cmd.CommandTimeout = 300
                ''cmd.Parameters.Add(New SqlParameter("@OVCID", myOVCID.ToString))
                ''cmd.Parameters.Add(New SqlParameter("@DateofVisit", dtpDateofVisit.Value))
                conn.Open()
                cmd.Connection = conn
                cmd.ExecuteReader()
                conn.Close()
                'MsgBox("Stored Procedure executed")
            End If

            'Open the excel file
            If ExtractCPIMSData(txtimportdirectory.Text.ToString) = False Then
                MsgBox("something wrong with Excel file.", MsgBoxStyle.Exclamation)
                Exit Sub
            End If

            'file import
            'If ImportfromDirectory(Application.StartupPath & "\Exports") = True Then
            '    'Actual synchronising from temp tables into main tables

            strReportProgress = "Merging imported data from temp_tables to main_tables"
            ImportWorker.ReportProgress(i + 1)

            cmd = New SqlCommand("dbo.SYNCOVCDATA_HQ")
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandTimeout = 3600
            conn.Open()
            cmd.Connection = conn
            cmd.ExecuteReader()
            conn.Close()
            ' MsgBox("Stored Procedure executed")
            'End If

            MsgBox("Data import and Synchronization SUCCESSFUL.", MsgBoxStyle.Information)
        Catch ex As Exception
            MsgBox("Data import NOT Successful.")
        End Try
    End Sub

    Private Function ExtractCPIMSData(ByVal cpimsExcelFile As String) As Boolean
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        Try

            strReportProgress = "Opening Excel file"
            'ImportWorker.ReportProgress(i + 1)
            Dim stream As FileStream = File.Open(cpimsExcelFile, FileMode.Open, FileAccess.Read)

            '1. Reading from a binary Excel file ('97-2003 format; *.xls)
            'Dim excelReader As IExcelDataReader = ExcelReaderFactory.CreateBinaryReader(stream)
            '...
            strReportProgress = "Initializing Excel Reader"
            'ImportWorker.ReportProgress(i + 1)
            '2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
            Dim excelReader As IExcelDataReader = ExcelReaderFactory.CreateOpenXmlReader(stream)
            '...
            '3. DataSet - The result of each spreadsheet will be created in the result.Tables
            'Dim result As DataSet = excelReader.AsDataSet()
            '...
            excelReader.IsFirstRowAsColumnNames = True
            Dim result As DataSet = excelReader.AsDataSet()
            'Dim myDataTable As DataTable = result.Tables(0)
            '5. Data Reader methods

            If bulkcopyxml(cpimsExcelFile, "temp_OVCRegistrationDetails", result) = False Then
                MsgBox("Excel data import failed.", MsgBoxStyle.Exclamation)
                Return False
            End If
            'While excelReader.Read()

            '    MsgBox(excelReader.GetString(0)) 'schoolname
            '    'MsgBox(excelReader.GetString(1))
            '    'MsgBox(excelReader.GetString(2)) 'district
            '    'MsgBox(excelReader.GetString(3)) 'schoollevel





            'End While

            '6. Free resources (IExcelDataReader is IDisposable)
            'excelReader.Close()



            MsgBox("CPIMS data upload complete.", MsgBoxStyle.Information)
            Return True

        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        Finally
            conn.Close()
        End Try
    End Function

    'Private Function ImportfromDirectory(ByVal importdirectory As String) As Boolean
    '    Try
    '        'loop through files in the directory
    '        ' Make a reference to a directory.
    '        Dim di As New DirectoryInfo(importdirectory)
    '        ' Get a reference to each file in that directory.
    '        Dim fiArr As FileInfo() = di.GetFiles("*.xml")
    '        ' Display the names of the files.
    '        Dim fri As FileInfo
    '        Dim filenamebilaextension, destinationxmltable As String
    '        For Each fri In fiArr

    '            'report progress
    '            strReportProgress = "Importing file " & fri.Name
    '            ImportWorker.ReportProgress(0)

    '            filenamebilaextension = System.IO.Path.GetFileNameWithoutExtension(fri.Name)
    '            'remove the digits at the end e.g Clientdetails000 becomes Clientdetails
    '            destinationxmltable = filenamebilaextension.Substring(0, filenamebilaextension.Length - 12)

    '            'import xml data into database
    '            If bulkcopyxml(importdirectory, filenamebilaextension, destinationxmltable) = False Then
    '                MsgBox("Import of " & fri.Name & " FAILED", MsgBoxStyle.Exclamation)
    '            End If

    '        Next fri

    '        Return True

    '    Catch ex As Exception
    '        MsgBox(ex.Message, MsgBoxStyle.Exclamation)
    '        Return False
    '    End Try
    'End Function

    Private Function ImportKeysfromDirectory(ByVal importdirectory As String) As Boolean
        Try
            'loop through files in the directory
            ' Make a reference to a directory.
            Dim di As New DirectoryInfo(importdirectory)
            ' Get a reference to each file in that directory.
            Dim fiArr As FileInfo() = di.GetFiles("*.xml")
            ' Display the names of the files.
            Dim fri As FileInfo
            Dim filenamebilaextension As String
            For Each fri In fiArr
                filenamebilaextension = System.IO.Path.GetFileNameWithoutExtension(fri.Name)

                'import xml data into database
                If bulkcopykeysxml(importdirectory, filenamebilaextension) = False Then
                    MsgBox("Import of Keys " & fri.Name & " FAILED", MsgBoxStyle.Exclamation)
                End If

            Next fri

            Return True

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation)
            Return False
        End Try
    End Function

    Private Function bulkcopykeysxml(ByVal importdirectory As String, ByVal strfilename As String) As Boolean
        Dim connectionstring As String = ConnectionStrings(SelectedConnectionString).ToString
        Try
            Using sqlconn As New SqlConnection(connectionstring)
                Dim ds As New DataSet()
                Dim sourcedata As New DataTable()
                ds.ReadXml(importdirectory & "\" & strfilename & ".xml")
                sourcedata = ds.Tables(0)
                sqlconn.Open()
                Using bulkcopy As New SqlBulkCopy(sqlconn)
                    bulkcopy.BatchSize = 5000
                    bulkcopy.DestinationTableName = "dbo.tempkeys_" & strfilename & ""
                    'bulkcopy.ColumnMappings.Add("FirstName", "FirstName")
                    'bulkcopy.ColumnMappings.Add("MiddleName", "MiddleName")
                    'bulkcopy.ColumnMappings.Add("Surname", "Surname")
                    bulkcopy.WriteToServer(sourcedata)
                End Using
            End Using

            Return True
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation)
            Return False
        End Try
    End Function


    Private Function bulkcopyxml(ByVal strfilename As String,
                                ByVal xmltablename As String, ByVal myds As DataSet) As Boolean
        Dim connectionstring As String = ConnectionStrings(SelectedConnectionString).ToString
        Dim bulkcopy As New SqlBulkCopy(connectionstring, SqlBulkCopyOptions.TableLock _
                                                  And SqlBulkCopyOptions.UseInternalTransaction _
                                                  And SqlBulkCopyOptions.KeepNulls)
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


                'ds.ReadXml(importdirectory & "\" & strfilename & ".xml")
                ds = myds

                sourcedata = ds.Tables(0)
                sqlconn.Open()
                Using bulkcopy
                    'As New SqlBulkCopy(connectionstring, SqlBulkCopyOptions.TableLock _
                    '                              And SqlBulkCopyOptions.UseInternalTransaction _
                    '                              And SqlBulkCopyOptions.KeepNulls)
                    bulkcopy.BulkCopyTimeout = 600
                    bulkcopy.BatchSize = 1000
                    bulkcopy.DestinationTableName = "dbo." & xmltablename & ""
                    bulkcopy.ColumnMappings.Clear()
                    For Each myCol In ds.Tables(0).Columns
                        'dont map Rowid coz it does not exist in db
                        'If myCol.ColumnName.Trim().ToString.ToLower <> "countyid" _
                        '    AndAlso myCol.ColumnName.Trim().ToString.ToLower <> "caregiver_gender" _
                        '    AndAlso myCol.ColumnName.Trim().ToString.ToLower <> "chv_gender" Then


                        bulkcopy.ColumnMappings.Add(myCol.ColumnName.Trim(), myCol.ColumnName.Trim())



                        Console.WriteLine(myCol.ColumnName)
                        'End If

                    Next

                    bulkcopy.WriteToServer(sourcedata)

                End Using
            End Using
            'Try
            '    Dim objBL As New SQLXMLBULKLOADLib.SQLXMLBulkLoad4
            '    objBL.ConnectionString = connectionstring
            '    objBL.ErrorLogFile = "error.xml"
            '    objBL.KeepIdentity = False
            '    'objBL.CheckConstraints = True
            '    'objBL.XMLFragment = True
            '    'objBL.SchemaGen = True
            '    'objBL.SGDropTables = True
            '    'objBL.Execute(Application.StartupPath & "\Schema\" & xmltablename & ".xsd", _
            '    '              importdirectory & "\" & strfilename & ".xml")
            '    objBL.Execute("I:\APHIA\OVCSystem Nakuru\Testing\OVCSystem\OVCSystem\" & _
            '                  "DataSet_Sync.xsd", _
            '                 importdirectory & "\" & strfilename & ".xml")
            'Catch e As Exception
            '    MsgBox(e.ToString())
            'End Try
            Return True
        Catch ex As Exception

            Dim errorMessage As String = String.Empty

            ' If (ex.Message.Contains("Received an invalid column length from the bcp client for colid")) Then

            ' this method gives message with column name with length.  
            errorMessage = GetBulkCopyColumnException(ex, bulkcopy)
            'End If
            MsgBox(errorMessage, MsgBoxStyle.Exclamation)
            Return False
        End Try
    End Function

    'This exception gives the column number And that Is Not easy To find When the Excel sheet contains a 
    'large number Of columns. So we are Using the following code To find the column name.
    Protected Function GetBulkCopyColumnException(ByVal ex As Exception, ByVal bulkcopy As SqlBulkCopy) As String
        Dim message As String = String.Empty

        ' If ex.Message.Contains("Received an invalid column length from the bcp client for colid") Then
        Dim pattern As String = "\d+"
            Dim match As Match = Regex.Match(ex.Message.ToString(), pattern)
            Dim index = Convert.ToInt32(match.Value) - 1
            Dim fi As FieldInfo = GetType(SqlBulkCopy).GetField("_sortedColumnMappings", BindingFlags.NonPublic Or BindingFlags.Instance)
            Dim sortedColumns = fi.GetValue(bulkcopy)
            Dim items = CType(sortedColumns.[GetType]().GetField("_items", BindingFlags.NonPublic Or BindingFlags.Instance).GetValue(sortedColumns), Object())
            Dim itemdata As FieldInfo = items(index).[GetType]().GetField("_metadata", BindingFlags.NonPublic Or BindingFlags.Instance)
            Dim metadata = itemdata.GetValue(items(index))
            Dim column = metadata.[GetType]().GetField("column", BindingFlags.[Public] Or BindingFlags.NonPublic Or BindingFlags.Instance).GetValue(metadata)
            Dim length = metadata.[GetType]().GetField("length", BindingFlags.[Public] Or BindingFlags.NonPublic Or BindingFlags.Instance).GetValue(metadata)
            message = (String.Format("Column: {0} contains data with a length greater than: {1}", column, length))
        'End If

        Return message
    End Function

    Private Function validatexml(ByVal xmlfile As String) As Boolean
        Try
            Dim m_xmlr As XmlTextReader
            'Create the XML Reader
            m_xmlr = New XmlTextReader(xmlfile)
            'Disable whitespace so that you don't have to read over whitespaces
            m_xmlr.WhitespaceHandling = WhitespaceHandling.None
            'read the xml declaration and advance to family tag
            m_xmlr.Read()
            'read the family tag
            m_xmlr.Read()
            'Load the Loop
            While Not m_xmlr.EOF
                'ikiingia hapa, then the file has some elements
                Return True
            End While

            'ikifika hapa, then there si no data on file
            Return False
        Catch ex As Exception
            'MsgBox(ex.Message)
            Return False
        End Try
    End Function




    Private Sub frmDataSync_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'AddRootNode()

        ''Hide HQ tabs
        'If UCase(AppSettings("IsMaintenance").ToString) = "TRUE" Then
        '    TabControl1.TabPages.Remove(TabPage1)
        '    TabControl1.TabPages.Remove(TabPage4)
        'Else
        '    'TabControl1.TabPages.Remove(TabPage2)
        '    TabControl1.TabPages.Remove(TabPage3)
        '    TabControl1.TabPages.Remove(TabPage5)
        'End If
    End Sub



    Private Sub btnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRun.Click
        Try
            Dim issuccessful As Boolean = runSQLScriptFiles(txtScriptFile.Text.ToString)

            If issuccessful Then
                MsgBox("Script run successful", MsgBoxStyle.Information)
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
            MsgBox("Script not successful", MsgBoxStyle.Information)
        End Try
    End Sub

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

    Private Sub cmdbrowsescript_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdbrowsescript.Click
        OpenFileDialog2.Title = "Please Select a File"
        'OpenFileDialog1.InitialDirectory = "C:OVC"

        OpenFileDialog2.ShowDialog()

    End Sub

    Private Sub OpenFileDialog2_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog2.FileOk

        'Dim strm As System.IO.Stream
        'strm = OpenFileDialog1.OpenFile()
        txtScriptFile.Text = OpenFileDialog2.FileName.ToString()

    End Sub


    Private Sub lnkNormalExport_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkNormalExport.LinkClicked
        Panel1.Visible = True
        Panel2.Visible = False
    End Sub

    Private Sub lnkRedoExport_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkRedoExport.LinkClicked
        Panel1.Visible = False
        Panel2.Visible = True
    End Sub

    Private Sub btnRedoExportData_Click(sender As Object, e As EventArgs) Handles btnRedoExportData.Click
        Dim connectionstring As String = ConnectionStrings(SelectedConnectionString).ToString

        'If you click the button when worker is still fetching results, cancel the click event
        If Exportworker.IsBusy = True Then
            MsgBox("Fetching results. Please be patient.", MsgBoxStyle.Exclamation)
        Else

            Me.WebBrowser1.Url = New System.Uri(Application.StartupPath & "\images\289.gif", System.UriKind.Absolute)
            ProgressPanel.Visible = True
            'run the report queries on a different thread to avoid freezing of application
            Exportworker.RunWorkerAsync()
        End If
    End Sub
End Class