<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSelectCBO
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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnfilter = New System.Windows.Forms.Button()
        Me.btnUnselectAll = New System.Windows.Forms.Button()
        Me.btnSelectAll = New System.Windows.Forms.Button()
        Me.TreeView1 = New System.Windows.Forms.TreeView()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnFilterCBOData = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.txtftppassword = New System.Windows.Forms.TextBox()
        Me.txtftpusername = New System.Windows.Forms.TextBox()
        Me.txtFtpHost = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cboImplementingPartner = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnfilter)
        Me.GroupBox1.Controls.Add(Me.btnUnselectAll)
        Me.GroupBox1.Controls.Add(Me.btnSelectAll)
        Me.GroupBox1.Controls.Add(Me.TreeView1)
        Me.GroupBox1.Location = New System.Drawing.Point(13, 13)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(474, 369)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Select CBO(s)"
        '
        'btnfilter
        '
        Me.btnfilter.Location = New System.Drawing.Point(365, 24)
        Me.btnfilter.Name = "btnfilter"
        Me.btnfilter.Size = New System.Drawing.Size(100, 23)
        Me.btnfilter.TabIndex = 7
        Me.btnfilter.Text = "Filter CBO Data"
        Me.btnfilter.UseVisualStyleBackColor = True
        '
        'btnUnselectAll
        '
        Me.btnUnselectAll.Location = New System.Drawing.Point(100, 24)
        Me.btnUnselectAll.Name = "btnUnselectAll"
        Me.btnUnselectAll.Size = New System.Drawing.Size(75, 23)
        Me.btnUnselectAll.TabIndex = 76
        Me.btnUnselectAll.Text = "Unselect All"
        Me.btnUnselectAll.UseVisualStyleBackColor = True
        '
        'btnSelectAll
        '
        Me.btnSelectAll.Location = New System.Drawing.Point(7, 25)
        Me.btnSelectAll.Name = "btnSelectAll"
        Me.btnSelectAll.Size = New System.Drawing.Size(75, 23)
        Me.btnSelectAll.TabIndex = 75
        Me.btnSelectAll.Text = "Select All"
        Me.btnSelectAll.UseVisualStyleBackColor = True
        '
        'TreeView1
        '
        Me.TreeView1.Location = New System.Drawing.Point(6, 54)
        Me.TreeView1.Name = "TreeView1"
        Me.TreeView1.Size = New System.Drawing.Size(459, 309)
        Me.TreeView1.TabIndex = 74
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(10, 541)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(100, 23)
        Me.btnSave.TabIndex = 1
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnFilterCBOData
        '
        Me.btnFilterCBOData.Location = New System.Drawing.Point(387, 388)
        Me.btnFilterCBOData.Name = "btnFilterCBOData"
        Me.btnFilterCBOData.Size = New System.Drawing.Size(100, 23)
        Me.btnFilterCBOData.TabIndex = 2
        Me.btnFilterCBOData.Text = "Filter CBO Data"
        Me.btnFilterCBOData.UseVisualStyleBackColor = True
        Me.btnFilterCBOData.Visible = False
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.txtftppassword)
        Me.GroupBox2.Controls.Add(Me.txtftpusername)
        Me.GroupBox2.Controls.Add(Me.txtFtpHost)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Controls.Add(Me.Label1)
        Me.GroupBox2.Controls.Add(Me.cboImplementingPartner)
        Me.GroupBox2.Controls.Add(Me.Label2)
        Me.GroupBox2.Location = New System.Drawing.Point(10, 388)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(477, 147)
        Me.GroupBox2.TabIndex = 6
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Settings for uploads to PIMA.."
        '
        'txtftppassword
        '
        Me.txtftppassword.Location = New System.Drawing.Point(218, 121)
        Me.txtftppassword.Name = "txtftppassword"
        Me.txtftppassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtftppassword.Size = New System.Drawing.Size(146, 20)
        Me.txtftppassword.TabIndex = 13
        Me.txtftppassword.Text = "xx"
        Me.txtftppassword.UseSystemPasswordChar = True
        '
        'txtftpusername
        '
        Me.txtftpusername.Location = New System.Drawing.Point(14, 118)
        Me.txtftpusername.Name = "txtftpusername"
        Me.txtftpusername.Size = New System.Drawing.Size(146, 20)
        Me.txtftpusername.TabIndex = 12
        Me.txtftpusername.Text = "xx"
        '
        'txtFtpHost
        '
        Me.txtFtpHost.Location = New System.Drawing.Point(14, 82)
        Me.txtFtpHost.Name = "txtFtpHost"
        Me.txtFtpHost.Size = New System.Drawing.Size(350, 20)
        Me.txtFtpHost.TabIndex = 11
        Me.txtFtpHost.Text = "xx"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(215, 105)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(79, 13)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "FTP Password:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(11, 105)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(83, 13)
        Me.Label3.TabIndex = 9
        Me.Label3.Text = "FTP UserName:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(10, 61)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(55, 13)
        Me.Label1.TabIndex = 8
        Me.Label1.Text = "FTP Host:"
        '
        'cboImplementingPartner
        '
        Me.cboImplementingPartner.FormattingEnabled = True
        Me.cboImplementingPartner.Location = New System.Drawing.Point(10, 33)
        Me.cboImplementingPartner.Name = "cboImplementingPartner"
        Me.cboImplementingPartner.Size = New System.Drawing.Size(459, 21)
        Me.cboImplementingPartner.TabIndex = 7
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(7, 16)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(109, 13)
        Me.Label2.TabIndex = 6
        Me.Label2.Text = "Implementing Partner:"
        '
        'frmSelectCBO
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(491, 567)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.btnFilterCBOData)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "frmSelectCBO"
        Me.Text = "frmSelectCBO"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents TreeView1 As System.Windows.Forms.TreeView
    Friend WithEvents btnUnselectAll As System.Windows.Forms.Button
    Friend WithEvents btnSelectAll As System.Windows.Forms.Button
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents btnFilterCBOData As System.Windows.Forms.Button
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents txtftppassword As TextBox
    Friend WithEvents txtftpusername As TextBox
    Friend WithEvents txtFtpHost As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents cboImplementingPartner As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents btnfilter As Button
End Class
