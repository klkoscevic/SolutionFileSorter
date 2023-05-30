﻿namespace OrderProjectsInSlnFile.Forms
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
            this.labelCaption = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(359, 100);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_click);
            // 
            // DoNotShowMesssageAnymore
            // 
            this.DoNotShowMesssageAnymore.AutoSize = true;
            this.DoNotShowMesssageAnymore.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.DoNotShowMesssageAnymore.Location = new System.Drawing.Point(12, 104);
            this.DoNotShowMesssageAnymore.Name = "DoNotShowMesssageAnymore";
            this.DoNotShowMesssageAnymore.Size = new System.Drawing.Size(200, 18);
            this.DoNotShowMesssageAnymore.TabIndex = 1;
            this.DoNotShowMesssageAnymore.Text = "&Do not show this message anymore";
            this.DoNotShowMesssageAnymore.UseVisualStyleBackColor = true;
            // 
            // labelCaption
            // 
            this.labelCaption.AutoSize = true;
            this.labelCaption.Location = new System.Drawing.Point(50, 42);
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.Size = new System.Drawing.Size(252, 13);
            this.labelCaption.TabIndex = 2;
            this.labelCaption.Text = "Projects in the .sln file are now sorted alphabetically.";
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(12, 32);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(32, 32);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox.TabIndex = 3;
            this.pictureBox.TabStop = false;
            // 
            // MyMessageDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 135);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.labelCaption);
            this.Controls.Add(this.DoNotShowMesssageAnymore);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MyMessageDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sorting .sln file";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.CheckBox DoNotShowMesssageAnymore;
        private System.Windows.Forms.Label labelCaption;
        private System.Windows.Forms.PictureBox pictureBox;
    }
}