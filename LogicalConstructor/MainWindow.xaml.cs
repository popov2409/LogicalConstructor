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
using Microsoft.Win32;

namespace LogicalConstructor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SaverClass _saver;
        public MainWindow()
        {
            InitializeComponent();
            _saver=new SaverClass();
        }

        private void AddElementMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ElementControl el=new ElementControl();
            ElementClass element=new ElementClass(){Type = 2,InCount = 1};
            el.Element = element;
            el.UpdateView();
            el.PreviewMouseMove += El_PreviewMouseMove;
            el.SetLocation(_mousePoint);
            EditorCanvas.Children.Add(el);
            _saver.Elements.Add(element);
        }

        private void El_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!((ElementControl) sender).IsSelected) return;
            ((ElementControl) sender).SetLocation(e.GetPosition(EditorCanvas));
        }

        private Point _mousePoint;
        private void EditorCanvas_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _mousePoint = e.GetPosition(EditorCanvas);
        }

        private void ExitMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog() {Title = "Введите имя файла", Filter = "json files (*.json)|*.json" };
            if (ofd.ShowDialog() == true)
            {
                _saver.SaveData(ofd.FileName);
            }анение 
        }

        private void OpenItem_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Title = "Выберите файл", Filter = "json files (*.json)|*.json" };
            if (ofd.ShowDialog() == true)
            {
                EditorCanvas.Children.Clear();
                _saver.LoadData(ofd.FileName);
                foreach (ElementClass element in _saver.Elements)
                {
                    ElementControl el=new ElementControl();
                    el.Element = element;
                    el.UpdateView();
                    el.PreviewMouseMove += El_PreviewMouseMove;
                    el.SetLocation(element.Location);
                    EditorCanvas.Children.Add(el);
                }
            }
        }
    }
}
