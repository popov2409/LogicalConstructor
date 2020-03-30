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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddElementMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ElementControl el=new ElementControl();
            el.PreviewMouseMove += El_PreviewMouseMove;

            EditorCanvas.Children.Add(el);
        }

        private void El_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!(sender as ElementControl).IsSelected) return;
            (sender as ElementControl).SetLocation(e.GetPosition(EditorCanvas));
            //Canvas.SetLeft(sender as ElementControl, e.GetPosition(EditorCanvas).X - 10);
            //Canvas.SetTop(sender as ElementControl, e.GetPosition(EditorCanvas).Y - 10);
            //Canvas.SetLeft(sender as ElementControl, e.GetPosition(EditorCanvas).X - (sender as ElementControl).MousePoint.X);
            //Canvas.SetTop(sender as ElementControl, e.GetPosition(EditorCanvas).Y - (sender as ElementControl).MousePoint.Y);
        }
    }
}
