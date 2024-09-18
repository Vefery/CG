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
        // Режим работы
        enum Mode : byte
        {
            Drawing,
            Dragging
        }
        struct Point
        {
            public short x, y;
            public SimpleColor color;

            public Point(short newX, short newY)
            {
                x = newX;
                y = newY;
                color = new SimpleColor();
            }
            public Point(short newX, short newY, SimpleColor newColor)
            {
                x = newX;
                y = newY;
                color = newColor;
            }
            public static Point operator +(Point a, Point b)
            {
                Point t;
                t.x = (short)(a.x + b.x);
                t.y = (short)(a.y + b.y);
                t.color = a.color;
                return t;
            }
        }
        // В c# и так есть Color, но он слишком комплексный
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
            public Point pivot;
            public bool hidden;

            public Triangle(Point newPoint1, Point newPoint2, Point newPoint3)
            {
                point1 = newPoint1;
                point2 = newPoint2;
                point3 = newPoint3;
                hidden = false;
                pivot = new Point((short)((point1.x + point2.x + point3.x) / 3), (short)((point1.y + point2.y + point3.y) / 3));
            }
            public void UpdatePivotPosition()
            {
                pivot.x = (short)((point1.x + point2.x + point3.x) / 3);
                pivot.y = (short)((point1.y + point2.y + point3.y) / 3);
            }
        }

        OpenGL gl;
        Mode mode = Mode.Drawing;
        List<Triangle> tris = new List<Triangle>();
        Point[] pointsBuffer = new Point[3];
        Point prevMouseLocation;
        byte pointsInBuffer = 0;
        int selectedTriangle = -1;

        public Form1()
        {
            InitializeComponent();
        }
        // Update функция OGL, вызывается каждый кадр
        private void openGLControl1_OpenGLDraw(object sender, RenderEventArgs args)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);	// Очистка экрана и буфера глубины
            // Цикл отрисовки всех треугольников
            foreach (Triangle tri in tris)
            {
                if (tri.hidden)
                    gl.Begin(OpenGL.GL_LINE_LOOP); // Без заливки, просто линии
                else
                    gl.Begin(OpenGL.GL_TRIANGLES); // С заливкой
                gl.Color(tri.point1.color.r, tri.point1.color.g, tri.point1.color.b);
                gl.Vertex(tri.point1.x, tri.point1.y);
                gl.Color(tri.point2.color.r, tri.point2.color.g, tri.point2.color.b);
                gl.Vertex(tri.point2.x, tri.point2.y);
                gl.Color(tri.point3.color.r, tri.point3.color.g, tri.point3.color.b);
                gl.Vertex(tri.point3.x, tri.point3.y);
                gl.End();
            }
            // Отрисовка точек из буфера
            for (int i = 0; i < pointsInBuffer; i++)
            {
                gl.PointSize(100);
                gl.Enable(OpenGL.GL_POINT_SMOOTH);
                gl.Begin(OpenGL.GL_POINTS);
                gl.Color(pointsBuffer[i].color.r, pointsBuffer[i].color.g, pointsBuffer[i].color.b);
                gl.Vertex(pointsBuffer[i].x, pointsBuffer[i].y);
                gl.End();
                gl.Disable(OpenGL.GL_POINT_SMOOTH);
            }
            gl.Finish();
        }
        // Вызывается при создании объекта OGL
        private void openGLControl1_Load(object sender, EventArgs e)
        {
            gl = openGLControl1.OpenGL;
        }
        // Ивент нажания мыши
        private void openGLControl1_MouseDown(object sender, MouseEventArgs e)
        {
            // Если нажата ЛКМ
            if (e.Button == MouseButtons.Left)
            {
                if (mode == Mode.Drawing)
                {
                    // Создаем точку с координатами клика и добавляем в буфер
                    Point clickPoint = new Point((short)e.Location.X, (short)e.Location.Y, new SimpleColor((byte)redInput.Value, (byte)greenInput.Value, (byte)blueInput.Value));
                    pointsBuffer[pointsInBuffer] = clickPoint;
                    pointsInBuffer++;

                    // Если буфер полный создаем треугольник и добавляем его
                    if (pointsInBuffer > 2)
                    {
                        pointsInBuffer = 0;
                        Triangle newTri = new Triangle(pointsBuffer[0], pointsBuffer[1], pointsBuffer[2]);

                        tris.Add(newTri);
                    }
                } 
                else if (mode == Mode.Dragging)
                {
                    // Создаем точку в месте клика
                    Point p = new Point((short)e.X, (short)e.Y);
                    selectedTriangle = -1;
                    // Перебор всех треугольников, проверка попали ли внутрь какого-нибудь. Обход с конца, поэтому выбирается самый верхний при нескольких
                    selectedTriangle = CheckForTriangle(p);
                    // Если нашелся треугольник, запоминаем его и запоминаем координаты клика
                    if (selectedTriangle != -1)
                    {
                        prevMouseLocation.x = p.x;
                        prevMouseLocation.y = p.y;
                    }
                }
            } 
            else if (e.Button == MouseButtons.Right) // Если нажата ПКМ
            {
                Point p = new Point((short)e.X, (short)e.Y);
                selectedTriangle = -1;

                selectedTriangle = CheckForTriangle(p);
                // Если попали в треугольник, запоминаем его и открываем в месте клика контекстное меню
                if (selectedTriangle != -1)
                    contextMenuStrip1.Show(openGLControl1, e.Location);
            }
        }
        // Проверка попала ли точка внутрь треугольника, через барицентрические координаты
        private bool isInsideTriangle(Triangle tri, Point point)
        {
            double det = ((tri.point2.y - tri.point3.y) * (tri.point1.x - tri.point3.x) + (tri.point3.x - tri.point2.x) * (tri.point1.y - tri.point3.y));
            double a = ((tri.point2.y - tri.point3.y) * (point.x - tri.point3.x) + (tri.point3.x - tri.point2.x) * (point.y - tri.point3.y)) / det;
            double b = ((tri.point3.y - tri.point1.y) * (point.x - tri.point3.x) + (tri.point1.x - tri.point3.x) * (point.y - tri.point3.y)) / det;
            double c = 1 - a - b;

            return (a >= 0 && b >= 0 && c >= 0);
        }
        private int CheckForTriangle(Point mousePos)
        {
            int result = -1;
            for (int i = tris.Count - 1; i >= 0 && result == -1; i--)
            {
                if (isInsideTriangle(tris[i], mousePos) && !tris[i].hidden)
                {
                    // Если попали в треугольник, возвращаем его
                    result = i;
                }
            }
            return result;
        }
        private void openGLControl1_Resized(object sender, EventArgs e)
        {
            gl = openGLControl1.OpenGL; // Вызывается раньше "load", поэтому самостоятельно присваиваем
            Control control = (Control)sender;
            gl.Viewport(0, 0, control.Width, control.Height); // Задаем вьюпорт: координаты левого нижнего угла (0,0 по дефолту) и разрешение
            gl.MatrixMode(OpenGL.GL_PROJECTION); // Задаем с какой матрицей работать
            gl.LoadIdentity();
            gl.Ortho2D(0, control.Width, control.Height, 0); // Задаем ортографическую проекцию, теперь коодринаты совпадают с пикселями с центром в левом верхнем углу
            gl.MatrixMode(OpenGL.GL_MODELVIEW); // Возвращаю дефолтное значение
        }
        // Ивент нажатия кнопки
        private void openGLControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z && e.Modifiers == Keys.Control)
            {
                // Удаляю последний треугольник, если есть
                if (tris.Count > 0)
                {
                    tris.RemoveAt(tris.Count - 1);
                }
            }
            else if (e.KeyCode == Keys.H && e.Modifiers == Keys.Alt)
            {
                Triangle temp;
                // Прохожу по всем треугольникам и убираю скрытие
                for (int i = 0; i < tris.Count; i++)
                {
                    temp = tris[i];
                    temp.hidden = false;
                    tris[i] = temp;
                }
            }
        }
        // Ивент движения мыши по области OGL
        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedTriangle != -1 && e.Button == MouseButtons.Left && mode == Mode.Dragging)
            {
                // Дельта перемещения мыши
                Point delta = new Point((short)(e.X - prevMouseLocation.x), (short)(e.Y - prevMouseLocation.y));
                
                Triangle curTri = tris[selectedTriangle];
                // Просто добавляю дельту ко всем координатам
                curTri.point1 += delta;
                curTri.point2 += delta;
                curTri.point3 += delta;
                curTri.UpdatePivotPosition();
                // Проверка выхода за экран
                HandleOutOfBounds(ref curTri);

                tris[selectedTriangle] = curTri;

                prevMouseLocation.x = (short)e.X;
                prevMouseLocation.y = (short)e.Y;
            }
        }
        private void HandleOutOfBounds(ref Triangle tri)
        {
            if (tri.pivot.x < 0)
            {
                tri.point1.x -= tri.pivot.x;
                tri.point2.x -= tri.pivot.x;
                tri.point3.x -= tri.pivot.x;
            }
            else if (tri.pivot.x > this.Width)
            {
                tri.point1.x -= (short)(tri.pivot.x - this.Width);
                tri.point2.x -= (short)(tri.pivot.x - this.Width);
                tri.point3.x -= (short)(tri.pivot.x - this.Width);
            }
            if (tri.pivot.y < 0)
            {
                tri.point1.y -= tri.pivot.y;
                tri.point2.y -= tri.pivot.y;
                tri.point3.y -= tri.pivot.y;
            }
            else if (tri.pivot.y > this.Height)
            {
                tri.point1.y -= (short)(tri.pivot.y - this.Height);
                tri.point2.y -= (short)(tri.pivot.y - this.Height);
                tri.point3.y -= (short)(tri.pivot.y - this.Height);
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
        // Ивент кнопки смены цвета из контекстного меню
        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Triangle curTri = tris[selectedTriangle];
            SimpleColor newColor = new SimpleColor((byte)redInput.Value, (byte)greenInput.Value, (byte)blueInput.Value);
            curTri.point1.color = newColor;
            curTri.point2.color = newColor;
            curTri.point3.color = newColor;
            tris[selectedTriangle] = curTri;
        }
        // Ивент кнопки скрыть из контекстного меню
        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Triangle curTri = tris[selectedTriangle];
            curTri.hidden = true;
            tris[selectedTriangle] = curTri;
            selectedTriangle = -1;
        }
        // Ивент кнопки удалить из контекстного меню
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tris.RemoveAt(selectedTriangle);
        }
    }
}
