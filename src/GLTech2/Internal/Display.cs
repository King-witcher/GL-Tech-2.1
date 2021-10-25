using System;
using System.Drawing;
using System.Windows.Forms;

using GLTech2.Scripting;

namespace GLTech2
{
    // This is a complete cheat. I use a windows forms window with a PictureBox to render everything =]
    // Unfortunately that's the only practical way I know yet.
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

            versionLabel.Text = Metadata.EngineName + ", Build " + Metadata.Last_Build;
        }

        internal void RePaint(object _ = null, EventArgs __ = null)
        {
            Behaviour.Frame.EndWindow();
            Behaviour.Frame.BeginWindow();
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
            // versionLabel
            // 
            this.versionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.versionLabel.Font = new System.Drawing.Font("Bahnschrift Condensed", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.versionLabel.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.versionLabel.Location = new System.Drawing.Point(371, 9);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(257, 19);
            this.versionLabel.TabIndex = 2;
            this.versionLabel.Text = "Version";
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
            this.Text = "GL Tech 2.1";
            this.Load += new System.EventHandler(this.Display_Load);
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
            Behaviour.Keyboard.KeyDown((InputKey)e.KeyCode);

        private void Display_KeyUp(object sender, KeyEventArgs e) =>
            Behaviour.Keyboard.KeyUp((InputKey)e.KeyCode);

        private void OnFocus(object sender, EventArgs e)
        {
            if (Renderer.CaptureMouse)
                Behaviour.Mouse.Enable();
        }

        private void OnLoseFocus(object _, EventArgs __)
        {
            if (Renderer.CaptureMouse)
                Behaviour.Mouse.Disable();
        }

        private void Display_Load(object sender, EventArgs e)
        {

        }
    }
}
