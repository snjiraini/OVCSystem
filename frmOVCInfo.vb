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
            Dim mySqlAction As String = "SELECT * from OVCRegistrationDetails where 1 = 1 and cbo_id in (" & strcbos & ")"

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
        populatefacilities()
        populatedisabilitytype()
    End Sub

    Private Sub populatedisabilitytype()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select disability_type_id,disability_type from disability_type  order by disability_type asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cbodisabilitytype
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "disability_type"
                .ValueMember = "disability_type_id"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "populatedisabilitytype", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populatefacilities()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select distinct facility from OVCRegistrationDetails  order by facility asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cboRehabCentre
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "facility"
                .ValueMember = 0
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "populatefacilities", ex.Message) ''---Write error to error log file

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
            BtnEdit.Enabled = True

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


    Private Sub BtnEdit_Click(sender As Object, e As EventArgs) Handles BtnEdit.Click
        Try
            Dim MyDBAction As New functions
            Dim mycpims_ovc_id As String = ""
            Dim myovcdisability_type As String = ""
            Dim mydisability_assessment_date As DateTime
            Dim myDiagnosis As String = ""
            Dim myInterventions As String = ""
            Dim myNCPWD_Contacts As String = ""
            Dim myNCPWD_Residence As String = ""
            Dim myNCPWD_Other_Support As String = ""
            Dim myNCPWD_Rehab_Centre As String = ""
            Dim myHIV_Screening_Date As DateTime
            Dim myHIV_Screening_Outcome As String = ""
            Dim myHIV_Testing_Date As DateTime



            If cbodisabilitytype.Text.Length > 0 Then
                myovcdisability_type = cbodisabilitytype.SelectedItem.ToString
            End If

            If cboDiagnosis.Text.Length > 0 Then
                myDiagnosis = cboDiagnosis.SelectedItem.ToString
            End If

            If cboIntervention.Text.Length > 0 Then
                myInterventions = cboIntervention.SelectedItem.ToString
            End If

            If cboScreeningOutcome.Text.Length > 0 Then
                myHIV_Screening_Outcome = cboScreeningOutcome.SelectedItem.ToString
            End If

            If dtpDateofDisabilityAssessment.Value = Date.Today Then
                mydisability_assessment_date = "1900-01-01 00:00:00.000"
            Else
                mydisability_assessment_date = dtpDateofDisabilityAssessment.Value
            End If

            If dtpScreeningDate.Value = Date.Today Then
                myHIV_Screening_Date = "1900-01-01 00:00:00.000"
            Else
                myHIV_Screening_Date = dtpDateofDisabilityAssessment.Value
            End If

            If dtpDateofTesting.Value = Date.Today Then
                myHIV_Testing_Date = "1900-01-01 00:00:00.000"
            Else
                myHIV_Testing_Date = dtpDateofDisabilityAssessment.Value
            End If

            myNCPWD_Contacts = txtNCPWDContacts.Text.ToString
            myNCPWD_Residence = txtward.Text.ToString
            myNCPWD_Other_Support = txtOtherSupport.Text.ToString

            If cboRehabCentre.Text.Length > 0 Then
                myNCPWD_Rehab_Centre = cboRehabCentre.SelectedItem.ToString
            Else
                myNCPWD_Rehab_Centre = ""
            End If

            If dtpScreeningDate.Value = Date.Today Then
                myHIV_Screening_Date = "1900-01-01 00:00:00.000"
            Else
                myHIV_Screening_Date = dtpScreeningDate.Value.ToString
            End If


            myHIV_Screening_Outcome = cboScreeningOutcome.SelectedItem.ToString

            If dtpScreeningDate.Value = Date.Today Then
                myHIV_Screening_Date = "1900-01-01 00:00:00.000"
            Else
                myHIV_Screening_Date = dtpScreeningDate.Value.ToString
            End If


            mycpims_ovc_id = txtOVCid.Text.ToString

            'MyDBAction.UpdateClient(txtOVC.Text.ToString, RegularExpressions.Regex.Replace(txtfname.Text.ToString, "[^a-zA-Z]", ""),
            '     RegularExpressions.Regex.Replace(txtmname.Text.ToString, "[^a-zA-Z]", ""), RegularExpressions.Regex.Replace(txtlname.Text.ToString, "[^a-zA-Z]", ""),
            '    cboGender.Text.ToString, Format(dtpDateofBirth.Value, "dd-MMM-yyyy"), chkHasCert.Checked, strCriteria, cboCBO.SelectedValue.ToString _
            '    , cboDistrict.SelectedValue.ToString, cboLocation.SelectedValue.ToString, strFacility.ToString, Format(dtpDateofLinkage.Value, "dd-MMM-yyyy"), cboImmunization.SelectedValue.ToString, cboCHWName.SelectedValue.ToString, cboSchoolLevel.SelectedValue.ToString, strschool, strclass, cboHIVStatus.SelectedValue.ToString _
            '    , strART, "0", guardian_id.ToString, father_id.ToString, mother_id.ToString, householdhead_id.ToString, cboCareTaker.SelectedItem, Format(dtpDateofVisit.Value, "dd-MMM-yyyy"), chkExit.Checked, Format(dtpDateofRegistration.Value, "dd-MMM-yyyy") _
            '    , Format(dtpDateofExit.Value, "dd-MMM-yyyy"), strExitReason, CDate(Date.Now.ToString), strsession.ToString, cboschoolingtype.Text.ToString,
            '       strbcertnumber.ToString, chkOVCDisabled.Checked, txtNCPWDnum.Text.ToString, txtcccnumber.Text.ToString, strmode)

            MyDBAction.UpdateClient(myovcdisability_type _
      , mydisability_assessment_date _
      , myDiagnosis _
      , myInterventions _
      , myNCPWD_Contacts _
      , myNCPWD_Residence _
      , myNCPWD_Other_Support _
      , myNCPWD_Rehab_Centre _
      , myHIV_Screening_Date _
      , myHIV_Screening_Outcome _
      , myHIV_Testing_Date _
        , mycpims_ovc_id)

            BtnEdit.Enabled = False

            MsgBox("Record edited successfully")
        Catch ex As Exception
            MsgBox(ex.Message, vbExclamation)
            MsgBox("Record update NOT successful", vbExclamation)

        End Try
    End Sub

End Class