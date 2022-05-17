namespace lintianwen.Cartography
{
    partial class graduatedcolorsForm
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
            this.label3 = new System.Windows.Forms.Label();
            this.btnCancle = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cmbnumclasses = new System.Windows.Forms.ComboBox();
            this.cmbSelLyr = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSelField = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("黑体", 23.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(33, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(146, 31);
            this.label3.TabIndex = 54;
            this.label3.Text = "定量色彩";
            // 
            // btnCancle
            // 
            this.btnCancle.BackColor = System.Drawing.SystemColors.Menu;
            this.btnCancle.ForeColor = System.Drawing.Color.Red;
            this.btnCancle.Location = new System.Drawing.Point(39, 260);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(253, 23);
            this.btnCancle.TabIndex = 53;
            this.btnCancle.Text = "取消";
            this.btnCancle.UseVisualStyleBackColor = false;
            this.btnCancle.Click += new System.EventHandler(this.btncancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnOK.Location = new System.Drawing.Point(39, 231);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(253, 23);
            this.btnOK.TabIndex = 52;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // cmbnumclasses
            // 
            this.cmbnumclasses.FormattingEnabled = true;
            this.cmbnumclasses.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7"});
            this.cmbnumclasses.Location = new System.Drawing.Point(39, 205);
            this.cmbnumclasses.Name = "cmbnumclasses";
            this.cmbnumclasses.Size = new System.Drawing.Size(253, 20);
            this.cmbnumclasses.TabIndex = 51;
            // 
            // cmbSelLyr
            // 
            this.cmbSelLyr.FormattingEnabled = true;
            this.cmbSelLyr.Location = new System.Drawing.Point(39, 105);
            this.cmbSelLyr.Name = "cmbSelLyr";
            this.cmbSelLyr.Size = new System.Drawing.Size(253, 20);
            this.cmbSelLyr.TabIndex = 50;
            this.cmbSelLyr.SelectedIndexChanged += new System.EventHandler(this.cmbSelLyr_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 188);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 49;
            this.label2.Text = "颜色";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 48;
            this.label1.Text = "图层";
            // 
            // cmbSelField
            // 
            this.cmbSelField.FormattingEnabled = true;
            this.cmbSelField.Location = new System.Drawing.Point(39, 156);
            this.cmbSelField.Name = "cmbSelField";
            this.cmbSelField.Size = new System.Drawing.Size(253, 20);
            this.cmbSelField.TabIndex = 56;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(37, 137);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 55;
            this.label4.Text = "字段";
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.FlatAppearance.BorderSize = 3;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(1, 1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(331, 310);
            this.button1.TabIndex = 57;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // graduatedcolorsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 311);
            this.Controls.Add(this.cmbSelField);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancle);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cmbnumclasses);
            this.Controls.Add(this.cmbSelLyr);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "graduatedcolorsForm";
            this.Text = "graduatedcolorsForm";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCancle;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cmbnumclasses;
        private System.Windows.Forms.ComboBox cmbSelLyr;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSelField;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
    }
}