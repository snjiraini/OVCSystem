Imports OVCSystem.functions
Imports System.Data.Odbc
Imports System.Configuration.ConfigurationManager
Imports System.Math
Imports System.Data.SqlClient
Imports System.Xml
Imports System.IO

Public Class frmServiceandStatusMonitoring

    Public myparentid, myguardianid, mychw, myGender As String 'to allow access from anywhere in the form
    Public HasBirthCert As Boolean
    Public myAge As Integer
    Dim IsHCBCClient As Boolean = False
    Dim IsCaregiver As Boolean = False
    Dim global_ssvid As String = "" 'The id of that particular visit
    Dim visitType As String = "monitor" 'the default is monitoring
    Dim strnames As String = "" 'this variable holds part of the search query for names
    Dim MyTreeView1DataTable, MylstpriorityListingDataTable As New Data.DataTable 'Helps store treeviewitems in memory to avoid reading db all the time 

    Dim myPreviousHIVStatus As String = "" 'will help us avoid HIV+ children changing back to HIV-
    Dim myPreviousOVCID As String = ""
  

    Private Sub frmServiceandStatusMonitoring_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        myOVCID = ""

        'check to see if it is an OVC or HCBC client
        If Me.Text.ToUpper.Contains("HCBC") Then
            IsHCBCClient = True
            GroupBox1.Text = "HCBC"
            'lnkmonitorstatus.Text = "HCBC Assessment"
            'lnkServicemode.Text = "HCBC Services"
        ElseIf Me.Text.ToUpper.Contains("CAREGIVER") Then
            IsCaregiver = True
            GroupBox1.Text = "CAREGIVER"
            'lnkmonitorstatus.Text = "CAREGIVER Assessment"
            'lnkServicemode.Text = "CAREGIVER Services"
        Else
            IsHCBCClient = False
            GroupBox1.Text = "OVC"
            'lnkmonitorstatus.Text = "OVC Assessment"
            'lnkServicemode.Text = "OVC Services"
        End If

        populatedistricts()
        'populatepriorityListing()
        AddRootNode("monitor")
        AddRootNodepriority()
        AddCriticalEvents()

        populateHIVStatus()

        'clear treeview and checkboxes
        clear_checked(TreeView1)
        clear_checked(lstpriorityListing)

        'populatepriorityListing()
        dtpDateofVisit.Value = Date.Today


        'dont hide priorities when monitoring
        Label17.Visible = True
        lstpriorityListing.Visible = True

        Label22.Visible = True
        lstCriticalEvents.Visible = True
        'fillgrid()

        Me.AcceptButton = btnSearch
    End Sub

    Private Sub AddCriticalEvents()
        Dim ErrorAction As New functions
        Try

            'populate the listbox
            Dim mySqlAction As String = ""
            If IsCaregiver = True Then
                mySqlAction = "select * from criticalevents where isnull(IsCaregiver,'False') = 'True' order by EventName"
            Else
                mySqlAction = "select * from criticalevents where isnull(IsOVC,'False') = 'True' order by EventName"
            End If

            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With lstCriticalEvents
                .DataSource = Nothing 'if already bound, throws a bug on any refresh
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "Eventname"
                .ValueMember = "CriticalEventsID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("CriticalEvents", "AddCriticalEvents", ex.Message) ''---Write error to error log file

        End Try
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
                .DataSource = Nothing 'if already bound, throws a bug on any refresh
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "District"
                .ValueMember = "DistrictID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("StatusMonitoring", "populatedistricts", ex.Message) ''---Write error to error log file

        End Try
    End Sub


    Private Sub populateCBO(ByVal mydistrict As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from CBO where districtid = '" & mydistrict & _
            "' and cboid in (" & strcbos & ") order by CBO asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboCBO
                .DataSource = Nothing 'if already bound, throws a bug on any refresh
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "CBO"
                .ValueMember = "CBOID"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("StatusMonitoring", "populatecbo", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    'Private Sub populatepriorityListing()
    '    Dim ErrorAction As New functions
    '    Try

    '        'populate the combobox
    '        Dim mySqlAction As String = "SELECT DomainID, Domain, CSID, CoreService, Type," & _
    '        "  SSID, ServiceStatus FROM vw_domain_services WHERE Type = 'Service' ORDER BY ServiceStatus"
    '        Dim MyDBAction As New functions
    '        Dim MyDatable As New Data.DataTable
    '        MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
    '        With lstPriorityListing
    '            .DataSource = Nothing 'if already bound, throws a bug on any refresh
    '            .Items.Clear()
    '            .DataSource = MyDatable
    '            .DisplayMember = "ServiceStatus"
    '            .ValueMember = "SSID"
    '            .SelectedIndex = -1 ' This line makes the combo default value to be blank
    '        End With
    '    Catch ex As Exception
    '        ErrorAction.WriteToErrorLogFile("StatusMonitoring", "populatepriorityListing", ex.Message) ''---Write error to error log file


    '    End Try
    'End Sub

    Private Sub AddRootNodepriority()
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)

        Try
            lstpriorityListing.CheckBoxes = True
            Dim MyDomainName As String = ""
            Dim MyDomainid As String = ""
            Dim MnuCount As Int16 = 0
            Dim j As Int16

            'Dim MyDataTable As New Data.DataTable
            'if its a hcbc client, load hcbc domains
            Dim sqlAction As String
            If IsHCBCClient = True Then
                sqlAction = "Select domainid,domain from domain where isnull(ishcbc,'False') = 'True' order by domainid asc"
            ElseIf IsCaregiver = True Then
                sqlAction = "Select domainid,domain from domain where isnull(iscaregiver,'False') = 'True' order by domainid asc"
            Else
                sqlAction = "select DomainID, Domain " & _
                 " from domain " & _
             " order by domainid asc"
            End If



            lstpriorityListing.Nodes.Clear() 'first remove any nodes

            'Dim MyDBAction As New functions
            'MyDataTable = TryCast(MyDBAction.DBAction(sqlAction, DBActionType.DataTable), Data.DataTable)

            If IsCaregiver = False Then
                Dim cmd As New SqlCommand(sqlAction, conn)
                conn.Open()
                Dim myreader As SqlDataReader = cmd.ExecuteReader()

                'If MyDataTable.Rows.Count > 0 Then
                '    MnuCount = MyDataTable.Rows.Count
                '    For j = 0 To MyDataTable.Rows.Count - 1
                Do While myreader.Read
                    MyDomainName = myreader("domain").ToString
                    MyDomainid = myreader("domainid").ToString
                    Dim MyMainNode As New TreeNode
                    MyMainNode.Tag = MyDomainid
                    MyMainNode.Text = MyDomainName.ToUpper.ToString
                    AddChildNodepriority(MyDomainid.ToString, MyMainNode) 'Add the services for each domain respectively
                    lstpriorityListing.Nodes.Add(MyMainNode)
                Loop
            End If



            '    Next

            'End If

            lstpriorityListing.ExpandAll()
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub
    Private Sub AddChildNodepriority(ByVal strMainDomainid As String, ByRef MyNode As TreeNode)
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)

        Try
            Dim MySubdomainName As String = ""
            Dim MySubdomainID As String = ""
            Dim myValidations As String = ""
            Dim MyURL As String = ""
            Dim MnuCount As Int16 = 0
            Dim j As Int16

            'Dim MyDataTable As New Data.DataTable
            'Is this HCBC or normal OVC
            Dim sqlAction As String
            If IsHCBCClient = True Then
                sqlAction = "SELECT  Domain.DomainID, Domain.Domain, CoreServices.CSID, CoreServices.CoreService, " & _
                               " CoreServices.Type, ServiceStatus.SSID, " & _
                           " ServiceStatus.ServiceStatus, ServiceStatus.IsPriority " & _
                           ",'' as agerequired, '' as gender, '' as needstobeselectedprior,'' as needstobeunselectedprior,'' as priornumberofmonths,'' as affectslongitudinalbiodata" & _
               " FROM  Domain INNER JOIN " & _
                                     " CoreServices ON Domain.DomainID = CoreServices.Domainid INNER JOIN " & _
                                     " ServiceStatus ON CoreServices.CSID = ServiceStatus.CSID " & _
               " WHERE (CoreServices.Type = 'Service') AND (ServiceStatus.IsPriority = 'True') and (isnull(ServiceStatus.IsHCBC,'False') = 'True') AND Domain.DomainID = '" & strMainDomainid.ToString & "' "

            ElseIf IsCaregiver = True Then
                sqlAction = "SELECT  Domain.DomainID, Domain.Domain, CoreServices.CSID, CoreServices.CoreService, " & _
                               " CoreServices.Type, ServiceStatus.SSID, " & _
                           " '[' + isnull(ServiceStatus.SSCode,'[--]') + '] ' + ServiceStatus.ServiceStatus AS ServiceStatus, ServiceStatus.IsPriority " & _
                           ",'' as agerequired, '' as gender, '' as needstobeselectedprior,'' as needstobeunselectedprior,'' as priornumberofmonths,'' as affectslongitudinalbiodata" & _
               " FROM  Domain INNER JOIN " & _
                                     " CoreServices ON Domain.DomainID = CoreServices.Domainid INNER JOIN " & _
                                     " ServiceStatus ON CoreServices.CSID = ServiceStatus.CSID " & _
               " WHERE (CoreServices.Type = 'Service') AND (ServiceStatus.IsPriority = 'True') and (isnull(ServiceStatus.Iscaregiver,'False') = 'True') AND Domain.DomainID = '" & strMainDomainid.ToString & "' "
            Else
                sqlAction = "SELECT  Domain.DomainID, Domain.Domain, CoreServices.CSID, CoreServices.CoreService, " & _
                               " CoreServices.Type, ServiceStatus.SSID, " & _
                           " '[' + isnull(ServiceStatus.SSCode,'[--]') + '] ' + ServiceStatus.ServiceStatus AS ServiceStatus, ServiceStatus.IsPriority " & _
                           ",agerequired,gender,needstobeselectedprior,needstobeunselectedprior,priornumberofmonths,affectslongitudinalbiodata" & _
               " FROM  Domain INNER JOIN " & _
                                     " CoreServices ON Domain.DomainID = CoreServices.Domainid INNER JOIN " & _
                                     " ServiceStatus ON CoreServices.CSID = ServiceStatus.CSID " & _
               " WHERE (CoreServices.Type = 'Service') AND (ServiceStatus.IsPriority = 'True') and (isnull(ServiceStatus.IsOVC,'False') = 'True') AND Domain.DomainID = '" & strMainDomainid.ToString & "' "
            End If



            'Dim MyDBAction As New functions
            'MyDataTable = TryCast(MyDBAction.DBAction(sqlAction, DBActionType.DataTable), Data.DataTable)

            Dim cmd As New SqlCommand(sqlAction, conn)
            conn.Open()
            Dim myreader As SqlDataReader = cmd.ExecuteReader()

            'If MyDataTable.Rows.Count > 0 Then
            '    MnuCount = MyDataTable.Rows.Count
            '    For j = 0 To MyDataTable.Rows.Count - 1
            Do While myreader.Read
                MySubdomainName = myreader("servicestatus").ToString
                MySubdomainID = myreader("ssid").ToString

                Dim MySubNode As New TreeNode
                MySubNode.Tag = MySubdomainID
                MySubNode.Text = MySubdomainName

                'Have the valiation fields as the tag, to avoid going back to db
                myValidations = _
                    IIf(Len(myreader("agerequired").ToString) = 0, 0, myreader("agerequired").ToString) & _
                    ":" & myreader("gender").ToString & _
                    ":" & myreader("Needstobeselectedprior").ToString & _
                    ":" & myreader("Needstobeunselectedprior").ToString & _
                    ":" & myreader("PriorNumberofMonths").ToString & _
                    ":" & myreader("AffectsLongitudinalbiodata").ToString

                MySubNode.ToolTipText = myValidations
                MyNode.Nodes.Add(MySubNode)
            Loop

            '    Next

            'End If
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    'Private Sub AddStatusNode(ByRef MyNode As TreeNode, ByVal MyCSID As String, ByVal mymode As String)

    '    Try
    '        Dim MyStatusName As String = ""
    '        Dim MystatusID As String = ""
    '        Dim mystatustype As String = ""
    '        Dim MyURL As String = ""
    '        Dim MnuCount As Int16 = 0
    '        Dim j As Int16


    '        Dim MyStatusTable As New Data.DataTable
    '        MyStatusTable = SelectDistinct(MyTreeView1DataTable, "csid", "servicestatus", "type", "ssid")


    '        If MyStatusTable.Rows.Count > 0 Then
    '            MnuCount = MyStatusTable.Rows.Count
    '            For j = 0 To MyStatusTable.Rows.Count - 1

    '                'Make sure we are populating node with correct children and also check if its monitor or serve
    '                If MyCSID.ToString = MyStatusTable.Rows(j).Item("csid").ToString AndAlso _
    '                    LCase(MyStatusTable.Rows(j).Item("type").ToString) = mymode Then
    '                    MyStatusName = MyStatusTable.Rows(j).Item("servicestatus").ToString
    '                    MystatusID = MyStatusTable.Rows(j).Item("ssid").ToString
    '                    mystatustype = MyStatusTable.Rows(j).Item("type").ToString


    '                    Dim MySubNode As New TreeNode
    '                    MySubNode.Tag = MystatusID
    '                    MySubNode.Text = MyStatusName
    '                    MySubNode.ToolTipText = mystatustype


    '                    MyNode.Nodes.Add(MySubNode)
    '                    'MsgBox(MySubNode.Tag & "-" & MySubNode.Text)
    '                End If


    '            Next

    '        End If





    '    Catch ex As Exception
    '        MsgBox(ex.Message)

    '    End Try
    'End Sub

    'Private Sub AddRootNodepriority()
    '    Dim conn As New SqlConnection(ConnectionStrings( SelectedConnectionString  ).ToString)

    '    Try
    '        lstpriorityListing.CheckBoxes = True
    '        Dim MyDomainName As String = ""
    '        Dim MyDomainid As String = ""
    '        Dim MnuCount As Int16 = 0
    '        Dim j As Int16

    '        'Dim MyDataTable As New Data.DataTable
    '        'if its a hcbc client, load hcbc domains
    '        Dim sqlAction As String
    '        If IsHCBCClient = True Then
    '            sqlAction = "Select domainid,domain from domain where isnull(ishcbc,'False') = 'True'"
    '        Else
    '            sqlAction = "select DomainID, " & _
    '        " CASE domain WHEN 'Food and Nutrition' THEN '1. Food and Nutrition' " & _
    '         " WHEN 'Shelter and Care' THEN '2. Shelter and Care'  " & _
    '         " WHEN 'Protection' THEN '3. Protection' " & _
    '         " WHEN 'Health' THEN '4. Health' WHEN 'Psychosocial' THEN '5. Psychosocial' " & _
    '         " WHEN 'Education and Skills Training' THEN '6. Education and Skills Training' " & _
    '          " WHEN 'Economic Opportunity' " & _
    '         " THEN '7. Economic Opportunity' END AS Domain " & _
    '             " from domain " & _
    '         " order by domain"
    '        End If



    '        lstpriorityListing.Nodes.Clear() 'first remove any nodes

    '        'Dim MyDBAction As New functions
    '        'MyDataTable = TryCast(MyDBAction.DBAction(sqlAction, DBActionType.DataTable), Data.DataTable)

    '        Dim cmd As New SqlCommand(sqlAction, conn)
    '        conn.Open()
    '        Dim myreader As SqlDataReader = cmd.ExecuteReader()

    '        'If MyDataTable.Rows.Count > 0 Then
    '        '    MnuCount = MyDataTable.Rows.Count
    '        '    For j = 0 To MyDataTable.Rows.Count - 1
    '        Do While myreader.Read
    '            MyDomainName = myreader("domain").ToString
    '            MyDomainid = myreader("domainid").ToString
    '            Dim MyMainNode As New TreeNode
    '            MyMainNode.Tag = MyDomainid
    '            MyMainNode.Text = MyDomainName.ToUpper.ToString
    '            AddChildNodepriority(MyDomainid.ToString, MyMainNode) 'Add the services for each domain respectively
    '            lstpriorityListing.Nodes.Add(MyMainNode)
    '        Loop


    '        '    Next

    '        'End If

    '        lstpriorityListing.ExpandAll()
    '    Catch ex As Exception
    '        MsgBox(ex.Message)
    '    Finally
    '        conn.Close()
    '    End Try
    'End Sub
    'Private Sub AddChildNodepriority(ByVal strMainDomainid As String, ByRef MyNode As TreeNode)
    '    Dim conn As New SqlConnection(ConnectionStrings( SelectedConnectionString  ).ToString)

    '    Try
    '        Dim MySubdomainName As String = ""
    '        Dim MySubdomainID As String = ""
    '        Dim MyURL As String = ""
    '        Dim MnuCount As Int16 = 0
    '        Dim j As Int16

    '        'Dim MyDataTable As New Data.DataTable
    '        'Is this HCBC or normal OVC
    '        Dim sqlAction As String
    '        If IsHCBCClient = True Then
    '            sqlAction = "SELECT  Domain.DomainID, Domain.Domain, CoreServices.CSID, CoreServices.CoreService, " & _
    '                           " CoreServices.Type, ServiceStatus.SSID, " & _
    '                       " ServiceStatus.ServiceStatus, ServiceStatus.IsPriority " & _
    '           " FROM  Domain INNER JOIN " & _
    '                                 " CoreServices ON Domain.DomainID = CoreServices.Domainid INNER JOIN " & _
    '                                 " ServiceStatus ON CoreServices.CSID = ServiceStatus.CSID " & _
    '           " WHERE (CoreServices.Type = 'Service') AND (ServiceStatus.IsPriority = 'True') and (isnull(ServiceStatus.IsHCBC,'False') = 'True') AND Domain.DomainID = '" & strMainDomainid.ToString & "' "
    '        Else
    '            sqlAction = "SELECT  Domain.DomainID, Domain.Domain, CoreServices.CSID, CoreServices.CoreService, " & _
    '                           " CoreServices.Type, ServiceStatus.SSID, " & _
    '                       " ServiceStatus.ServiceStatus, ServiceStatus.IsPriority " & _
    '           " FROM  Domain INNER JOIN " & _
    '                                 " CoreServices ON Domain.DomainID = CoreServices.Domainid INNER JOIN " & _
    '                                 " ServiceStatus ON CoreServices.CSID = ServiceStatus.CSID " & _
    '           " WHERE (CoreServices.Type = 'Service') AND (ServiceStatus.IsPriority = 'True') and (isnull(ServiceStatus.IsHCBC,'False') = 'False') AND Domain.DomainID = '" & strMainDomainid.ToString & "' "
    '        End If



    '        'Dim MyDBAction As New functions
    '        'MyDataTable = TryCast(MyDBAction.DBAction(sqlAction, DBActionType.DataTable), Data.DataTable)

    '        Dim cmd As New SqlCommand(sqlAction, conn)
    '        conn.Open()
    '        Dim myreader As SqlDataReader = cmd.ExecuteReader()

    '        'If MyDataTable.Rows.Count > 0 Then
    '        '    MnuCount = MyDataTable.Rows.Count
    '        '    For j = 0 To MyDataTable.Rows.Count - 1
    '        Do While myreader.Read
    '            MySubdomainName = myreader("servicestatus").ToString
    '            MySubdomainID = myreader("ssid").ToString

    '            Dim MySubNode As New TreeNode
    '            MySubNode.Tag = MySubdomainID
    '            MySubNode.Text = MySubdomainName

    '            MyNode.Nodes.Add(MySubNode)
    '        Loop

    '        '    Next

    '        'End If
    '    Catch ex As Exception
    '        MsgBox(ex.Message)
    '    Finally
    '        conn.Close()
    '    End Try
    'End Sub

    Private Sub AddRootNode(ByVal mode As String)
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)

        Try
            TreeView1.CheckBoxes = True
            Dim MyDomainName As String = ""
            Dim MyDomainid As String = ""
            Dim MnuCount As Int16 = 0
            Dim j As Int16
            Dim MyDomainTable As New Data.DataTable

            TreeView1.Nodes.Clear() 'first remove any nodes

            'Only fetch fresh records if this table is empty, which means its the first time form is being loaded
            If MyTreeView1DataTable.Rows.Count = 0 Then

                MyTreeView1DataTable = New Data.DataTable

                'Dim sqlAction As String = "SELECT ServiceStatus.SSID, CoreServices.CSID, ServiceStatus.ServiceStatus, " & _
                '"ServiceStatus.Duration, ServiceStatus.Procurable, CoreServices.Type, Domain.DomainID, " & _
                '"Domain.Domain, CoreServices.CoreService " & _
                '"FROM CoreServices INNER JOIN " & _
                '"ServiceStatus ON CoreServices.CSID = ServiceStatus.CSID INNER JOIN " & _
                '"Domain ON CoreServices.Domainid = Domain.DomainID"

                'Check if it is a HCBC  client of an OVC to fetch correct monitoring/services
                Dim cmd As SqlCommand
                If IsHCBCClient = True Then
                    cmd = New SqlCommand("dbo.HCBCForm1A_AddRootNode")
                ElseIf IsCaregiver = True Then
                    cmd = New SqlCommand("dbo.CAREGIVERForm1A_AddRootNode")
                Else
                    cmd = New SqlCommand("dbo.Form1A_AddRootNode")
                End If

                cmd.CommandType = CommandType.StoredProcedure
                cmd.CommandTimeout = 300
                ''cmd.Parameters.Add(New SqlParameter("@OVCID", myOVCID.ToString))
                ''cmd.Parameters.Add(New SqlParameter("@DateofVisit", dtpDateofVisit.Value))
                conn.Open()
                cmd.Connection = conn

                Dim myreader As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)

                'Load data from the reader to the datatable
                MyTreeView1DataTable.Load(myreader)

                'Dim MyDBAction As New functions
                'MyTreeView1DataTable = TryCast(MyDBAction.DBAction(sqlAction, DBActionType.DataTable), Data.DataTable)

            End If

            MyDomainTable = SelectDistinct(MyTreeView1DataTable, "domainid", "domain")

            If MyDomainTable.Rows.Count > 0 Then
                MnuCount = MyDomainTable.Rows.Count
                For j = 0 To MyDomainTable.Rows.Count - 1
                    'Do While myreader.Read
                    MyDomainName = MyDomainTable.Rows(j).Item("domain").ToString
                    MyDomainid = MyDomainTable.Rows(j).Item("domainid").ToString
                    Dim MyMainNode As New TreeNode
                    MyMainNode.Tag = MyDomainid
                    MyMainNode.Text = MyDomainName.ToUpper.ToString
                    AddChildNode(MyDomainid.ToString, MyMainNode, mode) 'Add the subdomains for each domain respectively
                    TreeView1.Nodes.Add(MyMainNode)
                    'Loop


                Next j

            End If

            TreeView1.ExpandAll()
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try
    End Sub

    Private Sub AddChildNode(ByVal strMainDomainid As String, ByRef MyNode As TreeNode, ByVal mymode As String)

        Try
            Dim MyCoreserviceName As String = ""
            Dim MycoreserviceID As String = ""
            Dim MyURL As String = ""
            Dim MnuCount As Int16 = 0
            Dim j As Int16



            Dim MyCoreServiceTable As New Data.DataTable
            MyCoreServiceTable = SelectDistinct(MyTreeView1DataTable, "domainid", "Coreservice", "csid", "type")


            If MyCoreServiceTable.Rows.Count > 0 Then
                MnuCount = MyCoreServiceTable.Rows.Count
                For j = 0 To MyCoreServiceTable.Rows.Count - 1

                    'Only populate nodes with children who belong there
                    If strMainDomainid.ToString = MyCoreServiceTable.Rows(j).Item("domainid").ToString _
                        AndAlso LCase(MyCoreServiceTable.Rows(j).Item("type").ToString) = mymode Then

                        MyCoreserviceName = MyCoreServiceTable.Rows(j).Item("Coreservice").ToString
                        MycoreserviceID = MyCoreServiceTable.Rows(j).Item("csid").ToString

                        Dim MySubNode As New TreeNode
                        MySubNode.Tag = MycoreserviceID
                        MySubNode.Text = MyCoreserviceName

                        AddStatusNode(MySubNode, MycoreserviceID, mymode)

                        MyNode.Nodes.Add(MySubNode)
                        'Loop
                    End If

                Next

            End If
        Catch ex As Exception
            MsgBox(ex.Message)
            '
        End Try
    End Sub

    Private Sub AddStatusNode(ByRef MyNode As TreeNode, ByVal MyCSID As String, ByVal mymode As String)

        Try
            Dim MyStatusName As String = ""
            Dim MystatusID As String = ""
            Dim mystatustype As String = ""
            Dim myValidations As String = ""
            Dim MyURL As String = ""
            Dim MnuCount As Int16 = 0
            Dim j As Int16


            Dim MyStatusTable As New Data.DataTable
            MyStatusTable = SelectDistinct(MyTreeView1DataTable, "csid", "servicestatus", "type", "ssid", _
                                           "AgeRequired", "Gender", "Needstobeselectedprior", "Needstobeunselectedprior", "PriorNumberofMonths", "AffectsLongitudinalbiodata")


            If MyStatusTable.Rows.Count > 0 Then
                MnuCount = MyStatusTable.Rows.Count
                For j = 0 To MyStatusTable.Rows.Count - 1

                    'Make sure we are populating node with correct children and also check if its monitor or serve
                    If MyCSID.ToString = MyStatusTable.Rows(j).Item("csid").ToString AndAlso _
                        LCase(MyStatusTable.Rows(j).Item("type").ToString) = mymode Then
                        MyStatusName = MyStatusTable.Rows(j).Item("servicestatus").ToString
                        MystatusID = MyStatusTable.Rows(j).Item("ssid").ToString
                        mystatustype = MyStatusTable.Rows(j).Item("type").ToString

                        'Have the valiation fields as the tag, to avoid going back to db
                        myValidations = _
                            IIf(Len(MyStatusTable.Rows(j).Item("agerequired").ToString) = 0, 0, MyStatusTable.Rows(j).Item("agerequired").ToString) & _
                            ":" & MyStatusTable.Rows(j).Item("gender").ToString & _
                            ":" & MyStatusTable.Rows(j).Item("Needstobeselectedprior").ToString & _
                            ":" & MyStatusTable.Rows(j).Item("Needstobeunselectedprior").ToString & _
                            ":" & MyStatusTable.Rows(j).Item("PriorNumberofMonths").ToString & _
                            ":" & MyStatusTable.Rows(j).Item("AffectsLongitudinalbiodata").ToString


                        Dim MySubNode As New TreeNode
                        MySubNode.Tag = MystatusID
                        MySubNode.Text = MyStatusName
                        MySubNode.ToolTipText = myValidations


                        MyNode.Nodes.Add(MySubNode)
                        'MsgBox(MySubNode.Tag & "-" & MySubNode.Text)
                    End If


                Next

            End If





        Catch ex As Exception
            MsgBox(ex.Message)

        End Try
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click

        Dim ErrorAction As New functions
        Try
            'if it is a hcbc client, search from parents table, else search from kawaida ovc table
            Dim mySqlAction As String
            If IsCaregiver = True Then
                '        mySqlAction = "SELECT Parentdetails.Parentid as Clientid, Parentdetails.Parentid as OVCID, Parentdetails.FirstName, Parentdetails.MiddleName, " & _
                '"Parentdetails.Surname, CHW.CHWID, CHW.FirstName AS CHWFirstName,Parentdetails.gender,'True' as BirthCert,Parentdetails.chw as volunteerid,'18' as Age,  " & _
                ' "CHW.MiddleName AS CHWMiddleName, CHW.Surname AS CHWSurname, CHW.ID  " & _
                ' "FROM Parentdetails LEFT OUTER JOIN  " & _
                ' "CHW ON Parentdetails.CHW = CHW.CHWID where isnull(ishcbcclient,'false') = 'true' and 1 = 1"
                mySqlAction = _
                    "SELECT   ParentDetails.ParentId AS Clientid, ParentDetails.ParentId AS OVCID, ParentDetails.FirstName, ParentDetails.MiddleName, " & _
                    " ParentDetails.Surname, CHW.CHWID, CHW.FirstName AS CHWFirstName, ParentDetails.Gender, 'True' AS BirthCert,  " & _
                    " CHW.CHWID AS volunteerid, '18' AS Age, CHW.MiddleName AS CHWMiddleName, CHW.Surname AS CHWSurname,  " & _
                    " CHW.ID " & _
                    " FROM  ParentDetails INNER JOIN " & _
                    " Clientdetails ON ParentDetails.ParentId = Clientdetails.HouseHoldheadID INNER JOIN " & _
                    " CHW ON Clientdetails.VolunteerId = CHW.CHWID where 1 = 1 "

                If IsNumeric(cboDistrict.SelectedValue) Then
                    mySqlAction = mySqlAction & " AND Clientdetails.District = '" & cboDistrict.SelectedValue & "'"
                End If
                If IsNumeric(cboCBO.SelectedValue) Then
                    mySqlAction = mySqlAction & " AND Clientdetails.cbo = '" & cboCBO.SelectedValue & "'"
                End If
                If txtsearchFname.Text.ToString.Length <> 0 Then
                    mySqlAction = mySqlAction & " AND Parentdetails.firstname = '" & txtsearchFname.Text.ToString & "'"
                End If
                If txtsearchMname.Text.ToString.Length <> 0 Then
                    mySqlAction = mySqlAction & " AND Parentdetails.middlename = '" & txtsearchMname.Text.ToString & "'"
                End If
                If txtsearchSurname.Text.ToString.Length <> 0 Then
                    mySqlAction = mySqlAction & " AND Parentdetails.surname = '" & txtsearchSurname.Text.ToString & "'"
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
                mySqlAction = mySqlAction & _
                    " GROUP BY ParentDetails.ParentId, ParentDetails.FirstName, ParentDetails.MiddleName, ParentDetails.Surname, " & _
                " CHW.CHWID, CHW.FirstName, ParentDetails.Gender, CHW.MiddleName, CHW.Surname, CHW.ID "
            Else
                'populate the datagrid with all the data
                mySqlAction = "SELECT Clientdetails.Clientid,Clientdetails.OVCID, Clientdetails.FirstName, Clientdetails.MiddleName,Clientdetails.Surname, CHW.CHWID, CHW.FirstName AS CHWFirstName,Clientdetails.gender,Clientdetails.BirthCert,Clientdetails.volunteerid,DATEDIFF(year, Clientdetails.DateofBirth, GETDATE()) as Age, " & _
            " CHW.MiddleName AS CHWMiddleName, CHW.Surname AS CHWSurname, CHW.ID" & _
            " FROM Clientdetails LEFT OUTER JOIN" & _
            " CHW ON Clientdetails.VolunteerId = CHW.CHWID where 1 = 1"

                If IsNumeric(cboDistrict.SelectedValue) Then
                    mySqlAction = mySqlAction & " AND Clientdetails.District = '" & cboDistrict.SelectedValue & "'"
                End If
                If IsNumeric(cboCBO.SelectedValue) Then
                    mySqlAction = mySqlAction & " AND Clientdetails.cbo = '" & cboCBO.SelectedValue & "'"
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
            End If


            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim myClientid, myFname, MyMname, myLname, myOVCID, mychwfname, mychwmname, mychwsurname, mychwid As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView1.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    'myClientid = MyDatable.Rows(K).Item("ClientID").ToString
                    myFname = MyDatable.Rows(K).Item("Firstname").ToString
                    MyMname = MyDatable.Rows(K).Item("Middlename").ToString
                    myLname = MyDatable.Rows(K).Item("Surname").ToString
                    myOVCID = MyDatable.Rows(K).Item("OVCID").ToString
                    mychw = MyDatable.Rows(K).Item("volunteerid").ToString

                    'populate other content that will be needed later within the form
                    If IsCaregiver = True Then
                        HasBirthCert = False
                    Else
                        HasBirthCert = CheckforBcert(myOVCID) 'CBool(MyDatable.Rows(K).Item("BirthCert"))
                    End If

       

                    myGender = MyDatable.Rows(K).Item("gender").ToString
                    myAge = MyDatable.Rows(K).Item("age").ToString
                    mychwfname = MyDatable.Rows(K).Item("chwfirstname").ToString
                    mychwmname = MyDatable.Rows(K).Item("chwmiddlename").ToString
                    mychwsurname = MyDatable.Rows(K).Item("chwsurname").ToString
                    mychwid = MyDatable.Rows(K).Item("chwid").ToString
                    DataGridView1.Rows.Add(myClientid, myFname, MyMname, myLname, _
                    myOVCID, mychwfname, mychwmname, mychwsurname, mychwid, "Select", HasBirthCert, myAge, myGender)
                Next
            Else 'if there are no rows returned
                MsgBox("Search query returned no results", MsgBoxStyle.Exclamation)
            End If

            'we set focus back to fname so that by clicking 'search' again, we loose focus on txtname thus 
            'recreating the search criteria again
            txtsearchFname.Focus()

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("StatusMonitoring", "clientsearch", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Function CheckforBcert(ByVal strovcid As String) As Boolean
        Try
            Dim MyDBAction As New functions
            Dim mySqlAction As String
            Dim birthcertexists As Boolean = False
            'Make sure we dont enter duplicate IDNumbers. Couldnt enforce this on DB coz there are blank IDs and autogenerated IDs too
            mySqlAction = "SELECT COUNT(Clientdetails.OVCID) " & _
            " FROM Clientdetails INNER JOIN " & _
            " ClientLongitudinalDetails ON Clientdetails.OVCID = ClientLongitudinalDetails.OVCID " & _
            " WHERE ((Clientdetails.OVCID = '" & strovcid & "') AND (Clientdetails.BirthCert = 'True')) " & _
            " OR  " & _
            " ((ClientLongitudinalDetails.OVCID = '" & strovcid & "') AND (ClientLongitudinalDetails.BirthCert = 'True')) "

            birthcertexists = CBool(MyDBAction.DBAction(mySqlAction, DBActionType.Scalar))

            Return birthcertexists

        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function CheckforHIVStatus(ByVal strhivstatus As String) As Int32
        Try
            Dim MyDBAction As New functions
            Dim mySqlAction As String
            Dim myHIVStatus As String = ""
            'Make sure we dont enter duplicate IDNumbers. Couldnt enforce this on DB coz there are blank IDs and autogenerated IDs too
            mySqlAction = "SELECT hivstatusid " & _
            " FROM hivstatus" & _
            " WHERE hivstatus = '" & strhivstatus & "'"
            myHIVStatus = MyDBAction.DBAction(mySqlAction, DBActionType.Scalar)

            Return myHIVStatus

        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub cboDistrict_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboDistrict.SelectedValueChanged
        Try
            If cboDistrict.SelectedValue <> Nothing Then
                populateCBO(cboDistrict.SelectedValue.ToString)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub DataGridView1_CellContentClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        Dim ErrorAction As New functions
        Try

            'select item to edit or delete
            Dim mysqlaction As String = ""
            Dim MyDBAction As New functions
            Dim K As Integer
            K = e.RowIndex

            'populate variables for later use when saving
            myOVCID = Me.DataGridView1.Rows(K).Cells(4).Value.ToString
            HasBirthCert = Me.DataGridView1.Rows(K).Cells(10).Value.ToString
            myAge = Me.DataGridView1.Rows(K).Cells(11).Value.ToString
            myGender = Me.DataGridView1.Rows(K).Cells(12).Value.ToString


            'Dim MyDatable As New Data.DataTable
            'mysqlaction = "select * from vw_client_hivstatus where clientid=" & Me.DataGridView1.Rows(K).Cells(0).Value & ""
            'MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)

            'Check kama ni HCBC client ama ovc wa kawaida, then use correct sp
            Dim cmd As SqlCommand
            If IsHCBCClient = True Then
                cmd = New SqlCommand("dbo.HCBCForm1A_FetchOVCandParentNames")
            ElseIf IsCaregiver = True Then
                cmd = New SqlCommand("dbo.CAREGIVERForm1A_FetchOVCandParentNames")
            Else
                cmd = New SqlCommand("dbo.Form1A_FetchOVCandParentNames")
            End If

            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandTimeout = 300
            cmd.Parameters.Add(New SqlParameter("@OVCID", myOVCID.ToString))
            ''cmd.Parameters.Add(New SqlParameter("@DateofVisit", dtpDateofVisit.Value))
            conn.Open()
            cmd.Connection = conn

            Dim myreader As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)


            'If MyDatable.Rows.Count > 0 Then
            '    If MyDatable.Rows.Count > 0 Then
            Do While myreader.Read
                clearcontrols() 'first clear previous data from controls

                'clear treeview and checkboxes
                clear_checked(TreeView1)
                clear_checked(lstpriorityListing)

                'clear critical events listbox
                Dim dview As DataRowView
                Dim i As Integer = 0
                For Each dview In lstCriticalEvents.Items
                    lstCriticalEvents.SetItemChecked(i, False)
                    i = i + 1
                Next

                'confirm who is the caretaker so tht we can populate his/her names in parent's fields
                If LCase(myreader("caretaker").ToString) = LCase(myreader("ninanihuyu").ToString) Then
                    txtparentfname.Text = myreader("parentfname").ToString
                    txtparentmname.Text = myreader("parentmname").ToString
                    txtparentsname.Text = myreader("parentsname").ToString
                    txtCBO.Text = myreader("cboname").ToString
                    txtchildfname.Text = myreader("firstname").ToString
                    txtchildmname.Text = myreader("middlename").ToString
                    txtchildsname.Text = myreader("surname").ToString
                    txtchwfname.Text = myreader("chwfname").ToString
                    txtchwmname.Text = myreader("chwmname").ToString
                    txtchwsname.Text = myreader("chwsname").ToString
                    myGender = myreader("gender").ToString

                    If IsCaregiver = True Then
                        myPreviousHIVStatus = "NEGATIVE"
                    Else
                        myPreviousHIVStatus = myreader("hivstatus").ToString.ToUpper
                    End If


                    'alert if child is exited
                    If CBool(myreader("exited")) = True Then
                        MsgBox("Kindly note that this child is has been Exit.", MsgBoxStyle.Information, "Alert")
                        Label21.Text = "Client is EXITED."
                        Label21.ForeColor = Color.OrangeRed

                        Panel1.Enabled = True
                        btnEdit.Enabled = False
                        btnpost.Enabled = False
                    Else
                        Label21.Text = "Client still ACTIVE."
                        Label21.ForeColor = Color.Green

                        Panel1.Enabled = True
                        btnEdit.Enabled = False
                        btnpost.Enabled = True
                    End If

                    'default view is monitoring [both treeviews visible]
                    lnkmonitorstatus.Enabled = True
                    lnkServicemode.Enabled = True
                    visitType = "monitor"
                    AddRootNode("monitor")
                    dtpDateofVisit.Value = Date.Today

                    'dont hide priorities when monitoring
                    Label17.Visible = True
                    lstpriorityListing.Visible = True

                    'dont hide number of visits when monitoring
                    'Label2.Visible = True
                    'txtNumofVisits.Visible = True
                    txtNumofVisits.Text = "0"

                    fillgrid()
                    TabControl1.SelectedIndex = 1

                    Exit Do
                End If



                '' ''load the caretakers details
                ' ''Dim caretaker As String = myreader("caretaker").ToString
                ' ''If caretaker = "Father" Then
                ' ''    myparentid = myreader("fatherid").ToString
                ' ''    'Depending on who caretaker is, load their details
                ' ''    showparent_guardiandetails("Father", myparentid, myOVCID)
                ' ''ElseIf caretaker = "Mother" Then
                ' ''    myparentid = myreader("motherid").ToString
                ' ''    'Depending on who caretaker is, load their details
                ' ''    showparent_guardiandetails("Mother", myparentid, myOVCID)
                ' ''ElseIf caretaker = "Guardian" Then
                ' ''    myparentid = myreader("guardianid").ToString
                ' ''    'Depending on who caretaker is, load their details
                ' ''    showparent_guardiandetails("Guardian", myparentid, myOVCID)
                ' ''End If


                'alert user if child is HIV positive
                If LCase(myreader("hivstatus").ToString) = "positive" Then
                    MsgBox("Kindly note that this child is HIV Positive.", MsgBoxStyle.Information, "Alert")
                    Label20.Text = "Child is HIV Positive."
                    Label20.ForeColor = Color.OrangeRed
                Else
                    Label20.Text = "Child is NOT HIV Positive."
                    Label20.ForeColor = Color.Green
                End If
            Loop

            'pre-select biodata items affected by services, example, HIvstatus and bcert using previous values
            cboHIVStatus.SelectedValue = CheckforHIVStatus(myPreviousHIVStatus)


            chkHasCert.Checked = HasBirthCert





            'fillgrid()
            'TabControl1.SelectedIndex = 1





            '    End If
            'End If

        Catch ex As Exception
            MsgBox(ex.Message)
            ErrorAction.WriteToErrorLogFile("StatusMonitoring", "cellcontentclick", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    'Private Sub showparent_guardiandetails(ByVal mycaretaker As String, ByVal myparent_guardian_id As String, ByVal myOVCID As String)
    '    Dim ErrorAction As New functions
    '    Try
    '        'select item to edit or delete
    '        Dim mysqlaction As String = ""
    '        Dim MyDBAction As New functions

    '        Dim MyDatable As New Data.DataTable


    '        mysqlaction = "select * from vw_servicemonitoring_" & mycaretaker.ToString & " where parentid='" & myparent_guardian_id & _
    '        "' and ovcid = '" & myOVCID.ToString & "'"
    '        MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
    '        If MyDatable.Rows.Count > 0 Then
    '            If MyDatable.Rows.Count > 0 Then
    '                txtparentfname.Text = MyDatable.Rows(0).Item("parentfname").ToString
    '                txtparentmname.Text = MyDatable.Rows(0).Item("parentmname").ToString
    '                txtparentsname.Text = MyDatable.Rows(0).Item("parentsname").ToString
    '                txtCBO.Text = MyDatable.Rows(0).Item("cboname").ToString
    '                txtchildfname.Text = MyDatable.Rows(0).Item("firstname").ToString
    '                txtchildmname.Text = MyDatable.Rows(0).Item("middlename").ToString
    '                txtchildsname.Text = MyDatable.Rows(0).Item("surname").ToString
    '                txtchwfname.Text = MyDatable.Rows(0).Item("chwfname").ToString
    '                txtchwmname.Text = MyDatable.Rows(0).Item("chwmname").ToString
    '                txtchwsname.Text = MyDatable.Rows(0).Item("chwsname").ToString
    '            End If
    '        End If

    '    Catch ex As Exception
    '        ErrorAction.WriteToErrorLogFile("StatusMonitoring", "showparent_guardiandetails", ex.Message) ''---Write error to error log file

    '    End Try
    'End Sub


    Private Sub clearcontrols()
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

    End Sub


    Private Sub btnpost_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnpost.Click
        're-check if systemdate has been adjusted after OLMIS has been started
        'check if the system date has been messed with, by checking currentdate - most recent services date
        Dim MyDBAction As New functions
        'Dim SqlAction As String = "SELECT top 1 dateofvisit from StatusAndServiceVisit order by dateofvisit desc"
        'Dim Mymaxdateofvisit As Date = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

        'If Date.Today < Mymaxdateofvisit Then
        '    MsgBox("Please check your system date", MsgBoxStyle.Exclamation)
        '    'Exit Sub
        'End If

        ''''''Validate so that CAREGIVERs are only assessed/served  on March, June, September and December
        '''''If Me.Text.ToString.ToUpper.Contains("CAREGIVER") AndAlso
        '''''    (Format(dtpDateofVisit.Value, "MMM").ToString.ToUpper <> "MAR" And
        '''''        Format(dtpDateofVisit.Value, "MMM").ToString.ToUpper <> "JUN" And
        '''''        Format(dtpDateofVisit.Value, "MMM").ToString.ToUpper <> "SEP" And
        '''''        Format(dtpDateofVisit.Value, "MMM").ToString.ToUpper <> "DEC") Then
        '''''    MsgBox("Caregivers are only assessed/served in March, June, September and December")
        '''''    Exit Sub
        '''''End If

        'validate before saving
        'if priority list box is visible, then we are monitoring else we are serving
        If lstpriorityListing.Visible = True Then
            If validate_priorities() = True AndAlso validate_servicesstatus() = True AndAlso _
            validate_rules() = True AndAlso validate_monitoring_service(myOVCID) = True AndAlso validatedurations(myOVCID) = True Then
                SaveServiceandMonitoring()
                fillgrid()
                TabControl1.SelectedIndex = 2

                TreeView1.ExpandAll()

                Panel1.Enabled = False
                btnEdit.Enabled = False
                btnpost.Enabled = False
            End If
        ElseIf lstpriorityListing.Visible = False Then
            If validate_servicesstatus() = True AndAlso validate_priorities() = True AndAlso validate_rules() = True AndAlso validate_monitoring_service(myOVCID) = True AndAlso validatedurations(myOVCID) = True Then
                'If validate_rules() = True Then
                SaveServiceandMonitoring()

                ''Longitudinal auto-update of birth-cert and HIV Testing incase they were served
                '' <add key="service_birth_registration" value="BIRTH CERTIFICATE"/>
                ''<add key="hiv_Counseling_testing" value="HIV COUNSELING AND TESTING"/>
                'validateLongitudinalServices(myOVCID)


                fillgrid()
                TabControl1.SelectedIndex = 2

                TreeView1.ExpandAll()

                Panel1.Enabled = False
                btnEdit.Enabled = False
                btnpost.Enabled = False
            End If
        End If


    End Sub

    Private Sub validateLongitudinalServices(ByVal myovcid As String)
        Try
            'initialize form1A longitudinal biodata items
            Longitudinal_bcert = False
            Longitudinal_hivstatus = False

            'first close any open biodata form
            If m_RegistrationFormNumber > 1 Then
                clientchildform.Close()
                m_RegistrationFormNumber = 0
            End If

            'Longitudinal auto-update of birth-cert and HIV Testing incase they were served
            ' <add key="birth_Cert_longitudinalservice" value="BIRTH CERTIFICATE"/>
            '<add key="hiv_Counseling_longitudinalservice" value="HIV COUNSELING AND TESTING"/>
            If find_selectedMonitor(AppSettings("birth_Cert_longitudinalservice").ToString) = True Then
                Longitudinal_bcert = True
            End If
            If find_selectedMonitor(AppSettings("hiv_Counseling_longitudinalservice").ToString) = True Then
                Longitudinal_hivstatus = True
            End If

            If Longitudinal_bcert = True Or Longitudinal_hivstatus = True Then
                Dim formbiodata As New frmClientInfo()
                ' Start message pump by using ShowDialog()
                formbiodata.Text = "Form1A Longitudinal Update"
                formbiodata.ShowDialog()
            End If



        Catch ex As Exception

        End Try
    End Sub


    Private Sub fillgrid()
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            'Dim mySqlAction As String = "select * from StatusandServicevisit " & _
            '" where OVCID = '" & myOVCID.ToString & "' order by CONVERT(date, StatusAndServiceVisit.DateofVisit, 103) asc"
            'Dim MyDBAction As New functions
            'Dim MyDatable As New Data.DataTable
            Dim myvisitid, myvisitdate, myvisittype As String
            'MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            'USE THE APPROPRIATE stored procedure to fetch the history of service visits
            Dim cmd As SqlCommand
            If IsCaregiver = True Then
                cmd = New SqlCommand("dbo.CAREGIVERForm1A_PopulateOVCVisits")
                cmd.Parameters.Add(New SqlParameter("@caregiverid", myOVCID.ToString))
            Else
                cmd = New SqlCommand("dbo.Form1A_PopulateOVCVisits")
                cmd.Parameters.Add(New SqlParameter("@OVCID", myOVCID.ToString))
            End If

            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandTimeout = 300

            ''cmd.Parameters.Add(New SqlParameter("@DateofVisit", dtpDateofVisit.Value))
            conn.Open()
            cmd.Connection = conn

            Dim myreader As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)

            DataGridView2.Rows.Clear()
            'If MyDatable.Rows.Count > 0 Then
            '    For K = 0 To MyDatable.Rows.Count - 1
            Do While myreader.Read
                myvisitid = myreader("ssvid").ToString
                myvisitdate = Format(CDate(myreader("DateofVisit").ToString), "dd-MMM-yyyy")
                myvisittype = myreader("visittype").ToString
                DataGridView2.Rows.Add(myvisitdate, myvisitid, myvisittype, "Select")
            Loop

            '    Next
            'End If
        Catch ex As Exception
            MsgBox(ex.Message)
            ErrorAction.WriteToErrorLogFile("Needs Assessment", "fillgrid", ex.Message) ''---Write error to error log file

        End Try
    End Sub


    Private Function validate_priorities() As Boolean
        Try
            ErrorProvider1.Clear()
            If dtpDateofVisit.Value >= Date.Today Then
                ErrorProvider1.SetError(dtpDateofVisit, "Please select [Date of Visit] to continue")
                MsgBox("Please select some value to continue", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                dtpDateofVisit.Focus()
                Return False
            End If

            'Only save if the month is may or November for OVC
            If (Me.Text.ToString.ToUpper.Contains("OVC") AndAlso
                Format(dtpDateofVisit.Value, "MMM").ToString.ToUpper = "JUL") Or
                 (Me.Text.ToString.ToUpper.Contains("OVC") AndAlso
                Format(dtpDateofVisit.Value, "MMM").ToString.ToUpper = "MAR") Or
                 (Me.Text.ToString.ToUpper.Contains("OVC") AndAlso
                Format(dtpDateofVisit.Value, "MMM").ToString.ToUpper = "NOV") Then

                If Me.Text.ToString.ToUpper.Contains("CAREGIVER") Then 'Caregivers have no Priorities
                    'MsgBox("Please note CAREGIVERS have no Priorities. " & _
                    '      vbCr & "Any priorities selected WILL BE IGNORED", MsgBoxStyle.Exclamation)
                    clear_checked(lstpriorityListing) 'make sure that nothing is saved
                    Return True
                End If

                If lstpriorityListing.Visible = True Then
                    '    'check how many priorities are selected
                    'There are 3 levels on the treeview Domain-Subdomain-Goals
                    Dim MyNode As New TreeNode
                    Dim MyChildNode As New TreeNode
                    Dim servicecount As Integer = 0 'start counter for every subdomain

                    For Each MyNode In lstpriorityListing.Nodes 'Loop thru level1 [Domains]

                        For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [priorities]



                            If MyChildNode.Checked = True Then
                                '-----TO BE UNCOMMENTED ONCE WHEN WE SHOULD NOT SELECT MORE THAN 3 PRIORITIES
                                servicecount = servicecount + 1 'count every selected goal
                                If servicecount > 3 Then 'if more than one selected Priorities, alert user
                                    ErrorProvider1.SetError(lstpriorityListing, "Please select only 3 or less priorities.")
                                    MsgBox("Please select only 3 or less priorities.", MsgBoxStyle.Exclamation)
                                    Return False 'validation not successful
                                End If

                            End If


                        Next

                    Next
                    'if no priority is selected alert user
                    If servicecount = 0 Then 'if there is no goal selected, alert user
                        ErrorProvider1.SetError(lstpriorityListing, "No Priority selected.")
                        MsgBox("Please select Priorities", MsgBoxStyle.Exclamation)
                        Return False 'validation not successful
                    End If
                End If


                Return True
            Else
                'MsgBox("Please note that Priorities are only selected in the month of MAY and NOV " & _
                '       vbCr & "Any priorities selected WILL BE IGNORED", MsgBoxStyle.Exclamation)
                clear_checked(lstpriorityListing)
                Return True
            End If







            If Not IsNumeric(txtNumofVisits.Text) AndAlso visitType = "monitor" Then
                ErrorProvider1.SetError(txtNumofVisits, "Please key-in number of household visits.")
                MsgBox("Please key-in number of household visits.", MsgBoxStyle.Exclamation)
                Return False
            Else
                Return True
            End If
            Return True
        Catch ex As Exception
            MsgBox(ex.Message & "[validatepriorities]")
            Return False
        End Try
    End Function

    Private Function validate_servicesstatus() As Boolean
        Try

            ErrorProvider1.Clear()

            Dim MyNode As New TreeNode
            Dim MyChildNode As New TreeNode
            Dim myservicenode As New TreeNode
            Dim servicecount As Integer = 0 'start counter for every subdomain

            For Each MyNode In TreeView1.Nodes 'Loop thru level1 [Domains]

                For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [services or monitoring]

                    For Each myservicenode In MyChildNode.Nodes
                        If myservicenode.Checked = True Then

                            servicecount = servicecount + 1 'count every service or monitoring

                        End If
                    Next

                Next

            Next
            'if no priority is selected alert user
            If servicecount = 0 Then 'if there is no goal selected, alert user
                ErrorProvider1.SetError(TreeView1, "No services or monitoring selected.")
                MsgBox("Please select services or monitoring", MsgBoxStyle.Exclamation)
                Return False 'validation not successful
            End If

            'make suere that if longitudinal fields are visible, they are not left blank
            If cboHIVStatus.Visible = True AndAlso Trim(cboHIVStatus.Text).Length = 0 Then
                ErrorProvider1.SetError(cboHIVStatus, "Please select HIV Status.")
                MsgBox("Please select HIV Status.", MsgBoxStyle.Exclamation)
                Return False
            End If

            If chkHasCert.Visible = True AndAlso chkHasCert.Checked = False Then
                ErrorProvider1.SetError(chkHasCert, "Please select BirthCert Status.")
                MsgBox("Please select BirthCert Status.", MsgBoxStyle.Exclamation)
                Return False
            End If

            Return True 'validation was successful
        Catch ex As Exception
            MsgBox(ex.Message & "[validateservicestatus]")
            Return False
        End Try
    End Function

    Private Function validate_rules() As Boolean
        Try

            ''Validations for Monitioring---------------------
            ErrorProvider1.Clear()
            'There are 3 levels on the treeview Domain-Subdomain-Goals
            Dim MyNode As New TreeNode
            Dim MyChildNode As New TreeNode
            Dim MyserviceNode As New TreeNode


            For Each MyNode In TreeView1.Nodes 'Loop thru level1 [Domains]

                For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [Core Services]
                    For Each MystatusNode In MyChildNode.Nodes '[services]
                        If MystatusNode.Checked = True Then
                            'split the tooltiptext which is where we have stored all validation fields, separated by [:]
                            '"AgeRequired", "Gender", "Needstobeselectedprior", "Needstobeunselectedprior", "PriorNumberofMonths", "AffectsLongitudinalbiodata"
                            Dim myvalidationsarray As Array = MystatusNode.ToolTipText.Split(":")

                            '1-------validate against age
                            If myvalidationsarray(0) <> 0 Then
                                If myvalidationsarray(0) > myAge Then
                                    MsgBox(MystatusNode.Text & " is only eligible for children " & myvalidationsarray(0).ToString.Trim & " and over.", MsgBoxStyle.Exclamation)
                                    Return False
                                End If
                            End If

                            '2-------validate against gender
                            If Trim(myvalidationsarray(1)).Length <> 0 Then
                                If LCase(myvalidationsarray(1).ToString.Trim) <> LCase(myGender.ToString.Trim) Then
                                    MsgBox(MystatusNode.Text & " is only eligible for children of the " & myvalidationsarray(1).ToString.Trim & " gender.", MsgBoxStyle.Exclamation)
                                    Return False
                                End If
                            End If

                        End If
                    Next

                Next

            Next

            '--------End of monitoring/services validations



            'There are 3 levels on the treeview Domain-Subdomain-Goals
            MyNode = New TreeNode
            MyChildNode = New TreeNode



            For Each MyNode In lstpriorityListing.Nodes 'Loop thru level1 [Domains]

                For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [Core Services]

                    If MyChildNode.Checked = True Then
                        'split the tooltiptext which is where we have stored all validation fields, separated by [:]
                        '"AgeRequired", "Gender", "Needstobeselectedprior", "Needstobeunselectedprior", "PriorNumberofMonths", "AffectsLongitudinalbiodata"
                        Dim myvalidationsarray As Array = MyChildNode.ToolTipText.Split(":")

                        '1-------validate against age
                        If myvalidationsarray(0) <> 0 Then
                            If myvalidationsarray(0) > myAge Then
                                MsgBox(MyChildNode.Text & " is only eligible for children " & myvalidationsarray(0).ToString.Trim & " and over.", MsgBoxStyle.Exclamation)
                                Return False
                            End If
                        End If

                        '2-------validate against gender
                        If Trim(myvalidationsarray(1)).Length <> 0 Then
                            If LCase(myvalidationsarray(1).ToString.Trim) <> LCase(myGender.ToString.Trim) Then
                                MsgBox(MyChildNode.Text & " is only eligible for children of the " & myvalidationsarray(1).ToString.Trim & " gender.", MsgBoxStyle.Exclamation)
                                Return False
                            End If
                        End If

                        '3-------If an ovc already has a birth certificate, then it cannot be a priority

                        '4-------If an ovc is already hiv positive, then testing cannot be a priority

                    End If


                Next

            Next


            '--------End of Priority validations

            Return True

        Catch ex As Exception
            MsgBox(ex.Message & "[validaterules]")
            Return False
        End Try
        'Dim dview As DataRowView
        'For Each dview In lstpriorityListing.CheckedItems
        '    '1. If child has birth certificate during
        '    ' registration one should not be allowed to enter the same in form 1A--------
        '    ' Loop through all the selected items of the listbox and save priorities
        '    If HasBirthCert = True AndAlso UCase(dview.Row.Item(1).ToString) = AppSettings("priority_birth_registration").ToString Then
        '        MsgBox("Child had Birth Certificate during registration.", MsgBoxStyle.Exclamation)
        '        Return False
        '    End If

        '    '---------------------
        '    '2.   Sanitary cannot be given to boys----
        '    If UCase(myGender) = "MALE" AndAlso UCase(dview.Row.Item(1).ToString) = AppSettings("priority_sanitary_pads").ToString Then
        '        MsgBox("Sanitary pads CANNOT be given to boys.", MsgBoxStyle.Exclamation)
        '        Return False
        '    End If
        '    '----------
        '    '3. Shelter cannot be in good condition and renovated------
        '    If UCase(dview.Row.Item(1).ToString) = AppSettings("priority_shelter").ToString Then
        '        If find_selectedMonitor(AppSettings("monitor_shelter").ToString) = True Then
        '            MsgBox("Shelter CANNOT be in good condition and renovated.", MsgBoxStyle.Exclamation)
        '            Return False
        '        End If

        '    End If

        '    '--------------
        'Next
        ''--------End of monitoring validations

        ''Validations for Service----------------------------------
        'If find_selectedMonitor(AppSettings("service_sanitary_pads").ToString) = True AndAlso UCase(myGender) = "MALE" Then
        '    MsgBox("CANNOT serve Boys with sanitary pads.", MsgBoxStyle.Exclamation)
        '    Return False
        'End If

        'If find_selectedMonitor(AppSettings("service_sanitary_pads").ToString) = True AndAlso myAge < 9 Then
        '    MsgBox("CANNOT serve OVC under 9 yrs of age with sanitary pads.", MsgBoxStyle.Exclamation)
        '    Return False
        'End If

        'If HasBirthCert = True AndAlso find_selectedMonitor(AppSettings("service_birth_registration").ToString) Then
        '    MsgBox("Child had Birth Certificate during registration.", MsgBoxStyle.Exclamation)
        '    Return False
        'End If

        ''--------End of Service validations



    End Function

    Private Function find_selectedMonitor(ByVal mymonitorlabel As String) As Boolean
        'There are 3 levels on the treeview 
        Dim MyNode As New TreeNode
        Dim MyChildNode As New TreeNode
        Dim MystatusNode As New TreeNode

        For Each MyNode In TreeView1.Nodes 'Loop thru level1 [Domains]

            For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [Coreservices]

                For Each MystatusNode In MyChildNode.Nodes 'Loop thru level3 [Services and Status]
                    If MystatusNode.Checked = True Then
                        If UCase(MystatusNode.Text.ToString) = UCase(mymonitorlabel.ToString) Then
                            'If UCase(MystatusNode.Text.ToString).Contains(UCase(Replace(mymonitorlabel.ToString, "_AND", "&"))) = True Then
                            Return True
                        End If

                    End If

                Next

            Next
        Next
        Return False
    End Function

    ''Private Function validate_rules() As Boolean
    ''    Try

    ''        ''Validations for Monitioring---------------------
    ''        ErrorProvider1.Clear()
    ''        'There are 3 levels on the treeview Domain-Subdomain-Goals
    ''        Dim MyNode As New TreeNode
    ''        Dim MyChildNode As New TreeNode
    ''        Dim MyserviceNode As New TreeNode

    ''        For Each MyNode In lstpriorityListing.Nodes 'Loop thru level1 [Domains]

    ''            For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [Services]

    ''                'Dim servicecount As Integer = 0 'start counter for every subdomain

    ''                If MyChildNode.Checked = True Then
    ''                    '1. If child has birth certificate during
    ''                    ' registration one should not be allowed to enter the same in form 1A--------
    ''                    ' Loop through all the selected items of the listbox and save priorities
    ''                    If HasBirthCert = True AndAlso UCase(MyChildNode.Text.ToString) = AppSettings("priority_birth_registration").ToString Then
    ''                        MsgBox("Child had Birth Certificate during registration.", MsgBoxStyle.Exclamation)
    ''                        Return False
    ''                    End If

    ''                    '---------------------
    ''                    '2.   Sanitary cannot be given to boys----
    ''                    If UCase(myGender) = "MALE" AndAlso UCase(MyChildNode.Text.ToString) = AppSettings("priority_sanitary_pads").ToString Then
    ''                        MsgBox("Sanitary pads CANNOT be given to boys.", MsgBoxStyle.Exclamation)
    ''                        Return False
    ''                    End If
    ''                    '----------
    ''                    '3. Shelter cannot be in good condition and renovated------
    ''                    If UCase(MyChildNode.Text.ToString) = AppSettings("priority_shelter").ToString Then
    ''                        If find_selectedMonitor(AppSettings("monitor_shelter").ToString) = True Then
    ''                            MsgBox("Shelter CANNOT be in good condition and renovated.", MsgBoxStyle.Exclamation)
    ''                            Return False
    ''                        End If

    ''                    End If

    ''                    '--------------
    ''                End If


    ''            Next

    ''        Next

    ''        '--------End of monitoring validations

    ''        'Validations for Service----------------------------------
    ''        If find_selectedMonitor(AppSettings("service_sanitary_pads").ToString) = True AndAlso UCase(myGender) = "MALE" Then
    ''            MsgBox("CANNOT serve Boys with sanitary pads.", MsgBoxStyle.Exclamation)
    ''            Return False
    ''        End If

    ''        If find_selectedMonitor(AppSettings("service_sanitary_pads").ToString) = True AndAlso myAge < 9 Then
    ''            MsgBox("CANNOT serve OVC under 9 yrs of age with sanitary pads.", MsgBoxStyle.Exclamation)
    ''            Return False
    ''        End If

    ''        If find_selectedMonitor(AppSettings("immunization").ToString) = True AndAlso myAge > 5 Then
    ''            MsgBox("CANNOT serve Immunization to children over 5 years.", MsgBoxStyle.Exclamation)
    ''            Return False
    ''        End If

    ''        If HasBirthCert = True AndAlso find_selectedMonitor(AppSettings("service_birth_registration").ToString) Then
    ''            MsgBox("Child had Birth Certificate during registration.", MsgBoxStyle.Exclamation)
    ''            Return False
    ''        End If

    ''        '--------End of Service validations

    ''        Return True

    ''    Catch ex As Exception
    ''        MsgBox(ex.Message & "[validaterules]")
    ''        Return False
    ''    End Try
    ''    'Dim dview As DataRowView
    ''    'For Each dview In lstpriorityListing.CheckedItems
    ''    '    '1. If child has birth certificate during
    ''    '    ' registration one should not be allowed to enter the same in form 1A--------
    ''    '    ' Loop through all the selected items of the listbox and save priorities
    ''    '    If HasBirthCert = True AndAlso UCase(dview.Row.Item(1).ToString) = AppSettings("priority_birth_registration").ToString Then
    ''    '        MsgBox("Child had Birth Certificate during registration.", MsgBoxStyle.Exclamation)
    ''    '        Return False
    ''    '    End If

    ''    '    '---------------------
    ''    '    '2.   Sanitary cannot be given to boys----
    ''    '    If UCase(myGender) = "MALE" AndAlso UCase(dview.Row.Item(1).ToString) = AppSettings("priority_sanitary_pads").ToString Then
    ''    '        MsgBox("Sanitary pads CANNOT be given to boys.", MsgBoxStyle.Exclamation)
    ''    '        Return False
    ''    '    End If
    ''    '    '----------
    ''    '    '3. Shelter cannot be in good condition and renovated------
    ''    '    If UCase(dview.Row.Item(1).ToString) = AppSettings("priority_shelter").ToString Then
    ''    '        If find_selectedMonitor(AppSettings("monitor_shelter").ToString) = True Then
    ''    '            MsgBox("Shelter CANNOT be in good condition and renovated.", MsgBoxStyle.Exclamation)
    ''    '            Return False
    ''    '        End If

    ''    '    End If

    ''    '    '--------------
    ''    'Next
    ''    ''--------End of monitoring validations

    ''    ''Validations for Service----------------------------------
    ''    'If find_selectedMonitor(AppSettings("service_sanitary_pads").ToString) = True AndAlso UCase(myGender) = "MALE" Then
    ''    '    MsgBox("CANNOT serve Boys with sanitary pads.", MsgBoxStyle.Exclamation)
    ''    '    Return False
    ''    'End If

    ''    'If find_selectedMonitor(AppSettings("service_sanitary_pads").ToString) = True AndAlso myAge < 9 Then
    ''    '    MsgBox("CANNOT serve OVC under 9 yrs of age with sanitary pads.", MsgBoxStyle.Exclamation)
    ''    '    Return False
    ''    'End If

    ''    'If HasBirthCert = True AndAlso find_selectedMonitor(AppSettings("service_birth_registration").ToString) Then
    ''    '    MsgBox("Child had Birth Certificate during registration.", MsgBoxStyle.Exclamation)
    ''    '    Return False
    ''    'End If

    ''    ''--------End of Service validations



    ''End Function

    ''Private Function find_selectedMonitor(ByVal mymonitorlabel As String) As Boolean
    ''    'There are 3 levels on the treeview 
    ''    Dim MyNode As New TreeNode
    ''    Dim MyChildNode As New TreeNode
    ''    Dim MystatusNode As New TreeNode

    ''    For Each MyNode In TreeView1.Nodes 'Loop thru level1 [Domains]

    ''        For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [Coreservices]

    ''            For Each MystatusNode In MyChildNode.Nodes 'Loop thru level3 [Services and Status]
    ''                If MystatusNode.Checked = True Then
    ''                    If UCase(MystatusNode.Text.ToString) = mymonitorlabel.ToString Then
    ''                        Return True
    ''                    End If

    ''                End If

    ''            Next

    ''        Next
    ''    Next
    ''    Return False
    ''End Function

    Private Sub SaveServiceandMonitoring()
        Dim ErrorAction As New functions
        Try
            Dim Myrandomkey As New Random
            Dim MySSVID = "SSVID" & "-" & Format(Date.Now, "yyyyMMddhhmmss") & "-" & Myrandomkey.Next
            Dim MyMainDomainid As String = ""
            Dim Mycoreserviceid As String = ""
            Dim Mystatusid As String = ""
            Dim MyPriorityid As String = ""

            'There are 3 levels on the treeview Domain-Subdomain-Goals
            Dim MyNode As New TreeNode
            Dim MyChildNode As New TreeNode
            Dim MystatusNode As New TreeNode


            For Each MyNode In TreeView1.Nodes 'Loop thru level1 [Domains]
                MyMainDomainid = MyNode.Tag.ToString
                'If MyNode.Nodes.Count > 0 Then

                For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [Coreservices]
                    Mycoreserviceid = MyChildNode.Tag.ToString

                    For Each MystatusNode In MyChildNode.Nodes 'Loop thru level3 [Services and Status]
                        If MystatusNode.Checked = True Then
                            Mystatusid = MystatusNode.Tag.ToString

                            'save to the ServiceandMonitoring table
                            Dim MySSMID As String = "SSMID" & "-" & Format(Date.Now, "yyyyMMddhhmmss") & "-" & Myrandomkey.Next 'unique row id. I could not put
                            InsertToServiceandStatusTable(MySSMID, MySSVID, Format(dtpDateofVisit.Value, "MMM"), Mystatusid)

                            ' ''TO BE UNCOMMENTED ONCE WE DECIDE TO ONLY ALLOW ONE ITEM PER SUBDOMAIN TO BE SAVED
                            ''Exit For 'Only one goal needs to be selected. So the first selection should be the only one
                        End If
                        'Threading.Thread.Sleep(100) 'if u dont put a wait, there is unique key violation in DB when saving
                    Next

                Next

            Next

            'Save details about the visit
            If visitType = "monitor" Then
                InsertToServiceandStatusVisitTable(MySSVID, myOVCID, mychw, Format(dtpDateofVisit.Value, "dd-MMM-yyyy"), visitType, CInt(txtNumofVisits.Text.ToString))

                'save to the ServiceandMonitoringVisit table
                'confirm if its monitor or serve. For serve, there is no priority saved
                For Each MyNode In lstpriorityListing.Nodes 'Loop thru level1 [Domains]
                    MyMainDomainid = MyNode.Tag.ToString
                    'If MyNode.Nodes.Count > 0 Then

                    For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [Services]
                        Dim MyPMID As String = "PMID" & "-" & Format(Date.Now, "yyyyMMddhhmmss") & "-" & Myrandomkey.Next 'unique row id. I could not put
                        If MyChildNode.Checked = True Then
                            MyPriorityid = MyChildNode.Tag.ToString

                            'save to the servicesprovided table
                            Dim MyPriorityNeedsRowID As String = "PRTID" & "-" & Format(Date.Now, "yyyyMMddhhmmss") & "-" & Myrandomkey.Next 'unique row id. I could not put
                            InsertToPriorityMonitoringTable(MyPMID, MySSVID, MyPriorityid)
                        End If
                    Next

                Next

                'save critical events related to this particular visit
                ' Loop through all the selected items of the listbox and save priorities
                Dim dview As DataRowView
                For Each dview In lstCriticalEvents.CheckedItems
                    Dim MyCriticalEventID As String = "CEID" & "-" & Format(Date.Now, "yyyyMMddhhmmss") & "-" & Myrandomkey.Next 'unique row id. I could not put
                    'Its now dview.row.item(5) coz we using vw_domain_services and priorityid/ssid is field 5
                    'InsertToPriorityMonitoringTable(MyPMID, MySSVID, dview.Row.Item(0).ToString)
                    InsertToCriticalEventVisitTable(MyCriticalEventID, MySSVID, dview.Row.Item(0).ToString)
                    'Threading.Thread.Sleep(100) 'if u dont put a wait, there is unique key violation in DB when saving
                Next
            ElseIf visitType = "service" Then

                InsertToServiceandStatusVisitTable(MySSVID, myOVCID, mychw, Format(dtpDateofVisit.Value, "dd-MMM-yyyy"), visitType)

                'Do this only if the biodata controls are visible
                If cboHIVStatus.Visible = True Or chkHasCert.Visible = True Then

                    'Save Longitudinal data if any
                    Dim myhivstatus As Int32 = 0
                    Dim mybcert As Boolean = 0

                    'If myPreviousHIVStatus.ToLower = "not known" Then
                    '    myhivstatus = CheckforHIVStatus(myPreviousHIVStatus)
                    'Else
                    myhivstatus = cboHIVStatus.SelectedValue.ToString
                    'End If




                    mybcert = chkHasCert.Checked


                    'only 
                    Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
                    Dim cmd As SqlCommand

                    cmd = New SqlCommand("dbo.Update_Client_ViaServices")


                    cmd.CommandType = CommandType.StoredProcedure
                    cmd.CommandTimeout = 30000
                    cmd.Parameters.Add(New SqlParameter("@OVCID", myOVCID.ToString))
                    cmd.Parameters.Add(New SqlParameter("@BirthCert", mybcert))
                    cmd.Parameters.Add(New SqlParameter("@HIVStatus", myhivstatus))
                    cmd.Parameters.Add(New SqlParameter("@lastmodifiedon", Date.Today))
                    cmd.Parameters.Add(New SqlParameter("@lastmodifiedsession", strsession.ToString))

                    conn.Open()
                    cmd.Connection = conn

                    cmd.ExecuteNonQuery()
                    conn.Close()

                End If
            End If





            ' ''If lstPriorityListing.Visible = True Then
            ' ''    ' Loop through all the selected items of the listbox and save priorities
            ' ''    Dim dview As DataRowView
            ' ''    For Each dview In lstPriorityListing.CheckedItems
            ' ''        Dim MyPMID As String = "PMID" & "-" & Format(Date.Now, "yyyyMMddhhmmss") & "-" & Myrandomkey.Next 'unique row id. I could not put
            ' ''        'Its now dview.row.item(5) coz we using vw_domain_services and priorityid/ssid is field 5
            ' ''        'InsertToPriorityMonitoringTable(MyPMID, MySSVID, dview.Row.Item(0).ToString)
            ' ''        InsertToPriorityMonitoringTable(MyPMID, MySSVID, dview.Row.Item(5).ToString)
            ' ''        'Threading.Thread.Sleep(100) 'if u dont put a wait, there is unique key violation in DB when saving
            ' ''    Next
            ' ''End If

            MsgBox("Record saved successfully.", MsgBoxStyle.Information)

        Catch ex As Exception
            MsgBox(ex.Message)
            ErrorAction.WriteToErrorLogFile("Needs Assessement", "Save", ex.Message) ''---Write error to error log file
            MsgBox("Record NOT saved successfully.", MsgBoxStyle.Exclamation)

        End Try
    End Sub

    Private Sub InsertToCriticalEventVisitTable(ByVal MyCriticalEventID As String, ByVal MySSVID As String, _
                                                ByVal CriticalEvent As String)
        Dim ErrorAction As New functions

        'Try
        'save record
        Dim mySqlAction As String = ""

        Dim MyDBAction As New functions

        If IsCaregiver = True Then
            mySqlAction = "Insert Into CareGiverVisitsCriticalEvents(CEID,SSVID,CriticalEventID) " & _
      " values('" & MyCriticalEventID.ToString & "','" & MySSVID.ToString & "','" & CriticalEvent & "')"
        Else
            mySqlAction = "Insert Into OVCVisitsCriticalEvents(CEID,SSVID,CriticalEventID) " & _
       " values('" & MyCriticalEventID.ToString & "','" & MySSVID.ToString & "','" & CriticalEvent & "')"
        End If


        MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)



        'Catch ex As Exception
        '    ErrorAction.WriteToErrorLogFile("Needs Assessement", "save goals", ex.Message) ''---Write error to error log file
        '    MsgBox("Record NOT saved successfully.", MsgBoxStyle.Exclamation)

        'End Try
    End Sub


    Private Function validatedurations(ByVal myovcid As String) As Boolean
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        Try



            Dim cmd As New SqlCommand("dbo.Validate_Durations")
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandTimeout = 600000
            cmd.Parameters.Add(New SqlParameter("@OVCID", myovcid.ToString))
            cmd.Parameters.Add(New SqlParameter("@DateofVisit", dtpDateofVisit.Value))
            conn.Open()
            cmd.Connection = conn

            Dim myreader As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)

            Dim duration As Integer = 0
            Dim myssidsList As New ArrayList
            Dim myservicestatusList As New ArrayList

            Do While myreader.Read
                For Each MyNode In TreeView1.Nodes 'Loop thru level1 [Domains]

                    For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [Coreservices]

                        For Each MystatusNode In MyChildNode.Nodes 'Loop thru level3 [Services and Status]
                            If MystatusNode.Checked = True Then
                                If MystatusNode.Tag.ToString = myreader("ssid").ToString Then

                                    'split the tooltiptext which is where we have stored all validation fields, separated by [:]
                                    '"AgeRequired", "Gender", "Needstobeselectedprior", "Needstobeunselectedprior", "PriorNumberofMonths", "AffectsLongitudinalbiodata"
                                    Dim myvalidationsarray As Array = MystatusNode.ToolTipText.Split(":")

                                    '1. validate so that services with durations defined e.g 3months, 1 year are adhered to--
                                    duration = DateDiff(DateInterval.Month, CDate(myreader("dateofvisit").ToString), dtpDateofVisit.Value)
                                    'Apparently, sometimes new forms are entered before the older forms and this poses issues on durations for
                                    'a. Monitoring with default [0] durations b. soft services with [0] durations
                                    'abs(duration) converts negative durations to positive
                                    If (CInt(myreader("duration").ToString) <> 0 Or LCase(myreader("visittype").ToString) <> "monitor") AndAlso CBool(myreader("procurable")) = True Then
                                        If (Abs(duration) < CInt(myreader("duration").ToString)) AndAlso myreader("visittype").ToString.ToLower = "service" Then
                                            MsgBox(myreader("servicestatus").ToString & " was served last to this OVC on " &
                                                   Format(CDate(myreader("dateofvisit").ToString), "dd-MMM-yyyy"), MsgBoxStyle.Exclamation, "Validation")
                                            If conn.State <> ConnectionState.Closed Then conn.Close()
                                            myreader.Close()
                                            Return False
                                        End If

                                    End If
                                    'End of 1. -----

                                    '''             '2. 'For tangible commodities, the system should cross check with 
                                    '''             'the OVC priority needs for the past 12 months. 
                                    '''             'If the record does not show priority need for a certain commodity, 0
                                    '''             'it should not allow the OVC to be served-------------------
                                    '''             If (CInt(myreader("duration").ToString) > 0 AndAlso CInt(myreader("duration").ToString) <= 12) _
                                    '''             AndAlso LCase(myreader("visittype").ToString) = "monitor" AndAlso CBool(myreader("procurable")) = True Then
                                    '''                 MsgBox("[" & myreader("servicestatus").ToString & "] has NOT been a priority for the last 12 months. " &
                                    '''"CANNOT serve OVC with this Service.", MsgBoxStyle.Exclamation, "Validation")
                                    '''                 If conn.State <> ConnectionState.Closed Then conn.Close()
                                    '''                 myreader.Close()
                                    '''                 Return False

                                    '''             End If

                                    'End of 2. -----

                                    '3. 'validate so that client is only monitored or served once a month
                                    duration = DateDiff(DateInterval.Month, CDate(myreader("dateofvisit").ToString), dtpDateofVisit.Value)
                                    If (Abs(duration) = 0 AndAlso myreader("visittype").ToString = "monitor" _
                                        AndAlso lstpriorityListing.Visible = True) Then 'if this is a monitor being saved, check for monitor
                                        MsgBox("Last visit was on " & Format(CDate(myreader("dateofvisit").ToString), "dd-MMM-yyyy") _
                                               & ". Can NOT monitor child this month.", MsgBoxStyle.Exclamation, "Validation")
                                        If conn.State <> ConnectionState.Closed Then conn.Close()
                                        myreader.Close()
                                        Return False
                                    ElseIf (Abs(duration) = 0 AndAlso myreader("visittype").ToString = "service" _
                                            AndAlso lstpriorityListing.Visible = False) Then 'if this is a service being saved, check for service
                                        MsgBox("Last visit was on " & Format(CDate(myreader("dateofvisit").ToString), "dd-MMM-yyyy") _
                                                & ". Can NOT serve child this month.", MsgBoxStyle.Exclamation, "Validation")
                                        If conn.State <> ConnectionState.Closed Then conn.Close()
                                        myreader.Close()
                                        Return False

                                    End If

                                    'End of 3. -----

                                End If
                            End If
                        Next

                    Next

                Next

                'Collect a list of ssids to compare items that needed to be selected or unselected previously
                myssidsList.Add(myreader("ssid").ToString.ToLower)

            Loop

            'If the service is not in the list of earlier assessments, then there was no priority need and cannot serve
            For Each MyNode In TreeView1.Nodes 'Loop thru level1 [Domains]

                For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [Coreservices]

                    For Each MystatusNode In MyChildNode.nodes 'Loop thru level3 [Services and Status]
                        Dim myssidlookupsarray As Array = LookupSSID(LCase(MystatusNode.Tag.ToString.ToLower)).Split(":")

                        '2. 'For tangible commodities, the system should cross check with 
                        'the OVC priority needs for the past 12 months. 
                        'If the record does not show priority need for a certain commodity, 0
                        'it should not allow the OVC to be served-------------------

                        'Confirm that from the list of ssids from validate_durations stored procedure, 
                        'that whatever we are serving has a corresponding need
                        Dim priorityexists As Boolean = False
                        If MystatusNode.checked = True Then
                            Dim FindThisString As String = myssidlookupsarray(0).ToString.ToLower

                            For Each Str As String In myssidsList
                                'confirm if priority exists, but for sanitary pads, no need for priority need
                                'also for Form1B Caregiver Services need no priorities
                                If LookupSSID(Str).ToLower.Contains(FindThisString) _
                                    Or FindThisString.ToString.ToLower.Contains("sanitary") Then
                                    priorityexists = True
                                End If
                            Next
                        End If


                        If MystatusNode.checked = True AndAlso priorityexists = False _
                            AndAlso visitType = "service" AndAlso CInt(myssidlookupsarray(1).ToString) = 1 _
                            AndAlso Me.Text.ToUpper.Contains("CAREGIVER") = False Then
                            MsgBox("[" & myssidlookupsarray(0).ToString &
                                                    "] has NOT been a priority for the last 12 months. " &
                            "CANNOT serve OVC with this Service.", MsgBoxStyle.Exclamation, "Validation")
                            Return False
                        ElseIf MystatusNode.checked = True AndAlso myssidlookupsarray(0).ToString.ToLower.Contains("birth") AndAlso
                            HasBirthCert = True Then
                            '---Checkif child already has birth certificate, no need to assess on the same
                            MsgBox("OVC already has a birth certificate. Cannot assess or serve OVC with birth certificate .",
                                   MsgBoxStyle.Exclamation, "Validation")
                            Return False

                        ElseIf MystatusNode.checked = True AndAlso myssidlookupsarray(0).ToString.ToLower.Contains("hiv") = True _
                            AndAlso myssidlookupsarray(0).ToString.Trim.ToLower.Contains("hiv+") = False AndAlso
                            LCase(myPreviousHIVStatus.ToString) = "positive" Then
                            '---Check if child is already hiv positive, no need to test again
                            MsgBox("OVC already is already HIV positive.",
                                   MsgBoxStyle.Exclamation, "Validation")
                            Return False

                        End If
                    Next
                Next
            Next


            'Loop through the treeview to find if selected items on treeview need prior selection or unselection
            Dim priorselectionmatch As Boolean = False
            Dim priorUNselectionmatch As Boolean = False
            Dim priorSelectedservicestatus As String = ""
            Dim priorUnSelectedservicestatus As String = ""
            Dim currentDeniedSelection As String = ""
            Dim mypriorselectedarray As Array = Nothing
            Dim mypriorUNselectedarray As Array = Nothing
            For Each MyNode In TreeView1.Nodes 'Loop thru level1 [Domains]

                For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [Coreservices]

                    For Each MystatusNode In MyChildNode.Nodes 'Loop thru level3 [Services and Status]
                        If MystatusNode.Checked = True Then

                            'split the tooltiptext which is where we have stored all validation fields, separated by [:]
                            '"AgeRequired", "Gender", "Needstobeselectedprior", "Needstobeunselectedprior", "PriorNumberofMonths", "AffectsLongitudinalbiodata"
                            Dim myvalidationsarray As Array = MystatusNode.ToolTipText.Split(":")

                            If Trim(myvalidationsarray(2)).Length <> 0 Then 'if there is a previous assessemnt that needs to have been selected prior
                                'Loop through myssids to see if there is a match. If no match, then alert user that there is a prior assessemnt that needed to have been selected
                                'Dim i As Int16

                                For i = 0 To myssidsList.Count - 1
                                    'split this since there might be multiple items
                                    mypriorselectedarray = myvalidationsarray(2).ToString.Split("$")

                                    For x = 0 To mypriorselectedarray.Length - 1
                                        If LCase(mypriorselectedarray(x)).ToString = LCase(myssidsList(i).ToString) Then
                                            priorselectionmatch = True
                                        End If

                                    Next x

                                Next i
                                'priorSelectedservicestatus = myvalidationsarray(2).ToString
                                currentDeniedSelection = MystatusNode.Text.ToString
                            End If

                            If Trim(myvalidationsarray(3)).Length <> 0 Then
                                For i = 0 To myssidsList.Count - 1

                                    'split this since there might be multiple items
                                    mypriorUNselectedarray = myvalidationsarray(3).ToString.Split("$")

                                    For x = 0 To mypriorUNselectedarray.Length - 1
                                        If LCase(mypriorUNselectedarray(x)).ToString = LCase(myssidsList(i).ToString) Then
                                            priorUNselectionmatch = True
                                            'priorUnSelectedservicestatus = LCase(mypriorUNselectedarray(x)).ToString
                                            currentDeniedSelection = MystatusNode.Text.ToString
                                        End If

                                    Next x

                                Next i
                            End If
                        End If
                    Next
                Next
            Next
            If conn.State <> ConnectionState.Closed Then conn.Close()

            If mypriorselectedarray Is Nothing = False Then
                If priorselectionmatch = False AndAlso mypriorselectedarray.Length <> 0 Then
                    'create a list of services from the arrays 
                    For x = 0 To mypriorselectedarray.Length - 1
                        priorSelectedservicestatus = priorSelectedservicestatus & "|" & LookupSSID(LCase(mypriorselectedarray(x)).ToString)
                    Next x

                    MsgBox("[" & priorSelectedservicestatus.ToString & "] needs to be previously selected" & Chr(13) & _
                           " before selecting [" & currentDeniedSelection.ToString & "]", MsgBoxStyle.Exclamation)
                    Return False
                End If
            End If

            If mypriorUNselectedarray Is Nothing = False Then
                If priorUNselectionmatch = True AndAlso mypriorUNselectedarray.Length <> 0 Then
                    'create a list of services from the arrays 
                    For x = 0 To mypriorUNselectedarray.Length - 1
                        priorUnSelectedservicestatus = priorUnSelectedservicestatus & "|" & LookupSSID(LCase(mypriorUNselectedarray(x)).ToString)
                    Next x

                    MsgBox("[" & priorUnSelectedservicestatus.ToString & "] needs to be previously unselected" & Chr(13) & _
                           " before selecting [" & currentDeniedSelection.ToString & "]", MsgBoxStyle.Exclamation)
                    Return False
                End If
            End If






            Return True




            '' ''validate so that child cannot be served before monitoring
            ' ''If MyDatable.Rows.Count > 0 Then
            ' ''    If visitType = "service" AndAlso MyDatable.Rows(0).Item("visittype").ToString <> "monitor" Then
            ' ''        MsgBox("CANNOT Serve child before monitoring", MsgBoxStyle.Exclamation, "Validation")
            ' ''        Return False
            ' ''    End If
            ' ''Else 'this caters for the first record in the db.First record shoudl be a monitoring record
            ' ''    If visitType = "service" AndAlso MyDatable.Rows.Count = 0 Then
            ' ''        MsgBox("CANNOT Serve child before monitoring", MsgBoxStyle.Exclamation, "Validation")
            ' ''        Return False
            ' ''    End If
            ' ''End If
            '-------------------------------------------------------------------------------------

        Catch ex As Exception
            MsgBox(ex.Message & "[validatedurations]")
            If conn.State <> ConnectionState.Closed Then conn.Close()
            Return False

        End Try
    End Function

    Private Function LookupSSID(ByVal myssid As String) As String
        Dim MyDBAction As New functions
        Dim SqlAction As String = "SELECT servicestatus + ':' + CAST(procurable AS varchar(12))procurable from servicestatus where ssid = '" & myssid.ToString & "'"
        Return MyDBAction.DBAction(SqlAction, DBActionType.Scalar)
    End Function

    'Private Function validatedurations(ByVal myserviceid As String, ByVal myovcid As String, _
    '                                   ByVal myservicename As String) As Boolean
    '    Dim conn As New SqlConnection(ConnectionStrings( SelectedConnectionString  ).ToString)
    '    Try
    '        '----------TO BE UNCOMMENTED ONCE OLD DATA IS FULLY ENTERED INTO SYSTEM----------
    '        Dim mySqlAction As String = "select top 1 * from vw_ssm_ssv " & _
    '            " where OVCID = '" & myovcid.ToString & "' and ssid = '" & myserviceid & "' order by CONVERT(date, DateofVisit, 103) desc" ' descending so that
    '        Dim MyDBAction As New functions
    '        Dim MyDatable As New Data.DataTable
    '        Dim duration As Int16
    '        'MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

    '        Dim cmd As New SqlCommand(mySqlAction, conn)
    '        conn.Open()
    '        Dim myreader As SqlDataReader = cmd.ExecuteReader()


    '        'validate so that services with durations defined e.g 3months, 1 year are adhered to
    '        'If MyDatable.Rows.Count > 0 Then
    '        Do While myreader.Read

    '            duration = DateDiff(DateInterval.Month, CDate(myreader("dateofvisit").ToString), dtpDateofVisit.Value)
    '            'Apparently, sometimes new forms are entered before the older forms and this poses issues on durations for
    '            'a. Monitoring with default [0] durations b. soft services with [0] durations
    '            'abs(duration) converts negative durations to positive
    '            If CInt(myreader("duration").ToString) <> 0 Or LCase(myreader("visittype").ToString) <> "monitor" Then
    '                If Abs(duration) < CInt(myreader("duration").ToString) Then
    '                    MsgBox(myreader("servicestatus").ToString & " was served last to this OVC on " & _
    '                           Format(CDate(myreader("dateofvisit").ToString), "dd-MMM-yyyy"), MsgBoxStyle.Exclamation, "Validation")
    '                    Return False
    '                End If
    '            End If
    '        Loop
    '        conn.Close()
    '        'End If

    '        'For tangible commodities, the system should cross check with 
    '        'the OVC priority needs for the past 12 months. 
    '        'If the record does not show priority need for a certain commodity, 
    '        'it should not allow the OVC to be served
    '        mySqlAction = "SELECT StatusAndServiceVisit.SSVID, StatusAndServiceVisit.OVCID, CONVERT(date, StatusAndServiceVisit.DateofVisit, 103) AS DateofVisit, " & _
    '         " ServiceStatus.SSID, ServiceStatus.ServiceStatus, ISNULL(ServiceStatus.Procurable, 'False') as Procurable , " & _
    '        " DATEDIFF(MONTH, CONVERT(date, StatusAndServiceVisit.DateofVisit, 103), '" & Format(CDate(dtpDateofVisit.Value), "dd-MMM-yyyy") & "' ) as MonthsSinceMonitor " & _
    '        " FROM statusAndServiceVisit INNER JOIN " & _
    '        " PriorityMonitoring ON StatusAndServiceVisit.SSVID = PriorityMonitoring.SSVID INNER JOIN " & _
    '        " ServiceStatus ON PriorityMonitoring.PriorityID = ServiceStatus.SSID " & _
    '        " WHERE OVCID = '" & myovcid.ToString & "' AND SSID = '" & myserviceid.ToString & "' AND Procurable = 'True' AND " & _
    '        " DATEDIFF(MONTH, CONVERT(date, StatusAndServiceVisit.DateofVisit, 103),'" & Format(CDate(dtpDateofVisit.Value), "dd-MMM-yyyy") & "' ) BETWEEN 1 AND 12"
    '        MyDatable = New Data.DataTable
    '        'MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

    '        cmd = New SqlCommand(mySqlAction, conn)
    '        conn.Open()
    '        myreader = cmd.ExecuteReader()

    '        If Myreader.HasRows OrElse IsProcurable(myserviceid.ToString) = False Then
    '            'Then nothing. If we return true, the below code will not run
    '            'Return True
    '        ElseIf IsProcurable(myserviceid.ToString) = True Then
    '            MsgBox("[" & myservicename.ToString & "] has NOT been a priority for the last 12 months. " & _
    '                   "CANNOT serve OVC with this Service.", MsgBoxStyle.Exclamation, "Validation")
    '            Return False
    '        End If
    '        conn.Close()




    '        'validate so that client is only monitored or served once a month
    '        mySqlAction = "select * from vw_ssm_ssv " & _
    '            " where OVCID = '" & myovcid.ToString & "'  order by Dateofvisit desc"

    '        cmd = New SqlCommand(mySqlAction, conn)
    '        conn.Open()
    '        myreader = cmd.ExecuteReader()
    '        ' MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

    '        'If MyDatable.Rows.Count > 0 Then
    '        Do While myreader.Read
    '            duration = DateDiff(DateInterval.Month, CDate(myreader("dateofvisit").ToString), dtpDateofVisit.Value)
    '            If (Abs(duration) = 0 AndAlso myreader("visittype").ToString = "monitor" AndAlso lstpriorityListing.Visible = True) Then 'if this is a monitor being saved, check for monitor
    '                MsgBox("Last visit was on " & Format(CDate(myreader("dateofvisit").ToString), "dd-MMM-yyyy") _
    '                       & ". Can NOT monitor child this month.", MsgBoxStyle.Exclamation, "Validation")
    '                Return False
    '            ElseIf (Abs(duration) = 0 AndAlso myreader("visittype").ToString = "service" AndAlso lstpriorityListing.Visible = False) Then 'if this is a service being saved, check for service
    '                MsgBox("Last visit was on " & Format(CDate(myreader("dateofvisit").ToString), "dd-MMM-yyyy") _
    '                       & ". Can NOT serve child this month.", MsgBoxStyle.Exclamation, "Validation")
    '                Return False

    '            End If
    '        Loop
    '        'End If

    '        '----the system should allow entry of Service form without monitor ------
    '        '' ''validate so that child cannot be served before monitoring
    '        ' ''If MyDatable.Rows.Count > 0 Then
    '        ' ''    If visitType = "service" AndAlso MyDatable.Rows(0).Item("visittype").ToString <> "monitor" Then
    '        ' ''        MsgBox("CANNOT Serve child before monitoring", MsgBoxStyle.Exclamation, "Validation")
    '        ' ''        Return False
    '        ' ''    End If
    '        ' ''Else 'this caters for the first record in the db.First record shoudl be a monitoring record
    '        ' ''    If visitType = "service" AndAlso MyDatable.Rows.Count = 0 Then
    '        ' ''        MsgBox("CANNOT Serve child before monitoring", MsgBoxStyle.Exclamation, "Validation")
    '        ' ''        Return False
    '        ' ''    End If
    '        ' ''End If
    '        '-------------------------------------------------------------------------------------
    '        Return True
    '    Catch ex As Exception
    '        MsgBox(ex.Message)
    '        Return False
    '    Finally
    '        conn.Close()
    '    End Try
    'End Function

    'Private Function IsProcurable(ByVal myserviceid As String) As Boolean
    '    Dim MyDBAction As New functions
    '    Dim SqlAction As String = "SELECT ISNULL(ServiceStatus.Procurable, 'False') as Procurable from servicestatus where ssid = '" & myserviceid.ToString & "'"
    '    Return CBool(MyDBAction.DBAction(SqlAction, DBActionType.Scalar))
    'End Function

    Private Function validate_monitoring_service(ByVal myovcid As String) As Boolean
        Try
            'Validate so that data should for previous month should be entered by 10th of the following month

            If (DateDiff(DateInterval.Day, CDate(strdateofExtension.ToString), Date.Today) <= CInt(strextensionexpiry.ToString)) Then
                'skip the rest since backlog extension is enabled

                Return True
            End If

            'Validate so that data should for previous month should be entered by 10th of the following month
            If DateDiff(DateInterval.Month, dtpDateofVisit.Value, Date.Today) > 0 _
             AndAlso Format(Date.Today, "dd") > CInt(strdatadefault.ToString) Then
                ErrorProvider1.SetError(dtpDateofVisit, "Please select [Date of Visit] to continue")
                MsgBox("Previous month's Form1As should be done by [date " & CInt(strdatadefault.ToString) & "] of the following month.", MsgBoxStyle.Exclamation, Me.Text)
                dtpDateofVisit.Focus()
                Return False
            End If

            ' ''If lstpriorityListing.Visible = False Then

            ' ''End If
            '' ''----------TO BE UNCOMMENTED ONCE OLD DATA IS FULLY ENTERED INTO SYSTEM----------
            ' ''Dim mySqlAction As String = "select * from vw_ssm_ssv " & _
            ' ''    " where OVCID = '" & myovcid.ToString & "'" ' order by Dateofvisit desc" ' descending so that
            ' ''Dim MyDBAction As New functions
            ' ''Dim MyDatable As New Data.DataTable
            ' ''Dim duration As Int16 = 0
            ' ''MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)



            'validate so that client is only monitored once a month
            '' '' ''mySqlAction = "select * from vw_ssm_ssv " & _
            '' '' ''    " where OVCID = '" & myovcid.ToString & "'  order by Dateofvisit desc"
            '' '' ''MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            'If MyDatable.Rows.Count > 0 Then
            '    duration = DateDiff(DateInterval.Month, CDate(MyDatable.Rows(0).Item("dateofvisit").ToString), dtpDateofVisit.Value)
            '    If (Abs(duration) = 0 AndAlso MyDatable.Rows(0).Item("visittype").ToString = "monitor" Then
            '        MsgBox("Last visit was on " & Format(CDate(MyDatable.Rows(0).Item("dateofvisit").ToString), "dd-MMM-yyyy") _
            '               & ". Can NOT monitor child this month.", MsgBoxStyle.Exclamation, "Validation")
            '        Return False
            '    End If
            'End If

            '----the system should allow entry of Service form without monitor ------
            '' ''validate so that child cannot be served before monitoring
            ' ''If MyDatable.Rows.Count > 0 Then
            ' ''    If visitType = "service" AndAlso MyDatable.Rows(0).Item("visittype").ToString <> "monitor" Then
            ' ''        MsgBox("CANNOT Serve child before monitoring", MsgBoxStyle.Exclamation, "Validation")
            ' ''        Return False
            ' ''    End If
            ' ''Else 'this caters for the first record in the db.First record shoudl be a monitoring record
            ' ''    If visitType = "service" AndAlso MyDatable.Rows.Count = 0 Then
            ' ''        MsgBox("CANNOT Serve child before monitoring", MsgBoxStyle.Exclamation, "Validation")
            ' ''        Return False
            ' ''    End If
            ' ''End If
            '-------------------------------------------------------------------------------------
            Return True
        Catch ex As Exception
            MsgBox(ex.Message & "[validate_monitoringservice]")
            Return False
        End Try
    End Function

    Private Sub InsertToServiceandStatusTable(ByVal MySSMID As String, ByVal MySSVID As String, ByVal myMonthofVisit As String, ByVal Mystatusid As String)
        Dim ErrorAction As New functions

        Try
            'save record
            Dim mySqlAction As String = ""

            Dim MyDBAction As New functions

            If visitType = "monitor" Then
                If IsCaregiver = True Then
                    mySqlAction = "Insert Into CAREGIVER_StatusAndServiceMonitoring_Assessment(SSMID,SSVID,Month,SSID) " & _
                " values('" & MySSMID.ToString & "','" & MySSVID.ToString & "','" & myMonthofVisit & "','" & Mystatusid.ToString & "')"
                Else
                    mySqlAction = "Insert Into StatusAndServiceMonitoring_Assessment(SSMID,SSVID,Month,SSID) " & _
                " values('" & MySSMID.ToString & "','" & MySSVID.ToString & "','" & myMonthofVisit & "','" & Mystatusid.ToString & "')"
                End If
            ElseIf visitType = "service" Then
                If IsCaregiver = True Then
                    mySqlAction = "Insert Into CAREGIVER_StatusAndServiceMonitoring_Service(SSMID,SSVID,Month,SSID) " & _
                " values('" & MySSMID.ToString & "','" & MySSVID.ToString & "','" & myMonthofVisit & "','" & Mystatusid.ToString & "')"
                Else
                    mySqlAction = "Insert Into StatusAndServiceMonitoring_Service(SSMID,SSVID,Month,SSID) " & _
                " values('" & MySSMID.ToString & "','" & MySSVID.ToString & "','" & myMonthofVisit & "','" & Mystatusid.ToString & "')"
                End If
            End If
           


            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)



        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("StatusandServiceMonitoring", "save to status table", ex.Message) ''---Write error to error log file
            MsgBox("Record NOT saved successfully.", MsgBoxStyle.Exclamation)

        End Try
    End Sub

    Private Sub InsertToServiceandStatusVisitTable(ByVal MySSVID As String, ByVal myOVCID As String, _
      ByVal mychw As String, ByVal myDateofVisit As Date, ByVal myvisittype As String, Optional ByVal myhouseholdvisits As Integer = 0)
        Dim ErrorAction As New functions

        Try
            'save record
            Dim mySqlAction As String = ""

            Dim MyDBAction As New functions

            If IsCaregiver = True Then
                mySqlAction = "Insert Into CAREGIVER_StatusAndServiceVisit(SSVID,CAREGIVERid,DateofVisit,visittype,HouseHoldVisits) " & _
           " values('" & MySSVID.ToString & "','" & myOVCID.ToString & "','" & Format(myDateofVisit, "dd-MMM-yyyy") & _
           "','" & myvisittype.ToString & "'," & myhouseholdvisits & ")"
            Else
                mySqlAction = "Insert Into StatusAndServiceVisit(SSVID,OVCID,DateofVisit,visittype,HouseHoldVisits) " & _
           " values('" & MySSVID.ToString & "','" & myOVCID.ToString & "','" & Format(myDateofVisit, "dd-MMM-yyyy") & _
           "','" & myvisittype.ToString & "'," & myhouseholdvisits & ")"
            End If


            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)



        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("StatusandServiceMonitoring", "save Visits", ex.Message) ''---Write error to error log file
            MsgBox("Record NOT saved successfully.", MsgBoxStyle.Exclamation)

        End Try
    End Sub

    Private Sub InsertToPriorityMonitoringTable(ByVal myprioritymonitoringid As String, ByVal MySSVID As String, _
                                                ByVal mypriorityID As String)
        Dim ErrorAction As New functions

        Try
            'save record
            Dim mySqlAction As String = ""

            Dim MyDBAction As New functions
            If IsCaregiver = True Then
                mySqlAction = "Insert Into CAREGIVER_PriorityMonitoring(prioritymonitoringid,SSVID,priorityid) " & _
           " values('" & myprioritymonitoringid.ToString & "','" & MySSVID.ToString & "','" & mypriorityID & "')"
            Else
                mySqlAction = "Insert Into PriorityMonitoring(prioritymonitoringid,SSVID,priorityid) " & _
                       " values('" & myprioritymonitoringid.ToString & "','" & MySSVID.ToString & "','" & mypriorityID & "')"
            End If


            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)



        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("StatusandServiceMonitoring", "save goals", ex.Message) ''---Write error to error log file
            MsgBox("Record NOT saved successfully.", MsgBoxStyle.Exclamation)

        End Try
    End Sub

    Private Sub dtpDateofVisit_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Label19.Text = Format(dtpDateofVisit.Value, "MMM") & " " & Format(dtpDateofVisit.Value, "yyyy")
    End Sub


    Private Sub DataGridView2_CellContentClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView2.CellContentClick
        Dim ErrorAction As New functions
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)

        Try
            lnkmonitorstatus.Enabled = False
            lnkServicemode.Enabled = False

            'AddRootNode("all")
            'AddRootNodepriority()

            'select item to edit or delete
            Dim mysqlaction As String = ""
            Dim MyDBAction As New functions
            Dim K As Integer

            K = e.RowIndex

            'Dim MyDatable As New Data.DataTable

            'mysqlaction = "SELECT * FROM   statusandservicevisit WHERE SSVID ='" & Me.DataGridView2.Rows(K).Cells(1).Value & "'"
            'MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            Dim cmd As SqlCommand
            If IsCaregiver = True Then
                cmd = New SqlCommand("dbo.CAREGIVERForm1A_fetchspecificvisit")
            Else
                cmd = New SqlCommand("dbo.Form1A_fetchspecificvisit")
            End If

            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandTimeout = 300
            cmd.Parameters.Add(New SqlParameter("@ssvid", Me.DataGridView2.Rows(K).Cells(1).Value.ToString))
            ''cmd.Parameters.Add(New SqlParameter("@DateofVisit", dtpDateofVisit.Value))
            conn.Open()
            cmd.Connection = conn

            Dim myreader As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)

            'If MyDatable.Rows.Count > 0 Then

            'Depending if it is a monitoring visit or a service visit
            'so hide the priority list box, else show it
            If LCase(Me.DataGridView2.Rows(K).Cells(2).Value.ToString) = "monitor" Then
                'clear treeview and checkboxes
                visitType = "monitor"
                AddRootNode("monitor")
                dtpDateofVisit.Value = Date.Today

                AddCriticalEvents()

                'dont hide priorities when monitoring
                Label17.Visible = True
                lstpriorityListing.Visible = True

                Label22.Visible = True
                lstCriticalEvents.Visible = True

                'dont hide number of visits when monitoring
                Label2.Visible = True
                txtNumofVisits.Visible = True
            ElseIf LCase(Me.DataGridView2.Rows(K).Cells(2).Value.ToString) = "service" Then
                'clear treeview and checkboxes
                visitType = "service"
                AddRootNode("service")
                dtpDateofVisit.Value = Date.Today

                'hide priorities when monitoring
                Label17.Visible = False
                lstpriorityListing.Visible = False

                Label22.Visible = False
                lstCriticalEvents.Visible = False

                'hide number of visits when monitoring
                Label2.Visible = False
                txtNumofVisits.Visible = False

            End If

            Do While myreader.Read

                'First check which treeview should be populated
                'then repopulate saved service and monitoring and priorities
                If myreader("treeviewgani").ToString = "serviceandmonitoring" Then
                    For Each MyNode In TreeView1.Nodes 'Loop thru level1 [Domains]
                        If MyNode.Tag.ToString = myreader("domainid").ToString Then
                            For Each MyChildNode In MyNode.Nodes 'loop through subdomains
                                If MyChildNode.Tag.ToString = myreader("csid").ToString Then
                                    For Each MyServiceNode In MyChildNode.Nodes
                                        If MyServiceNode.Tag.ToString = myreader("ssid").ToString Then
                                            MyServiceNode.Checked = True 'check the Service
                                            'MsgBox("ServiceandMonitoring - " & MyServiceNode.text.ToString)
                                            Exit For
                                        End If
                                    Next
                                End If
                            Next
                        End If
                    Next
                ElseIf myreader("treeviewgani").ToString = "priorities" Then
                    For Each MypriorityNode In lstpriorityListing.Nodes 'Loop thru level1 [Domains]
                        If MypriorityNode.Tag.ToString = myreader("domainid").ToString Then
                            For Each MypriorityChildNode In MypriorityNode.Nodes 'loop through services
                                'If MyChildNode.Tag.ToString = myreader("PriorityID").ToString Then
                                If MypriorityChildNode.Tag.ToString = myreader("SSID").ToString Then
                                    MypriorityChildNode.Checked = True 'check the services
                                    'MsgBox("priorities - " & MypriorityChildNode.text.ToString)
                                    Exit For
                                End If
                            Next
                        End If
                    Next
                End If

                'repopulate saved service and monitoring and critical events
                populate_serviceandmonitoring_criticalevents(myreader("SSVID").ToString, IsCaregiver)
                'populate_ssmPriorities(myreader("SSVID").ToString)
                dtpDateofVisit.Value = myreader("DateofVisit").ToString
                txtNumofVisits.Text = myreader("HouseHoldVisits").ToString
                global_ssvid = myreader("SSVID").ToString
            Loop
            ' End If
            'End If



            'take focus back to the first tab
            TabControl1.SelectedIndex = 1

            Panel1.Enabled = True
            btnEdit.Enabled = True
            btnpost.Enabled = False
        Catch ex As Exception
            MsgBox(ex.Message)
            ErrorAction.WriteToErrorLogFile("ServiceandMonitoring", "Cellcontentclick", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populate_serviceandmonitoring_criticalevents(ByVal myssvid As String, ByVal myIscaregiver As Boolean)
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        Try

            'read critical events separate
            Dim cmd As SqlCommand
            Dim myreader As SqlDataReader
            Dim dview As DataRowView
            Dim i As Integer
            If myIscaregiver = True Then
                cmd = New SqlCommand("dbo.CAREGIVERForm1A_fetchspecificvisit_criticalevents")
            Else
                cmd = New SqlCommand("dbo.Form1A_fetchspecificvisit_criticalevents")
            End If

            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandTimeout = 300
            cmd.Parameters.Add(New SqlParameter("@ssvid", myssvid.ToString))
            ''cmd.Parameters.Add(New SqlParameter("@DateofVisit", dtpDateofVisit.Value))
            conn.Open()
            cmd.Connection = conn

            myreader = cmd.ExecuteReader(CommandBehavior.CloseConnection)

            Do While myreader.Read
                i = 0
                For Each dview In lstCriticalEvents.Items
                    'Its now dview.row.item(0) coz CRITICALEVENTID is field 0

                    If dview.Row.Item(0).ToString = myreader("CriticalEventID").ToString Then
                        lstCriticalEvents.SetItemChecked(i, True)
                        Exit For
                    End If
                    i = i + 1
                Next
            Loop
        Catch ex As Exception
            MsgBox(Err.Description)
            If conn.State = ConnectionState.Open Then conn.Close()
        End Try
    End Sub
    Private Sub populate_serviceandmonitoring_and_Priorities(ByVal myssvid As String)
        Dim ErrorAction As New functions
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)

        Try

            'TreeView1.Nodes(1).Nodes(0).Nodes(0).Checked = True
            'loop through domains
            Dim MyDomainName As String = ""
            Dim MyDomainid As String = ""
            Dim MnuCount As Int16 = 0
            Dim j As Int16

            'Dim MyDataTable As New Data.DataTable
            'Dim sqlAction As String = "SELECT * FROM   vw_servicemonitoring_history WHERE ssvID = '" & myssvid.ToString & "'"


            Dim MyDBAction As New functions
            Dim MyNode As New TreeNode
            Dim MyChildNode As New TreeNode
            Dim MyServiceNode As New TreeNode
            'MyDataTable = TryCast(MyDBAction.DBAction(sqlAction, DBActionType.DataTable), Data.DataTable)

            Dim cmd As New SqlCommand("dbo.Form1A_fetchspecificvisit")
            cmd.CommandType = CommandType.StoredProcedure
            cmd.CommandTimeout = 300
            cmd.Parameters.Add(New SqlParameter("@ssvid", myssvid.ToString))
            ''cmd.Parameters.Add(New SqlParameter("@DateofVisit", dtpDateofVisit.Value))
            conn.Open()
            cmd.Connection = conn

            Dim myreader As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)
            'If MyDataTable.Rows.Count > 0 Then
            Do While myreader.Read


                'MnuCount = MyDataTable.Rows.Count
                'For j = 0 To MyDataTable.Rows.Count - 1


                'First check which treeview should be populated
                If myreader("visittype").ToString = "monitor" Then
                    For Each MyNode In TreeView1.Nodes 'Loop thru level1 [Domains]
                        If MyNode.Tag.ToString = myreader("domainid").ToString Then
                            For Each MyChildNode In MyNode.Nodes 'loop through subdomains
                                If MyChildNode.Tag.ToString = myreader("csid").ToString Then
                                    For Each MyServiceNode In MyChildNode.Nodes
                                        If MyServiceNode.Tag.ToString = myreader("ssid").ToString Then
                                            MyServiceNode.Checked = True 'check the Service
                                            Exit For
                                        End If
                                    Next
                                End If
                            Next
                        End If
                    Next
                ElseIf myreader("visittype").ToString = "service" Then
                    For Each MyNode In lstpriorityListing.Nodes 'Loop thru level1 [Domains]
                        If MyNode.Tag.ToString = myreader("domainid").ToString Then
                            For Each MyChildNode In MyNode.Nodes 'loop through services
                                If MyChildNode.Tag.ToString = myreader("PriorityID").ToString Then
                                    MyChildNode.Checked = True 'check the services
                                    Exit For
                                End If
                            Next
                        End If
                    Next
                End If
                'Next
            Loop
            'End If
            TreeView1.ExpandAll()
            lstpriorityListing.ExpandAll()
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("ServiceandMonitoring", "populate_serviceandmonitoring", ex.Message) ''---Write error to error log file
        Finally
            conn.Close()

        End Try
    End Sub
    Private Sub populate_ssmPriorities(ByVal myssvid As String)
        Dim ErrorAction As New functions
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)

        Try

            'TreeView1.Nodes(1).Nodes(0).Nodes(0).Checked = True
            'loop through domains
            Dim MyDomainName As String = ""
            Dim MyDomainid As String = ""
            Dim MnuCount As Int16 = 0
            Dim j As Int16

            Dim MyDataTable As New Data.DataTable
            Dim sqlAction As String = "SELECT ServiceStatus.CSID, CoreServices.Domainid, PriorityMonitoring.SSVID, " & _
            " PriorityMonitoring.PriorityID FROM  CoreServices INNER JOIN " & _
                      " ServiceStatus ON CoreServices.CSID = ServiceStatus.CSID INNER JOIN " & _
                      " PriorityMonitoring ON ServiceStatus.SSID = PriorityMonitoring.PriorityID WHERE ssvID = '" & myssvid.ToString & "' order by priorityid asc"
            Dim MyDBAction As New functions
            Dim MyNode As New TreeNode
            Dim MyChildNode As New TreeNode
            Dim MyProviderNode As New TreeNode
            'MyDataTable = TryCast(MyDBAction.DBAction(sqlAction, DBActionType.DataTable), Data.DataTable)

            Dim cmd As New SqlCommand(sqlAction, conn)
            conn.Open()
            Dim myreader As SqlDataReader = cmd.ExecuteReader()


            'If MyDataTable.Rows.Count > 0 Then
            '    MnuCount = MyDataTable.Rows.Count
            '    For j = 0 To MyDataTable.Rows.Count - 1
            Do While myreader.Read
                For Each MyNode In lstpriorityListing.Nodes 'Loop thru level1 [Domains]
                    If MyNode.Tag.ToString = myreader("domainid").ToString Then
                        For Each MyChildNode In MyNode.Nodes 'loop through services
                            If MyChildNode.Tag.ToString = myreader("PriorityID").ToString Then
                                MyChildNode.Checked = True 'check the services
                                Exit For
                            End If
                        Next
                    End If
                Next
            Loop
            '    Next

            'End If
            lstpriorityListing.ExpandAll()
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("serviceandmonitoring", "populate_ssmpriorities", ex.Message) ''---Write error to error log file
        Finally
            conn.Close()

        End Try
        'Dim ErrorAction As New functions
        'Try
        '    'TreeView1.Nodes(1).Nodes(0).Nodes(0).Checked = True
        '    'loop through domains
        '    Dim MyDomainName As String = ""
        '    Dim MyDomainid As String = ""
        '    Dim MnuCount As Int16 = 0
        '    Dim j As Int16

        '    Dim MyDataTable As New Data.DataTable
        '    Dim sqlAction As String = "SELECT * FROM   prioritymonitoring WHERE ssvID = '" & myssvid.ToString & "' order by priorityid asc"


        '    Dim MyDBAction As New functions
        '    Dim MyNode As New TreeNode
        '    Dim MyChildNode As New TreeNode
        '    Dim MyGoalNode As New TreeNode
        '    Dim dview As DataRowView
        '    Dim i As Integer
        '    MyDataTable = TryCast(MyDBAction.DBAction(sqlAction, DBActionType.DataTable), Data.DataTable)

        '    If MyDataTable.Rows.Count > 0 Then
        '        MnuCount = MyDataTable.Rows.Count
        '        For j = 0 To MyDataTable.Rows.Count - 1

        '            ' Loop through all the  items of the listbox and check/select what had been saved
        '            i = 0
        '            For Each dview In lstPriorityListing.Items
        '                'Its now dview.row.item(5) coz we using vw_domain_services and priorityid/ssid is field 5
        '                'If dview.Row.Item(0).ToString = MyDataTable.Rows(j).Item("Priorityid").ToString Then
        '                If dview.Row.Item(5).ToString = MyDataTable.Rows(j).Item("Priorityid").ToString Then
        '                    lstPriorityListing.SetItemChecked(i, True) 'check records that exist in Priorityneeds table
        '                    Exit For
        '                End If
        '                i = i + 1

        '            Next


        '        Next

        '    End If
        '    TreeView1.ExpandAll()
        'Catch ex As Exception
        '    ErrorAction.WriteToErrorLogFile("serviceandmonitoring", "populate_ssmpriorities", ex.Message) ''---Write error to error log file

        'End Try
    End Sub

    Private Sub btnEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEdit.Click
        're-check if systemdate has been adjusted after OLMIS has been started
        'check if the system date has been messed with, by checking currentdate - most recent services date
        Dim MyDBAction As New functions
        'Dim SqlAction As String = "SELECT top 1 dateofvisit from StatusAndServiceVisit order by dateofvisit desc"
        'Dim Mymaxdateofvisit As Date = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

        'If Date.Today < Mymaxdateofvisit Then
        '    MsgBox("Please check your system date", MsgBoxStyle.Exclamation)
        '    'Exit Sub
        'End If

        ''Validate so that CAREGIVERs are only assessed/served  on March, June, September and December
        'If Me.Text.ToString.ToUpper.Contains("CAREGIVER") AndAlso _
        '    (Format(dtpDateofVisit.Value, "MMM").ToString.ToUpper <> "MAR" And
        '        Format(dtpDateofVisit.Value, "MMM").ToString.ToUpper <> "JUN" And
        '        Format(dtpDateofVisit.Value, "MMM").ToString.ToUpper <> "SEP" And
        '        Format(dtpDateofVisit.Value, "MMM").ToString.ToUpper <> "DEC") Then
        '    MsgBox("Caregivers are only assessed/served in March, June, September and December")
        '    Exit Sub
        'End If

        'first delete existing records
        'Dim MyDBAction As New OVCSystem.functions
        MyDBAction.DeleteServiceMonitoring(global_ssvid)

        'validate before saving
        'If validate_status() = True AndAlso validate_priorities() = True Then
        If validate_priorities() = True AndAlso validate_rules() = True AndAlso validate_monitoring_service(myOVCID) = True Then
            SaveServiceandMonitoring()
            fillgrid()
            TabControl1.SelectedIndex = 2
            Panel1.Enabled = False
            btnEdit.Enabled = False
            btnpost.Enabled = False
        End If

        TreeView1.ExpandAll()


    End Sub


    Private Sub BtnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnExit.Click
        Me.Close()
    End Sub

    Private Sub txtsearchFname_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtsearchFname.LostFocus
        If txtsearchFname.Text.Length <> 0 Then
            strnames = strnames & " AND firstname = '" & txtsearchFname.Text & "' "
        Else
            strnames = strnames
        End If
        'and if there is still data in other textboxex combine it here
        If txtsearchMname.Text.Length <> 0 Then
            strnames = strnames & " AND middlename = '" & txtsearchMname.Text & "'"
        End If
        If txtsearchSurname.Text.Length <> 0 Then
            strnames = strnames & " AND surname = '" & txtsearchSurname.Text & "'"
        End If
    End Sub

    Private Sub txtsearchMname_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtsearchMname.LostFocus
        If txtsearchMname.Text.Length <> 0 Then
            strnames = strnames & " AND middlename = '" & txtsearchMname.Text & "' "
        End If
        'and if there is still data in other textboxex combine it here
        If txtsearchFname.Text.Length <> 0 Then
            strnames = strnames & " AND firstname = '" & txtsearchFname.Text & "'"
        End If
        If txtsearchSurname.Text.Length <> 0 Then
            strnames = strnames & " AND surname = '" & txtsearchSurname.Text & "'"
        End If
    End Sub

    Private Sub txtsearchSurname_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtsearchSurname.LostFocus
        If txtsearchSurname.Text.Length <> 0 Then
            strnames = strnames & " AND surname = '" & txtsearchSurname.Text & "' "
        End If
        'and if there is still data in other textboxex combine it here
        If txtsearchFname.Text.Length <> 0 Then
            strnames = strnames & " AND firstname = '" & txtsearchFname.Text & "'"
        End If
        If txtsearchMname.Text.Length <> 0 Then
            strnames = strnames & " AND middlename = '" & txtsearchMname.Text & "'"
        End If
    End Sub

    Private Sub clear_checked(ByVal mytreeview As TreeView)
        'Check what is selected or not selected and make sure that all are selected
        Dim MyNode As New TreeNode
        Dim MyChildNode As New TreeNode
        Dim MychildsubNode As New TreeNode

        For Each MyNode In mytreeview.Nodes 'Loop thru level1 

            For Each MyChildNode In MyNode.Nodes 'Loop thru level2 

                'if selected, unselect it
                If MyChildNode.Checked = True Then
                    MyChildNode.Checked = False

                End If
                For Each MychildsubNode In MyChildNode.Nodes 'Loop thru level3 

                    'if selected, unselect it
                    If MychildsubNode.Checked = True Then
                        MychildsubNode.Checked = False

                    End If

                    'If we are monitoring, give service nodes a different color and vice versa
                    If mytreeview.Name = "treeview1" Then
                        If visitType = "monitor" Then
                            If MychildsubNode.ToolTipText = "Service" Then
                                MychildsubNode.BackColor = Color.Red
                            End If
                        ElseIf visitType = "service" Then
                            If MychildsubNode.ToolTipText = "Monitor" Then
                                MychildsubNode.BackColor = Color.Red
                            End If
                        End If
                    End If

                Next
            Next

        Next
    End Sub

    Private Sub lnkmonitorstatus_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkmonitorstatus.LinkClicked
        'clear treeview and checkboxes
        AddRootNode("monitor")
        visitType = "monitor"

        AddCriticalEvents()

        'AddRootNodepriority()
        clear_checked(lstpriorityListing)
        clear_checked(TreeView1)

        'clear critical events listbox
        Dim dview As DataRowView
        Dim i As Integer = 0
        For Each dview In lstCriticalEvents.Items
            lstCriticalEvents.SetItemChecked(i, False)
            i = i + 1
        Next
        

        dtpDateofVisit.Value = Date.Today

        'dont hide priorities when monitoring
        Label17.Visible = True
        lstpriorityListing.Visible = True


        Label22.Visible = True
        lstCriticalEvents.Visible = True

        'dont hide number of visits when monitoring
        'Label2.Visible = True
        'txtNumofVisits.Visible = True
        txtNumofVisits.Text = "0"


        'Label20.Text = ""
    End Sub

    Private Sub lnkServicemode_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkServicemode.LinkClicked
        Try

        
        'clear treeview and checkboxes
        AddRootNode("service")
        visitType = "service"
        'AddRootNodepriority()

        clear_checked(TreeView1)
        clear_checked(lstpriorityListing)

        'clear critical events listbox
        Dim dview As DataRowView
        Dim i As Integer = 0
        For Each dview In lstCriticalEvents.Items
            lstCriticalEvents.SetItemChecked(i, False)
            i = i + 1
        Next

        dtpDateofVisit.Value = Date.Today


        'hide priorities when monitoring
        Label17.Visible = False
        lstpriorityListing.Visible = False

        Label22.Visible = False
        lstCriticalEvents.Visible = False

            'hide number of visits when monitoring
            'Label2.Visible = False
            'txtNumofVisits.Visible = False
            txtNumofVisits.Text = "0"

        Catch ex As Exception

        End Try
        'Label20.Text = ""
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabControl1.SelectedIndexChanged
        If TabControl1.SelectedIndex = 0 Then
            Me.AcceptButton = btnSearch
        ElseIf TabControl1.SelectedIndex = 1 Then
            Me.AcceptButton = btnpost
        ElseIf TabControl1.SelectedIndex = 2 Then
        End If
    End Sub

#Region "Treeview Population"

    'Open the XML file, and start to populate the treeview
    Private Sub populateTreeview()
        Dim dlg As New OpenFileDialog()
        dlg.Title = "Open XML Document"
        dlg.Filter = "XML Files (*.xml)|*.xml"
        dlg.FileName = Application.StartupPath & "\..\..\example.xml"
        If dlg.ShowDialog() = DialogResult.OK Then
            Try
                'Just a good practice -- change the cursor to a wait cursor while the nodes populate
                Me.Cursor = Cursors.WaitCursor

                'First, we'll load the Xml document
                Dim xDoc As New XmlDocument()
                xDoc.Load(dlg.FileName)

                'Now, clear out the treeview, and add the first (root) node
                TreeView1.Nodes.Clear()
                TreeView1.Nodes.Add(New TreeNode(xDoc.DocumentElement.Name))
                Dim tNode As New TreeNode()
                tNode = DirectCast(TreeView1.Nodes(0), TreeNode)

                'We make a call to AddNode, where we'll add all of our nodes
                addTreeNode(xDoc.DocumentElement, tNode)

                'Expand the treeview to show all nodes

                TreeView1.ExpandAll()
            Catch xExc As XmlException
                'Exception is thrown is there is an error in the Xml
                MessageBox.Show(xExc.Message)
            Catch ex As Exception
                'General exception
                MessageBox.Show(ex.Message)
            Finally
                'Change the cursor back
                Me.Cursor = Cursors.[Default]
            End Try
        End If
    End Sub

    'This function is called recursively until all nodes are loaded
    Private Sub addTreeNode(ByVal xmlNode As XmlNode, ByVal treeNode As TreeNode)
        Dim xNode As XmlNode
        Dim tNode As TreeNode
        Dim xNodeList As XmlNodeList

        If xmlNode.HasChildNodes Then
            'The current node has children
            xNodeList = xmlNode.ChildNodes

            For x As Integer = 0 To xNodeList.Count - 1
                'Loop through the child nodes
                xNode = xmlNode.ChildNodes(x)
                treeNode.Nodes.Add(New TreeNode(xNode.Name))
                tNode = treeNode.Nodes(x)
                addTreeNode(xNode, tNode)
            Next
        Else
            'No children, so add the outer xml (trimming off whitespace)
            treeNode.Text = xmlNode.OuterXml.Trim()
        End If
    End Sub

#End Region

#Region "XML Writer Methods"

    Private Sub serializeTreeview2()
        Dim dlg As New SaveFileDialog()
        dlg.FileName = Me.TreeView1.Nodes(0).Text & ".xml"
        dlg.Filter = "XML Files (*.xml)|*.xml"
        If dlg.ShowDialog() = DialogResult.OK Then
            exportToXml2(TreeView1, dlg.FileName)
        End If
    End Sub

    'We use this in the exportToXml2 and the saveNode2 functions, though it's only instantiated once.
    Private xr As XmlTextWriter

    Public Sub exportToXml2(ByVal tv As TreeView, ByVal filename As String)
        xr = New XmlTextWriter(filename, System.Text.Encoding.UTF8)

        xr.WriteStartDocument()
        'Write our root node
        xr.WriteStartElement(TreeView1.Nodes(0).Text)
        For Each node As TreeNode In tv.Nodes
            saveNode2(node.Nodes)
        Next
        'Close the root node
        xr.WriteEndElement()
        xr.Close()
    End Sub

    Private Sub saveNode2(ByVal tnc As TreeNodeCollection)
        For Each node As TreeNode In tnc
            'If we have child nodes, we'll write a parent node, then iterrate through
            'the children
            If node.Nodes.Count > 0 Then
                xr.WriteStartElement(node.Text)
                saveNode2(node.Nodes)
                xr.WriteEndElement()
            Else
                'No child nodes, so we just write the text
                xr.WriteString(node.Text)
            End If
        Next
    End Sub

#End Region

#Region "Stream Writer Methods"

    'Opens a save file dialog so we know where to save the XML. This method uses a streamwriter.
    Private Sub serializeTreeview()
        Dim dlg As New SaveFileDialog()
        dlg.FileName = Me.TreeView1.Nodes(0).Text & ".xml"
        dlg.Filter = "XML Files (*.xml)|*.xml"
        If dlg.ShowDialog() = DialogResult.OK Then
            exportToXml(TreeView1, dlg.FileName)
        End If
    End Sub

    'We use this in the export and the saveNode functions, though it's only instantiated once.
    Private sr As StreamWriter

    Public Sub exportToXml(ByVal tv As TreeView, ByVal filename As String)
        sr = New StreamWriter(filename, False, System.Text.Encoding.UTF8)
        'Write the header
        sr.WriteLine("<?xml version=""1.0"" encoding=""utf-8"" ?>")
        'Write our root node
        sr.WriteLine("<" & TreeView1.Nodes(0).Text & ">")
        For Each node As TreeNode In tv.Nodes
            saveNode(node.Nodes)
        Next
        'Close the root node
        sr.WriteLine("</" & TreeView1.Nodes(0).Text & ">")
        sr.Close()
    End Sub

    Private Sub saveNode(ByVal tnc As TreeNodeCollection)
        For Each node As TreeNode In tnc
            'If we have child nodes, we'll write a parent node, then iterrate through
            'the children
            If node.Nodes.Count > 0 Then
                sr.WriteLine("<" & node.Text & ">")
                saveNode(node.Nodes)
                sr.WriteLine("</" & node.Text & ">")
            Else
                'No child nodes, so we just write the text
                sr.WriteLine(node.Text)
            End If
        Next
    End Sub

#End Region

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        serializeTreeview2()
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        populateTreeview()
    End Sub

    '**********these code helps select distinct records on datatable. All this to avoid reading from dB multiple times
    Public Shared Function SelectDistinct(ByVal SourceTable As DataTable, ByVal ParamArray FieldNames() As String) As DataTable
        Dim lastValues() As Object
        Dim newTable As DataTable

        If FieldNames Is Nothing OrElse FieldNames.Length = 0 Then
            Throw New ArgumentNullException("FieldNames")
        End If

        lastValues = New Object(FieldNames.Length - 1) {}
        newTable = New DataTable

        For Each field As String In FieldNames
            newTable.Columns.Add(field, SourceTable.Columns(field).DataType)
        Next

        For Each Row As DataRow In SourceTable.Select("", String.Join(", ", FieldNames))
            If Not fieldValuesAreEqual(lastValues, Row, FieldNames) Then
                newTable.Rows.Add(createRowClone(Row, newTable.NewRow(), FieldNames))

                setLastValues(lastValues, Row, FieldNames)
            End If
        Next

        Return newTable
    End Function


    Private Shared Function fieldValuesAreEqual(ByVal lastValues() As Object, ByVal currentRow As DataRow, ByVal fieldNames() As String) As Boolean
        Dim areEqual As Boolean = True

        For i As Integer = 0 To fieldNames.Length - 1
            If lastValues(i) Is Nothing OrElse Not lastValues(i).Equals(currentRow(fieldNames(i))) Then
                areEqual = False
                Exit For
            End If
        Next

        Return areEqual
    End Function


    Private Shared Function createRowClone(ByVal sourceRow As DataRow, ByVal newRow As DataRow, ByVal fieldNames() As String) As DataRow
        For Each field As String In fieldNames
            newRow(field) = sourceRow(field)
        Next

        Return newRow
    End Function


    Private Shared Sub setLastValues(ByVal lastValues() As Object, ByVal sourceRow As DataRow, ByVal fieldNames() As String)
        For i As Integer = 0 To fieldNames.Length - 1
            lastValues(i) = sourceRow(fieldNames(i))
        Next
    End Sub

   
    '************************************

    Private Sub TreeView1_AfterCheck(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterCheck
        Try
            'split the tooltiptext which is where we have stored all validation fields, separated by [:]
            '"AgeRequired", "Gender", "Needstobeselectedprior", "Needstobeunselectedprior", "PriorNumberofMonths", "AffectsLongitudinalbiodata"
            Dim myvalidationsarray As Array = e.Node.ToolTipText.Split(":")

            'If the selected node has [Affectslongitudinalbiodata] validation
            If e.Node.Checked = True And Trim(myvalidationsarray(5)).Length <> 0 Then
                Select Case LCase(myvalidationsarray(5).ToString)
                    Case "hiv status"
                        Label23.Visible = True
                        cboHIVStatus.Visible = True
                    Case "immunization"
                        Label24.Visible = True
                        cboImmunization.Visible = True
                    Case "birth certificate"
                        If HasBirthCert = True AndAlso btnpost.Enabled = False Then
                            MsgBox("Child already has a birth certificate.", vbExclamation)
                            Exit Sub
                        Else
                            chkHasCert.Visible = True
                        End If

                End Select
            Else
                Select Case LCase(myvalidationsarray(5).ToString)
                    Case "hiv status"
                        Label23.Visible = False
                        cboHIVStatus.Visible = False
                    Case "immunization"
                        Label24.Visible = False
                        cboImmunization.Visible = False
                    Case "birth certificate"
                        chkHasCert.Visible = False
                End Select
            End If
        Catch ex As Exception
            ' MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub cboDistrict_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboDistrict.SelectedIndexChanged

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
            ErrorAction.WriteToErrorLogFile("ServiceMonitoring", "populatehivstatus", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub cboHIVStatus_Click(sender As Object, e As EventArgs) Handles cboHIVStatus.Click

    End Sub

  
    Private Sub cboHIVStatus_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboHIVStatus.SelectedIndexChanged
        Try
            'make sure that status is not changing from HIV+ve to something else
            If myPreviousHIVStatus.ToUpper = "POSITIVE" _
                AndAlso myOVCID = myPreviousOVCID _
                AndAlso cboHIVStatus.Text.ToUpper <> myPreviousHIVStatus.ToUpper Then
                ErrorProvider1.SetError(cboHIVStatus, "Please select correct status")
                MsgBox("HIV status can NOT turn from POSITIVE to something else.", vbExclamation)
                cboHIVStatus.SelectedIndex = -1
                cboHIVStatus.Focus()
                Exit Sub
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub DataGridView1_RowLeave(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.RowLeave
        'track the previous OVC ID as we move to a different row to avoid 
        'false validations about HIV status changing from Positive to something else
        myPreviousOVCID = myOVCID
    End Sub

    
End Class