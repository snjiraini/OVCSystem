<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmStarterKitListing
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.lnkNew = New System.Windows.Forms.LinkLabel()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.btnpost = New System.Windows.Forms.Button()
        Me.BtnExit = New System.Windows.Forms.Button()
        Me.BtnDelete = New System.Windows.Forms.Button()
        Me.BtnEdit = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cbovaluechain = New System.Windows.Forms.ComboBox()
        Me.txtstarterkitname = New System.Windows.Forms.TextBox()
        Me.txtStarterkitID = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column4 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column5 = New System.Windows.Forms.DataGridViewLinkColumn()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lnkNew
        '
        Me.lnkNew.AutoSize = True
        Me.lnkNew.Location = New System.Drawing.Point(4, -2)
        Me.lnkNew.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lnkNew.Name = "lnkNew"
        Me.lnkNew.Size = New System.Drawing.Size(73, 20)
        Me.lnkNew.TabIndex = 44
        Me.lnkNew.TabStop = True
        Me.lnkNew.Text = "Add New"
        '
        'DataGridView1
        '
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridView1.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Column1, Me.Column2, Me.Column3, Me.Column4, Me.Column5})
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridView1.DefaultCellStyle = DataGridViewCellStyle2
        Me.DataGridView1.Location = New System.Drawing.Point(8, 219)
        Me.DataGridView1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.DataGridView1.Name = "DataGridView1"
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridView1.RowHeadersDefaultCellStyle = DataGridViewCellStyle3
        Me.DataGridView1.Size = New System.Drawing.Size(656, 411)
        Me.DataGridView1.TabIndex = 43
        '
        'btnpost
        '
        Me.btnpost.Enabled = False
        Me.btnpost.Location = New System.Drawing.Point(687, 153)
        Me.btnpost.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnpost.Name = "btnpost"
        Me.btnpost.Size = New System.Drawing.Size(130, 55)
        Me.btnpost.TabIndex = 41
        Me.btnpost.Text = "&Save"
        Me.btnpost.UseVisualStyleBackColor = True
        '
        'BtnExit
        '
        Me.BtnExit.Location = New System.Drawing.Point(687, 219)
        Me.BtnExit.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BtnExit.Name = "BtnExit"
        Me.BtnExit.Size = New System.Drawing.Size(130, 55)
        Me.BtnExit.TabIndex = 42
        Me.BtnExit.Text = "&Close"
        Me.BtnExit.UseVisualStyleBackColor = True
        '
        'BtnDelete
        '
        Me.BtnDelete.Enabled = False
        Me.BtnDelete.Location = New System.Drawing.Point(687, 86)
        Me.BtnDelete.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BtnDelete.Name = "BtnDelete"
        Me.BtnDelete.Size = New System.Drawing.Size(130, 55)
        Me.BtnDelete.TabIndex = 40
        Me.BtnDelete.Text = "&Delete"
        Me.BtnDelete.UseVisualStyleBackColor = True
        '
        'BtnEdit
        '
        Me.BtnEdit.Enabled = False
        Me.BtnEdit.Location = New System.Drawing.Point(687, 22)
        Me.BtnEdit.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BtnEdit.Name = "BtnEdit"
        Me.BtnEdit.Size = New System.Drawing.Size(130, 55)
        Me.BtnEdit.TabIndex = 39
        Me.BtnEdit.Text = "&Edit"
        Me.BtnEdit.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.AccessibleName = ""
        Me.Panel1.AccessibleRole = System.Windows.Forms.AccessibleRole.None
        Me.Panel1.Controls.Add(Me.Label3)
        Me.Panel1.Controls.Add(Me.cbovaluechain)
        Me.Panel1.Controls.Add(Me.txtstarterkitname)
        Me.Panel1.Controls.Add(Me.txtStarterkitID)
        Me.Panel1.Controls.Add(Me.Label2)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Enabled = False
        Me.Panel1.Location = New System.Drawing.Point(9, 22)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(656, 145)
        Me.Panel1.TabIndex = 38
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(56, 100)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(95, 20)
        Me.Label3.TabIndex = 34
        Me.Label3.Text = "Value Chain"
        '
        'cbovaluechain
        '
        Me.cbovaluechain.FormattingEnabled = True
        Me.cbovaluechain.Location = New System.Drawing.Point(244, 95)
        Me.cbovaluechain.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cbovaluechain.Name = "cbovaluechain"
        Me.cbovaluechain.Size = New System.Drawing.Size(236, 28)
        Me.cbovaluechain.TabIndex = 33
        '
        'txtstarterkitname
        '
        Me.txtstarterkitname.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper
        Me.txtstarterkitname.Location = New System.Drawing.Point(244, 54)
        Me.txtstarterkitname.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtstarterkitname.Name = "txtstarterkitname"
        Me.txtstarterkitname.Size = New System.Drawing.Size(236, 26)
        Me.txtstarterkitname.TabIndex = 5
        '
        'txtStarterkitID
        '
        Me.txtStarterkitID.Enabled = False
        Me.txtStarterkitID.Location = New System.Drawing.Point(244, 12)
        Me.txtStarterkitID.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.txtStarterkitID.Name = "txtStarterkitID"
        Me.txtStarterkitID.ReadOnly = True
        Me.txtStarterkitID.Size = New System.Drawing.Size(236, 26)
        Me.txtStarterkitID.TabIndex = 4
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(56, 54)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(126, 20)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Starter Kit Name"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(56, 12)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(95, 20)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Starterkit ID"
        '
        'Column1
        '
        Me.Column1.HeaderText = "StarterKit ID"
        Me.Column1.Name = "Column1"
        '
        'Column2
        '
        Me.Column2.HeaderText = "StarterKit Name"
        Me.Column2.Name = "Column2"
        '
        'Column3
        '
        Me.Column3.HeaderText = "ValueChain"
        Me.Column3.Name = "Column3"
        '
        'Column4
        '
        Me.Column4.HeaderText = "ValueChain Type"
        Me.Column4.Name = "Column4"
        '
        'Column5
        '
        Me.Column5.HeaderText = "Select"
        Me.Column5.Name = "Column5"
        '
        'frmStarterKitListing
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(830, 667)
        Me.Controls.Add(Me.lnkNew)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.btnpost)
        Me.Controls.Add(Me.BtnExit)
        Me.Controls.Add(Me.BtnDelete)
        Me.Controls.Add(Me.BtnEdit)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "frmStarterKitListing"
        Me.Text = "frmStarterKitListing"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lnkNew As LinkLabel
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents btnpost As Button
    Friend WithEvents BtnExit As Button
    Friend WithEvents BtnDelete As Button
    Friend WithEvents BtnEdit As Button
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents cbovaluechain As ComboBox
    Friend WithEvents txtstarterkitname As TextBox
    Friend WithEvents txtStarterkitID As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents Column1 As DataGridViewTextBoxColumn
    Friend WithEvents Column2 As DataGridViewTextBoxColumn
    Friend WithEvents Column3 As DataGridViewTextBoxColumn
    Friend WithEvents Column4 As DataGridViewTextBoxColumn
    Friend WithEvents Column5 As DataGridViewLinkColumn
End Class
