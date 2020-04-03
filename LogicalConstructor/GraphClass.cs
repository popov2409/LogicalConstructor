using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using LogicalConstructor.DbProxy;

namespace LogicalConstructor
{
    public static class GraphClass
    {
        /// <summary>
        /// Формирует коллекцию точек для связи между двумя элементами
        /// </summary>
        /// <returns></returns>
        public static PointCollection GetPointCollectionBetweenTwoElements(ElementClass inControl, ElementClass outControl)
        {
            ElementControl control=new ElementControl();
            Point p1 = new Point(inControl.Location.X + Math.Truncate(control.Width / 2),
                inControl.Location.Y + Math.Truncate(control.Height / 2));
            Point p6 = GetPointConnection(outControl);
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

            return new PointCollection() { p1, p2, p3, p4, p5, p6 };
        }

        /// <summary>
        /// Получить координату конечной точки связи в зависимости от колличества входов и их занятости
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        static Point GetPointConnection(ElementClass element)
        {
            ElementControl control=new ElementControl();
            var dCon = control.Height / element.InCount / 2;

            Point point = new Point(element.Location.X + Math.Truncate(control.Width / 2),
                element.Location.Y + dCon);

            for (int i = 1; i < element.InElements.Count; i++)
            {
                point.Y += dCon * 2;
            }

            return point;
        }

        /// <summary>
        /// Снять выделения со всех элементов
        /// </summary>
        public static void ClearAllSelection(Canvas canvas)
        {
            foreach (UIElement child in canvas.Children)
            {
                if (child is ElementControl control)
                {
                    control.Unselected();
                }
            }
        }

        /// <summary>
        /// Получить контрол по id элемента
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ElementClass GetElementById(Guid id,SaverClass saver)
        {
            return saver.Elements.First(c => c.Id == id);
        }

        /// <summary>
        /// Получниение выделенного элемента (через костыли)
        /// </summary>
        /// <returns></returns>
        public static ElementControl GetSelectedElement(Canvas canvas)
        {
            foreach (UIElement child in canvas.Children)
            {
                if (child is ElementControl control)
                {
                    if (control.IsSelected)
                    {
                        return control;
                    }

                }
            }
            return null;
        }

        /// <summary>
        /// Создание нового элемента
        /// </summary>
        /// <returns></returns>
        public static ElementControl CreateElementControl(ElementClass element,RoutedEventHandler menuHandler)
        {
            ElementControl el = new ElementControl();
            Panel.SetZIndex(el, GraphClass.ElementZIndex++);
            el.Element = element;
            el.UpdateView();
            el.SetLocation(element.Location);
            MenuItem conntectionMenuItem = new MenuItem() { Header = "_Соединить элемент" };
            conntectionMenuItem.Click += menuHandler;
            el.MainGrid.ContextMenu?.Items.Insert(2, conntectionMenuItem);
            return el;
        }

        /// <summary>
        /// Получение связи между двумя элементами для прорисовки линий
        /// </summary>
        /// <param name="inControl"></param>
        /// <param name="outControl"></param>
        /// <returns></returns>
        public static Connection GetConnectionByTwoControls(ElementClass inControl, ElementClass outControl)
        {


            Polyline polyline = new Polyline()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Points = GraphClass.GetPointCollectionBetweenTwoElements(inControl, outControl),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            Panel.SetZIndex(polyline, ConnectionZIndex++);
            Connection connection = new Connection()
            {
                In = inControl.Id,
                Out = outControl.Id,
                Line = polyline
            };
            return connection;
        }


        /// <summary>
        /// Порядок элемента на форме
        /// </summary>
        public static int ElementZIndex = 10000;
        /// <summary>
        /// Порядок связи на форме
        /// </summary>
        public static int ConnectionZIndex = 0;
    }
}
