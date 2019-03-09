Imports OVCSystem.functions
Imports System.Data.Odbc

Public Class frmVSLAListing
    Private Sub frmVSLAListing_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        populatecounties()
        fillgrid()
    End Sub

    Private Sub populatecounties()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select distinct county_id,county from wards order by county"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cbocounty
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "county"
                .ValueMember = "county_id"
                .SelectedIndex = -1
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("VSLA Listing", "populatecounties", ex.Message) ''---Write error to error log file

        End Try
    End Sub


    Private Sub populatewards(ByVal strcounty_id As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select distinct ward_id,ward from wards  " &
                " where county_id = '" & strcounty_id.ToString & "' order by ward"
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
            ErrorAction.WriteToErrorLogFile("VSLA Listing", "populatecounties", ex.Message) ''---Write error to error log file

        End Try
    End Sub


    Private Sub fillgrid()
        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            Dim mySqlAction As String = "select * from vsla_list order by vsla_name,county,ward"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim myvslaid, myvsla, myward, mycounty, mydateofregistration As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView1.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    myvslaid = MyDatable.Rows(K).Item("vslaid").ToString
                    myvsla = MyDatable.Rows(K).Item("vsla_name").ToString
                    mycounty = MyDatable.Rows(K).Item("county").ToString
                    myward = MyDatable.Rows(K).Item("ward").ToString
                    mydateofregistration = MyDatable.Rows(K).Item("registration_date").ToString
                    DataGridView1.Rows.Add(myvslaid, myvsla, mycounty, myward, mydateofregistration, "Select")
                Next
            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("VSLAListing", "Fillgrid", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub btnpost_Click(sender As Object, e As EventArgs) Handles btnpost.Click
        Dim ErrorAction As New functions
        Try

            'save record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            mySqlAction =
            "INSERT INTO [dbo].[vsla_list] " &
           "([vsla_name] " &
           ",[registration_date] " &
           ",[ward_id] " &
           ",[ward] " &
           ",[county_id] " &
           ",[county]) " &
            "VALUES " &
           "( '" & txtName.Text.ToString & "'," &
          "'" & dtpDateRegistered.Value & "'," &
          " " & cbowards.SelectedValue.ToString & "," &
          " '" & cbowards.Text.ToString & "'," &
          " '" & cbocounty.SelectedValue.ToString & "'," &
           "'" & cbocounty.Text.ToString & "')   "



            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
            MsgBox("Record saved successfully.", MsgBoxStyle.Information)

            fillgrid()

            Panel1.Enabled = False
            btnpost.Enabled = False
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("vslaListing", "Save", ex.Message) ''---Write error to error log file

        End Try
    End Sub


    Private Sub cbocounty_SelectedValueChanged(sender As Object, e As EventArgs) Handles cbocounty.SelectedValueChanged
        Try

            If IsNumeric(cbocounty.SelectedValue) = True Then
                populatewards(cbocounty.SelectedValue.ToString)
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub cbocounty_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbocounty.SelectedIndexChanged

    End Sub

    Private Sub BtnEdit_Click(sender As Object, e As EventArgs)

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
            mysqlaction = "select * from vsla_list where vslaid=" & Me.DataGridView1.Rows(K).Cells(0).Value & ""
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then

                txtVSLAID.Text = MyDatable.Rows(0).Item("vslaid").ToString
                txtName.Text = MyDatable.Rows(0).Item("vsla_name").ToString
                cbocounty.SelectedValue = MyDatable.Rows(0).Item("county_ID").ToString
                cbowards.SelectedValue = MyDatable.Rows(0).Item("ward_ID").ToString
                dtpDateRegistered.Value = MyDatable.Rows(0).Item("registration_date").ToString

            End If

            Panel1.Enabled = True
            BtnEdit.Enabled = True
            BtnDelete.Enabled = True
        Catch ex As Exception
            MsgBox(ex.Message, vbExclamation)
            ErrorAction.WriteToErrorLogFile("VSLA Listing", "GridCellClick", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub BtnEdit_Click_1(sender As Object, e As EventArgs) Handles BtnEdit.Click

    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs) Handles BtnDelete.Click

    End Sub

    Private Sub lnkNew_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkNew.LinkClicked
        Panel1.Enabled = True
        btnpost.Enabled = True
    End Sub

    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles BtnExit.Click
        Me.Close()

    End Sub
End Class