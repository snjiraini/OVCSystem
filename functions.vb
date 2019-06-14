Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Data.Odbc
Imports System.Configuration.ConfigurationManager
Imports System.IO
Imports System.Text
Imports System.Security.Cryptography
Imports System.Text.RegularExpressions
Imports System.Xml
Imports Ionic.Zip
Imports Ionic.Zlib
Imports Ionic.Crc



Public Class AppSecurity

    Protected key As String = "*&/>:@>?"

    Public Sub New()
        'constructor
    End Sub

    Public Function encryptString(ByVal strtext As String) As String
        Return Encrypt(strtext, key)
    End Function

    Public Function decryptString(ByVal strtext As String) As String
        Return Decrypt(strtext, key)
    End Function

    'The function used to encrypt the text
    Private Function Encrypt(ByVal strText As String, ByVal strEncrKey _
             As String) As String
        Dim byKey() As Byte = {}
        Dim IV() As Byte = {&H12, &H34, &H56, &H78, &H90, &HAB, &HCD, &HEF}

        Try
            byKey = System.Text.Encoding.UTF8.GetBytes(Left(strEncrKey, 8))

            Dim des As New DESCryptoServiceProvider()
            Dim inputByteArray() As Byte = Encoding.UTF8.GetBytes(strText)
            Dim ms As New MemoryStream()
            Dim cs As New CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write)
            cs.Write(inputByteArray, 0, inputByteArray.Length)
            cs.FlushFinalBlock()
            Return Convert.ToBase64String(ms.ToArray())

        Catch ex As Exception
            Return ex.Message
        End Try

    End Function

    'The function used to decrypt the text
    Private Function Decrypt(ByVal strText As String, ByVal sDecrKey _
               As String) As String
        Dim byKey() As Byte = {}
        Dim IV() As Byte = {&H12, &H34, &H56, &H78, &H90, &HAB, &HCD, &HEF}
        Dim inputByteArray(strText.Length) As Byte

        Try
            byKey = System.Text.Encoding.UTF8.GetBytes(Left(sDecrKey, 8))
            Dim des As New DESCryptoServiceProvider()
            inputByteArray = Convert.FromBase64String(strText)
            Dim ms As New MemoryStream()
            Dim cs As New CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write)

            cs.Write(inputByteArray, 0, inputByteArray.Length)
            cs.FlushFinalBlock()
            Dim encoding As System.Text.Encoding = System.Text.Encoding.UTF8

            Return encoding.GetString(ms.ToArray())

        Catch ex As Exception
            Return ex.Message
        End Try

    End Function
End Class

Public Class functions

    Public Enum DBActionType
        DataReader = 1
        Dataset = 2
        Scalar = 3
        Delete = 4
        Insert = 5
        Update = 6
        DataTable = 7
        Execute = 8

    End Enum
    Public Function DBAction(ByVal MyDBAction As Object, ByVal ActionType As DBActionType) As Object
        DBAction = Nothing
        Dim MyConnection As String = ConnectionStrings(SelectedConnectionString).ToString


        Dim MyODBCConnection As New SqlConnection(MyConnection)
        If MyODBCConnection.State <> Data.ConnectionState.Closed Then MyODBCConnection.Close()

        Try

            MyODBCConnection.Open()



            Select Case ActionType
                Case 1
                    Dim myCommand As New SqlCommand(MyDBAction.ToString, MyODBCConnection)
                    myCommand.CommandTimeout = 3600 'this will timeout if it takes 1hr to execute
                    Return myCommand.ExecuteReader

                Case 2
                    Dim MyAdpater As New SqlDataAdapter(MyDBAction.ToString, MyODBCConnection)
                    Dim myDataSet As New Data.DataSet
                    MyAdpater.SelectCommand.CommandTimeout = 3600
                    MyAdpater.Fill(myDataSet)

                    MyODBCConnection.Close()

                    Return myDataSet
                Case 3
                    Dim myCommand As New SqlCommand(MyDBAction.ToString, MyODBCConnection)
                    myCommand.CommandTimeout = 3600 'this will timeout if it takes 1hr to execute
                    Dim ScalarItem As Object = myCommand.ExecuteScalar

                    MyODBCConnection.Close()

                    If IsDBNull(ScalarItem) Then
                        ScalarItem = ""
                    End If
                    Return ScalarItem

                Case 4, 5, 6, 8
                    Dim myCommand As New SqlCommand(MyDBAction.ToString, MyODBCConnection)
                    myCommand.CommandTimeout = 600000 'this will timeout if it takes 1hr to execute
                    myCommand.ExecuteNonQuery()
                    MyODBCConnection.Close()

                Case 7
                    Dim MyAdpater As New SqlDataAdapter(MyDBAction.ToString, MyODBCConnection)
                    MyAdpater.SelectCommand.CommandTimeout = 3600
                    Dim MyDataTable As New Data.DataTable
                    MyAdpater.Fill(MyDataTable)
                    MyODBCConnection.Close()

                    Return MyDataTable




            End Select
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally

            If MyODBCConnection.State <> Data.ConnectionState.Closed Then
                MyODBCConnection.Close()

                MyODBCConnection.Dispose()
            End If
        End Try

    End Function

    Public Function ConfirmFK_Relationships(ByVal tablename As String, ByVal primarykey As String) As Boolean
        Try
            Dim recordcount As Int64 = 0
            Dim recordcount_cummulative As Int64 = 0

            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions

            Select Case tablename
                Case "county"
                    mySqlAction = "SELECT        count(County.CountyID)" &
                        " FROM            County INNER JOIN " &
                         " District ON County.CountyID = District.Countyid" &
                         "where County.CountyID = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    mySqlAction = "SELECT        count(County.CountyID)" &
                            " FROM            County INNER JOIN " &
                         " Wards On County.CountyID = Wards.Countyid " &
                         "where County.CountyID = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    If recordcount_cummulative > 0 Then Return True

                Case "district"
                    mySqlAction = " SELECT        count(District.DistrictID) " &
                    " FROM            District INNER JOIN " &
                                             " CBO ON District.DistrictID = CBO.DistrictID " &
                                             "where district.districtID = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    mySqlAction = " SELECT        count(District.DistrictID) " &
                    " FROM            District INNER JOIN " &
                                             " Location ON District.DistrictID = Location.DistrictID " &
                                        "where district.districtID = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    mySqlAction = " SELECT        count(District.DistrictID) " &
                    " FROM            District INNER JOIN " &
                                             " Clientdetails ON District.DistrictID = Clientdetails.District " &
                                             "where County.CountyID = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    If recordcount_cummulative > 0 Then Return True
                Case "cbo"

                    mySqlAction = " SELECT        count(CBO.CBOID) " &
                    " FROM            CBO INNER JOIN " &
                                            "  CHW ON CBO.CBOID = CHW.CBOID " &
                                                               "where cbo.cboid = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    mySqlAction = " SELECT        count(CBO.CBOID) " &
                    " FROM            CBO INNER JOIN " &
                                             " Clientdetails ON CBO.CBOID = Clientdetails.Cbo " &
                                                               "where cbo.cboid = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    If recordcount_cummulative > 0 Then Return True

                Case "location"
                    mySqlAction = " SELECT        count(Location.LocationID) " &
                    " FROM            Location INNER JOIN " &
                                             " Clientdetails ON Location.LocationID = Clientdetails.Location " &
                                                                                  "where location.locationid = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    If recordcount_cummulative > 0 Then Return True
                Case "wards"
                    mySqlAction = " SELECT        count(Wards.Wardid) " &
                    " FROM            Wards INNER JOIN " &
                                             " Location ON Wards.Wardid = Location.Wardid " &
                                                                                 "where wards.wardid = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    If recordcount_cummulative > 0 Then Return True
                Case "facilities"
                    mySqlAction = " SELECT        count(Facilities.Facilityid) " &
                    " FROM            Facilities INNER JOIN " &
                        "  Clientdetails ON Facilities.Facilityid = Clientdetails.Facility " &
                                                                                "where Facilities.Facilityid = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    If recordcount_cummulative > 0 Then Return True
                Case "schools"
                    mySqlAction = " SELECT        count(Schools.SchoolID) " &
                    " FROM            Schools INNER JOIN " &
                         " Clientdetails ON Schools.SchoolID = Clientdetails.School " &
                        "where Schools.SchoolID = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount
                    If recordcount_cummulative > 0 Then Return True
                Case "schoollevel"
                    mySqlAction = "SELECT count(SchoolLevel.SchoolLevelID) " &
                 " FROM            SchoolLevel INNER JOIN " &
                      " Clientdetails On SchoolLevel.SchoolLevelID = Clientdetails.SchoolLevel " &
                      "where schoollevelid = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    mySqlAction = " SELECT        count(SchoolLevel.SchoolLevelID) " &
                    " FROM            SchoolLevel INNER JOIN " &
                         " Clientdetails ON SchoolLevel.SchoolLevelID = Clientdetails.SchoolLevel " &
                      "where schoollevelid = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    If recordcount_cummulative > 0 Then Return True
                Case "chw"
                    mySqlAction = " SELECT        count(CHW.CHWID) " &
                    " FROM            CHW INNER JOIN " &
                         " Clientdetails ON CHW.CHWID = Clientdetails.VolunteerId " &
                     "where CHW.CHWID = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    If recordcount_cummulative > 0 Then Return True

                Case "servicestatus"
                    mySqlAction = " SELECT        count(ServiceStatus.SSID) " &
                    " FROM            ServiceStatus INNER JOIN " &
                        "  StatusAndServiceMonitoring_Assessment ON ServiceStatus.SSID = StatusAndServiceMonitoring_Assessment.SSID " &
                                       "where ServiceStatus.SSID = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    mySqlAction = " SELECT        count(ServiceStatus.SSID) " &
                    " FROM            ServiceStatus INNER JOIN " &
                                             " StatusAndServiceMonitoring_Service ON ServiceStatus.SSID = StatusAndServiceMonitoring_Service.SSID " &
                                                           "where ServiceStatus.SSID = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    mySqlAction = " SELECT        count(ServiceStatus.SSID) " &
                    " FROM            ServiceStatus INNER JOIN " &
                                             " caregiver_StatusAndServiceMonitoring_Assessment ON ServiceStatus.SSID = caregiver_StatusAndServiceMonitoring_Assessment.SSID " &
                                                           "where ServiceStatus.SSID = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    mySqlAction = " SELECT        count(ServiceStatus.SSID) " &
                    " FROM            ServiceStatus INNER JOIN " &
                                             " caregiver_StatusAndServiceMonitoring_Service ON ServiceStatus.SSID = caregiver_StatusAndServiceMonitoring_Service.SSID " &
                                                           "where ServiceStatus.SSID = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    If recordcount_cummulative > 0 Then Return True
                Case "parentdetails"
                    mySqlAction = " SELECT        count(ParentDetails.ParentId) " &
                    " FROM            ParentDetails INNER JOIN " &
                                             " Clientdetails ON ParentDetails.ParentId = Clientdetails.GuardianID " &
                                        "where ParentDetails.ParentId = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    mySqlAction = " SELECT        count(ParentDetails.ParentId) " &
                    " FROM            ParentDetails INNER JOIN " &
                                             " Clientdetails ON ParentDetails.ParentId = Clientdetails.MotherID " &
                                        "where ParentDetails.ParentId = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    mySqlAction = " SELECT        count(ParentDetails.ParentId) " &
                    " FROM            ParentDetails INNER JOIN " &
                                             " Clientdetails ON ParentDetails.ParentId = Clientdetails.FatherID " &
                                        "where ParentDetails.ParentId = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    mySqlAction = " SELECT        count(ParentDetails.ParentId) " &
                    " FROM            ParentDetails INNER JOIN " &
                                             " Clientdetails ON ParentDetails.ParentId = Clientdetails.householdheadID " &
                                        "where ParentDetails.ParentId = '" & primarykey & "'"
                    recordcount = MyDBAction.DBAction(mySqlAction, functions.DBActionType.Scalar)
                    recordcount_cummulative = recordcount_cummulative + recordcount

                    If recordcount_cummulative > 0 Then Return True


            End Select

            'if there is no relationship, return false
            Return False

        Catch ex As Exception

            Return True
        End Try
    End Function
    Public Function AddClient(ByVal myOVCID As String, ByVal myFirstName As String, ByVal myMiddleName As String _
           , ByVal mySurname As String, ByVal myGender As String, ByVal myDateofBirth As Date, ByVal myBirthCert As Integer _
           , ByVal myClientType As Double, ByVal myCbo As Integer, ByVal myDistrict As Integer, ByVal myLocation As Integer _
           , ByVal myFacility As Integer, ByVal mydateoflinkage As Date, ByVal myImmunization As Integer, ByVal myVolunteerId As String _
           , ByVal mySchoolLevel As Integer, ByVal mySchool As Integer, ByVal myschoolclass As Integer, ByVal myHIVStatus As Integer, ByVal myARTStatus As Integer, ByVal myCriteria As Integer, ByVal myguardianID As String _
           , ByVal myfatherID As String, ByVal mymotherID As String, ByVal myhouseholdheadID As String, ByVal myCareTakerID As String _
           , ByVal myDateofVisit As Date, ByVal myExit As Integer, ByVal mydateofreg As Date,
           ByVal mydateofexit As Date, ByVal myreasonforexit As Integer, ByVal mycreatedon As Date,
           ByVal mycreatedsession As String, ByVal myschoolingtype As String,
           ByVal mybcertnumber As String, ByVal myisdisabled As Integer, ByVal myncpwdnumber As String, ByVal mycccnumber As String, ByVal mymode As String) As Integer

        Dim myID As Double = 0
        Dim myConnection As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        Dim myCommand As SqlCommand = Nothing
        myConnection.Open()
        'select wht entry mode,normal or longitudinal
        If mymode = "normal" Then
            myCommand = New SqlCommand("Add_Client", myConnection)
        ElseIf mymode = "longitudinal" Then
            myCommand = New SqlCommand("Add_LongClient", myConnection)
        End If

        Try
            With myCommand
                .CommandType = Data.CommandType.StoredProcedure
                .Parameters.Add("@ClientID", Data.SqlDbType.Real).Direction = Data.ParameterDirection.Output
                .Parameters.Add("@OVCID", Data.SqlDbType.VarChar, 50).Value = myOVCID
                .Parameters.Add("@FirstName", Data.SqlDbType.VarChar, 50).Value = myFirstName
                .Parameters.Add("@MiddleName", Data.SqlDbType.VarChar, 50).Value = myMiddleName
                .Parameters.Add("@Surname", Data.SqlDbType.VarChar, 50).Value = mySurname
                .Parameters.Add("@Gender", Data.SqlDbType.VarChar, 10).Value = myGender
                .Parameters.Add("@DateofBirth", Data.SqlDbType.DateTime).Value = myDateofBirth
                .Parameters.Add("@BirthCert", Data.SqlDbType.Int).Value = myBirthCert
                .Parameters.Add("@ClientType", Data.SqlDbType.BigInt).Value = myClientType
                .Parameters.Add("@Cbo", Data.SqlDbType.Int).Value = myCbo
                .Parameters.Add("@District", Data.SqlDbType.Int).Value = myDistrict
                .Parameters.Add("@Location", Data.SqlDbType.Int).Value = myLocation
                .Parameters.Add("@Facility", Data.SqlDbType.Int).Value = myFacility
                .Parameters.Add("@DateofLinkage", Data.SqlDbType.DateTime).Value = mydateoflinkage
                .Parameters.Add("@Immunization", Data.SqlDbType.Int).Value = myImmunization
                .Parameters.Add("@VolunteerId", Data.SqlDbType.VarChar, 50).Value = myVolunteerId
                .Parameters.Add("@SchoolLevel", Data.SqlDbType.Int).Value = mySchoolLevel
                .Parameters.Add("@School", Data.SqlDbType.Int).Value = mySchool
                .Parameters.Add("@Class", Data.SqlDbType.Int).Value = myschoolclass
                .Parameters.Add("@HIVStatus", Data.SqlDbType.Int).Value = myHIVStatus
                .Parameters.Add("@ARTStatus", Data.SqlDbType.Int).Value = myARTStatus
                .Parameters.Add("@EligibilityCriteria", Data.SqlDbType.Int).Value = myCriteria
                .Parameters.Add("@FatherID", Data.SqlDbType.VarChar, 50).Value = myfatherID
                .Parameters.Add("@MotherID", Data.SqlDbType.VarChar, 50).Value = mymotherID
                .Parameters.Add("@GuardianID", Data.SqlDbType.VarChar, 50).Value = myguardianID
                .Parameters.Add("@CareTaker", Data.SqlDbType.VarChar, 50).Value = myCareTakerID
                .Parameters.Add("@HouseHoldheadID", Data.SqlDbType.VarChar, 50).Value = myhouseholdheadID
                .Parameters.Add("@DateofVisit", Data.SqlDbType.DateTime).Value = myDateofVisit
                .Parameters.Add("@DateofExit", Data.SqlDbType.DateTime).Value = mydateofexit
                .Parameters.Add("@DateofRegistration", Data.SqlDbType.DateTime).Value = mydateofreg
                .Parameters.Add("@Exited", Data.SqlDbType.Int).Value = myExit
                .Parameters.Add("@ReasonforExit", Data.SqlDbType.Int).Value = myreasonforexit
                .Parameters.Add("@createdon", Data.SqlDbType.DateTime).Value = mycreatedon
                .Parameters.Add("@createdsession", Data.SqlDbType.VarChar, 50).Value = mycreatedsession
                .Parameters.Add("@schoolingtype", Data.SqlDbType.VarChar, 50).Value = myschoolingtype
                .Parameters.Add("@bcertnumber", Data.SqlDbType.VarChar, 50).Value = mybcertnumber
                .Parameters.Add("@isdisabled", Data.SqlDbType.Int).Value = myisdisabled
                .Parameters.Add("@NCPWDNumber", Data.SqlDbType.VarChar, 50).Value = myncpwdnumber
                .Parameters.Add("@cccNumber", Data.SqlDbType.VarChar, 50).Value = mycccnumber
                .ExecuteNonQuery()
                myID = .Parameters.Item("@ClientID").Value
            End With

            myConnection.Close()

            Return myID

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical, "Biodata update")
        End Try

    End Function

    Public Function UpdateClient(ByVal my_ovcdisability_type As String _
      , ByVal my_disability_assessment_date As DateTime _
      , ByVal my_Diagnosis As String _
      , ByVal my_Interventions As String _
      , ByVal my_NCPWD_Contacts As String _
      , ByVal my_NCPWD_Residence As String _
      , ByVal my_NCPWD_Other_Support As String _
      , ByVal my_NCPWD_Rehab_Centre As String _
      , ByVal my_HIV_Screening_Date As DateTime _
      , ByVal my_HIV_Screening_Outcome As String _
      , ByVal my_HIV_Testing_Date As DateTime _
        , ByVal my_cpims_ovc_id As String)

        Dim myID As Double = 0
        Dim myConnection As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        Dim myCommand As SqlCommand = Nothing
        myConnection.Open()

        myCommand = New SqlCommand("Update_Client", myConnection)


        Try
            With myCommand
                .CommandType = Data.CommandType.StoredProcedure

                .Parameters.Add("@ovcdisability_type", Data.SqlDbType.VarChar, 50).Value = my_ovcdisability_type
                .Parameters.Add("@disability_assessment_date", Data.SqlDbType.DateTime).Value = my_disability_assessment_date
                .Parameters.Add("@Diagnosis", Data.SqlDbType.VarChar, 50).Value = my_Diagnosis
                .Parameters.Add("@Interventions", Data.SqlDbType.VarChar, 50).Value = my_Interventions
                .Parameters.Add("@NCPWD_Contacts", Data.SqlDbType.VarChar, 50).Value = my_NCPWD_Contacts
                .Parameters.Add("@NCPWD_Residence", Data.SqlDbType.VarChar, 50).Value = my_NCPWD_Residence
                .Parameters.Add("@NCPWD_Other_Support", Data.SqlDbType.VarChar, 250).Value = my_NCPWD_Other_Support
                .Parameters.Add("@NCPWD_Rehab_Centre", Data.SqlDbType.VarChar, 50).Value = my_NCPWD_Rehab_Centre
                .Parameters.Add("@HIV_Screening_Date", Data.SqlDbType.DateTime).Value = my_HIV_Screening_Date
                .Parameters.Add("@HIV_Screening_Outcome", Data.SqlDbType.VarChar, 50).Value = my_HIV_Screening_Outcome
                .Parameters.Add("@HIV_Testing_Date", Data.SqlDbType.DateTime).Value = my_HIV_Testing_Date
                .Parameters.Add("@cpims_ovc_id", Data.SqlDbType.VarChar, 50).Value = my_cpims_ovc_id

                .ExecuteNonQuery()

            End With

            myConnection.Close()

            Return myID

        Catch ex As Exception
            MsgBox(ex.Message, vbCritical, "Biodata update")

        End Try

    End Function

    Public Function DeleteNeeds(ByVal myNeedsAssessementid As String)

        Dim myConnection As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        myConnection.Open()
        Dim myCommand As New SqlCommand("Delete_Needs", myConnection)

        Try
            With myCommand
                .CommandType = Data.CommandType.StoredProcedure
                .Parameters.Add("@NeedsAssessmentid", Data.SqlDbType.VarChar, 50).Value = myNeedsAssessementid
                .ExecuteNonQuery()

            End With

            myConnection.Close()

        Catch ex As Exception

        End Try

    End Function

    Public Function DeleteServiceMonitoring(ByVal myssvid As String)

        Dim myConnection As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        myConnection.Open()
        Dim myCommand As New SqlCommand("Delete_ServiceMonitoring", myConnection)

        Try
            With myCommand
                .CommandType = Data.CommandType.StoredProcedure
                .Parameters.Add("@ssvid", Data.SqlDbType.VarChar, 50).Value = myssvid
                .ExecuteNonQuery()

            End With

            myConnection.Close()

        Catch ex As Exception

        End Try

    End Function



    Public Function FilterCBOData() As Boolean

        Dim myConnection As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        myConnection.Open()
        Dim myCommand As New SqlCommand("FilterCBOData", myConnection)

        Try
            With myCommand
                .CommandTimeout = 3600000
                .CommandType = Data.CommandType.StoredProcedure
                .ExecuteNonQuery()

            End With

            myConnection.Close()
            Return True
        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try

    End Function

    'Public Sub ClearControls()

    '    Try

    '        Dim mycontrol As Control
    '        Dim MyTextBox As New TextBox
    '        Dim MyDropdownlist As New ComboBox

    '        For Each mycontrol In 
    '            MyTextBox = TryCast(mycontrol, TextBox)
    '            If MyTextBox IsNot Nothing Then
    '                MyTextBox.Text = ""
    '            End If

    '            MyDropdownlist = TryCast(mycontrol, DropDownList)
    '            If MyDropdownlist IsNot Nothing Then
    '                MyDropdownlist.SelectedValue = ""
    '            End If

    '            MyHiddenField = TryCast(mycontrol, HiddenField)
    '            If MyHiddenField IsNot Nothing Then
    '                MyHiddenField.Value = ""
    '            End If
    '        Next
    '    Catch ex As Exception

    '    End Try

    'End Sub
    Public Sub WriteToErrorLogFile(ByVal PageAffected As String, ByVal ModuleAffected As String, ByVal ErrorDescription As String)

        Try

            Dim ErrorDate As DateTime = Date.Now()

            Dim writer As StreamWriter = Nothing
            Dim MystringBuilder As New StringBuilder
            Dim UploadTemppath As String = Application.StartupPath

            'Dim UploadTemppath As String = WebConfigurationManager.AppSettings("ErrorLogsLocation").ToString

            UploadTemppath = UploadTemppath & "AphiaError.txt"
            '& 
            If File.Exists(UploadTemppath) Then
                File.GetAccessControl(UploadTemppath)
                File.Delete(UploadTemppath)
            Else
                'File.Create(UploadTemppath)
                If File.Exists(UploadTemppath) Then
                    File.GetAccessControl(UploadTemppath)
                End If

            End If
            writer = New StreamWriter(UploadTemppath, True)

            '***********************************************************************************
            writer = System.IO.File.AppendText(UploadTemppath)

            writer.WriteLine("CLIENT", ModuleAffected, ErrorDescription, ErrorDate)

            '*******************************************************************************

            writer.Close()
            writer.Dispose()


        Catch ex As Exception

        End Try

    End Sub

    Public Function GenRandomPassword(ByVal keylength As Integer) As String
        Dim KeyGen As RandomStringGenerator
        Dim NumKeys As Integer
        'Dim i_Keys As Integer
        Dim RandomKey As String

        ' MODIFY THIS TO GET MORE KEYS    - LAITH - 27/07/2005 22:48:30 -
        NumKeys = 1

        KeyGen = New RandomStringGenerator


        RandomKey = RandomStringGenerator.GenerateRandomString(keylength)

        Return RandomKey

        'For i_Keys = 1 To NumKeys
        '    RandomKey = KeyGen.Generate()
        '    Console.WriteLine(RandomKey)
        'Next
        'Console.WriteLine("Press any key To Exit...")
        'Console.Read()
    End Function

   

    Public Function SystemUpdate(ByVal zippedfile As String, ByVal extractlocation As String) As Boolean
        Try
            If Directory.Exists(extractlocation) = True Then
                Directory.Delete(extractlocation)
            End If


            '1. Unzip files into a folder
            If ExtractZip(zippedfile, extractlocation) = False Then
                Return False
            End If

            'Run all scripts
            ' Make a reference to a directory.
            Dim di As New DirectoryInfo(extractlocation)
            ' Get a reference to each file in that directory.
            Dim fiArr As FileInfo() = di.GetFiles()
            ' Display the names of the files.
            Dim fri As FileInfo
            For Each fri In fiArr
                'If fri.Extension = ".sql" AndAlso fri.FullName.ToLower.Contains("backupdb.sql") = False Then
                '    If runSQLScriptFiles(fri.FullName) = False Then
                '        Return False
                '    End If
                'End If

                'copy report templates
                If fri.Extension = ".xlsm" Then
                    ' If file already exists in destination, delete it.
                    If File.Exists(Application.StartupPath & "\TemplatesExcelReports\" & fri.Name) Then
                        File.Delete(Application.StartupPath & "\TemplatesExcelReports\" & fri.Name)
                    End If
                    File.Copy(fri.FullName, Application.StartupPath & "\TemplatesExcelReports\" & fri.Name)
                End If
                'MsgBox("Reports")
                'copy the version.txt file from the new updates folder
                'Also copy sql scripts. they have an issue being called from their update directory. Lets place them in C:\OVC
                If fri.Extension = ".txt" Or fri.Extension = ".sql" Then
                    ' If file already exists in destination, delete it.
                    If File.Exists(Application.StartupPath & "\" & fri.Name) Then
                        File.Delete(Application.StartupPath & "\" & fri.Name)
                    End If
                    File.Copy(fri.FullName, Application.StartupPath & "\" & fri.Name)
                End If

                'Copy th EXE
                If fri.Name.ToLower = "OVCSystem.exe" Then
                    ' If file already exists in destination, delete it.
                    If File.Exists(Application.StartupPath & "\OVCSystem_copy.exe") Then
                        File.Delete(Application.StartupPath & "\OVCSystem_copy.exe")
                    End If
                    File.Copy(fri.FullName, Application.StartupPath & "\OVCSystem_copy.exe")
                End If




            Next fri

            '2. Backup files into Backups folder
            '1st step is to confirm if this is the  server or Client machine. Scripts should only run on the server
            Dim builder As New System.Data.SqlClient.SqlConnectionStringBuilder()
            builder.ConnectionString = ConnectionStrings(SelectedConnectionString).ToString
            Dim server As String = UCase(builder.DataSource)
            Dim database As String = UCase(builder.InitialCatalog)


            'If this machine is the server, backup the db and execute update scripts
            If (server.Contains(UCase("localhost")) = True Or server.Contains(".\") = True Or server.Contains(UCase(strmachinename)) = True) Then
                'Backup database
                If runSQLScriptFiles(extractlocation & "\backupdb.sql") = False Then
                    Return False
                End If
            End If

            'If this machine is the server, backup the db and execute update scripts
            If (server.Contains(UCase("localhost")) = True Or server.Contains(".\") = True Or server.Contains(UCase(strmachinename)) = True) Then
               
                'MsgBox("backup")
                'run ddl update file
                'd. Restart OLMIS
                System.Diagnostics.Process.Start(extractlocation & "\SchemaUpdates.bat")
                'Dim su As New ProcessStartInfo("cmd.exe", "/c cd " & extractlocation & " " & extractlocation & "\SchemaUpdates.bat")
                '' Redirect both streams so we can write/read them.
                'su.RedirectStandardInput = True
                'su.RedirectStandardOutput = True
                'su.UseShellExecute = True
                '' Start the procses.
                'Dim psu As Process = Process.Start(su)
                'System.Diagnostics.Process.Start("cmd.exe", "/c cd " & extractlocation & " " & extractlocation & "\SchemaUpdates.bat")
                'MsgBox("dbupdate")

            End If

            
            'MsgBox("version.txt")

            '3 

            '3. Invoke Updates batch file, which will
            'a. Stop OLMIS Exe
            'b. Backup database to C:\OVC\Backups
            'c. Copy and replace files
            'd. Restart OLMIS
            System.Diagnostics.Process.Start(extractlocation & "\updates.bat")

            'Process.Start("cmd.exe", "/c cd C:\TestFolder & C:\TestFolder\MyBatchFile.bat")

            'Dim si As New ProcessStartInfo("cmd.exe", "/c cd " & extractlocation & " " & extractlocation & "\updates.bat")
            '' Redirect both streams so we can write/read them.
            'si.RedirectStandardInput = True
            'si.RedirectStandardOutput = True
            'si.UseShellExecute = True
            '' Start the procses.
            'Dim p As Process = Process.Start(si)
            'System.Diagnostics.Process.Start("cmd.exe", "/c cd " & extractlocation & " " & extractlocation & "\updates.bat")
            'MsgBox("updates.bat")

            Return True
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation)
            Return False
        End Try

    End Function

    Private Function ExtractZip(ByVal zippedfile As String, ByVal extractlocation As String) As Boolean
        Try

            Dim ZipToUnpack As String = zippedfile '"C1P3SML.zip"
            Dim UnpackDirectory As String = extractlocation '"Extracted Files"


            Using zip1 As ZipFile = ZipFile.Read(ZipToUnpack)
                Dim e As ZipEntry
                ' here, we extract every entry, but we could extract conditionally,
                ' based on entry name, size, date, checkbox status, etc.   
                For Each e In zip1
                    e.Extract(UnpackDirectory, ExtractExistingFileAction.OverwriteSilently)
                Next
            End Using


            Return True
        Catch ex As Exception
            'File.Delete(zippedfile)
            MsgBox(ex.Message & vbCr & " File download will retry shortly.", vbExclamation)
            Return False
        End Try

    End Function



    Private Function runSQLScriptFiles(ByVal scriptfilename As String) As Boolean
        Dim connectionstring As String = ConnectionStrings(SelectedConnectionString).ToString
        Try

            'first create stored procedures to create temporary tables
            Dim cmd As New System.Data.SqlClient.SqlCommand()
            Dim conn As New System.Data.SqlClient.SqlConnection(connectionstring)
            cmd.Connection = conn
            cmd.CommandTimeout = 3600000 '60 minutes just incase
            cmd.CommandText = Readscript(scriptfilename)
            conn.Open()
            cmd.ExecuteNonQuery()
            Return True
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation)
            Return False
        End Try
    End Function

    Private Function Readscript(ByVal scriptfile As String) As String
        Try
            Using sr As New StreamReader(scriptfile)
                Return sr.ReadToEnd()
            End Using
        Catch e As Exception
            MsgBox("The file could not be read:" & e.Message, MsgBoxStyle.Critical)
        End Try
    End Function
End Class


'This class will handle create, read, write and delete of files
Public Class FileOperations
    Public Function createfile(ByVal fileloc As String) As Boolean
        Try
            Dim fs As FileStream = Nothing
            If (Not File.Exists(fileloc)) Then
                fs = File.Create(fileloc)
                Using fs

                End Using
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function writeCreateBlankCBOdbscript(ByVal dbname As String, ByVal dbfile As String) As Boolean
        Try
            'first, dynamically create the attach script to allow change of directory location
            Dim newscript As New FileOperations
            Dim fileloc As String = AppSettings("ScriptPath").ToString & "createcbodb.sql"
            Dim IsSuccess As Boolean
            IsSuccess = newscript.createfile(fileloc)

            If IsSuccess = True Then
                If File.Exists(fileloc) Then
                    Using sw As StreamWriter = New StreamWriter(fileloc)
                        sw.WriteLine("USE [master]")
                        sw.WriteLine("GO")
                        sw.WriteLine("sp_attach_db """ & dbname & """, """ & dbfile & """ ")
                        sw.WriteLine("GO")

                    End Using
                End If
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function writeattachdbscript(ByVal dbname As String, ByVal dbfile As String) As Boolean
        Try
            'first, dynamically create the attach script to allow change of directory location
            Dim newscript As New FileOperations
            Dim fileloc As String = AppSettings("ScriptPath").ToString & "attachdb.sql"
            Dim IsSuccess As Boolean
            IsSuccess = newscript.createfile(fileloc)

            If IsSuccess = True Then
                If File.Exists(fileloc) Then
                    Using sw As StreamWriter = New StreamWriter(fileloc)
                        sw.WriteLine("USE [master]")
                        sw.WriteLine("GO")
                        sw.WriteLine("sp_attach_db """ & dbname & """, """ & dbfile & """ ")
                        sw.WriteLine("GO")

                    End Using
                End If
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function writedetachdbscript(ByVal dbname As String) As Boolean
        Try
            'first, dynamically create the attach script to allow change of directory location
            Dim newscript As New FileOperations
            Dim fileloc As String = AppSettings("ScriptPath").ToString & "detachdb.sql"
            Dim IsSuccess As Boolean
            IsSuccess = newscript.createfile(fileloc)

            If IsSuccess = True Then
                If File.Exists(fileloc) Then
                    Using sw As StreamWriter = New StreamWriter(fileloc)
                        sw.WriteLine("USE Master")
                        sw.WriteLine("GO")
                        sw.WriteLine("sp_detach_db " & dbname & "")
                        sw.WriteLine("GO")

                    End Using
                End If
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function writefile(ByVal fileloc As String) As Boolean
        Try
            If File.Exists(fileloc) Then
                Using sw As StreamWriter = New StreamWriter(fileloc)
                    sw.Write("Some sample text for the file")
                End Using
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function


    Public Function readfile(ByVal fileloc As String) As Boolean
        Try
            If File.Exists(fileloc) Then
                Using tr As TextReader = New StreamReader(fileloc)
                    MessageBox.Show(tr.ReadLine())
                End Using
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Function copyfile(ByVal fileloc As String, ByVal fileLocCopy As String) As Boolean
        Try
            'Dim fileLocCopy As String = "d:\sample1.txt"
            If File.Exists(fileloc) Then
                ' If file already exists in destination, delete it.
                If File.Exists(fileLocCopy) Then
                    File.Delete(fileLocCopy)
                End If
                File.Copy(fileloc, fileLocCopy)
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Function movefile(ByVal fileloc As String, ByVal fileLocMove As String) As Boolean
        Try
            ' Create unique file name
            'Dim fileLocMove As String = "d:\sample1" & System.DateTime.Now.Ticks & ".txt"
            If File.Exists(fileloc) Then
                File.Move(fileloc, fileLocMove)
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Function deletefile(ByVal fileloc As String) As Boolean
        Try
            If File.Exists(fileloc) Then
                File.Delete(fileloc)
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Function replacetextinfile(ByVal strfile As String, ByVal stringtofind As String, ByVal replacewith As String) As Boolean
        Try
            'Dim strFile As String = "C:\Documents and Settings\Orbops\Desktop\Ben's stuff\1.txt"
            Dim result As String

            Dim reader As TextReader = File.OpenText(strfile)

            result = Regex.Replace(reader.ReadToEnd(), "" & stringtofind & "", "" & replacewith & "")

            reader.Close()
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Function fetch_Operationaldistrict() As String
        Dim m_xmld As New XmlDocument
        Dim m_nodelist As XmlNodeList
        Dim m_node As XmlNode
        'Create the XML Document
        m_xmld = New XmlDocument()
        'Load the Xml file
        m_xmld.Load("mysettings.xml")
        'Get the list of name nodes 
        m_nodelist = m_xmld.SelectNodes("/SETTINGS1/SETTINGS")
        'Loop through the nodes
        For Each m_node In m_nodelist

            'Get the Gender Attribute Value
            Dim attribute_districtlist As String = m_node.Attributes.GetNamedItem("districtlist").Value
            Return attribute_districtlist

        Next
    End Function

    Public Function fetch_Operationalcbo() As String

    End Function
End Class
