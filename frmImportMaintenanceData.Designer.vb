<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmImportMaintenanceData
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
        Me.Progresspanel = New System.Windows.Forms.Panel
        Me.lblProgress = New System.Windows.Forms.Label
        Me.btnCancel = New System.Windows.Forms.Button
        Me.WebBrowser1 = New System.Windows.Forms.WebBrowser
        Me.btnImportMaintenanceData = New System.Windows.Forms.Button
        Me.worker = New System.ComponentModel.BackgroundWorker
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
        Me.cmdbrowse = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtimportdirectory = New System.Windows.Forms.TextBox
        ''Me.OVCSystemDBSQLDataset1 = New OVCSystem.OVCSystemDBSQLDataset
        Me.Progresspanel.SuspendLayout()
        'CType('Me.OVCSystemDBSQLDataset1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Progresspanel
        '
        Me.Progresspanel.Controls.Add(Me.lblProgress)
        Me.Progresspanel.Controls.Add(Me.btnCancel)
        Me.Progresspanel.Controls.Add(Me.WebBrowser1)
        Me.Progresspanel.Location = New System.Drawing.Point(12, 107)
        Me.Progresspanel.Name = "Progresspanel"
        Me.Progresspanel.Size = New System.Drawing.Size(529, 157)
        Me.Progresspanel.TabIndex = 3
        Me.Progresspanel.Visible = False
        '
        'lblProgress
        '
        Me.lblProgress.AutoSize = True
        Me.lblProgress.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblProgress.ForeColor = System.Drawing.Color.Blue
        Me.lblProgress.Location = New System.Drawing.Point(198, 52)
        Me.lblProgress.Name = "lblProgress"
        Me.lblProgress.Size = New System.Drawing.Size(15, 13)
        Me.lblProgress.TabIndex = 4
        Me.lblProgress.Text = "--"
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(436, 0)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(93, 23)
        Me.btnCancel.TabIndex = 3
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'WebBrowser1
        '
        Me.WebBrowser1.Location = New System.Drawing.Point(3, 4)
        Me.WebBrowser1.MinimumSize = New System.Drawing.Size(20, 20)
        Me.WebBrowser1.Name = "WebBrowser1"
        Me.WebBrowser1.ScrollBarsEnabled = False
        Me.WebBrowser1.Size = New System.Drawing.Size(171, 148)
        Me.WebBrowser1.TabIndex = 2
        Me.WebBrowser1.Url = New System.Uri("", System.UriKind.Relative)
        '
        'btnImportMaintenanceData
        '
        Me.btnImportMaintenanceData.Location = New System.Drawing.Point(327, 63)
        Me.btnImportMaintenanceData.Name = "btnImportMaintenanceData"
        Me.btnImportMaintenanceData.Size = New System.Drawing.Size(214, 23)
        Me.btnImportMaintenanceData.TabIndex = 2
        Me.btnImportMaintenanceData.Text = "Import Maintenance Data"
        Me.btnImportMaintenanceData.UseVisualStyleBackColor = True
        '
        'worker
        '
        Me.worker.WorkerReportsProgress = True
        Me.worker.WorkerSupportsCancellation = True
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'cmdbrowse
        '
        Me.cmdbrowse.Location = New System.Drawing.Point(467, 34)
        Me.cmdbrowse.Name = "cmdbrowse"
        Me.cmdbrowse.Size = New System.Drawing.Size(75, 23)
        Me.cmdbrowse.TabIndex = 8
        Me.cmdbrowse.Text = "Browse"
        Me.cmdbrowse.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(62, 13)
        Me.Label1.TabIndex = 7
        Me.Label1.Text = "Import from:"
        '
        'txtimportdirectory
        '
        Me.txtimportdirectory.Location = New System.Drawing.Point(13, 36)
        Me.txtimportdirectory.Name = "txtimportdirectory"
        Me.txtimportdirectory.Size = New System.Drawing.Size(452, 20)
        Me.txtimportdirectory.TabIndex = 6
        '
        'OVCSystemDBSQLDataset1
        '
        'Me.OVCSystemDBSQLDataset1.DataSetName = "OVCSystemDBSQLDataset"
        'Me.OVCSystemDBSQLDataset1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'frmImportMaintenanceData
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(549, 271)
        Me.Controls.Add(Me.cmdbrowse)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtimportdirectory)
        Me.Controls.Add(Me.Progresspanel)
        Me.Controls.Add(Me.btnImportMaintenanceData)
        Me.Name = "frmImportMaintenanceData"
        Me.Text = "frmImportMaintenanceData"
        Me.Progresspanel.ResumeLayout(False)
        Me.Progresspanel.PerformLayout()
        'CType('Me.OVCSystemDBSQLDataset1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Progresspanel As System.Windows.Forms.Panel
    Friend WithEvents lblProgress As System.Windows.Forms.Label
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents WebBrowser1 As System.Windows.Forms.WebBrowser
    Friend WithEvents btnImportMaintenanceData As System.Windows.Forms.Button
    Friend WithEvents worker As System.ComponentModel.BackgroundWorker
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents cmdbrowse As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtimportdirectory As System.Windows.Forms.TextBox
    'Friend WithEvents OVCSystemDBSQLDataset1 As OVCSystem.OVCSystemDBSQLDataset
End Class
