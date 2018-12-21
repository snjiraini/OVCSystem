Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Math
Imports System.Data.SqlClient
Imports System.Xml
Imports System.IO
Imports System.Text
Imports Ionic.Zip
Imports Ionic.Zlib
'Imports SharpCompress.Common
'Imports SharpCompress.Archive
Imports System.Configuration
Imports OVCSystem.functions

Public Class frmExportMaintenanceData

    Private m_XMLSplitter As New XMLDocumentSplitter()

    Private Sub frmExportMaintenanceData_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.WebBrowser1.Url = New System.Uri(Application.StartupPath & "\images\358.gif", System.UriKind.Absolute)
    End Sub

    Private Sub btnExportMaintenanceData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExportMaintenanceData.Click
        Try

            'If you click the button when worker is still fetching results, cancel the click event
            If worker.IsBusy = True Then
                MsgBox("Fetching results. Please be patient.", MsgBoxStyle.Exclamation)
            Else
                Progresspanel.Visible = True
                'run the report queries on a different thread to avoid freezing of application
                worker.RunWorkerAsync()
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub worker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs) Handles worker.DoWork
        Try
            ' For i = 1 To 10

            If worker.CancellationPending = True Then
                e.Cancel = True
                'Exit For
            Else
                ' Perform a time consuming operation and report progress.
                'Threading.Thread.Sleep(500)
                'worker.ReportProgress(i * 10)
                If NormalExport() = True Then
                    ZipExports(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"), "MaintainenanceData")
                End If

            End If
            'Next
        Catch ex As Exception

        End Try

    End Sub

    Private Sub worker_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs) Handles worker.ProgressChanged
        Me.lblProgress.Text = e.ProgressPercentage.ToString() & "%"
    End Sub

    Private Sub worker_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles worker.RunWorkerCompleted
        Try
            Progresspanel.Visible = False
            If e.Cancelled = True Then
                MsgBox("Export Canceled.", MsgBoxStyle.Exclamation)
            ElseIf e.Error IsNot Nothing Then
                MsgBox("Error: " & e.Error.Message, MsgBoxStyle.Critical)
            Else
                MsgBox("Export Successul.", MsgBoxStyle.Information)
            End If

        Catch ex As Exception

        End Try


    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Try
            If worker.WorkerSupportsCancellation = True Then
                worker.CancelAsync()
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Function NormalExport() As Boolean
        Try

            'create a folder for today's exports
            ' Determine whether the directory exists.
            If Directory.Exists(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy")) = False Then
                Dim di As DirectoryInfo = Directory.CreateDirectory(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"))
            Else
                'delete it
                My.Computer.FileSystem.DeleteDirectory(Application.StartupPath & "\Exports\" & _
                Format(Date.Now, "dd-MMM-yyyy"), FileIO.DeleteDirectoryOption.DeleteAllContents)

                'then create it
                Dim di As DirectoryInfo = Directory.CreateDirectory(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"))
            End If

            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim mySqlAction As String

            '1. Regions
            '1a. District
            
            mySqlAction = "SELECT [DistrictID],[District] as District_name,[Countyid] FROM [District]" & _
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('District')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("District")

            End If

            '1 Wards
            mySqlAction = "select [WardID],[Ward],[Countyid] from  Wards " & _
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('wards')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("wards")

            End If

            '1a.a County
            mySqlAction = "select [CountyID],[County] from  county " & _
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('county')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("county")

            End If

            '1b. cbo
            mySqlAction = "SELECT [CBOID],[CBO] as CBO_name ,[DistrictID] FROM [CBO]" & _
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('cbo')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("cbo")

            End If

            '1c. Location
            mySqlAction = "SELECT [LocationID],[Location] as Location_name,[DistrictID],[Wardid]FROM [Location]" & _
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('location')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("location")

            End If

            '1d. Clusters
            mySqlAction = "SELECT [ClusterId],[ClusterName],[ClusterCBOs] FROM [Clusters]" & _
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('clusters')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("clusters")

            End If

            'show 10% progress
            worker.ReportProgress(10)

            '2. Education
            '2a. schoollevel
            mySqlAction = "SELECT [SchoolLevelID],[SchoolLevel] as SchoolLevel_name FROM [SchoolLevel]" & _
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('schoollevel')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("schoollevel")

            End If

            '2b. schoolname
            mySqlAction = "SELECT [SchoolID],[SchoolName],[District],[Schoollevel] FROM [Schools]" & _
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('schools')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("schools")

            End If

            '2c. class
            mySqlAction = "SELECT [ClassId],[Class] as Class_name,[SchoolLevel] FROM [Class]" & _
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('class')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("class")

            End If

            'show 20% progress
            worker.ReportProgress(20)

            '3. Health
            '3a. hivstatus
            mySqlAction = "SELECT [HIVStatusID],[HIVStatus] as HIVStatus_name FROM [HIVStatus]" & _
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('hivstatus')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("hivstatus")

            End If

            '3b. artstatus
            mySqlAction = "SELECT [ARTStatusID] ,[ARTStatus] as ARTStatus_name FROM [ARTStatus]" & _
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('artstatus')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("artstatus")

            End If

            '3c. facilities
            mySqlAction = "SELECT [Facilityid] ,[Facility] ,[DistrictId] FROM [Facilities]" & _
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('facilities')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("facilities")

            End If

            'show 30% progress
            worker.ReportProgress(30)

            '4. Domains
            '4a. domain
            mySqlAction = "SELECT [DomainID],[Domain] as Domain_name,[IsHCBC],[IsOVC],[IsActive] ,[IsCaregiver] FROM [Domain]" & _
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('domain')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("domain")

            End If

            '4b. subdomain
            mySqlAction = "SELECT [SubdomainID] ,[DomainID] ,[Subdomain] as Subdomain_name FROM [SubDomain]" & _
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('subdomain')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("subdomain")

            End If

            '4c. CoreServices
            mySqlAction = "SELECT [CSID],[Domainid],[CoreService],[MustSelect],[Type] FROM [CoreServices]" & _
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('CoreServices')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("CoreServices")

            End If

            '4d. ServiceStatus
            'when a field has same name as table, errors come up. So servicestatus comes as [Status], but needs to be mapped back to servicestatus
            mySqlAction = "select [SSID],[CSID],[ServiceStatus] as ServiceStatus_name,[Duration],[Procurable] " & _
      " ,[IsPriority],[IsHCBC],[IsOVC],[SSCode],[IsActive],[IsCaregiver] " & _
     " ,[AgeRequired],[Gender],[Needstobeselectedprior],[Needstobeunselectedprior] " & _
      " ,[PriorNumberofMonths],[AffectsLongitudinalbiodata] " & _
        " from ServiceStatus " & _
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('ServiceStatus')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("ServiceStatus")

            End If

            'show 40% progress
            worker.ReportProgress(40)

            '5. Eligibility
            mySqlAction = "select [ClienttypeID] ,[ClientType] as ClientType_name from  [ClientType] " & _
                                    " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('clienttype')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("clienttype")

            End If

            'show 50% progress
            worker.ReportProgress(50)

            '6. Reason for exit
            mySqlAction = "SELECT [HidingReasonID] ,[HidingReason] as HidingReason_name FROM [HidingReason]" & _
                                              " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('hidingreason')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("hidingreason")

            End If

            'show 60% progress
            worker.ReportProgress(60)

            '7a. CSI providers
            mySqlAction = "select [ProviderID],[Provider] as Provider_name from  [Provider] " & _
                                              " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('provider')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("provider")

            End If

            '7b. Goals
            mySqlAction = "select [GoalID],[Goal] as Goal_name from  [Goal]" & _
                                              " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('goal')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("goal")

            End If

            'show 70% progress
            worker.ReportProgress(70)

            '8. Cause of Death
            mySqlAction = "select [CauseofDeathID],[CauseofDeath] as CauseofDeath_name from  [CauseofDeath] " & _
                                             " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('causeofdeath')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("causeofdeath")

            End If

            'show 80% progress
            worker.ReportProgress(80)

            '9. Relationship
            mySqlAction = "select [RelationshipID],[Relationship] as Relationship_name  from  [Relationship] " & _
                                              " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('relationship')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("relationship")

            End If

            'show 90% progress
            worker.ReportProgress(90)

            '10. Employment Types
            mySqlAction = "select [employmenttypeid],[EmploymentType] as EmploymentType_name  from  [EmploymentType] " & _
                                              " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('employmenttype')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("employmenttype")

            End If

            '11. Critical Events
            mySqlAction = "SELECT [CriticalEventsID],[EventName],[IsOVC],[IsCaregiver] FROM [dbo].[CriticalEvents] " & _
                                              " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('criticalevents')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("criticalevents")

            End If

            '12. Immunization
            mySqlAction = "SELECT [ImmunizationID],[ImmunizationStatus] FROM [dbo].[Immunization] " & _
                                              " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('immunization')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("immunization")

            End If

            '13. HHDomains
            mySqlAction = "SELECT [HHDomainid],[HHDomain] FROM [dbo].[HHAssessmentDomain] " & _
                                              " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('HHAssessmentDomain')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("HHAssessmentDomain")

            End If

            '14. HHAssessentCodes
            mySqlAction = "SELECT [HHCodeid],[HHCode],[HHDomainid] FROM [dbo].[HHAssessentCodes] " & _
                                              " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('HHAssessentCodes')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("HHAssessentCodes")

            End If

            '15. HHResponses
            mySqlAction = "SELECT [Responseid],[HHCodeid],[Response] FROM [dbo].[HHResponses] " & _
                                              " FOR XML AUTO,TYPE, ELEMENTS ,ROOT('HHResponses')"
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 And MyDatable.Rows(0).Item(0).ToString.Length > 0 Then
                ' convert string to stream
                Dim byteArray As Byte() = Encoding.ASCII.GetBytes(MyDatable.Rows(0).Item(0).ToString)
                Dim stream As New MemoryStream(byteArray)


                m_XMLSplitter.SplitType = XMLDocumentSplitter.SplitTypes.ByElementCount
                m_XMLSplitter.SplitSize = CType("1000", Long)

                m_XMLSplitter.Split(New System.IO.StreamReader(stream), AddressOf SplitHandler)

                renamesplitfiles("HHResponses")

            End If

            'show 100% progress
            worker.ReportProgress(100)

            Return True
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation)
            Return False
        End Try
    End Function

    Private Sub SplitHandler(ByVal count As Long, ByVal document As String)
        Dim stream_writer As New System.IO.StreamWriter(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy") & "\" & _
        String.Format("Splitter{0:D8}.xml", count))
        stream_writer.Write(document)
        stream_writer.Close()
    End Sub

    Private Sub renamesplitfiles(ByVal newprefix As String)
        Dim di As New IO.DirectoryInfo(Application.StartupPath & "\Exports\" & Format(Date.Now, "dd-MMM-yyyy"))
        RecurseDirectories(di, newprefix)
    End Sub

    Private Sub RecurseDirectories(ByVal di As DirectoryInfo, ByVal newprefix As String)
        Try

            'For Each d In di.GetDirectories()
            Dim strFileSize As String = ""
            Dim fi As IO.FileInfo

            Dim aryFi As IO.FileInfo() = di.GetFiles("*.*")
            For Each fi In aryFi

                If fi.Name.ToString.Contains("Splitter") = True Then
                    Dim Myrandomkey As New Random
                    Dim oldname As String = fi.Name
                    Dim newname As String = Replace(oldname, "Splitter", newprefix.ToString & Myrandomkey.Next(1111, 9999))
                    My.Computer.FileSystem.RenameFile(fi.FullName, newname)

                End If

            Next

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        'Next
        'Catch
        'End Try
    End Sub

    Private Sub ZipExports(ByVal ExportsDirectory As String, ByVal zipname As String)
        Try
            ''Split file incase it is big
            Using zip As New ZipFile()
                ' add all those files to the  folder in the zip file
                zip.AddDirectory(ExportsDirectory)

                ''we want to extract straight into the /Exports folder and ditch the dated-folder
                'Dim di As New DirectoryInfo(ExportsDirectory)
                'Dim strFileSize As String = ""
                'Dim fi As IO.FileInfo

                'Dim aryFi As IO.FileInfo() = di.GetFiles("*.*")
                'For Each fi In aryFi
                '    'add the individual files into the zip
                '    zip.AddFile(fi.FullName)
                'Next

                zip.Comment = "This zip was created at " & System.DateTime.Now.ToString("G")
                zip.MaxOutputSegmentSize = 2 * 1024 * 1024   '' 2mb
                zip.Save(zipname & "-" & Format(Date.Now, "dd-MMM-yyyy") & ".zip")
            End Using


            MsgBox("Data Export Zipping Successful.", MsgBoxStyle.Information)

        Catch ex As Exception
            MsgBox("Data Exports Zipping NOT successful.", MsgBoxStyle.Exclamation)
        End Try

    End Sub
End Class