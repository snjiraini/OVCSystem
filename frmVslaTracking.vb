Imports OVCSystem.functions
Imports System.Data.Odbc
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Text
Public Class frmVslaTracking
    Private Sub frmVslaTracking_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        populatecounties()
        populatemonths()
    End Sub

    Private Sub populatecounties()
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
            ErrorAction.WriteToErrorLogFile("VSLA tracking", "populatecounties", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populatemonths()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select distinct [MMYYYY],[MonthYear],[month],[year] from [DateDimension] " &
                " where datekey > 20150101  and [year] <= year(getdate())  order by [year],[month] asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cboMonth
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "MonthYear"
                .ValueMember = "MMYYYY"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With

        Catch ex As Exception
            ' ErrorAction.WriteToErrorLogFile("VSLA tracking", "populatecounties", ex.Message) ''---Write error to error log file

        End Try
    End Sub
    Private Sub cbosearchcounty_SelectedValueChanged(sender As Object, e As EventArgs) Handles cbosearchcounty.SelectedValueChanged
        Try
            'If cboDistrict.Text.Trim.Length <> 0 Or cboDistrict.Text <> "System.Data.DataRowView" Then
            If IsNumeric(cbosearchcounty.SelectedIndex) = True Then
                populatesearchWard(cbosearchcounty.Text.ToString)
            End If

        Catch ex As Exception

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
            ErrorAction.WriteToErrorLogFile("VSLA tracking", "populatesearchward", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Try
            'populate the datagrid with all the data
            Dim mySqlAction As String = "SELECT [vslaid],[vsla_name] ,[ward],[county],[chairperson] FROM [dbo].[vsla_list] where 1 = 1 "

            If cbosearchcounty.Text.ToString.Length <> 0 Then
                mySqlAction = mySqlAction & " AND vsla_list.county = '" & cbosearchcounty.Text & "'"
            End If
            If cbosearchward.Text.ToString.Length <> 0 Then
                mySqlAction = mySqlAction & " AND vsla_list.ward = '" & cbosearchward.Text & "'"
            End If


            mySqlAction = mySqlAction & " order by [vsla_name] ,[county],[ward],[chairperson] asc"

            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim myvslaid, Mycounty, myvsla_name, myward, mychairperson As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView1.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    myvslaid = MyDatable.Rows(K).Item("vslaid").ToString
                    myvsla_name = MyDatable.Rows(K).Item("vsla_name").ToString
                    Mycounty = MyDatable.Rows(K).Item("county").ToString
                    myward = MyDatable.Rows(K).Item("ward").ToString
                    mychairperson = MyDatable.Rows(K).Item("chairperson").ToString


                    DataGridView1.Rows.Add(myvslaid, myvsla_name, mychairperson, Mycounty, myward, "Select")
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
        Try
            Dim K As Integer

            K = e.RowIndex '- 1

            'select item to edit or delete
            Dim mysqlaction As String = ""
            Dim MyDBAction As New functions

            Dim MyDatable As New Data.DataTable
            Dim strvslaid As String = Me.DataGridView1.Rows(K).Cells(0).Value

            mysqlaction = "SELECT [vslaid],[vsla_name] ,[ward],[ward_id],[county],[chairperson] FROM [dbo].[vsla_list] where vslaid ='" & strvslaid & "'"
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then
                If MyDatable.Rows.Count > 0 Then

                    clearcontrols() 'first clear previous data from controls

                    txtvslaid.Text = MyDatable.Rows(0).Item("vslaid").ToString
                    txtVSLAname.Text = MyDatable.Rows(0).Item("vsla_name").ToString
                    txtwards.Text = MyDatable.Rows(0).Item("ward").ToString
                    'dtpDateofLinkage.Value = IIf(IsDate(MyDatable.Rows(e.RowIndex).Item("date_of_linkage").ToString) = False, "1900-01-01 00:00:00.000", MyDatable.Rows(e.RowIndex).Item("date_of_linkage").ToString)
                    'txtcccnumber.Text = MyDatable.Rows(e.RowIndex).Item("ccc_number").ToString

                End If
            End If


            'fillgrid()

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

    Private Sub lnkNew_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkNew.LinkClicked

        btnpost.Enabled = True
        btnDelete.Enabled = False
        BtnEdit.Enabled = False
        GroupBox1.Enabled = True
    End Sub

    Private Sub btnpost_Click(sender As Object, e As EventArgs) Handles btnpost.Click
        Try
            'If validatecontrols() = False Then
            '    Exit Sub
            'End If

            'save record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            mySqlAction = "INSERT INTO [dbo].[vsla_tracking] " &
                               "([vslaid] " &
                               ",[totalsavings] " &
                               ",[numberofloans] " &
                               ",[totalloanedout] " &
                               ",[totalCashBalanceForTheMonth] " &
                               ",[Month] ) " &
                         "VALUES " &
                               "('" & txtvslaid.Text & "', " &
                               "" & CDbl(txttotalsavings.Text) & ", " &
                              "" & CInt(txtnumberofloans.Text) & ", " &
                               "" & CDbl(txtTotalloanedout.Text) & ", " &
                               "" & CDbl(txtcashbalanceforthemonth.Text) & ", " &
                               "" & cboMonth.SelectedValue & ")"

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
            MsgBox("Record saved successfully.", MsgBoxStyle.Information)

            'fillgrid()

            btnpost.Enabled = False
            GroupBox1.Enabled = False

        Catch ex As Exception
            MsgBox(ex.Message, vbExclamation)
        End Try
    End Sub

    Private Function validatecontrols() As Boolean
        Try

            ErrorProvider1.Clear()

            'If cboTraining.Text.Trim.Length = 0 Then
            '    ErrorProvider1.SetError(cboTraining, "Please select a Training to continue")
            '    MsgBox("Please select a Training to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
            '    cboTraining.Focus()
            '    Return False
            '    'ElseIf cboVSLA.Text.Trim.Length = 0 Then
            '    '    ErrorProvider1.SetError(cboVSLA, "Please select a Training to continue")
            '    '    MsgBox("Please select a VSLA to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
            '    '    cboVSLA.Focus()

            'End If

            'IF all controls have been validated, return true
            Return True

        Catch ex As Exception
            Return False 'incase this function throws exception, return false
        End Try
    End Function

    Private Sub cbosearchcounty_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbosearchcounty.SelectedIndexChanged

    End Sub
End Class