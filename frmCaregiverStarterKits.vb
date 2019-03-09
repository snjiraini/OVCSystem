Imports OVCSystem.functions
Imports System.Data.Odbc
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Text

Public Class frmCaregiverStarterKits
    Public str_caregiver_starterkit_id As String = ""
    Private Sub frmCaregiverStarterKits_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        populatecounties()
        populatestarterkits()
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
            ErrorAction.WriteToErrorLogFile("CaregiverStarterKits", "populatecounties", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populatestarterkits()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "SELECT [starterkit_id] " &
                                          ", [starterkit_name] " &
                                          ",[valuechain_id] " &
                                          ",[starterkit_type] " &
                                      "FROM [dbo].[starterkit_list] " &
                                      "order by [starterkit_name]"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cboStarterKit
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "starterkit_name"
                .ValueMember = "starterkit_id"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("CaregiverStarterKits", "populatestarterkits", ex.Message) ''---Write error to error log file

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
            ErrorAction.WriteToErrorLogFile("CaregiverStarterKits", "populatesearchcbo", ex.Message) ''---Write error to error log file

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
            ErrorAction.WriteToErrorLogFile("CaregiverStarterKits", "populatesearchward", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            Dim mySqlAction As String = "SELECT distinct caregiver_id,caregiver_names,county,cbo,ward,chv_names  from OVCRegistrationDetails where 1 = 1"

            If IsNumeric(cbosearchcounty.SelectedValue) Then
                mySqlAction = mySqlAction & " AND OVCRegistrationDetails.county = '" & cbosearchcounty.Text & "'"
            End If
            If IsNumeric(cbosearchcbo.SelectedValue) Then
                mySqlAction = mySqlAction & " AND OVCRegistrationDetails.cbo_id = '" & cbosearchcbo.SelectedValue & "'"
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


                    DataGridView1.Rows.Add(mycpimsid, mychvnames, mycbo, Mycounty, myward, "Select")
                Next
            Else 'if there are no rows returned
                MsgBox("Search query returned no results", MsgBoxStyle.Exclamation)
            End If

            'we set focus back to fname so that by clicking 'search' again, we loose focus on txtname thus 
            'recreating the search criteria again
            cbosearchcounty.Focus()

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("CaregiverStarterKits", "clientsearch", ex.Message) ''---Write error to error log file

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

    Private Sub fillgrid()
        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            Dim mySqlAction As String = "SELECT distinct  ovc_caregiver_starterkit.ovc_caregiver_starterkit_id, OVCRegistrationDetails.caregiver_names, " &
                                           " starterkit_list.starterkit_name, ovc_caregiver_starterkit.starterkit_id, ovc_caregiver_starterkit.date_provided,  " &
                                            "valuechain_List.valuechain_name " &
                                            "FROM   ovc_caregiver_starterkit INNER JOIN " &
                                            "OVCRegistrationDetails ON ovc_caregiver_starterkit.ovc_or_caregiver_id = OVCRegistrationDetails.caregiver_id  " &
                                            "INNER JOIN starterkit_list On ovc_caregiver_starterkit.starterkit_id = starterkit_list.starterkit_id  " &
                                            "INNER JOIN valuechain_List ON starterkit_list.valuechain_id = valuechain_List.valuechain_id " &
                                            "where OVCRegistrationDetails.caregiver_id  = '" & txtcpimsid.Text.ToString & "'"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim mycaregiverstarterkitid, mystarterkit, myvaluechain, mydateprovided As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView2.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    mycaregiverstarterkitid = MyDatable.Rows(K).Item("ovc_caregiver_starterkit_id").ToString
                    'mycaregivername = MyDatable.Rows(K).Item("caregiver_names").ToString
                    mystarterkit = MyDatable.Rows(K).Item("starterkit_name").ToString
                    myvaluechain = MyDatable.Rows(K).Item("valuechain_name").ToString
                    mydateprovided = MyDatable.Rows(K).Item("date_provided").ToString

                    DataGridView2.Rows.Add(mycaregiverstarterkitid, mystarterkit, myvaluechain, mydateprovided, "Select")
                Next
            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("CaregiverStarterKits", "Fillgrid", ex.Message) ''---Write error to error log file

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

            mysqlaction = "SELECT distinct caregiver_id,caregiver_names,county,cbo,ward,chv_names  from OVCRegistrationDetails where caregiver_id ='" & strcaregiverid & "'"
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
            ErrorAction.WriteToErrorLogFile("CaregiverStarterKits", "GridCellClick", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub lnkNew_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkNew.LinkClicked
        cboStarterKit.SelectedIndex = -1
        btnpost.Enabled = True
        btnDelete.Enabled = False
        BtnEdit.Enabled = False
        GroupBox1.Enabled = True
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
            mySqlAction = "INSERT INTO [dbo].[ovc_caregiver_starterkit] " &
                                  " ([ovc_or_caregiver_id] " &
                                  " ,[persons_type] " &
                                  " ,[starterkit_id] " &
                                  " ,[date_provided]) " &
                             "VALUES " &
                                  " ('" & txtcpimsid.Text.ToString & "' " &
                                   ",'CAREGIVER' " &
                                  " ,'" & cboStarterKit.SelectedValue.ToString & "' " &
                                   ",'" & dtpDateprovided.Value & "')"

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
            MsgBox("Record saved successfully.", MsgBoxStyle.Information)

            fillgrid()

            btnpost.Enabled = False
            GroupBox1.Enabled = False
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("CaregiverStarterKits", "Save", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Function validatecontrols() As Boolean
        Try

            ErrorProvider1.Clear()

            If cboStarterKit.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(cboStarterKit, "Please select a starterkit to continue")
                MsgBox("Please select a starterkit to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                cboStarterKit.Focus()
                Return False
            ElseIf dtpDateprovided.Value >= Date.Today Then
                ErrorProvider1.SetError(dtpDateprovided, "Please select correct date to continue")
                MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                dtpDateprovided.Focus()
                Return False

            End If

            'IF all controls have been validated, return true
            Return True

        Catch ex As Exception
            Return False 'incase this function throws exception, return false
        End Try
    End Function

    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles BtnExit.Click
        Me.Close()
    End Sub

    Private Sub DataGridView2_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView2.CellContentClick
        Dim ErrorAction As New functions
        Try

            'select item to edit or delete
            Dim mysqlaction As String = ""
            Dim MyDBAction As New functions
            Dim K As Integer

            K = e.RowIndex '- 1


            Dim MyDatable As New Data.DataTable
            mysqlaction = "SELECT distinct  ovc_caregiver_starterkit.ovc_caregiver_starterkit_id,ovc_caregiver_starterkit.ovc_or_caregiver_id, OVCRegistrationDetails.caregiver_names, " &
                                           " starterkit_list.starterkit_name, ovc_caregiver_starterkit.starterkit_id, ovc_caregiver_starterkit.date_provided,  " &
                                            "valuechain_List.valuechain_name " &
                                            "FROM   ovc_caregiver_starterkit INNER JOIN " &
                                            "OVCRegistrationDetails ON ovc_caregiver_starterkit.ovc_or_caregiver_id = OVCRegistrationDetails.caregiver_id  " &
                                            "INNER JOIN starterkit_list On ovc_caregiver_starterkit.starterkit_id = starterkit_list.starterkit_id  " &
                                            "INNER JOIN valuechain_List ON starterkit_list.valuechain_id = valuechain_List.valuechain_id " &
                                            "where ovc_caregiver_starterkit.ovc_caregiver_starterkit_id = " & Me.DataGridView2.Rows(K).Cells(0).Value & ""

            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then
                txtcpimsid.Text = MyDatable.Rows(0).Item("ovc_or_caregiver_id").ToString
                txtcaregivernames.Text = MyDatable.Rows(0).Item("caregiver_names").ToString
                cboStarterKit.SelectedValue = MyDatable.Rows(0).Item("starterkit_id").ToString
                dtpDateprovided.Value = MyDatable.Rows(0).Item("date_provided").ToString

                'initialize the record identifier
                str_caregiver_starterkit_id = MyDatable.Rows(0).Item("ovc_caregiver_starterkit_id").ToString

            End If



            'Panel1.Enabled = True
            BtnEdit.Enabled = True
            btnDelete.Enabled = True
            GroupBox1.Enabled = True
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("CaregiverStarterKits", "GridCellClick", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub BtnEdit_Click(sender As Object, e As EventArgs) Handles BtnEdit.Click
        Dim ErrorAction As New functions
        Try

            'update record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            mySqlAction = "UPDATE [dbo].[ovc_caregiver_starterkit] " &
                              " SET " &
                                 " [starterkit_id] = '" & cboStarterKit.SelectedValue.ToString & "' " &
                                 " ,[date_provided] = '" & dtpDateprovided.Value & "' " &
                             "WHERE ovc_caregiver_starterkit_id = '" & str_caregiver_starterkit_id & "'"

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Update)
            MsgBox("Record updated successfully.", MsgBoxStyle.Information)

            fillgrid()

            str_caregiver_starterkit_id = ""

            BtnEdit.Enabled = False
            btnDelete.Enabled = False
            GroupBox1.Enabled = False
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("CaregiverStarterKits", "Edit", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub cbosearchcounty_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbosearchcounty.SelectedIndexChanged

    End Sub
End Class