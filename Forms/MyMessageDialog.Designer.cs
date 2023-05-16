namespace OrderProjectsInSlnFile.Forms
{
    partial class MyMessageDialog
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
            this.btnOk = new System.Windows.Forms.Button();
            this.DoNotShowMesssageAnymore = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(295, 100);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(102, 34);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_click);
            // 
            // DoNotShowMesssageAnymore
            // 
            this.DoNotShowMesssageAnymore.AutoSize = true;
            this.DoNotShowMesssageAnymore.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.DoNotShowMesssageAnymore.Location = new System.Drawing.Point(12, 76);
            this.DoNotShowMesssageAnymore.Name = "DoNotShowMesssageAnymore";
            this.DoNotShowMesssageAnymore.Size = new System.Drawing.Size(200, 18);
            this.DoNotShowMesssageAnymore.TabIndex = 1;
            this.DoNotShowMesssageAnymore.Text = "Do not show this message anymore";
            this.DoNotShowMesssageAnymore.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(365, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "Your projects in the .sln file are sorted alphabetically.";
            // 
            // MyMessageDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 154);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DoNotShowMesssageAnymore);
            this.Controls.Add(this.btnOk);
            this.Name = "MyMessageDialog";
            this.Text = "Sorting .sln file is done.";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.CheckBox DoNotShowMesssageAnymore;
        private System.Windows.Forms.Label label1;
    }
}