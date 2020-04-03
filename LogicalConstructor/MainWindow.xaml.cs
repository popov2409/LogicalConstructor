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

        #region Переменные для определения порядка связей и элементов для отображения

        private int _connectionZIndex = 0;

        private int _elementZIndex = 10000;

        #endregion

        private List<Connection> _connections;
        public MainWindow()
        {
            InitializeComponent();
            _saver=new SaverClass();
            _connections=new List<Connection>();
        }

        /// <summary>
        /// Получение связи между двумя элементами для прорисовки линий
        /// </summary>
        /// <param name="inControl"></param>
        /// <param name="outControl"></param>
        /// <returns></returns>
        public Connection GetConnectionByTwoControls(ElementControl inControl, ElementControl outControl)
        {

            Point p1 = new Point(Canvas.GetLeft(inControl) + inControl.Width / 2,
                Canvas.GetTop(inControl) + inControl.Height / 2);
            Point p4 = new Point(Canvas.GetLeft(outControl) + outControl.Width / 2,
                Canvas.GetTop(outControl) + outControl.Height / 2);
            //if (p1.X > p4.X)
            //{
            //    Point pp = p1;
            //    p1 = p4;
            //    p4 = pp;
            //}

            Point p2=new Point((p4.X-p1.X)/2+p1.X, (p1.Y));
            Point p3 = new Point((p4.X - p1.X) / 2+p1.X, (p4.Y));

            PointCollection pointCollection = new PointCollection(){p1, p2,p3,p4};
            Polyline polyline = new Polyline()
            {
                Stroke = Brushes.Black, StrokeThickness = 2, Points = pointCollection,
                HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top
            };
            Panel.SetZIndex(polyline,_connectionZIndex);
            _connectionZIndex++;
            Connection connection=new Connection()
            {
                In = inControl.Element.Id,
                Out = outControl.Element.Id,
                Line = polyline
            };
            return connection;
        }

        /// <summary>
        /// Получить контрол по id элемента
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ElementControl GetElementById(Guid id)
        {
            foreach (UIElement child in EditorCanvas.Children)
            {
                if (child is ElementControl control)
                {
                    if (control.Element.Id == id) return control;

                }
            }
            return null;
        }

        private void AddElementMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            EditorCanvas.Children.Add(AddNewElementControl());
        }

        /// <summary>
        /// Создание нового элемента
        /// </summary>
        /// <returns></returns>
        ElementControl AddNewElementControl()
        {
            ElementControl el = new ElementControl();
            Panel.SetZIndex(el,_elementZIndex);
            _elementZIndex++;
            ElementClass element = new ElementClass() { Type = 2, InCount = 1 };
            el.Element = element;
            el.UpdateView();
            el.PreviewMouseMove += El_PreviewMouseMove;
            el.SetLocation(_mousePoint);
            MenuItem conntectionMenuItem = new MenuItem() { Header = "_Соединить элемент" };
            conntectionMenuItem.Click += ConntectionMenuItem_Click;
            el.MainGrid.ContextMenu?.Items.Insert(2, conntectionMenuItem);
            _saver.Elements.Add(element);
            return el;
        }

        private void El_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!((ElementControl) sender).IsSelected) return;
            if(!((ElementControl)sender).IsDrag) return;
            ((ElementControl) sender).SetLocation(e.GetPosition(EditorCanvas));
        }

        /// <summary>
        /// Снять выделения со всех элементов
        /// </summary>
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
                _saver.LoadData(ofd.FileName);
                foreach (ElementClass element in _saver.Elements)
                {
                    ElementControl el = new ElementControl { Element = element };
                    Panel.SetZIndex(el, _elementZIndex);
                    _elementZIndex++;
                    el.UpdateView();
                    el.PreviewMouseMove += El_PreviewMouseMove;
                    el.SetLocation(element.Location);
                    MenuItem conntectionMenuItem = new MenuItem() { Header = "_Соединить элемент" };
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

        /// <summary>
        /// Удаление элемента
        /// </summary>
        /// <param name="el"></param>
        void RemoveElement(ElementControl el)
        {
            _saver.Elements.Remove(_saver.Elements.First(c => c.Id == el.Element.Id));
            EditorCanvas.Children.Remove(el);
        }


        /// <summary>
        /// Получниение выделенного элемента (через костыли)
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Связывание элементов
        /// </summary>
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
            EditorCanvas.Children.Add(GetConnectionByTwoControls(GetElementById(_idSource), el).Line);

            //MessageBox.Show("Связь добавлена!");
            _connectionMode = false;
        }

        private void MainWindow_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_connectionMode) return;
            SetConnection();
        }
    }
}
