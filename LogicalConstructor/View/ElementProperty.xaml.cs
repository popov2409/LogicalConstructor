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
using System.Windows.Shapes;
using LogicalConstructor.DbProxy;

namespace LogicalConstructor.View
{
    /// <summary>
    /// Логика взаимодействия для ElementProperty.xaml
    /// </summary>
    public partial class ElementProperty : Window
    {
        private ElementControl _element;
        public ElementProperty(ElementControl element)
        {
            InitializeComponent();
            _element = element;
            ElementTypeComboBox.SelectedIndex = _element.Element.Type;
            InCountTextBox.Text = _element.Element.InCount.ToString();
            this.Left = Canvas.GetLeft(element);
            this.Top = Canvas.GetTop(element);
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ElementTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InCountTextBox==null) return;
            InCountTextBox.Text = (ElementTypeComboBox.SelectedIndex == 2) ? "1" : InCountTextBox.Text;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            _element.Element.Type = ElementTypeComboBox.SelectedIndex;
            _element.Element.InCount = int.Parse(InCountTextBox.Text);
            this.Close();
        }
    }
}
