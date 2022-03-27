using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Engine
{
    // This is a complete cheat. I use a windows forms window with a PictureBox to render everything =]
    // Unfortunately that's the only practical way I know yet.
    class ConcreteWindow : Form, IDisposable
    {
        PictureBox outputBox;  // Will only be released by GC

        public event Action<TimeSpan> Render;

        public ConcreteWindow(Image source, bool fullscreen)
        {
            Stopwatch rePaintStopwatch = new Stopwatch();

            InitializeComponent();
            this.Dimensions = (source.Width, source.Height);
            if (fullscreen)
            {
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }


            outputBox.Paint += RePaint; // Refactor
            void RePaint(object _ = null, EventArgs __ = null)
            {
                rePaintStopwatch.Restart();
                Render?.Invoke(rePaintStopwatch.Elapsed);
                outputBox.Image = source;
            }
        }

        public (int width, int height) Dimensions
        {
            get => (ClientSize.Width, ClientSize.Height);
            init
            {
                Size size = new Size(value.width, value.height);
                ClientSize = size;
            }
        }

        public bool Maximized { get => WindowState == FormWindowState.Maximized; }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConcreteWindow));
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
            this.Name = "ConcreteWindow";
            this.Text = "GL Tech 2.1";
            ((System.ComponentModel.ISupportInitialize)(this.outputBox)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
