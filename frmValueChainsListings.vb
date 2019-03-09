Imports OVCSystem.functions
Imports System.Data.Odbc

Public Class frmValueChainsListings
    Private Sub lnkNew_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkNew.LinkClicked
        Panel1.Enabled = True
        btnpost.Enabled = True
    End Sub

    Private Sub BtnEdit_Click(sender As Object, e As EventArgs) Handles BtnEdit.Click
        Dim ErrorAction As New functions
        Try

            'update record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            mySqlAction = "update valuechain_List set valuechain_name = " &
            " '" & txtName.Text.ToString & "'" &
            " where valuechain_id = " & txtValueChainID.Text & ""

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Update)
            MsgBox("Record updated successfully.", MsgBoxStyle.Information)

            fillgrid()

            Panel1.Enabled = False
            BtnEdit.Enabled = False
            BtnDelete.Enabled = False
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("valuechain_List", "Edit", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub frmValueChainsListings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        fillgrid()
    End Sub

    Private Sub fillgrid()
        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            Dim mySqlAction As String = "select * from valuechain_List order by valuechain_name"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim myvaluechainid, myvaluechainname As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView1.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    myvaluechainid = MyDatable.Rows(K).Item("valuechain_id").ToString
                    myvaluechainname = MyDatable.Rows(K).Item("valuechain_name").ToString

                    DataGridView1.Rows.Add(myvaluechainid, myvaluechainname, "Select")
                Next
            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("valuechain_List", "Fillgrid", ex.Message) ''---Write error to error log file

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
            mysqlaction = "select * from valuechain_List where valuechain_id=" & Me.DataGridView1.Rows(K).Cells(0).Value & ""
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then

                txtValueChainID.Text = MyDatable.Rows(0).Item("valuechain_id").ToString
                txtName.Text = MyDatable.Rows(0).Item("valuechain_name").ToString


            End If

            Panel1.Enabled = True
            BtnEdit.Enabled = True
            BtnDelete.Enabled = True
        Catch ex As Exception
            MsgBox(ex.Message, vbExclamation)
            ErrorAction.WriteToErrorLogFile("valuechain_List", "GridCellClick", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub btnpost_Click(sender As Object, e As EventArgs) Handles btnpost.Click
        Dim ErrorAction As New functions
        Try

            'save record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            mySqlAction =
            "INSERT INTO [dbo].[valuechain_List] " &
           "([valuechain_name])" &
            "VALUES " &
           "( '" & txtName.Text.ToString & "')"


            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
            MsgBox("Record saved successfully.", MsgBoxStyle.Information)

            fillgrid()

            Panel1.Enabled = False
            btnpost.Enabled = False
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("valuechain_List", "Save", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs) Handles BtnDelete.Click

    End Sub

    Private Sub BtnExit_Click(sender As Object, e As EventArgs) Handles BtnExit.Click
        Me.Close()

    End Sub
End Class