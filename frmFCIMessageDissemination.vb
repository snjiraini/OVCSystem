Imports OVCSystem.functions
Imports System.Data.Odbc
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Text
Public Class frmFCIMessageDissemination
    Dim global_message_dissemination_id As String = ""
    Private Sub frmFCIMessageDissemination_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        populatesearchcounties()
        populatecounties()
        populatetypeofgroup()
        populatemessagetype()
    End Sub
    Private Sub populatesearchcounties()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select distinct county from OVCRegistrationDetails where cbo_id in (" & strcbos & ")  order by county asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cbosearchcounty
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "County"
                .ValueMember = 0
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With


        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("FCI Message Dissemination", "populatecounties", ex.Message) ''---Write error to error log file

        End Try
    End Sub
    Private Sub populatecounties()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select distinct county from OVCRegistrationDetails where cbo_id in (" & strcbos & ")  order by county asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cboCounty
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "County"
                .ValueMember = 0
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With


        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("FCI Message Dissemination", "populatecounties", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populateLIPs(ByVal strcounties As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "SELECT DISTINCT LIPs.LIP FROM   OVCRegistrationDetails INNER JOIN LIPs ON " &
                " OVCRegistrationDetails.cbo = LIPs.cbo where county = '" & strcounties & "'  order by LIP asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cboLIP
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "LIP"
                .ValueMember = 0
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With


        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("FCI Message Dissemination", "populateLIps ", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populatesearchLIPs(ByVal strcounties As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "SELECT DISTINCT LIPs.LIP FROM   OVCRegistrationDetails INNER JOIN LIPs ON " &
                " OVCRegistrationDetails.cbo = LIPs.cbo where county = '" & strcounties & "'  order by LIP asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cbosearchLIP
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "LIP"
                .ValueMember = 0
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With


        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("FCI Message Dissemination", "populateLips", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub cboCounty_SelectedValueChanged(sender As Object, e As EventArgs) Handles cboCounty.SelectedValueChanged
        Try
            'If cboDistrict.Text.Trim.Length <> 0 Or cboDistrict.Text <> "System.Data.DataRowView" Then
            If IsNumeric(cboCounty.SelectedIndex) = True Then
                populateLIPs(cboCounty.Text.ToString)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub cbosearchCounty_SelectedValueChanged(sender As Object, e As EventArgs) Handles cbosearchcounty.SelectedValueChanged
        Try
            'If cboDistrict.Text.Trim.Length <> 0 Or cboDistrict.Text <> "System.Data.DataRowView" Then
            If IsNumeric(cbosearchcounty.SelectedIndex) = True Then
                populatesearchLIPs(cbosearchcounty.Text.ToString)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub populatetypeofgroup()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "SELECT [type_of_group_id]" &
                                        " ,[type_of_group] FROM [dbo].[FCI_type_of_group]  order by type_of_group asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cboTypeofGroup
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "type_of_group"
                .ValueMember = "type_of_group_id"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With


        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("FCI Message Dissemination", "populatetypeofgroup", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populatemessagetype()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "SELECT distinct [message_type] FROM [dbo].[FCI_message_type]"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cboMessageType
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "message_type"
                .ValueMember = "0"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With


        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("FCI Message Dissemination", "populatemessagetype", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populatemessage(ByVal strmessagetype As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "SELECT [message_type_id],[message] FROM [dbo].[FCI_message_type] where message_type = '" & strmessagetype & "' order by message"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cboMessage
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "message"
                .ValueMember = "message_type_id"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With


        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("FCI Message Dissemination", "populatemessage", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub cboMessageType_SelectedValueChanged(sender As Object, e As EventArgs) Handles cboMessageType.SelectedValueChanged
        Try
            'If cboDistrict.Text.Trim.Length <> 0 Or cboDistrict.Text <> "System.Data.DataRowView" Then
            If IsNumeric(cboMessageType.SelectedIndex) = True Then
                populatemessage(cboMessageType.Text.ToString)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub btnpost_Click(sender As Object, e As EventArgs) Handles btnpost.Click
        Dim ErrorAction As New functions
        Dim myrandomkey As New Random
        Try
            '----Save Message disseminationd details [parent record]-------
            '      [Msg_dissemination_id]
            ',[group_name]
            ',[type_of_group]
            ',[dissemination_date]
            ',[total_sessions]
            ',[LIP]
            ',[County]

            'save record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            Dim myMsg_dissemination_id As String = "msgdid_" & Format(Date.Today, "yyyyMMddhhss") & myrandomkey.Next(9000)

            mySqlAction = "INSERT INTO [dbo].[FCI_message_dissemination] " &
                               "([Msg_dissemination_id],[group_name] " &
                               ",[type_of_group] " &
                               ",[type_of_message] " &
                               ",[message] " &
                               ",[dissemination_date] " &
                               ",[total_sessions] " &
                               ",[LIP] " &
                               ",[County]) " &
                         "VALUES " &
                               "('" & myMsg_dissemination_id & "'" &
                               ",'" & txtgroupname.Text.ToUpper & "'" &
                               ",'" & cboTypeofGroup.Text.ToUpper & "'" &
                               ",'" & cboMessageType.Text.ToUpper & "'" &
                               ",'" & cboMessage.Text.ToUpper & "'" &
                               ",'" & Format(dtpDisseminationDate.Value, "dd-MMM-yyyy") & "'" &
                               ",'0' " &
                               ",'" & cboLIP.Text.ToString & "'" &
                               ",'" & cboCounty.Text.ToString & "') "

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)



            '-----Save Message Dessemination clients [loop through grid]
            Dim MyDatable As New Data.DataTable
            Dim myclient_id, myfirstname, mymiddlename, mysurname, mygender, myemailaddress As String
            Dim myage As Integer
            Dim mytelephoneno As Int64


            If DataGridView2.Rows.Count > 0 Then
                For K = 0 To DataGridView2.Rows.Count - 1
                    If (DataGridView2.Rows(K).Cells(1).Value = Nothing) = False Then
                        myclient_id = "clientid_" & Format(Date.Today, "yyyyMMddhhss") & myrandomkey.Next(9000)
                        myfirstname = DataGridView2.Rows(K).Cells(1).Value.ToString.ToUpper
                        mymiddlename = DataGridView2.Rows(K).Cells(2).Value.ToString.ToUpper
                        mysurname = DataGridView2.Rows(K).Cells(3).Value.ToString.ToUpper
                        mygender = DataGridView2.Rows(K).Cells(4).Value.ToString
                        myage = DataGridView2.Rows(K).Cells(5).Value.ToString
                        mytelephoneno = DataGridView2.Rows(K).Cells(6).Value.ToString
                        myemailaddress = DataGridView2.Rows(K).Cells(7).Value.ToString

                        mySqlAction = "INSERT INTO [dbo].[FCI_clients] " &
                                       "([client_id],[firstname] " &
                                       ",[middlename] " &
                                       ",[surname] " &
                                       ",[gender] " &
                                       ",[age] " &
                                       ",[telephoneNo] " &
                                       ",[emailaddress] " &
                                       ") " &
                                 "VALUES " &
                                      "('" & myclient_id & "'" &
                                      ",'" & myfirstname & "' " &
                                       ",'" & mymiddlename & "' " &
                                       ",'" & mysurname & "' " &
                                      ",'" & mygender & "' " &
                                       ",'" & myage & "' " &
                                       ",'" & mytelephoneno & "' " &
                                       ",'" & myemailaddress & "' " &
                                       ")"


                        MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)

                        '-----Save/Link clients to message dissemination
                        Dim myclient_message_dissemination_id As String
                        myclient_message_dissemination_id = "cmdid_" & Format(Date.Today, "yyyyMMddhhss") & myrandomkey.Next(9000)
                        mySqlAction = "INSERT INTO [dbo].[FCI_client_message_dissemination] " &
                                               "([ID] " &
                                               ",[Msg_dissemination_id] " &
                                               ",[client_id]) " &
                                        " VALUES " &
                                               "('" & myclient_message_dissemination_id & "' " &
                                               ",'" & myMsg_dissemination_id & "'" &
                                               ",'" & myclient_id & "'" &
                                       " )"


                        MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)

                    End If
                Next

            End If



            MsgBox("Record saved successfully.", MsgBoxStyle.Information)
            GroupBox1.Enabled = False
            DataGridView2.Rows.Clear()
            DataGridView2.Enabled = False
            btnpost.Enabled = False
            '**TO DO  *********fillgrid()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation) ''---Write error to error log file

        End Try
    End Sub

    Private Sub lnkNew_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkNew.LinkClicked

        clearcontrols()
        DataGridView2.Rows.Clear()
        GroupBox1.Enabled = True
        DataGridView2.Enabled = True
        btnpost.Enabled = True

    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim ErrorAction As New functions
        Try



            'populate the datagrid with all the data
            Dim mySqlAction As String = "SELECT [Msg_dissemination_id] " &
                                          ",[group_name] " &
                                          ",[type_of_group] " &
                                          ",[type_of_message] " &
                                          ",[message] " &
                                          ",[dissemination_date] " &
                                          ",[total_sessions] " &
                                          ",[LIP] " &
                                          ",[County] " &
                                      "FROM [dbo].[FCI_message_dissemination] " &
                                    "where 1 = 1 AND LIP IN " &
                                            "(SELECT distinct LIPs.LIP " &
                                                "From LIPs INNER Join " &
                                                         "OVCRegistrationDetails On LIPs.cbo = OVCRegistrationDetails.cbo " &
                                                "Where OVCRegistrationDetails.cbo_id In (" & strcbos & ")) " &
                                     " AND [dissemination_date] between '" & Format(dtpsearchdatefrom.Value, "dd-MMM-yyyy") & "'" &
                                     " And '" & Format(dtpsearchdateto.Value, "dd-MMM-yyyy") & "'"

            If cbosearchcounty.Text.ToString.Length <> 0 Then
                mySqlAction = mySqlAction & " And FCI_message_dissemination.county = '" & cbosearchcounty.Text & "'"
            End If
            If cbosearchLIP.Text.ToString.Length <> 0 Then
                mySqlAction = mySqlAction & " AND FCI_message_dissemination.LIP = '" & cbosearchLIP.Text & "'"
            End If


            mySqlAction = mySqlAction & " order by group_name, type_of_group,type_of_message,message,dissemination_date asc"




            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim mymessage_disseminationid, mycounty, myLIP, mygroupname, mygrouptype, mymessagetype, mymessage, mydisseminationdate As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView1.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    mymessage_disseminationid = MyDatable.Rows(K).Item("Msg_dissemination_id").ToString
                    mycounty = MyDatable.Rows(K).Item("county").ToString
                    myLIP = MyDatable.Rows(K).Item("LIP").ToString
                    mygroupname = MyDatable.Rows(K).Item("group_name").ToString
                    mygrouptype = MyDatable.Rows(K).Item("type_of_group").ToString
                    mymessagetype = MyDatable.Rows(K).Item("type_of_message").ToString
                    mymessage = MyDatable.Rows(K).Item("message").ToString
                    mydisseminationdate = MyDatable.Rows(K).Item("dissemination_date").ToString

                    DataGridView1.Rows.Add(mymessage_disseminationid, mycounty, myLIP, mygroupname, mygrouptype, mymessagetype, mymessage, mydisseminationdate, "Select")
                Next
            Else 'if there are no rows returned
                MsgBox("Search query returned no results", MsgBoxStyle.Exclamation)
            End If

            'we set focus back to fname so that by clicking 'search' again, we loose focus on txtname thus 
            'recreating the search criteria again
            cbosearchcounty.Focus()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        Dim ErrorAction As New functions
        Try
            Dim K As Integer

            K = e.RowIndex '- 1

            'select item to edit or delete
            Dim mysqlaction As String = ""
            Dim MyDBAction As New functions

            Dim MyDatable As New Data.DataTable
            Dim strMsg_dissemination_id As String = Me.DataGridView1.Rows(K).Cells(0).Value

            mysqlaction = "SELECT [Msg_dissemination_id] " &
                                          ",[group_name] " &
                                          ",[type_of_group] " &
                                          ",[type_of_message] " &
                                          ",[message] " &
                                          ",[dissemination_date] " &
                                          ",[total_sessions] " &
                                          ",[LIP] " &
                                          ",[County] " &
                                      "FROM [dbo].[FCI_message_dissemination] " &
                             " where Msg_dissemination_id = '" & strMsg_dissemination_id & "' "
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then
                If MyDatable.Rows.Count > 0 Then

                    clearcontrols() 'first clear previous data from controls

                    global_message_dissemination_id = MyDatable.Rows(0).Item("Msg_dissemination_id").ToString
                    cboCounty.SelectedText = MyDatable.Rows(0).Item("county").ToString
                    cboLIP.SelectedText = MyDatable.Rows(0).Item("LIP").ToString
                    txtgroupname.Text = MyDatable.Rows(0).Item("group_name").ToString
                    cboTypeofGroup.SelectedText = MyDatable.Rows(0).Item("type_of_group").ToString
                    cboMessageType.SelectedText = MyDatable.Rows(0).Item("type_of_message").ToString
                    cboMessage.SelectedText = MyDatable.Rows(0).Item("message").ToString
                    dtpDisseminationDate.Value = Format(MyDatable.Rows(0).Item("dissemination_date"), "dd-MMM-yyyy")

                    'populate grid with clients signed up for this group



                End If
            End If

            mysqlaction = "SELECT FCI_client_message_dissemination.ID, FCI_client_message_dissemination.Msg_dissemination_id, " &
                                "FCI_client_message_dissemination.client_id, FCI_clients.firstname, FCI_clients.middlename,  " &
                                "FCI_clients.surname, FCI_clients.gender, FCI_clients.age, FCI_clients.telephoneNo, FCI_clients.emailaddress " &
                                "FROM  FCI_client_message_dissemination INNER JOIN " &
                                "FCI_clients ON FCI_client_message_dissemination.client_id = FCI_clients.client_id " &
                                "where FCI_client_message_dissemination.Msg_dissemination_id = '" & global_message_dissemination_id & "' "

            Dim myclient_id, myfirstname, mymiddlename, mysurname, mygender, myemailaddress As String
            Dim myage As Integer
            Dim mytelephoneno As Int64
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            DataGridView2.Rows.Clear()
            DataGridView3.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    myclient_id = MyDatable.Rows(K).Item("client_id").ToString
                    myfirstname = MyDatable.Rows(K).Item("firstname").ToString
                    mymiddlename = MyDatable.Rows(K).Item("middlename").ToString
                    mysurname = MyDatable.Rows(K).Item("surname").ToString
                    mygender = MyDatable.Rows(K).Item("gender").ToString
                    myage = MyDatable.Rows(K).Item("age").ToString
                    mytelephoneno = MyDatable.Rows(K).Item("telephoneno").ToString
                    myemailaddress = MyDatable.Rows(K).Item("emailaddress").ToString

                    DataGridView2.Rows.Add(myclient_id, myfirstname, mymiddlename, mysurname, mygender, myage, mytelephoneno)
                    DataGridView3.Rows.Add(myclient_id, myfirstname, mymiddlename, mysurname, mygender, False)
                Next

            End If

            'fillgrid()
            'fill session history grid
            fillSessionhistorygrid(global_message_dissemination_id)

            'take focus back to the first tab
            TabControl1.SelectedIndex = 0

        Catch ex As Exception
            MsgBox(ex.Message, vbExclamation)


        End Try
    End Sub
    Private Sub clearcontrols()
        Try


            'to clear textBoxes,radiobutton and checkboxes
            ' that are in containers 
            Dim ctrl As Control = Me.GetNextControl(Me, True)
            Do Until ctrl Is Nothing
                If TypeOf ctrl Is TextBox Then
                    ctrl.Text = String.Empty
                ElseIf TypeOf ctrl Is RadioButton Then
                    DirectCast(ctrl, RadioButton).Checked = False
                ElseIf TypeOf ctrl Is CheckBox Then
                    DirectCast(ctrl, CheckBox).Checked = False
                ElseIf TypeOf ctrl Is ComboBox Then
                    DirectCast(ctrl, ComboBox).Text = ""
                ElseIf TypeOf ctrl Is DateTimePicker Then
                    DirectCast(ctrl, DateTimePicker).Value = Date.Today
                ElseIf TypeOf ctrl Is MaskedTextBox Then
                    ctrl.Text = String.Empty
                End If

                ctrl = Me.GetNextControl(ctrl, True)

            Loop

        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim ErrorAction As New functions
        Dim myrandomkey As New Random
        Try
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            Dim mymsg_dissemination_sessions_id As String = ""

            'Save session clients attendance
            Dim myclient_id As String
            If DataGridView3.Rows.Count > 0 Then
                For K = 0 To DataGridView3.Rows.Count - 1
                    If (DataGridView3.Rows(K).Cells(1).Value = Nothing) = False AndAlso DataGridView3.Rows(K).Cells(5).Value = True Then
                        mymsg_dissemination_sessions_id = "msgdsessid_" & Format(Date.Today, "yyyyMMddhhss") & myrandomkey.Next(9000)
                        myclient_id = DataGridView3.Rows(K).Cells(0).Value.ToString


                        mySqlAction = "INSERT INTO [dbo].[FCI_message_dissemination_sessions] " &
                                               "([FCI_message_dissemination_sessions_id] " &
                                               ",[session_date] " &
                                               ",[client_id] " &
                                               ",[message_dissemination_id]) " &
                                         "VALUES " &
                                               "('" & mymsg_dissemination_sessions_id & "' " &
                                               ",'" & Format(dtpsessiondate.Value, "dd-MMM-yyyy") & "' " &
                                               ",'" & myclient_id & "' " &
                                               ",'" & global_message_dissemination_id & "')"

                        MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
                    End If

                Next
            End If

            MsgBox("Session Saved Successfully.", vbInformation)
            'fill session history grid
            fillSessionhistorygrid(global_message_dissemination_id)
        Catch ex As Exception
            MsgBox(ex.Message, vbExclamation)
        End Try
    End Sub
    Private Sub fillSessionhistorygrid(ByVal mymessage_dissemination_id As String)
        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            Dim mySqlAction As String = "SELECT distinct [session_date] " &
                                              "FROM [dbo].[FCI_message_dissemination_sessions] " &
                                          "where message_dissemination_id = '" & mymessage_dissemination_id & "'"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim mysessiondate As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView4.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    mysessiondate = Format(MyDatable.Rows(K).Item("session_date"), "dd-MMM-yyyy")

                    DataGridView4.Rows.Add(mysessiondate, "Select")
                Next
            End If
        Catch ex As Exception
            MsgBox(ex.Message, vbExclamation)

        End Try
    End Sub

    Private Sub DataGridView4_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView4.CellContentClick
        Try
            Dim K As Integer

            K = e.RowIndex '- 1

            'select item to edit or delete
            Dim mysqlaction As String = ""
            Dim MyDBAction As New functions

            Dim MyDatable As New Data.DataTable
            Dim strsession_date As String = Me.DataGridView4.Rows(K).Cells(0).Value

            dtpsessiondate.Value = strsession_date

            'Uncheck all checkboxes for all clients before checking them with correct attendance
            For j = 0 To DataGridView3.Rows.Count - 1
                If (DataGridView3.Rows(j).Cells(1).Value = Nothing) = False Then 'ignore blank rows
                    DataGridView3.Rows(j).Cells(5).Value = False
                End If
            Next

            mysqlaction = "SELECT [client_id] " &
                        "From [dbo].[FCI_message_dissemination_sessions] " &
                             " where message_dissemination_id = '" & global_message_dissemination_id & "' " &
                             " and [session_date] = '" & strsession_date & "'"
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then
                If MyDatable.Rows.Count > 0 Then
                    For K = 0 To MyDatable.Rows.Count - 1
                        'Loop through clients for this dissemination check who attended that particular session
                        If DataGridView3.Rows.Count > 0 Then
                            For j = 0 To DataGridView3.Rows.Count - 1
                                If (DataGridView3.Rows(K).Cells(1).Value = Nothing) = False Then 'ignore blank rows
                                    If DataGridView3.Rows(j).Cells(0).Value.ToString = MyDatable.Rows(K).Item("client_id").ToString Then
                                        DataGridView3.Rows(j).Cells(5).Value = True
                                        Exit For
                                    End If
                                End If
                            Next
                        End If
                    Next

                End If
            End If

        Catch ex As Exception
            'MsgBox(ex.Message, vbExclamation)
        End Try
    End Sub
End Class