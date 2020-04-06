using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LogicalConstructor.DbProxy
{
    public class ElementClass : IDisposable
    {
        public Guid Id { get; set; }
        public Point Location { get; set; }
        /// <summary>
        /// Тип элемента 0-И, 1-ИЛИ, 2-НЕ, 3-И-НЕ, 4-ИЛИ-НЕ
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// С кем связан(входы)
        /// </summary>
        public List<Guid> InElements { get; set; }
        /// <summary>
        /// Колличество входов
        /// </summary>
        public int InCount { get; set; }
        
        public string Name { get; set; }

        public ElementClass()
        {
            Id=Guid.NewGuid();
            InElements=new List<Guid>();
            Type = 2;
            InCount = 1;

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class Connection
    {
        public ElementClass Start { get; set; }
        public ElementClass Finish { get; set; }
        public Polyline Line { get; set; }

        public Connection()
        {
            Line = new Polyline()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            Line.PreviewMouseDown += Line_PreviewMouseDown;
        }


        Point GetFinishPoint()
        {
            ElementControl control = new ElementControl();
            var dCon = control.Height / Finish.InCount / 2;
            int number = Finish.InElements.IndexOf(Start.Id)+1;

            Point point = new Point(Finish.Location.X + control.Width / 2,
                Finish.Location.Y + dCon);

            for (int i = 0; i < number-1; i++)
            {
                point.Y += dCon * 2;
            }
            //if (number > 1) point.Y += dCon * number;
            return point;
        }


        public void CalculatePoints()
        {
            ElementControl control = new ElementControl();
            Point p1 = new Point(Start.Location.X + Math.Truncate(control.Width / 2), Start.Location.Y + control.Height / 2);
            Point p6 = GetFinishPoint();
            Point p2 = new Point((p6.X - p1.X) / 2 + p1.X, (p1.Y));
            Point p3 = new Point((p6.X - p1.X) / 2 + p1.X, (p6.Y));
            Point p4 = p3;
            Point p5 = p3;


            if (p1.X >= p4.X + 10)
            {
                p2 = new Point(p1.X + 40, p1.Y);
                var dY = (p1.Y - p6.Y) / 2;
                p3 = new Point(p2.X, p1.Y - dY);
                p4 = new Point(p6.X - 40, p3.Y);
                p5 = new Point(p4.X, p6.Y);
            }

            Line.Points= new PointCollection() { p1, p2, p3, p4, p5, p6 };
        }


        private void Line_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Selected();
        }

        public void Selected()
        {
            Line.Stroke = Brushes.Blue;
        }

        public void UnSelected()
        {
            Line.Stroke = Brushes.Black;
        }
    }
}
