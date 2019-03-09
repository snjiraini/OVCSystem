Imports OVCSystem.functions
Imports System.Data.Odbc
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Text

Public Class frmOVCInfo
    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            Dim mySqlAction As String = "SELECT * from OVCRegistrationDetails where 1 = 1"

            If IsNumeric(cbosearchcounty.SelectedValue) Then
                mySqlAction = mySqlAction & " AND OVCRegistrationDetails.county = '" & cbosearchcounty.Text & "'"
            End If
            If IsNumeric(cbosearchcbo.SelectedValue) Then
                mySqlAction = mySqlAction & " AND OVCRegistrationDetails.cbo_id = '" & cbosearchcbo.SelectedValue & "'"
            End If
            If txtsearchchvname.Text.ToString.Length <> 0 Then
                mySqlAction = mySqlAction & " AND (OVCRegistrationDetails.chv_names like '%" & txtsearchchvname.Text.ToString & "%')"
            End If
            If txtsearchovcname.Text.ToString.Length <> 0 Then
                mySqlAction = mySqlAction & " AND (OVCRegistrationDetails.ovc_names like '%" & txtsearchovcname.Text.ToString & "%')"
            End If

            mySqlAction = mySqlAction & " order by OVCRegistrationDetails.ovc_names, OVCRegistrationDetails.chv_names asc"




            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim mycpimsid, myovcnames, Mycounty, mycbo, myward, mychvnames, mycaregivernames As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView1.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    mycpimsid = MyDatable.Rows(K).Item("cpims_ovc_id").ToString
                    myovcnames = MyDatable.Rows(K).Item("ovc_names").ToString
                    Mycounty = MyDatable.Rows(K).Item("county").ToString
                    mycbo = MyDatable.Rows(K).Item("cbo").ToString
                    myward = MyDatable.Rows(K).Item("ward").ToString
                    mychvnames = MyDatable.Rows(K).Item("chv_names").ToString
                    mycaregivernames = MyDatable.Rows(K).Item("caregiver_names").ToString

                    DataGridView1.Rows.Add(mycpimsid, myovcnames, mycbo, Mycounty, myward, mychvnames,
                                           mycaregivernames, "Select")
                Next
            Else 'if there are no rows returned
                MsgBox("Search query returned no results", MsgBoxStyle.Exclamation)
            End If

            'we set focus back to fname so that by clicking 'search' again, we loose focus on txtname thus 
            'recreating the search criteria again
            cbosearchcounty.Focus()

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "clientsearch", ex.Message) ''---Write error to error log file

        End Try

    End Sub



    Private Sub frmOVCInfo_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        populatecounties()
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
            ErrorAction.WriteToErrorLogFile("clientinfo", "populatecounties", ex.Message) ''---Write error to error log file

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
            Dim mySqlAction As String = "select distinct cbo_id,cbo from OVCRegistrationDetails where county = '" & mycounty.ToString & "' order by cbo asc"
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
            ErrorAction.WriteToErrorLogFile("clientinfo", "populatesearchcbo", ex.Message) ''---Write error to error log file

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
            ErrorAction.WriteToErrorLogFile("clientinfo", "populatesearchward", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        Dim ErrorAction As New functions
        Try
            Dim K As Integer

            K = e.RowIndex

            'select item to edit or delete
            Dim mysqlaction As String = ""
            Dim MyDBAction As New functions

            Dim MyDatable As New Data.DataTable
            Dim strovcid As String = Me.DataGridView1.Rows(K).Cells(0).Value

            mysqlaction = "select * from OVCRegistrationDetails where cpims_ovc_id ='" & strovcid & "'"
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then
                If MyDatable.Rows.Count > 0 Then

                    clearcontrols() 'first clear previous data from controls

                    txtOVCid.Text = MyDatable.Rows(0).Item("cpims_ovc_id").ToString
                    txtovcnames.Text = MyDatable.Rows(0).Item("ovc_names").ToString
                    txtgender.Text = MyDatable.Rows(0).Item("gender").ToString
                    dtpDateofBirth.Value = MyDatable.Rows(0).Item("dob").ToString
                    txtAge.Text = MyDatable.Rows(0).Item("age").ToString
                    chkHasCert.Checked = IIf(MyDatable.Rows(0).Item("birthcert").ToString = "NO BIRTHCERT", False, True)
                    txtbcertnumber.Text = MyDatable.Rows(0).Item("bcertnumber").ToString

                    chkOVCDisabled.Checked = IIf(MyDatable.Rows(0).Item("ovcdisability") = "NO DISABILITY", False, True)
                    txtNCPWDnum.Text = MyDatable.Rows(0).Item("ncpwdnumber").ToString


                    txtcounty.Text = MyDatable.Rows(0).Item("county").ToString
                    txtcbo.Text = MyDatable.Rows(0).Item("cbo").ToString
                    txtward.Text = MyDatable.Rows(0).Item("ward").ToString
                    txtchvnames.Text = MyDatable.Rows(0).Item("chv_names").ToString
                    txtcaregivernames.Text = MyDatable.Rows(0).Item("caregiver_names").ToString
                    txtcaregiverhivstatus.Text = MyDatable.Rows(0).Item("caregiverhivstatus").ToString
                    txtcaregiverID.Text = MyDatable.Rows(0).Item("caregiver_id").ToString

                    txthivstatus.Text = MyDatable.Rows(0).Item("ovchivstatus").ToString
                    txtartstatus.Text = MyDatable.Rows(0).Item("artstatus").ToString
                    txtfacility.Text = MyDatable.Rows(0).Item("facility").ToString
                    dtpDateofLinkage.Value = IIf(IsDate(MyDatable.Rows(0).Item("date_of_linkage").ToString) = False, "1900-01-01 00:00:00.000", MyDatable.Rows(0).Item("date_of_linkage").ToString)
                    txtcccnumber.Text = MyDatable.Rows(0).Item("ccc_number").ToString
                    txtimmunization.Text = MyDatable.Rows(0).Item("immunization").ToString
                    txtschoollevel.Text = MyDatable.Rows(0).Item("schoollevel").ToString
                    txtschoolname.Text = MyDatable.Rows(0).Item("school_name").ToString
                    txtclass.Text = MyDatable.Rows(0).Item("class").ToString
                    chkExit.Checked = IIf(MyDatable.Rows(0).Item("exit_status").ToString = "ACTIVE", False, True)
                    dtpDateofExit.Value = IIf(IsDate(MyDatable.Rows(0).Item("exit_date").ToString) = False, "1900-01-01 00:00:00.000", MyDatable.Rows(0).Item("exit_date").ToString)
                    txtreasonforexit.Text = MyDatable.Rows(0).Item("exit_reason").ToString
                    dtpDateofRegistration.Value = IIf(IsDate(MyDatable.Rows(0).Item("registration_date").ToString) = False, "1900-01-01 00:00:00.000", MyDatable.Rows(0).Item("registration_date").ToString)

                End If
            End If

            'take focus back to the first tab
            TabControl1.SelectedIndex = 0

        Catch ex As Exception
            MsgBox(ex.Message, vbExclamation)
            ErrorAction.WriteToErrorLogFile("clientinfo", "GridCellClick", ex.Message) ''---Write error to error log file

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

    Private Sub chkExit_CheckedChanged(sender As Object, e As EventArgs) Handles chkExit.CheckedChanged

    End Sub
End Class