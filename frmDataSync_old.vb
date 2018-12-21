Imports APHIAPlus.functions
Imports System.Data.Odbc
Imports APHIAPlus.ScriptAutomation
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient

Public Class frmDataSync_old

    Private Sub btnSync_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSync.Click
        Dim IsSuccess As Boolean = False
        '1. Attach Database from partners---------------'
        IsSuccess = AttachPartnerDb(AppSettings("AttachedDatabase").ToString, txtpartnermdf.Text.ToString)
        If IsSuccess = True Then
            PictureBox1.Visible = True
            PictureBox2.Visible = False
            MsgBox("Database Attachment SUCCESSFUL", MsgBoxStyle.Information)
        Else
            PictureBox2.Visible = True
            PictureBox1.Visible = False
            MsgBox("Attaching DB NOT successful", MsgBoxStyle.Exclamation)
            Exit Sub
        End If
        '------------------------------------------------

        '2.Syncronize Partner's data-------------------
        Dim conn As New SqlConnection(ConnectionStrings("AphiaPlusDBConnectionString").ToString)
        Dim cmd As New SqlCommand("dbo.SYNCOVCDATA")
        cmd.CommandType = CommandType.StoredProcedure
        cmd.CommandTimeout = 7200 'Keep connection open for 2hrs. It keeps taking longer and longer to do sync
        'cmd.Parameters.Add(New SqlParameter("@OVCID", myOVCID.ToString))
        ''cmd.Parameters.Add(New SqlParameter("@DateofVisit", dtpDateofVisit.Value))
        conn.Open()
        cmd.Connection = conn
        Dim myreader As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)

        PictureBox4.Visible = True
        PictureBox3.Visible = False
        MsgBox("Data Sync SUCCESSFUL", MsgBoxStyle.Information)
        '------------------------------------------------

        '3. Detach partner's DB-------------------------
        IsSuccess = detachpartnerdb(AppSettings("AttachedDatabase").ToString)
        If IsSuccess = True Then
            PictureBox6.Visible = True
            PictureBox5.Visible = False
            MsgBox("Database Detachment SUCCESSFUL", MsgBoxStyle.Information)
        Else
            PictureBox5.Visible = True
            PictureBox6.Visible = False
            MsgBox("Detaching Partner's DB NOT successful", MsgBoxStyle.Exclamation)
            Exit Sub
        End If
        '------------------------------------------------

        MsgBox("Data Synchronisation SUCCESSFUL", MsgBoxStyle.Information)

    End Sub
    Private Function PopulatePartnerDb() As Boolean
        Try

            Dim newscript As New FileOperations
            Dim executescript As New CreateDatabaseFixture
            Dim IsScriptSuccess As Boolean
            Dim strArguments As String

            Dim User As String = AppSettings("SQLEXPRESSUserName").ToString
            Dim Password As String = AppSettings("SQLEXPRESSPassWord").ToString
            Dim Server As String = AppSettings("SQLEXPRESSServerName").ToString
            Dim Database As String = AppSettings("CBODatabaseName").ToString
            Dim ScriptFolder As String = AppSettings("ScriptPath").ToString

            'now we can create the commandline arguments and run the script just created
            'If you are logged on with a user account that is trusted on the server that is running SQL Server Express, you can omit the -U and -P arguments:
            'strArguments = String.Format("-U ""{0}"" -P ""{1}"" -S ""{2}"" -d ""{3}"" -i ""{4}""", User, Password, Server, Database, AppSettings("ScriptPath").ToString & "attachdb.sql")
            strArguments = String.Format(" -S ""{2}"" -d ""{3}"" -i ""{4}""", User, Password, Server, Database, AppSettings("ScriptPath").ToString & "maindb.sql")
            IsScriptSuccess = executescript.ExecuteSqlCmdScript(AppSettings("ScriptPath").ToString & "maindb.sql", strArguments)

            If IsScriptSuccess = True Then
                Return True
            Else
                Return False
            End If


        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function AttachPartnerDb(ByVal attachdbname As String, ByVal attachdbfile As String) As Boolean
        Try

            Dim attachCon As New SqlConnection(ConnectionStrings("AphiaPlusMasterConnectionString").ToString)

            Dim cmd As New SqlCommand("sp_attach_db")
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandTimeout = 30
            cmd.Parameters.Add(New SqlParameter("@dbname", attachdbname.ToString))
            cmd.Parameters.Add(New SqlParameter("@filename1", attachdbfile.ToString))
            attachCon.Open()
            cmd.Connection = attachCon
            cmd.ExecuteNonQuery()

            Return True
            '=======================================================
            'Service provided by Telerik (www.telerik.com)
            'Conversion powered by NRefactory.
            'Twitter: @telerik, @toddanglin
            'Facebook: facebook.com/telerik
            '=======================================================
        Catch ex As Exception
            MsgBox(Err.Description & " AttachPartnerDB")
            Return False
        End Try
    End Function

    Private Function detachpartnerdb(ByVal attacheddb As String) As Boolean
        Try

            Dim attachCon As New SqlConnection(ConnectionStrings("AphiaPlusMasterConnectionString").ToString)

            Dim cmd As New SqlCommand("sp_detach_db")
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandTimeout = 30
            cmd.Parameters.Add(New SqlParameter("@dbname", attacheddb.ToString))
            'cmd.Parameters.Add(New SqlParameter("@filename1", attachdbfile.ToString))
            attachCon.Open()
            cmd.Connection = attachCon
            cmd.ExecuteNonQuery()

            Return True
            '=======================================================
            'Service provided by Telerik (www.telerik.com)
            'Conversion powered by NRefactory.
            'Twitter: @telerik, @toddanglin
            'Facebook: facebook.com/telerik
            '=======================================================
        Catch ex As Exception
            MsgBox(Err.Description & " DetachPartnerDB")
            Return False
        End Try
    End Function
    'Private Function AttachPartnerDb(ByVal attachdbname As String, ByVal attachdbfile As String) As Boolean
    '    Try

    '        Dim newscript As New FileOperations
    '        Dim executescript As New CreateDatabaseFixture
    '        Dim IsSuccess As Boolean
    '        Dim IsScriptSuccess As Boolean
    '        Dim strArguments As String

    '        Dim User As String = AppSettings("UserName").ToString
    '        Dim Password As String = AppSettings("PassWord").ToString
    '        Dim Server As String = AppSettings("MainServerName").ToString
    '        Dim Database As String = "Master" 'AppSettings("Database").ToString

    '        IsSuccess = newscript.writeattachdbscript(attachdbname, attachdbfile)



    '        'if creation is successful, write scripts in file
    '        If IsSuccess = True Then
    '            'now we can create the commandline arguments and run the script just created
    '            '----This one below has username and password parameters. Lets lenga those for now----
    '            '---strArguments = String.Format("-U ""{0}"" -P ""{1}"" -S ""{2}"" -d ""{3}"" -i ""{4}""", User, Password, Server, Database, AppSettings("ScriptPath").ToString & "attachdb.sql")
    '            strArguments = String.Format("-S ""{2}"" -d ""{3}"" -i ""{4}""", User, Password, Server, Database, AppSettings("ScriptPath").ToString & "attachdb.sql")

    '            'MsgBox("attachdbname:" & attachdbname & " attachdbfile:" & attachdbfile & " arguments:" & strArguments)

    '            IsScriptSuccess = executescript.ExecuteSqlCmdScript(AppSettings("ScriptPath").ToString & "attachdb.sql", strArguments)

    '            If IsScriptSuccess = True Then
    '                Return True
    '            Else
    '                Return False
    '            End If

    '        Else
    '            Return False

    '        End If
    '    Catch ex As Exception
    '        MsgBox(Err.Description & " AttachPartnerDB")
    '        Return False
    '    End Try
    'End Function

    'Private Function detachpartnerdb(ByVal attacheddb As String) As Boolean
    '    Try

    '        Dim newscript As New FileOperations
    '        Dim executescript As New CreateDatabaseFixture
    '        Dim IsSuccess As Boolean
    '        Dim IsScriptSuccess As Boolean
    '        Dim strArguments As String

    '        Dim User As String = AppSettings("UserName").ToString
    '        Dim Password As String = AppSettings("PassWord").ToString
    '        Dim Server As String = AppSettings("MainServerName").ToString
    '        Dim Database As String = "Master" 'AppSettings("AttachedDatabase").ToString

    '        IsSuccess = newscript.writedetachdbscript(attacheddb)

    '        'if creation is successful, write scripts in file
    '        If IsSuccess = True Then
    '            'now we can create the commandline arguments and run the script just created
    '            '----This one below has username and password parameters. Lets lenga those for now----
    '            '---strArguments = String.Format("-U ""{0}"" -P ""{1}"" -S ""{2}"" -d ""{3}"" -i ""{4}""", User, Password, Server, Database, AppSettings("ScriptPath").ToString & "attachdb.sql")
    '            strArguments = String.Format("-S ""{2}"" -d ""{3}"" -i ""{4}""", User, Password, Server, Database, AppSettings("ScriptPath").ToString & "detachdb.sql")
    '            IsScriptSuccess = executescript.ExecuteSqlCmdScript(AppSettings("ScriptPath").ToString & "detachdb.sql", strArguments)

    '            If IsScriptSuccess = True Then
    '                Return True
    '            Else
    '                Return False
    '            End If

    '        Else
    '            Return False

    '        End If
    '    Catch ex As Exception
    '        MsgBox(Err.Description)
    '        Return False
    '    End Try
    'End Function

    Private Function ScriptMainDb() As Boolean
        Try

            Dim newscript As New FileOperations
            Dim executescript As New CreateDatabaseFixture
            Dim IsScriptSuccess As Boolean
            Dim strArguments As String

            Dim User As String = AppSettings("UserName").ToString
            Dim Password As String = AppSettings("PassWord").ToString
            Dim Server As String = AppSettings("MainServerName").ToString
            Dim Database As String = AppSettings("MainDatabase").ToString

            'now we can create the commandline arguments and run the script just created
            strArguments = String.Format(" script -d {3} -S {2} -U {0} -P {1} -targetserver 2008 ""{4}"" -f", User, Password, Server, Database, AppSettings("ScriptPath").ToString & "maindb.sql")
            IsScriptSuccess = executescript.ExecuteSqlPubWizScript(AppSettings("ScriptPath").ToString & "maindb.sql", strArguments)

            If IsScriptSuccess = True Then
                Return True
            Else
                Return False
            End If


        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function CreateBlankCBODb() As Boolean
        Try

            Dim newscript As New FileOperations
            Dim executescript As New CreateDatabaseFixture

            Dim IsScriptSuccess As Boolean

            IsScriptSuccess = executescript.CreateBlankDatabase

            If IsScriptSuccess = True Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub frmDataSync_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        populatedistricts()
    End Sub

    Private Sub populatedistricts()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from District order by district asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cboDistrict
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "District"
                .ValueMember = "DistrictID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("datasync", "populatedistricts", ex.Message) ''---Write error to error log file

        End Try
    End Sub
    Private Sub populateCBO(ByVal mydistrict As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from CBO where districtid = '" & mydistrict.ToString & "' order by cbo asc"
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
            ErrorAction.WriteToErrorLogFile("datasync", "populatecbo", ex.Message) ''---Write error to error log file

        End Try
    End Sub


    Private Sub cboDistrict_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboDistrict.SelectedIndexChanged
        Try
            'If cboDistrict.Text.Trim.Length <> 0 Or cboDistrict.Text <> "System.Data.DataRowView" Then
            If IsNumeric(cboDistrict.SelectedValue) = True Then
                populateCBO(cboDistrict.SelectedValue.ToString)

            End If

        Catch ex As Exception

        End Try
    End Sub


    Private Sub btnfilterCBOData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnfilterCBOData.Click
        Dim IsSuccess As Boolean = False
        '1. Scripting Main DB----------------------------
        IsSuccess = ScriptMainDb()
        If IsSuccess = True Then
            PictureBox12.Visible = True
            PictureBox11.Visible = False
        Else
            PictureBox11.Visible = True
            PictureBox12.Visible = False
            MsgBox("Scripting MainDB NOT successful", MsgBoxStyle.Exclamation)
            Exit Sub
        End If
        '-------------------------------------------------
        '2. Create blank DB----------------------------
        IsSuccess = CreateBlankCBODb()
        If IsSuccess = True Then
            PictureBox10.Visible = True
            PictureBox9.Visible = False
        Else
            PictureBox9.Visible = True
            PictureBox10.Visible = False
            MsgBox("Creating Blank DB NOT successful", MsgBoxStyle.Exclamation)
            Exit Sub
        End If
        '-------------------------------------------------
        '3. Populate new CBO DB with Main DB data using maindb.sql script---------------'
        IsSuccess = PopulatePartnerDb()
        If IsSuccess = True Then
            PictureBox8.Visible = True
            PictureBox7.Visible = False
        Else
            PictureBox7.Visible = True
            PictureBox8.Visible = False
            MsgBox("Populating CBO DB NOT successful", MsgBoxStyle.Exclamation)
            Exit Sub
        End If
        '------------------------------------------------
    End Sub

    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        OpenFileDialog1.Title = "Please Select a File"
        'OpenFileDialog1.InitialDirectory = "C:OVC"

        OpenFileDialog1.ShowDialog()

    End Sub

    Private Sub OpenFileDialog1_FileOk(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk

        'Dim strm As System.IO.Stream
        'strm = OpenFileDialog1.OpenFile()
        txtpartnermdf.Text = OpenFileDialog1.FileName.ToString()
        btnSync.Enabled = True

    End Sub
End Class