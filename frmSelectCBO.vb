Imports OVCSystem.functions
Imports System.Data.Odbc
Imports System.Configuration.ConfigurationManager
Imports System.Xml
Imports System.Text

Public Class frmSelectCBO
    Private Sub PopulateCBOs()
        'This part effects the selections immediately
        'read from the settings xml file to get the districts and cbos to work with
        Dim myarraylist As New ArrayList
        Dim mycboarraylist As New ArrayList
        myarraylist = ReadXML("mysettings.xml")
        strcbos = myarraylist.Item(1).ToString

        If myarraylist.Count > 3 Then
            strimplementingpartner = myarraylist.Item(3).ToString
        Else
            strimplementingpartner = ""
            strimplementingpartnerid = -1
        End If
        If myarraylist.Count > 4 Then
            strimplementingpartnerid = myarraylist.Item(4).ToString
        Else
            strimplementingpartner = ""
            strimplementingpartnerid = -1
        End If

        cboImplementingPartner.SelectedValue = strimplementingpartnerid

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

    Private Sub populateimplementingpartners()
        Dim ErrorAction As New functions
        Try

            'populate the combobox
            Dim mySqlAction As String = "select * from implementingpartners order by Partnername asc"
            Dim MyDBAction As New functions
            Dim MyDatable As New Data.DataTable
            MyDatable = TryCast(MyDBAction.DBAction(mySqlAction, DBActionType.DataTable), Data.DataTable)
            With cboImplementingPartner
                .Items.Clear()
                .DataSource = MyDatable
                .DisplayMember = "Partnername"
                .ValueMember = "partnerid"
                .SelectedIndex = -1 ' This line makes the combo default value to be blank
            End With
        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("selectcbo", "populateimplementingpartners", ex.Message) ''---Write error to error log file

        End Try
    End Sub
    Private Sub frmSelectCBO_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        populateimplementingpartners()
        AddRootNode()
        PopulateCBOs()

        'Show filtercbo only for admin
        If strusername.ToString.ToLower = "admin" Then
            btnfilter.Visible = True
        Else
            btnfilter.Visible = False
        End If

    End Sub
    Private Sub AddRootNode()
        TreeView1.CheckBoxes = True
        Dim MyDomainName As String = ""
        Dim MyDomainid As String = ""
        Dim MnuCount As Int16 = 0
        Dim j As Int16

        Dim MyDataTable As New Data.DataTable
        Dim sqlAction As String = "Select Distinct districtid,district from district order by district"


        TreeView1.Nodes.Clear() 'first remove any nodes

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
                TreeView1.Nodes.Add(MyMainNode)

            Next

        End If

        TreeView1.ExpandAll()
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

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click

        Dim MyMainDistrictid As String = ""
        Dim Mycboid As String = ""
        Dim Mystatusid As String = ""
        'Dim myarraylist As New ArrayList
        Dim mydistrictlist As String = ""
        Dim mycbolist As String = ""
        Dim mydistrictcbolist As String = ""
        Dim myimplementingpartner As String = ""
        Dim myimplementingpartnerid As Int16 = -1
        Dim myftphost As String = ""
        Dim myftpusername As String = ""
        Dim myftppassword As String = ""
        Dim Issuccess As Boolean

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
                    If mydistrictcbolist.Length = 0 Then
                        mydistrictcbolist = "(" & MyMainDistrictid & "-" & Mycboid & ")"
                    Else
                        mydistrictcbolist = mydistrictcbolist & ",(" & MyMainDistrictid & "-" & Mycboid & ")"
                    End If


                End If
                ' MsgBox(MyMainDistrictid & " " & Mycboid)
            Next

        Next

        Dim myAppCrypto As New AppSecurity


        'save the folder which will host updates in the server, and also ftp credentials
        myimplementingpartner = cboImplementingPartner.Text
        myimplementingpartnerid = cboImplementingPartner.SelectedValue
        myftphost = txtFtpHost.Text.ToString
        myftpusername = txtftpusername.Text.ToString
        myftppassword = myAppCrypto.encryptString(txtftppassword.Text.ToString).ToString

        Issuccess = WriteXML("mysettings.xml", mydistrictlist, mycbolist, mydistrictcbolist, myimplementingpartner, myimplementingpartnerid,
                             myftphost, myftpusername, myftppassword)

        If Issuccess = True Then
            MsgBox("Details Successfully Saved.", MsgBoxStyle.Information)
        Else
            MsgBox("Details NOT Saved.", MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        'This part effects the selections immediately
        'read from the settings xml file to get the districts and cbos to work with
        Dim myarraylist As New ArrayList
        myarraylist = ReadXML("mysettings.xml")
        strdistricts = myarraylist.Item(0).ToString
        strcbos = myarraylist.Item(1).ToString
        strdistrictcbo = myarraylist.Item(2).ToString
        strimplementingpartner = myarraylist.Item(3).ToString
        strimplementingpartnerid = myarraylist.Item(4).ToString
        strftphost = myarraylist.Item(5).ToString
        strftpusername = myarraylist.Item(6).ToString
        strftppassword = myarraylist.Item(7).ToString


    End Sub
    Private Function WriteXML(ByVal xmlfile As String, ByVal mydistrictlist As String,
                              ByVal mycbolist As String, ByVal mydistrictcbolist As String,
                              ByVal myimplementingpartner As String, ByVal myimplementingpartnerid As Int16,
                              ByVal myftphost As String, ByVal myftpusername As String, ByVal myftppassword As String) As Boolean
        Try



            Dim enc As Encoding
            'Create file, overwrite if exists
            'enc is encoding object required by constructor
            'It is null, so default encoding is used
            Dim objXMLTW As New XmlTextWriter(xmlfile, enc)
            objXMLTW.WriteStartDocument()
            'Top level (Parent element)
            objXMLTW.WriteStartElement("Settings")

            'Child elements, from request form
            objXMLTW.WriteStartElement("districtlist")
            objXMLTW.WriteString(mydistrictlist)
            objXMLTW.WriteEndElement()

            objXMLTW.WriteStartElement("cbolist")
            objXMLTW.WriteString(mycbolist)
            objXMLTW.WriteEndElement()

            objXMLTW.WriteStartElement("districtcbolist")
            objXMLTW.WriteString(mydistrictcbolist)
            objXMLTW.WriteEndElement()

            objXMLTW.WriteStartElement("implementingpartner")
            objXMLTW.WriteString(myimplementingpartner)
            objXMLTW.WriteEndElement()

            objXMLTW.WriteStartElement("implementingpartnerid")
            objXMLTW.WriteString(myimplementingpartnerid)
            objXMLTW.WriteEndElement()

            objXMLTW.WriteStartElement("ftphost")
            objXMLTW.WriteString(myftphost)
            objXMLTW.WriteEndElement()

            objXMLTW.WriteStartElement("ftpusername")
            objXMLTW.WriteString(myftpusername)
            objXMLTW.WriteEndElement()

            objXMLTW.WriteStartElement("ftppassword")
            objXMLTW.WriteString(myftppassword)
            objXMLTW.WriteEndElement()

            objXMLTW.WriteEndElement() 'End top level element
            objXMLTW.WriteEndDocument() 'End Document
            objXMLTW.Flush() 'Write to file
            objXMLTW.Close()

            Return True

        Catch Ex As Exception
            MsgBox("The following error occurred: " & Ex.Message)
            Return False
        End Try
    End Function

    Private Function ReadXML(ByVal FileName As String) As ArrayList
        Try


            Dim objXMLTR As New XmlTextReader(FileName)
            Dim sCategory As String = ""
            Dim bNested As Boolean
            Dim sLastElement As String = ""
            Dim sValue As String = ""
            Dim myarraylist As New ArrayList

            'Read method loops through the XML stream
            Do While objXMLTR.Read

                'Output elements and values
                If objXMLTR.NodeType = XmlNodeType.Element Then
                    If bNested = True Then
                        If sCategory <> "" Then sCategory = sCategory '& " > "
                        sCategory = sCategory & sLastElement
                    End If
                    bNested = True
                    sLastElement = objXMLTR.Name

                ElseIf objXMLTR.NodeType = XmlNodeType.Text Or _
                  objXMLTR.NodeType = XmlNodeType.CDATA Then
                    bNested = False
                    sCategory = sCategory
                    sValue = objXMLTR.Value
                    '
                    'place the values of districts and cbos in an array. We will split later
                    'districtlist - cbolist - districtcbolist
                    myarraylist.Add(sValue)

                    sLastElement = ""
                    sCategory = ""
                End If
            Loop
            objXMLTR.Close()

            Return myarraylist
            'For j = 0 To (myarraylist.Count - 1)
            '    'Loop through the list of RefenceNumbers
            '    MsgBox(myarraylist.Item(j).ToString)

            'Next

        Catch Ex As Exception
            'MsgBox("The following error occurred: " & Ex.Message)
        End Try

    End Function


    Private Sub btnSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelectAll.Click
        'Check what is selected or not selected and make sure that all are selected
        Dim MyNode As New TreeNode
        Dim MyChildNode As New TreeNode
       
        For Each MyNode In TreeView1.Nodes 'Loop thru level1 [Districts]

            For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [CBOs]

                'if not selected, select it
                If MyChildNode.Checked = False Then
                    MyChildNode.Checked = True

                End If

            Next

        Next
    End Sub

    Private Sub btnUnselectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUnselectAll.Click
        'Check what is selected or not selected and make sure that all are selected
        Dim MyNode As New TreeNode
        Dim MyChildNode As New TreeNode

        For Each MyNode In TreeView1.Nodes 'Loop thru level1 [Districts]

            For Each MyChildNode In MyNode.Nodes 'Loop thru level2 [CBOs]

                'if selected, unselect it
                If MyChildNode.Checked = True Then
                    MyChildNode.Checked = False

                End If

            Next

        Next
    End Sub

    Private Sub btnfilter_Click(sender As Object, e As EventArgs) Handles btnfilter.Click
        'first delete records from clientdetails for OVC outside CBOs chosen
        Dim ErrorAction As New functions
        Try
            Dim MyDBAction As New functions
            'delete record
            Dim confirm As Boolean
            confirm = MsgBox("Confirm Deletion of record", MsgBoxStyle.Exclamation + MsgBoxStyle.OkCancel)

            If confirm = True Then
                Dim mySqlAction As String = ""

                mySqlAction = "delete from clientdetails " &
                "  where cbo NOT IN (" & strcbos & ")"

                MyDBAction.DBAction(mySqlAction, functions.DBActionType.Delete)


                '' ''mySqlAction = " use [master] " & _
                '' ''"DBCC SHRINKDATABASE (APHIAMAINDB)"
                '' ''MyDBAction.DBAction(mySqlAction, functions.DBActionType.Delete)

            End If


            'run procedure to clean data for OVCs deleted above
            If MyDBAction.FilterCBOData() = True Then
                MsgBox("CBO Data filtered successfully.", MsgBoxStyle.Information)
            End If


        Catch ex As Exception
            ErrorAction.WriteToErrorLogFile("FilterCBOData", "Delete", ex.Message) ''---Write error to error log file
            MsgBox("CBO Data NOT filtered successfully.", MsgBoxStyle.Exclamation)
        End Try
    End Sub
End Class