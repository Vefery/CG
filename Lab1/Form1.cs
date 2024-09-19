using SharpGL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab1
{
    // Режим работы
    enum Mode : byte
    {
        Drawing,
        Dragging,
        Grouping
    }
    public struct Point
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
    public interface IRenderableObject
    {
        bool hidden { set; get; }
        void Draw(OpenGL gl, bool forceHidden);
        void UpdatePivotPosition();
        Point GetPivotPosition();
        void Translate(Point delta);
        bool CheckIfPointInside(Point p);
        void HandleOutOfBounds(Control control);
        void ChangeColor(SimpleColor newColor);
    }
    public partial class Form1 : Form
    {
        OpenGL gl;
        Mode mode = Mode.Drawing;
        List<IRenderableObject> tris = new List<IRenderableObject>();
        List<IRenderableObject> backupPointer;
        List<int> groupingBuffer = new List<int>();
        Point[] pointsBuffer = new Point[3];
        Point prevMouseLocation;
        byte pointsInBuffer = 0;
        int selectedObject = -1;
        TriangleGroup editedGroup;

        public Form1()
        {
            InitializeComponent();
        }
        // Update функция OGL, вызывается каждый кадр
        private void openGLControl1_OpenGLDraw(object sender, RenderEventArgs args)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);	// Очистка экрана и буфера глубины
            // Цикл отрисовки всех треугольников
            if (editedGroup is null)
            {
                foreach (IRenderableObject tri in tris)
                {
                    tri.Draw(gl, false);
                }
            } 
            else
            {
                foreach (IRenderableObject tri in backupPointer)
                {
                    if (tri != editedGroup)
                        tri.Draw(gl, true);
                }
                foreach (IRenderableObject tri in tris)
                {
                    tri.Draw(gl, false);
                }
            }
            
            // Отрисовка точек из буфера
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
                    Point clickPoint = new Point((short)e.Location.X, (short)e.Location.Y, new SimpleColor(colorButton.BackColor.R, colorButton.BackColor.G, colorButton.BackColor.B));
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
                    selectedObject = -1;
                    // Перебор всех треугольников, проверка попали ли внутрь какого-нибудь. Обход с конца, поэтому выбирается самый верхний при нескольких
                    selectedObject = CheckForObject(p);
                    // Если нашелся треугольник, запоминаем его и запоминаем координаты клика
                    if (selectedObject != -1)
                    {
                        prevMouseLocation.x = p.x;
                        prevMouseLocation.y = p.y;
                    }
                } 
                else if (mode == Mode.Grouping)
                {
                    // Создаем точку в месте клика
                    Point p = new Point((short)e.X, (short)e.Y);

                    selectedObject = CheckForObject(p);

                    if (selectedObject != -1 && !groupingBuffer.Contains(selectedObject))
                        groupingBuffer.Add(selectedObject);

                    selectedObject = -1;
                }
            } 
            else if (e.Button == MouseButtons.Right) // Если нажата ПКМ
            {
                Point p = new Point((short)e.X, (short)e.Y);
                selectedObject = -1;

                selectedObject = CheckForObject(p);
                // Если попали в треугольник, запоминаем его и открываем в месте клика контекстное меню
                if (selectedObject != -1)
                    contextMenuStrip1.Show(openGLControl1, e.Location);
            }
        }
        private int CheckForObject(Point mousePos)
        {
            int result = -1;
            for (int i = tris.Count - 1; i >= 0 && result == -1; i--)
            {
                if (tris[i].CheckIfPointInside(mousePos) && !tris[i].hidden)
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
            if (e.KeyCode == Keys.Back)
            {
                // Удаляю последний треугольник, если есть
                if (tris.Count > 0)
                {
                    tris.RemoveAt(tris.Count - 1);
                }
            }
            else if (e.KeyCode == Keys.H && e.Modifiers == Keys.Alt)
            {
                // Прохожу по всем треугольникам и убираю скрытие
                for (int i = 0; i < tris.Count; i++)
                    tris[i].hidden = false;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (groupingBuffer.Count > 1)
                {
                    TriangleGroup group = new TriangleGroup();

                    foreach (int num in groupingBuffer)
                        group.tris.Add(tris[num]);

                    foreach (int num in groupingBuffer)
                        tris[num] = null;
                    tris.RemoveAll(x => x == null);

                    groupingBuffer.Clear();
                    tris.Add(group);

                    TrianglesGroupUC newGroup = new TrianglesGroupUC((TriangleGroup)tris.Last(), $"Group {groupsContainer.Controls.Count}");
                    newGroup.onStartEdit += StartEditingGroup;
                    groupsContainer.Controls.Add(newGroup);
                }
                else
                {
                    // Error here
                    MessageBox.Show("At least 2 objects must be selected in order to make a group!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    groupingBuffer.Clear();
                }
            }
            else if (e.KeyCode == Keys.F)
            {
                if (editedGroup is null)
                    return;

                tris = backupPointer;
                editedGroup.isBeingEdited = false;
                editedGroup = null;

                foreach (TrianglesGroupUC groupPanel in groupsContainer.Controls)
                    groupPanel.SwitchButton(true);
            }
        }
        private void StartEditingGroup(TriangleGroup group)
        {
            foreach (TrianglesGroupUC groupPanel in groupsContainer.Controls)
                groupPanel.SwitchButton(false);

            backupPointer = tris;
            tris = group.tris;
            editedGroup = group;
            editedGroup.isBeingEdited = true;
            editedGroup.hidden = false;
        }
        // Ивент движения мыши по области OGL
        private void openGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedObject != -1 && e.Button == MouseButtons.Left && mode == Mode.Dragging)
            {
                // Дельта перемещения мыши
                Point delta = new Point((short)(e.X - prevMouseLocation.x), (short)(e.Y - prevMouseLocation.y));
                
                IRenderableObject curObj = tris[selectedObject];
                // Просто добавляю дельту ко всем координатам
                curObj.Translate(delta);
                curObj.UpdatePivotPosition();
                // Проверка выхода за экран
                curObj.HandleOutOfBounds(this);

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
                selectedObject = -1;
        }
        // Ивент кнопки смены цвета из контекстного меню
        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                tris[selectedObject].ChangeColor(new SimpleColor(colorDialog1.Color));
        }
        // Ивент кнопки скрыть из контекстного меню
        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tris[selectedObject].hidden = true;
            selectedObject = -1;
        }
        // Ивент кнопки удалить из контекстного меню
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tris.RemoveAt(selectedObject);
        }

        private void colorButton_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                colorButton.BackColor = colorDialog1.Color;
        }

        private void groupButton_Click(object sender, EventArgs e)
        {
            mode = Mode.Grouping;
        }
    }
    public class Triangle : IRenderableObject
    {
        public Point point1, point2, point3;
        public Point pivot;
        public bool hidden { get; set; }

        public Triangle(Point newPoint1, Point newPoint2, Point newPoint3)
        {
            point1 = newPoint1;
            point2 = newPoint2;
            point3 = newPoint3;
            hidden = false;
            pivot = new Point((short)((point1.x + point2.x + point3.x) / 3), (short)((point1.y + point2.y + point3.y) / 3));
        }

        public void Draw(OpenGL gl, bool forceHidden)
        {
            if (hidden || forceHidden)
                gl.Begin(OpenGL.GL_LINE_LOOP); // Без заливки, просто линии
            else
                gl.Begin(OpenGL.GL_TRIANGLES); // С заливкой
            gl.Color(point1.color.r, point1.color.g, point1.color.b);
            gl.Vertex(point1.x, point1.y);
            gl.Color(point2.color.r, point2.color.g, point2.color.b);
            gl.Vertex(point2.x, point2.y);
            gl.Color(point3.color.r, point3.color.g, point3.color.b);
            gl.Vertex(point3.x, point3.y);
            gl.End();
        }

        public void UpdatePivotPosition()
        {
            pivot.x = (short)((point1.x + point2.x + point3.x) / 3);
            pivot.y = (short)((point1.y + point2.y + point3.y) / 3);
        }

        public void Translate(Point delta)
        {
            point1 += delta;
            point2 += delta;
            point3 += delta;
        }

        public Point GetPivotPosition()
        {
            return pivot;
        }

        public bool CheckIfPointInside(Point p)
        {
            double det = ((point2.y - point3.y) * (point1.x - point3.x) + (point3.x - point2.x) * (point1.y - point3.y));
            double a = ((point2.y - point3.y) * (p.x - point3.x) + (point3.x - point2.x) * (p.y - point3.y)) / det;
            double b = ((point3.y - point1.y) * (p.x - point3.x) + (point1.x - point3.x) * (p.y - point3.y)) / det;
            double c = 1 - a - b;

            return (a >= 0 && b >= 0 && c >= 0);
        }

        public void HandleOutOfBounds(Control control)
        {
            if (pivot.x < 0)
            {
                point1.x -= pivot.x;
                point2.x -= pivot.x;
                point3.x -= pivot.x;
            }
            else if (pivot.x > control.Width)
            {
                point1.x -= (short)(pivot.x - control.Width);
                point2.x -= (short)(pivot.x - control.Width);
                point3.x -= (short)(pivot.x - control.Width);
            }
            if (pivot.y < 0)
            {
                point1.y -= pivot.y;
                point2.y -= pivot.y;
                point3.y -= pivot.y;
            }
            else if (pivot.y > control.Height)
            {
                point1.y -= (short)(pivot.y - control.Height);
                point2.y -= (short)(pivot.y - control.Height);
                point3.y -= (short)(pivot.y - control.Height);
            }
        }

        public void ChangeColor(SimpleColor newColor)
        {
            point1.color = newColor;
            point2.color = newColor;
            point3.color = newColor;
        }
    }

    public class TriangleGroup : IRenderableObject
    {
        public bool hidden { get; set; }
        public bool isBeingEdited = false;
        public List<IRenderableObject> tris = new List<IRenderableObject>();
        public Point pivot;

        public void ChangeColor(SimpleColor newColor)
        {
            foreach (IRenderableObject tri in tris)
            {
                tri.ChangeColor(newColor);
            }
        }

        public bool CheckIfPointInside(Point p)
        {
            bool hit = false;
            for (int i = tris.Count - 1; i >= 0 && !hit; i--)
            {
                hit = tris[i].CheckIfPointInside(p);
            }
            return hit;
        }

        public void Draw(OpenGL gl, bool forceHidden)
        {
            foreach (IRenderableObject tri in tris)
                tri.Draw(gl, forceHidden);
        }

        public Point GetPivotPosition()
        {
            return pivot;
        }

        public void HandleOutOfBounds(Control control)
        {
            if (pivot.x < 0)
            {
                foreach(Triangle tri in tris)
                {
                    tri.point1.x -= pivot.x;
                    tri.point2.x -= pivot.x;
                    tri.point3.x -= pivot.x;
                }
                
            }
            else if (pivot.x > control.Width)
            {
                foreach(Triangle tri in tris)
                {
                    tri.point1.x -= (short)(pivot.x - control.Width);
                    tri.point2.x -= (short)(pivot.x - control.Width);
                    tri.point3.x -= (short)(pivot.x - control.Width);
                }
            }
            if (pivot.y < 0)
            {
                foreach(Triangle tri in tris)
                {
                    tri.point1.y -= pivot.y;
                    tri.point2.y -= pivot.y;
                    tri.point3.y -= pivot.y;
                }
            }
            else if (pivot.y > control.Height)
            {
                foreach(Triangle tri in tris)
                {
                    tri.point1.y -= (short)(pivot.y - control.Height);
                    tri.point2.y -= (short)(pivot.y - control.Height);
                    tri.point3.y -= (short)(pivot.y - control.Height);
                }
            }
        }

        public void Translate(Point delta)
        {
            foreach (IRenderableObject tri in tris)
            {
                tri.Translate(delta);
            }
        }

        public void UpdatePivotPosition()
        {
            foreach (IRenderableObject tri in tris)
                tri.UpdatePivotPosition();

            int xSum = tris.Sum(x => x.GetPivotPosition().x);
            int ySum = tris.Sum(y => y.GetPivotPosition().y);

            pivot.x = (short)(xSum / tris.Count);
            pivot.y = (short)(ySum / tris.Count);
        }
    }
}
