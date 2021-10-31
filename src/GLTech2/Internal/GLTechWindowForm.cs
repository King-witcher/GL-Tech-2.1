using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace GLTech2
{
    // This is a complete cheat. I use a windows forms window with a PictureBox to render everything =]
    // Unfortunately that's the only practical way I know yet.
    class GLTechWindowForm : Form, IDisposable
    {
        Image source;
        Label versionLabel;
        PictureBox outputBox;  // Will only be released by GC

        public event Action<TimeSpan> Render;

        bool maximize = false;
        public bool Maximize
        {
            get => maximize;
            set
            {
                maximize = value;

                if (value)
                {
                    FormBorderStyle = FormBorderStyle.None;
                    WindowState = FormWindowState.Maximized;
                }
                else
                {
                    FormBorderStyle = FormBorderStyle.FixedSingle;
                    WindowState = FormWindowState.Normal;
                    Dimensions = (Width, Height);
                }
            }
        }

        public (int width, int height) Dimensions
        {
            get => (ClientSize.Width, ClientSize.Height);
            set
            {
                Size size = new Size(value.width, value.height);
                ClientSize = Size;
            }
        }

        public GLTechWindowForm(Image source)
        {
            InitializeComponent();
            this.source = source;
            versionLabel.Text = Metadata.EngineName + ", Build " + Metadata.Last_Build; // Spaguetti, acoplamento
            outputBox.Paint += RePaint; // Refactor
        }

        Stopwatch rePaintStopwatch = new Stopwatch();
        internal void RePaint(object _ = null, EventArgs __ = null)
        {
            rePaintStopwatch.Restart();
            Render?.Invoke(rePaintStopwatch.Elapsed);
            outputBox.Image = source;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GLTechWindowForm));
            this.outputBox = new System.Windows.Forms.PictureBox();
            this.versionLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.outputBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.outputBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputBox.Location = new System.Drawing.Point(0, 0);
            this.outputBox.Name = "pictureBox";
            this.outputBox.Size = new System.Drawing.Size(640, 360);
            this.outputBox.TabIndex = 0;
            this.outputBox.TabStop = false;
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
            this.Controls.Add(this.outputBox);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Display";
            this.Text = "GL Tech 2.1";
            ((System.ComponentModel.ISupportInitialize)(this.outputBox)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
