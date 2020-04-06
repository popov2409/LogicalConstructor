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

        private List<Connection> _connections;
        public MainWindow()
        {
            InitializeComponent();
            _saver=new SaverClass();
            _connections=new List<Connection>();
        }


        void InitializeOutControl()
        {
            if (_saver.Elements.Count(c => c.Type == 11) > 0) return;
            ElementClass element = new ElementClass {Type = 11, InCount = 1};
            _saver.Elements.Add(element);
            InOutControl outControl = new InOutControl()
            {
                Element = element,
                NameLabel = {Text = "Y"}
                
            };
            outControl.SetLocation(new Point(EditorCanvas.ActualWidth - 40, EditorCanvas.ActualHeight / 2 - 50));

            EditorCanvas.Children.Add(outControl);
        }

        private void AddElementMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ElementClass element=new ElementClass(){Location = _mousePoint};
            ElementControl el = GraphClass.CreateElementControl(element, ConntectionMenuItem_Click);
            el.PreviewMouseMove += El_PreviewMouseMove;
            _saver.Elements.Add(element);
            EditorCanvas.Children.Add(el);
        }


        private void El_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ElementControl element=sender as ElementControl;
            if (sender as ElementControl == null || !element.IsSelected || !element.IsDrag) return;
            element.SetLocation(e.GetPosition(EditorCanvas));
            foreach (Connection connection in _connections
                .Where(c => c.Start.Id == element.Element.Id || c.Finish.Id == element.Element.Id).ToList())
            {
                connection.CalculatePoints();
            }

        }

        

        private Point _mousePoint;
        private void EditorCanvas_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            GraphClass.ClearAllSelection(EditorCanvas);
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
            OpenSchema();
        }


        /// <summary>
        /// Загрузка схемы
        /// </summary>
        void OpenSchema()
        {
            OpenFileDialog ofd = new OpenFileDialog() { Title = "Выберите файл", Filter = "json files (*.json)|*.json" };
            if (ofd.ShowDialog() == true)
            {
                EditorCanvas.Children.Clear();
                _connections.Clear();
                _saver.LoadData(ofd.FileName);
                GraphClass.ElementZIndex = 10000;
                GraphClass.ConnectionZIndex = 0;
                foreach (ElementClass element in _saver.Elements)
                {
                    ElementControl el = GraphClass.CreateElementControl(element, ConntectionMenuItem_Click);
                    el.PreviewMouseMove += El_PreviewMouseMove;
                    EditorCanvas.Children.Add(el);
                    foreach (Guid id in element.InElements)
                    {
                        Connection connection = new Connection()
                        {
                            Start = _saver.Elements.First(c => c.Id == id),
                            Finish = el.Element
                        };
                        connection.CalculatePoints();
                        Panel.SetZIndex(connection.Line, GraphClass.ConnectionZIndex++);
                        _connections.Add(connection);
                        EditorCanvas.Children.Add(connection.Line);
                    }
                }
            }

        }

        private bool _connectionMode = false;
        private Guid _idSource;
        private void ConntectionMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _connectionMode = true;
            _idSource = GraphClass.GetSelectedElement(EditorCanvas).Element.Id;
        }

        /// <summary>
        /// Удаление элемента
        /// </summary>
        /// <param name="el"></param>
        void RemoveElement(ElementControl el)
        {
            _saver.Elements.Remove(_saver.Elements.First(c => c.Id == el.Element.Id));
            EditorCanvas.Children.Remove(el);
        }

        private void EditorCanvas_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                ElementControl el = GraphClass.GetSelectedElement(EditorCanvas);

                if (el == null) return;
                if (MessageBox.Show("Вы действительно хотите удалить элемент?", "", MessageBoxButton.YesNo) !=
                    MessageBoxResult.Yes) return;
                RemoveElement(el);
            }
        }

        /// <summary>
        /// Связывание элементов
        /// </summary>
        void SetConnection()
        {
            ElementControl el = GraphClass.GetSelectedElement(EditorCanvas);
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

            _saver.Elements.First(c => c.Id == el.Element.Id).InElements.Add(_idSource);
            Connection connection = new Connection()
            {
                Start = _saver.Elements.First(c => c.Id == _idSource),
                Finish = el.Element
            };
            Panel.SetZIndex(connection.Line, GraphClass.ConnectionZIndex++);
            connection.CalculatePoints();
            EditorCanvas.Children.Add(connection.Line);
            _connections.Add(connection);
            _connectionMode = false;
        }

        private void MainWindow_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_connectionMode) return;
            SetConnection();
        }

        private void AddInOutMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            InOutControl control = new InOutControl
            {
                NameLabel = {Text = $"X{_saver.Elements.Count(c => c.Type == 10)}"}
            };
            ElementClass element = new ElementClass() {InCount = 0, Type = 10};
            control.Element = element;
            _saver.Elements.Add(element);
            EditorCanvas.Children.Add(control);
            UpdateViewInOut();
            InitializeOutControl();

        }

        void UpdateViewInOut()
        {
            var dEl = (EditorCanvas.ActualHeight-50) / _saver.Elements.Count(c => c.Type == 10)/2;
            Point startPoint = new Point(20, dEl);

            foreach (InOutControl inControl in EditorCanvas.Children.OfType<InOutControl>().Where(c=>c.Element.Type==10))
            {
                inControl.SetLocation(startPoint);
                startPoint.Y += 2 * dEl;
            }
        }
    }
}
