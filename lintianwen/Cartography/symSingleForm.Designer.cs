namespace lintianwen.Cartography
{
    partial class symSingleForm
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
            this.cmbSelColor = new System.Windows.Forms.ComboBox();
            this.cmbSelLyr = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("黑体", 23.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(41, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(179, 31);
            this.label3.TabIndex = 47;
            this.label3.Text = "单一符号化";
            // 
            // btnCancle
            // 
            this.btnCancle.BackColor = System.Drawing.SystemColors.Menu;
            this.btnCancle.ForeColor = System.Drawing.Color.Red;
            this.btnCancle.Location = new System.Drawing.Point(47, 212);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(253, 23);
            this.btnCancle.TabIndex = 46;
            this.btnCancle.Text = "取消";
            this.btnCancle.UseVisualStyleBackColor = false;
            this.btnCancle.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnOK.Location = new System.Drawing.Point(47, 183);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(253, 23);
            this.btnOK.TabIndex = 45;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cmbSelColor
            // 
            this.cmbSelColor.FormattingEnabled = true;
            this.cmbSelColor.Location = new System.Drawing.Point(47, 157);
            this.cmbSelColor.Name = "cmbSelColor";
            this.cmbSelColor.Size = new System.Drawing.Size(253, 20);
            this.cmbSelColor.TabIndex = 44;
            this.cmbSelColor.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cmbSelColor_DrawItem);
            this.cmbSelColor.SelectedIndexChanged += new System.EventHandler(this.cmbSelColor_SelectedIndexChanged);
            // 
            // cmbSelLyr
            // 
            this.cmbSelLyr.FormattingEnabled = true;
            this.cmbSelLyr.Location = new System.Drawing.Point(47, 107);
            this.cmbSelLyr.Name = "cmbSelLyr";
            this.cmbSelLyr.Size = new System.Drawing.Size(253, 20);
            this.cmbSelLyr.TabIndex = 43;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(45, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 42;
            this.label2.Text = "颜色";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 41;
            this.label1.Text = "图层";
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.FlatAppearance.BorderSize = 3;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(1, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(345, 261);
            this.button1.TabIndex = 48;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // symSingleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 263);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancle);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cmbSelColor);
            this.Controls.Add(this.cmbSelLyr);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "symSingleForm";
            this.Text = "symSingleForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.symSingleForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCancle;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cmbSelColor;
        private System.Windows.Forms.ComboBox cmbSelLyr;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;

    }
}