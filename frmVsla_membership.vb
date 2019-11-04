Imports OVCSystem.functions
Imports System.Data.Odbc
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Text


Public Class frmVsla_membership
    Dim str_vsla_membership As String = ""

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
                    mycpimsid = MyDatable.Rows(0).Item("caregiver_id").ToString
                    mycaregivernames = MyDatable.Rows(0).Item("caregiver_names").ToString
                    Mycounty = MyDatable.Rows(0).Item("county").ToString
                    mycbo = MyDatable.Rows(0).Item("cbo").ToString
                    myward = MyDatable.Rows(0).Item("ward").ToString
                    mychvnames = MyDatable.Rows(0).Item("chv_names").ToString


                    DataGridView1.Rows.Add(mycpimsid, mycaregivernames, mychvnames, mycbo, Mycounty, myward, "Select")
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

            populatevslaList(MyDatable.Rows(0).Item("countyid").ToString)
            fillgrid()

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

    Private Sub populatevslaList(ByVal mycountyid As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select distinct vslaid,vsla_name + ' - ' + county as vsla_name from vsla_list " &
                " where county_id = '" & mycountyid & "' order by vslaid,vsla_name + ' - ' + county  asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cboVSLA
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "vsla_name"
                .ValueMember = "vslaid"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("vsla_Membership", "populatevslaList", ex.Message) ''---Write error to error log file

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
            mySqlAction = "Insert Into vsla_membership(caregiver_id,vsla_id,date_joined,member_active_vsla) " &
            " values('" & txtcpimsid.Text.ToString & "','" & cboVSLA.SelectedValue.ToString &
            "','" & Format(dtpDateJoined.Value, "dd-MMM-yyyy") & "','" & chkMemberActive.Checked & "')"

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
            MsgBox("Record saved successfully.", MsgBoxStyle.Information)

            fillgrid()

            btnpost.Enabled = False
            GroupBox1.Enabled = False
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("vslamembership", "Save", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub fillgrid()
        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            Dim mySqlAction As String = "SELECT distinct  vsla_membership.vsla_membership_id, vsla_membership.caregiver_id, " &
                                        "vsla_membership.vsla_id, vsla_membership.date_joined, vsla_membership.member_active_vsla,  " &
                                        " vsla_list.vsla_name, OVCRegistrationDetails.caregiver_names " &
                                        "FROM   vsla_membership INNER JOIN " &
                                        "OVCRegistrationDetails ON vsla_membership.caregiver_id = OVCRegistrationDetails.caregiver_id INNER JOIN " &
                                        "vsla_list ON vsla_membership.vsla_id = vsla_list.vslaid where vsla_membership.caregiver_id = '" & txtcpimsid.Text.ToString & "'"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim myvslamembershipid, mycaregivername, myvsla, mydatejoined, mymemberactive As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView2.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    myvslamembershipid = MyDatable.Rows(0).Item("vsla_membership_id").ToString
                    'mycaregivername = MyDatable.Rows(0).Item("caregiver_names").ToString
                    myvsla = MyDatable.Rows(0).Item("vsla_name").ToString
                    mydatejoined = MyDatable.Rows(0).Item("date_joined").ToString
                    mymemberactive = CBool(MyDatable.Rows(0).Item("member_active_vsla").ToString)
                    DataGridView2.Rows.Add(myvslamembershipid, myvsla, mydatejoined, mymemberactive, "Select")
                Next
            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("vsla_membership", "Fillgrid", ex.Message) ''---Write error to error log file

        End Try
    End Sub
    Private Sub lnkNew_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkNew.LinkClicked
        cboVSLA.SelectedIndex = -1
        dtpDateJoined.Value = Date.Today
        btnpost.Enabled = True
        btnDelete.Enabled = False
        BtnEdit.Enabled = False
        GroupBox1.Enabled = True
    End Sub



    Private Function validatecontrols() As Boolean
        Try

            ErrorProvider1.Clear()

            If cboVSLA.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(cboVSLA, "Please select a VSLA to continue")
                MsgBox("Please select a VSLA to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                cboVSLA.Focus()
                Return False
            ElseIf dtpDateJoined.Value >= Date.Today Then
                ErrorProvider1.SetError(dtpDateJoined, "Please select correct date to continue")
                MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                dtpDateJoined.Focus()
                Return False

            End If

            'IF all controls have been validated, return true
            Return True

        Catch ex As Exception
            Return False 'incase this function throws exception, return false
        End Try
    End Function



    Private Sub BtnEdit_Click(sender As Object, e As EventArgs) Handles BtnEdit.Click
        Dim ErrorAction As New functions
        Try

            'update record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            mySqlAction = "update vsla_membership set vsla_id = " &
            " '" & cboVSLA.SelectedValue.ToString & "', date_joined = '" & Format(dtpDateJoined.Value, "dd-MMM-yyyy") & "'," &
            "member_active_vsla = '" & chkMemberActive.Checked & "'" &
            " where vsla_membership_id = '" & str_vsla_membership & "'"

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Update)
            MsgBox("Record updated successfully.", MsgBoxStyle.Information)

            fillgrid()

            str_vsla_membership = ""

            BtnEdit.Enabled = False
            btnDelete.Enabled = False
            GroupBox1.Enabled = False
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("vsla_membership", "Edit", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub DataGridView2_CellContentClick_1(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellContentClick
        Dim ErrorAction As New functions
        Try

            'select item to edit or delete
            Dim mysqlaction As String = ""
            Dim MyDBAction As New functions
            Dim K As Integer

            K = e.RowIndex '- 1


            Dim MyDatable As New Data.DataTable
            mysqlaction = "SELECT distinct  vsla_membership.vsla_membership_id, vsla_membership.caregiver_id, " &
                                        "vsla_membership.vsla_id, vsla_membership.date_joined, vsla_membership.member_active_vsla,  " &
                                        " vsla_list.vsla_name, OVCRegistrationDetails.caregiver_names " &
                                        "FROM   vsla_membership INNER JOIN " &
                                        "OVCRegistrationDetails ON vsla_membership.caregiver_id = OVCRegistrationDetails.caregiver_id INNER JOIN " &
                                        "vsla_list ON vsla_membership.vsla_id = vsla_list.vslaid where vsla_membership.vsla_membership_id = " & Me.DataGridView2.Rows(K).Cells(0).Value & ""
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then
                txtcpimsid.Text = MyDatable.Rows(0).Item("caregiver_id").ToString
                txtcaregivernames.Text = MyDatable.Rows(0).Item("caregiver_names").ToString
                cboVSLA.SelectedValue = MyDatable.Rows(0).Item("vsla_id").ToString
                dtpDateJoined.Value = MyDatable.Rows(0).Item("date_joined").ToString
                chkMemberActive.Checked = CBool(MyDatable.Rows(0).Item("member_active_vsla").ToString)


                'initialize the record identifier
                str_vsla_membership = MyDatable.Rows(0).Item("vsla_membership_id").ToString

            End If



            'Panel1.Enabled = True
            BtnEdit.Enabled = True
            btnDelete.Enabled = True
            GroupBox1.Enabled = True
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("vslamembership", "GridCellClick", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub cbosearchcounty_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbosearchcounty.SelectedIndexChanged

    End Sub

    Private Sub cboVSLA_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboVSLA.SelectedIndexChanged

    End Sub

    Private Sub cboVSLA_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cboVSLA.KeyPress
        e.Handled = True
    End Sub

    Private Sub cbosearchcounty_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cbosearchcounty.KeyPress
        e.Handled = True
    End Sub

    Private Sub cbosearchcbo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbosearchcbo.SelectedIndexChanged

    End Sub

    Private Sub cbosearchcbo_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cbosearchcbo.KeyPress
        e.Handled = True
    End Sub

    Private Sub cbosearchward_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbosearchward.SelectedIndexChanged

    End Sub

    Private Sub cbosearchward_KeyPress(sender As Object, e As KeyPressEventArgs) Handles cbosearchward.KeyPress
        e.Handled = True
    End Sub
End Class