Imports System.Windows.Forms
Imports System.Configuration.ConfigurationManager
Imports System.Xml
Imports System.IO
Imports OVCSystem.functions
Imports System.Data.SqlClient
Imports System.Data.Odbc
Imports Excel
Imports System.Net

Public Class MDIMain

    Delegate Sub ChangeTextsSafe(ByVal length As Long, ByVal position As Integer, ByVal percent As Integer, ByVal speed As Double)
    Delegate Sub DownloadCompleteSafe(ByVal cancelled As Boolean)
    Delegate Sub OlmisUpdates()






    Public Sub ChangeTexts(ByVal length As Long, ByVal position As Integer, ByVal percent As Integer, ByVal speed As Double)

        'Me.Label3.Text = "File Size: " & Math.Round((length / 1024), 2) & " KB"

        'Me.Label5.Text = "Downloading: " & Me.txtFileName.Text

        'Me.Label4.Text = "Downloaded " & Math.Round((position / 1024), 2) & " KB of " & Math.Round((length / 1024), 2) & "KB (" & Me.ProgressBar1.Value & "%)"

        'If speed = -1 Then
        '    Me.Label2.Text = "Speed: calculating..."
        'Else
        '    Me.Label2.Text = "Speed: " & Math.Round((speed / 1024), 2) & " KB/s"
        'End If

        ' Me.ToolStripProgressBar1.Value = percent

       


    End Sub
    'Dim whereToSave As String = Application.StartupPath & _
    '"\OlmisUpdates" 'Where the program save the file
    Private Sub ShowNewForm(ByVal sender As Object, ByVal e As EventArgs) Handles Biodata.Click
        Try
            m_RegistrationFormNumber += 1

            If m_RegistrationFormNumber > 1 Then
                MsgBox("Form already opened.", MsgBoxStyle.Exclamation)
                Exit Sub
            End If
            ' Create a new instance of the child form.
            clientchildform = New frmOVCInfo
            ' Make it a child of this MDI form before showing it.
            'clientchildform.MdiParent = Me


            clientchildform.Text = "OVC Registration" '"Window " & m_ChildFormNumber



            clientchildform.Show()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub OpenFile(ByVal sender As Object, ByVal e As EventArgs)
        Dim OpenFileDialog As New OpenFileDialog
        OpenFileDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        OpenFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
        If (OpenFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
            Dim FileName As String = OpenFileDialog.FileName
            ' TODO: Add code here to open the file.
        End If
    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim SaveFileDialog As New SaveFileDialog
        SaveFileDialog.InitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        SaveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"

        If (SaveFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
            Dim FileName As String = SaveFileDialog.FileName
            ' TODO: Add code here to save the current contents of the form to a file.
        End If
    End Sub


    Private Sub ExitToolsStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Closemenu.Click
        Me.Close()
    End Sub

    Private Sub CloseAllToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Close all child forms of the parent.
        For Each ChildForm As Form In Me.MdiChildren
            ChildForm.Close()
        Next
    End Sub











    Public Sub GetMenues(ByVal Current As ToolStripItem, ByRef menues As List(Of ToolStripItem))
        menues.Add(Current)
        If TypeOf (Current) Is ToolStripMenuItem Then
            For Each menu As ToolStripItem In DirectCast(Current, ToolStripMenuItem).DropDownItems
                GetMenues(menu, menues)
            Next
        End If
    End Sub

    Private Sub MDIMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        ''Update session record with timeout
        'Dim mySqlAction As String = ""
        'Dim MyDBAction As New functions
        'mySqlAction = "update audittrail set timeout = " & _
        '" '" & Date.Now & "' where sessionid = '" & strsession.ToString & "'"
        'MyDBAction.DBAction(mySqlAction, functions.DBActionType.Update)

        ''end whole application. Without this, non mdi-children forms remain hanging
        'End
    End Sub

    Private Sub MDIMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)

        Try

            'first disable all menus
            Dim menues As New List(Of ToolStripItem)
            For Each t As ToolStripItem In Me.MenuStrip.Items
                GetMenues(t, menues)
            Next

            For Each t As ToolStripItem In menues
                't.Enabled = False
                t.Enabled = True
            Next

            ''enable menus depending on user rights
            Dim mySqlAction As String = "select * from userrights where username = '" & strusername.ToString &
            "' and canaccess = 'True'"
            Dim MyDBAction As New functions
            'Dim MyDatable As New Data.DataTable
            Dim mydecryptedpass As String = ""
            'MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            Dim cmd As New SqlCommand(mySqlAction, conn)
            conn.Open()
            Dim myreader As SqlDataReader = cmd.ExecuteReader()

            'If MyDatable.Rows.Count > 0 Then
            '    For K = 0 To MyDatable.Rows.Count - 1
            Do While myreader.Read
                For Each t As ToolStripItem In menues
                    If myreader("menuid").ToString = t.Name Then
                        t.Enabled = True
                    End If

                Next
            Loop






            'backrgound image
            Me.BackgroundImage = System.Drawing.Image.FromFile(Application.StartupPath &
                "\images\systemlogo.png")
            Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch

            'write on the status bar
            StatusStrip.Items.Add("User Name: " & strusername.ToString & "          SessionID: " & strsession.ToString &
            "          Machine Name: " & strmachinename & "          Partner Name: --") ' & strimplementingpartner.ToString.ToUpper

            'save session details in database for audit trail--

            mySqlAction = "Insert Into audittrail(sessionid,username,machinename,timein) " &
            " values('" & strsession.ToString & "','" & strusername.ToString & "','" & strmachinename.ToString &
            "'," & Date.Now.ToShortDateString & ")"
            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
            '-----



            'VNavPane1.Height = Me.Height - 110

            Me.KeyPreview = True

        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub





    Private Sub ToolStripReports_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub DataSyncToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        ' Create a new instance of the child form.
        Dim ChildForm As New frmDataSync
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "Synchronize Data" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub



    Private Sub ManageClustersToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ManageClusters.Click
        ' Create a new instance of the child form.
        Dim ChildForm As New frmMaintainClusters
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "Maintain Clusters" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub

    Private Sub UserManagementToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UserManagement.Click
        ' Create a new instance of the child form.
        Dim ChildForm As New frmUserManagement
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "Manage Users" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub


    Private Sub CompileDateToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub CHWNotInSystemfixToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        Dim mySqlAction As String = ""
        Dim MyDBAction As New functions

        'Fetch all CHWs who exist in ClientDetails but not in CHW table


        If MsgBox("Confirm Insertion of Non-excistent CHWs", MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel) = vbOK Then

            'mySqlAction = "select * from clientdetails where volunteerid not in (select chwid as volunteerid from chw)"
            mySqlAction = "SELECT DISTINCT Clientdetails.VolunteerId, CBO.CBO, CBO.CBOID, CBO.DistrictID " & _
                            " FROM  CBO INNER JOIN " & _
                            " Clientdetails ON CBO.CBOID = Clientdetails.Cbo " & _
                            " WHERE (Clientdetails.VolunteerId NOT IN " & _
                          " (SELECT CHWID AS volunteerid " & _
                           "  FROM  CHW)) "

            Dim cmd As New SqlCommand(mySqlAction, conn)
            conn.Open()
            Dim myreader As SqlDataReader = cmd.ExecuteReader()
            Dim strchwid As String = ""
            Dim strcboid As String = ""
            Dim recordcount As Double = 0
            Do While myreader.Read
                strchwid = myreader("VolunteerId").ToString
                strcboid = myreader("cboid").ToString
                'Insert a record in CHW
                mySqlAction = "INSERT INTO [CHW] " & _
                "([CHWID] " & _
                ",[FirstName] " & _
                ",[MiddleName] " & _
                ",[Surname] " & _
                ",[ID] " & _
                ",[CBOID]) " & _
                 "VALUES" & _
               "('" & strchwid & " ' " & _
               ",'NOT IN SYSTEM' " & _
               ",'-' " & _
               ",'-' " & _
               ",'00' " & _
               ",'" & strcboid & "') "

                MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)

                recordcount = recordcount + 1
            Loop

            MsgBox(recordcount.ToString & " [CHW] records updated.")

            conn.Close()
        Else
            MsgBox("No records to insert")
        End If
    End Sub

    Private Sub CBOsMovingDistrictsfixToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        Dim mySqlAction As String = ""
        Dim MyDBAction As New functions

        If MsgBox("Confirm Update of inconsistent districts", MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel) = vbOK Then

            'mySqlAction = "select * from clientdetails where volunteerid not in (select chwid as volunteerid from chw)"
            mySqlAction = "select * from cbo where cboid in( " & _
            " Select distinct(CBO.CBOID) " & _
            " FROM  Clientdetails INNER JOIN " & _
             "  CBO ON Clientdetails.Cbo = CBO.CBOID " & _
            " where(clientdetails.cbo = CBO.cboid And clientdetails.district <> CBO.districtid) " & _
            " ) "

            Dim cmd As New SqlCommand(mySqlAction, conn)
            conn.Open()
            Dim myreader As SqlDataReader = cmd.ExecuteReader()
            Dim strcboid As String = ""
            Dim strdistrictid As String = ""

            Dim recordcount As Double = 0
            Do While myreader.Read
                strcboid = myreader("cboid").ToString
                strdistrictid = myreader("districtid").ToString
                mySqlAction = "UPDATE [Clientdetails] " & _
                               "SET [District] = '" & strdistrictid & "' " & _
                             "WHERE cbo = '" & strcboid & "' and district <> '" & strdistrictid & "'"

                MyDBAction.DBAction(mySqlAction, functions.DBActionType.Update)

                recordcount = recordcount + 1
            Loop

            MsgBox(recordcount.ToString & " Records updated.")

            conn.Close()
        Else
            MsgBox("No records to update")
        End If

    End Sub


    Private Sub MDIMain_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        ' VNavPane1.Height = Me.Height - 110
    End Sub



    Private Sub UploadSchoolsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        Try
            Dim confirm As DialogResult
            'Displays the MessageBox
            confirm = MsgBox("Confirm Uploading of Schools.", MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel)


            ' Gets the result of the MessageBox display.
            If confirm = DialogResult.OK Then
                'loop thru districts
                'enable menus depending on user rights
                Dim mySqlAction As String = "select * from district order by district"
                Dim MyDBAction As New functions
                'Dim MyDatable As New Data.DataTable
                Dim mydecryptedpass As String = ""
                'MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

                '4. DataSet - Create column names from first row


                Dim cmd As New SqlCommand(mySqlAction, conn)
                conn.Open()
                Dim myreader As SqlDataReader = cmd.ExecuteReader()

                Do While myreader.Read
                    Dim stream As FileStream = File.Open(My.Application.Info.DirectoryPath & "\SCHOOLS.xls", FileMode.Open, FileAccess.Read)

                    '1. Reading from a binary Excel file ('97-2003 format; *.xls)
                    Dim excelReader As IExcelDataReader = ExcelReaderFactory.CreateBinaryReader(stream)
                    '...
                    '2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
                    'Dim excelReader As IExcelDataReader = ExcelReaderFactory.CreateOpenXmlReader(stream)
                    '...
                    '3. DataSet - The result of each spreadsheet will be created in the result.Tables
                    'Dim result As DataSet = excelReader.AsDataSet()
                    '...
                    excelReader.IsFirstRowAsColumnNames = True
                    Dim result As DataSet = excelReader.AsDataSet()
                    '5. Data Reader methods

                    While excelReader.Read()
                        'MsgBox(excelReader.GetString(0)) 'schoolname
                        'MsgBox(excelReader.GetString(1))
                        'MsgBox(excelReader.GetString(2)) 'district
                        'MsgBox(excelReader.GetString(3)) 'schoollevel

                        If UCase(excelReader.GetString(2)).ToString = UCase(myreader("district")).ToString Then
                            'know what school level to save
                            Dim strschoollevel As Integer
                            Select Case LCase(excelReader.GetString(3))
                                Case "nursery"
                                    strschoollevel = 1
                                Case "primary"
                                    strschoollevel = 2
                                Case "secondary"
                                    strschoollevel = 3
                                Case "tertiary"
                                    strschoollevel = 4
                            End Select

                            mySqlAction = "Insert Into schools(schoolname,district,schoollevel) " & _
                                        " values('" & Replace(UCase(excelReader.GetString(0)), "'", "") & "','" & myreader("districtid") & "','" & strschoollevel.ToString & _
                                        "')"
                            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
                        End If


                    End While

                    '6. Free resources (IExcelDataReader is IDisposable)
                    excelReader.Close()
                Loop


                MsgBox("Schools upload complete.", MsgBoxStyle.Information)
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    Private Sub CleanupEligibilityToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        Try

            Dim confirm As DialogResult
            'Displays the MessageBox
            confirm = MsgBox("Confirm Eligibility Cleanup.", MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel)


            ' Gets the result of the MessageBox display.
            If confirm = DialogResult.OK Then

                '1. check any integer less than 10
                Dim mySqlAction As String = "select clientid,clienttype from clientdetails where clienttype < 10 order by clientid"
                Dim MyDBAction As New functions
                Dim cmd As New SqlCommand(mySqlAction, conn)
                conn.Open()
                Dim myreader As SqlDataReader = cmd.ExecuteReader()
                Dim strPreviousCriteria As String = ""

                Do While myreader.Read
                    'generate a number to reference and relate criteria chosen[table clientcriteria] and clientdetails table
                    '2. Insert  [above] in clienteligibility plus [older criteria]
                    'Dim Myrandomkey As New Random
                    Dim strCriteria As String
                    'strCriteria = Format(Date.Today, "yyyyMMddhhss") & Myrandomkey.Next(9000)
                    strCriteria = Format(Date.Today, "yyyyMMddhhss") & myreader("clientid").ToString


                    mySqlAction = "Insert Into ClientCriteria(ClientCriteria,Criteria) " & _
                    " values('" & strCriteria.ToString & "','" & myreader("clienttype").ToString & "')"
                    MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)

                    '3. update clientdetails table
                    mySqlAction = "update clientdetails set clienttype = " & _
            " '" & strCriteria.ToString & "' where clientid = " & myreader("clientid").ToString & ""
                    MyDBAction.DBAction(mySqlAction, functions.DBActionType.Update)


                    strCriteria = ""
                Loop

                MsgBox("Eligibility Cleanup Complete.", MsgBoxStyle.Information)


            End If
        Catch ex As Exception
            MsgBox("Eligibility Cleanup NOT Complete.", MsgBoxStyle.Exclamation)
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub




    Private Sub ExportMaintenanceData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExportMaintenanceData.Click
        ' Create a new instance of the child form.
        Dim ChildForm As New frmExportMaintenanceData
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "Export Maintenance Data" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub

    Private Sub ImportMaintenanceData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ImportMaintenanceData.Click
        ' Create a new instance of the child form.
        Dim ChildForm As New frmImportMaintenanceData
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "Import Maintenance Data" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub






    Private Sub ExcelReports_Click(sender As Object, e As EventArgs) Handles ExcelReports.Click
        m_excelreportFormNumber += 1

        If m_excelreportFormNumber > 1 Then
            MsgBox("Form already opened.", MsgBoxStyle.Exclamation)
            Exit Sub
        End If
        ' Create a new instance of the child form.
        Dim ChildForm As New frmExcelReports
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        ChildForm.Text = "Excel Reports" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub

   

    Public Shared Function CheckForInternetConnection() As Boolean
        Try
            If My.Computer.Network.Ping("www.Google.com") = True Then
                Return True
            Else
                Return False
            End If
            '        Using client = New WebClient()
            '            Using stream = client.OpenRead("http://www.google.com")
            '                Return True
            '            End Using
            '        End Using
        Catch
            Return False
        End Try
    End Function





    Private Function ReadVersionFile(ByVal strVersionfile As String) As Int64
        Try
            ' Create an instance of StreamReader to read from a file.
            Using sr As StreamReader = New StreamReader(strVersionfile)
                Dim line As String
                ' Read and display the lines from the file until the end 
                ' of the file is reached.
                'Do
                line = sr.ReadLine()
                Return CInt(line)
                'Console.WriteLine(line)
                'Loop Until line Is Nothing
            End Using
        Catch E As Exception

            ' Let the user know what went wrong.
            'MsgBox(E.Message, MsgBoxStyle.Exclamation)
            Return 0
        End Try
    End Function

    'Public Sub ChangeTexts(ByVal length As Long, ByVal position As Integer, ByVal percent As Integer, ByVal speed As Double)

    '    ToolStripProgressBar1.Value = percent

    'End Sub





    Private Sub MDIMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        ' When the user presses  the 'ALT' key and 'CTRL' key  and 'SHIFT' key and letter 'O',
        ' KeyPreview is set to False, and a data entry extension window appears.
        ' By default, keypreview is turned back to False, so we set it to true again to allow a second capture of these keys
        If e.Alt And e.Control And e.Shift And e.KeyCode.ToString = "O" Then
            ' Create a new instance of the child form.
            Dim ChildForm As New frmDateExtensions
            ' Make it a child of this MDI form before showing it.
            ChildForm.MdiParent = Me

            m_ChildFormNumber += 1
            ChildForm.Text = "Provide deadline extension for data entry" '"Window " & m_ChildFormNumber

            ChildForm.Show()

            Me.KeyPreview = True

        ElseIf e.Alt And e.Control And e.KeyValue = 35 Then 'Ctr+Alt+End
            ' Create a new instance of the child form.
            Dim ChildForm As New frmChangePassword
            ' Make it a child of this MDI form before showing it.
            ChildForm.MdiParent = Me

                m_ChildFormNumber += 1
            ChildForm.Text = "Change Passwords" '"Window " & m_ChildFormNumber

            ChildForm.Show()

                Me.KeyPreview = True




        End If

    End Sub


    Private Sub ToolStripbtnChangepassword_Click(sender As Object, e As EventArgs) Handles ToolStripbtnChangepassword.Click
        ' Create a new instance of the child form.
        Dim ChildForm As New frmChangePassword
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "Change Passwords" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub

    Private Sub VSLAListing_Click(sender As Object, e As EventArgs) Handles VSLAListing.Click
        ' Create a new instance of the child form.
        Dim ChildForm As New frmVSLAListing
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "VSLA Listing" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub

    Private Sub VSLAmembership_Click(sender As Object, e As EventArgs) Handles VSLAmembership.Click
        ' Create a new instance of the child form.
        Dim ChildForm As New frmVsla_membership
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "VSLA Membership" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub

    Private Sub ValueChains_Click(sender As Object, e As EventArgs) Handles ValueChains.Click
        ' Create a new instance of the child form.
        Dim ChildForm As New frmValueChainsListings
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "ValueChains Listing" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub

    Private Sub StarterKits_Click(sender As Object, e As EventArgs) Handles StarterKits.Click
        ' Create a new instance of the child form.
        Dim ChildForm As New frmStarterKitListing
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "StarterKit Listing" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub

    Private Sub TrainingsListing_Click(sender As Object, e As EventArgs) Handles TrainingsListing.Click
        ' Create a new instance of the child form.
        Dim ChildForm As New frmTrainingListing
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "Training Listing" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub

    Private Sub CaregiverStarterKits_Click(sender As Object, e As EventArgs) Handles CaregiverStarterKits.Click
        ' Create a new instance of the child form.
        Dim ChildForm As New frmCaregiverStarterKits
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "Caregiver StarterKits" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub

    Private Sub CaregiverValueChains_Click(sender As Object, e As EventArgs) Handles CaregiverValueChains.Click
        ' Create a new instance of the child form.
        Dim ChildForm As New frmCaregiverValueChains
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "Caregiver ValueChains" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub

    Private Sub TrainingAttendance_Click(sender As Object, e As EventArgs) Handles TrainingAttendance.Click
        ' Create a new instance of the child form.
        Dim ChildForm As New frmTrainingAttendance
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "Training Attendance" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub

    Private Sub ImportCPIMSData_Click(sender As Object, e As EventArgs) Handles ImportCPIMSData.Click
        ' Create a new instance of the child form.
        Dim ChildForm As New frmDataSync
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "Import CPIMS Data" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub

    Private Sub VSLALoanTracking_Click(sender As Object, e As EventArgs) Handles VSLALoanTracking.Click
        ' Create a new instance of the child form.
        Dim ChildForm As New frmVslaTracking
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "VSLA loans tracking" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub

    Private Sub MessageDissemination_Click(sender As Object, e As EventArgs) Handles MessageDissemination.Click
        ' Create a new instance of the child form.
        Dim ChildForm As New frmFCIMessageDissemination
        ' Make it a child of this MDI form before showing it.
        ChildForm.MdiParent = Me

        m_ChildFormNumber += 1
        ChildForm.Text = "MESSAGES DISSEMINATION TO ORGANIZED GROUPS" '"Window " & m_ChildFormNumber

        ChildForm.Show()
    End Sub

    Private Sub FCIReports_Click(sender As Object, e As EventArgs) Handles FCIReports.Click

    End Sub
End Class
