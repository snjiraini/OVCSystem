Imports System.Data.Odbc
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Xml
Imports OVCSystem.functions
Imports OVCSystem.AppSecurity
Imports System.IO
Imports System.Configuration

Public Class LoginForm

    ' TODO: Insert code to perform custom authentication using the provided username and password 
    ' (See http://go.microsoft.com/fwlink/?LinkId=35339).  
    ' The custom principal can then be attached to the current thread's principal as follows: 
    '     My.User.CurrentPrincipal = CustomPrincipal
    ' where CustomPrincipal is the IPrincipal implementation used to perform authentication. 
    ' Subsequently, My.User will return identity information encapsulated in the CustomPrincipal object
    ' such as the username, display name, etc.

    Private Sub OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK.Click
        Dim conn As New SqlConnection(ConnectionStrings(SelectedConnectionString).ToString)

        Try
            ' MsgBox(Format(Date.Today, "MMM"))
            ''authentication
            'Dim SqlAction As String = "select username,password,cbolist,districtlist from users where username = '" & UsernameTextBox.Text.ToString & "' "

            ''check if the system date has been messed with, by checking currentdate - most recent services date
            'Dim MyDBAction As New functions
            'Dim SqlAction As String = "SELECT top 1 dateofvisit from StatusAndServiceVisit order by dateofvisit desc"
            'Dim Mymaxdateofvisit As Date = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

            'If Date.Today < Mymaxdateofvisit Then
            '    MsgBox("Please check your system date", MsgBoxStyle.Exclamation)
            '    'End
            'End If


            Dim mydecryptedpass As String = ""


            Dim cmd As New SqlCommand("dbo.Login")
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.Add(New SqlParameter("@username", UsernameTextBox.Text.ToString))
            conn.Open()
            cmd.Connection = conn

            Dim myreader As SqlDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection)

            If myreader.Read Then
                Dim myAppCrypto As New AppSecurity
                mydecryptedpass = myAppCrypto.decryptString(myreader("password").ToString)
            Else 'username does not exist
                MsgBox("Username does NOT exist.", MsgBoxStyle.Exclamation, "Login")
                Exit Sub
            End If

            'compare if password entered is the same as the decrypted password
            If PasswordTextBox.Text.ToString = mydecryptedpass.ToString Then
                'see if to use universal cbo&districts or user specific cbo&districts
                If myreader("cbolist").ToString <> "" Then
                    strdistricts = myreader("districtlist").ToString
                    strcbos = myreader("cbolist").ToString
                Else
                    'read from the settings xml file to get the districts and cbos to work with
                    Dim myarraylist As New ArrayList
                    myarraylist = ReadXML("mysettings.xml")
                    strdistricts = myarraylist.Item(0).ToString
                    strcbos = myarraylist.Item(1).ToString
                    strdistrictcbo = myarraylist.Item(2).ToString
                    'strimplementingpartner = myarraylist.Item(3).ToString
                    'strimplementingpartnerid = myarraylist.Item(4).ToString
                End If

                'read from the settings xml file to get the online updates folder to work with
                Dim myarraylistImplementingpartner As New ArrayList
                myarraylistImplementingpartner = ReadXML("mysettings.xml")
                If myarraylistImplementingpartner.Count > 3 Then
                    strimplementingpartner = myarraylistImplementingpartner.Item(3).ToString
                Else
                    strimplementingpartner = ""
                    strimplementingpartnerid = -1
                End If

                If myarraylistImplementingpartner.Count > 4 Then
                    strimplementingpartnerid = myarraylistImplementingpartner.Item(4).ToString
                Else
                    strimplementingpartner = ""
                    strimplementingpartnerid = -1
                End If

                If myarraylistImplementingpartner.Count > 5 Then
                    strftphost = myarraylistImplementingpartner.Item(5).ToString
                Else
                    strftphost = ""
                End If

                If myarraylistImplementingpartner.Count > 6 Then
                    strftpusername = myarraylistImplementingpartner.Item(6).ToString
                Else
                    strftpusername = ""
                End If

                'Keep encryptedpassword in system variable
                If myarraylistImplementingpartner.Count > 7 Then
                    strftppassword = myarraylistImplementingpartner.Item(7).ToString
                Else
                    strftppassword = ""
                End If

                'Read settings to do with data entry deadlines and extensions
                'read from the settings xml file to get the dataentry validation durations and extensions
                Dim myAppCrypto As New AppSecurity
                Dim myarraylist1 As New ArrayList
                myarraylist1 = ReadXML("mydateextensions.xml")


                strregistrationdefault = myAppCrypto.decryptString(myarraylist1.Item(0).ToString)
                strexitsdefault = myAppCrypto.decryptString(myarraylist1.Item(1).ToString)
                strdatadefault = myAppCrypto.decryptString(myarraylist1.Item(2).ToString)
                strdatabacklogdefault = myAppCrypto.decryptString(myarraylist1.Item(3).ToString)
                strregistrationextension = myAppCrypto.decryptString(myarraylist1.Item(4).ToString)
                strexitsextension = myAppCrypto.decryptString(myarraylist1.Item(5).ToString)
                strdataextension = myAppCrypto.decryptString(myarraylist1.Item(6).ToString)
                strdatabacklogextension = myAppCrypto.decryptString(myarraylist1.Item(7).ToString)
                strdateofExtension = myAppCrypto.decryptString(myarraylist1.Item(8).ToString)
                strextensionexpiry = myAppCrypto.decryptString(myarraylist1.Item(9).ToString)

                'do a quick calculation to determine if extension of data entry is expired, and if so, revert back to defaults
                If DateDiff(DateInterval.Day, CDate(strdateofExtension.ToString), Date.Today) <= CInt(strextensionexpiry.ToString) Then
                    strregistrationdefault = strregistrationextension
                    strexitsdefault = strexitsextension
                    strdatadefault = strdataextension
                    strdatabacklogdefault = strdatabacklogextension

                    Dim strdateofexpiry As String = DateAdd(DateInterval.Day, CInt(strextensionexpiry.ToString), CDate(strdateofExtension.ToString))
                    Dim strdaystoexpiry As String = DateDiff(DateInterval.Day, CDate(DateAdd(DateInterval.Day, CInt(strextensionexpiry.ToString), CDate(strdateofExtension.ToString))), Date.Today)
                    MsgBox("Your extension for [backlog entries] will expire on " & strdateofexpiry & vbCr &
                          strdaystoexpiry & " days to go.", MsgBoxStyle.Exclamation)
                End If


                'can this guy be able to delete OVC data
                candelete = CBool(IIf(IsDBNull(myreader("allowdelete")), 0, myreader("allowdelete")))



                    ' Create a new instance of the child form.
                    mdiform = New MDIMain
                    mdiform.Text = "OVC Longitudinal Database Management System"
                    strusername = UsernameTextBox.Text.ToString



                    MDIMain.Show()
                    Me.Close()

                    'System.Diagnostics.Process.Start("C:\OVC\OlmisUpdates\updates.bat")
                Else
                    MsgBox("Wrong Password.", MsgBoxStyle.Exclamation, "Login")
                Exit Sub
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            conn.Close()
        End Try

    End Sub

    'Private Function ConfirmSystemUpdates() As Boolean
    '    Try
    '        'if none of the files and folders needed are available, return false.
    '        If File.Exists(Application.StartupPath & "\Version.txt") = False Or _
    '            File.Exists(Application.StartupPath & "\OlmisUpdates\Version.txt") = False Then
    '            Return False
    '        End If

    '        'if there is an error downloading the text file, then it immediately overwrites Olmisupdates/version.txt with blank file
    '        'if this happens, immediately replace the txt back with the one from C:\OVC\Version.txt
    '        'This avoids the issue of Version zero [0] and giving messages about the system not being up to date.
    '        If ReadVersionFile(Application.StartupPath & "\OlmisUpdates\Version.txt") = 0 Then
    '            My.Computer.FileSystem.CopyFile(
    '            Application.StartupPath & "\Version.txt",
    '            Application.StartupPath & "\OlmisUpdates\Version.txt", Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
    '            Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing)
    '        End If


    '        'Compare the Current version and the one on internet. If internet is newer, then download else, Exit sub
    '        intCurrentVersion = ReadVersionFile(Application.StartupPath & "\Version.txt")
    '        intOnlineVersion = ReadVersionFile(Application.StartupPath & "\OlmisUpdates\Version.txt")

    '        'check database version
    '        Dim MyDBAction As New functions
    '        Dim SqlAction As String = "SELECT top 1 dbver from Versions order by id desc"
    '            Dim Mydbversion As Int64 = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)
    '            If intCurrentVersion <> Mydbversion Then
    '            MsgBox("Your Application version is " & intCurrentVersion.ToString & vbCr & _
    '                   "Your Database version is " & Mydbversion.ToString & vbCr & _
    '                   "Versions inconsistency", MsgBoxStyle.Exclamation)

    '            ' Create a new instance of the child form.
    '            Dim ChildForm As New frmOlmisUpdates
    '            ' Make it a child of this MDI form before showing it.
    '            'ChildForm.MdiParent = Me

    '            m_ChildFormNumber += 1
    '            ChildForm.Text = "OLMIS updates" '"Window " & m_ChildFormNumber


    '            ChildForm.ShowDialog()

    '            Return False

    '            End If




    '        If intCurrentVersion <> intOnlineVersion Then

    '            ' Create a new instance of the child form.
    '            Dim ChildForm As New frmOlmisUpdates
    '            ' Make it a child of this MDI form before showing it.
    '            'ChildForm.MdiParent = Me

    '            m_ChildFormNumber += 1
    '            ChildForm.Text = "OLMIS updates" '"Window " & m_ChildFormNumber


    '            ChildForm.ShowDialog()



    '            'Dim Myfunctions As New functions
    '            'If Myfunctions.SystemUpdate(Application.StartupPath & "\OlmisUpdates\" & intOnlineVersion & ".zip", _
    '            '                         Application.StartupPath & "\OlmisUpdates\" & intOnlineVersion & "\") = True Then
    '            '    MsgBox("Updates successful..BINGO!!.", MsgBoxStyle.Information)
    '            ''    Return True
    '            'End If

    '            Return False
    '        Else
    '            Return True
    '        End If

    '    Catch ex As Exception
    '        MsgBox(ex.Message)
    '        Return False
    '    End Try
    'End Function

    Private Function ReadVersionFile(ByVal strVersionfile As String) As Int64
        Try
            ' Create an instance of StreamReader to read from a file.
            Using sr As StreamReader = New StreamReader(strVersionfile)
                Dim line As String
                ' Read and display the lines from the file until the end 
                ' of the file is reached.
                'Do
                line = sr.ReadLine()
                Return CInt(line)
                'Console.WriteLine(line)
                'Loop Until line Is Nothing
            End Using
        Catch E As Exception

            ' Let the user know what went wrong.
            'MsgBox(E.Message, MsgBoxStyle.Exclamation)
            Return 0
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

    Private Sub Cancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel.Click
        Me.Close()
    End Sub


    Private Sub LoginForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.LogoPictureBox.Image = System.Drawing.Image.FromFile(Application.StartupPath & _
                "\images\OVCSystemUSAIDlogo.jpg")
        '"OVCSystemUSAIDlogo.jpeg"

        'Read EXE compilation date and display it
        Dim strDate As String
        Dim fName As String
        fName = Application.StartupPath & "\OVCSystem.exe"
        strDate = File.GetLastWriteTime(fName)
        Label1.Text = "Last Revised: " & Format(CDate(strDate), "dd-MMM-yyyy")

        'show all connectionstrings in a dropdown
        GetAllConnectionStrings()

        SelectedConnectionString = "DefaultConnection"

        'check if the system date has been messed with, by checking currentdate - most recent services date
        Dim MyDBAction As New functions
        Dim SqlAction As String = "SELECT top 1 dateofvisit from StatusAndServiceVisit order by dateofvisit desc"
        Dim Mymaxdateofvisit As Date = MyDBAction.DBAction(SqlAction, DBActionType.Scalar)

        If Date.Today < Mymaxdateofvisit Then
            MsgBox("Please check your system date", MsgBoxStyle.Exclamation)
            'End
        End If
    End Sub

    Private Sub GetAllConnectionStrings()
        'Clear the listbox
        Me.cboConnectionStrings.Items.Clear()
        'Declare a collection that will contains all the connection strings 
        'retrieved from the app.config file
        Dim collCS As ConnectionStringSettingsCollection
        Try
            collCS = ConfigurationManager.ConnectionStrings
        Catch ex As Exception
            'the section is surely not found!
            collCS = Nothing
        End Try
        'If the collection is not empty
        If collCS IsNot Nothing Then
            'Loop through all settings
            For Each cs As ConnectionStringSettings In collCS
                'Add the name of the connection string to the listbox
                Me.cboConnectionStrings.Items.Add(cs.Name)
            Next
        End If
        collCS = Nothing
    End Sub

    Private Sub cboConnectionStrings_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboConnectionStrings.SelectedIndexChanged
        'store the selected connectionstring
        SelectedConnectionString = cboConnectionStrings.Text
    End Sub


End Class
