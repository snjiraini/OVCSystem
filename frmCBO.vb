Imports OVCSystem.functions
Imports System.Data.Odbc

Public Class frmCBO

    Private Sub frmCBO_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        populatedistricts()
        fillgrid()
    End Sub

    Private Sub populatedistricts()
        Dim ErrorAction As New functions
        Try
        
            'populate the combobox
            Dim mySqlAction As String = "select * from District "
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboDistrict
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "District"
                .ValueMember = "DistrictID"
                .SelectedIndex = -1
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("CBO", "populatedistricts", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub fillgrid()
        Dim ErrorAction As New functions
        Try
        
            'populate the datagrid with all the data
            Dim mySqlAction As String = "select * from CBO "
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim myCBOid, myCBO As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView1.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    myCBOid = MyDatable.Rows(K).Item("CBOid").ToString
                    myCBO = MyDatable.Rows(K).Item("CBO").ToString
                    DataGridView1.Rows.Add(myCBOid, myCBO, "Select")
                Next
            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("CBO", "Fillgrid", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub btnpost_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnpost.Click
        Dim ErrorAction As New functions
        Try

            'save record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            mySqlAction = "Insert Into CBO(CBO,DistrictID) " & _
            " values('" & txtCBO.Text.ToString & "','" & cboDistrict.SelectedValue.ToString & "')"

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
            MsgBox("Record saved successfully.", MsgBoxStyle.Information)

            fillgrid()

            Panel1.Enabled = False
            btnpost.Enabled = False
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("CBO", "Save", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub DataGridView1_CellContentClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        Dim ErrorAction As New functions
        Try

            'select item to edit or delete
            Dim mysqlaction As String = ""
            Dim MyDBAction As New functions
            Dim K As Integer

            K = e.RowIndex


            Dim MyDatable As New Data.DataTable
            mysqlaction = "select * from CBO where CBOid=" & Me.DataGridView1.Rows(K).Cells(0).Value & ""
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then

                txtCBOID.Text = MyDatable.Rows(0).Item("CBOid").ToString
                txtCBO.Text = MyDatable.Rows(0).Item("CBO").ToString
                cboDistrict.SelectedValue = MyDatable.Rows(0).Item("DistrictID").ToString

            End If

            Panel1.Enabled = True
            BtnEdit.Enabled = True
            BtnDelete.Enabled = True
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("CBO", "GridCellClick", ex.Message) ''---Write error to error log file

        End Try

    End Sub

    Private Sub BtnEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnEdit.Click
        Dim ErrorAction As New functions
        Try

            'update record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            mySqlAction = "update CBO set CBO = " & _
            " '" & txtCBO.Text.ToString & "', districtid = '" & cboDistrict.SelectedValue.ToString & _
            "' where cboid = " & txtCBOID.Text & ""

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Update)
            MsgBox("Record updated successfully.", MsgBoxStyle.Information)

            fillgrid()

            Panel1.Enabled = False
            BtnEdit.Enabled = False
            BtnDelete.Enabled = False
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("CBO", "Edit", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub BtnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDelete.Click
        Dim ErrorAction As New functions
        Try

            'delete record
            Dim confirm As Boolean
            confirm = MsgBox("Confirm Deletion of record", MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel)

            'confirm relationships
            Dim MyconfirmFK As New functions
            Dim Isrelated As Boolean
            Isrelated = MyconfirmFK.ConfirmFK_Relationships("cbo", txtCBOID.Text)
            If Isrelated = True Then
                MsgBox("Related data detected. Deletion NOT successful.", MsgBoxStyle.Exclamation)
                Exit Sub
            End If

            If confirm = True Then
                Dim mySqlAction As String = ""
                Dim MyDBAction As New functions
                mySqlAction = "delete from CBO " & _
                "  where CBOid = " & txtCBOID.Text & ""

                MyDBAction.DBAction(mySqlAction, functions.DBActionType.Delete)
                MsgBox("Record deleted successfully.", MsgBoxStyle.Information)

                fillgrid()


                Panel1.Enabled = False
                BtnDelete.Enabled = False
                BtnEdit.Enabled = False
            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("CBO", "Delete", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub BtnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnExit.Click
        Me.Close()
    End Sub

    Private Sub lnkNew_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkNew.LinkClicked
        Panel1.Enabled = True
        btnpost.Enabled = True
        BtnDelete.Enabled = False
        BtnEdit.Enabled = False
        txtCBOID.Focus()
        txtCBO.Text = ""
    End Sub
End Class