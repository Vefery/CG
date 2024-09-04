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
            Draw,
            Drag
        }
        struct Point
        {
            public short x, y;

            public static Point operator +(Point a, Point b)
            {
                Point t;
                t.x = (short)(a.x + b.x);
                t.y = (short)(a.y + b.y);
                return t;
            }
        }
        struct Triangle
        {
            public Point point1, point2, point3;
            public Color color;
        }

        OpenGL gl;
        Mode mode = Mode.Draw;
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
                gl.Color(tri.color.R, tri.color.G, tri.color.B);
                gl.Begin(OpenGL.GL_TRIANGLES);
                gl.Vertex(tri.point1.x, tri.point1.y);
                gl.Vertex(tri.point2.x, tri.point2.y);
                gl.Vertex(tri.point3.x, tri.point3.y);
                gl.End();

            }
            gl.Finish();
        }

        private void openGLControl1_Load(object sender, EventArgs e)
        {
            gl = openGLControl1.OpenGL;

            contextMenuStrip1.Items.Add("Change Color");
        }

        private void openGLControl1_MouseDown(object sender, MouseEventArgs e)
        {
            // Если нажата ЛКМ
            if (e.Button == MouseButtons.Left)
            {
                if (mode == Mode.Draw)
                {
                    // Создаем точку с координатами клика и добавляем в буфер
                    Point clickPoint;
                    clickPoint.x = (short)e.Location.X;
                    clickPoint.y = (short)e.Location.Y;
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
                        newTri.color = Color.Red;

                        tris.Add(newTri);
                    }
                } 
                else if (mode == Mode.Drag)
                {
                    Point p;
                    p.x = (short)e.X;
                    p.y = (short)e.Y;
 
                    for (int i = 0; i < tris.Count && selectedTriangle == -1; i++)
                    {
                        if (isInsideTriangle(tris[i], p))
                        {
                            selectedTriangle = i;
                            prevMouseLocation.x = (short)e.X;
                            prevMouseLocation.y = (short)e.Y;
                        }
                    }
                }
            } else if (e.Button == MouseButtons.Right)
            {
                Point p;
                p.x = (short)e.X;
                p.y = (short)e.Y;

                for (int i = 0; i < tris.Count && selectedTriangle == -1; i++)
                {
                    if (isInsideTriangle(tris[i], p))
                    {
                        selectedTriangle = i;
                        prevMouseLocation.x = (short)e.X;
                        prevMouseLocation.y = (short)e.Y;

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
        }

        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedTriangle != -1 && e.Button == MouseButtons.Left && mode == Mode.Drag)
            {
                Point delta;
                delta.x = (short)(e.X - prevMouseLocation.x);
                delta.y = (short)(e.Y - prevMouseLocation.y);
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
            mode = Mode.Draw;
        }

        private void dragButton_Click(object sender, EventArgs e)
        {
            mode = Mode.Drag;
        }

        private void openGLControl1_MouseUp(object sender, MouseEventArgs e)
        {
            selectedTriangle = -1;
        }
    }
}
