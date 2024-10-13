using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Lab1
{
    public struct Point
    {
        public float x, y, z;
        public SimpleColor color;

        public Point(float newX, float newY, float newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
            color = new SimpleColor();
        }
        public Point(float newX, float newY, float newZ, SimpleColor newColor)
        {
            x = newX;
            y = newY;
            z = newZ;
            color = newColor;
        }
        public static Point operator +(Point a, Point b)
        {
            Point t;
            t.x = a.x + b.x;
            t.y = a.y + b.y;
            t.z = a.z + b.z;
            t.color = a.color;
            return t;
        }
    }
    // В c# и так есть Color, но он слишком комплексный
    public struct SimpleColor
    {
        public byte r, g, b;

        public SimpleColor(byte newR, byte newG, byte newB)
        {
            r = newR;
            g = newG;
            b = newB;
        }
        public SimpleColor(Color color)
        {
            r = color.R;
            g = color.G;
            b = color.B;
        }
    }
    public partial class Form1 : Form
    {
        OpenGL gl;
        List<Point> vertices = new List<Point>();
        List<Point> slice = new List<Point>(6);
        List<Point> trajectory = new List<Point>();
        Point prevMouseLocation;

        public Form1()
        {
            InitializeComponent();

            using (StreamReader filein = new StreamReader("../../slice.txt"))
            {
                string line;
                while ((line = filein.ReadLine()) != null)
                {
                    string[] coords = line.Split(' ');
                    slice.Add(new Point(float.Parse(coords[0]), float.Parse(coords[1]), 0));
                }
            }
            using (StreamReader filein = new StreamReader("../../traj.txt"))
            {
                string line;
                while ((line = filein.ReadLine()) != null)
                {
                    string[] coords = line.Split(' ');
                    trajectory.Add(new Point(float.Parse(coords[0]), float.Parse(coords[1]), float.Parse(coords[2])));
                }
            }
        }
        // Update функция OGL, вызывается каждый кадр
        private void openGLControl1_OpenGLDraw(object sender, RenderEventArgs args)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);  // Очистка экрана и буфера глубины
            gl.Rotate(0, 1, 0);
            gl.Begin(OpenGL.GL_POLYGON);
            // Отрисовка первого сечения
            gl.Normal(0, 0, -1);
            foreach (Point p in slice)
            {
                gl.Vertex(p.x, p.y, p.z);
            }
            gl.End();
            gl.Begin(OpenGL.GL_POLYGON);
            gl.Normal(0, 0, 1);
            foreach (Point p in slice.Select(x => x + trajectory.Last()))
            {
                gl.Vertex(p.x, p.y, p.z);
            }
            gl.End();

            /*gl.Begin(OpenGL.GL_QUADS);
            foreach (Point p in vertices)
            {
                gl.Vertex(p.x, p.y, p.z);
            }
            gl.End();*/

            gl.Finish();
        }
        // Вызывается при создании объекта OGL
        private void openGLControl1_Load(object sender, EventArgs e)
        {
            gl = openGLControl1.OpenGL;

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Enable(OpenGL.GL_NORMALIZE);
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_TWO_SIDE, OpenGL.GL_TRUE);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, new float[] { 0, 0, -1, 0 });

            // Просчет точек и нормалей
            List<Point> slice_start;
            List<Point> slice_end;
            for (int i = 0; i < trajectory.Count - 1; i++)
            {
                slice_start = slice.Select(x => x + trajectory[i]).ToList();
                slice_end = slice.Select(x => x + trajectory[i + 1]).ToList();
                for (int j = 0; j < slice_start.Count - 1; j++)
                {
                    vertices.Add(slice_start[j]);
                    vertices.Add(slice_end[j]);
                    vertices.Add(slice_end[j + 1]);
                    vertices.Add(slice_start[j + 1]);
                }
                vertices.Add(slice_start[slice_start.Count - 1]);
                vertices.Add(slice_end[slice_start.Count - 1]);
                vertices.Add(slice_end[0]);
                vertices.Add(slice_start[0]);
            }
        }

        private void openGLControl1_Resized(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            int w = control.ClientSize.Width;
            int h = control.ClientSize.Height;
            double aspectRatio = (double)w / h;
            gl = openGLControl1.OpenGL; // Вызывается раньше "load", поэтому самостоятельно присваиваем
            gl.Viewport(0, 0, w, h);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(20, aspectRatio, 1, 5000);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            gl.LookAt(0, 0, -2000, 0, 0, 0, 0, 1, 0);
        }
    }
}