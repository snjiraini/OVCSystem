Imports System.Math
Module ModMain
    'Track internet connection
    Public HasInternetConnection As Boolean = False
    Public IsDownloadSuccessful As Boolean = False
    Public IsSystemUpdated As Boolean = False
    Public intCurrentVersion As Int64 = 0
    Public intOnlineVersion As Int64 = 0


    'connection string selected
    Public SelectedConnectionString As String = ""

    'background worker progress string
    Public strReportProgress As String = ""

    'user and session management
    Dim Myrandomkey As New Random
    Public strusername As String = ""
    Public strsession As String = Format(Date.Today, "yyyyMMddhhss") & Myrandomkey.Next(9999)
    Public strmachinename As String = UCase(My.Computer.Name)

    'Keep track of ovc details anywhere from app
    Public myOVCID As String = ""

    'Keep track of form1A longitudinal biodata items
    Public Longitudinal_bcert As Boolean = False
    Public Longitudinal_hivstatus As Boolean = False

    'These variables store, are we searching or adding father,mother and guardian
    'Public parent_id As String = 0
    Public father_id As String = 0
    Public mother_id As String = 0
    Public guardian_id As String = 0
    Public householdhead_id As String = 0
    Public hcbcid As String = 0

    'This had to be here so tht I can call this instance of this form from any page e.g linking parent from father form
    Public clientchildform As frmClientInfo
    Public mdiform As MDIMain
    Public m_RegistrationFormNumber As Integer
    Public m_ChildFormNumber As Integer
    Public m_reportFormNumber, m_excelreportFormNumber As Integer

    'When you log in to a particular cbo, we need to keep the cbo and district id here
    Public strcbos, strdistricts, strdistrictcbo, strimplementingpartner,
        strimplementingpartnerid, strftphost, strftpusername, strftppassword As String

    'Hold details about data entry deadlines and Extensions
    Public strregistrationdefault As String
    Public strexitsdefault As String
    Public strdatadefault As String
    Public strdatabacklogdefault As String
    Public strregistrationextension As String
    Public strexitsextension As String
    Public strdataextension As String
    Public strdatabacklogextension As String
    Public strdateofExtension As String
    Public strextensionexpiry As String

    'This variable keeps track on whether the loggedin guy has rights to even see the delete button on biodata
    Public candelete As Boolean

    'Are we adding new client, editing existing or adding longitudinal data
    Public entry_mode As String

    'Track which folder is being imported e.g Exports\27-DEC-2012
    Public import_folder As String = ""

    'Asynchrounous data processing flag
    Public isExecuting As Boolean = False

End Module
