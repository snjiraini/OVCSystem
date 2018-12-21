
Imports System.Data.Odbc
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Xml
Imports OVCSystem.functions
Imports OVCSystem.AppSecurity
Imports System.IO
Imports System.Configuration
Public Class frmChangePassword
    Private Sub btnChangePassword_Click(sender As Object, e As EventArgs) Handles btnChangePassword.Click
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        Try
            'make sure that textboxes are not blank
            If validatecontrols() = True Then
                'read password from db and decrypt it
                Dim mydecryptedpass As String = ""
                Dim cmd As New SqlCommand("dbo.Login")
                cmd.CommandType = CommandType.StoredProcedure
                cmd.Parameters.Add(New SqlParameter("@username", strusername.ToString))
                conn.Open()
                cmd.Connection = conn

                Dim myreader As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)

                If myreader.Read Then
                    Dim myAppCrypto As New AppSecurity
                    mydecryptedpass = myAppCrypto.decryptString(myreader("password").ToString)

                    'compare decrypted password and old password
                    If mydecryptedpass.ToString <> txtoldpassword.Text.ToString Then
                        MsgBox("Please provide correct old password", MsgBoxStyle.Exclamation + MsgBoxStyle.OkOnly, Me.Text)
                        Exit Sub
                    Else 'if old password is correct, go ahead and change it to new password
                        'update record
                        Dim mySqlAction As String = ""
                        Dim MyDBAction As New functions
                        mySqlAction = "update users set password = " &
                        " '" & myAppCrypto.encryptString(txtnewpassword.Text.ToString) & "'" &
                        " where username = '" & strusername.ToString & "'"

                        MyDBAction.DBAction(mySqlAction, functions.DBActionType.Update)
                        MsgBox("Password changed successfully.", MsgBoxStyle.Information)
                    End If
                End If
            Else
                Exit Sub
            End If

        Catch ex As Exception
            MsgBox("Password change NOT successfully." & vbCrLf & ex.Message, MsgBoxStyle.Exclamation)
        End Try
    End Sub

    Private Function validatecontrols() As Boolean
        Try
            ErrorProvider1.Clear()

            If txtoldpassword.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(txtoldpassword, "Please provide old password")
                MsgBox("Please provide old password", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                txtoldpassword.Focus()
                Return False
            ElseIf txtnewpassword.Text.Trim.Length = 0 Then
                ErrorProvider1.SetError(txtnewpassword, "Please provide new password")
                MsgBox("Please provide new password", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                txtnewpassword.Focus()
                Return False
            ElseIf txtnewpassword.Text <> txtconfirmpassword.Text Then
                ErrorProvider1.SetError(txtnewpassword, "Confirmation password does not match")
                MsgBox("Confirmation password does not match", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, Me.Text)
                txtnewpassword.Focus()
                Return False
            End If

            'ErrorProvider1.Clear()

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
End Class