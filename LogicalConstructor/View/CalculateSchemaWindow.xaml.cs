﻿using System;
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
    /// Логика взаимодействия для CalculateSchemaWindow.xaml
    /// </summary>
    public partial class CalculateSchemaWindow : Window
    {
        public CalculateSchemaWindow()
        {
            InitializeComponent();
            Calculater.Initialize();
            InitializeDataSchema();
            
            
        }

        List<string> _inSignals;
        private List<TextBlock> _outTextBlocks;

        void InitializeDataSchema()
        {
            _inSignals=new List<string>();
            int i = 0;
            foreach (ElementClass elementClass in SaverClass.Elements.Where(c => c.Type == 10).OrderByDescending(c => c.Name))
            {
                InOutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });
                TextBlock tx = new TextBlock()
                {
                    Text = elementClass.Name,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetRow(tx, 0);
                Grid.SetColumn(tx, i);
                CheckBox cb = new CheckBox()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    IsChecked = false
                };
                cb.Checked += Cb_Checked;
                cb.Unchecked += Cb_Unchecked;
                Grid.SetRow(cb, 1);
                Grid.SetColumn(cb, i);
                InOutGrid.Children.Add(tx);
                InOutGrid.Children.Add(cb);
                _inSignals.Add("0");
                i++;
            }

            InOutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });
            i++;

            _outTextBlocks=new List<TextBlock>();
            foreach (ElementClass elementClass in SaverClass.Elements.Where(c => c.Type == 11)
                .OrderByDescending(c => c.Name))
            {
                InOutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });
                TextBlock txx = new TextBlock()
                {
                    Text = elementClass.Name,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetRow(txx, 0);
                Grid.SetColumn(txx, i);
                TextBlock _outTb = new TextBlock()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetRow(_outTb, 1);
                Grid.SetColumn(_outTb, i);
                InOutGrid.Children.Add(txx);
                InOutGrid.Children.Add(_outTb);
                _outTextBlocks.Add(_outTb);
                i++;
            }
        }

        private void Cb_Unchecked(object sender, RoutedEventArgs e)
        {
            _inSignals[Grid.GetColumn(sender as CheckBox)] = "0";
        }

        private void Cb_Checked(object sender, RoutedEventArgs e)
        {
            _inSignals[Grid.GetColumn(sender as CheckBox)] = "1";
        }

        private void CalculateButton_OnClick(object sender, RoutedEventArgs e)
        {
            List<string> lst = Calculater.Calculate(_inSignals);
            for (int i = 0; i < lst.Count; i++)
            {
                _outTextBlocks[i].Text = lst[i].Split('#')[1];
            }
            
        }
    }
}
