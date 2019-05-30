<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MDIMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MDIMain))
        Me.MenuStrip = New System.Windows.Forms.MenuStrip()
        Me.ClientMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.Biodata = New System.Windows.Forms.ToolStripMenuItem()
        Me.VSLAmembership = New System.Windows.Forms.ToolStripMenuItem()
        Me.CaregiverStarterKits = New System.Windows.Forms.ToolStripMenuItem()
        Me.CaregiverValueChains = New System.Windows.Forms.ToolStripMenuItem()
        Me.TrainingAttendance = New System.Windows.Forms.ToolStripMenuItem()
        Me.Closemenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.MaintenanceMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.VSLAListing = New System.Windows.Forms.ToolStripMenuItem()
        Me.StarterKits = New System.Windows.Forms.ToolStripMenuItem()
        Me.ValueChains = New System.Windows.Forms.ToolStripMenuItem()
        Me.TrainingsListing = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportMaintenanceData = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImportMaintenanceData = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImportCPIMSData = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExcelReports = New System.Windows.Forms.ToolStripMenuItem()
        Me.Tools = New System.Windows.Forms.ToolStripMenuItem()
        Me.ManageClusters = New System.Windows.Forms.ToolStripMenuItem()
        Me.UserManagement = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.ToolStrip = New System.Windows.Forms.ToolStrip()
        Me.ToolStripbtnChangepassword = New System.Windows.Forms.ToolStripButton()
        Me.ItemPanel = New System.Windows.Forms.Panel()
        Me.RegionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip.SuspendLayout()
        Me.StatusStrip.SuspendLayout()
        Me.ToolStrip.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip
        '
        Me.MenuStrip.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.MenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ClientMenu, Me.MaintenanceMenu, Me.ViewMenu, Me.Tools})
        Me.MenuStrip.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip.Name = "MenuStrip"
        Me.MenuStrip.Padding = New System.Windows.Forms.Padding(9, 3, 0, 3)
        Me.MenuStrip.Size = New System.Drawing.Size(1419, 35)
        Me.MenuStrip.TabIndex = 5
        Me.MenuStrip.Text = "MenuStrip"
        '
        'ClientMenu
        '
        Me.ClientMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.Biodata, Me.VSLAmembership, Me.CaregiverStarterKits, Me.CaregiverValueChains, Me.TrainingAttendance, Me.Closemenu})
        Me.ClientMenu.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder
        Me.ClientMenu.Name = "ClientMenu"
        Me.ClientMenu.Size = New System.Drawing.Size(105, 29)
        Me.ClientMenu.Text = "&Client Info"
        '
        'Biodata
        '
        Me.Biodata.ImageTransparentColor = System.Drawing.Color.Black
        Me.Biodata.Name = "Biodata"
        Me.Biodata.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.N), System.Windows.Forms.Keys)
        Me.Biodata.Size = New System.Drawing.Size(296, 30)
        Me.Biodata.Text = "&OVC Registration"
        '
        'VSLAmembership
        '
        Me.VSLAmembership.Name = "VSLAmembership"
        Me.VSLAmembership.Size = New System.Drawing.Size(296, 30)
        Me.VSLAmembership.Text = "VSLA membership"
        '
        'CaregiverStarterKits
        '
        Me.CaregiverStarterKits.Name = "CaregiverStarterKits"
        Me.CaregiverStarterKits.Size = New System.Drawing.Size(296, 30)
        Me.CaregiverStarterKits.Text = "Caregiver StarterKits"
        '
        'CaregiverValueChains
        '
        Me.CaregiverValueChains.Name = "CaregiverValueChains"
        Me.CaregiverValueChains.Size = New System.Drawing.Size(296, 30)
        Me.CaregiverValueChains.Text = "Caregiver ValueChains"
        '
        'TrainingAttendance
        '
        Me.TrainingAttendance.Name = "TrainingAttendance"
        Me.TrainingAttendance.Size = New System.Drawing.Size(296, 30)
        Me.TrainingAttendance.Text = "Training Attendance"
        '
        'Closemenu
        '
        Me.Closemenu.Name = "Closemenu"
        Me.Closemenu.Size = New System.Drawing.Size(296, 30)
        Me.Closemenu.Text = "E&xit"
        '
        'MaintenanceMenu
        '
        Me.MaintenanceMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.VSLAListing, Me.StarterKits, Me.ValueChains, Me.TrainingsListing, Me.ExportMaintenanceData, Me.ImportMaintenanceData, Me.ImportCPIMSData})
        Me.MaintenanceMenu.Name = "MaintenanceMenu"
        Me.MaintenanceMenu.Size = New System.Drawing.Size(124, 29)
        Me.MaintenanceMenu.Text = "&Maintenance"
        '
        'VSLAListing
        '
        Me.VSLAListing.Name = "VSLAListing"
        Me.VSLAListing.Size = New System.Drawing.Size(299, 30)
        Me.VSLAListing.Text = "VSLA Listing"
        '
        'StarterKits
        '
        Me.StarterKits.Name = "StarterKits"
        Me.StarterKits.Size = New System.Drawing.Size(299, 30)
        Me.StarterKits.Text = "Stater Kits"
        '
        'ValueChains
        '
        Me.ValueChains.Name = "ValueChains"
        Me.ValueChains.Size = New System.Drawing.Size(299, 30)
        Me.ValueChains.Text = "Value Chains"
        '
        'TrainingsListing
        '
        Me.TrainingsListing.Name = "TrainingsListing"
        Me.TrainingsListing.Size = New System.Drawing.Size(299, 30)
        Me.TrainingsListing.Text = "Trainings Listing"
        '
        'ExportMaintenanceData
        '
        Me.ExportMaintenanceData.Name = "ExportMaintenanceData"
        Me.ExportMaintenanceData.Size = New System.Drawing.Size(299, 30)
        Me.ExportMaintenanceData.Text = "Export Maintenance Data"
        Me.ExportMaintenanceData.Visible = False
        '
        'ImportMaintenanceData
        '
        Me.ImportMaintenanceData.Name = "ImportMaintenanceData"
        Me.ImportMaintenanceData.Size = New System.Drawing.Size(299, 30)
        Me.ImportMaintenanceData.Text = "Import Maintenance Data"
        Me.ImportMaintenanceData.Visible = False
        '
        'ImportCPIMSData
        '
        Me.ImportCPIMSData.Name = "ImportCPIMSData"
        Me.ImportCPIMSData.Size = New System.Drawing.Size(299, 30)
        Me.ImportCPIMSData.Text = "ImportCPIMSData"
        '
        'ViewMenu
        '
        Me.ViewMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExcelReports})
        Me.ViewMenu.Name = "ViewMenu"
        Me.ViewMenu.Size = New System.Drawing.Size(61, 29)
        Me.ViewMenu.Text = "&View"
        '
        'ExcelReports
        '
        Me.ExcelReports.Name = "ExcelReports"
        Me.ExcelReports.Size = New System.Drawing.Size(201, 30)
        Me.ExcelReports.Text = "Excel Reports"
        '
        'Tools
        '
        Me.Tools.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ManageClusters, Me.UserManagement})
        Me.Tools.Name = "Tools"
        Me.Tools.Size = New System.Drawing.Size(65, 29)
        Me.Tools.Text = "Tools"
        '
        'ManageClusters
        '
        Me.ManageClusters.Name = "ManageClusters"
        Me.ManageClusters.Size = New System.Drawing.Size(242, 30)
        Me.ManageClusters.Text = "Manage Clusters"
        '
        'UserManagement
        '
        Me.UserManagement.Name = "UserManagement"
        Me.UserManagement.Size = New System.Drawing.Size(242, 30)
        Me.UserManagement.Text = "User Management"
        '
        'StatusStrip
        '
        Me.StatusStrip.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.StatusStrip.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel2, Me.ToolStripStatusLabel1})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 729)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Padding = New System.Windows.Forms.Padding(3, 0, 21, 0)
        Me.StatusStrip.Size = New System.Drawing.Size(1419, 22)
        Me.StatusStrip.TabIndex = 7
        '
        'ToolStripStatusLabel2
        '
        Me.ToolStripStatusLabel2.ForeColor = System.Drawing.Color.Red
        Me.ToolStripStatusLabel2.LinkColor = System.Drawing.Color.Lime
        Me.ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        Me.ToolStripStatusLabel2.Size = New System.Drawing.Size(435, 32)
        Me.ToolStripStatusLabel2.Text = "Internet Connection NOT Successful!"
        Me.ToolStripStatusLabel2.Visible = False
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.ForeColor = System.Drawing.Color.Lime
        Me.ToolStripStatusLabel1.LinkColor = System.Drawing.Color.Lime
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(377, 32)
        Me.ToolStripStatusLabel1.Text = "Internet Connection Successful."
        Me.ToolStripStatusLabel1.Visible = False
        '
        'ToolStrip
        '
        Me.ToolStrip.ImageScalingSize = New System.Drawing.Size(40, 40)
        Me.ToolStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripbtnChangepassword})
        Me.ToolStrip.Location = New System.Drawing.Point(0, 35)
        Me.ToolStrip.Name = "ToolStrip"
        Me.ToolStrip.Padding = New System.Windows.Forms.Padding(0, 0, 3, 0)
        Me.ToolStrip.Size = New System.Drawing.Size(1419, 32)
        Me.ToolStrip.TabIndex = 6
        Me.ToolStrip.Text = "ToolStrip"
        '
        'ToolStripbtnChangepassword
        '
        Me.ToolStripbtnChangepassword.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.ToolStripbtnChangepassword.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.ToolStripbtnChangepassword.ForeColor = System.Drawing.Color.Blue
        Me.ToolStripbtnChangepassword.Image = CType(resources.GetObject("ToolStripbtnChangepassword.Image"), System.Drawing.Image)
        Me.ToolStripbtnChangepassword.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripbtnChangepassword.Name = "ToolStripbtnChangepassword"
        Me.ToolStripbtnChangepassword.Size = New System.Drawing.Size(297, 29)
        Me.ToolStripbtnChangepassword.Text = "Change Password [Ctrl+Alt+End]"
        '
        'ItemPanel
        '
        Me.ItemPanel.AutoScroll = True
        Me.ItemPanel.Location = New System.Drawing.Point(1, 30)
        Me.ItemPanel.Name = "ItemPanel"
        Me.ItemPanel.Size = New System.Drawing.Size(198, 0)
        Me.ItemPanel.TabIndex = 1
        '
        'RegionsToolStripMenuItem
        '
        Me.RegionsToolStripMenuItem.Name = "RegionsToolStripMenuItem"
        Me.RegionsToolStripMenuItem.Size = New System.Drawing.Size(116, 22)
        Me.RegionsToolStripMenuItem.Text = "Regions"
        '
        'MDIMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Lavender
        Me.BackgroundImage = Global.OVCSystem.My.Resources.Resources.page01
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.ClientSize = New System.Drawing.Size(1419, 751)
        Me.Controls.Add(Me.ToolStrip)
        Me.Controls.Add(Me.MenuStrip)
        Me.Controls.Add(Me.StatusStrip)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.IsMdiContainer = True
        Me.MainMenuStrip = Me.MenuStrip
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "MDIMain"
        Me.Text = "MDIMain"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.MenuStrip.ResumeLayout(False)
        Me.MenuStrip.PerformLayout()
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()
        Me.ToolStrip.ResumeLayout(False)
        Me.ToolStrip.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents StatusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents Closemenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Biodata As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ClientMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MenuStrip As System.Windows.Forms.MenuStrip
    Friend WithEvents MaintenanceMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ViewMenu As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Tools As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStrip As System.Windows.Forms.ToolStrip
    Friend WithEvents ManageClusters As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UserManagement As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ItemPanel As System.Windows.Forms.Panel
    Friend WithEvents RegionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExcelReports As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExportMaintenanceData As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ImportMaintenanceData As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel2 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ToolStripbtnChangepassword As ToolStripButton
    Friend WithEvents VSLAListing As ToolStripMenuItem
    Friend WithEvents StarterKits As ToolStripMenuItem
    Friend WithEvents ValueChains As ToolStripMenuItem
    Friend WithEvents VSLAmembership As ToolStripMenuItem
    Friend WithEvents TrainingsListing As ToolStripMenuItem
    Friend WithEvents CaregiverStarterKits As ToolStripMenuItem
    Friend WithEvents CaregiverValueChains As ToolStripMenuItem
    Friend WithEvents TrainingAttendance As ToolStripMenuItem
    Friend WithEvents ImportCPIMSData As ToolStripMenuItem
End Class
