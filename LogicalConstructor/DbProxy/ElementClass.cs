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
    public class ElementClass
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
        

        public ElementClass()
        {
            Id=Guid.NewGuid();
            InElements=new List<Guid>();
            Type = 2;
            InCount = 1;

        }

    }

    public class Connection
    {
        public Guid Start { get; set; }
        public Guid End { get; set; }
        //public Point StartPoint { get; set; }
        //public Point EndPoint { get; set; }
        public Polyline Line { get; set; }

        public Connection()
        {
            Line=new Polyline()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            Line.PreviewMouseDown += Line_PreviewMouseDown;
        }

        private void Line_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Selected();
        }

        public void Selected()
        {
            Line.Stroke = Brushes.Blue;
        }

        public void Unselected()
        {
            Line.Stroke = Brushes.Black;
        }
    }
}
