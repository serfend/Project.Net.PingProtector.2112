namespace CipperGenerator
{
    partial class FrmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.TxtInput = new System.Windows.Forms.TextBox();
			this.TxtFile = new System.Windows.Forms.TextBox();
			this.BtnSelect = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// TxtInput
			// 
			this.TxtInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TxtInput.Location = new System.Drawing.Point(12, 38);
			this.TxtInput.Multiline = true;
			this.TxtInput.Name = "TxtInput";
			this.TxtInput.Size = new System.Drawing.Size(578, 520);
			this.TxtInput.TabIndex = 0;
			this.TxtInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtInput_KeyPress);
			// 
			// TxtFile
			// 
			this.TxtFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.TxtFile.Location = new System.Drawing.Point(12, 2);
			this.TxtFile.Multiline = true;
			this.TxtFile.Name = "TxtFile";
			this.TxtFile.Size = new System.Drawing.Size(526, 30);
			this.TxtFile.TabIndex = 0;
			this.TxtFile.TextChanged += new System.EventHandler(this.TxtFile_TextChanged);
			this.TxtFile.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtInput_KeyPress);
			// 
			// BtnSelect
			// 
			this.BtnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BtnSelect.Location = new System.Drawing.Point(544, 2);
			this.BtnSelect.Name = "BtnSelect";
			this.BtnSelect.Size = new System.Drawing.Size(46, 29);
			this.BtnSelect.TabIndex = 1;
			this.BtnSelect.Text = "浏览";
			this.BtnSelect.UseVisualStyleBackColor = true;
			this.BtnSelect.Click += new System.EventHandler(this.BtnSelect_Click);
			// 
			// FrmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(602, 562);
			this.Controls.Add(this.BtnSelect);
			this.Controls.Add(this.TxtFile);
			this.Controls.Add(this.TxtInput);
			this.Name = "FrmMain";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.FrmMain_Load);
			this.Resize += new System.EventHandler(this.FrmMain_Resize);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private TextBox TxtInput;
        private TextBox TxtFile;
        private Button BtnSelect;
    }
}