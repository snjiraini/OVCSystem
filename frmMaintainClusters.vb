Imports OVCSystem.functions
Imports System.Data.Odbc
Imports System.Configuration.ConfigurationManager
Imports System.Xml
Imports System.Text

Public Class frmMaintainClusters

    Private Sub frmMaintainClusters_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddRootNode()
        fillgrid()
    End Sub
    Private Sub AddRootNode()
        trvCBOs.CheckBoxes = True
        Dim MyDomainName As String = ""
        Dim MyDomainid As String = ""
        Dim MnuCount As Int16 = 0
        Dim j As Int16

        Dim MyDataTable As New Data.DataTable
        Dim sqlAction As String = "Select Distinct districtid,district from district order by district"


        trvCBOs.Nodes.Clear() 'first remove any nodes

        Dim MyDBAction As New functions
        MyDataTable = TryCast(MyDBAction.DBAction(sqlAction, DBActionType.DataTable), Data.DataTable)

        If MyDataTable.Rows.Count > 0 Then
            MnuCount = MyDataTable.Rows.Count
            For j = 0 To MyDataTable.Rows.Count - 1
                MyDomainName = MyDataTable.Rows(j).Item("district").ToString
                MyDomainid = MyDataTable.Rows(j).Item("districtid").ToString
                Dim MyMainNode As New TreeNode
                MyMainNode.Tag = MyDomainid
                MyMainNode.Text = MyDomainName.ToUpper.ToString
                AddChildNode(MyDomainid.ToString, MyMainNode) 'Add the cbos for each district respectively
                trvCBOs.Nodes.Add(MyMainNode)

            Next

        End If

        trvCBOs.ExpandAll()
    End Sub

    Private Sub AddChildNode(ByVal strMainDistrictid As String, ByRef MyNode As TreeNode)

        Dim MyCoreserviceName As String = ""
        Dim MycoreserviceID As String = ""
        Dim MyURL As String = ""
        Dim MnuCount As Int16 = 0
        Dim j As Int16

        Dim MyDataTable As New Data.DataTable



        Dim sqlAction As String = "Select * from cbo where districtid = '" & strMainDistrictid.ToString & "'"


        Dim MyDBAction As New functions
        MyDataTable = TryCast(MyDBAction.DBAction(sqlAction, DBActionType.DataTable), Data.DataTable)

        If MyDataTable.Rows.Count > 0 Then
            MnuCount = MyDataTable.Rows.Count
            For j = 0 To MyDataTable.Rows.Count - 1
                MyCoreserviceName = MyDataTable.Rows(j).Item("cbo").ToString
                MycoreserviceID = MyDataTable.Rows(j).Item("cboid").ToString

                Dim MySubNode As New TreeNode
                MySubNode.Tag = MycoreserviceID
                MySubNode.Text = MyCoreserviceName

                'AddStatusNode(MySubNode, MycoreserviceID)

                MyNode.Nodes.Add(MySubNode)
            Next

        End If
    End Sub

    Private Sub fillgrid()
        Dim ErrorAction As New functions
        Try
            'populate the datagrid with all the data
            Dim mySqlAction As String = "select clusterid,ClusterName from Clusters order by ClusterName"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            Dim myclusterid, myclusterName As String
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            DataGridView1.Rows.Clear()
            If MyDatable.Rows.Count > 0 Then
                For K = 0 To MyDatable.Rows.Count - 1
                    myclusterid = MyDatable.Rows(K).Item("clusterid").ToString
                    myclusterName = MyDatable.Rows(K).Item("ClusterName").ToString
                    DataGridView1.Rows.Add(myclusterid, myclusterName, "Select")
                Next
            End If
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("Maintain Clusters", "fillgrid", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub lnkNew_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkNew.LinkClicked
        'Check what is selected or not selected and make sure that all are selected
        Dim MyNode As New TreeNode
        Dim MyChildNode As New TreeNode

        For Each MyNode In trvCBOs.Nodes 'Loop thru level1 [Districts]

            For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [CBOs]

                'if selected, unselect it
                If MyChildNode.Checked = True Then
                    MyChildNode.Checked = False

                End If

            Next

        Next

        Panel3.Enabled = True
        cmdSave.Enabled = True
        cmdDelete.Enabled = False
        cmdEdit.Enabled = False
        trvCBOs.Enabled = True
        txtClustername.Enabled = True
        txtClusterID.Text = ""
        txtClustername.Text = ""
        txtClustername.Focus()
    End Sub

    Private Sub cmdSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSave.Click
        Dim ErrorAction As New functions

        Try
            Dim myDistrictid As String = ""
            Dim myCBOid As String = ""
            Dim myCBO_Cluster_List As String = ""
            Dim Myrandomkey As New Random

            'Loop thru clustered cbo selections to save
            For Each MyNode In trvCBOs.Nodes 'Loop thru level1 [Districts]
                myDistrictid = MyNode.Tag.ToString
                'If MyNode.Nodes.Count > 0 Then

                For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [CBOs]

                    If MyChildNode.Checked = True Then
                        myCBOid = MyChildNode.Tag.ToString & ","

                        'save to the servicesprovided table
                        'Dim myClusterid As String = "CLUSTER" & "-" & Format(Date.Now, "yyyyMMddhhmmss") & "-" & Myrandomkey.Next 'unique row id.
                        myCBO_Cluster_List = myCBO_Cluster_List & myCBOid
                    End If
                Next
            Next

            'save to clusters table
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions

            'This line [myCBO_Cluster_List.Remove(myCBO_Cluster_List.Length - 1).ToString]...deletes the last character before saving
            'e.g "Sammy," becomes "Sammy"
            mySqlAction = "Insert Into clusters(ClusterName,ClusterCBOs) " & _
            " values('" & txtClustername.Text.ToString & "','" & myCBO_Cluster_List.Remove(myCBO_Cluster_List.Length - 1).ToString & "')"

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)

            MsgBox("Cluster saved successfully.", MsgBoxStyle.Information)

            fillgrid()

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("Maintain Clusters", "save clusters", ex.Message) ''---Write error to error log file
            MsgBox("Record NOT saved successfully.", MsgBoxStyle.Exclamation)

        End Try
    End Sub

    Private Sub populate_clustercbos(ByVal mycbolist As ArrayList)

        For i = 0 To mycbolist.Count - 1
            For Each MyNode In trvCBOs.Nodes 'Loop thru level1 [Districts]

                For Each MyChildNode In MyNode.Nodes 'loop through Cbos
                    If MyChildNode.Tag.ToString = mycbolist.Item(i).ToString Then
                        MyChildNode.Checked = True 'check the services
                        Exit For
                    End If
                Next

            Next
        Next
    End Sub

    Private Sub DataGridView1_CellContentClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        Dim ErrorAction As New functions
        Try
            AddRootNode()

            'select item to edit or delete
            Dim mysqlaction As String = ""
            Dim MyDBAction As New functions
            Dim K As Integer

            K = e.RowIndex

            Dim MyDatable As New Data.DataTable
            Dim MyCbolist As New ArrayList

            mysqlaction = "SELECT * FROM   Clusters WHERE ClusterID ='" & Me.DataGridView1.Rows(K).Cells(0).Value & "'"
            MyDatable = TryCast(MyDBAction.DBAction(mysqlaction, DBActionType.DataTable), Data.DataTable)
            If MyDatable.Rows.Count > 0 Then
                txtClusterID.Text = MyDatable.Rows(0).Item("ClusterId")
                txtClustername.Text = MyDatable.Rows(0).Item("ClusterName")
                'repopulate ClusterCBOs
                MyCbolist.AddRange(Split(MyDatable.Rows(0).Item("ClusterCBOs"), ","))
                populate_clustercbos(MyCbolist)

            End If

            'take focus back to the first tab
            TabControl1.SelectedIndex = 0


            Panel3.Enabled = True
            txtClustername.Enabled = True
            cmdEdit.Enabled = True
            cmdDelete.Enabled = True
            cmdSave.Enabled = False
            trvCBOs.Enabled = True
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("NeedsAssessment", "Cellcontentclick", ex.Message) ''---Write error to error log file

        End Try
    End Sub

    Private Sub cmdEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdEdit.Click
        Dim ErrorAction As New functions

        Try

            '1. first delete from clusters table----
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions


            mySqlAction = "delete from clusters where clusterid = '" & txtClusterID.Text & "'"

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Delete)

            '1.--------------------

            '2. Save afresh the new selections-----
            Dim myDistrictid As String = ""
            Dim myCBOid As String = ""
            Dim myCBO_Cluster_List As String = ""
            Dim Myrandomkey As New Random

            'Loop thru clustered cbo selections to save
            For Each MyNode In trvCBOs.Nodes 'Loop thru level1 [Districts]
                myDistrictid = MyNode.Tag.ToString
                'If MyNode.Nodes.Count > 0 Then

                For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [CBOs]

                    If MyChildNode.Checked = True Then
                        myCBOid = MyChildNode.Tag.ToString & ","

                        'save to the servicesprovided table
                        'Dim myClusterid As String = "CLUSTER" & "-" & Format(Date.Now, "yyyyMMddhhmmss") & "-" & Myrandomkey.Next 'unique row id.
                        myCBO_Cluster_List = myCBO_Cluster_List & myCBOid
                    End If
                Next
            Next

           

            'This line [myCBO_Cluster_List.Remove(myCBO_Cluster_List.Length - 1).ToString]...deletes the last character before saving
            'e.g "Sammy," becomes "Sammy"
            mySqlAction = "Insert Into clusters(ClusterName,ClusterCBOs) " & _
            " values('" & txtClustername.Text.ToString & "','" & myCBO_Cluster_List.Remove(myCBO_Cluster_List.Length - 1).ToString & "')"

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Insert)

            MsgBox("Cluster Edited successfully.", MsgBoxStyle.Information)

            '2.-----------

            Panel3.Enabled = True
            cmdSave.Enabled = False
            cmdDelete.Enabled = False
            cmdEdit.Enabled = False
            trvCBOs.Enabled = False
            txtClustername.Enabled = False

            fillgrid()

            TabControl1.SelectedIndex = 0

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("Maintain Clusters", "Edit clusters", ex.Message) ''---Write error to error log file
            MsgBox("Record NOT Edited successfully.", MsgBoxStyle.Exclamation)
        End Try
    End Sub

    Private Sub cmdDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdDelete.Click
        Dim ErrorAction As New functions

        Try

            'delete from clusters table
            Dim mySqlAction As String = ""
            Dim MyDBAction As New functions


            mySqlAction = "delete from clusters where clusterid = '" & txtClusterID.Text & "'"

            MyDBAction.DBAction(mySqlAction, functions.DBActionType.Delete)

            MsgBox("Cluster Deleted successfully.", MsgBoxStyle.Information)

            fillgrid()

            'Check what is selected or not selected and make sure that all are selected
            Dim MyNode As New TreeNode
            Dim MyChildNode As New TreeNode

            For Each MyNode In trvCBOs.Nodes 'Loop thru level1 [Districts]

                For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [CBOs]

                    'if selected, unselect it
                    If MyChildNode.Checked = True Then
                        MyChildNode.Checked = False

                    End If

                Next

            Next

            Panel3.Enabled = True
            cmdSave.Enabled = False
            cmdDelete.Enabled = False
            cmdEdit.Enabled = False
            trvCBOs.Enabled = False
            txtClustername.Enabled = False
            txtClusterID.Text = ""
            txtClustername.Text = ""

            TabControl1.SelectedIndex = 1

        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("Maintain Clusters", "delete clusters", ex.Message) ''---Write error to error log file
            MsgBox("Record NOT Deleted successfully.", MsgBoxStyle.Exclamation)
        End Try
    End Sub

    Private Sub cmdClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdClose.Click
        Me.Close()
    End Sub
End Class