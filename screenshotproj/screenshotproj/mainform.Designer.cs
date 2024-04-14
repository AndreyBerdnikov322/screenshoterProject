using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ScreenCaptureApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.CaptureButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CaptureButton
            // 
            this.CaptureButton.Location = new System.Drawing.Point(12, 12);
            this.CaptureButton.Name = "CaptureButton";
            this.CaptureButton.Size = new System.Drawing.Size(200, 50);
            this.CaptureButton.TabIndex = 0;
            this.CaptureButton.Text = "Сделать скрин";
            this.CaptureButton.UseVisualStyleBackColor = true;
            this.CaptureButton.Click += new System.EventHandler(this.CaptureButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(200, 100);
            this.Controls.Add(this.CaptureButton);
            this.Name = "MainForm";
            this.Text = "Screen Capture App";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button CaptureButton;
    }
}


