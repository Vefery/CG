using SharpGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab1
{
    public partial class Form1 : Form
    {
        enum Mode
        {
            Drawing,
            Dragging
        }
        struct Point
        {
            public short x, y;
            public SimpleColor color;

            public static Point operator +(Point a, Point b)
            {
                Point t;
                t.x = (short)(a.x + b.x);
                t.y = (short)(a.y + b.y);
                t.color = a.color;
                return t;
            }
            public static Point operator *(Point a, int b)
            {
                Point t;
                t.x = (short)(a.x * b);
                t.y = (short)(a.y * b);
                t.color = a.color;
                return t;
            }
        }
        struct SimpleColor
        {
            public byte r, g, b;

            public SimpleColor(byte newR, byte newG, byte newB)
            {
                r = newR;
                g = newG;
                b = newB;
            }
        }
        struct Triangle
        {
            public Point point1, point2, point3;
            public bool hidden;
        }

        OpenGL gl;
        Mode mode = Mode.Drawing;
        List<Triangle> tris = new List<Triangle>();
        Point[] pointsBuffer = new Point[3];
        byte pointsInBuffer = 0;
        int selectedTriangle = -1;
        Point prevMouseLocation;

        public Form1()
        {
            InitializeComponent();
        }

        private void openGLControl1_OpenGLDraw(object sender, RenderEventArgs args)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);	// Clear The Screen And The Depth Buffer
            gl.LoadIdentity();
            foreach (Triangle tri in tris)
            {
                if (tri.hidden)
                    gl.Begin(OpenGL.GL_LINE_LOOP);
                else
                    gl.Begin(OpenGL.GL_TRIANGLES);
                gl.Color(tri.point1.color.r, tri.point1.color.g, tri.point1.color.b);
                gl.Vertex(tri.point1.x, tri.point1.y);
                gl.Color(tri.point2.color.r, tri.point2.color.g, tri.point2.color.b);
                gl.Vertex(tri.point2.x, tri.point2.y);
                gl.Color(tri.point3.color.r, tri.point3.color.g, tri.point3.color.b);
                gl.Vertex(tri.point3.x, tri.point3.y);
                gl.End();

            }
            for (int i = 0; i < pointsInBuffer; i++)
            {
                gl.PointSize(20);
                gl.Enable(OpenGL.GL_POINT_SMOOTH);
                gl.Begin(OpenGL.GL_POINTS);
                gl.Color(pointsBuffer[i].color.r, pointsBuffer[i].color.g, pointsBuffer[i].color.b);
                gl.Vertex(pointsBuffer[i].x, pointsBuffer[i].y);
                gl.End();
                gl.Disable(OpenGL.GL_POINT_SMOOTH);
            }
            gl.Finish();
        }

        private void openGLControl1_Load(object sender, EventArgs e)
        {
            gl = openGLControl1.OpenGL;

            gl.Enable(OpenGL.GL_MULTISAMPLE);
        }

        private void openGLControl1_MouseDown(object sender, MouseEventArgs e)
        {
            // Если нажата ЛКМ
            if (e.Button == MouseButtons.Left)
            {
                if (mode == Mode.Drawing)
                {
                    // Создаем точку с координатами клика и добавляем в буфер
                    Point clickPoint;
                    clickPoint.x = (short)e.Location.X;
                    clickPoint.y = (short)e.Location.Y;
                    clickPoint.color = new SimpleColor((byte)redInput.Value, (byte)greenInput.Value, (byte)blueInput.Value);
                    pointsBuffer[pointsInBuffer] = clickPoint;
                    pointsInBuffer++;

                    // Если буфер полный создаем треугольник и добавляем его
                    if (pointsInBuffer > 2)
                    {
                        pointsInBuffer = 0;
                        Triangle newTri;
                        newTri.point1 = pointsBuffer[0];
                        newTri.point2 = pointsBuffer[1];
                        newTri.point3 = pointsBuffer[2];
                        newTri.hidden = false;

                        tris.Add(newTri);
                    }
                } 
                else if (mode == Mode.Dragging)
                {
                    Point p;
                    p.x = (short)e.X;
                    p.y = (short)e.Y;
                    p.color = new SimpleColor();
                    selectedTriangle = -1;

                    for (int i = tris.Count - 1; i >= 0 && selectedTriangle == -1; i--)
                    {
                        if (isInsideTriangle(tris[i], p) && !tris[i].hidden)
                        {
                            selectedTriangle = i;
                            prevMouseLocation.x = (short)e.X;
                            prevMouseLocation.y = (short)e.Y;
                        }
                    }
                }
            } 
            else if (e.Button == MouseButtons.Right)
            {
                Point p;
                p.x = (short)e.X;
                p.y = (short)e.Y;
                p.color = new SimpleColor();
                selectedTriangle = -1;

                for (int i = tris.Count - 1; i >= 0 && selectedTriangle == -1; i--)
                {
                    if (isInsideTriangle(tris[i], p) && !tris[i].hidden)
                    {
                        selectedTriangle = i;

                        contextMenuStrip1.Show(openGLControl1, e.Location);
                    }
                }
            }
        }

        private bool isInsideTriangle(Triangle tri, Point point)
        {
            double det = ((tri.point2.y - tri.point3.y) * (tri.point1.x - tri.point3.x) + (tri.point3.x - tri.point2.x) * (tri.point1.y - tri.point3.y));
            double a = ((tri.point2.y - tri.point3.y) * (point.x - tri.point3.x) + (tri.point3.x - tri.point2.x) * (point.y - tri.point3.y)) / det;
            double b = ((tri.point3.y - tri.point1.y) * (point.x - tri.point3.x) + (tri.point1.x - tri.point3.x) * (point.y - tri.point3.y)) / det;
            double c = 1 - a - b;

            return (a >= 0 && b >= 0 && c >= 0);
        }

        private void openGLControl1_Resized(object sender, EventArgs e)
        {
            gl = openGLControl1.OpenGL;
            Control control = (Control)sender;
            gl.Viewport(0, 0, control.Width, control.Height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Ortho2D(0, control.Width, control.Height, 0);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
        }
        private void openGLControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z && e.Modifiers == Keys.Control)
            {
                if (tris.Count > 0)
                {
                    tris.RemoveAt(tris.Count - 1);
                }
            }
            else if (e.KeyCode == Keys.H && e.Modifiers == Keys.Alt)
            {
                Triangle temp;
                for (int i = 0; i < tris.Count; i++)
                {
                    temp = tris[i];
                    temp.hidden = false;
                    tris[i] = temp;
                }
            }
        }

        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedTriangle != -1 && e.Button == MouseButtons.Left && mode == Mode.Dragging)
            {
                Point delta;
                delta.x = (short)(e.X - prevMouseLocation.x);
                delta.y = (short)(e.Y - prevMouseLocation.y);
                delta.color = new SimpleColor();
                Triangle curTri = tris[selectedTriangle];
                curTri.point1 += delta;
                curTri.point2 += delta;
                curTri.point3 += delta;
                tris[selectedTriangle] = curTri;

                prevMouseLocation.x = (short)e.X;
                prevMouseLocation.y = (short)e.Y;
            }
        }

        private void drawButton_Click(object sender, EventArgs e)
        {
            mode = Mode.Drawing;
        }

        private void dragButton_Click(object sender, EventArgs e)
        {
            mode = Mode.Dragging;
        }

        private void openGLControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (mode == Mode.Dragging && e.Button == MouseButtons.Left)
                selectedTriangle = -1;
        }

        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Triangle curTri = tris[selectedTriangle];
            SimpleColor newColor = new SimpleColor((byte)redInput.Value, (byte)greenInput.Value, (byte)blueInput.Value);
            curTri.point1.color = newColor;
            curTri.point2.color = newColor;
            curTri.point3.color = newColor;
            tris[selectedTriangle] = curTri;
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Triangle curTri = tris[selectedTriangle];
            curTri.hidden = true;
            tris[selectedTriangle] = curTri;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tris.RemoveAt(selectedTriangle);
        }
    }
}
