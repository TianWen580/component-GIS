namespace lintianwen.Cartography
{
    partial class uniqueValueDualFields
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSelLyr = new System.Windows.Forms.ComboBox();
            this.groupControl2 = new System.Windows.Forms.GroupBox();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupControl1 = new System.Windows.Forms.GroupBox();
            this.lstboxField = new System.Windows.Forms.ListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnDelOne = new System.Windows.Forms.Button();
            this.btnAddOne = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.groupControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "图层";
            // 
            // cmbSelLyr
            // 
            this.cmbSelLyr.FormattingEnabled = true;
            this.cmbSelLyr.Location = new System.Drawing.Point(43, 93);
            this.cmbSelLyr.Name = "cmbSelLyr";
            this.cmbSelLyr.Size = new System.Drawing.Size(250, 20);
            this.cmbSelLyr.TabIndex = 1;
            this.cmbSelLyr.SelectedIndexChanged += new System.EventHandler(this.cmbSelLyr_SelectedIndexChanged);
            // 
            // groupControl2
            // 
            this.groupControl2.Controls.Add(this.dataGridView);
            this.groupControl2.Location = new System.Drawing.Point(44, 260);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(250, 159);
            this.groupControl2.TabIndex = 24;
            this.groupControl2.TabStop = false;
            this.groupControl2.Text = "渲染字段";
            // 
            // dataGridView
            // 
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1});
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 17);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.Size = new System.Drawing.Size(244, 139);
            this.dataGridView.TabIndex = 6;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "字段";
            this.Column1.Name = "Column1";
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.lstboxField);
            this.groupControl1.Location = new System.Drawing.Point(43, 130);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(250, 95);
            this.groupControl1.TabIndex = 23;
            this.groupControl1.TabStop = false;
            this.groupControl1.Text = "字段选择";
            // 
            // lstboxField
            // 
            this.lstboxField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstboxField.FormattingEnabled = true;
            this.lstboxField.ItemHeight = 12;
            this.lstboxField.Location = new System.Drawing.Point(3, 17);
            this.lstboxField.Name = "lstboxField";
            this.lstboxField.Size = new System.Drawing.Size(244, 75);
            this.lstboxField.TabIndex = 2;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCancel.ForeColor = System.Drawing.Color.Red;
            this.btnCancel.Location = new System.Drawing.Point(43, 454);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(250, 23);
            this.btnCancel.TabIndex = 22;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnOK.Location = new System.Drawing.Point(43, 425);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(250, 23);
            this.btnOK.TabIndex = 21;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnDelOne
            // 
            this.btnDelOne.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnDelOne.Font = new System.Drawing.Font("Consolas", 10F);
            this.btnDelOne.ForeColor = System.Drawing.Color.Red;
            this.btnDelOne.Location = new System.Drawing.Point(43, 231);
            this.btnDelOne.Name = "btnDelOne";
            this.btnDelOne.Size = new System.Drawing.Size(122, 23);
            this.btnDelOne.TabIndex = 20;
            this.btnDelOne.Text = "×";
            this.btnDelOne.UseVisualStyleBackColor = false;
            this.btnDelOne.Click += new System.EventHandler(this.btnDelOne_Click);
            // 
            // btnAddOne
            // 
            this.btnAddOne.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnAddOne.Location = new System.Drawing.Point(171, 231);
            this.btnAddOne.Name = "btnAddOne";
            this.btnAddOne.Size = new System.Drawing.Size(122, 23);
            this.btnAddOne.TabIndex = 19;
            this.btnAddOne.Text = "↓";
            this.btnAddOne.UseVisualStyleBackColor = true;
            this.btnAddOne.Click += new System.EventHandler(this.btnAddOne_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("黑体", 23.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(38, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(278, 31);
            this.label3.TabIndex = 55;
            this.label3.Text = "唯一值符号化·多";
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.FlatAppearance.BorderSize = 3;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(338, 510);
            this.button1.TabIndex = 56;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // uniqueValueDualFields
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 510);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupControl2);
            this.Controls.Add(this.groupControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnDelOne);
            this.Controls.Add(this.btnAddOne);
            this.Controls.Add(this.cmbSelLyr);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "uniqueValueDualFields";
            this.Text = "uniqueValueDualFields";
            this.TopMost = true;
            this.groupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSelLyr;
        private System.Windows.Forms.GroupBox groupControl2;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.GroupBox groupControl1;
        private System.Windows.Forms.ListBox lstboxField;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnDelOne;
        private System.Windows.Forms.Button btnAddOne;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
    }
}