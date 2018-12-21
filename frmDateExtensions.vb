Imports OVCSystem.functions
Imports System.Data.Odbc
Imports System.Configuration.ConfigurationManager
Imports System.Xml
Imports System.Text
Public Class frmDateExtensions
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            Dim myAppCrypto As New AppSecurity
            Dim Issuccess As Boolean = False

            'default dates
            Dim myregistrationdefault As String = myAppCrypto.encryptString(txtRegistrationDefault.Text.ToString).ToString
            Dim myexitsdefault As String = myAppCrypto.encryptString(txtExitsDefault.Text.ToString).ToString
            Dim mydatadefault As String = myAppCrypto.encryptString(txtDataDefault.Text.ToString).ToString
            Dim mybacklogdefault As String = myAppCrypto.encryptString(txtbacklogdefault.Text.ToString).ToString

            'extension dates
            Dim myregistrationextension As String = myAppCrypto.encryptString(txtRegistrationExtension.Text.ToString).ToString
            Dim myexitsextension As String = myAppCrypto.encryptString(txtExitsExtension.Text.ToString).ToString
            Dim mydataextension As String = myAppCrypto.encryptString(txtDataExtension.Text.ToString).ToString
            Dim mydateofExtension As String = myAppCrypto.encryptString(Date.Now.ToString).ToString
            Dim myextensionexpiry As String = myAppCrypto.encryptString(txtdaystoexpiry.Text.ToString).ToString
            Dim mybacklogextension As String = myAppCrypto.encryptString(txtbacklogextension.Text.ToString).ToString


            Issuccess = WriteXML("mydateextensions.xml", myregistrationdefault, myexitsdefault, mydatadefault, mybacklogdefault,
                                 myregistrationextension, myexitsextension, mydataextension, mybacklogextension, mydateofExtension, myextensionexpiry)

            If Issuccess = True Then
                MsgBox("Details Successfully Saved.", MsgBoxStyle.Information)
            Else
                MsgBox("Details NOT Saved.", MsgBoxStyle.Exclamation)
                Exit Sub
            End If


            'read from the settings xml file to get the dataentry validation durations and extensions
            Dim myarraylist As New ArrayList
            myarraylist = ReadXML("mydateextensions.xml")


            strregistrationdefault = myAppCrypto.decryptString(myarraylist.Item(0).ToString)
            strexitsdefault = myAppCrypto.decryptString(myarraylist.Item(1).ToString)
            strdatadefault = myAppCrypto.decryptString(myarraylist.Item(2).ToString)
            strdatabacklogdefault = myAppCrypto.decryptString(myarraylist.Item(3).ToString)
            strregistrationextension = myAppCrypto.decryptString(myarraylist.Item(4).ToString)
            strexitsextension = myAppCrypto.decryptString(myarraylist.Item(5).ToString)
            strdataextension = myAppCrypto.decryptString(myarraylist.Item(6).ToString)
            strdatabacklogextension = myAppCrypto.decryptString(myarraylist.Item(7).ToString)
            strdateofExtension = myAppCrypto.decryptString(myarraylist.Item(8).ToString)
            strextensionexpiry = myAppCrypto.decryptString(myarraylist.Item(9).ToString)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Function WriteXML(ByVal xmlfile As String, ByVal myregistrationdefault As String,
                              ByVal myexitsdefault As String, ByVal mydatadefault As String, ByVal mydatabacklogdefault As String,
                              ByVal myregistrationextension As String, ByVal myexitsextension As String,
                              ByVal mydataextension As String, ByVal mydatabacklogextension As String, ByVal mydateofExtension As String,
                              ByVal myextensionexpiry As String) As Boolean
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
            objXMLTW.WriteStartElement("registrationdefault")
            objXMLTW.WriteString(myregistrationdefault)
            objXMLTW.WriteEndElement()

            objXMLTW.WriteStartElement("exitsdefault")
            objXMLTW.WriteString(myexitsdefault)
            objXMLTW.WriteEndElement()

            objXMLTW.WriteStartElement("datadefault")
            objXMLTW.WriteString(mydatadefault)
            objXMLTW.WriteEndElement()

            objXMLTW.WriteStartElement("databacklogdefault")
            objXMLTW.WriteString(mydatabacklogdefault)
            objXMLTW.WriteEndElement()

            objXMLTW.WriteStartElement("registrationextension")
            objXMLTW.WriteString(myregistrationextension)
            objXMLTW.WriteEndElement()

            objXMLTW.WriteStartElement("exitsextension")
            objXMLTW.WriteString(myexitsextension)
            objXMLTW.WriteEndElement()

            objXMLTW.WriteStartElement("dataextension")
            objXMLTW.WriteString(mydataextension)
            objXMLTW.WriteEndElement()

            objXMLTW.WriteStartElement("databacklogextension")
            objXMLTW.WriteString(mydatabacklogextension)
            objXMLTW.WriteEndElement()

            objXMLTW.WriteStartElement("dateofExtension")
            objXMLTW.WriteString(mydateofExtension)
            objXMLTW.WriteEndElement()

            objXMLTW.WriteStartElement("extensionexpiry")
            objXMLTW.WriteString(myextensionexpiry)
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

                ElseIf objXMLTR.NodeType = XmlNodeType.Text Or
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

    Private Sub frmDateExtensions_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Dim myAppCrypto As New AppSecurity
            'read from the settings xml file to get the dataentry validation durations and extensions
            Dim myarraylist As New ArrayList
            myarraylist = ReadXML("mydateextensions.xml")


            txtRegistrationDefault.Text = myAppCrypto.decryptString(myarraylist.Item(0).ToString)
            txtExitsDefault.Text = myAppCrypto.decryptString(myarraylist.Item(1).ToString)
            txtDataDefault.Text = myAppCrypto.decryptString(myarraylist.Item(2).ToString)
            txtbacklogdefault.Text = myAppCrypto.decryptString(myarraylist.Item(3).ToString)
            txtRegistrationExtension.Text = myAppCrypto.decryptString(myarraylist.Item(4).ToString)
            txtExitsExtension.Text = myAppCrypto.decryptString(myarraylist.Item(5).ToString)
            txtDataExtension.Text = myAppCrypto.decryptString(myarraylist.Item(6).ToString)
            txtbacklogextension.Text = myAppCrypto.decryptString(myarraylist.Item(7).ToString)
            Label17.Text = myAppCrypto.decryptString(myarraylist.Item(8).ToString)
            txtdaystoexpiry.Text = myAppCrypto.decryptString(myarraylist.Item(9).ToString)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
End Class