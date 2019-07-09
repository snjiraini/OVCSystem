Imports OVCSystem.functions
Imports System.Data.Odbc

Public Class frmTrainingListing
    Private Sub frmTrainingListing_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        populatetypeoftraining()
        populatecounties()
        fillgrid()

    End Sub

    Private Sub populatecounties()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select distinct county_id,county from wards  order by county asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cbocounty
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "County"
                .ValueMember = "county_id"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("TrainingListing", "populatecounties", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub fillgrid()
        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            Dim mySqlAction As String = "SELECT distinct training_list.training_id, type_of_training.type_of_training, " &
                                            " training_list.facilitator, training_list.start_date_of_training, wards.ward As location " &
                                            " FROM   training_list INNER JOIN " &
                                            " type_of_training On training_list.type_of_training = type_of_training.trainingtypeid INNER JOIN " &
                                            " wards ON training_list.location = wards.ward_id"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim mytrainingid, mytrainingtype, myfacilitator, myward, mydateoftraining As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView1.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    mytrainingid = MyDatable.Rows(K).Item("training_id").ToString
                    mytrainingtype = MyDatable.Rows(K).Item("type_of_training").ToString
                    myfacilitator = MyDatable.Rows(K).Item("facilitator").ToString
                    myward = MyDatable.Rows(K).Item("location").ToString
                    mydateoftraining = MyDatable.Rows(K).Item("start_date_of_training").ToString
                    DataGridView1.Rows.Add(mytrainingid, mytrainingtype, myfacilitator, myward, mydateoftraining, "Select")
                Next
            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("TrainingListing", "Fillgrid", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub lnkNew_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkNew.LinkClicked
        Panel1.Enabled = True
        btnpost.Enabled = True
        BtnDelete.Enabled = False
        BtnEdit.Enabled = False
        txttrainingID.Focus()
        txttrainingID.Text = ""
        txtfacilitator.Text = ""
        cboTypeOfTraining.SelectedIndex = -1
        cbowards.SelectedIndex = -1
        dtpstartdate.Value = Date.Today
        clearcontrols()
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

    Private Sub populatetypeoftraining()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from type_of_training order by  type_of_training"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboTypeOfTraining
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "type_of_training"
                .ValueMember = "trainingtypeid"
                .SelectedIndex = -1
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("TrainingListing", "populatetypeoftraining", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populatewards(ByVal mycountyid As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select distinct ward_id,ward + ' - ' + county as ward from wards where county_id = '" & mycountyid & "'   order by ward + ' - ' + county"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cbowards
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "ward"
                .ValueMember = "ward_id"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            MsgBox(ex.Message, vbExclamation)
            ErrorAction.WriteToErrorLogFile("Training Listing", "populatewards", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub btnpost_Click(sender As Object, e As EventArgs) Handles btnpost.Click
        Dim ErrorAction As New functions
        Try

            'save record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            mySqlAction = "INSERT INTO [dbo].[training_list] " &
           "([type_of_training] " &
           ",[facilitator] " &
           ",[start_date_of_training] " &
           ",[enddate_of_training] " &
           ",[county_id] " &
           ",[location])" &
            " values('" & cboTypeOfTraining.SelectedValue.ToString & "','" &
            txtfacilitator.Text.ToString & "','" &
            Format(dtpstartdate.Value, "dd-MMM-yyyy") & "','" &
            Format(dtpenddate.Value, "dd-MMM-yyyy") & "','" &
            cbocounty.SelectedValue.ToString & "','" &
            cbowards.SelectedValue.ToString & "')"

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
            MsgBox("Record saved successfully.", MsgBoxStyle.Information)

            fillgrid()

            Panel1.Enabled = False
            btnpost.Enabled = False
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("training_list", "Save", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        Dim ErrorAction As New functions
        Try

            'select item to edit or delete
            Dim mysqlaction As String = ""
            Dim MyDBAction As New functions
            Dim K As Integer

            K = e.RowIndex


            Dim MyDatable As New Data.DataTable
            mysqlaction = "select * from [training_list] where [training_id]=" & Me.DataGridView1.Rows(K).Cells(0).Value & ""
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then

                txttrainingID.Text = MyDatable.Rows(0).Item("training_id").ToString
                cboTypeOfTraining.SelectedValue = MyDatable.Rows(0).Item("type_of_training").ToString
                txtfacilitator.Text = MyDatable.Rows(0).Item("facilitator").ToString
                cbocounty.SelectedValue = MyDatable.Rows(0).Item("county_id").ToString
                cbowards.SelectedValue = MyDatable.Rows(0).Item("location").ToString
                dtpstartdate.Value = MyDatable.Rows(0).Item("start_date_of_training").ToString
                dtpenddate.Value = MyDatable.Rows(0).Item("enddate_of_training").ToString
            End If

            Panel1.Enabled = True
            BtnEdit.Enabled = True
            BtnDelete.Enabled = True
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("TrainingListing", "GridCellClick", ex.Message) ''---Write error to error log file

        End Try

    End Sub

    Private Sub BtnEdit_Click(sender As Object, e As EventArgs) Handles BtnEdit.Click
        Dim ErrorAction As New functions
        Try

            'update record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            mySqlAction = "UPDATE [dbo].[training_list] " &
                          " SET [type_of_training] = '" & cboTypeOfTraining.SelectedValue.ToString & "' " &
                              ",[facilitator] = '" & txtfacilitator.Text.ToString & "' " &
                             " ,[start_date_of_training] = '" & Format(dtpstartdate.Value, "dd-MMM-yyyy") & "' " &
                             " ,[enddate_of_training] = '" & Format(dtpenddate.Value, "dd-MMM-yyyy") & "' " &
                             " ,[county_id] = '" & cbocounty.SelectedValue.ToString & "' " &
                              ",[location] = '" & cbowards.SelectedValue.ToString & "' " &
                         "WHERE [training_id] = '" & txttrainingID.Text.ToString & "'"

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Update)
            MsgBox("Record updated successfully.", MsgBoxStyle.Information)

            fillgrid()

            Panel1.Enabled = False
            BtnEdit.Enabled = False
            BtnDelete.Enabled = False
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("TrainingListing", "Edit", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles BtnExit.Click
        Me.Close()
    End Sub

    Private Sub cbocounty_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbocounty.SelectedIndexChanged

    End Sub

    Private Sub cbocounty_SelectedValueChanged(sender As Object, e As EventArgs) Handles cbocounty.SelectedValueChanged
        Try
            'If cboDistrict.Text.Trim.Length <> 0 Or cboDistrict.Text <> "System.Data.DataRowView" Then
            If IsNumeric(cbocounty.SelectedIndex) = True AndAlso (cbocounty.SelectedIndex + 1) > 0 Then

                populatewards(cbocounty.SelectedValue)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub cboTypeOfTraining_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTypeOfTraining.SelectedIndexChanged

    End Sub

    Private Sub cboTypeOfTraining_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cboTypeOfTraining.KeyPress
        e.Handled = True
    End Sub

    Private Sub cbocounty_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cbocounty.KeyPress
        e.Handled = True
    End Sub

    Private Sub cbowards_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbowards.SelectedIndexChanged

    End Sub

    Private Sub cbowards_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cbowards.KeyPress
        e.Handled = True
    End Sub
End Class