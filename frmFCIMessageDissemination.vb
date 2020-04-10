Imports OVCSystem.functions
Imports System.Data.Odbc
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Text
Public Class frmFCIMessageDissemination
    Private Sub frmFCIMessageDissemination_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        populatesearchcounties()
        populatecounties()
        populatetypeofgroup()
        populatemessagetype()
    End Sub
    Private Sub populatesearchcounties()
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
            ErrorAction.WriteToErrorLogFile("FCI Message Dissemination", "populatecounties", ex.Message) ''---Write error to error log file

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

            With cboCounty
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "County"
                .ValueMember = 0
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With


        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("FCI Message Dissemination", "populatecounties", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populateLIPs(ByVal strcounties As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "SELECT DISTINCT LIPs.LIP FROM   OVCRegistrationDetails INNER JOIN LIPs ON " &
                " OVCRegistrationDetails.cbo = LIPs.cbo where county = '" & strcounties & "'  order by LIP asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cboLIP
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "LIP"
                .ValueMember = 0
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With


        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("FCI Message Dissemination", "populateLIps ", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populatesearchLIPs(ByVal strcounties As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "SELECT DISTINCT LIPs.LIP FROM   OVCRegistrationDetails INNER JOIN LIPs ON " &
                " OVCRegistrationDetails.cbo = LIPs.cbo where county = '" & strcounties & "'  order by LIP asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cbosearchLIP
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "LIP"
                .ValueMember = 0
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With


        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("FCI Message Dissemination", "populateLips", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub cboCounty_SelectedValueChanged(sender As Object, e As EventArgs) Handles cboCounty.SelectedValueChanged
        Try
            'If cboDistrict.Text.Trim.Length <> 0 Or cboDistrict.Text <> "System.Data.DataRowView" Then
            If IsNumeric(cboCounty.SelectedIndex) = True Then
                populateLIPs(cboCounty.Text.ToString)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub cbosearchCounty_SelectedValueChanged(sender As Object, e As EventArgs) Handles cbosearchcounty.SelectedValueChanged
        Try
            'If cboDistrict.Text.Trim.Length <> 0 Or cboDistrict.Text <> "System.Data.DataRowView" Then
            If IsNumeric(cbosearchcounty.SelectedIndex) = True Then
                populatesearchLIPs(cbosearchcounty.Text.ToString)
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub populatetypeofgroup()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "SELECT [type_of_group_id]" &
                                        " ,[type_of_group] FROM [dbo].[FCI_type_of_group]  order by type_of_group asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cboTypeofGroup
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "type_of_group"
                .ValueMember = "type_of_group_id"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With


        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("FCI Message Dissemination", "populatetypeofgroup", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populatemessagetype()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "SELECT distinct [message_type] FROM [dbo].[FCI_message_type]"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cboMessageType
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "message_type"
                .ValueMember = "0"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With


        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("FCI Message Dissemination", "populatemessagetype", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populatemessage(ByVal strmessagetype As String)
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "SELECT [message_type_id],[message] FROM [dbo].[FCI_message_type] where message_type = '" & strmessagetype & "' order by message"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)

            With cboMessage
                .DataSource = Nothing
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "message"
                .ValueMember = "message_type_id"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With


        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("FCI Message Dissemination", "populatemessage", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub cboMessageType_SelectedValueChanged(sender As Object, e As EventArgs) Handles cboMessageType.SelectedValueChanged
        Try
            'If cboDistrict.Text.Trim.Length <> 0 Or cboDistrict.Text <> "System.Data.DataRowView" Then
            If IsNumeric(cboMessageType.SelectedIndex) = True Then
                populatemessage(cboMessageType.Text.ToString)
            End If

        Catch ex As Exception

        End Try
    End Sub
End Class