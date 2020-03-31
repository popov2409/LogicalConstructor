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
        public bool IsSelected;
        public Point MousePoint;
        public ElementClass Element;
        public bool IsDrag;

        public ElementControl()
        {
            InitializeComponent();
            IsSelected = false;
        }

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

        public void Selected()
        {
            IsSelected = true;
            SetColor(Brushes.Blue);
           // this.CaptureMouse();
        }

        public void Unselected()
        {
            IsSelected = false;
            SetColor(Brushes.Black);
           // this.ReleaseMouseCapture();
        }

        private void ElementControl_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Selected();
            MousePoint = e.GetPosition(this);
            IsDrag = true;
        }



        public void SetLocation(Point point)
        {
            Point resPoint = point - (Vector)MousePoint;
            Canvas.SetLeft(this,resPoint.X);
            Canvas.SetTop(this,resPoint.Y);
            Element.Location = resPoint;

        }

        private void PropertyItem_OnClick(object sender, RoutedEventArgs e)
        {
            new ElementProperty(this).ShowDialog();
            UpdateView();
        }

        public void UpdateView()
        {
            Ellipse.Visibility = Element.Type > 1 ? Visibility.Visible : Visibility.Hidden;
            ElementName.Content = (Element.Type == 0 || Element.Type == 3) ? "&" : "1";
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
