using SharpGL;
using SharpGL.SceneGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Lab1
{
    public struct Vertex
    {
        public Point location;
        public List<Point> normals { get; }
        public Vertex(Point location, Point normal)
        {
            this.location = location;
            normals = new List<Point>() { normal };
        }
        public Vertex(Point location)
        {
            this.location = location;
            normals = new List<Point>();
        }
        public void AddNormal(Point newNormal)
        {
            normals.Add(newNormal.Normalized());
        }
    }
    public struct Point
    {
        public float x, y, z;

        public Point(float newX, float newY, float newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        public float[] ToArray()
        {
            return new float[] { x, y, z };
        }
        private float Length()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }
        public Point Normalized()
        {
            float length = Length();
            return new Point(x / length, y / length, z / length);
        }
        public void Zero()
        {
            x = 0;
            y = 0;
            z = 0;
        }
        public static Point operator +(Point a, Point b)
        {
            Point t;
            t.x = a.x + b.x;
            t.y = a.y + b.y;
            t.z = a.z + b.z;
            return t;
        }
        public static Point operator -(Point a, Point b)
        {
            Point t;
            t.x = a.x - b.x;
            t.y = a.y - b.y;
            t.z = a.z - b.z;
            return t;
        }
        public static Point operator -(Point a)
        {
            Point t;
            t.x = -a.x;
            t.y = -a.y;
            t.z = -a.z;
            return t;
        }
        public static bool operator ==(Point a, Point b)
        {
            if ((a.x - b.x < 1e-7f) && (a.y - b.y < 1e-7f) && (a.z - b.z < 1e-7f))
                return true;
            else
                return false;
        }
        public static bool operator !=(Point a, Point b)
        {
            if ((a.x - b.x >= 1e-7f) && (a.y - b.y >= 1e-7f) && (a.z - b.z >= 1e-7f))
                return true;
            else
                return false;
        }
    }
    public partial class Form1 : Form
    {
        OpenGL gl;
        List<Vertex> vertices = new List<Vertex>();
        List<Point> slice = new List<Point>(6);
        List<Point> trajectory = new List<Point>();
        Point prevMouseLocation;
        Point cameraLocation = new Point(50, 50, 50);
        int sliceVertices;
        bool orbiting = false;
        bool smoothNormals = false;
        bool drawNormals = false;

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
                sliceVertices = slice.Count;
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
        private Point CrossProduct(Point a, Point b)
        {
            return new Point(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        }
        private Point ChooseNormal(Vertex a, Vertex b)
        {
            return a.normals.Intersect(b.normals).ToList()[0];
        }
        private void DrawGrid()
        {
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT, new float[] { 0f, 0f, 0f, 1f });
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, new float[] { 0f, 0f, 0f, 1f });
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_EMISSION, new float[] { 0.5f, 0.5f, 0.5f, 1f });
            for (float i = -100; i <= 100; i += 5)
            {
                gl.Begin(OpenGL.GL_LINES);
                // Ось Х
                gl.Vertex(-100, 0, i);
                gl.Vertex(100, 0, i);

                // Ось Z
                gl.Vertex(i, 0, -100);
                gl.Vertex(i, 0, 100);
                gl.End();
            }

            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT, new float[] { 0.2f, 0.2f, 0.2f, 1f });
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, new float[] { 0.8f, 0.8f, 0.8f, 1f });
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_EMISSION, new float[] { 0f, 0f, 0f, 1f });
        }

        private void DrawNormals()
        {
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT, new float[] { 0f, 0f, 0f, 1f });
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, new float[] { 0f, 0f, 0f, 1f });
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_EMISSION, new float[] { 1f, 0.5f, 0.5f, 1f });
            
            foreach (Vertex vertex in vertices)
            {
                if (smoothNormals)
                {
                    Point finalNormal = new Point();
                    foreach (Point normal in vertex.normals)
                        finalNormal += normal;

                    gl.Begin(OpenGL.GL_LINES);
                    gl.Vertex(vertex.location.ToArray());
                    gl.Vertex((vertex.location + finalNormal.Normalized()).ToArray());
                    gl.End();
                }
                else
                {
                    foreach (Point normal in vertex.normals)
                    {
                        gl.Begin(OpenGL.GL_LINES);
                        gl.Vertex(vertex.location.ToArray());
                        gl.Vertex((vertex.location + normal.Normalized()).ToArray());
                        gl.End();
                    }
                }
            }

            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_AMBIENT, new float[] { 0.2f, 0.2f, 0.2f, 1f });
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, new float[] { 0.8f, 0.8f, 0.8f, 1f });
            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_EMISSION, new float[] { 0f, 0f, 0f, 1f });
        }
        // Update функция OGL, вызывается каждый кадр
        private void openGLControl1_OpenGLDraw(object sender, RenderEventArgs args)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);  // Очистка экрана и буфера глубины
            //gl.Rotate(0, 1, 0);

            DrawGrid();
            if (drawNormals)
                DrawNormals();

            gl.Material(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_EMISSION, new float[] { 0f, 0f, 0f, 1f });
            // Отрисовка сечений
            gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL);
            gl.Begin(OpenGL.GL_POLYGON);
            if (smoothNormals)
            {
                Point smoothNormal = new Point();
                for (int i = 0; i < sliceVertices; i++)
                {
                    foreach (Point p in vertices[i].normals)
                        smoothNormal += p;
                    gl.Normal(smoothNormal.Normalized().ToArray());
                    gl.Vertex(vertices[i].location.ToArray());
                    smoothNormal.Zero();
                }
            }
            else
            {
                gl.Normal(vertices[0].normals[0].Normalized().ToArray());
                for (int i = 0; i < sliceVertices; i++)
                    gl.Vertex(vertices[i].location.ToArray());
            }
            gl.End();

            gl.Begin(OpenGL.GL_POLYGON);
            if (smoothNormals)
            {
                Point smoothNormal = new Point();
                for (int i = vertices.Count() - 1; i >= vertices.Count() - sliceVertices; i--)
                {
                    foreach (Point p in vertices[i].normals)
                        smoothNormal += p;
                    gl.Normal(smoothNormal.Normalized().ToArray());
                    gl.Vertex(vertices[i].location.ToArray());
                    smoothNormal.Zero();
                }
            }
            else
            {
                gl.Normal(vertices.Last().normals.Last().Normalized().ToArray());
                for (int i = vertices.Count() - 1; i >= vertices.Count() - sliceVertices; i--)
                    gl.Vertex(vertices[i].location.ToArray());
            }
            gl.End();

            // Отрисовка ребер
            gl.Begin(OpenGL.GL_QUADS);
            for (int i = 0; i < trajectory.Count() - 1; i++)
            {
                int offset = i * sliceVertices;
                Point normal;
                for (int j = offset; j < offset + sliceVertices - 1; j++)
                {
                    if (smoothNormals)
                    {
                        normal = new Point();
                        foreach (Point norm in vertices[j].normals)
                            normal += norm;
                        gl.Normal(normal.Normalized().ToArray());
                        gl.Vertex(vertices[j].location.ToArray());

                        normal.Zero();
                        foreach (Point norm in vertices[j + 1].normals)
                            normal += norm;
                        gl.Normal(normal.Normalized().ToArray());
                        gl.Vertex(vertices[j + 1].location.ToArray());

                        normal.Zero();
                        foreach (Point norm in vertices[j + 1 + sliceVertices].normals)
                            normal += norm;
                        gl.Normal(normal.Normalized().ToArray());
                        gl.Vertex(vertices[j + 1 + sliceVertices].location.ToArray());

                        normal.Zero();
                        foreach (Point norm in vertices[j + sliceVertices].normals)
                            normal += norm;
                        gl.Normal(normal.Normalized().ToArray());
                        gl.Vertex(vertices[j + sliceVertices].location.ToArray());
                    }
                    else
                    {
                        normal = ChooseNormal(vertices[j], vertices[j + 1 + sliceVertices]);
                        gl.Normal(normal.ToArray());
                        gl.Vertex(vertices[j].location.ToArray());
                        gl.Vertex(vertices[j + 1].location.ToArray());
                        gl.Vertex(vertices[j + 1 + sliceVertices].location.ToArray());
                        gl.Vertex(vertices[j + sliceVertices].location.ToArray());
                    }
                }
                if (smoothNormals)
                {
                    normal = new Point();
                    foreach (Point norm in vertices[offset + sliceVertices - 1].normals)
                        normal += norm;
                    gl.Normal(normal.Normalized().ToArray());
                    gl.Vertex(vertices[offset + sliceVertices - 1].location.ToArray());

                    normal.Zero();
                    foreach (Point norm in vertices[offset + 2 * sliceVertices - 1].normals)
                        normal += norm;
                    gl.Normal(normal.Normalized().ToArray());
                    gl.Vertex(vertices[offset + 2 * sliceVertices - 1].location.ToArray());

                    normal.Zero();
                    foreach (Point norm in vertices[offset + sliceVertices].normals)
                        normal += norm;
                    gl.Normal(normal.Normalized().ToArray());
                    gl.Vertex(vertices[offset + sliceVertices].location.ToArray());

                    normal.Zero();
                    foreach (Point norm in vertices[offset].normals)
                        normal += norm;
                    gl.Normal(normal.Normalized().ToArray());
                    gl.Vertex(vertices[offset].location.ToArray());
                }
                else
                {
                    normal = ChooseNormal(vertices[offset + sliceVertices - 1], vertices[offset + sliceVertices]);
                    gl.Normal((normal).ToArray());
                    gl.Vertex(vertices[offset + sliceVertices - 1].location.ToArray());
                    gl.Vertex(vertices[offset + 2 * sliceVertices - 1].location.ToArray());
                    gl.Vertex(vertices[offset + sliceVertices].location.ToArray());
                    gl.Vertex(vertices[offset].location.ToArray());
                }
            }
            gl.End();

            gl.Finish();
        }
        // Вызывается при создании объекта OGL
        private void openGLControl1_Load(object sender, EventArgs e)
        {
            gl = openGLControl1.OpenGL;

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Enable(OpenGL.GL_LIGHT1);
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_TWO_SIDE, OpenGL.GL_TRUE);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, new float[] { 0.5f, 0.5f, 0.5f, 0 });
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, new float[] { 1f, 1f, 1f, 0 });
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, new float[] { -1f, -1f, -1f, 0 });

            // Просчет точек и нормалей
            List<Point> slice_start = slice.Select(x => x + trajectory[0]).ToList();
            List<Point> slice_next;

            foreach (Point p in slice_start)
                vertices.Add(new Vertex(p));

            for (int i = 0; i < sliceVertices; i++)
                vertices[i].AddNormal(new Point(0, 0, -1));

            for (int i = 1; i < trajectory.Count; i++)
            {
                slice_next = slice.Select(x => x + trajectory[i]).ToList();

                foreach (Point p in slice_next)
                    vertices.Add(new Vertex(p));

                Point normal;
                int offset = i * sliceVertices;
                for (int j = offset; j < offset + sliceVertices - 1; j++)
                {
                    normal = CrossProduct(vertices[j + 1].location - vertices[j - sliceVertices].location, vertices[j].location - vertices[j - sliceVertices].location);
                    vertices[j].AddNormal(normal);
                    vertices[j + 1].AddNormal(normal);
                    vertices[j - sliceVertices].AddNormal(normal);
                    vertices[j - sliceVertices + 1].AddNormal(normal);
                }
                normal = CrossProduct(vertices[offset].location - vertices[offset - 1].location, vertices[offset + sliceVertices - 1].location - vertices[offset - 1].location);
                vertices[offset + sliceVertices - 1].AddNormal(normal);
                vertices[offset].AddNormal(normal);
                vertices[offset - 1].AddNormal(normal);
                vertices[offset - sliceVertices].AddNormal(normal);
            }

            for (int i = vertices.Count() - 1; i >= vertices.Count() - sliceVertices; i--)
                vertices[i].AddNormal(new Point(0, 0, 1));
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
            gl.Perspective(60, aspectRatio, 1, 5000);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            gl.LookAt(cameraLocation.x, cameraLocation.y, cameraLocation.z, 0, 0, 0, 0, 1, 0);
        }

        private void openGLControl1_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y, 0);
            prevMouseLocation = p;
            orbiting = true;
        }

        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (orbiting)
            {
                // Дельта перемещения мыши
                Point delta = new Point((e.X - prevMouseLocation.x), (e.Y - prevMouseLocation.y), 0);
                cameraLocation.x = (float)(cameraLocation.x * Math.Cos(-delta.x * Math.PI / 180f) + cameraLocation.z * Math.Sin(-delta.x * Math.PI / 180f));
                cameraLocation.z = (float)(-cameraLocation.x * Math.Sin(-delta.x * Math.PI / 180f) + cameraLocation.z * Math.Cos(-delta.x * Math.PI / 180f));
                Point forward = cameraLocation.Normalized();
                Point up = new Point(0, 1, 0);
                Point right = CrossProduct(forward, up).Normalized();

                gl.MatrixMode(OpenGL.GL_MODELVIEW);
                //gl.Rotate(-delta.y, right.x, right.y, right.z);
                gl.Rotate(delta.x, 0, 1, 0);

                prevMouseLocation.x = e.X;
                prevMouseLocation.y = e.Y;
            }
        }

        private void openGLControl1_MouseUp(object sender, MouseEventArgs e)
        {
            orbiting = false;
        }

        private void checkedListBox1_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.Index == 0)
            {
                if (e.NewValue == CheckState.Checked)
                    smoothNormals = true;
                else
                    smoothNormals = false;
            }
            if (e.Index == 1)
            {
                if (e.NewValue == CheckState.Checked)
                    drawNormals = true;
                else
                    drawNormals = false;
            }
        }
    }
}