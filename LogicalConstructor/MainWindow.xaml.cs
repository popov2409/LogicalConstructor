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
using LogicalConstructor.View;
using Microsoft.Win32;

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
            ElementClass element=new ElementClass{Location = _mousePoint};
            ElementControl el = GraphClass.CreateElementControl(element, ConntectionMenuItem_Click);
            el.PreviewMouseMove += El_PreviewMouseMove;
            SaverClass.Elements.Add(element);
            EditorCanvas.Children.Add(el);
            new ElementProperty(el).ShowDialog();
        }

        private void El_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ElementControl element=sender as ElementControl;
            if (!(sender is ElementControl) || !element.IsSelected || !element.IsDrag) return;
            element.SetLocation(e.GetPosition(EditorCanvas));
            foreach (Connection connection in GraphClass.Connections
                .Where(c => c.Start.Id == element.Element.Id || c.Finish.Id == element.Element.Id).ToList())
            {
                connection.CalculatePoints();
            }

        }

        private Point _mousePoint;
        private void EditorCanvas_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            GraphClass.ClearAllSelection();
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
                SaverClass.SaveData(ofd.FileName);
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
                GraphClass.Connections.Clear();
                SaverClass.LoadData(ofd.FileName);
                GraphClass.ElementZIndex = 10000;
                GraphClass.ConnectionZIndex = 0;
                foreach (ElementClass element in SaverClass.Elements)
                {
                    switch (element.Type)
                    {
                        case 10:
                        {

                            InOutControl inControl = GraphClass.CreateInControl(element);
                            inControl.MainGrid.ContextMenu = new ContextMenu();
                            inControl.MainGrid.ContextMenu.Items.Add(new MenuItem() {Header = "_Соединить вход"});
                            (inControl.MainGrid.ContextMenu.Items[0] as MenuItem).Click += ConntectionMenuItem_Click;
                            EditorCanvas.Children.Add(inControl);
                            continue;
                        }
                        case 11:
                        {
                            EditorCanvas.Children.Add(GraphClass.CreateOutControl(element));
                            foreach (Guid id in element.InElements)
                            {
                                Connection connection = new Connection()
                                {
                                    Start = SaverClass.Elements.First(c => c.Id == id),
                                    Finish = element
                                };
                                connection.CalculatePoints();
                                Panel.SetZIndex(connection.Line, GraphClass.ConnectionZIndex++);
                                GraphClass.Connections.Add(connection);
                                EditorCanvas.Children.Add(connection.Line);
                            }
                            continue;
                        }
                    }

                    ElementControl el = GraphClass.CreateElementControl(element, ConntectionMenuItem_Click);
                    el.PreviewMouseMove += El_PreviewMouseMove;
                    EditorCanvas.Children.Add(el);
                    foreach (Guid id in element.InElements)
                    {
                        Connection connection = new Connection()
                        {
                            Start = SaverClass.Elements.First(c => c.Id == id),
                            Finish = element
                        };
                        connection.CalculatePoints();
                        Panel.SetZIndex(connection.Line, GraphClass.ConnectionZIndex++);
                        GraphClass.Connections.Add(connection);
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
            _idSource = Guid.Parse(GraphClass.GetIdElementSource());
        }

        /// <summary>
        /// Удаление элемента
        /// </summary>
        /// <param name="el"></param>
        void RemoveElement(Guid idElement)
        {
            
            EditorCanvas.Children.Remove(GraphClass.Elements.First(c => c.Element.Id == idElement));
            foreach (Connection connection in GraphClass.Connections.Where(c=>c.Start.Id==idElement||c.Finish.Id==idElement).ToList())
            {
                EditorCanvas.Children.Remove(connection.Line);
                GraphClass.Connections.Remove(connection);
            }
            GraphClass.RemoveElement(idElement);
        }

        private void EditorCanvas_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                ElementControl el = GraphClass.Elements.FirstOrDefault(c => c.IsSelected);
                if (el == null) return;
                if (MessageBox.Show("Вы действительно хотите удалить элемент?", "", MessageBoxButton.YesNo) !=
                    MessageBoxResult.Yes) return;
                RemoveElement(el.Element.Id);
            }
        }

        /// <summary>
        /// Связывание элементов
        /// </summary>
        void SetConnection()
        {
            string elID = GraphClass.GetIdElementSource();
            if (elID == null)
            {
                _connectionMode = false;
                return;
            }
            //var el = GraphClass.Elements.FirstOrDefault(c => c.Element.Id == Guid.Parse(elID));

            var el = SaverClass.Elements.First(c => c.Id == Guid.Parse(elID)); 

            if (el.InElements.Count >= el.InCount)
            {
                MessageBox.Show("У данного элемента задействованы все входы!");
                _connectionMode = false;
                return;
            }

            if (el.InElements.Contains(_idSource))
            {
                MessageBox.Show("Данные элементы уже связаны!");
                _connectionMode = false;
                return;
            }

            SaverClass.Elements.First(c => c.Id == el.Id).InElements.Add(_idSource);
            Connection connection = new Connection()
            {
                Start = SaverClass.Elements.First(c => c.Id == _idSource),
                Finish = el
            };
            Panel.SetZIndex(connection.Line, GraphClass.ConnectionZIndex++);
            connection.CalculatePoints();
            EditorCanvas.Children.Add(connection.Line);
            GraphClass.Connections.Add(connection);
            _connectionMode = false;
        }

        private void MainWindow_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_connectionMode) return;
            SetConnection();
        }

        private void AddInMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ElementClass element = new ElementClass() {InCount = 0, Type = 10,Name = $"X{SaverClass.Elements.Count(c => c.Type == 10)}" };
            SaverClass.Elements.Add(element);
            InOutControl inControl = GraphClass.CreateInControl(element);
            inControl.MainGrid.ContextMenu=new ContextMenu();
            inControl.MainGrid.ContextMenu.Items.Add(new MenuItem() {Header = "_Соединить вход"});
            (inControl.MainGrid.ContextMenu.Items[0] as MenuItem).Click += ConntectionMenuItem_Click;
            EditorCanvas.Children.Add(inControl);
            UpdateViewIn();
        }


        private void AddOutMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ElementClass element = new ElementClass() { InCount = 1, Type = 11, Name = $"Y{SaverClass.Elements.Count(c => c.Type == 11)}" };
            SaverClass.Elements.Add(element);
            InOutControl outControl = GraphClass.CreateInControl(element);
            EditorCanvas.Children.Add(outControl);
            UpdateViewOut();
        }

        /// <summary>
        /// Обновить отображение входов при добавлении
        /// </summary>
        void UpdateViewIn()
        {
            var dEl = (EditorCanvas.ActualHeight-50) / SaverClass.Elements.Count(c => c.Type == 10)/2;
            Point startPoint = new Point(20, dEl);

            foreach (InOutControl inControl in EditorCanvas.Children.OfType<InOutControl>().Where(c=>c.Element.Type==10))
            {
                inControl.SetLocation(startPoint);
                startPoint.Y += 2 * dEl;
            }
        }

        void UpdateViewOut()
        {
            var dEl = (EditorCanvas.ActualHeight - 50) / SaverClass.Elements.Count(c => c.Type == 11) / 2;
            Point startPoint = new Point(EditorCanvas.ActualWidth - 50, dEl);
            foreach (InOutControl outControl in EditorCanvas.Children.OfType<InOutControl>().Where(c => c.Element.Type == 11))
            {
                outControl.SetLocation(startPoint);
                startPoint.Y += 2 * dEl;
            }
        }

        private void ClearAllMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            EditorCanvas.Children.Clear();
            GraphClass.RemoveAll();
        }

        private void CalculateSchemaItem_OnClick(object sender, RoutedEventArgs e)
        {
            new CalculateSchemaWindow().ShowDialog();
        }

    }
}
