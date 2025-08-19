namespace CommandScheduler
{
    partial class LogForm
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
            this.textBoxLogs = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            //
            // textBoxLogs
            //
            this.textBoxLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLogs.Location = new System.Drawing.Point(0, 0);
            this.textBoxLogs.Multiline = true;
            this.textBoxLogs.Name = "textBoxLogs";
            this.textBoxLogs.ReadOnly = true;
            this.textBoxLogs.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxLogs.Size = new System.Drawing.Size(800, 450);
            this.textBoxLogs.TabIndex = 0;
            this.textBoxLogs.WordWrap = false;
            //
            // LogForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBoxLogs);
            this.Name = "LogForm";
            this.Text = "Command Execution Log";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox textBoxLogs;
    }
}
