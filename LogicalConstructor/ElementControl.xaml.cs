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

namespace LogicalConstructor
{
    /// <summary>
    /// Логика взаимодействия для ElementControl.xaml
    /// </summary>
    public partial class ElementControl : UserControl
    {
        public bool IsSelected;
        public Point MousePoint;
        public ElementControl()
        {
            InitializeComponent();
            IsSelected = false;
        }

        public void Selected(Brush brushes)
        {
            Rectangle.Stroke = brushes;
            Ellipse.Stroke = brushes;
            ElementName.Foreground = brushes;
        }

        public void Unselected()
        {
            Selected(Brushes.Black);
        }

        private void ElementControl_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsSelected = true;
            Selected(Brushes.Blue);
            this.CaptureMouse();
            MousePoint = e.GetPosition(this);
            //MessageBox.Show($"{MousePoint.X}   {MousePoint.Y}");
        }

        private void ElementControl_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

            IsSelected = false;
            Selected(Brushes.Black);
            this.ReleaseMouseCapture();
        }

        private void ElementControl_OnMouseLeave(object sender, MouseEventArgs e)
        {
            //IsSelected = false;
            //Selected(Brushes.Black);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Ellipse.Visibility = Visibility.Visible;
        }

       private void AndItem_OnChecked(object sender, RoutedEventArgs e)
        {
            ElementName.Content = "&";
            Ellipse.Visibility = Visibility.Hidden;
        }

        private void OrItem_OnChecked(object sender, RoutedEventArgs e)
        {
            if (ElementName==null) return;
            ElementName.Content = "1";
            Ellipse.Visibility = Visibility.Hidden;
        }

        private void AndNotItem_OnChecked(object sender, RoutedEventArgs e)
        {
            ElementName.Content = "&";
            Ellipse.Visibility = Visibility.Visible;
        }

        private void OrNotItem_OnChecked(object sender, RoutedEventArgs e)
        {
            ElementName.Content = "1";
            Ellipse.Visibility = Visibility.Visible;
        }

        public void SetLocation(Point point)
        {
            Point resPoint = point - (Vector)MousePoint;
            Canvas.SetLeft(this,resPoint.X);
            Canvas.SetTop(this,resPoint.Y);


        }
    }
}
