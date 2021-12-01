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
            ((System.ComponentModel.ISupportInitialize)(this.outputBox)).BeginInit();
            this.SuspendLayout();
            // 
            // outputBox
            // 
            this.outputBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputBox.Location = new System.Drawing.Point(0, 0);
            this.outputBox.Name = "outputBox";
            this.outputBox.Size = new System.Drawing.Size(640, 360);
            this.outputBox.TabIndex = 0;
            this.outputBox.TabStop = false;
            // 
            // GLTechWindowForm
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.WhiteSpace;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(640, 360);
            this.Controls.Add(this.outputBox);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "GLTechWindowForm";
            this.Text = "GL Tech 2.1";
            ((System.ComponentModel.ISupportInitialize)(this.outputBox)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
