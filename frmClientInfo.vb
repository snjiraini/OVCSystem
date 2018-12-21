Imports OVCSystem.functions
Imports System.Data.Odbc
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Text

Public Class frmClientInfo
    Public intclientlongid As Integer = 0
    Dim strnames As String = "" 'this variable holds part of the search query for names

    Dim myPreviousHIVStatus As String = "" 'will help us avoid HIV+ children changing back to HIV-
    Dim myPreviousBCert As Boolean = False 'will help us track birthcertificate to avoid OVC having Bcert, then later having no Bcert.

    Private Sub fillgrid()
        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            Dim mySqlAction As String = "select * from Clientdetails where district in (" & strdistricts & ") and cbo in (" & strcbos & ")"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim myClientid, myFname, MyMname, myLname, myOVCID As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView1.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    myClientid = MyDatable.Rows(K).Item("ClientID").ToString
                    myFname = MyDatable.Rows(K).Item("Firstname").ToString
                    MyMname = MyDatable.Rows(K).Item("Middlename").ToString
                    myLname = MyDatable.Rows(K).Item("Surname").ToString
                    myOVCID = MyDatable.Rows(K).Item("OVCID").ToString
                    DataGridView1.Rows.Add(myClientid, myFname, MyMname, myLname, myOVCID, "Select")
                Next
            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("Clientinfo", "fillgrid", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub filllongitudinalgrid()
        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            Dim mySqlAction As String = "select * from Clientlongitudinaldetails where ovcid = '" & txtOVC.Text & "' "
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim myClientlongid, myOVCID As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView2.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    myClientlongid = MyDatable.Rows(K).Item("ClientlongID").ToString
                    myOVCID = MyDatable.Rows(K).Item("OVCID").ToString
                    DataGridView2.Rows.Add(myClientlongid, myOVCID, "Select")
                Next
            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("Clientinfo", "filllongitudinalgrid", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub frmClientInfo_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        m_RegistrationFormNumber = 0
    End Sub

    Private Sub frmClientInfo_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'populate the comboboxes on page
        'populatecombos("District")

        populatedistricts()
        populatesearchdistricts()
        populateFacilities()
        populateCriteria()
        populateSchoolLevel()

        populateHIVStatus()
        populateARTStatus()
        populateImmunizationStatus()



        populateFatherCauseofDeath()
        populateMotherCauseofDeath()
        populateHidingReason()
        'populateCHW()
        populateRelationship()
        populateFatherHIVStatus()
        populateMotherHIVStatus()
        'fillgrid()
        showOVCCount()

        'if user has right to delete, show the delete button
        'also allow for Exiting using the same priviledge
        If candelete = True Then
            btnDelete.Visible = False
            chkExit.Enabled = True
        Else
            btnDelete.Visible = False
            chkExit.Enabled = False
        End If

        Me.AcceptButton = btnpost


        'Run this if we are calling this form from Form1A Longitudinal update of a service
        If UCase(Me.Text) = UCase("Form1A Longitudinal Update") Then
            select_ovc(myOVCID, 10)
        End If
    End Sub

    Private Function showHistoricalLongitudinalCount(ByVal strovcid As String) As Double
        Dim MyDBAction As New functions
        Dim SqlAction As String = "SELECT count(clientlongid) from clientlongitudinaldetails where ovcid = '" & strovcid & "'"
        Dim Myhistorycount As Double = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)
        Return Myhistorycount
    End Function

    Private Sub showOVCCount()
        Dim MyDBAction As New functions
        Dim SqlAction As String = "SELECT count(clientid) from clientdetails where gender = 'Male' and district in (" & strdistricts & ") and cbo in (" & strcbos & ")"
        Dim Mymales As Double = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

        SqlAction = "SELECT count(clientid) from clientdetails where gender = 'Female' and district in (" & strdistricts & ") and cbo in (" & strcbos & ")"
        Dim myFemales As Double = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

        Label45.Text = "[Ever Registered]     Male:" & Mymales & "   Female:" & myFemales & "   Total:" & (Mymales + myFemales)

        SqlAction = "SELECT count(clientid) from clientdetails where  isnull([Exited],'False') = 'False' and gender = 'Male' " &
        " and district in (" & strdistricts & ") and cbo in (" & strcbos & ")"
        Mymales = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

        SqlAction = "SELECT count(clientid) from clientdetails where isnull([Exited],'False') = 'False' and gender = 'Female' " &
        " and district in (" & strdistricts & ") and cbo in (" & strcbos & ")"
        myFemales = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

        Label53.Text = "[Current Registered] Male:" & Mymales & "   Female:" & myFemales & "   Total:" & (Mymales + myFemales)

    End Sub

    Private Sub populateRelationship()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from Relationship "
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboRelationship
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "Relationship"
                .ValueMember = "RelationshipID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("Clientinfo", "populaterelationship", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populateFacilities()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from Facilities  order by Facility asc "
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboFacility
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "Facility"
                .ValueMember = "FacilityID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("Clientinfo", "populatefacilities", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populateCHW(ByVal mycboid As String)
        'populate the combobox
        Dim mySqlAction As String = "SELECT CHWID, FirstName + ' ' + MiddleName + ' ' + Surname + '---' + ID AS name " &
        " FROM CHW where cboid = '" & mycboid.ToString & "' ORDER BY FirstName, MiddleName, Surname"

        Dim MyDBAction As New functions
        Dim MyDatable As New Data.DataTable
        MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
        MyDatable.Rows.Add()
        With cboCHWName
            .DataSource = Nothing
            .Items.Clear()
            .DataSource = MyDatable
            .DisplayMember = "name"
            .ValueMember = "CHWID"
            .SelectedIndex = -1 ' This line makes the combo default value to be blank
        End With

    End Sub
    Private Sub populatedistricts()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from District where districtid in (" & strdistricts & ") order by district asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cboDistrict
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "District"
                .ValueMember = "DistrictID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "populatedistricts", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populatesearchdistricts()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from District where districtid in (" & strdistricts & ") order by district asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cbosearchdistrict
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "District"
                .ValueMember = "DistrictID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "populatesearchdistricts", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populatelocations(ByVal mydistrict As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from Location where districtid ='" & mydistrict & "'"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cboLocation
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "Location"
                .ValueMember = "LocationID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "populatelocations", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populateCriteria()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "SELECT * from clienttype ORDER BY clienttype"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With lstCriteria
                .DataSource = Nothing 'if already bound, throws a bug on any refresh
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "clienttype"
                .ValueMember = "clienttypeid"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "populatecriteria", ex.Message) ''---Write error to error log file


        End Try
    End Sub

    'Private Sub populateclienttype()
    '    Dim ErrorAction As New functions
    '    Try

    '        'populate the combobox
    '        Dim mySqlAction As String = "select * from Clienttype "
    '        Dim MyDBAction As New functions
    '        Dim MyDatable As New Data.DataTable
    '        MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
    '        With cboClienttype
    '            .DataSource = Nothing
    '            .Items.Clear()
    '            .DataSource = MyDatable
    '            .DisplayMember = "Clienttype"
    '            .ValueMember = "ClienttypeID"
    '            .SelectedIndex = -1 ' This line makes the combo default value to be blank
    '        End With
    '    Catch ex As Exception
    '        ErrorAction.WriteToErrorLogFile("clientinfo", "populateclienttype", ex.Message) ''---Write error to error log file

    '    End Try
    'End Sub
    Private Sub populateCBO(ByVal mydistrict As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from CBO where districtid = '" & mydistrict.ToString &
            "' and cboid in (" & strcbos & ")  order by cbo asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboCBO
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "CBO"
                .ValueMember = "CBOID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "populatecbo", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populatesearchCBO(ByVal mydistrict As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from CBO where districtid = '" & mydistrict.ToString &
            "' and cboid in (" & strcbos & ")  order by cbo asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cbosearchcbo
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "CBO"
                .ValueMember = "CBOID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "populatesearchcbo", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populateSchoolLevel()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from SchoolLevel "
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboSchoolLevel
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "SchoolLevel"
                .ValueMember = "SchoolLevelID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "populateschoollevel", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populateSchools(ByVal myschoollevel As Integer)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from Schools where schoollevel = '" & myschoollevel & "' order by schoolname asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboSchoolName
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "Schoolname"
                .ValueMember = "SchoolID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "populateschools", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populateclass(ByVal myschoollevel As Integer)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from class where schoollevel = '" & myschoollevel & "'"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboClass
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "class"
                .ValueMember = "classID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "populateclass", ex.Message) ''---Write error to error log file

        End Try
    End Sub




    Private Sub populateHIVStatus()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from HIVStatus "
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboHIVStatus
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "HIVStatus"
                .ValueMember = "HIVStatusID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "populatehivstatus", ex.Message) ''---Write error to error log file

        End Try
    End Sub
    Private Sub populateMotherHIVStatus()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from HIVStatus "
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboMotherHIVStatus
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "HIVStatus"
                .ValueMember = "HIVStatusID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "populatemotherhivstatus", ex.Message) ''---Write error to error log file

        End Try
    End Sub
    Private Sub populateFatherHIVStatus()
        Dim ErrorAction As New functions
        Try
            'populate the combobox
            Dim mySqlAction As String = "select * from HIVStatus "
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboFatherHIVStatus
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "HIVStatus"
                .ValueMember = "HIVStatusID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "populatefatherhivstatus", ex.Message) ''---Write error to error log file

        End Try
    End Sub
    Private Sub populateARTStatus()
        Dim ErrorAction As New functions
        Try
            'populate the combobox
            Dim mySqlAction As String = "select * from ARTStatus "
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboARTStatus
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "ARTStatus"
                .ValueMember = "ARTStatusID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "populateartstatus", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populateImmunizationStatus()
        Dim ErrorAction As New functions
        Try
            'populate the combobox
            Dim mySqlAction As String = "select * from Immunization "
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboImmunization
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "ImmunizationStatus"
                .ValueMember = "ImmunizationID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "populateimmunizationstatus", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populateHidingReason()
        Dim ErrorAction As New functions
        Try
            'populate the combobox
            Dim mySqlAction As String = "select * from HidingReason "
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboHideReason
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "HidingReason"
                .ValueMember = "HidingReasonID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "populatehidingreason", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populateFatherCauseofDeath()
        Dim ErrorAction As New functions
        Try
            'populate the combobox
            Dim mySqlAction As String = "select * from CauseofDeath "
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboFatherCauseofDeath
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "CauseofDeath"
                .ValueMember = "CauseofDeathID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "fathercauseofdeath", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populateMotherCauseofDeath()
        Dim ErrorAction As New functions
        Try
            'populate the combobox
            Dim mySqlAction As String = "select * from CauseofDeath "
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboMotherCauseofDeath
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "CauseofDeath"
                .ValueMember = "CauseofDeathID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "mothercauseofdeath", ex.Message) ''---Write error to error log file

        End Try
    End Sub
    '' ''to avoid copy pasting for populating combos, just pass the 
    '' ''table name as a parameter and reuse the same code
    ' ''Private Sub populatecombos(ByVal tblname As String)
    ' ''    'populate the combobox
    ' ''    Dim mySqlAction As String = "select * from " & tblname & " "
    ' ''    Dim MyDBAction As New functions
    ' ''    Dim MyDatable As New Data.DataTable
    ' ''    Dim comboname As String
    ' ''    MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
    ' ''    'comboname = "cbo" & tblname
    ' ''    Dim comboname1 As New ComboBox
    ' ''    'comboname1.Name = comboname
    ' ''    'comboname1 = TryCast(comboname, ComboBox) 'combo box name e.g cboDistrict
    ' ''    With comboname1
    ' ''        .Items.Clear()
    ' ''        .DataSource = MyDatable
    ' ''        .DisplayMember = tblname 'the main field name e.g District
    ' ''        .ValueMember = tblname & "ID" 'the table primary key e.g DistrictID
    ' ''    End With

    ' ''End Sub



    'Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click

    '    Dim ErrorAction As New functions
    '    Try

    '        'populate the datagrid with all the data
    '        Dim mySqlAction As String = "SELECT Clientdetails.Clientid,Clientdetails.OVCID, Clientdetails.FirstName, Clientdetails.MiddleName,Clientdetails.Surname, CHW.CHWID, CHW.FirstName AS CHWFirstName, " & _
    '    " CHW.MiddleName AS CHWMiddleName, CHW.Surname AS CHWSurname, CHW.ID" & _
    '    " FROM Clientdetails LEFT OUTER JOIN" & _
    '    " CHW ON Clientdetails.VolunteerId = CHW.CHWID where 1 = 1"

    '        If IsNumeric(cbosearchdistrict.SelectedValue) Then
    '            mySqlAction = mySqlAction & " AND Clientdetails.District = '" & cbosearchdistrict.SelectedValue & "'"
    '        End If
    '        If IsNumeric(cbosearchcbo.SelectedValue) Then
    '            mySqlAction = mySqlAction & " AND Clientdetails.cbo = '" & cbosearchcbo.SelectedValue & "'"
    '        End If
    '        If txtsearchFname.Text.ToString.Length <> 0 Then
    '            mySqlAction = mySqlAction & " AND Clientdetails.firstname = '" & txtsearchFname.Text.ToString & "'"
    '        End If
    '        If txtsearchMname.Text.ToString.Length <> 0 Then
    '            mySqlAction = mySqlAction & " AND Clientdetails.middlename = '" & txtsearchMname.Text.ToString & "'"
    '        End If
    '        If txtsearchSurname.Text.ToString.Length <> 0 Then
    '            mySqlAction = mySqlAction & " AND Clientdetails.surname = '" & txtsearchSurname.Text.ToString & "'"
    '        End If
    '        If txtsearchchwfname.Text.ToString.Length <> 0 Then
    '            mySqlAction = mySqlAction & " AND Chw.firstname = '" & txtsearchchwfname.Text.ToString & "'"
    '        End If
    '        If txtsearchchwmname.Text.ToString.Length <> 0 Then
    '            mySqlAction = mySqlAction & " AND Chw.middlename = '" & txtsearchchwmname.Text.ToString & "'"
    '        End If
    '        If txtsearchchwsurname.Text.ToString.Length <> 0 Then
    '            mySqlAction = mySqlAction & " AND Chw.surname = '" & txtsearchchwsurname.Text.ToString & "'"
    '        End If
    '        mySqlAction = mySqlAction & " order by Clientdetails.firstname, Clientdetails.middlename, Clientdetails.surname asc"

    '        'Dim mySqlAction As String = "select * from Clientdetails where district = '" & cbosearchdistrict.SelectedValue.ToString & "' " & _
    '        '" and CBO = '" & cbosearchcbo.SelectedValue.ToString & "' " & strnames & " order by firstname, middlename, surname asc"

    '        'clear the strnames variable so as to create new criteria on next search
    '        strnames = ""


    '        Dim MyDBAction As New functions
    '        Dim MyDatable As New Data.DataTable
    '        Dim myClientid, myFname, MyMname, myLname, myOVCID, mychwfname, mychwmname, mychwsurname, mychwid As String
    '        MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
    '        DataGridView1.Rows.Clear()
    '        If MyDatable.Rows.Count > 0 Then
    '            For K = 0 To MyDatable.Rows.Count - 1
    '                myClientid = MyDatable.Rows(K).Item("ClientID").ToString
    '                myFname = MyDatable.Rows(K).Item("Firstname").ToString
    '                MyMname = MyDatable.Rows(K).Item("Middlename").ToString
    '                myLname = MyDatable.Rows(K).Item("Surname").ToString
    '                myOVCID = MyDatable.Rows(K).Item("OVCID").ToString
    '                mychwfname = MyDatable.Rows(K).Item("chwfirstname").ToString
    '                mychwmname = MyDatable.Rows(K).Item("chwmiddlename").ToString
    '                mychwsurname = MyDatable.Rows(K).Item("chwsurname").ToString
    '                mychwid = MyDatable.Rows(K).Item("chwid").ToString
    '                DataGridView1.Rows.Add(myClientid, myFname, MyMname, myLname, myOVCID, mychwfname, mychwmname, mychwsurname, mychwid, "Select")
    '            Next
    '        Else 'if there are no rows returned
    '            MsgBox("Search query returned no results", MsgBoxStyle.Exclamation)
    '        End If

    '        'we set focus back to fname so that by clicking 'search' again, we loose focus on txtname thus 
    '        'recreating the search criteria again
    '        txtsearchFname.Focus()

    '    Catch ex As Exception
    '        ErrorAction.WriteToErrorLogFile("clientinfo", "clientsearch", ex.Message) ''---Write error to error log file

    '    End Try
    'End Sub

    'Private Function validatecontrols() As Boolean
    '    Try

    '        ErrorProvider1.Clear()
    '        If txtfname.Text.Trim.Length = 0 Then
    '            ErrorProvider1.SetError(txtfname, "Please Enter FirstName to continue")
    '            MsgBox("Please Enter some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '            txtfname.Focus()
    '            Return False
    '        ElseIf txtmname.Text.Trim.Length = 0 Then
    '            ErrorProvider1.SetError(txtmname, "Please Enter MiddleName to continue")
    '            MsgBox("Please Enter some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '            txtmname.Focus()
    '            Return False
    '        ElseIf txtlname.Text.Trim.Length = 0 Then
    '            ErrorProvider1.SetError(txtlname, "Please Enter LastName to continue")
    '            MsgBox("Please Enter some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '            txtlname.Focus()
    '            Return False
    '        ElseIf cboGender.Text.Trim.Length = 0 Then
    '            ErrorProvider1.SetError(cboGender, "Please Enter Gender to continue")
    '            MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '            cboGender.Focus()
    '            Return False
    '        ElseIf dtpDateofBirth.Value = Date.Today Then
    '            ErrorProvider1.SetError(dtpDateofBirth, "Please select DOB to continue")
    '            MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '            dtpDateofBirth.Focus()
    '            Return False
    '        ElseIf cboDistrict.Text.Trim.Length = 0 Then
    '            ErrorProvider1.SetError(cboDistrict, "Please select district to continue")
    '            MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '            cboDistrict.Focus()
    '            Return False
    '        ElseIf cboLocation.Text.Trim.Length = 0 Then
    '            ErrorProvider1.SetError(cboLocation, "Please select Location to continue")
    '            MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '            cboLocation.Focus()
    '            Return False
    '        ElseIf cboCBO.Text.Trim.Length = 0 Then
    '            ErrorProvider1.SetError(cboCBO, "Please select CBO to continue")
    '            MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '            cboCBO.Focus()
    '            Return False
    '        ElseIf cboCHWName.Text.Trim.Length = 0 Then
    '            ErrorProvider1.SetError(cboCHWName, "Please select volunteer to continue")
    '            MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '            cboCHWName.Focus()
    '            Return False
    '            'ElseIf cboClienttype.Text.Trim.Length = 0 Then 'Client type has changed to Orphanhood status
    '            '    ErrorProvider1.SetError(cboClienttype, "Please select Orphanhood status to continue")
    '            '    MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '            '    cboClienttype.Focus()
    '            '    Return False
    '        ElseIf cboSchoolLevel.Text.Trim.Length = 0 Then
    '            ErrorProvider1.SetError(cboSchoolLevel, "Please select school level to continue")
    '            MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '            cboSchoolLevel.Focus()
    '            Return False
    '        ElseIf cboHIVStatus.Text.Trim.Length = 0 Then
    '            ErrorProvider1.SetError(cboHIVStatus, "Please select HIV Status to continue")
    '            MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '            cboHIVStatus.Focus()
    '            Return False
    '        ElseIf cboHIVStatus.Text = "Positive" And cboARTStatus.Text.Trim.Length = 0 Then
    '            ErrorProvider1.SetError(cboARTStatus, "Please select ART Status to continue")
    '            MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '            cboARTStatus.Focus()
    '            Return False
    '        ElseIf father_id = "0" And mother_id = "0" And guardian_id = "0" Then
    '            ErrorProvider1.SetError(GroupBox4, "Please select father to continue")
    '            ErrorProvider1.SetError(GroupBox5, "Please select mother to continue")
    '            ErrorProvider1.SetError(GroupBox6, "Please select guardian to continue")
    '            MsgBox("Please provide parent or gurdian details to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '            Return False
    '        ElseIf chkExit.Checked = True Then
    '            If cboHideReason.Text.Trim.Length = 0 Then
    '                ErrorProvider1.SetError(cboHideReason, "Please select Exit Reason to continue")
    '                MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '                cboHideReason.Focus()
    '                Return False
    '            ElseIf dtpDateofExit.Value >= Date.Today Then
    '                ErrorProvider1.SetError(dtpDateofExit, "Please select DateofExit to continue")
    '                MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '                dtpDateofExit.Focus()
    '                Return False
    '                'date of exit should never be more then 2 mnths ago
    '            ElseIf DateDiff(DateInterval.Month, dtpDateofExit.Value, Date.Today) > 2 AndAlso chkExit.Checked = True Then
    '                ErrorProvider1.SetError(dtpDateofExit, "Please select [Date of Exit] to continue")
    '                MsgBox("Exits cannot be more than 2 months ago.", MsgBoxStyle.Exclamation, Me.Text)
    '                dtpDateofExit.Focus()
    '                Return False
    '            End If



    '        ElseIf dtpDateofRegistration.Value >= Date.Today Then
    '            ErrorProvider1.SetError(dtpDateofExit, "Please select DateofRegistration to continue")
    '            MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '            dtpDateofExit.Focus()
    '            Return False

    '            'date of registration should never be more then 2 mnths ago, but do so only when saving new, not when editing existing
    '        ElseIf DateDiff(DateInterval.Month, dtpDateofRegistration.Value, Date.Today) > 2 AndAlso (lblMode.Text = "[Normal Edit Mode]") AndAlso btnpost.Enabled = True Then
    '            ErrorProvider1.SetError(dtpDateofRegistration, "Please select [Date of Registration] to continue")
    '            MsgBox("Registration cannot be more than 2 months ago.", MsgBoxStyle.Exclamation, Me.Text)
    '            dtpDateofRegistration.Focus()
    '            Return False

    '            '    'date of registration for previous month should be done by 10th of the next month
    '            'ElseIf DateDiff(DateInterval.Month, dtpDateofRegistration.Value, Date.Today) > 0 _
    '            ' AndAlso Format(Date.Today, "dd") > 10 Then
    '            '    ErrorProvider1.SetError(dtpDateofRegistration, "Please select [Date of Registration] to continue")
    '            '    MsgBox("Previous month's registrations should be done by 10th of the next month.", MsgBoxStyle.Exclamation, Me.Text)
    '            '    dtpDateofRegistration.Focus()
    '            '    Return False

    '        ElseIf cboCareTaker.Text.Trim.Length = 0 Then
    '            ErrorProvider1.SetError(cboCareTaker, "Please select CareTaker to continue")
    '            MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
    '            cboCareTaker.Focus()
    '            Return False
    '        End If

    '        'IF all controls have been validated, return true
    '        Return True

    '    Catch ex As Exception
    '        Return False 'incase this function throws exception, return false
    '    End Try
    'End Function

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click

        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            Dim mySqlAction As String = "SELECT Clientdetails.Clientid,Clientdetails.OVCID, Clientdetails.FirstName, Clientdetails.MiddleName,Clientdetails.Surname, CHW.CHWID, CHW.FirstName AS CHWFirstName, " &
        " CHW.MiddleName AS CHWMiddleName, CHW.Surname AS CHWSurname, CHW.ID," &
        " (select count(clientlongid) from ClientLongitudinalDetails where ClientLongitudinalDetails.OVCID = Clientdetails.OVCID) as LongitudinalRecords," &
         " CASE WHEN  (select count(needsassessmentid) from Needsassessmentmain where Needsassessmentmain.OVCID = Clientdetails.OVCID)  > 0 THEN 'TRUE' ELSE 'FALSE' END as HasCSIAssessment," &
          " CASE WHEN clientdetails.hhvulnerabilitystatus in ('Lower','Medium','High') THEN 'TRUE' ELSE 'FALSE' END   as HasHHAssessment" &
        " FROM Clientdetails LEFT OUTER JOIN" &
        " CHW ON Clientdetails.VolunteerId = CHW.CHWID where 1 = 1"

            If IsNumeric(cbosearchdistrict.SelectedValue) Then
                mySqlAction = mySqlAction & " AND Clientdetails.District = '" & cbosearchdistrict.SelectedValue & "'"
            End If
            If IsNumeric(cbosearchcbo.SelectedValue) Then
                mySqlAction = mySqlAction & " AND Clientdetails.cbo = '" & cbosearchcbo.SelectedValue & "'"
            End If
            If txtsearchFname.Text.ToString.Length <> 0 Then
                mySqlAction = mySqlAction & " AND Clientdetails.firstname = '" & txtsearchFname.Text.ToString & "'"
            End If
            If txtsearchMname.Text.ToString.Length <> 0 Then
                mySqlAction = mySqlAction & " AND Clientdetails.middlename = '" & txtsearchMname.Text.ToString & "'"
            End If
            If txtsearchSurname.Text.ToString.Length <> 0 Then
                mySqlAction = mySqlAction & " AND Clientdetails.surname = '" & txtsearchSurname.Text.ToString & "'"
            End If
            If txtsearchchwfname.Text.ToString.Length <> 0 Then
                mySqlAction = mySqlAction & " AND Chw.firstname = '" & txtsearchchwfname.Text.ToString & "'"
            End If
            If txtsearchchwmname.Text.ToString.Length <> 0 Then
                mySqlAction = mySqlAction & " AND Chw.middlename = '" & txtsearchchwmname.Text.ToString & "'"
            End If
            If txtsearchchwsurname.Text.ToString.Length <> 0 Then
                mySqlAction = mySqlAction & " AND Chw.surname = '" & txtsearchchwsurname.Text.ToString & "'"
            End If
            mySqlAction = mySqlAction & " order by Clientdetails.firstname, Clientdetails.middlename, Clientdetails.surname asc"

            'Dim mySqlAction As String = "select * from Clientdetails where district = '" & cbosearchdistrict.SelectedValue.ToString & "' " & _
            '" and CBO = '" & cbosearchcbo.SelectedValue.ToString & "' " & strnames & " order by firstname, middlename, surname asc"

            'clear the strnames variable so as to create new criteria on next search
            strnames = ""


            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim myClientid, myFname, MyMname, myLname, myOVCID, mychwfname, mychwmname, mychwsurname,
                mychwid, mylongitudinalrecords As String
            Dim myHasCSIAssessment, myHasHHAssessment As Boolean
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView1.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    myClientid = MyDatable.Rows(K).Item("ClientID").ToString
                    myFname = MyDatable.Rows(K).Item("Firstname").ToString
                    MyMname = MyDatable.Rows(K).Item("Middlename").ToString
                    myLname = MyDatable.Rows(K).Item("Surname").ToString
                    myOVCID = MyDatable.Rows(K).Item("OVCID").ToString
                    mychwfname = MyDatable.Rows(K).Item("chwfirstname").ToString
                    mychwmname = MyDatable.Rows(K).Item("chwmiddlename").ToString
                    mychwsurname = MyDatable.Rows(K).Item("chwsurname").ToString
                    mychwid = MyDatable.Rows(K).Item("chwid").ToString
                    myHasCSIAssessment = CBool(MyDatable.Rows(K).Item("hascsiassessment").ToString)
                    myHasHHAssessment = CBool(MyDatable.Rows(K).Item("hashhassessment").ToString)
                    mylongitudinalrecords = MyDatable.Rows(K).Item("LongitudinalRecords").ToString
                    DataGridView1.Rows.Add(myClientid, myFname, MyMname, myLname, myOVCID, mychwfname,
                                           mychwmname, mychwsurname, mychwid, "Select", "", mylongitudinalrecords, myHasCSIAssessment, myHasHHAssessment)
                Next
            Else 'if there are no rows returned
                MsgBox("Search query returned no results", MsgBoxStyle.Exclamation)
            End If

            'we set focus back to fname so that by clicking 'search' again, we loose focus on txtname thus 
            'recreating the search criteria again
            txtsearchFname.Focus()

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "clientsearch", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Function validatecontrols() As Boolean
        Try

            ErrorProvider1.Clear()
            If txtOVC.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(txtOVC, "Please Enter OVCid to continue")
                MsgBox("Please Enter some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                txtOVC.Focus()
                Return False
            ElseIf txtfname.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(txtfname, "Please Enter FirstName to continue")
                MsgBox("Please Enter some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                txtfname.Focus()
                Return False
            ElseIf txtmname.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(txtmname, "Please Enter MiddleName to continue")
                MsgBox("Please Enter some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                txtmname.Focus()
                Return False
            ElseIf txtlname.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(txtlname, "Please Enter LastName to continue")
                MsgBox("Please Enter some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                txtlname.Focus()
                Return False
            ElseIf cboGender.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(cboGender, "Please Enter Gender to continue")
                MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                cboGender.Focus()
                Return False
            ElseIf dtpDateofBirth.Value >= Date.Today Then
                ErrorProvider1.SetError(dtpDateofBirth, "Please select correct DOB to continue")
                MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                dtpDateofBirth.Focus()
                Return False
            ElseIf cboDistrict.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(cboDistrict, "Please select district to continue")
                MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                cboDistrict.Focus()
                Return False
            ElseIf cboLocation.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(cboLocation, "Please select Location to continue")
                MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                cboLocation.Focus()
                Return False
            ElseIf cboCBO.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(cboCBO, "Please select CBO to continue")
                MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                cboCBO.Focus()
                Return False
            ElseIf cboCHWName.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(cboCHWName, "Please select volunteer to continue")
                MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                cboCHWName.Focus()
                Return False
            ElseIf lstCriteria.CheckedItems.Count = 0 Then
                ErrorProvider1.SetError(lstCriteria, "Please select vulnerability Criteria to continue")
                MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                lstCriteria.Focus()
                Return False
                'ElseIf cboClienttype.Text.Trim.Length = 0 Then 'Client type has changed to Orphanhood status
                '    ErrorProvider1.SetError(cboClienttype, "Please select Orphanhood status to continue")
                '    MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                '    cboClienttype.Focus()
                '    Return False
            ElseIf cboSchoolLevel.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(cboSchoolLevel, "Please select school level to continue")
                MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                cboSchoolLevel.Focus()
                Return False
            ElseIf cboHIVStatus.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(cboHIVStatus, "Please select HIV Status to continue")
                MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                cboHIVStatus.Focus()
                Return False
            ElseIf cboFacility.Text.Trim.Length > 0 AndAlso dtpDateofLinkage.Value >= Date.Today Then
                ErrorProvider1.SetError(dtpDateofLinkage, "Please select Date of linkage to continue")
                MsgBox("Please select Date of linkage to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                dtpDateofLinkage.Focus()
                Return False
            ElseIf cboFacility.Text.Trim.Length > 0 AndAlso txtcccnumber.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(txtcccnumber, "Please enter CCC to continue")
                MsgBox("Please enter CCC to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                txtcccnumber.Focus()
                Return False
            ElseIf cboHIVStatus.Text = "Positive" And cboARTStatus.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(cboARTStatus, "Please select ART Status to continue")
                MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                cboARTStatus.Focus()
                Return False
            ElseIf father_id = "0" And mother_id = "0" And guardian_id = "0" Then
                ErrorProvider1.SetError(GroupBox4, "Please select father to continue")
                ErrorProvider1.SetError(GroupBox5, "Please select mother to continue")
                ErrorProvider1.SetError(GroupBox6, "Please select guardian to continue")
                MsgBox("Please provide parent or gurdian details to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                Return False
            ElseIf chkExit.Checked = True Then
                If cboHideReason.Text.Trim.Length = 0 Then
                    ErrorProvider1.SetError(cboHideReason, "Please select Exit Reason to continue")
                    MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                    cboHideReason.Focus()
                    Return False
                ElseIf dtpDateofExit.Value >= Date.Today Then
                    ErrorProvider1.SetError(dtpDateofExit, "Please select DateofExit to continue")
                    MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                    dtpDateofExit.Focus()
                    Return False
                    'date of exit should never be more then 2 mnths ago
                ElseIf DateDiff(DateInterval.Month, dtpDateofExit.Value, Date.Today) > CInt(strexitsdefault.ToString) AndAlso chkExit.Checked = True Then
                    ErrorProvider1.SetError(dtpDateofExit, "Please select [Date of Exit] to continue")
                    MsgBox("Exits cannot be more than " & CInt(strexitsdefault.ToString) & " months ago.", MsgBoxStyle.Exclamation, Me.Text)
                    dtpDateofExit.Focus()
                    Return False
                End If

            ElseIf dtpDateofRegistration.Value >= Date.Today Then
                ErrorProvider1.SetError(dtpDateofExit, "Please select DateofRegistration to continue")
                MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                dtpDateofExit.Focus()
                Return False

                'date of registration should never be more then 2 mnths ago, but do so only when saving new, not when editing existing
            ElseIf DateDiff(DateInterval.Month, dtpDateofRegistration.Value, Date.Today) > CInt(strregistrationdefault.ToString) AndAlso (lblMode.Text = "[Normal Edit Mode]") AndAlso btnpost.Enabled = True Then
                ErrorProvider1.SetError(dtpDateofRegistration, "Please select [Date of Registration] to continue")
                MsgBox("Registration cannot be more than " & CInt(strregistrationdefault.ToString) & " months ago.", MsgBoxStyle.Exclamation, Me.Text)
                dtpDateofRegistration.Focus()
                Return False

                '    '    'date of registration for previous month should be done by 10th of the next month
                'ElseIf DateDiff(DateInterval.Month, dtpDateofRegistration.Value, Date.Today) > 0 _
                ' AndAlso Format(Date.Today, "dd") > 10 Then
                '    ErrorProvider1.SetError(dtpDateofRegistration, "Please select [Date of Registration] to continue")
                '    MsgBox("Previous month's registrations should be done by 10th of the next month.", MsgBoxStyle.Exclamation, Me.Text)
                '    dtpDateofRegistration.Focus()
                '    Return False

            ElseIf cboCareTaker.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(cboCareTaker, "Please select CareTaker to continue")
                MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                cboCareTaker.Focus()
                Return False
            End If

            'IF all controls have been validated, return true
            Return True

        Catch ex As Exception
            Return False 'incase this function throws exception, return false
        End Try
    End Function

    Private Sub btnpost_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnpost.Click
        Dim ErrorAction As New functions
        Try
            'Call a stored procedure to save this record
            Dim MyDBAction As New OVCSystem.functions
            Dim srtClientid As Integer = 0
            Dim strExitReason As String = "0"
            Dim strART As String = "0"
            Dim strfacility As String = "0"
            Dim strschool As String = "0"
            Dim strclass As String = "0"
            Dim strCriteria As String = "0"
            Dim strbcertnumber As String = "0"
            Dim IsDisabled As Boolean = False
            Dim NCPWDNumber As String = ""
            Dim CCCNumber As String = ""

            'validate controls to make sure they have content
            Dim IsValidated As Boolean
            IsValidated = validatecontrols()
            If IsValidated = False Then
                Exit Sub
            End If

            'confirm if client is being exit
            If chkExit.Checked = True Then
                Dim confirm As Integer
                confirm = MsgBox("Confirm Client Exit", MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel)
                If confirm <> vbOK Then Exit Sub
            End If

            'Determine whose id is the household id
            If cboCareTaker.Text.ToString = "Father" Then
                'check if caretaker has valid nationalid
                If txtFatherID.Text.Trim.Length = 0 Then
                    MsgBox("Caretaker needs to have a valid National ID number.", MsgBoxStyle.Exclamation)
                    Exit Sub
                Else
                    householdhead_id = father_id
                End If

            ElseIf cboCareTaker.Text.ToString = "Mother" Then
                'check if caretaker has valid nationalid
                If txtMotherID.Text.Trim.Length = 0 Then
                    MsgBox("Caretaker needs to have a valid National ID number.", MsgBoxStyle.Exclamation)
                    Exit Sub
                Else
                    householdhead_id = mother_id
                End If

            ElseIf cboCareTaker.Text.ToString = "Guardian" Then
                'check if caretaker has valid nationalid
                If txtguardianID.Text.Trim.Length = 0 Then
                    MsgBox("Caretaker needs to have a valid National ID number.", MsgBoxStyle.Exclamation)
                    Exit Sub
                Else
                    householdhead_id = guardian_id
                End If

            End If



            If srtClientid = 0 Then
                If cboHideReason.Text.Trim.Length = 0 Then
                    strExitReason = "0"
                Else
                    strExitReason = cboHideReason.SelectedValue.ToString
                End If

                If cboARTStatus.Text.Trim.Length = 0 Then
                    strART = "0"
                Else
                    strART = cboARTStatus.SelectedValue.ToString
                End If


                If Not IsNothing(cboFacility.SelectedValue) Then
                    strfacility = cboFacility.SelectedValue.ToString
                Else
                    strfacility = "0"
                End If

                If Not IsNothing(cboSchoolName.SelectedValue) Then
                    strschool = cboSchoolName.SelectedValue.ToString
                End If

                If Not IsNothing(cboClass.SelectedValue) Then
                    strclass = cboClass.SelectedValue.ToString
                End If


                If chkHasCert.Checked = True Then
                    strbcertnumber = txtbcertnumber.Text.ToString
                Else
                    strbcertnumber = ""
                End If

                If chkOVCDisabled.Checked = True Then
                    NCPWDNumber = txtNCPWDnum.Text.ToString
                Else
                    NCPWDNumber = ""
                End If

                'generate a number to reference and relate criteria chosen[table clientcriteria] and clientdetails table
                Dim Myrandomkey As New Random
                strCriteria = Format(Date.Today, "yyyyMMddhhss") & Myrandomkey.Next(9000)

                'select wht entry mode,normal or longitudinal
                Dim strmode As String = ""
                If lblMode.Text = "[Normal Edit Mode]" Then
                    strmode = "normal"
                Else
                    strmode = "longitudinal"
                End If

                'validate names to avoid duplicate children --12th Sep 2013 [after olmis concerns meeting]
                Dim IsOVCExists As Boolean
                IsOVCExists = OVCExists(RegularExpressions.Regex.Replace(txtfname.Text.ToString, "[^a-zA-Z]", ""),
                                        RegularExpressions.Regex.Replace(txtmname.Text.ToString, "[^a-zA-Z]", ""),
                                        RegularExpressions.Regex.Replace(txtlname.Text.ToString, "[^a-zA-Z]", ""), Format(dtpDateofBirth.Value, "dd-MMM-yyyy"))
                If IsOVCExists = True AndAlso strmode = "normal" Then
                    MsgBox("OVC already exists", MsgBoxStyle.Exclamation)
                    Exit Sub
                End If

                'make [not known] default value for instances where combobox is invisible
                If cboImmunization.Visible = False Or cboImmunization.Text.ToString.Trim.Length = 0 Then
                    cboImmunization.SelectedValue = 4
                End If

                'RegularExpressions.Regex.Replace(txtParentID.Text.ToString, "[^a-zA-Z]", "")


                srtClientid = MyDBAction.AddClient(txtOVC.Text.ToString, RegularExpressions.Regex.Replace(txtfname.Text.ToString, "[^a-zA-Z]", ""),
                RegularExpressions.Regex.Replace(txtmname.Text.ToString, "[^a-zA-Z]", ""), RegularExpressions.Regex.Replace(txtlname.Text.ToString, "[^a-zA-Z]", ""),
                cboGender.Text.ToString, Format(dtpDateofBirth.Value, "dd-MMM-yyyy"), chkHasCert.Checked, strCriteria, cboCBO.SelectedValue.ToString _
                , cboDistrict.SelectedValue.ToString, cboLocation.SelectedValue.ToString, strfacility.ToString, Format(dtpDateofLinkage.Value, "dd-MMM-yyyy"),
                cboImmunization.SelectedValue.ToString, cboCHWName.SelectedValue.ToString, cboSchoolLevel.SelectedValue.ToString, strschool, strclass, cboHIVStatus.SelectedValue.ToString _
                , strART, "0", guardian_id.ToString, father_id.ToString, mother_id.ToString, householdhead_id.ToString, cboCareTaker.SelectedItem, Format(dtpDateofVisit.Value, "dd-MMM-yyyy"),
                chkExit.Checked, Format(dtpDateofRegistration.Value, "dd-MMM-yyyy") _
                , Format(dtpDateofExit.Value, "dd-MMM-yyyy"), strExitReason, CDate(Date.Now.ToString), strsession.ToString, cboschoolingtype.Text.ToString,
                strbcertnumber.ToString, chkOVCDisabled.Checked, txtNCPWDnum.Text.ToString, txtcccnumber.Text.ToString, strmode)

                'save eligibility criteria in separate table since its a multiple selection[which wsnt the case]--
                ' Loop through all the selected items of the listbox and save priorities
                Dim dview As DataRowView
                For Each dview In lstCriteria.CheckedItems
                    'Its now dview.row.item(0) coz we using clienttypeid is field 5
                    'save record
                    Dim mySqlAction As String = "Insert Into ClientCriteria(EligibilityCriteria,Criteria) " &
                    " values('" & strCriteria.ToString & "','" & dview.Row.Item(0).ToString & "')"

                    MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
                Next

                '----------------------
                lnkLongitudinal.Text = "Longitudinal Records:" & showHistoricalLongitudinalCount(txtOVC.Text)

                'fillgrid() 'refresh the grid

                'clear these variables awaiting next client details
                father_id = 0
                mother_id = 0
                guardian_id = 0
                householdhead_id = 0

                Panel1.Enabled = False
                btnNeeds.Enabled = True
                btnHHAssessment.Enabled = True
                btnpost.Enabled = False

                If srtClientid <> 0 Then
                    MsgBox("Client successfully saved.", MsgBoxStyle.Information)
                Else
                    MsgBox("Client NOT successfully saved. Check DB.", MsgBoxStyle.Exclamation)
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
            ErrorAction.WriteToErrorLogFile("clientinfo", "Save", ex.Message) ''---Write error to error log file
            MsgBox("Exception. Client NOT successfully saved.", MsgBoxStyle.Exclamation)
        End Try
    End Sub

    Private Function OVCExists(ByVal firstname As String, ByVal middlename As String, ByVal surname As String, ByVal dateofbirth As String) As Boolean
        Dim MyDBAction As New functions
        Dim MyTable As Data.DataTable

        Dim MySqlAction As String = "Select 1 from clientdetails where firstname = '" & firstname.ToString & "' " &
        " and middlename = '" & middlename.ToString & "' and surname = '" & surname.ToString & "' and dateofbirth = '" & dateofbirth.ToString & "'"

        MyTable = TryCast(MyDBAction.DBAction(MySqlAction, functions.DBActionType.DataTable), Data.DataTable)
        If MyTable.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Sub DataGridView1_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        Dim ErrorAction As New functions
        Try
            Dim K As Integer

            K = e.RowIndex

            'Validation warning to alert if OVC has HH or CSI assessemnt
            If CBool(Me.DataGridView1.Rows(K).Cells(12).Value) = False Then
                MsgBox("Kindly note that this OVC has no CSI Assessment.", MsgBoxStyle.Exclamation)
            End If

            'If CBool(Me.DataGridView1.Rows(K).Cells(13).Value) = False Then
            '    MsgBox("Kindly note that this OVC has no HouseHold Assessment.", MsgBoxStyle.Exclamation)
            'End If

            select_ovc(Me.DataGridView1.Rows(K).Cells(4).Value, e.ColumnIndex)

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "GridCellClick", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Public Sub select_ovc(ByVal strovcid As String, ByVal intcolumnindex As Integer, Optional ByVal field_to_edit As String = "all")
        Try
            'select item to edit or delete
            Dim mysqlaction As String = ""
            Dim MyDBAction As New functions

            Dim MyDatable As New Data.DataTable

            mysqlaction = "select * from clientdetails where ovcid='" & strovcid & "'"
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then
                If MyDatable.Rows.Count > 0 Then

                    clearcontrols() 'first clear previous data from controls

                    txtfname.Text = MyDatable.Rows(0).Item("firstname").ToString
                    txtmname.Text = MyDatable.Rows(0).Item("middlename").ToString
                    txtlname.Text = MyDatable.Rows(0).Item("surname").ToString
                    txtOVC.Text = MyDatable.Rows(0).Item("ovcid").ToString
                    'cboDistrict.SelectedItem = MyDatable.Rows(0).Item("district").ToString
                    cboGender.SelectedItem = MyDatable.Rows(0).Item("gender").ToString
                    chkHasCert.Checked = MyDatable.Rows(0).Item("birthcert").ToString
                    txtbcertnumber.Text = MyDatable.Rows(0).Item("bcertnumber").ToString

                    chkOVCDisabled.Checked = IIf(IsDBNull(MyDatable.Rows(0).Item("isdisabled")), "0", MyDatable.Rows(0).Item("isdisabled"))
                    txtNCPWDnum.Text = MyDatable.Rows(0).Item("ncpwdnumber").ToString

                    'store the BCert for later comparison to avoid canging from HasBcert to HasNoBcert
                    myPreviousBCert = chkHasCert.CheckState

                    dtpDateofBirth.Value = MyDatable.Rows(0).Item("dateofbirth").ToString
                    'cboClienttype.SelectedValue = MyDatable.Rows(0).Item("clienttype").ToString

                    populate_ExistingCriteria(MyDatable.Rows(0).Item("clienttype").ToString)

                    cboDistrict.SelectedValue = MyDatable.Rows(0).Item("district").ToString
                    cboLocation.SelectedValue = MyDatable.Rows(0).Item("Location").ToString
                    cboFacility.SelectedValue = MyDatable.Rows(0).Item("Facility").ToString
                    dtpDateofLinkage.Value = IIf(IsDBNull(MyDatable.Rows(0).Item("dateoflinkage")), Date.Today, MyDatable.Rows(0).Item("dateoflinkage").ToString)
                    cboImmunization.SelectedValue = IIf(Len(MyDatable.Rows(0).Item("Immunization").ToString) = 0, 4, MyDatable.Rows(0).Item("Immunization").ToString)

                    txtcccnumber.Text = MyDatable.Rows(0).Item("cccnumber").ToString

                    cboCBO.SelectedValue = MyDatable.Rows(0).Item("cbo").ToString
                    cboCHWName.SelectedValue = MyDatable.Rows(0).Item("volunteerid").ToString
                    cboSchoolLevel.SelectedValue = MyDatable.Rows(0).Item("schoollevel").ToString

                    If MyDatable.Rows(0).Item("school").ToString <> "" Then
                        cboSchoolName.SelectedValue = MyDatable.Rows(0).Item("school").ToString
                    End If

                    If MyDatable.Rows(0).Item("class").ToString <> "" Then
                        cboClass.SelectedValue = MyDatable.Rows(0).Item("class").ToString
                    End If

                    If MyDatable.Rows(0).Item("schoolingtype").ToString <> "" Then
                        cboschoolingtype.SelectedItem = MyDatable.Rows(0).Item("schoolingtype").ToString
                    End If

                    cboHIVStatus.SelectedValue = MyDatable.Rows(0).Item("hivstatus").ToString

                    'store the HIV status for later comparison to avoid canging from HIV+ to -ve
                    myPreviousHIVStatus = cboHIVStatus.Text

                    cboARTStatus.SelectedValue = MyDatable.Rows(0).Item("artstatus").ToString
                    'cboEligibilityCriteria.SelectedValue = MyDatable.Rows(0).Item("EligibilityCriteria").ToString
                    father_id = MyDatable.Rows(0).Item("fatherid").ToString
                    mother_id = MyDatable.Rows(0).Item("motherid").ToString
                    guardian_id = MyDatable.Rows(0).Item("guardianid").ToString
                    dtpDateofVisit.Value = MyDatable.Rows(0).Item("dateofvisit").ToString
                    chkExit.Checked = MyDatable.Rows(0).Item("exited").ToString
                    dtpDateofExit.Value = MyDatable.Rows(0).Item("dateofexit").ToString
                    dtpDateofRegistration.Value = MyDatable.Rows(0).Item("dateofregistration").ToString
                    cboHideReason.SelectedValue = MyDatable.Rows(0).Item("reasonforexit").ToString
                    cboCareTaker.SelectedItem = MyDatable.Rows(0).Item("caretaker").ToString

                    'pull father/mother and guardian details
                    fetchparent(father_id)
                    fetchparent(mother_id)
                    fetchguardian(guardian_id)
                End If
            End If

            'show longitudinal record count
            'showHistoricalLongitudinalCount()
            lnkLongitudinal.Text = "Longitudinal Records:" & showHistoricalLongitudinalCount(txtOVC.Text)

            'take focus back to the first tab
            TabControl1.SelectedIndex = 0



            'When we do normal select, its different from Longitudinal Mode
            If intcolumnindex <> 10 Then 'if its not longitudinal
                lblMode.Text = "[Normal Edit Mode]"
                lnkNewClient.Text = "Add New Client"
                Panel1.Enabled = True
                btnNeeds.Enabled = True
                btnHHAssessment.Enabled = True
                BtnEdit.Enabled = True
                'btnViralLoad.Enabled = True
            Else 'if its longitudinal mode
                lblMode.Text = "[Longitudinal Edit Mode]"
                lnkNewClient.Text = "Add New Longitudinal Details"
                Panel1.Enabled = False
                btnNeeds.Enabled = False
                btnHHAssessment.Enabled = False
                BtnEdit.Enabled = False
                btnpost.Enabled = True
                'btnViralLoad.Enabled = False
            End If

            'if we check negative, no need for art status
            If LCase(cboHIVStatus.Text) <> "negative" Then
                Label17.Visible = True
                cboARTStatus.Visible = True
            Else
                Label17.Visible = False
                cboARTStatus.Visible = False
            End If

            'if age is more than 5yrs hide immunization
            If txtAge.Text > 5 Then
                cboImmunization.Visible = False
            Else
                cboImmunization.Visible = True
            End If

            'Only enable Viral Load button for positive OVCs on ARV
            If LCase(cboHIVStatus.Text) = "positive" AndAlso Trim(cboARTStatus.Text).Length <> 0 Then
                btnViralLoad.Enabled = True
            Else
                btnViralLoad.Enabled = False
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub populate_ExistingCriteria(ByVal mycriteriaid As String)
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)

        Try

            'loop through domains
            Dim MyDomainName As String = ""
            Dim MyDomainid As String = ""


            Dim MyDataTable As New Data.DataTable
            Dim sqlAction As String = "SELECT * FROM   clientcriteria WHERE eligibilitycriteria = '" & mycriteriaid.ToString & "'"


            Dim MyDBAction As New functions

            Dim dview As DataRowView
            Dim i As Integer
            'MyDataTable = TryCast(MyDBAction.DBAction(sqlAction, DBActionType.DataTable), Data.DataTable)
            Dim cmd As New SqlCommand(sqlAction, conn)
            conn.Open()
            Dim myreader As SqlDataReader = cmd.ExecuteReader()

            'If MyDataTable.Rows.Count > 0 Then
            '    MnuCount = MyDataTable.Rows.Count
            '    For j = 0 To MyDataTable.Rows.Count - 1
            If myreader.HasRows = True Then
                Do While myreader.Read
                    ' Loop through all the  items of the listbox and check/select what had been saved
                    i = 0
                    For Each dview In lstCriteria.Items
                        'Its now dview.row.item(0) coz we using table clientcriteria and criteria is field 1
                        If dview.Row.Item(0).ToString = myreader("criteria").ToString Then
                            lstCriteria.SetItemChecked(i, True) 'check records that exist in Priorityneeds table
                            Exit For
                        End If
                        i = i + 1

                    Next
                Loop
            End If


        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try


    End Sub

    Private Sub fetchparent(ByVal parentid As String)
        Dim ErrorAction As New functions
        Try

            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim mysqlaction As String
            mysqlaction = "select * from ParentDetails where Parentid='" & parentid & "'"
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then
                'if gender of parent is male, then populate father details 
                If MyDatable.Rows(0).Item("gender").ToString.ToLower = "male" Then
                    txtFatherFirstName.Text = MyDatable.Rows(0).Item("firstname").ToString
                    txtFatherMiddlename.Text = MyDatable.Rows(0).Item("middlename").ToString
                    txtFatherLastName.Text = MyDatable.Rows(0).Item("surname").ToString
                    txtFatherID.Text = MyDatable.Rows(0).Item("IDNumber").ToString
                    txtFatherMobile.Text = MyDatable.Rows(0).Item("MobileNumber").ToString
                    cboFatherCauseofDeath.SelectedValue = MyDatable.Rows(0).Item("CauseofDeath").ToString
                    cboFatherHIVStatus.SelectedValue = MyDatable.Rows(0).Item("HIVStatus").ToString

                Else 'else then populate mother details
                    txtMotherFirstName.Text = MyDatable.Rows(0).Item("firstname").ToString
                    txtMotherMiddlename.Text = MyDatable.Rows(0).Item("middlename").ToString
                    txtmotherLastName.Text = MyDatable.Rows(0).Item("surname").ToString
                    txtMotherID.Text = MyDatable.Rows(0).Item("IDNumber").ToString
                    txtMotherMobile.Text = MyDatable.Rows(0).Item("MobileNumber").ToString
                    cboMotherCauseofDeath.SelectedValue = MyDatable.Rows(0).Item("CauseofDeath").ToString
                    cboMotherHIVStatus.SelectedValue = MyDatable.Rows(0).Item("HIVStatus").ToString

                End If



            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "GridCellClick", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub fetchguardian(ByVal guardianid As String)
        Dim ErrorAction As New functions
        Try

            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim mysqlaction As String
            mysqlaction = "select * from ParentDetails where Parentid='" & guardianid & "'"
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then

                txtguardianID.Text = MyDatable.Rows(0).Item("Parentid").ToString
                txtguardianFirstname.Text = MyDatable.Rows(0).Item("firstname").ToString
                txtguardianMiddlename.Text = MyDatable.Rows(0).Item("middlename").ToString
                txtguardianLastname.Text = MyDatable.Rows(0).Item("surname").ToString
                txtguardianID.Text = MyDatable.Rows(0).Item("IDNumber").ToString
                txtguardianMobile.Text = MyDatable.Rows(0).Item("MobileNumber").ToString
                cboguardianGender.SelectedItem = MyDatable.Rows(0).Item("Gender").ToString
                cboRelationship.SelectedValue = MyDatable.Rows(0).Item("relationship").ToString

            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "GridCellClick", ex.Message) ''---Write error to error log file

        End Try
    End Sub


    Private Sub cmdsaveservice_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try

            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions

            'Dim o As Object
            ' For Each o In chkindicators.CheckedItems
            '     mySqlAction = "Insert into clientservice( clientid, indicator) " & _
            '" values(" & txtclientid.Text.ToString & ",'" & o.ToString & "')"

            '     MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
            ' Next o
            MsgBox("Record saved successfully.", MsgBoxStyle.Information)
        Catch ex As Exception

        End Try
    End Sub

    Private m_ChildFormNumber As Integer


    Private Sub txtOVC_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtOVC.GotFocus
        'autogenerate OVCID number by combining  DOB - Currenttime
        txtOVC.Text = Format(dtpDateofBirth.Value, "yyyyMMdd") & "-" & Format(Date.Now, "yyyyMMddhhmmss")
    End Sub


    Private Sub BtnEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnEdit.Click
        Dim ErrorAction As New functions
        Try
            'Call a stored procedure to save this record
            Dim MyDBAction As New OVCSystem.functions
            Dim strExitReason As String = "0"
            Dim strART As String = "0"
            Dim strFacility As String = "0"
            Dim strschool As String = "0"
            Dim strclass As String = "0"
            Dim strCriteria As String = "0"
            Dim strbcertnumber As String = "0"
            Dim IsDisabled As Boolean = False
            Dim NCPWDNumber As String = ""
            Dim CCCNumber As String = ""


            'validate controls to make sure they have content
            Dim IsValidated As Boolean
            IsValidated = validatecontrols()
            If IsValidated = False Then
                Exit Sub
            End If

            'make sure that status is not changing from HIV+ve to something else. Only selected personell are allowed to do this incase of an error during data entry
            If candelete = False Then
                If myPreviousHIVStatus.ToUpper = "POSITIVE" AndAlso cboHIVStatus.Text.ToUpper <> myPreviousHIVStatus.ToUpper Then
                    ErrorProvider1.SetError(cboHIVStatus, "Please select correct status")
                    MsgBox("You are not authorised to reverse HIV status from +ve to something else.", vbExclamation)
                    cboHIVStatus.Focus()
                    Exit Sub
                End If
            End If


            'make sure that Bcert is not changing from HasBcert to NoBcert
            If myPreviousBCert = True AndAlso chkHasCert.Checked = False Then
                ErrorProvider1.SetError(cboHIVStatus, "Please select correct BCert status")
                MsgBox("OVC already had Birth Certificate.", vbExclamation)
                chkHasCert.Focus()
                Exit Sub
            End If

            'confirm if client is being exit
            If chkExit.Checked = True Then
                Dim confirm As Integer
                confirm = MsgBox("Confirm Client Exit", MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel)
                If confirm <> vbOK Then Exit Sub
            End If



            If cboHideReason.Text.Trim.Length = 0 Then
                strExitReason = "0"
            Else
                strExitReason = cboHideReason.SelectedValue.ToString
            End If

            If cboARTStatus.Text.Trim.Length = 0 Then
                strART = "0"
            Else
                strART = cboARTStatus.SelectedValue.ToString
            End If

            If Not IsNothing(cboFacility.SelectedValue) Then
                strFacility = cboFacility.SelectedValue.ToString
            End If

            If Not IsNothing(cboSchoolName.SelectedValue) Then
                strschool = cboSchoolName.SelectedValue.ToString
            End If

            If Not IsNothing(cboClass.SelectedValue) Then
                strclass = cboClass.SelectedValue.ToString
            End If

            If chkHasCert.Checked = True Then
                strbcertnumber = txtbcertnumber.Text.ToString
            Else
                strbcertnumber = ""
            End If

            If chkOVCDisabled.Checked = True Then
                NCPWDNumber = txtNCPWDnum.Text.ToString
            Else
                NCPWDNumber = ""
            End If

            'generate a number to reference and relate criteria chosen[table clientcriteria] and clientdetails table
            Dim Myrandomkey As New Random
            strCriteria = Format(Date.Today, "yyyyMMddmmss") & Myrandomkey.Next(9000)

            'Determine whose id is the household id
            If cboCareTaker.Text.ToString = "Father" Then
                householdhead_id = father_id
            ElseIf cboCareTaker.Text.ToString = "Mother" Then
                householdhead_id = mother_id
            ElseIf cboCareTaker.Text.ToString = "Guardian" Then
                householdhead_id = guardian_id
            End If

            'RegularExpressions.Regex.Replace(txtParentID.Text.ToString, "[^a-zA-Z]", "")

            'select wht entry mode,normal or longitudinal
            Dim strmode As String = ""
            If lblMode.Text = "[Normal Edit Mode]" Then
                strmode = "normal"
                MyDBAction.UpdateClient(txtOVC.Text.ToString, RegularExpressions.Regex.Replace(txtfname.Text.ToString, "[^a-zA-Z]", ""),
                 RegularExpressions.Regex.Replace(txtmname.Text.ToString, "[^a-zA-Z]", ""), RegularExpressions.Regex.Replace(txtlname.Text.ToString, "[^a-zA-Z]", ""),
                cboGender.Text.ToString, Format(dtpDateofBirth.Value, "dd-MMM-yyyy"), chkHasCert.Checked, strCriteria, cboCBO.SelectedValue.ToString _
                , cboDistrict.SelectedValue.ToString, cboLocation.SelectedValue.ToString, strFacility.ToString, Format(dtpDateofLinkage.Value, "dd-MMM-yyyy"), cboImmunization.SelectedValue.ToString, cboCHWName.SelectedValue.ToString, cboSchoolLevel.SelectedValue.ToString, strschool, strclass, cboHIVStatus.SelectedValue.ToString _
                , strART, "0", guardian_id.ToString, father_id.ToString, mother_id.ToString, householdhead_id.ToString, cboCareTaker.SelectedItem, Format(dtpDateofVisit.Value, "dd-MMM-yyyy"), chkExit.Checked, Format(dtpDateofRegistration.Value, "dd-MMM-yyyy") _
                , Format(dtpDateofExit.Value, "dd-MMM-yyyy"), strExitReason, CDate(Date.Now.ToString), strsession.ToString, cboschoolingtype.Text.ToString,
                   strbcertnumber.ToString, chkOVCDisabled.Checked, txtNCPWDnum.Text.ToString, txtcccnumber.Text.ToString, strmode)
                'ElseIf lblMode.Text = "[Longitudinal Edit Mode]" Then
                '    strmode = "longitudinal"
                '    MyDBAction.UpdateClient(txtOVC.Text.ToString, txtfname.Text.ToString, txtmname.Text.ToString, txtlname.Text.ToString,
                '    cboGender.Text.ToString, Format(dtpDateofBirth.Value, "dd-MMM-yyyy"), chkHasCert.Checked, strCriteria, cboCBO.SelectedValue.ToString _
                '    , cboDistrict.SelectedValue.ToString, cboLocation.SelectedValue.ToString, strFacility.ToString, Format(dtpDateofLinkage.Value, "dd-MMM-yyyy"), cboImmunization.SelectedValue.ToString, cboCHWName.SelectedValue.ToString, cboSchoolLevel.SelectedValue.ToString, strschool, strclass, cboHIVStatus.SelectedValue.ToString _
                '    , strART, "0", guardian_id.ToString, father_id.ToString, mother_id.ToString, householdhead_id.ToString, cboCareTaker.SelectedItem, Format(dtpDateofVisit.Value, "dd-MMM-yyyy"), chkExit.Checked, Format(dtpDateofRegistration.Value, "dd-MMM-yyyy") _
                '    , Format(dtpDateofExit.Value, "dd-MMM-yyyy"), strExitReason, CDate(Date.Now.ToString), strsession.ToString, strmode, intclientlongid)
                '    intclientlongid = 0 'reinitialize this variable
            End If


            'save eligibility criteria in separate table since its a multiple selection[which wsnt the case]--
            ' Loop through all the selected items of the listbox and save priorities
            Dim dview As DataRowView
            For Each dview In lstCriteria.CheckedItems
                'Its now dview.row.item(0) coz we using clienttypeid is field 5
                'save record
                Dim mySqlAction As String = "Insert Into ClientCriteria(EligibilityCriteria,Criteria) " &
                " values('" & strCriteria.ToString & "','" & dview.Row.Item(0).ToString & "')"

                MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
            Next

            '----------------------
            lnkLongitudinal.Text = "Longitudinal Records:" & showHistoricalLongitudinalCount(txtOVC.Text)
            'fillgrid() 'refresh the grid

            'clear these variables awaiting next client details
            father_id = 0
            mother_id = 0
            guardian_id = 0
            householdhead_id = 0

            Panel1.Enabled = False
            btnNeeds.Enabled = False
            BtnEdit.Enabled = False

            MsgBox("Client successfully updated.", MsgBoxStyle.Information)

        Catch ex As Exception
            MsgBox("Client NOT successfully saved.", MsgBoxStyle.Exclamation)
            ErrorAction.WriteToErrorLogFile("clientinfo", "Save", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub dtpDateofBirth_LostFocus(sender As Object, e As EventArgs) Handles dtpDateofBirth.LostFocus

        'alert if someone is over18yrs, but can still continue to save
        If DateDiff(DateInterval.Year, dtpDateofBirth.Value, Date.Today) >= 18 Then
            MsgBox("Please Note that OVC is 18 years or more.", MsgBoxStyle.Exclamation + MsgBoxStyle.OkOnly, Me.Text)
        End If
        If DateDiff(DateInterval.Year, dtpDateofBirth.Value, Date.Today) > 5 Then
            cboImmunization.Visible = False
        Else
            cboImmunization.Visible = True
        End If


    End Sub

    Private Sub dtpDateofBirth_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpDateofBirth.ValueChanged
        'calculate age automatically
        txtAge.Text = DateDiff(DateInterval.Year, dtpDateofBirth.Value, Date.Today)
    End Sub

    Private Sub lnkNewClient_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkNewClient.LinkClicked


        'select wht entry mode,normal or longitudinal
        Dim strmode As String = ""
        If lblMode.Text = "[Normal Edit Mode]" Then
            'clear all controls ready for a new client
            father_id = 0
            mother_id = 0
            guardian_id = 0
            householdhead_id = 0

            Panel1.Enabled = True
            btnNeeds.Enabled = False
            btnHHAssessment.Enabled = False
            btnpost.Enabled = True
            txtfname.Focus()
            clearcontrols()
            txtOVC.Enabled = True
        ElseIf lblMode.Text = "[Longitudinal Edit Mode]" Then
            txtOVC.Enabled = False
            Panel1.Enabled = True
            btnNeeds.Enabled = False
            btnHHAssessment.Enabled = False
            btnpost.Enabled = True
            BtnEdit.Enabled = False

            'based on which longitudinal modules
            If Longitudinal_bcert = True Then
                chkHasCert.Checked = True
                ErrorProvider1.SetError(chkHasCert, "Please Confirm B-Cert Status to continue")
            End If
            If Longitudinal_hivstatus = True Then
                Dim MyDBAction As New functions
                Dim SqlAction As String = "SELECT hivstatusid from hivstatus where hivstatus = '" &
                AppSettings("hiv_Counseling_longitudinalSelection").ToString & "'"

                cboHIVStatus.SelectedValue = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

                ErrorProvider1.SetError(cboHIVStatus, "Please Confirm HIV Status to continue")
            End If
        End If



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
            ' Loop through all the selected items of the listbox and save priorities
            'For Each item As ListViewItem In lstCriteria.Items
            '    item.Checked = False
            'Next
            Dim myEnumerator As IEnumerator
            myEnumerator = lstCriteria.CheckedIndices.GetEnumerator()
            Dim y As Integer
            While myEnumerator.MoveNext() <> False
                y = CInt(myEnumerator.Current)
                lstCriteria.SetItemChecked(y, False)
            End While
        Catch ex As Exception

        End Try
    End Sub

    'Private Sub disablecontrols()
    '    Try
    '        'to clear textBoxes,radiobutton and checkboxes
    '        ' that are in containers 
    '        Dim ctrl As Control = Me.GetNextControl(Me, True)
    '        Do Until ctrl Is Nothing
    '            If TypeOf ctrl Is TextBox Then
    '                ctrl.Enabled = False
    '            ElseIf TypeOf ctrl Is RadioButton Then
    '                ctrl.Enabled = False
    '            ElseIf TypeOf ctrl Is CheckBox Then
    '                ctrl.Enabled = False
    '            ElseIf TypeOf ctrl Is ComboBox Then
    '                ctrl.Enabled = False
    '            ElseIf TypeOf ctrl Is DateTimePicker Then
    '                ctrl.Enabled = False
    '            ElseIf TypeOf ctrl Is MaskedTextBox Then
    '                ctrl.Enabled = False
    '            End If
    '        Loop

    '    Catch ex As Exception

    '    End Try
    'End Sub



    Private Sub chkExit_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkExit.CheckedChanged
        If chkExit.Checked = True Then
            'make visible exit controls
            Label39.Visible = True
            Label41.Visible = True
            cboHideReason.Visible = True
            dtpDateofExit.Visible = True
        Else
            'make visible exit controls
            Label39.Visible = False
            Label41.Visible = False
            cboHideReason.Visible = False
            dtpDateofExit.Visible = False
        End If

    End Sub


    Private Sub cboHIVStatus_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboHIVStatus.SelectedIndexChanged
        'if we check negative, no need for art status
        If LCase(cboHIVStatus.Text) = "positive" Then
            Label17.Visible = True
            cboARTStatus.Visible = True


            Label44.Visible = True
            cboFacility.Visible = True

            Label56.Visible = True
            dtpDateofLinkage.Visible = True

            Label59.Visible = True
            txtcccnumber.Visible = True
        Else
            cboARTStatus.SelectedIndex = -1
            Label17.Visible = False
            cboARTStatus.Visible = False

            cboFacility.SelectedIndex = -1
            Label44.Visible = False
            cboFacility.Visible = False

            Label56.Visible = False
            dtpDateofLinkage.Visible = False

            Label59.Visible = False
            txtcccnumber.Visible = False
        End If
    End Sub

    Private Sub cboCareTaker_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboCareTaker.KeyPress
        e.Handled = True ' true means the keypress is suppressed
    End Sub


    'Private Sub cboClienttype_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboClienttype.SelectedIndexChanged
    '    If LCase(cboClienttype.Text) = LCase("father dead") Then
    '        'if father/mother dead,dont allow to add father details
    '        btnAddFather.Enabled = False
    '        btnSearchFather.Enabled = False
    '    ElseIf LCase(cboClienttype.Text) = LCase("mother dead") Then
    '        btnAddMother.Enabled = False
    '        btnSearchMother.Enabled = False
    '    Else
    '        btnAddFather.Enabled = True
    '        btnSearchFather.Enabled = True
    '        btnAddMother.Enabled = True
    '        btnSearchMother.Enabled = True
    '    End If
    'End Sub

    Private Sub cboCareTaker_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboCareTaker.SelectedIndexChanged
        'confirm that caretaker exists
        If cboCareTaker.Text = "Father" AndAlso father_id = "0" Then
            MsgBox("Please choose an existing care taker.", MsgBoxStyle.Exclamation)
            cboCareTaker.SelectedIndex = -1
        ElseIf cboCareTaker.Text = "Mother" AndAlso mother_id = "0" Then
            MsgBox("Please choose an existing care taker.", MsgBoxStyle.Exclamation)
            cboCareTaker.SelectedIndex = -1
        ElseIf cboCareTaker.Text = "Guardian" AndAlso guardian_id = "0" Then
            MsgBox("Please choose an existing care taker.", MsgBoxStyle.Exclamation)
            cboCareTaker.SelectedIndex = -1
        End If
    End Sub

    Private Sub cboDistrict_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboDistrict.KeyPress
        e.Handled = True ' true means the keypress is suppressed
    End Sub

    Private Sub cboDistrict_SelectedValueChanged1(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboDistrict.SelectedValueChanged
        Try
            'If cboDistrict.Text.Trim.Length <> 0 Or cboDistrict.Text <> "System.Data.DataRowView" Then
            If IsNumeric(cboDistrict.SelectedValue) = True Then
                populatelocations(cboDistrict.SelectedValue.ToString)
                populateCBO(cboDistrict.SelectedValue.ToString)

            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub cboCBO_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboCBO.KeyPress
        e.Handled = True ' true means the keypress is suppressed
    End Sub


    Private Sub cboCBO_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboCBO.SelectedValueChanged
        Try
            If IsNumeric(cboCBO.SelectedValue) Then
                populateCHW(cboCBO.SelectedValue.ToString)
            End If

        Catch ex As Exception

        End Try

    End Sub

    Private Sub BtnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnExit.Click
        Me.Close()
    End Sub

    Private Sub lnkLongitudinal_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkLongitudinal.LinkClicked
        filllongitudinalgrid()
        TabControl1.SelectedIndex = 2
    End Sub

    Private Sub DataGridView2_CellContentClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView2.CellContentClick
        Dim ErrorAction As New functions
        Try

            'select item to edit or delete
            Dim mysqlaction As String = ""
            Dim MyDBAction As New functions
            Dim K As Integer

            K = e.RowIndex
            Dim MyDatable As New Data.DataTable

            mysqlaction = "select * from clientlongitudinaldetails where clientlongid=" & Me.DataGridView2.Rows(K).Cells(0).Value & ""
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then
                If MyDatable.Rows.Count > 0 Then

                    clearcontrols() 'first clear previous data from controls

                    intclientlongid = MyDatable.Rows(0).Item("clientlongid").ToString
                    txtfname.Text = MyDatable.Rows(0).Item("firstname").ToString
                    txtmname.Text = MyDatable.Rows(0).Item("middlename").ToString
                    txtlname.Text = MyDatable.Rows(0).Item("surname").ToString
                    txtOVC.Text = MyDatable.Rows(0).Item("ovcid").ToString
                    'cboDistrict.SelectedItem = MyDatable.Rows(0).Item("district").ToString
                    cboGender.SelectedItem = MyDatable.Rows(0).Item("gender").ToString
                    chkHasCert.Checked = MyDatable.Rows(0).Item("birthcert").ToString
                    dtpDateofBirth.Value = MyDatable.Rows(0).Item("dateofbirth").ToString
                    'cboClienttype.SelectedValue = MyDatable.Rows(0).Item("clienttype").ToString
                    cboDistrict.SelectedValue = MyDatable.Rows(0).Item("district").ToString
                    cboLocation.SelectedValue = MyDatable.Rows(0).Item("Location").ToString
                    cboFacility.SelectedValue = MyDatable.Rows(0).Item("Facility").ToString
                    cboCBO.SelectedValue = MyDatable.Rows(0).Item("cbo").ToString
                    cboCHWName.SelectedValue = MyDatable.Rows(0).Item("volunteerid").ToString
                    cboSchoolLevel.SelectedValue = MyDatable.Rows(0).Item("schoollevel").ToString
                    cboHIVStatus.SelectedValue = MyDatable.Rows(0).Item("hivstatus").ToString
                    cboARTStatus.SelectedValue = MyDatable.Rows(0).Item("artstatus").ToString
                    father_id = MyDatable.Rows(0).Item("fatherid").ToString
                    mother_id = MyDatable.Rows(0).Item("motherid").ToString
                    guardian_id = MyDatable.Rows(0).Item("guardianid").ToString
                    dtpDateofVisit.Value = MyDatable.Rows(0).Item("dateofvisit").ToString
                    chkExit.Checked = MyDatable.Rows(0).Item("exited").ToString
                    dtpDateofExit.Value = MyDatable.Rows(0).Item("dateofexit").ToString
                    dtpDateofRegistration.Value = MyDatable.Rows(0).Item("dateofregistration").ToString
                    cboHideReason.SelectedValue = MyDatable.Rows(0).Item("reasonforexit").ToString
                    cboCareTaker.SelectedItem = MyDatable.Rows(0).Item("caretaker").ToString

                    'pull father/mother and guardian details
                    fetchparent(father_id)
                    fetchparent(mother_id)
                    fetchguardian(guardian_id)
                End If
            End If

            'show longitudinal record count
            'showHistoricalLongitudinalCount()

            lnkLongitudinal.Text = "Longitudinal Records:" & showHistoricalLongitudinalCount(txtOVC.Text)

            'take focus back to the first tab
            TabControl1.SelectedIndex = 0



            'When we do normal select, its different from Longitudinal Mode
            lblMode.Text = "[Longitudinal Edit Mode]"
            lnkNewClient.Text = "Add New Longitudinal Details"
            Panel1.Enabled = True
            btnNeeds.Enabled = False
            BtnEdit.Enabled = True
            btnpost.Enabled = False


            'if we check negative, no need for art status
            If LCase(cboHIVStatus.Text) <> "negative" Then
                Label17.Visible = True
                cboARTStatus.Visible = True
            Else
                Label17.Visible = False
                cboARTStatus.Visible = False
            End If


        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("clientinfo", "Grid2CellClick", ex.Message) ''---Write error to error log file

        End Try
    End Sub


    Private Sub cbosearchdistrict_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cbosearchdistrict.KeyPress
        e.Handled = True ' true means the keypress is suppressed
    End Sub

    Private Sub cbosearchdistrict_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbosearchdistrict.SelectedValueChanged
        Try
            'If cboDistrict.Text.Trim.Length <> 0 Or cboDistrict.Text <> "System.Data.DataRowView" Then
            If IsNumeric(cbosearchdistrict.SelectedValue) = True Then

                populatesearchCBO(cbosearchdistrict.SelectedValue.ToString)

            End If

        Catch ex As Exception

        End Try
    End Sub


    Private Sub cboGender_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboGender.KeyPress

        e.Handled = True ' true means the keypress is suppressed



    End Sub

    Private Sub cboARTStatus_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboARTStatus.KeyPress
        e.Handled = True ' true means the keypress is suppressed
    End Sub

    Private Sub cboCHWName_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboCHWName.KeyPress
        e.Handled = True ' true means the keypress is suppressed
    End Sub

    Private Sub cboClienttype_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        e.Handled = True ' true means the keypress is suppressed
    End Sub

    Private Sub cboFacility_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboFacility.KeyPress
        e.Handled = True ' true means the keypress is suppressed
    End Sub

    Private Sub cboMotherCauseofDeath_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboMotherCauseofDeath.KeyPress
        e.Handled = True ' true means the keypress is suppressed
    End Sub

    Private Sub cboMotherHIVStatus_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboMotherHIVStatus.KeyPress
        e.Handled = True ' true means the keypress is suppressed
    End Sub

    Private Sub cboRelationship_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboRelationship.KeyPress
        e.Handled = True ' true means the keypress is suppressed
    End Sub

    Private Sub cboSchoolLevel_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboSchoolLevel.KeyPress
        e.Handled = True ' true means the keypress is suppressed
    End Sub

    Private Sub cbosearchcbo_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cbosearchcbo.KeyPress
        e.Handled = True ' true means the keypress is suppressed
    End Sub


    Private Sub TabControl1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabControl1.SelectedIndexChanged
        'set the active buttons during runtime
        If TabControl1.SelectedIndex = 0 Then
            Me.AcceptButton = btnpost
        ElseIf TabControl1.SelectedIndex = 1 Then
            Me.AcceptButton = btnSearch
        ElseIf TabControl1.SelectedIndex = 2 Then
        End If
    End Sub

    Private Sub cboDistrict_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboDistrict.SelectedIndexChanged

    End Sub

    Private Sub cboSchoolName_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboSchoolName.KeyPress
        e.Handled = True ' true means the keypress is suppressed
    End Sub

    Private Sub cboClass_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles cboClass.KeyPress
        e.Handled = True ' true means the keypress is suppressed
    End Sub

    Private Sub cboEligibilityCriteria_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs)
        e.Handled = True ' true means the keypress is suppressed
    End Sub

    Private Sub cboSchoolLevel_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboSchoolLevel.SelectedValueChanged
        Try
            'If cboDistrict.Text.Trim.Length <> 0 Or cboDistrict.Text <> "System.Data.DataRowView" Then
            If IsNumeric(cboSchoolLevel.SelectedValue) = True Then
                populateSchools(cboSchoolLevel.SelectedValue.ToString)
                populateclass(cboSchoolLevel.SelectedValue.ToString)

            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Try

            Dim confirm As DialogResult
            'Displays the MessageBox
            confirm = MsgBox("Confirm Deletion of OVC data.", MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel)

            ' Gets the result of the MessageBox display.
            If confirm = DialogResult.OK Then
                Dim myConnection As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
                myConnection.Open()
                Dim myCommand As New SqlCommand("DeleteOVCData", myConnection)

                With myCommand
                    .CommandType = Data.CommandType.StoredProcedure
                    .CommandTimeout = 300
                    .Parameters.Add(New SqlParameter("@OVCID", txtOVC.Text.ToString))
                    .ExecuteNonQuery()

                End With

                myConnection.Close()
                MsgBox("OVC deleted successfully.", MsgBoxStyle.Information)
            End If

        Catch ex As Exception
            MsgBox(ex.Message & " [delete ovc]")
        End Try
    End Sub


    Private Sub dtpDateofRegistration_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpDateofRegistration.ValueChanged
        ' ''date of registration should never be more then 2 mnths ago
        ''If DateDiff(DateInterval.Month, dtpDateofRegistration.Value, Date.Today) > 2 Then
        ''    ErrorProvider1.SetError(dtpDateofRegistration, "Please select [Date of Registration] to continue")
        ''    MsgBox("Registration cannot be more than 2 months ago.", MsgBoxStyle.Exclamation, Me.Text)
        ''    dtpDateofRegistration.Focus()
        ''    Exit Sub
        ''End If
        ' ''date of registration should never be a future date
        ''If dtpDateofRegistration.Value > Date.Today Then
        ''    ErrorProvider1.SetError(dtpDateofRegistration, "Please select [Date of Registration] to continue")
        ''    MsgBox("Registration cannot be a future date.", MsgBoxStyle.Exclamation, Me.Text)
        ''    dtpDateofRegistration.Focus()
        ''    Exit Sub
        ''End If

        ' ''date of registration for previous month should be done by 10th of the next month
        ''If DateDiff(DateInterval.Month, dtpDateofRegistration.Value, Date.Today) > 0 _
        ''AndAlso Format(Date.Today, "dd") > 10 Then
        ''    ErrorProvider1.SetError(dtpDateofRegistration, "Please select [Date of Registration] to continue")
        ''    MsgBox("Previous month's registrations should be done by 10th of the next month.", MsgBoxStyle.Exclamation, Me.Text)
        ''    dtpDateofRegistration.Focus()
        ''    Exit Sub
        ''End If
    End Sub





    Private Sub txtOVC_TextChanged(sender As Object, e As EventArgs) Handles txtOVC.TextChanged

    End Sub

    Private Sub chkHasCert_CheckedChanged(sender As Object, e As EventArgs) Handles chkHasCert.CheckedChanged
        Try
            If chkHasCert.Checked = True Then
                Label57.Enabled = True
                txtbcertnumber.Enabled = True
            Else
                Label57.Enabled = False
                txtbcertnumber.Enabled = False
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub chkOVCDisabled_CheckedChanged(sender As Object, e As EventArgs) Handles chkOVCDisabled.CheckedChanged
        Try
            If chkOVCDisabled.Checked = True Then
                Label58.Enabled = True
                txtNCPWDnum.Enabled = True
            Else
                Label58.Enabled = False
                txtNCPWDnum.Enabled = False
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub cboLocation_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboLocation.SelectedIndexChanged

    End Sub


End Class
