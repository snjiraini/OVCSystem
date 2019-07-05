Imports OVCSystem.functions
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports Microsoft.Office.Interop
Imports ExcelInterop = Microsoft.Office.Interop.Excel
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.ComponentModel


Public Class frmExcelReports
    Dim rptformula As String = ""
    Dim selectedreport As String = ""
    Dim rptdatefrom, rptdateto As String
    Dim rptcboclusters As String = ""
    Dim rptdistricts As String = ""
    Dim rptcbos As String = ""
    Dim rptpriorities As String = ""
    Dim rptcboclustername As String = ""
    Dim rptcboname As String = ""
    Dim rptdistrictname As String = ""
    Dim reportingmode As String = ""
    Dim rptNeedsNumberofMonths As Integer

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

    Private Sub frmReports_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        m_excelreportFormNumber = 0
    End Sub

    Private Sub frmBenefitsSummary_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        populatereportslist()
        'populatepriorityNeeds()
        Me.AcceptButton = btnSearch
        Me.Height = 150

    End Sub

    'Private Sub populatepriorityNeeds()
    '    Dim ErrorAction As New functions
    '    Try

    '        'populate the combobox
    '        Dim mySqlAction As String = "SELECT SSID, ServiceStatus FROM servicestatus " & _
    '        " WHERE isnull(Procurable,'False') = 'True' ORDER BY ServiceStatus"
    '        Dim MyDBAction As New functions
    '        Dim MyDatable As New Data.DataTable
    '        MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
    '        With lstPriorityNeeds
    '            .DataSource = Nothing 'if already bound, throws a bug on any refresh
    '            .Items.Clear()
    '            .DataSource = MyDatable
    '            .DisplayMember = "ServiceStatus"
    '            .ValueMember = "SSID"
    '            .SelectedIndex = -1 ' This line makes the combo default value to be blank
    '        End With
    '    Catch ex As Exception
    '        ErrorAction.WriteToErrorLogFile("Reports", "populatepriorityNeeds", ex.Message) ''---Write error to error log file


    '    End Try
    'End Sub

    Private Sub populatereportslist()
        cboReportType.Items.Clear()


        cboReportType.Items.Add("OVC Registration Summary")
        cboReportType.Items.Add("VSLA groups list")
        cboReportType.Items.Add("VSLA groups membership list")
        cboReportType.Items.Add("Caregiver/Valuechain linkage list")
        cboReportType.Items.Add("Caregiver/starterkit linkage list")
        cboReportType.Items.Add("Caregiver/OVC valuechain benefit summary")
        cboReportType.Items.Add("Caregiver/OVC starterkit benefit summary")
        cboReportType.Items.Add("Trainings list")
        cboReportType.Items.Add("Trainings attendance list")

    End Sub

    Private Sub ReportParameters()
        'clear variables incase they have previous data
        rptcboclusters = ""
        rptdistricts = ""
        rptcbos = ""
        rptpriorities = ""
        rptcboclustername = ""
        rptcboname = ""
        rptdistrictname = ""
        rptNeedsNumberofMonths = 0


        'put selected report from dropdown in a variable to enable it to be accessed from backgroundworker thread
        selectedreport = LCase(cboReportType.Text)


        'store report parameters in variables for easy reference from background worker thread

        rptdatefrom = Format(dtpFrom.Value, "dd-MMM-yyyy").ToString
        rptdateto = Format(dtpTo.Value, "dd-MMM-yyyy").ToString



    End Sub

    Private Function validatecontrols() As Boolean
        Try
            ErrorProvider1.Clear()

            If cboReportType.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(cboReportType, "Please select a report to continue")
                MsgBox("Please select a report to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                cboReportType.Focus()
                Return False

            ElseIf dtpFrom.Value >= Date.Today Then
                ErrorProvider1.SetError(dtpFrom, "Please select start and end dates to continue")
                MsgBox("Please select start and end dates to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                dtpFrom.Focus()
                Return False
            End If

            ErrorProvider1.Clear()

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function


    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click

        If validatecontrols() = False Then
            Exit Sub
        End If



        'put selected report from dropdown in a variable to enable it to be accessed from backgroundworker thread
        ReportParameters()

        'If you click the button when worker is still fetching results, cancel the click event
        If worker.IsBusy = True Then
            MsgBox("Fetching results. Please be patient.", MsgBoxStyle.Exclamation)
        Else
            Me.Height = 360
            Me.WebBrowser1.Url = New System.Uri(Application.StartupPath & "\images\320.gif", System.UriKind.Absolute)
            ProgressPanel.Visible = True
            'run the report queries on a different thread to avoid freezing of application
            worker.RunWorkerAsync()
        End If
        ''search filter report formula
        'ConfigureCrystalReports(LCase(cboReportType.Text))
    End Sub

    Private Sub worker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles worker.DoWork
        ConfigureCrystalReports(selectedreport)
    End Sub

    Private Sub worker_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles worker.RunWorkerCompleted
        Me.Height = 150
        ProgressPanel.Visible = False

    End Sub

    Private Sub ConfigureCrystalReports(ByVal myreporttype As String)

        Dim mySqlAction As String = ""
        Dim MyNewReportPath As String = ""
        Dim MyDBAction As New functions
        Dim MyDataTable As New Data.DataTable

        Dim TemplateFilePath As String = ""
        Dim OutputFilePath As String = ""
        Dim StoredProcName As String = ""
        Dim TemplateFileName As String = ""
        Dim periodtype As String = ""



        'Record what duration the report is for, and what time it was done. This will be part of the file name.
        Dim timeMark As String = " " & Format(dtpFrom.Value, "ddMMMyyyy") & "-" &
                Format(dtpTo.Value, "ddMMMyyyy") & " " & DateTime.Now.ToString("HHmmss")
        Select Case myreporttype
            '("OVC Registration Summary")
            '("VSLA groups list")
            '("VSLA groups membership list")
            '("Caregiver/Valuechain linkage list")
            '("Caregiver/starterkit linkage list")
            '("Caregiver/OVC valuechain benefit summary")
            '("Caregiver/OVC starterkit benefit summary")
            '("Trainings list")
            '("Trainings attendance list")

            Case "ovc registration summary"
                TemplateFileName = "OVCRegistrationSummary"
                StoredProcName = "dbo.rpt_OVCRegistrationSummary"
            Case "vsla groups list"
                TemplateFileName = "VSLAGroupsList"
                StoredProcName = "dbo.rpt_vsla_groups_list"
            Case "vsla groups membership list"
                TemplateFileName = "VSLAGroupsMembershipList"
                StoredProcName = "dbo.rpt_vsla_groups_membership_list"
            Case "caregiver/valuechain linkage list"
                TemplateFileName = "Caregiver_Valuechain_Linkage_List"
                StoredProcName = "dbo.rpt_caregiver_valuechain_linkage_list"
            Case "caregiver/starterkit linkage list"
                TemplateFileName = "Caregiver_Starterkit_Linkage_List"
                StoredProcName = "dbo.rpt_caregiver_starterkit_linkage_list"
            Case "trainings list"
                TemplateFileName = "Trainings_List"
                StoredProcName = "dbo.rpt_trainings_list"
            Case "trainings attendance list"
                TemplateFileName = "Trainings_Attendance_List"
                StoredProcName = "dbo.rpt_trainings_attendance_list"
            Case "caregiver/ovc valuechain benefit summary"
                TemplateFileName = "Caregiver_OVC_Valuechain_Benefit_Summary"
                StoredProcName = "dbo.rpt_caregiver_ovc_valuechain_benefit_summary"
            Case "caregiver/ovc starterkit benefit summary"
                TemplateFileName = "Caregiver_OVC_Starterkit_Benefit_Summary"
                StoredProcName = "dbo.rpt_caregiver_ovc_starterkit_benefit_summary"
        End Select

        'Template and Output report names and directories
        TemplateFilePath = Application.StartupPath &
        "\TemplatesExcelReports\" & TemplateFileName & ".xlsm"

        If Directory.Exists(Application.StartupPath &
            "\OutputExcelReports\" & Format(Date.Now, "dd-MMM-yyyy")) = False Then
            Dim di As DirectoryInfo = Directory.CreateDirectory(Application.StartupPath &
            "\OutputExcelReports\" & Format(Date.Now, "dd-MMM-yyyy"))
        End If

        OutputFilePath = Application.StartupPath & "\OutputExcelReports\" &
     Format(Date.Now, "dd-MMM-yyyy") & "\" & TemplateFileName & " " & timeMark & ".xlsm"

        'Populate data into relevant excel templates for reports
        Dim demoDataSet As DataSet = Me.getDataSet(StoredProcName)

        FastExportingMethod.ExportToExcel(demoDataSet, OutputFilePath, TemplateFilePath)


        MsgBox("Report Generated.")


        'Open Excel application and open current excel report
        Dim oExcel As Microsoft.Office.Interop.Excel.Application
        Dim obook As Microsoft.Office.Interop.Excel.Workbook
        Try
            'Grab a running instance of Excel.
            oExcel = Marshal.GetActiveObject("Excel.Application")
        Catch ex As COMException
            'If no instance exist then create a new one.
            oExcel = New ExcelInterop.Application
        End Try
        oExcel.Visible = True
        obook = oExcel.Workbooks.Open(OutputFilePath)

        'Run the macros for data pivoting and data presentation immediately after opening the excel file
        oExcel.Run("Auto_Open")

    End Sub


    'Private Function selected_priorities() As String

    '    ' Loop through all the selected items of the listbox and save priorities
    '    Dim dview As DataRowView
    '    Dim strpriorities As String = ""
    '    For Each dview In lstPriorityNeeds.CheckedItems

    '        'Its now dview.row.item(5) coz we using vw_domain_services and priorityid/ssid is field 5
    '        'InsertToPriorityMonitoringTable(MyPMID, MySSVID, dview.Row.Item(0).ToString)
    '        strpriorities = strpriorities & "," & dview.Row.Item(0).ToString

    '    Next
    '    Return strpriorities
    'End Function

    Private Sub KillExcelProcess()
        Try
            Dim proc As System.Diagnostics.Process

            For Each proc In System.Diagnostics.Process.GetProcessesByName("EXCEL")

                proc.Kill()
            Next
        Catch ex As Exception

        End Try


    End Sub

    Private Function getDataSet(ByVal mystoredproc As String) As DataSet
        Dim conn As New SqlClient.SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)


        Dim cmd As SqlCommand

        cmd = New SqlCommand(mystoredproc)


        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandTimeout = 1200

        'create storedproc parameters depending on which cbo or clusters have been selected

        cmd.Parameters.Add(New SqlParameter("@startdate", rptdatefrom.ToString))
        cmd.Parameters.Add(New SqlParameter("@enddate", rptdateto.ToString))

        conn.Open()
        cmd.Connection = conn

        Dim dt As New DataTable
        Dim ds As New DataSet()
        Dim myreader As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)

        'Load data from the reader to the datatable
        dt.Load(myreader)

        ds.Tables.Add(dt)

        'Dim ds As New DataSet()
        'dt.Fill(ds)
        'ds.Tables(0).TableName = "Clientdetails"


        Return ds
    End Function



    Private Sub cboReportType_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboReportType.SelectedValueChanged
        'If LCase(cboReportType.Text).Contains("needs vs served") = True Then
        '    Label6.Visible = True
        '    NumMonthsUpDown.Visible = True
        'Else
        '    Label6.Visible = False
        '    NumMonthsUpDown.Visible = False
        'End If
    End Sub

   
   
End Class