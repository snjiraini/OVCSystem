<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTrainingListing
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
        Me.lnkNew = New System.Windows.Forms.LinkLabel()
        Me.btnpost = New System.Windows.Forms.Button()
        Me.BtnExit = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.txtfacilitator = New System.Windows.Forms.TextBox()
        Me.cboTypeOfTraining = New System.Windows.Forms.ComboBox()
        Me.dtpDateofTraining = New System.Windows.Forms.DateTimePicker()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.cbowards = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txttrainingID = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column4 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column6 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column5 = New System.Windows.Forms.DataGridViewLinkColumn()
        Me.BtnDelete = New System.Windows.Forms.Button()
        Me.BtnEdit = New System.Windows.Forms.Button()
        Me.Panel1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lnkNew
        '
        Me.lnkNew.AutoSize = True
        Me.lnkNew.Location = New System.Drawing.Point(13, 9)
        Me.lnkNew.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lnkNew.Name = "lnkNew"
        Me.lnkNew.Size = New System.Drawing.Size(73, 20)
        Me.lnkNew.TabIndex = 65
        Me.lnkNew.TabStop = True
        Me.lnkNew.Text = "Add New"
        '
        'btnpost
        '
        Me.btnpost.Enabled = False
        Me.btnpost.Location = New System.Drawing.Point(948, 149)
        Me.btnpost.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnpost.Name = "btnpost"
        Me.btnpost.Size = New System.Drawing.Size(130, 41)
        Me.btnpost.TabIndex = 62
        Me.btnpost.Text = "&Save"
        Me.btnpost.UseVisualStyleBackColor = True
        '
        'BtnExit
        '
        Me.BtnExit.Location = New System.Drawing.Point(948, 206)
        Me.BtnExit.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BtnExit.Name = "BtnExit"
        Me.BtnExit.Size = New System.Drawing.Size(130, 41)
        Me.BtnExit.TabIndex = 63
        Me.BtnExit.Text = "&Close"
        Me.BtnExit.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.AccessibleName = ""
        Me.Panel1.AccessibleRole = System.Windows.Forms.AccessibleRole.None
        Me.Panel1.Controls.Add(Me.txtfacilitator)
        Me.Panel1.Controls.Add(Me.cboTypeOfTraining)
        Me.Panel1.Controls.Add(Me.dtpDateofTraining)
        Me.Panel1.Controls.Add(Me.Label5)
        Me.Panel1.Controls.Add(Me.Label4)
        Me.Panel1.Controls.Add(Me.cbowards)
        Me.Panel1.Controls.Add(Me.Label3)
        Me.Panel1.Controls.Add(Me.txttrainingID)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Enabled = False
        Me.Panel1.Location = New System.Drawing.Point(17, 31)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(655, 221)
        Me.Panel1.TabIndex = 59
        '
        'txtfacilitator
        '
        Me.txtfacilitator.Location = New System.Drawing.Point(245, 95)
        Me.txtfacilitator.Name = "txtfacilitator"
        Me.txtfacilitator.Size = New System.Drawing.Size(386, 26)
        Me.txtfacilitator.TabIndex = 41
        '
        'cboTypeOfTraining
        '
        Me.cboTypeOfTraining.FormattingEnabled = True
        Me.cboTypeOfTraining.Location = New System.Drawing.Point(245, 56)
        Me.cboTypeOfTraining.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cboTypeOfTraining.Name = "cboTypeOfTraining"
        Me.cboTypeOfTraining.Size = New System.Drawing.Size(386, 28)
        Me.cboTypeOfTraining.TabIndex = 40
        '
        'dtpDateofTraining
        '
        Me.dtpDateofTraining.Location = New System.Drawing.Point(245, 190)
        Me.dtpDateofTraining.Margin = New System.Windows.Forms.Padding(2)
        Me.dtpDateofTraining.Name = "dtpDateofTraining"
        Me.dtpDateofTraining.Size = New System.Drawing.Size(236, 26)
        Me.dtpDateofTraining.TabIndex = 38
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(56, 190)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(122, 20)
        Me.Label5.TabIndex = 37
        Me.Label5.Text = "Date of Training"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(56, 95)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(78, 20)
        Me.Label4.TabIndex = 36
        Me.Label4.Text = "Facilitator"
        '
        'cbowards
        '
        Me.cbowards.FormattingEnabled = True
        Me.cbowards.Location = New System.Drawing.Point(245, 138)
        Me.cbowards.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cbowards.Name = "cbowards"
        Me.cbowards.Size = New System.Drawing.Size(386, 28)
        Me.cbowards.TabIndex = 35
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(56, 138)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(47, 20)
        Me.Label3.TabIndex = 34
        Me.Label3.Text = "Ward"
        '
        'txttrainingID
        '
        Me.txttrainingID.Enabled = False
        Me.txttrainingID.Location = New System.Drawing.Point(245, 12)
        Me.txttrainingID.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txttrainingID.Name = "txttrainingID"
        Me.txttrainingID.ReadOnly = True
        Me.txttrainingID.Size = New System.Drawing.Size(236, 26)
        Me.txttrainingID.TabIndex = 4
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(56, 54)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(121, 20)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Type of Training"
        '
        'Label1
        '
        Me.Label1.AccessibleRole = System.Windows.Forms.AccessibleRole.None
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(56, 12)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(86, 20)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Training ID"
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Column1, Me.Column2, Me.Column4, Me.Column3, Me.Column6, Me.Column5})
        Me.DataGridView1.Location = New System.Drawing.Point(17, 261)
        Me.DataGridView1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(1061, 369)
        Me.DataGridView1.TabIndex = 64
        '
        'Column1
        '
        Me.Column1.HeaderText = "Training ID"
        Me.Column1.Name = "Column1"
        '
        'Column2
        '
        Me.Column2.HeaderText = "TrainingType"
        Me.Column2.Name = "Column2"
        '
        'Column4
        '
        Me.Column4.HeaderText = "Facilitator"
        Me.Column4.Name = "Column4"
        '
        'Column3
        '
        Me.Column3.HeaderText = "Ward"
        Me.Column3.Name = "Column3"
        '
        'Column6
        '
        Me.Column6.HeaderText = "Training Date"
        Me.Column6.Name = "Column6"
        '
        'Column5
        '
        Me.Column5.HeaderText = "Select"
        Me.Column5.Name = "Column5"
        '
        'BtnDelete
        '
        Me.BtnDelete.Enabled = False
        Me.BtnDelete.Location = New System.Drawing.Point(948, 98)
        Me.BtnDelete.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BtnDelete.Name = "BtnDelete"
        Me.BtnDelete.Size = New System.Drawing.Size(130, 41)
        Me.BtnDelete.TabIndex = 61
        Me.BtnDelete.Text = "&Delete"
        Me.BtnDelete.UseVisualStyleBackColor = True
        '
        'BtnEdit
        '
        Me.BtnEdit.Enabled = False
        Me.BtnEdit.Location = New System.Drawing.Point(948, 49)
        Me.BtnEdit.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BtnEdit.Name = "BtnEdit"
        Me.BtnEdit.Size = New System.Drawing.Size(130, 41)
        Me.BtnEdit.TabIndex = 60
        Me.BtnEdit.Text = "&Edit"
        Me.BtnEdit.UseVisualStyleBackColor = True
        '
        'frmTrainingListing
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1092, 649)
        Me.Controls.Add(Me.lnkNew)
        Me.Controls.Add(Me.btnpost)
        Me.Controls.Add(Me.BtnExit)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.BtnDelete)
        Me.Controls.Add(Me.BtnEdit)
        Me.Name = "frmTrainingListing"
        Me.Text = "frmTrainingListing"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lnkNew As LinkLabel
    Friend WithEvents btnpost As Button
    Friend WithEvents BtnExit As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents dtpDateofTraining As DateTimePicker
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents cbowards As ComboBox
    Friend WithEvents Label3 As Label
    Friend WithEvents txttrainingID As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents BtnDelete As Button
    Friend WithEvents BtnEdit As Button
    Friend WithEvents txtfacilitator As TextBox
    Friend WithEvents cboTypeOfTraining As ComboBox
    Friend WithEvents Column1 As DataGridViewTextBoxColumn
    Friend WithEvents Column2 As DataGridViewTextBoxColumn
    Friend WithEvents Column4 As DataGridViewTextBoxColumn
    Friend WithEvents Column3 As DataGridViewTextBoxColumn
    Friend WithEvents Column6 As DataGridViewTextBoxColumn
    Friend WithEvents Column5 As DataGridViewLinkColumn
End Class
