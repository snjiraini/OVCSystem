<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmExportMaintenanceData
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
        Me.btnExportMaintenanceData = New System.Windows.Forms.Button
        Me.Progresspanel = New System.Windows.Forms.Panel
        Me.lblProgress = New System.Windows.Forms.Label
        Me.btnCancel = New System.Windows.Forms.Button
        Me.WebBrowser1 = New System.Windows.Forms.WebBrowser
        Me.worker = New System.ComponentModel.BackgroundWorker
        Me.Progresspanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnExportMaintenanceData
        '
        Me.btnExportMaintenanceData.Location = New System.Drawing.Point(12, 12)
        Me.btnExportMaintenanceData.Name = "btnExportMaintenanceData"
        Me.btnExportMaintenanceData.Size = New System.Drawing.Size(178, 23)
        Me.btnExportMaintenanceData.TabIndex = 0
        Me.btnExportMaintenanceData.Text = "Export Maintenance Data"
        Me.btnExportMaintenanceData.UseVisualStyleBackColor = True
        '
        'Progresspanel
        '
        Me.Progresspanel.Controls.Add(Me.lblProgress)
        Me.Progresspanel.Controls.Add(Me.btnCancel)
        Me.Progresspanel.Controls.Add(Me.WebBrowser1)
        Me.Progresspanel.Location = New System.Drawing.Point(12, 41)
        Me.Progresspanel.Name = "Progresspanel"
        Me.Progresspanel.Size = New System.Drawing.Size(407, 157)
        Me.Progresspanel.TabIndex = 1
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
        Me.btnCancel.Location = New System.Drawing.Point(301, 4)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(89, 23)
        Me.btnCancel.TabIndex = 3
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'WebBrowser1
        '
        Me.WebBrowser1.Location = New System.Drawing.Point(3, 3)
        Me.WebBrowser1.MinimumSize = New System.Drawing.Size(20, 20)
        Me.WebBrowser1.Name = "WebBrowser1"
        Me.WebBrowser1.ScrollBarsEnabled = False
        Me.WebBrowser1.Size = New System.Drawing.Size(171, 148)
        Me.WebBrowser1.TabIndex = 2
        Me.WebBrowser1.Url = New System.Uri("", System.UriKind.Relative)
        '
        'worker
        '
        Me.worker.WorkerReportsProgress = True
        Me.worker.WorkerSupportsCancellation = True
        '
        'frmExportMaintenanceData
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(426, 207)
        Me.Controls.Add(Me.Progresspanel)
        Me.Controls.Add(Me.btnExportMaintenanceData)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "frmExportMaintenanceData"
        Me.Text = "Export Maintenance Data"
        Me.Progresspanel.ResumeLayout(False)
        Me.Progresspanel.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnExportMaintenanceData As System.Windows.Forms.Button
    Friend WithEvents Progresspanel As System.Windows.Forms.Panel
    Friend WithEvents WebBrowser1 As System.Windows.Forms.WebBrowser
    Friend WithEvents worker As System.ComponentModel.BackgroundWorker
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents lblProgress As System.Windows.Forms.Label
End Class
