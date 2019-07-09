Imports OVCSystem.functions
Imports OVCSystem.AppSecurity
Imports System.Data.Odbc
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient

Public Class frmUserManagement
    Private Sub Button1_Click()
        'SetMenuItem("UserManagementToolStripMenuItem", False)
        Dim menues As New List(Of ToolStripItem)
        For Each t As ToolStripItem In MDIMain.MenuStrip.Items
            GetMenues(t, menues)
        Next

        Dim msg As String = ""
        For Each t As ToolStripItem In menues
            msg &= t.Name & vbCrLf
        Next
        MessageBox.Show(msg)
    End Sub
    Public Sub GetMenues(ByVal Current As ToolStripItem, ByRef menues As List(Of ToolStripItem))
        menues.Add(Current)
        If TypeOf (Current) Is ToolStripMenuItem Then
            For Each menu As ToolStripItem In DirectCast(Current, ToolStripMenuItem).DropDownItems
                GetMenues(menu, menues)
            Next
        End If
    End Sub

    'Private Function SetMenuItem(ByVal name As String, ByVal enabled As Boolean)
    '    Dim m As ToolStripMenuItem
    '    m = Me.FindToolStripMenuItem(MDIMain.MenuStrip.Items, name)
    '    If m IsNot Nothing Then
    '        m.Enabled = enabled
    '        Return True
    '    Else
    '        Return False
    '    End If
    'End Function

    'Private Function FindToolStripMenuItem(ByRef menus As ToolStripItemCollection, ByVal name As String) As ToolStripMenuItem
    '    Dim found As Boolean = False
    '    Dim t, temp As ToolStripMenuItem
    '    t = menus(name)
    '    If t Is Nothing Then
    '        Dim i As Integer = 0
    '        While Not found And i < menus.Count
    '            If menus(i).GetType Is GetType(ToolStripMenuItem) Then
    '                temp = menus(i)
    '                t = Me.FindToolStripMenuItem(temp.DropDownItems, name)
    '                found = (t IsNot Nothing)
    '            End If
    '            i += 1
    '        End While
    '    End If
    '    Return t
    'End Function
    Private Sub PopulateCBOs(ByVal strusername As String)
        'This part effects the selections immediately
        Dim mysqlaction As String = ""
        Dim MyDBAction As New functions
        Dim myarraylist As New ArrayList
        Dim mycboarraylist As New ArrayList
        'myarraylist = ReadXML("mysettings.xml")

        Dim MyDatable As New Data.DataTable
        mysqlaction = "select cbolist from users where username = '" & strusername & "'"
        MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
        strcbos = MyDatable.Rows(0).Item(0).ToString
        mycboarraylist.AddRange(Split(strcbos, ","))

        For i = 0 To mycboarraylist.Count - 1
            For Each MyNode In TreeView1.Nodes 'Loop thru level1 [Districts]

                For Each MyChildNode In MyNode.Nodes 'loop through Cbos
                    If MyChildNode.Tag.ToString = mycboarraylist.Item(i).ToString Then
                        MyChildNode.Checked = True 'check the services
                        Exit For
                    End If
                Next

            Next
        Next


    End Sub
    Private Sub populatemenus()
        lstMenus.Items.Clear()
        Dim menues As New List(Of ToolStripItem)
        For Each t As ToolStripItem In MDIMain.MenuStrip.Items
            GetMenues(t, menues)
        Next

        For Each t As ToolStripItem In menues
            lstMenus.Items.Add(t.Name)
        Next
    End Sub

    Private Sub AddRootNode()
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        Dim ErrorAction As New functions
        Try

            TreeView1.CheckBoxes = True
            Dim MyDomainName As String = ""
            Dim MyDomainid As String = ""
            Dim MnuCount As Int16 = 0
            Dim j As Int16

            'Dim MyDataTable As New Data.DataTable
            Dim sqlAction As String = "Select Distinct districtid,district from district order by district"


            TreeView1.Nodes.Clear() 'first remove any nodes

            Dim MyDBAction As New functions
            'MyDataTable = TryCast(MyDBAction.DBAction(sqlAction, DBActionType.DataTable), Data.DataTable)

            Dim cmd As New SqlCommand(sqlAction, conn)
            conn.Open()
            Dim myreader As SqlDataReader = cmd.ExecuteReader()


            'If MyDataTable.Rows.Count > 0 Then
            '    MnuCount = MyDataTable.Rows.Count
            '    For j = 0 To MyDataTable.Rows.Count - 1
            Do While myreader.Read
                MyDomainName = myreader("district").ToString
                MyDomainid = myreader("districtid").ToString
                Dim MyMainNode As New TreeNode
                MyMainNode.Tag = MyDomainid
                MyMainNode.Text = MyDomainName.ToUpper.ToString
                AddChildNode(MyDomainid.ToString, MyMainNode) 'Add the cbos for each district respectively
                TreeView1.Nodes.Add(MyMainNode)
            Loop


            '    Next

            'End If

            TreeView1.ExpandAll()

        Catch ex As Exception
            MsgBox(ex.Message)
            ErrorAction.WriteToErrorLogFile("UserManagment", "AddRootNode", ex.Message) ''---Write error to error log file
        Finally
            conn.Close()
        End Try
    End Sub

    Private Sub AddChildNode(ByVal strMainDistrictid As String, ByRef MyNode As TreeNode)
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)
        Dim ErrorAction As New functions
        Try
            Dim MyCoreserviceName As String = ""
            Dim MycoreserviceID As String = ""
            Dim MyURL As String = ""
            Dim MnuCount As Int16 = 0
            Dim j As Int16

            'Dim MyDataTable As New Data.DataTable



            Dim sqlAction As String = "Select * from cbo where districtid = '" & strMainDistrictid.ToString & "'"


            Dim MyDBAction As New functions
            'MyDataTable = TryCast(MyDBAction.DBAction(sqlAction, DBActionType.DataTable), Data.DataTable)

            Dim cmd As New SqlCommand(sqlAction, conn)
            conn.Open()
            Dim myreader As SqlDataReader = cmd.ExecuteReader()

            'If MyDataTable.Rows.Count > 0 Then
            '    MnuCount = MyDataTable.Rows.Count
            '    For j = 0 To MyDataTable.Rows.Count - 1
            Do While myreader.Read
                MyCoreserviceName = myreader("cbo").ToString
                MycoreserviceID = myreader("cboid").ToString

                Dim MySubNode As New TreeNode
                MySubNode.Tag = MycoreserviceID
                MySubNode.Text = MyCoreserviceName

                'AddStatusNode(MySubNode, MycoreserviceID)

                MyNode.Nodes.Add(MySubNode)
            Loop

            '    Next

            'End If

        Catch ex As Exception
            MsgBox(ex.Message)
            ErrorAction.WriteToErrorLogFile("UserManagment", "AddChildNode", ex.Message) ''---Write error to error log file
        Finally
            conn.Close()
        End Try
    End Sub
    Private Sub frmUserManagement_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        populatemenus()
        'AddRootNode()
        fillgrid()
    End Sub

    Private Sub BtnEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnEdit.Click
        Dim ErrorAction As New functions
        Try
            'delete record
            Dim confirm As Boolean
            confirm = MsgBox("Confirm Edit of user", MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel)

            If confirm = True Then
                Dim mySqlAction As String = ""
                Dim MyDBAction As New functions

                'first delete userrights entries for this user
                mySqlAction = "delete from userrights " & _
                "  where username = '" & txtUserName.Text & "'"
                MyDBAction.DBAction(mySqlAction, functions.DBActionType.Delete)


                'save them afresh
                ' Loop through all the selected items of the listbox and save user-menus
                ' Determine if there are any items checked.
                If lstMenus.CheckedItems.Count <> 0 Then
                    ' If so, loop through all checked items and print results.
                    Dim x As Integer
                    For x = 0 To lstMenus.CheckedItems.Count - 1
                        mySqlAction = "Insert Into userrights(username,menuid,canaccess) " & _
                                    " values('" & txtUserName.Text.ToString & "','" & lstMenus.CheckedItems(x).ToString & "','True')"
                        MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
                    Next x

                End If

                '--------district and cbo listing -----
                'save user cbos
                'There are 2 levels on the treeview District-CBO
                Dim MyMainDistrictid As String = ""
                Dim Mycboid As String = ""
                Dim Mystatusid As String = ""
                'Dim myarraylist As New ArrayList
                Dim mydistrictlist As String = ""
                Dim mycbolist As String = ""
                Dim mydistrictcbolist As String = ""

                'There are 2 levels on the treeview District-CBO
                Dim MyNode As New TreeNode
                Dim MyChildNode As New TreeNode

                For Each MyNode In TreeView1.Nodes 'Loop thru level1 [Districts]
                    MyMainDistrictid = MyNode.Tag.ToString
                    'If MyNode.Nodes.Count > 0 Then

                    For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [cbos]
                        If MyChildNode.Checked = True Then
                            Mycboid = MyChildNode.Tag.ToString
                            If mydistrictlist.Length = 0 Then
                                mydistrictlist = MyMainDistrictid
                            Else
                                mydistrictlist = mydistrictlist & "," & MyMainDistrictid
                            End If
                            If mycbolist.Length = 0 Then
                                mycbolist = Mycboid
                            Else
                                mycbolist = mycbolist & "," & Mycboid
                            End If
                            If mydistrictlist.Length = 0 Then
                                mydistrictlist = MyMainDistrictid
                            Else
                                mydistrictlist = mydistrictlist & "," & MyMainDistrictid
                            End If


                        End If
                        ' MsgBox(MyMainDistrictid & " " & Mycboid)
                    Next

                Next

                'Edit district and cbos for user

                mySqlAction = "update users set " & _
                " districtlist = '" & mydistrictlist.ToString & "', cbolist = '" & mycbolist.ToString & "' " & _
                ", allowdelete = '" & CBool(chkallowdelete.Checked) & "' " & _
                "where username = '" & txtUserName.Text.ToString & "'"
                MyDBAction.DBAction(mySqlAction, functions.DBActionType.Update)
                '----------------------------------------

               

                MsgBox("User Edited successfully.", MsgBoxStyle.Information)
            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("UserManagement", "Edit", ex.Message) ''---Write error to error log file
            MsgBox(Err.Description & "+" & Err.Number & "+" & ErrorToString())
        End Try
      
    End Sub

    Private Sub BtnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnDelete.Click
        Dim ErrorAction As New functions
        Try
            'delete record
            Dim confirm As Boolean
            confirm = MsgBox("Confirm Deletion of user", MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel)

            If confirm = True Then
                Dim mySqlAction As String = ""
                Dim MyDBAction As New functions
                mySqlAction = "delete from users " & _
                "  where username = '" & txtUserName.Text & "'"
                MyDBAction.DBAction(mySqlAction, functions.DBActionType.Delete)

                mySqlAction = "delete from usersrights " & _
                "  where username = '" & txtUserName.Text & "'"
                MyDBAction.DBAction(mySqlAction, functions.DBActionType.Delete)

                MsgBox("User deleted successfully.", MsgBoxStyle.Information)

                fillgrid()

                Panel1.Enabled = False
                BtnDelete.Enabled = False
                BtnEdit.Enabled = False

                TabControl1.SelectedIndex = 0
            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("UserManagement", "Delete", ex.Message) ''---Write error to error log file
            MsgBox(Err.Description & "+" & Err.Number & "+" & ErrorToString())
        End Try
    End Sub

    Private Sub btnpost_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnpost.Click
        Dim ErrorAction As New functions
        Try
            'first encrypt the password string
            Dim myAppCrypto As New AppSecurity
            Dim encryptedpassword As String = myAppCrypto.encryptString(txtpassword.Text)

            '--------district and cbo listing -----
            'save user cbos
            'There are 2 levels on the treeview District-CBO
            Dim MyMainDistrictid As String = ""
            Dim Mycboid As String = ""
            Dim Mystatusid As String = ""
            'Dim myarraylist As New ArrayList
            Dim mydistrictlist As String = ""
            Dim mycbolist As String = ""
            Dim mydistrictcbolist As String = ""

            'There are 2 levels on the treeview District-CBO
            Dim MyNode As New TreeNode
            Dim MyChildNode As New TreeNode

            For Each MyNode In TreeView1.Nodes 'Loop thru level1 [Districts]
                MyMainDistrictid = MyNode.Tag.ToString
                'If MyNode.Nodes.Count > 0 Then

                For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [cbos]
                    If MyChildNode.Checked = True Then
                        Mycboid = MyChildNode.Tag.ToString
                        If mydistrictlist.Length = 0 Then
                            mydistrictlist = MyMainDistrictid
                        Else
                            mydistrictlist = mydistrictlist & "," & MyMainDistrictid
                        End If
                        If mycbolist.Length = 0 Then
                            mycbolist = Mycboid
                        Else
                            mycbolist = mycbolist & "," & Mycboid
                        End If
                        If mydistrictlist.Length = 0 Then
                            mydistrictlist = MyMainDistrictid
                        Else
                            mydistrictlist = mydistrictlist & "," & MyMainDistrictid
                        End If


                    End If
                    ' MsgBox(MyMainDistrictid & " " & Mycboid)
                Next

            Next
            '----------------------------------------

            'save record
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions
            mySqlAction = "Insert Into users(username,password,districtlist,cbolist,allowdelete,datecreated,createdby) " & _
            " values('" & txtUserName.Text.ToString & "','" & encryptedpassword.ToString & "'," & _
            "'" & mydistrictlist.ToString & "','" & mycbolist.ToString & "','" & CBool(chkallowdelete.Checked) & "'," & _
            "'" & Format(Date.Today, "dd-MMM-yyyy") & "','SYSTEM')"
            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)

            'save menu access details
            ' Loop through all the selected items of the listbox and save user-menus
            ' Determine if there are any items checked.
            If lstMenus.CheckedItems.Count <> 0 Then
                ' If so, loop through all checked items and print results.
                Dim x As Integer
                For x = 0 To lstMenus.CheckedItems.Count - 1
                    mySqlAction = "Insert Into userrights(username,menuid,canaccess) " & _
                                " values('" & txtUserName.Text.ToString & "','" & lstMenus.CheckedItems(x).ToString & "','True')"
                    MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)
                Next x

            End If

           
           

            MsgBox("User saved successfully.", MsgBoxStyle.Information)

            Panel1.Enabled = False
            btnpost.Enabled = False
            fillgrid()
            TabControl1.SelectedIndex = 1
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("UserManagement", "save", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub BtnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnExit.Click
        Me.Close()
    End Sub


    Private Sub fillgrid()
        Dim ErrorAction As New functions
        Try

            'populate the datagrid with all the data
            Dim mySqlAction As String = "select * from users order by username,datecreated,createdby"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim myusername, mydatecreated, mycreatedby, myallowdelete As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView1.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    myusername = MyDatable.Rows(K).Item("username").ToString
                    mydatecreated = Format(CDate(MyDatable.Rows(K).Item("datecreated").ToString), "dd-MMM-yyyy")
                    mycreatedby = MyDatable.Rows(K).Item("createdby").ToString
                    myallowdelete = IIf(IsDBNull(MyDatable.Rows(K).Item("allowdelete")), 0, (MyDatable.Rows(K).Item("allowdelete")))
                    DataGridView1.Rows.Add(myusername, mycreatedby, mydatecreated, "Select", myallowdelete)
                Next
            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("UserManagement", "fillgrid", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub lnkNew_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkNew.LinkClicked
        txtpassword.Enabled = True
        txtUserName.Enabled = True
        Panel1.Enabled = True
        lstMenus.Enabled = True
        TreeView1.Enabled = True
        btnpost.Enabled = True
        BtnDelete.Enabled = False
        BtnEdit.Enabled = False
        txtUserName.Focus()
        txtUserName.Text = ""
        txtpassword.Text = ""
        populatemenus()
    End Sub

    Private Sub DataGridView1_CellContentClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        Dim ErrorAction As New functions
        Try
            'select item to edit or delete
            Dim mysqlaction As String = ""
            Dim MyDBAction As New functions
            Dim K As Integer

            K = e.RowIndex
            txtUserName.Text = Me.DataGridView1.Rows(K).Cells(0).Value
            chkallowdelete.Checked = CBool(Me.DataGridView1.Rows(K).Cells(4).Value)

            Dim MyDatable As New Data.DataTable
            mysqlaction = "select * from userrights where username = '" & Me.DataGridView1.Rows(K).Cells(0).Value & "'"
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)

            'clear all selections by reloading fresh menus on list
            populatemenus()

            PopulateCBOs(Me.DataGridView1.Rows(K).Cells(0).Value)

            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    'select menus viewable by this user
                    Dim x As Integer
                    For x = 0 To lstMenus.Items.Count - 1
                        If lstMenus.Items(x).ToString = MyDatable.Rows(K).Item("menuid").ToString Then
                            lstMenus.SetItemChecked(x, True)
                        End If

                    Next x

                Next

            End If
            txtpassword.Enabled = False
            txtUserName.Enabled = False
            Panel1.Enabled = True
            BtnEdit.Enabled = True
            BtnDelete.Enabled = True
            btnpost.Enabled = False
            lstMenus.Enabled = True
            TreeView1.Enabled = True

            TabControl1.SelectedIndex = 0

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("UserManagement", "Cellcontentclick", ex.Message) ''---Write error to error log file

        End Try
    End Sub
End Class