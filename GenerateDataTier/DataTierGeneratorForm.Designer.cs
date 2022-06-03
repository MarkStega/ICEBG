namespace ICEBG.GenerateDataTier
{
    partial class DataTierGeneratorForm
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.generateButton = new System.Windows.Forms.Button();
            this.labelStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(13, 13);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(490, 23);
            this.progressBar.TabIndex = 0;
            // 
            // generateButton
            // 
            this.generateButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.generateButton.Location = new System.Drawing.Point(516, 13);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(72, 24);
            this.generateButton.TabIndex = 12;
            this.generateButton.Text = "Generate";
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // labelStatus
            // 
            this.labelStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatus.Location = new System.Drawing.Point(12, 49);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(491, 23);
            this.labelStatus.TabIndex = 14;
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DataTierGeneratorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 93);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.generateButton);
            this.Controls.Add(this.progressBar);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DataTierGeneratorForm";
            this.Text = "ICEBG Data Tier Generator";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button generateButton;
        private System.Windows.Forms.Label labelStatus;
    }
}
