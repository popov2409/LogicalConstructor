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

namespace LogicalConstructor
{
    /// <summary>
    /// Логика взаимодействия для InOutControl.xaml
    /// </summary>
    public partial class InOutControl : UserControl
    {
        public bool IsSelected = false;
        public InOutControl()
        {
            InitializeComponent();
        }

        public ElementClass Element { get; set; }

        public void SetLocation(Point point)
        {
            Canvas.SetLeft(this, point.X);
            Canvas.SetTop(this,point.Y);
            if(Element==null) return;
            Element.Location = point;
        }

        void SetColor(Brush brush)
        {
            NameLabel.Foreground = brush;
            Ellipse.Stroke = brush;
        }

        public void Selected()
        {
            SetColor(Brushes.Blue);
            IsSelected = true;
        }
        public void UnSelected()
        {
            SetColor(Brushes.Black);
            IsSelected = false;
        }

        private void InOutControl_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Selected();
        }
    }
}
