using System;
using System.Windows.Forms;
using System.Drawing;

namespace GLTech2
{
    internal sealed class Display : Form, IDisposable
    {
        private Image image;
        private Label versionLabel;
        private PictureBox pictureBox;  // Will only be released by GC

        internal Display(bool fullscreen, int width, int height, Bitmap videoSource)
        {
            InitializeComponent();
            pictureBox.Click += OnFocus;
            LostFocus += OnLoseFocus;

            image = videoSource;

            if (fullscreen)
            {
                WindowState = FormWindowState.Maximized;
                FormBorderStyle = FormBorderStyle.None;
            }
            else
                SetSize(width, height);

            // This create an infinite loop that keeps updating the image on the screen.
            pictureBox.Paint += RePaint;
            OnFocus(null, null);
        }

        internal void RePaint(object _ = null, EventArgs __ = null)
        {
            pictureBox.Image = image;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Display));
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.versionLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(640, 360);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // label2
            // 
            this.versionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.versionLabel.Font = new System.Drawing.Font("Bahnschrift Condensed", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.versionLabel.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.versionLabel.Location = new System.Drawing.Point(371, 9);
            this.versionLabel.Name = "label2";
            this.versionLabel.Size = new System.Drawing.Size(257, 19);
            this.versionLabel.TabIndex = 2;
            this.versionLabel.Text = "GL Tech 2.1, build 2406";
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // Display
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.WhiteSpace;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(640, 360);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.pictureBox);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Display";
            this.Text = "GL Tech 2 Screen";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Display_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Display_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        public void SetSize(int width, int height)
        {
            pictureBox.Size = new System.Drawing.Size(width, height);
            this.ClientSize = new System.Drawing.Size(width, height);
        }

        private void Display_KeyDown(object sender, KeyEventArgs e) =>
            Keyboard.KeyDown((Key)e.KeyCode);

        private void Display_KeyUp(object sender, KeyEventArgs e) =>
            Keyboard.KeyUp((Key)e.KeyCode);

        private void OnFocus(object sender, EventArgs e)
        {
            if (Renderer.CaptureMouse)
                Mouse.Enable();
        }

        private void OnLoseFocus(object _, EventArgs __)
        {
            if (Renderer.CaptureMouse)
                Mouse.Disable();
        }
	}
}
