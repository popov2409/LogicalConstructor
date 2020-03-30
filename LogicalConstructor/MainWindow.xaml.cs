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

            EditorGrid.Children.Add(el);
        }

        private void El_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!(sender as ElementControl).IsSelected) return;

            Thickness thickness = (sender as ElementControl).Margin;
            thickness.Left = e.GetPosition(EditorGrid).X-5;
            thickness.Top = e.GetPosition(EditorGrid).Y-5;
            (sender as ElementControl).Margin = thickness;
        }
    }
}
