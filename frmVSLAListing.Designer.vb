<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmVSLAListing
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
        Me.cbocounty = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.cbowards = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column4 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column6 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.dtpDateRegisteredCBO = New System.Windows.Forms.DateTimePicker()
        Me.Column5 = New System.Windows.Forms.DataGridViewLinkColumn()
        Me.lnkNew = New System.Windows.Forms.LinkLabel()
        Me.btnpost = New System.Windows.Forms.Button()
        Me.BtnExit = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.txtchairpersonnumber = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtchairpersonname = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.txtVSLAID = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.BtnDelete = New System.Windows.Forms.Button()
        Me.BtnEdit = New System.Windows.Forms.Button()
        Me.dtpDateRegisteredGov = New System.Windows.Forms.DateTimePicker()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.chkIsGovRegistered = New System.Windows.Forms.CheckBox()
        Me.Panel1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'cbocounty
        '
        Me.cbocounty.FormattingEnabled = True
        Me.cbocounty.Location = New System.Drawing.Point(280, 94)
        Me.cbocounty.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cbocounty.Name = "cbocounty"
        Me.cbocounty.Size = New System.Drawing.Size(338, 28)
        Me.cbocounty.TabIndex = 39
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(14, 190)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(182, 20)
        Me.Label5.TabIndex = 37
        Me.Label5.Text = "Date Registered at CBO"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(14, 95)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(59, 20)
        Me.Label4.TabIndex = 36
        Me.Label4.Text = "County"
        '
        'cbowards
        '
        Me.cbowards.FormattingEnabled = True
        Me.cbowards.Location = New System.Drawing.Point(280, 138)
        Me.cbowards.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cbowards.Name = "cbowards"
        Me.cbowards.Size = New System.Drawing.Size(338, 28)
        Me.cbowards.TabIndex = 35
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(14, 138)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(47, 20)
        Me.Label3.TabIndex = 34
        Me.Label3.Text = "Ward"
        '
        'Column1
        '
        Me.Column1.HeaderText = "VSLA ID"
        Me.Column1.Name = "Column1"
        '
        'Column2
        '
        Me.Column2.HeaderText = "VSLA Name"
        Me.Column2.Name = "Column2"
        '
        'Column4
        '
        Me.Column4.HeaderText = "County"
        Me.Column4.Name = "Column4"
        '
        'Column3
        '
        Me.Column3.HeaderText = "Ward"
        Me.Column3.Name = "Column3"
        '
        'Column6
        '
        Me.Column6.HeaderText = "DateRegistered"
        Me.Column6.Name = "Column6"
        '
        'dtpDateRegisteredCBO
        '
        Me.dtpDateRegisteredCBO.Location = New System.Drawing.Point(280, 190)
        Me.dtpDateRegisteredCBO.Margin = New System.Windows.Forms.Padding(2)
        Me.dtpDateRegisteredCBO.Name = "dtpDateRegisteredCBO"
        Me.dtpDateRegisteredCBO.Size = New System.Drawing.Size(236, 26)
        Me.dtpDateRegisteredCBO.TabIndex = 38
        '
        'Column5
        '
        Me.Column5.HeaderText = "Select"
        Me.Column5.Name = "Column5"
        '
        'lnkNew
        '
        Me.lnkNew.AutoSize = True
        Me.lnkNew.Location = New System.Drawing.Point(8, 9)
        Me.lnkNew.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lnkNew.Name = "lnkNew"
        Me.lnkNew.Size = New System.Drawing.Size(73, 20)
        Me.lnkNew.TabIndex = 58
        Me.lnkNew.TabStop = True
        Me.lnkNew.Text = "Add New"
        '
        'btnpost
        '
        Me.btnpost.Enabled = False
        Me.btnpost.Location = New System.Drawing.Point(943, 149)
        Me.btnpost.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnpost.Name = "btnpost"
        Me.btnpost.Size = New System.Drawing.Size(130, 41)
        Me.btnpost.TabIndex = 55
        Me.btnpost.Text = "&Save"
        Me.btnpost.UseVisualStyleBackColor = True
        '
        'BtnExit
        '
        Me.BtnExit.Location = New System.Drawing.Point(943, 206)
        Me.BtnExit.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BtnExit.Name = "BtnExit"
        Me.BtnExit.Size = New System.Drawing.Size(130, 41)
        Me.BtnExit.TabIndex = 56
        Me.BtnExit.Text = "&Close"
        Me.BtnExit.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.AccessibleName = ""
        Me.Panel1.AccessibleRole = System.Windows.Forms.AccessibleRole.None
        Me.Panel1.Controls.Add(Me.chkIsGovRegistered)
        Me.Panel1.Controls.Add(Me.dtpDateRegisteredGov)
        Me.Panel1.Controls.Add(Me.Label8)
        Me.Panel1.Controls.Add(Me.txtchairpersonnumber)
        Me.Panel1.Controls.Add(Me.Label7)
        Me.Panel1.Controls.Add(Me.txtchairpersonname)
        Me.Panel1.Controls.Add(Me.Label6)
        Me.Panel1.Controls.Add(Me.cbocounty)
        Me.Panel1.Controls.Add(Me.dtpDateRegisteredCBO)
        Me.Panel1.Controls.Add(Me.Label5)
        Me.Panel1.Controls.Add(Me.Label4)
        Me.Panel1.Controls.Add(Me.cbowards)
        Me.Panel1.Controls.Add(Me.Label3)
        Me.Panel1.Controls.Add(Me.txtName)
        Me.Panel1.Controls.Add(Me.txtVSLAID)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Enabled = False
        Me.Panel1.Location = New System.Drawing.Point(12, 31)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(655, 401)
        Me.Panel1.TabIndex = 52
        '
        'txtchairpersonnumber
        '
        Me.txtchairpersonnumber.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtchairpersonnumber.Location = New System.Drawing.Point(280, 355)
        Me.txtchairpersonnumber.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtchairpersonnumber.Name = "txtchairpersonnumber"
        Me.txtchairpersonnumber.Size = New System.Drawing.Size(236, 26)
        Me.txtchairpersonnumber.TabIndex = 43
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(14, 355)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(201, 20)
        Me.Label7.TabIndex = 42
        Me.Label7.Text = "Chairperson PhoneNumber"
        '
        'txtchairpersonname
        '
        Me.txtchairpersonname.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtchairpersonname.Location = New System.Drawing.Point(280, 319)
        Me.txtchairpersonname.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtchairpersonname.Name = "txtchairpersonname"
        Me.txtchairpersonname.Size = New System.Drawing.Size(338, 26)
        Me.txtchairpersonname.TabIndex = 41
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(14, 319)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(139, 20)
        Me.Label6.TabIndex = 40
        Me.Label6.Text = "Chairperson name"
        '
        'txtName
        '
        Me.txtName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtName.Location = New System.Drawing.Point(280, 54)
        Me.txtName.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(338, 26)
        Me.txtName.TabIndex = 5
        '
        'txtVSLAID
        '
        Me.txtVSLAID.Enabled = False
        Me.txtVSLAID.Location = New System.Drawing.Point(280, 12)
        Me.txtVSLAID.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtVSLAID.Name = "txtVSLAID"
        Me.txtVSLAID.ReadOnly = True
        Me.txtVSLAID.Size = New System.Drawing.Size(236, 26)
        Me.txtVSLAID.TabIndex = 4
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(14, 54)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(97, 20)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "VSLA Name"
        '
        'Label1
        '
        Me.Label1.AccessibleRole = System.Windows.Forms.AccessibleRole.None
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(14, 12)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(72, 20)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "VSLA ID"
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Column1, Me.Column2, Me.Column4, Me.Column3, Me.Column6, Me.Column5})
        Me.DataGridView1.Location = New System.Drawing.Point(12, 442)
        Me.DataGridView1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(1061, 411)
        Me.DataGridView1.TabIndex = 57
        '
        'BtnDelete
        '
        Me.BtnDelete.Enabled = False
        Me.BtnDelete.Location = New System.Drawing.Point(943, 98)
        Me.BtnDelete.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BtnDelete.Name = "BtnDelete"
        Me.BtnDelete.Size = New System.Drawing.Size(130, 41)
        Me.BtnDelete.TabIndex = 54
        Me.BtnDelete.Text = "&Delete"
        Me.BtnDelete.UseVisualStyleBackColor = True
        '
        'BtnEdit
        '
        Me.BtnEdit.Enabled = False
        Me.BtnEdit.Location = New System.Drawing.Point(943, 49)
        Me.BtnEdit.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BtnEdit.Name = "BtnEdit"
        Me.BtnEdit.Size = New System.Drawing.Size(130, 41)
        Me.BtnEdit.TabIndex = 53
        Me.BtnEdit.Text = "&Edit"
        Me.BtnEdit.UseVisualStyleBackColor = True
        '
        'dtpDateRegisteredGov
        '
        Me.dtpDateRegisteredGov.Location = New System.Drawing.Point(280, 286)
        Me.dtpDateRegisteredGov.Margin = New System.Windows.Forms.Padding(2)
        Me.dtpDateRegisteredGov.Name = "dtpDateRegisteredGov"
        Me.dtpDateRegisteredGov.Size = New System.Drawing.Size(236, 26)
        Me.dtpDateRegisteredGov.TabIndex = 45
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(14, 286)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(250, 20)
        Me.Label8.TabIndex = 44
        Me.Label8.Text = "Date Registered with Government"
        '
        'chkIsGovRegistered
        '
        Me.chkIsGovRegistered.AutoSize = True
        Me.chkIsGovRegistered.Location = New System.Drawing.Point(280, 237)
        Me.chkIsGovRegistered.Name = "chkIsGovRegistered"
        Me.chkIsGovRegistered.Size = New System.Drawing.Size(222, 24)
        Me.chkIsGovRegistered.TabIndex = 46
        Me.chkIsGovRegistered.Text = "Is Government Registered"
        Me.chkIsGovRegistered.UseVisualStyleBackColor = True
        '
        'frmVSLAListing
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1086, 867)
        Me.Controls.Add(Me.lnkNew)
        Me.Controls.Add(Me.btnpost)
        Me.Controls.Add(Me.BtnExit)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.BtnDelete)
        Me.Controls.Add(Me.BtnEdit)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "frmVSLAListing"
        Me.Text = "frmVSLAListing"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cbocounty As ComboBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents cbowards As ComboBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Column1 As DataGridViewTextBoxColumn
    Friend WithEvents Column2 As DataGridViewTextBoxColumn
    Friend WithEvents Column4 As DataGridViewTextBoxColumn
    Friend WithEvents Column3 As DataGridViewTextBoxColumn
    Friend WithEvents Column6 As DataGridViewTextBoxColumn
    Friend WithEvents dtpDateRegisteredCBO As DateTimePicker
    Friend WithEvents Column5 As DataGridViewLinkColumn
    Friend WithEvents lnkNew As LinkLabel
    Friend WithEvents btnpost As Button
    Friend WithEvents BtnExit As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents txtName As TextBox
    Friend WithEvents txtVSLAID As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents BtnDelete As Button
    Friend WithEvents BtnEdit As Button
    Friend WithEvents txtchairpersonnumber As TextBox
    Friend WithEvents Label7 As Label
    Friend WithEvents txtchairpersonname As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents chkIsGovRegistered As CheckBox
    Friend WithEvents dtpDateRegisteredGov As DateTimePicker
    Friend WithEvents Label8 As Label
End Class
