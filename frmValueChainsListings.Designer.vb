<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmValueChainsListings
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
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.txtValueChainID = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lnkNew = New System.Windows.Forms.LinkLabel()
        Me.btnpost = New System.Windows.Forms.Button()
        Me.BtnExit = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cbovaluechaintype = New System.Windows.Forms.ComboBox()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column5 = New System.Windows.Forms.DataGridViewLinkColumn()
        Me.BtnDelete = New System.Windows.Forms.Button()
        Me.BtnEdit = New System.Windows.Forms.Button()
        Me.Panel1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtName
        '
        Me.txtName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtName.Location = New System.Drawing.Point(245, 54)
        Me.txtName.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(236, 26)
        Me.txtName.TabIndex = 5
        '
        'txtValueChainID
        '
        Me.txtValueChainID.Enabled = False
        Me.txtValueChainID.Location = New System.Drawing.Point(245, 12)
        Me.txtValueChainID.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtValueChainID.Name = "txtValueChainID"
        Me.txtValueChainID.ReadOnly = True
        Me.txtValueChainID.Size = New System.Drawing.Size(236, 26)
        Me.txtValueChainID.TabIndex = 4
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(56, 54)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(137, 20)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "ValueChain Name"
        '
        'Label1
        '
        Me.Label1.AccessibleRole = System.Windows.Forms.AccessibleRole.None
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(56, 12)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(109, 20)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Valuechain ID"
        '
        'lnkNew
        '
        Me.lnkNew.AutoSize = True
        Me.lnkNew.Location = New System.Drawing.Point(13, 9)
        Me.lnkNew.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lnkNew.Name = "lnkNew"
        Me.lnkNew.Size = New System.Drawing.Size(73, 20)
        Me.lnkNew.TabIndex = 51
        Me.lnkNew.TabStop = True
        Me.lnkNew.Text = "Add New"
        '
        'btnpost
        '
        Me.btnpost.Enabled = False
        Me.btnpost.Location = New System.Drawing.Point(784, 145)
        Me.btnpost.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnpost.Name = "btnpost"
        Me.btnpost.Size = New System.Drawing.Size(130, 41)
        Me.btnpost.TabIndex = 48
        Me.btnpost.Text = "&Save"
        Me.btnpost.UseVisualStyleBackColor = True
        '
        'BtnExit
        '
        Me.BtnExit.Location = New System.Drawing.Point(784, 202)
        Me.BtnExit.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BtnExit.Name = "BtnExit"
        Me.BtnExit.Size = New System.Drawing.Size(130, 41)
        Me.BtnExit.TabIndex = 49
        Me.BtnExit.Text = "&Close"
        Me.BtnExit.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.AccessibleName = ""
        Me.Panel1.AccessibleRole = System.Windows.Forms.AccessibleRole.None
        Me.Panel1.Controls.Add(Me.Label3)
        Me.Panel1.Controls.Add(Me.cbovaluechaintype)
        Me.Panel1.Controls.Add(Me.txtName)
        Me.Panel1.Controls.Add(Me.txtValueChainID)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Enabled = False
        Me.Panel1.Location = New System.Drawing.Point(17, 33)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(655, 221)
        Me.Panel1.TabIndex = 45
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(56, 101)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(129, 20)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "ValueChain Type"
        '
        'cbovaluechaintype
        '
        Me.cbovaluechaintype.FormattingEnabled = True
        Me.cbovaluechaintype.Location = New System.Drawing.Point(245, 98)
        Me.cbovaluechaintype.Name = "cbovaluechaintype"
        Me.cbovaluechaintype.Size = New System.Drawing.Size(236, 28)
        Me.cbovaluechaintype.TabIndex = 6
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Column1, Me.Column2, Me.Column3, Me.Column5})
        Me.DataGridView1.Location = New System.Drawing.Point(17, 263)
        Me.DataGridView1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(897, 369)
        Me.DataGridView1.TabIndex = 50
        '
        'Column1
        '
        Me.Column1.HeaderText = "ValueChain ID"
        Me.Column1.Name = "Column1"
        '
        'Column2
        '
        Me.Column2.HeaderText = "ValueChain Name"
        Me.Column2.Name = "Column2"
        '
        'Column3
        '
        Me.Column3.HeaderText = "ValueChain Type"
        Me.Column3.Name = "Column3"
        '
        'Column5
        '
        Me.Column5.HeaderText = "Select"
        Me.Column5.Name = "Column5"
        '
        'BtnDelete
        '
        Me.BtnDelete.Enabled = False
        Me.BtnDelete.Location = New System.Drawing.Point(784, 94)
        Me.BtnDelete.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BtnDelete.Name = "BtnDelete"
        Me.BtnDelete.Size = New System.Drawing.Size(130, 41)
        Me.BtnDelete.TabIndex = 47
        Me.BtnDelete.Text = "&Delete"
        Me.BtnDelete.UseVisualStyleBackColor = True
        '
        'BtnEdit
        '
        Me.BtnEdit.Enabled = False
        Me.BtnEdit.Location = New System.Drawing.Point(784, 45)
        Me.BtnEdit.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BtnEdit.Name = "BtnEdit"
        Me.BtnEdit.Size = New System.Drawing.Size(130, 41)
        Me.BtnEdit.TabIndex = 46
        Me.BtnEdit.Text = "&Edit"
        Me.BtnEdit.UseVisualStyleBackColor = True
        '
        'frmValueChainsListings
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(922, 648)
        Me.Controls.Add(Me.lnkNew)
        Me.Controls.Add(Me.btnpost)
        Me.Controls.Add(Me.BtnExit)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.BtnDelete)
        Me.Controls.Add(Me.BtnEdit)
        Me.Name = "frmValueChainsListings"
        Me.Text = "frmValueChainsListings"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtName As TextBox
    Friend WithEvents txtValueChainID As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents lnkNew As LinkLabel
    Friend WithEvents btnpost As Button
    Friend WithEvents BtnExit As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents BtnDelete As Button
    Friend WithEvents BtnEdit As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents cbovaluechaintype As ComboBox
    Friend WithEvents Column1 As DataGridViewTextBoxColumn
    Friend WithEvents Column2 As DataGridViewTextBoxColumn
    Friend WithEvents Column3 As DataGridViewTextBoxColumn
    Friend WithEvents Column5 As DataGridViewLinkColumn
End Class
