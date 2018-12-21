<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDataSync
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnImport = New System.Windows.Forms.Button()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.ProgressPanel = New System.Windows.Forms.Panel()
        Me.WebBrowser1 = New System.Windows.Forms.WebBrowser()
        Me.lnkRedoExport = New System.Windows.Forms.LinkLabel()
        Me.lnkNormalExport = New System.Windows.Forms.LinkLabel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.btnRedoExportData = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.dtpTo = New System.Windows.Forms.DateTimePicker()
        Me.dtpFrom = New System.Windows.Forms.DateTimePicker()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.btnExportData = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.ProgressPanel2 = New System.Windows.Forms.Panel()
        Me.WebBrowser2 = New System.Windows.Forms.WebBrowser()
        Me.cmdbrowse = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtimportdirectory = New System.Windows.Forms.TextBox()
        Me.TabPage6 = New System.Windows.Forms.TabPage()
        Me.cmdbrowsescript = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtScriptFile = New System.Windows.Forms.TextBox()
        Me.btnRun = New System.Windows.Forms.Button()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.OpenFileDialog2 = New System.Windows.Forms.OpenFileDialog()
        Me.Exportworker = New System.ComponentModel.BackgroundWorker()
        Me.ImportWorker = New System.ComponentModel.BackgroundWorker()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.WebBrowser3 = New System.Windows.Forms.WebBrowser()
        Me.LinkLabel2 = New System.Windows.Forms.LinkLabel()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.ProgressPanel.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.ProgressPanel2.SuspendLayout()
        Me.TabPage6.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnImport
        '
        Me.btnImport.Location = New System.Drawing.Point(527, 54)
        Me.btnImport.Name = "btnImport"
        Me.btnImport.Size = New System.Drawing.Size(75, 23)
        Me.btnImport.TabIndex = 2
        Me.btnImport.Text = "Import"
        Me.btnImport.UseVisualStyleBackColor = True
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage6)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Location = New System.Drawing.Point(12, 12)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(632, 226)
        Me.TabControl1.TabIndex = 3
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.ProgressPanel)
        Me.TabPage1.Controls.Add(Me.lnkRedoExport)
        Me.TabPage1.Controls.Add(Me.lnkNormalExport)
        Me.TabPage1.Controls.Add(Me.Panel2)
        Me.TabPage1.Controls.Add(Me.Panel1)
        Me.TabPage1.Controls.Add(Me.Label4)
        Me.TabPage1.Controls.Add(Me.Label2)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(624, 200)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Export Data"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'ProgressPanel
        '
        Me.ProgressPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.ProgressPanel.Controls.Add(Me.WebBrowser1)
        Me.ProgressPanel.Location = New System.Drawing.Point(320, 141)
        Me.ProgressPanel.Name = "ProgressPanel"
        Me.ProgressPanel.Size = New System.Drawing.Size(298, 53)
        Me.ProgressPanel.TabIndex = 7
        Me.ProgressPanel.Visible = False
        '
        'WebBrowser1
        '
        Me.WebBrowser1.Location = New System.Drawing.Point(3, 3)
        Me.WebBrowser1.MinimumSize = New System.Drawing.Size(20, 20)
        Me.WebBrowser1.Name = "WebBrowser1"
        Me.WebBrowser1.ScrollBarsEnabled = False
        Me.WebBrowser1.Size = New System.Drawing.Size(284, 39)
        Me.WebBrowser1.TabIndex = 1
        Me.WebBrowser1.Url = New System.Uri("", System.UriKind.Relative)
        '
        'lnkRedoExport
        '
        Me.lnkRedoExport.AutoSize = True
        Me.lnkRedoExport.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lnkRedoExport.Location = New System.Drawing.Point(320, 19)
        Me.lnkRedoExport.Name = "lnkRedoExport"
        Me.lnkRedoExport.Size = New System.Drawing.Size(77, 13)
        Me.lnkRedoExport.TabIndex = 6
        Me.lnkRedoExport.TabStop = True
        Me.lnkRedoExport.Text = "Redo Export"
        '
        'lnkNormalExport
        '
        Me.lnkNormalExport.AutoSize = True
        Me.lnkNormalExport.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lnkNormalExport.Location = New System.Drawing.Point(39, 19)
        Me.lnkNormalExport.Name = "lnkNormalExport"
        Me.lnkNormalExport.Size = New System.Drawing.Size(86, 13)
        Me.lnkNormalExport.TabIndex = 5
        Me.lnkNormalExport.TabStop = True
        Me.lnkNormalExport.Text = "Normal Export"
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.btnRedoExportData)
        Me.Panel2.Controls.Add(Me.Label3)
        Me.Panel2.Controls.Add(Me.Label5)
        Me.Panel2.Controls.Add(Me.dtpTo)
        Me.Panel2.Controls.Add(Me.dtpFrom)
        Me.Panel2.Location = New System.Drawing.Point(320, 38)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(298, 100)
        Me.Panel2.TabIndex = 4
        Me.Panel2.Visible = False
        '
        'btnRedoExportData
        '
        Me.btnRedoExportData.Location = New System.Drawing.Point(212, 17)
        Me.btnRedoExportData.Name = "btnRedoExportData"
        Me.btnRedoExportData.Size = New System.Drawing.Size(75, 23)
        Me.btnRedoExportData.TabIndex = 12
        Me.btnRedoExportData.Text = "Export Data"
        Me.btnRedoExportData.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(3, 46)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(46, 13)
        Me.Label3.TabIndex = 11
        Me.Label3.Text = "Date To"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(3, 2)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(56, 13)
        Me.Label5.TabIndex = 10
        Me.Label5.Text = "Date From"
        '
        'dtpTo
        '
        Me.dtpTo.Location = New System.Drawing.Point(6, 62)
        Me.dtpTo.Name = "dtpTo"
        Me.dtpTo.Size = New System.Drawing.Size(200, 20)
        Me.dtpTo.TabIndex = 9
        '
        'dtpFrom
        '
        Me.dtpFrom.Location = New System.Drawing.Point(6, 20)
        Me.dtpFrom.Name = "dtpFrom"
        Me.dtpFrom.Size = New System.Drawing.Size(200, 20)
        Me.dtpFrom.TabIndex = 8
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.btnExportData)
        Me.Panel1.Location = New System.Drawing.Point(39, 38)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(200, 100)
        Me.Panel1.TabIndex = 3
        Me.Panel1.Visible = False
        '
        'btnExportData
        '
        Me.btnExportData.Location = New System.Drawing.Point(22, 18)
        Me.btnExportData.Name = "btnExportData"
        Me.btnExportData.Size = New System.Drawing.Size(75, 23)
        Me.btnExportData.TabIndex = 0
        Me.btnExportData.Text = "Export Data"
        Me.btnExportData.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.DarkOrange
        Me.Label4.Location = New System.Drawing.Point(39, 170)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(58, 17)
        Me.Label4.TabIndex = 2
        Me.Label4.Text = "----------"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.ForeColor = System.Drawing.Color.DarkOrange
        Me.Label2.Location = New System.Drawing.Point(39, 144)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(58, 17)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "----------"
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.Label6)
        Me.TabPage2.Controls.Add(Me.ProgressPanel2)
        Me.TabPage2.Controls.Add(Me.cmdbrowse)
        Me.TabPage2.Controls.Add(Me.Label1)
        Me.TabPage2.Controls.Add(Me.txtimportdirectory)
        Me.TabPage2.Controls.Add(Me.btnImport)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(624, 200)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Import Data"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.ForeColor = System.Drawing.Color.Olive
        Me.Label6.Location = New System.Drawing.Point(70, 122)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(37, 13)
        Me.Label6.TabIndex = 9
        Me.Label6.Text = "----------"
        '
        'ProgressPanel2
        '
        Me.ProgressPanel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.ProgressPanel2.Controls.Add(Me.WebBrowser2)
        Me.ProgressPanel2.Location = New System.Drawing.Point(304, 119)
        Me.ProgressPanel2.Name = "ProgressPanel2"
        Me.ProgressPanel2.Size = New System.Drawing.Size(298, 53)
        Me.ProgressPanel2.TabIndex = 8
        Me.ProgressPanel2.Visible = False
        '
        'WebBrowser2
        '
        Me.WebBrowser2.Location = New System.Drawing.Point(11, 3)
        Me.WebBrowser2.MinimumSize = New System.Drawing.Size(20, 20)
        Me.WebBrowser2.Name = "WebBrowser2"
        Me.WebBrowser2.ScrollBarsEnabled = False
        Me.WebBrowser2.Size = New System.Drawing.Size(284, 39)
        Me.WebBrowser2.TabIndex = 1
        Me.WebBrowser2.Url = New System.Uri("", System.UriKind.Relative)
        '
        'cmdbrowse
        '
        Me.cmdbrowse.Location = New System.Drawing.Point(527, 19)
        Me.cmdbrowse.Name = "cmdbrowse"
        Me.cmdbrowse.Size = New System.Drawing.Size(75, 23)
        Me.cmdbrowse.TabIndex = 5
        Me.cmdbrowse.Text = "Browse"
        Me.cmdbrowse.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 26)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(62, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Import from:"
        '
        'txtimportdirectory
        '
        Me.txtimportdirectory.Location = New System.Drawing.Point(73, 21)
        Me.txtimportdirectory.Name = "txtimportdirectory"
        Me.txtimportdirectory.Size = New System.Drawing.Size(452, 20)
        Me.txtimportdirectory.TabIndex = 3
        '
        'TabPage6
        '
        Me.TabPage6.Controls.Add(Me.cmdbrowsescript)
        Me.TabPage6.Controls.Add(Me.Label7)
        Me.TabPage6.Controls.Add(Me.txtScriptFile)
        Me.TabPage6.Controls.Add(Me.btnRun)
        Me.TabPage6.Location = New System.Drawing.Point(4, 22)
        Me.TabPage6.Name = "TabPage6"
        Me.TabPage6.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage6.Size = New System.Drawing.Size(624, 200)
        Me.TabPage6.TabIndex = 5
        Me.TabPage6.Text = "Run Any Script"
        Me.TabPage6.UseVisualStyleBackColor = True
        '
        'cmdbrowsescript
        '
        Me.cmdbrowsescript.Location = New System.Drawing.Point(529, 19)
        Me.cmdbrowsescript.Name = "cmdbrowsescript"
        Me.cmdbrowsescript.Size = New System.Drawing.Size(75, 23)
        Me.cmdbrowsescript.TabIndex = 8
        Me.cmdbrowsescript.Text = "Browse"
        Me.cmdbrowsescript.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(3, 24)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(56, 13)
        Me.Label7.TabIndex = 7
        Me.Label7.Text = "Script File:"
        '
        'txtScriptFile
        '
        Me.txtScriptFile.Location = New System.Drawing.Point(70, 19)
        Me.txtScriptFile.Name = "txtScriptFile"
        Me.txtScriptFile.Size = New System.Drawing.Size(452, 20)
        Me.txtScriptFile.TabIndex = 6
        '
        'btnRun
        '
        Me.btnRun.Location = New System.Drawing.Point(529, 48)
        Me.btnRun.Name = "btnRun"
        Me.btnRun.Size = New System.Drawing.Size(75, 23)
        Me.btnRun.TabIndex = 5
        Me.btnRun.Text = "Run Script"
        Me.btnRun.UseVisualStyleBackColor = True
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'OpenFileDialog2
        '
        Me.OpenFileDialog2.FileName = "OpenFileDialog1"
        '
        'Exportworker
        '
        Me.Exportworker.WorkerReportsProgress = True
        '
        'ImportWorker
        '
        Me.ImportWorker.WorkerReportsProgress = True
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.Panel3)
        Me.TabPage3.Controls.Add(Me.LinkLabel2)
        Me.TabPage3.Controls.Add(Me.Panel5)
        Me.TabPage3.Controls.Add(Me.Label10)
        Me.TabPage3.Controls.Add(Me.Label11)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(624, 200)
        Me.TabPage3.TabIndex = 6
        Me.TabPage3.Text = "Migrate OVC Data"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'Panel3
        '
        Me.Panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.Panel3.Controls.Add(Me.WebBrowser3)
        Me.Panel3.Location = New System.Drawing.Point(304, 135)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(298, 53)
        Me.Panel3.TabIndex = 14
        Me.Panel3.Visible = False
        '
        'WebBrowser3
        '
        Me.WebBrowser3.Location = New System.Drawing.Point(3, 3)
        Me.WebBrowser3.MinimumSize = New System.Drawing.Size(20, 20)
        Me.WebBrowser3.Name = "WebBrowser3"
        Me.WebBrowser3.ScrollBarsEnabled = False
        Me.WebBrowser3.Size = New System.Drawing.Size(284, 39)
        Me.WebBrowser3.TabIndex = 1
        Me.WebBrowser3.Url = New System.Uri("", System.UriKind.Relative)
        '
        'LinkLabel2
        '
        Me.LinkLabel2.AutoSize = True
        Me.LinkLabel2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LinkLabel2.Location = New System.Drawing.Point(23, 13)
        Me.LinkLabel2.Name = "LinkLabel2"
        Me.LinkLabel2.Size = New System.Drawing.Size(86, 13)
        Me.LinkLabel2.TabIndex = 12
        Me.LinkLabel2.TabStop = True
        Me.LinkLabel2.Text = "Normal Export"
        '
        'Panel5
        '
        Me.Panel5.Controls.Add(Me.Button2)
        Me.Panel5.Location = New System.Drawing.Point(23, 32)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(579, 100)
        Me.Panel5.TabIndex = 10
        Me.Panel5.Visible = False
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(501, 74)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 0
        Me.Button2.Text = "Export Data"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.ForeColor = System.Drawing.Color.DarkOrange
        Me.Label10.Location = New System.Drawing.Point(23, 164)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(58, 17)
        Me.Label10.TabIndex = 9
        Me.Label10.Text = "----------"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.ForeColor = System.Drawing.Color.DarkOrange
        Me.Label11.Location = New System.Drawing.Point(23, 138)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(58, 17)
        Me.Label11.TabIndex = 8
        Me.Label11.Text = "----------"
        '
        'frmDataSync
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(647, 244)
        Me.Controls.Add(Me.TabControl1)
        Me.Name = "frmDataSync"
        Me.Text = "frmDataSync"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.ProgressPanel.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        Me.ProgressPanel2.ResumeLayout(False)
        Me.TabPage6.ResumeLayout(False)
        Me.TabPage6.PerformLayout()
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel5.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnImport As System.Windows.Forms.Button
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtimportdirectory As System.Windows.Forms.TextBox
    Friend WithEvents TabPage6 As System.Windows.Forms.TabPage
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtScriptFile As System.Windows.Forms.TextBox
    Friend WithEvents btnRun As System.Windows.Forms.Button
    Friend WithEvents cmdbrowse As System.Windows.Forms.Button
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents cmdbrowsescript As System.Windows.Forms.Button
    Friend WithEvents OpenFileDialog2 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnExportData As System.Windows.Forms.Button
    Friend WithEvents lnkRedoExport As System.Windows.Forms.LinkLabel
    Friend WithEvents lnkNormalExport As System.Windows.Forms.LinkLabel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents btnRedoExportData As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents dtpTo As System.Windows.Forms.DateTimePicker
    Friend WithEvents dtpFrom As System.Windows.Forms.DateTimePicker
    Friend WithEvents ProgressPanel As System.Windows.Forms.Panel
    Friend WithEvents WebBrowser1 As System.Windows.Forms.WebBrowser
    Friend WithEvents Exportworker As System.ComponentModel.BackgroundWorker
    Friend WithEvents ProgressPanel2 As System.Windows.Forms.Panel
    Friend WithEvents WebBrowser2 As System.Windows.Forms.WebBrowser
    Friend WithEvents ImportWorker As System.ComponentModel.BackgroundWorker
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents WebBrowser3 As System.Windows.Forms.WebBrowser
    Friend WithEvents LinkLabel2 As System.Windows.Forms.LinkLabel
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
End Class
