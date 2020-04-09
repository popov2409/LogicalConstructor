using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LogicalConstructor.DbProxy;
using LogicalConstructor.View;

namespace LogicalConstructor
{
    /// <summary>
    /// Логика взаимодействия для ElementControl.xaml
    /// </summary>
    public partial class ElementControl : UserControl
    {
        /// <summary>
        /// Выделен ли контрол
        /// </summary>
        public bool IsSelected=false;
        
        /// <summary>
        /// Положение мыши относительно контрола(необходимо для правильного позиционирования при перетаскивании элемента)
        /// </summary>
        private Point _mousePoint;
        public ElementClass Element;
        /// <summary>
        /// Возможность перетаскивания
        /// </summary>
        public bool IsDrag;

        private Dictionary<int, string> _names = new Dictionary<int, string>
        {
            {0, "И"},
            {1, "ИЛИ"},
            {2, "НЕ"},
            {3, "И-НЕ"},
            {4, "ИЛИ-НЕ"}
        };
    public ElementControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Установить цвет линий всего содержимого
        /// </summary>
        /// <param name="brushes"></param>
        public void SetColor(Brush brushes)
        {
            Rectangle.Stroke = brushes;
            Ellipse.Stroke = brushes;
            ElementName.Foreground = brushes;
            OutLine.Stroke = brushes;
            foreach (var child in InGrid.Children)
            {
                if (child is Line)
                {
                    (child as Line).Stroke = brushes;
                }

            }
        }

        /// <summary>
        /// Выделение элемента
        /// </summary>
        public void Selected()
        {
            IsSelected = true;
            SetColor(Brushes.Blue);
        }

        /// <summary>
        /// Снятие выделения
        /// </summary>
        public void UnSelected()
        {
            IsSelected = false;
            SetColor(Brushes.Black);
        }

        private void ElementControl_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Selected();
            _mousePoint = e.GetPosition(this);
            IsDrag = true;
        }


        /// <summary>
        /// Задание положения элемента при создании или перемещении
        /// </summary>
        /// <param name="point"></param>
        public void SetLocation(Point point)
        {
            Point resPoint = point - (Vector)_mousePoint;
            Canvas.SetLeft(this,resPoint.X);
            Canvas.SetTop(this,resPoint.Y);
            Element.Location = resPoint;

        }

        private void PropertyItem_OnClick(object sender, RoutedEventArgs e)
        {
            new ElementProperty(this).ShowDialog();
        }

        /// <summary>
        /// Обновление контрола при изменении свойств элемента
        /// </summary>
        public void UpdateView()
        {
            Ellipse.Visibility = Element.Type > 1 ? Visibility.Visible : Visibility.Hidden;
            ElementName.Text = (Element.Type == 0 || Element.Type == 3) ? "&" : "1";
            InGrid.Children.Clear();
            InGrid.RowDefinitions.Clear();
            for (int i = 0; i < Element.InCount; i++)
            {
                InGrid.RowDefinitions.Add(new RowDefinition());
                Line l = new Line()
                {
                    Stroke = Brushes.Black, StrokeThickness = 2, X1 = 0, X2 = 10, Y1 = 0, Y2 = 0,
                    VerticalAlignment = VerticalAlignment.Center
                };
                l.PreviewMouseDown += L_PreviewMouseDown;
                Grid.SetRow(l,i);
                InGrid.Children.Add(l);
            }
        }

        /// <summary>
        /// Выделение части стыковки элемента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void L_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((Line)sender).Stroke = Brushes.Crimson;
        }

        private void UserControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            IsDrag = false;
        }

        private void AddConnectorItem_OnClick(object sender, RoutedEventArgs e)
        {
            Element.InCount++;
            UpdateView();
        }

        private void DeleteConnectorItem_OnClick(object sender, RoutedEventArgs e)
        {
            Element.InCount--;
            UpdateView();
        }
        
    }
}
