namespace ParaglidingToolbox
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolPanel = new System.Windows.Forms.Panel();
            this.skglControl = new SkiaSharp.Views.Desktop.SKGLControl();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(719, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(37, 20);
            this.toolStripMenuItem1.Text = "&File";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 500);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(719, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip";
            // 
            // toolPanel
            // 
            this.toolPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.toolPanel.Location = new System.Drawing.Point(531, 24);
            this.toolPanel.Name = "toolPanel";
            this.toolPanel.Size = new System.Drawing.Size(188, 476);
            this.toolPanel.TabIndex = 2;
            // 
            // skglControl
            // 
            this.skglControl.BackColor = System.Drawing.Color.Black;
            this.skglControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skglControl.Location = new System.Drawing.Point(0, 24);
            this.skglControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.skglControl.Name = "skglControl";
            this.skglControl.Size = new System.Drawing.Size(531, 476);
            this.skglControl.TabIndex = 3;
            this.skglControl.VSync = true;
            this.skglControl.PaintSurface += new System.EventHandler<SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs>(this.skglControl_PaintSurface);
            this.skglControl.SizeChanged += new System.EventHandler(this.skglControl_SizeChanged);
            this.skglControl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.skglControl_KeyDown);
            this.skglControl.KeyUp += new System.Windows.Forms.KeyEventHandler(this.skglControl_KeyUp);
            this.skglControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.skglControl_MouseDown);
            this.skglControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.skglControl_MouseMove);
            this.skglControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.skglControl_MouseUp);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(719, 522);
            this.Controls.Add(this.skglControl);
            this.Controls.Add(this.toolPanel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "ParaglidingToolbox";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private StatusStrip statusStrip1;
        private Panel toolPanel;
        private SkiaSharp.Views.Desktop.SKGLControl skglControl;
    }
}