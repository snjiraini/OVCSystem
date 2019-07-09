Imports OVCSystem.functions
Imports System.Data.Odbc
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Text

Public Class frmTrainingAttendance
    Dim str_training_attendance As String = ""
    Private Sub BtnEdit_Click(sender As Object, e As EventArgs) Handles BtnEdit.Click
        Dim ErrorAction As New functions
        Try

            'update record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            mySqlAction = "UPDATE [dbo].[training_attendance] " &
                              " SET [caregiver_id] = '" & txtcpimsid.Text.ToString & "' " &
                                "  ,[training_id] = '" & cboTraining.SelectedValue.ToString & "' " &
                             "WHERE training_attendance_id = '" & str_training_attendance & "'"

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Update)
            MsgBox("Record updated successfully.", MsgBoxStyle.Information)

            fillgrid()

            str_training_attendance = ""

            BtnEdit.Enabled = False
            btnDelete.Enabled = False
            GroupBox1.Enabled = False
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("vsla_membership", "Edit", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub frmTrainingAttendance_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        populatecounties()
        populatetrainingList()
    End Sub



    Private Sub populatetrainingList()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "SELECT training_list.training_id, type_of_training.type_of_training + ' - [' + wards.county + ' - ' + wards.ward + '] - '  +  DateDimension.MonthYear as Training " &
                                        "FROM   training_list INNER JOIN " &
                                                     "type_of_training ON training_list.type_of_training = type_of_training.trainingtypeid INNER JOIN " &
                                                     "wards ON training_list.location = wards.ward_id INNER JOIN " &
                                                     "DateDimension ON training_list.start_date_of_training = DateDimension.Date " &
                                        "ORDER BY type_of_training.type_of_training, wards.county, wards.ward, training_list.start_date_of_training"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cboTraining
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "Training"
                .ValueMember = "training_id"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("TrainingAttendance", "populatetrainingList", ex.Message) ''---Write error to error log file

        End Try
    End Sub
    Private Sub populatecounties()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select distinct county from OVCRegistrationDetails  order by county asc"
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
            ErrorAction.WriteToErrorLogFile("TrainingAttendance", "populatecounties", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            Dim mySqlAction As String = "SELECT distinct caregiver_id,caregiver_names,county,cbo,ward,chv_names  from OVCRegistrationDetails where 1 = 1"

            If cbosearchcounty.Text.ToString.Length <> 0 Then
                mySqlAction = mySqlAction & " AND OVCRegistrationDetails.county = '" & cbosearchcounty.Text & "'"
            End If
            If cbosearchward.Text.ToString.Length <> 0 Then
                mySqlAction = mySqlAction & " AND OVCRegistrationDetails.ward = '" & cbosearchward.Text & "'"
            End If
            If cbosearchcbo.Text.ToString.Length <> 0 Then
                mySqlAction = mySqlAction & " AND OVCRegistrationDetails.cbo = '" & cbosearchcbo.Text.ToString & "'"
            End If
            If txtsearchchvname.Text.ToString.Length <> 0 Then
                mySqlAction = mySqlAction & " AND (OVCRegistrationDetails.chv_names like '%" & txtsearchchvname.Text.ToString & "%')"
            End If
            If txtsearchcaregivername.Text.ToString.Length <> 0 Then
                mySqlAction = mySqlAction & " AND (OVCRegistrationDetails.caregiver_names like '%" & txtsearchcaregivername.Text.ToString & "%')"
            End If

            mySqlAction = mySqlAction & " order by OVCRegistrationDetails.caregiver_names, OVCRegistrationDetails.chv_names asc"




            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim mycpimsid, Mycounty, mycbo, myward, mychvnames, mycaregivernames As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView1.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    mycpimsid = MyDatable.Rows(K).Item("caregiver_id").ToString
                    mycaregivernames = MyDatable.Rows(K).Item("caregiver_names").ToString
                    Mycounty = MyDatable.Rows(K).Item("county").ToString
                    mycbo = MyDatable.Rows(K).Item("cbo").ToString
                    myward = MyDatable.Rows(K).Item("ward").ToString
                    mychvnames = MyDatable.Rows(K).Item("chv_names").ToString


                    DataGridView1.Rows.Add(mycpimsid, mycaregivernames, mychvnames, mycbo, Mycounty, myward, "Select")
                Next
            Else 'if there are no rows returned
                MsgBox("Search query returned no results", MsgBoxStyle.Exclamation)
            End If

            'we set focus back to fname so that by clicking 'search' again, we loose focus on txtname thus 
            'recreating the search criteria again
            cbosearchcounty.Focus()

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("TrainingAttendance", "clientsearch", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub lnkNew_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkNew.LinkClicked

        cboTraining.SelectedIndex = -1
        btnpost.Enabled = True
        btnDelete.Enabled = False
        BtnEdit.Enabled = False
        GroupBox1.Enabled = True
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
            Dim strcaregiverid As String = Me.DataGridView1.Rows(K).Cells(0).Value

            mysqlaction = "SELECT distinct caregiver_id,caregiver_names,county,countyid,cbo,ward,chv_names  from OVCRegistrationDetails where caregiver_id ='" & strcaregiverid & "'"
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then
                If MyDatable.Rows.Count > 0 Then

                    clearcontrols() 'first clear previous data from controls

                    txtcpimsid.Text = MyDatable.Rows(0).Item("caregiver_id").ToString
                    txtcaregivernames.Text = MyDatable.Rows(0).Item("caregiver_names").ToString

                    'dtpDateofLinkage.Value = IIf(IsDate(MyDatable.Rows(0).Item("date_of_linkage").ToString) = False, "1900-01-01 00:00:00.000", MyDatable.Rows(0).Item("date_of_linkage").ToString)
                    'txtcccnumber.Text = MyDatable.Rows(0).Item("ccc_number").ToString

                End If
            End If


            fillgrid()

            'take focus back to the first tab
            TabControl1.SelectedIndex = 0

        Catch ex As Exception
            MsgBox(ex.Message, vbExclamation)
            ErrorAction.WriteToErrorLogFile("TrainingAttendance", "GridCellClick", ex.Message) ''---Write error to error log file

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
                    DirectCast(ctrl, ComboBox).SelectedIndex = -1
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

    Private Sub cbosearchcounty_SelectedValueChanged(sender As Object, e As EventArgs) Handles cbosearchcounty.SelectedValueChanged
        Try
            'If cboDistrict.Text.Trim.Length <> 0 Or cboDistrict.Text <> "System.Data.DataRowView" Then
            If IsNumeric(cbosearchcounty.SelectedIndex) = True Then

                populatesearchCBO(cbosearchcounty.Text.ToString)
                populatesearchWard(cbosearchcounty.Text.ToString)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub populatesearchCBO(ByVal mycounty As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select ROW_NUMBER() OVER(ORDER BY cbo ASC) AS cbo_id, cbo,county " &
                                        "from " &
                                        "(select distinct cbo,county from OVCRegistrationDetails) tbl_cbo  " &
                                        "where county = '" & mycounty.ToString & "' order by cbo asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cbosearchcbo
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "CBO"
                .ValueMember = "CBO_ID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("TrainingAttendance", "populatesearchcbo", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populatesearchWard(ByVal mycounty As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select distinct ward_id,ward from OVCRegistrationDetails where county = '" & mycounty.ToString &
            "' order by ward asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cbosearchward
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "ward"
                .ValueMember = "ward_ID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("TrainingAttendance", "populatesearchward", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub fillgrid()
        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            Dim mySqlAction As String = "SELECT DISTINCT training_attendance.training_attendance_id, training_attendance.caregiver_id, " &
                                            "training_attendance.training_id,  OVCRegistrationDetails.caregiver_names,  " &
                                            "training_list.facilitator, training_list.start_date_of_training, wards.ward, wards.county,  " &
                                           " type_of_training.type_of_training " &
                                        "FROM   training_attendance INNER JOIN " &
                                            "OVCRegistrationDetails On training_attendance.caregiver_id = OVCRegistrationDetails.caregiver_id INNER JOIN " &
                                           " training_list ON training_attendance.training_id = training_list.training_id INNER JOIN " &
                                           " wards On training_list.location = wards.ward_id  INNER JOIN " &
                                           "type_of_training On training_list.type_of_training = type_of_training.trainingtypeid  " &
                                        " where training_attendance.caregiver_id = '" & txtcpimsid.Text.ToString & "'"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim mytrainingmembershipid, myTraining, myfacilitator, mylocation, mycounty, myvsla As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView2.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    mytrainingmembershipid = MyDatable.Rows(K).Item("training_attendance_id").ToString
                    'mycaregivername = MyDatable.Rows(K).Item("caregiver_names").ToString
                    myTraining = MyDatable.Rows(K).Item("type_of_training").ToString
                    myfacilitator = MyDatable.Rows(K).Item("facilitator").ToString
                    mylocation = MyDatable.Rows(K).Item("ward").ToString
                    mycounty = MyDatable.Rows(K).Item("county").ToString

                    DataGridView2.Rows.Add(mytrainingmembershipid, myTraining, myfacilitator, mylocation, mycounty, "Select")
                Next
            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("training_attendance", "Fillgrid", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub btnpost_Click(sender As Object, e As EventArgs) Handles btnpost.Click
        Dim ErrorAction As New functions
        Try
            If validatecontrols() = False Then
                Exit Sub
            End If

            'save record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            mySqlAction = "INSERT INTO [dbo].[training_attendance] " &
                                   " ([caregiver_id]" &
                                   ",[training_id])" &
                             "VALUES" &
                                  "('" & txtcpimsid.Text.ToString & "'" &
                                  " ,'" & cboTraining.SelectedValue.ToString & "')"

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
            MsgBox("Record saved successfully.", MsgBoxStyle.Information)

            fillgrid()

            btnpost.Enabled = False
            GroupBox1.Enabled = False
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("training_attendance", "Save", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Function validatecontrols() As Boolean
        Try

            ErrorProvider1.Clear()

            If cboTraining.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(cboTraining, "Please select a Training to continue")
                MsgBox("Please select a Training to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                cboTraining.Focus()
                Return False
                'ElseIf cboVSLA.Text.Trim.Length = 0 Then
                '    ErrorProvider1.SetError(cboVSLA, "Please select a Training to continue")
                '    MsgBox("Please select a VSLA to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                '    cboVSLA.Focus()

            End If

            'IF all controls have been validated, return true
            Return True

        Catch ex As Exception
            Return False 'incase this function throws exception, return false
        End Try
    End Function

    Private Sub DataGridView2_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellContentClick
        Dim ErrorAction As New functions
        Try

            'select item to edit or delete
            Dim mysqlaction As String = ""
            Dim MyDBAction As New functions
            Dim K As Integer

            K = e.RowIndex '- 1


            Dim MyDatable As New Data.DataTable
            mysqlaction = "Select DISTINCT training_attendance.training_attendance_id, training_attendance.caregiver_id, " &
                                            "training_attendance.training_id,  OVCRegistrationDetails.caregiver_names,  " &
                                            "training_list.facilitator, training_list.start_date_of_training, wards.ward, wards.county,  " &
                                           "  type_of_training.type_of_training " &
                                        "FROM   training_attendance INNER JOIN " &
                                            "OVCRegistrationDetails On training_attendance.caregiver_id = OVCRegistrationDetails.caregiver_id INNER JOIN " &
                                           " training_list On training_attendance.training_id = training_list.training_id INNER JOIN " &
                                           " wards On training_list.location = wards.ward_id  INNER JOIN " &
                                           "type_of_training On training_list.type_of_training = type_of_training.trainingtypeid " &
                                            "where training_attendance.training_attendance_id = " & Me.DataGridView2.Rows(K).Cells(0).Value & ""

            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then
                txtcpimsid.Text = MyDatable.Rows(0).Item("caregiver_id").ToString
                txtcaregivernames.Text = MyDatable.Rows(0).Item("caregiver_names").ToString
                cboTraining.SelectedValue = MyDatable.Rows(0).Item("training_id").ToString

                'initialize the record identifier
                str_training_attendance = MyDatable.Rows(0).Item("training_attendance_id").ToString

            End If



            'Panel1.Enabled = True
            BtnEdit.Enabled = True
            btnDelete.Enabled = True
            GroupBox1.Enabled = True
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("training_attendance", "GridCellClick", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub cbosearchcounty_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbosearchcounty.SelectedIndexChanged

    End Sub

    Private Sub cboTraining_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTraining.SelectedIndexChanged

    End Sub

    Private Sub cboTraining_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cboTraining.KeyPress
        e.Handled = True
    End Sub

    Private Sub cbosearchcounty_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cbosearchcounty.KeyPress
        e.Handled = True
    End Sub



    Private Sub cbosearchward_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbosearchward.SelectedIndexChanged

    End Sub

    Private Sub cbosearchward_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cbosearchward.KeyPress
        e.Handled = True
    End Sub

    Private Sub cbosearchcbo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbosearchcbo.SelectedIndexChanged

    End Sub

    Private Sub cbosearchcbo_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cbosearchcbo.KeyPress
        e.Handled = True
    End Sub
End Class