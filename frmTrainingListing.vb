Imports OVCSystem.functions
Imports System.Data.Odbc

Public Class frmTrainingListing
    Private Sub frmTrainingListing_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        populatetypeoftraining()
        populatewards()
        fillgrid()

    End Sub

    Private Sub fillgrid()
        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            Dim mySqlAction As String = "SELECT distinct training_list.training_id, type_of_training.type_of_training, " &
                                            " training_list.facilitator, training_list.date_of_training, wards.ward As location " &
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
                    mydateoftraining = MyDatable.Rows(K).Item("date_of_training").ToString
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
        dtpDateofTraining.Value = Date.Today

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

    Private Sub populatewards()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select distinct ward_id,ward + ' - ' + county as ward from wards   order by ward + ' - ' + county"
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
           ",[date_of_training] " &
           ",[location])" &
            " values('" & cboTypeOfTraining.SelectedValue.ToString & "','" &
            txtfacilitator.Text.ToString & "','" &
            dtpDateofTraining.Value.ToString & "','" &
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
                cbowards.SelectedValue = MyDatable.Rows(0).Item("location").ToString
                dtpDateofTraining.Value = MyDatable.Rows(0).Item("date_of_training").ToString
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
                             " ,[date_of_training] = '" & dtpDateofTraining.Value.ToString & "' " &
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
End Class