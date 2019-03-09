Imports OVCSystem.functions
Imports System.Data.Odbc

Public Class frmStarterKitListing
    Private Sub frmStarterKitListing_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        populatevaluechains()
        populatevalueStarterKitTypes()
        fillgrid()

    End Sub

    Private Sub lnkNew_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkNew.LinkClicked
        Panel1.Enabled = True
        btnpost.Enabled = True
        BtnDelete.Enabled = False
        BtnEdit.Enabled = False
        txtstarterkitname.Focus()
        txtStarterkitID.Text = ""
        txtstarterkitname.Text = ""
        cboStarterKitType.SelectedIndex = -1
        cbovaluechain.SelectedIndex = -1
    End Sub

    Private Sub populatevaluechains()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from valuechain_List order by  valuechain_name"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cbovaluechain
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "valuechain_name"
                .ValueMember = "valuechain_id"
                .SelectedIndex = -1
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("StarterkitListing", "populatevaluechains", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub populatevalueStarterKitTypes()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from starter_kit_type order by  starter_kit_type"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboStarterKitType
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "starter_kit_type"
                .ValueMember = "starter_kit_type_id"
                .SelectedIndex = -1
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("StarterkitListing", "populatevalueStarterKitTypes", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub fillgrid()
        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            Dim mySqlAction As String = "SELECT starterkit_list.starterkit_id, starterkit_list.starterkit_name, " &
                                        "starterkit_list.starterkit_type, valuechain_List.valuechain_name " &
                                        "FROM   starterkit_list INNER JOIN valuechain_List  " &
                                        "ON starterkit_list.valuechain_id = valuechain_List.valuechain_id order by  starterkit_name"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim mystarterkitid, mystarterkitname, myValueChain, mystarterkittype As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView1.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    mystarterkitid = MyDatable.Rows(K).Item("starterkit_id").ToString
                    mystarterkitname = MyDatable.Rows(K).Item("starterkit_name").ToString
                    myValueChain = MyDatable.Rows(K).Item("valuechain_name").ToString
                    mystarterkittype = MyDatable.Rows(K).Item("starterkit_type").ToString
                    DataGridView1.Rows.Add(mystarterkitid, mystarterkitname, myValueChain, mystarterkittype, "Select")
                Next
            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("Starterkitlisting", "Fillgrid", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub btnpost_Click(sender As Object, e As EventArgs) Handles btnpost.Click
        Dim ErrorAction As New functions
        Try

            'save record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            mySqlAction = "Insert Into starterkit_list([starterkit_name],[valuechain_id],[starterkit_type]) " &
            " values('" & txtstarterkitname.Text.ToString & "','" & cbovaluechain.SelectedValue.ToString & "','" & cboStarterKitType.Text.ToString & "')"

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
            MsgBox("Record saved successfully.", MsgBoxStyle.Information)

            fillgrid()

            Panel1.Enabled = False
            btnpost.Enabled = False
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("starterkit_list", "Save", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        Dim ErrorAction As New functions
        Try

            'select item to edit or delete
            Dim mysqlaction As String = ""
            Dim MyDBAction As New functions
            Dim K As Integer

            K = e.RowIndex


            Dim MyDatable As New Data.DataTable
            mysqlaction = "select * from [starterkit_list] where [starterkit_id]=" & Me.DataGridView1.Rows(K).Cells(0).Value & ""
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then

                txtStarterkitID.Text = MyDatable.Rows(0).Item("starterkit_id").ToString
                txtstarterkitname.Text = MyDatable.Rows(0).Item("starterkit_name").ToString
                cbovaluechain.SelectedValue = MyDatable.Rows(0).Item("valuechain_id").ToString
                cboStarterKitType.SelectedText = MyDatable.Rows(0).Item("starterkit_type").ToString
            End If

            Panel1.Enabled = True
            BtnEdit.Enabled = True
            BtnDelete.Enabled = True
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("StarterKitListing", "GridCellClick", ex.Message) ''---Write error to error log file

        End Try

    End Sub

    Private Sub BtnEdit_Click(sender As Object, e As EventArgs) Handles BtnEdit.Click
        Dim ErrorAction As New functions
        Try

            'update record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            mySqlAction = "UPDATE [dbo].[starterkit_list] " &
                               "SET [starterkit_name] = '" & txtstarterkitname.Text.ToString & "' " &
                                  ",[valuechain_id] = '" & cbovaluechain.SelectedValue.ToString & "' " &
                                  ",[starterkit_type] = '" & cboStarterKitType.Text.ToString & "' " &
                             "WHERE starterkit_id = '" & txtStarterkitID.Text.ToString & "'"

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Update)
            MsgBox("Record updated successfully.", MsgBoxStyle.Information)

            fillgrid()

            Panel1.Enabled = False
            BtnEdit.Enabled = False
            BtnDelete.Enabled = False
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("StarterKitListing", "Edit", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles BtnExit.Click
        Me.Close()
    End Sub
End Class