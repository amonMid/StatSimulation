namespace StatSimulation
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            wb1 = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)wb1).BeginInit();
            SuspendLayout();
            // 
            // wb1
            // 
            wb1.AllowExternalDrop = true;
            wb1.CreationProperties = null;
            wb1.DefaultBackgroundColor = Color.White;
            wb1.Dock = DockStyle.Fill;
            wb1.Location = new Point(0, 0);
            wb1.Margin = new Padding(3, 2, 3, 2);
            wb1.Name = "wb1";
            wb1.Size = new Size(700, 338);
            wb1.TabIndex = 0;
            wb1.ZoomFactor = 1D;
            wb1.WebMessageReceived += wb1_WebMessageReceived;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(700, 338);
            Controls.Add(wb1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "RO ";
            WindowState = FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)wb1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 wb1;
    }
}
