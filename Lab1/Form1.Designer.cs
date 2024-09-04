namespace Lab1
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.openGLControl1 = new SharpGL.OpenGLControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.drawButton = new System.Windows.Forms.Button();
            this.dragButton = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.colorPickPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.redLable = new System.Windows.Forms.Label();
            this.greenLable = new System.Windows.Forms.Label();
            this.blueLable = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.changeColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redInput = new System.Windows.Forms.NumericUpDown();
            this.greenInput = new System.Windows.Forms.NumericUpDown();
            this.blueInput = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.openGLControl1)).BeginInit();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.colorPickPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.redInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueInput)).BeginInit();
            this.SuspendLayout();
            // 
            // openGLControl1
            // 
            this.openGLControl1.Cursor = System.Windows.Forms.Cursors.Default;
            this.openGLControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.openGLControl1.DrawFPS = true;
            this.openGLControl1.FrameRate = 60;
            this.openGLControl1.Location = new System.Drawing.Point(100, 0);
            this.openGLControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.openGLControl1.Name = "openGLControl1";
            this.openGLControl1.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            this.openGLControl1.RenderContextType = SharpGL.RenderContextType.DIBSection;
            this.openGLControl1.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
            this.openGLControl1.Size = new System.Drawing.Size(700, 450);
            this.openGLControl1.TabIndex = 0;
            this.openGLControl1.OpenGLDraw += new SharpGL.RenderEventHandler(this.openGLControl1_OpenGLDraw);
            this.openGLControl1.Resized += new System.EventHandler(this.openGLControl1_Resized);
            this.openGLControl1.Load += new System.EventHandler(this.openGLControl1_Load);
            this.openGLControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.openGLControl1_KeyDown);
            this.openGLControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.openGLControl1_MouseDown);
            this.openGLControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.openGLControl1_MouseMove);
            this.openGLControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.openGLControl1_MouseUp);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.openGLControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(100, 0, 0, 0);
            this.panel1.Size = new System.Drawing.Size(800, 450);
            this.panel1.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.drawButton);
            this.flowLayoutPanel1.Controls.Add(this.dragButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(100, 450);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // drawButton
            // 
            this.drawButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.drawButton.Location = new System.Drawing.Point(3, 3);
            this.drawButton.Name = "drawButton";
            this.drawButton.Size = new System.Drawing.Size(95, 23);
            this.drawButton.TabIndex = 0;
            this.drawButton.Text = "Draw";
            this.drawButton.UseVisualStyleBackColor = true;
            this.drawButton.Click += new System.EventHandler(this.drawButton_Click);
            // 
            // dragButton
            // 
            this.dragButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.dragButton.Location = new System.Drawing.Point(3, 32);
            this.dragButton.Name = "dragButton";
            this.dragButton.Size = new System.Drawing.Size(95, 23);
            this.dragButton.TabIndex = 1;
            this.dragButton.Text = "Move";
            this.dragButton.UseVisualStyleBackColor = true;
            this.dragButton.Click += new System.EventHandler(this.dragButton_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeColorToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(167, 28);
            // 
            // colorPickPanel
            // 
            this.colorPickPanel.Controls.Add(this.tableLayoutPanel1);
            this.colorPickPanel.Location = new System.Drawing.Point(300, 220);
            this.colorPickPanel.Name = "colorPickPanel";
            this.colorPickPanel.Size = new System.Drawing.Size(200, 100);
            this.colorPickPanel.TabIndex = 1;
            this.colorPickPanel.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.6F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.4F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 67F));
            this.tableLayoutPanel1.Controls.Add(this.blueInput, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.greenInput, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.redLable, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.greenLable, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.blueLable, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.button1, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.redInput, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.21429F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.78571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(200, 100);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // redLable
            // 
            this.redLable.AutoSize = true;
            this.redLable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.redLable.Location = new System.Drawing.Point(3, 0);
            this.redLable.Name = "redLable";
            this.redLable.Size = new System.Drawing.Size(59, 29);
            this.redLable.TabIndex = 0;
            this.redLable.Text = "R";
            this.redLable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // greenLable
            // 
            this.greenLable.AutoSize = true;
            this.greenLable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.greenLable.Location = new System.Drawing.Point(68, 0);
            this.greenLable.Name = "greenLable";
            this.greenLable.Size = new System.Drawing.Size(61, 29);
            this.greenLable.TabIndex = 1;
            this.greenLable.Text = "G";
            this.greenLable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // blueLable
            // 
            this.blueLable.AutoSize = true;
            this.blueLable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blueLable.Location = new System.Drawing.Point(135, 0);
            this.blueLable.Name = "blueLable";
            this.blueLable.Size = new System.Drawing.Size(62, 29);
            this.blueLable.TabIndex = 2;
            this.blueLable.Text = "B";
            this.blueLable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(68, 64);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(61, 33);
            this.button1.TabIndex = 6;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // changeColorToolStripMenuItem
            // 
            this.changeColorToolStripMenuItem.Name = "changeColorToolStripMenuItem";
            this.changeColorToolStripMenuItem.Size = new System.Drawing.Size(166, 24);
            this.changeColorToolStripMenuItem.Text = "Change color";
            this.changeColorToolStripMenuItem.Click += new System.EventHandler(this.changeColorToolStripMenuItem_Click);
            // 
            // redInput
            // 
            this.redInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.redInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.redInput.Location = new System.Drawing.Point(3, 32);
            this.redInput.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.redInput.Name = "redInput";
            this.redInput.Size = new System.Drawing.Size(59, 27);
            this.redInput.TabIndex = 7;
            // 
            // greenInput
            // 
            this.greenInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.greenInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.greenInput.Location = new System.Drawing.Point(68, 32);
            this.greenInput.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.greenInput.Name = "greenInput";
            this.greenInput.Size = new System.Drawing.Size(61, 27);
            this.greenInput.TabIndex = 8;
            // 
            // blueInput
            // 
            this.blueInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blueInput.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.blueInput.Location = new System.Drawing.Point(135, 32);
            this.blueInput.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.blueInput.Name = "blueInput";
            this.blueInput.Size = new System.Drawing.Size(62, 27);
            this.blueInput.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.colorPickPanel);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.openGLControl1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.colorPickPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.redInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueInput)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private SharpGL.OpenGLControl openGLControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button drawButton;
        private System.Windows.Forms.Button dragButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Panel colorPickPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label redLable;
        private System.Windows.Forms.Label greenLable;
        private System.Windows.Forms.Label blueLable;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripMenuItem changeColorToolStripMenuItem;
        private System.Windows.Forms.NumericUpDown redInput;
        private System.Windows.Forms.NumericUpDown blueInput;
        private System.Windows.Forms.NumericUpDown greenInput;
    }
}