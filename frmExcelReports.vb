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
    

        populatereportscbo()
        populatedistricts()
        populateclusters()
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

    Private Sub populatereportscbo()
        cboReportType.Items.Clear()

        cboCBO.Enabled = True
        cboDistrict.Enabled = True
        chkClusters.Visible = True

        cboReportType.Items.Add("OVC Registration List")
        cboReportType.Items.Add("OVC Registration List by CHV")
        cboReportType.Items.Add("OVC Registration List by HH")
        cboReportType.Items.Add("OVC Priority Needs List")
        cboReportType.Items.Add("OVC CSI Analysis")
        cboReportType.Items.Add("PEPFAR Summary")
        cboReportType.Items.Add("PEPFAR Detailed Summary")
        'cboReportType.Items.Add("PEPFAR Summary - Monthly Trends")
        'cboReportType.Items.Add("PEPFAR Summary - Quarterly Trends")
        cboReportType.Items.Add("OVC Overall View")
        'cboReportType.Items.Add("Exits Summary")
        'cboReportType.Items.Add("School Going OVC Summary")
        'cboReportType.Items.Add("registration per month")
        cboReportType.Items.Add("CHV Reporting Rates Summary per month")
        cboReportType.Items.Add("OVC NOT Served list")
        cboReportType.Items.Add("Needs Vs Served by Domain")
        cboReportType.Items.Add("Needs Vs Served Detailed Summary")
        cboReportType.Items.Add("Comprehensive Summary")
        cboReportType.Items.Add("OVC [F1A] summary")
        cboReportType.Items.Add("Caregiver [F1B] summary")
        cboReportType.Items.Add("Caregiver [F1B] by OVC summary")
        cboReportType.Items.Add("Datim summary")
        cboReportType.Items.Add("PPT summary")
        cboReportType.Items.Add("ViralLoad summary")
    End Sub

    Private Sub populateclusters()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from clusters  order by clustername asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboClusters
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "clustername"
                .ValueMember = "clustercbos"
                .SelectedIndex = -1
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("Reports", "populateclusters", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populatedistricts()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from District where districtid in (" & strdistricts & ") order by district asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboDistrict
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "District"
                .ValueMember = "DistrictID"
                .SelectedIndex = -1
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("Reports", "populatedistricts", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populateCBO(ByVal mydistrict As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from CBO where districtid = '" & mydistrict.ToString & _
            "' and cboid in (" & strcbos & ") order by cbo asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboCBO
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "CBO"
                .ValueMember = "CBOID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("Reports", "populatecbo", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub cboDistrict_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboDistrict.SelectedIndexChanged
        Try
            'If cboDistrict.Text.Trim.Length <> 0 Or cboDistrict.Text <> "System.Data.DataRowView" Then
            If IsNumeric(cboDistrict.SelectedValue) = True Then
                populateCBO(cboDistrict.SelectedValue.ToString)
            End If

        Catch ex As Exception

        End Try
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

        If chkClusters.Checked = True Then
            'rptcboclusters = cboClusters.SelectedValue.ToString
            rptcbos = cboClusters.SelectedValue.ToString
            rptcboclustername = cboClusters.Text.ToString
        Else
            'rptcboclusters = ""
            'rptcbos = ""
        End If

        If Panel1.Enabled = True Then
            rptdistricts = cboDistrict.SelectedValue.ToString
            rptdistrictname = cboDistrict.Text.ToString
            rptcbos = cboCBO.SelectedValue.ToString
            rptcboname = cboCBO.Text.ToString

            'rptcboclusters = ""
            'rptcboclustername = ""
        Else
            'rptdistricts = ""
            'rptcbos = ""
        End If

        'If chkPriorityNeeds.Checked = True Then
        '    Dim dview As DataRowView
        '    For Each dview In lstPriorityNeeds.CheckedItems
        '        If rptpriorities.Length = 0 Then
        '            rptpriorities = "'" & dview.Row.Item(0).ToString() & "'"
        '        Else
        '            rptpriorities = rptpriorities & ",'" & dview.Row.Item(0).ToString() & "'"
        '        End If
        '    Next
        'Else
        '    'rptpriorities = ""
        'End If

        ''If NumMonthsUpDown.Visible = True Then
        'rptNeedsNumberofMonths = NumMonthsUpDown.Value
        ''End If

    End Sub

    Private Function validatecontrols() As Boolean
        Try
            ErrorProvider1.Clear()

            If cboReportType.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(cboReportType, "Please select a report to continue")
                MsgBox("Please select a report to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                cboReportType.Focus()
                Return False
            ElseIf chkClusters.Checked = True AndAlso cboClusters.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(cboClusters, "Please select a cluster continue")
                MsgBox("Please select a cluster to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                cboClusters.Focus()
                Return False
            ElseIf chkClusters.Checked = False AndAlso (cboDistrict.Text.Trim.Length = 0 Or cboCBO.Text.Trim.Length = 0) Then
                ErrorProvider1.SetError(cboDistrict, "Please select a district or cbo continue")
                MsgBox("Please select a district or cbo continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                cboDistrict.Focus()
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
        Dim timeMark As String = " " & Format(dtpFrom.Value, "ddMMMyyyy") & "-" & _
                Format(dtpTo.Value, "ddMMMyyyy") & " " & DateTime.Now.ToString("HHmmss")
        Select Case myreporttype

            Case "pepfar summary"

                TemplateFileName = "PepfarSummary"
                StoredProcName = "dbo.rpt_PepfarSummary"
               

            Case "pepfar detailed summary"

                TemplateFileName = "PepfarDetailedSummary"
                StoredProcName = "dbo.rpt_PepfarSummary_detailed"

            Case "pepfar summary - monthly trends"

                TemplateFileName = "PepfarTrendsSummary"
                StoredProcName = "dbo.rpt_PepfarSummary_trends"
                periodtype = "monthly"

            Case "pepfar summary - quarterly trends"

                TemplateFileName = "PepfarTrendsSummary"
                StoredProcName = "dbo.rpt_PepfarSummary_trends"
                periodtype = "quartely"

            Case "ovc overall view"
                TemplateFileName = "OVC_OverallView"
                StoredProcName = "dbo.rpt_Overallviewsummary"

            Case "exits summary"
                TemplateFileName = "ExitsSummary"
                StoredProcName = "dbo.rpt_exitssummary"

            Case "school going ovc summary"
                TemplateFileName = "SchoolGoingOVCsSummary"
                StoredProcName = "dbo.rpt_schoolgoingovcs"

            Case "registration per month"
                TemplateFileName = "Registrationpermonth"
                StoredProcName = "dbo.rpt_registrationpermonth"


            Case "chv reporting rates summary per month"
                TemplateFileName = "CHVReportingRatesSummary"
                StoredProcName = "dbo.rpt_chvreportingrates_summary"


            Case "needs vs served by domain"
                TemplateFileName = "NeedsVsServedSummary"
                StoredProcName = "dbo.rpt_NeedsVsServed"

            Case "needs vs served detailed summary"
                TemplateFileName = "NeedsVsServedDetailedSummary"
                StoredProcName = "dbo.rpt_NeedsVsServed_detailed"

            Case "ovc registration list"
                TemplateFileName = "ovcregistrationlist"
                StoredProcName = "dbo.rpt_Registration"

            Case "ovc registration list by chv"
                TemplateFileName = "ovcregistrationlistbychv"
                StoredProcName = "dbo.rpt_Registrationbychv_byHH"

            Case "ovc registration list by hh"
                TemplateFileName = "ovcregistrationlistbyHH"
                StoredProcName = "dbo.rpt_Registrationbychv_byHH"

            Case "ovc priority needs list"
                TemplateFileName = "ovcbeneficiarylist"
                StoredProcName = "dbo.rpt_beneficiarylist"

            Case "comprehensive summary"
                TemplateFileName = "ConsolidatedReportSummary"
                StoredProcName = "dbo.rpt_ConsolidatedReport"

            Case "caregiver [f1b] summary"
                TemplateFileName = "form1b_AssessmentsAndServices"
                StoredProcName = "dbo.rpt_form1b_AssessmentsAndServices"
            Case "caregiver [f1b] by ovc summary"
                TemplateFileName = "form1b_AssessmentsAndServices_OVC"
                StoredProcName = "dbo.rpt_form1b_AssessmentsAndServices_OVC"


            Case "ovc [f1a] summary"
                TemplateFileName = "form1a_Summary"
                StoredProcName = "dbo.rpt_f1a_Summary"

            Case "ovc not served list"
                TemplateFileName = "ovcnotservedlist"
                StoredProcName = "dbo.rpt_ovcnotserved"

            Case "datim summary"
                TemplateFileName = "datimsummary"
                StoredProcName = "dbo.rpt_datim"

            Case "ppt summary"
                TemplateFileName = "pptsummary"
                StoredProcName = "dbo.rpt_PPTSystemExport"

            Case "ovc csi analysis"
                TemplateFileName = "CSIAnalysis"
                StoredProcName = "dbo.rpt_csi"
            Case "viralload summary"
                TemplateFileName = "ViralLoadsummary"
                StoredProcName = "dbo.rpt_viralLoad"
        End Select

        'Template and Output report names and directories
        TemplateFilePath = Application.StartupPath & _
        "\TemplatesExcelReports\" & TemplateFileName & ".xlsm"

        If Directory.Exists(Application.StartupPath & _
            "\OutputExcelReports\" & Format(Date.Now, "dd-MMM-yyyy")) = False Then
            Dim di As DirectoryInfo = Directory.CreateDirectory(Application.StartupPath & _
            "\OutputExcelReports\" & Format(Date.Now, "dd-MMM-yyyy"))
        End If

        OutputFilePath = Application.StartupPath & "\OutputExcelReports\" & _
     Format(Date.Now, "dd-MMM-yyyy") & "\" & TemplateFileName & " " & timeMark & ".xlsm"

        'Populate data into relevant excel templates for reports
        Dim demoDataSet As DataSet = Me.getDataSet(StoredProcName, rptcbos.ToString, periodtype)

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

    Private Function getDataSet(ByVal mystoredproc As String, ByVal cbolist As String, ByVal periodtype As String) As DataSet
        Dim conn As New SqlClient.SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)


        Dim cmd As SqlCommand

        cmd = New SqlCommand(mystoredproc)


        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandTimeout = 1200

        'create storedproc parameters depending on which cbo or clusters have been selected

        If LCase(mystoredproc).Contains("trends") Then
            cmd.Parameters.Add(New SqlParameter("@periodtype", periodtype))
        End If


        If cbolist.ToString.Length = 0 Then
            cmd.Parameters.Add(New SqlParameter("@startdate", rptdatefrom.ToString))
            cmd.Parameters.Add(New SqlParameter("@enddate", rptdateto.ToString))
        Else
            cmd.Parameters.Add(New SqlParameter("@startdate", rptdatefrom.ToString))
            cmd.Parameters.Add(New SqlParameter("@enddate", rptdateto.ToString))
            cmd.Parameters.Add(New SqlParameter("@cbolist", cbolist.ToString))
        End If
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

    Private Sub chkClusters_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkClusters.CheckedChanged
        If chkClusters.Checked = True Then
            Panel1.Enabled = False
            cboClusters.Visible = True
        Else
            Panel1.Enabled = True
            cboClusters.Visible = False
        End If
    End Sub


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