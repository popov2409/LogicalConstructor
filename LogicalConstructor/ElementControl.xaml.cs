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
        }

        private void ElementControl_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

            IsSelected = false;
            Selected(Brushes.Black);
        }

        private void ElementControl_OnMouseLeave(object sender, MouseEventArgs e)
        {
            //IsSelected = false;
            //Selected(Brushes.Black);
        }

    }
}
