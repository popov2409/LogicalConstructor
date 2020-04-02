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

        private int _connectionZIndex = 0;

        private int _elementZIndex = 10000;

        private List<Connection> _connections;
        public MainWindow()
        {
            InitializeComponent();
            _saver=new SaverClass();
            _connections=new List<Connection>();
        }

        public void AddConnection(ElementControl inControl, ElementControl outControl)
        {
            Point p1 = new Point(Canvas.GetLeft(inControl) + inControl.Width / 2,
                Canvas.GetTop(inControl) + inControl.Height / 2);
            Point p2 = new Point(Canvas.GetLeft(outControl) + outControl.Width / 2,
                Canvas.GetTop(outControl) + outControl.Height / 2);
            Polyline polyline=new Polyline(){Stroke = Brushes.Black};
            Connection connection=new Connection(){};
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
            if(!((ElementControl)sender).IsDrag) return;
            ((ElementControl) sender).SetLocation(e.GetPosition(EditorCanvas));
        }

        void ClearAllSelection()
        {
            foreach (UIElement child in EditorCanvas.Children)
            {
                if (child is ElementControl control)
                {
                    control.Unselected();
                }
            }
        }

        private Point _mousePoint;
        private void EditorCanvas_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearAllSelection();
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
                _ = _saver.SaveData(ofd.FileName);
            }
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
                    ElementControl el = new ElementControl {Element = element};
                    Canvas.SetZIndex(el,_elementZIndex);
                    _elementZIndex++;
                    el.UpdateView();
                    el.PreviewMouseMove += El_PreviewMouseMove;
                    el.SetLocation(element.Location);
                    MenuItem conntectionMenuItem =new MenuItem() { Header = "_Соединить элемент" };
                    conntectionMenuItem.Click += ConntectionMenuItem_Click;
                    el.MainGrid.ContextMenu?.Items.Insert(2, conntectionMenuItem);
                    EditorCanvas.Children.Add(el);
                }
            }
        }

        private bool _connectionMode = false;
        private Guid _idSource;
        private void ConntectionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _connectionMode = true;
            _idSource = GetSelectedElement().Element.Id;
        }

        void RemoveElement(ElementControl el)
        {
            _saver.Elements.Remove(_saver.Elements.First(c => c.Id == el.Element.Id));
            EditorCanvas.Children.Remove(el);
        }



        ElementControl GetSelectedElement()
        {
            foreach (UIElement child in EditorCanvas.Children)
            {
                if (child is ElementControl control)
                {
                    if (control.IsSelected)
                    {
                        return control ;
                    }

                }
            }
            return null;
        }

        private void EditorCanvas_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                ElementControl el = GetSelectedElement();
                if (el == null) return;
                if (MessageBox.Show("Вы действительно хотите удалить элемент?", "", MessageBoxButton.YesNo) !=
                    MessageBoxResult.Yes) return;
                RemoveElement(el);
            }
        }

        void SetConnection()
        {
            ElementControl el = GetSelectedElement();
            if (el == null)
            {
                _connectionMode = false;
                return;
            }

            if (el.Element.InElements.Count >= el.Element.InCount)
            {
                MessageBox.Show("У данного элемента задействованы все входы!");
                _connectionMode = false;
                return;
            }

            if (el.Element.InElements.Contains(_idSource))
            {
                MessageBox.Show("Данные элементы уже связаны!");
                _connectionMode = false;
                return;
            }
            _saver.Elements.First(c=>c.Id==el.Element.Id).InElements.Add(_idSource);
            MessageBox.Show("Связь добавлена!");
            _connectionMode = false;
        }

        private void MainWindow_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_connectionMode) return;
            SetConnection();
        }
    }
}
