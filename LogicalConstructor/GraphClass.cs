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


        public static List<ElementControl> Elements=new List<ElementControl>();
        public static List<Connection> Connections = new List<Connection>();
        public static List<InOutControl> InOutControls=new List<InOutControl>();

        /// <summary>
        /// Получить координату конечной точки связи в зависимости от колличества входов и их занятости
        /// </summary>
        /// <param name="element"></param>
        /// <param name="numConnection"></param>
        /// <returns></returns>
        static Point GetFinishPoint(ElementClass element, int numConnection)
        {
            ElementControl control=new ElementControl();
            var dCon = control.Height / element.InCount / 2;

            Point point = new Point(element.Location.X + Math.Truncate(control.Width / 2),
                element.Location.Y + dCon);

            if (numConnection > 0)
            {
                if (numConnection > 1) point.Y += dCon * numConnection;
            }
            else
            {
                for (int i = 1; i < element.InElements.Count; i++)
                {
                    point.Y += dCon * 2;
                }
            }


            return point;
        }

        /// <summary>
        /// Снять выделения со всех элементов
        /// </summary>
        public static void ClearAllSelection()
        {
            foreach (ElementControl elementControl in Elements)
            {
                elementControl.UnSelected();
            }

            foreach (Connection connection in Connections)
            {
                connection.UnSelected();
            }

            foreach (InOutControl inOutControl in InOutControls)
            {
                inOutControl.UnSelected();
            }

        }


        /// <summary>
        /// Получниение Id выделенного элемента
        /// </summary>
        /// <returns></returns>
        public static string GetIdElementSource()
        {
            ElementControl el= Elements.FirstOrDefault(c => c.IsSelected);
            InOutControl inControl = InOutControls.FirstOrDefault(c => c.IsSelected);

            return el != null ? el.Element.Id.ToString() : (inControl != null ? inControl.Element.Id.ToString() : null);
        }

        /// <summary>
        /// Создание нового элемента
        /// </summary>
        /// <returns></returns>
        public static ElementControl CreateElementControl(ElementClass element,RoutedEventHandler menuHandler)
        {
            ElementControl el = new ElementControl();
            Panel.SetZIndex(el, ElementZIndex++);
            el.Element = element;
            el.UpdateView();
            el.SetLocation(element.Location);
            MenuItem connectionMenuItem = new MenuItem() { Header = "_Соединить элемент" };
            connectionMenuItem.Click += menuHandler;
            el.MainGrid.ContextMenu?.Items.Insert(2, connectionMenuItem);
            Elements.Add(el);
            return el;
        }

        public static InOutControl CreateInControl(ElementClass element)
        {
            InOutControl control = new InOutControl {NameLabel = {Text = element.Name}, Element = element};
            Panel.SetZIndex(control, ElementZIndex++);
            control.SetLocation(element.Location);
            InOutControls.Add(control);
            return control;
        }

        public static InOutControl CreateOutControl(ElementClass element)
        {
            InOutControl control = new InOutControl { NameLabel = { Text = element.Name }, Element = element };
            Panel.SetZIndex(control, ElementZIndex++);
            control.SetLocation(element.Location);
            InOutControls.Add(control);
            return control;
        }

        public static void RemoveAll()
        {
            ElementZIndex = 10000;
            ConnectionZIndex = 0;
            Elements.Clear();
            SaverClass.Elements.Clear();
            Connections.Clear();
            InOutControls.Clear();

        }

        /// <summary>
        /// Удаление элемента
        /// </summary>
        /// <param name="idElement"></param>
        public static void RemoveElement(Guid idElement)
        {
            SaverClass.Elements.Remove(SaverClass.Elements.First(c => c.Id == idElement));
            Elements.Remove(GraphClass.Elements.First(c => c.Element.Id == idElement));

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
