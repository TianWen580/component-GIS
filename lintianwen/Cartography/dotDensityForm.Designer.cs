namespace lintianwen.Cartography
{
    partial class dotDensityForm
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
            this.cmbSelField = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCancle = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cmbRendererDensity = new System.Windows.Forms.ComboBox();
            this.cmbSelLyr = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbSelField
            // 
            this.cmbSelField.FormattingEnabled = true;
            this.cmbSelField.Location = new System.Drawing.Point(41, 150);
            this.cmbSelField.Name = "cmbSelField";
            this.cmbSelField.Size = new System.Drawing.Size(253, 20);
            this.cmbSelField.TabIndex = 74;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 131);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 73;
            this.label4.Text = "字段";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("黑体", 23.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(35, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 31);
            this.label3.TabIndex = 72;
            this.label3.Text = "点密度";
            // 
            // btnCancle
            // 
            this.btnCancle.BackColor = System.Drawing.SystemColors.Menu;
            this.btnCancle.ForeColor = System.Drawing.Color.Red;
            this.btnCancle.Location = new System.Drawing.Point(41, 254);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Size = new System.Drawing.Size(253, 23);
            this.btnCancle.TabIndex = 71;
            this.btnCancle.Text = "取消";
            this.btnCancle.UseVisualStyleBackColor = false;
            this.btnCancle.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.btnOK.Location = new System.Drawing.Point(41, 225);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(253, 23);
            this.btnOK.TabIndex = 70;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cmbRendererDensity
            // 
            this.cmbRendererDensity.FormattingEnabled = true;
            this.cmbRendererDensity.Location = new System.Drawing.Point(41, 199);
            this.cmbRendererDensity.Name = "cmbRendererDensity";
            this.cmbRendererDensity.Size = new System.Drawing.Size(253, 20);
            this.cmbRendererDensity.TabIndex = 69;
            // 
            // cmbSelLyr
            // 
            this.cmbSelLyr.FormattingEnabled = true;
            this.cmbSelLyr.Location = new System.Drawing.Point(41, 99);
            this.cmbSelLyr.Name = "cmbSelLyr";
            this.cmbSelLyr.Size = new System.Drawing.Size(253, 20);
            this.cmbSelLyr.TabIndex = 68;
            this.cmbSelLyr.SelectedIndexChanged += new System.EventHandler(this.cmbSelLyr_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 182);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 67;
            this.label2.Text = "渲染密度";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 66;
            this.label1.Text = "图层";
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.FlatAppearance.BorderSize = 3;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(0, 1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(338, 302);
            this.button1.TabIndex = 75;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // dotDensityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 303);
            this.Controls.Add(this.cmbSelField);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnCancle);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cmbRendererDensity);
            this.Controls.Add(this.cmbSelLyr);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "dotDensityForm";
            this.Text = "dotDensityForm";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbSelField;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCancle;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cmbRendererDensity;
        private System.Windows.Forms.ComboBox cmbSelLyr;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
    }
}