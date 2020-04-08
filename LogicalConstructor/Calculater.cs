using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LogicalConstructor.DbProxy;

namespace LogicalConstructor
{
    public class Calculater
    {
        //{0, "И"},
        //{1, "ИЛИ"},
        //{2, "НЕ"},
        //{3, "И-НЕ"},
        //{4, "ИЛИ-НЕ"}

        static int _n = 0;
        static int[,] _smejnost;

        static string [] _type;

        private static ElementClass[] _elements;
        public static void Initialize()
        {
            _n = SaverClass.Elements.Count;
            _smejnost = new int[_n, _n];
            _type = new string[_n];
            _elements=new ElementClass[_n];
            int k = 0;
            foreach (ElementClass element in SaverClass.Elements.Where(c=>c.Type==10).OrderByDescending(c=>c.Name))
            {
                _elements[k] = element;
                k++;
            }
            foreach (ElementClass element in SaverClass.Elements.Where(c => c.Type < 10))
            {
                _elements[k] = element;
                k++;
            }
            foreach (ElementClass element in SaverClass.Elements.Where(c => c.Type == 11).OrderByDescending(c => c.Name))
            {
                _elements[k] = element;
                k++;
            }

            for (int i = 0; i < _n; i++) // Ввод матрицы типов размера N*3
            {
                _type[i] = $"{_elements[i].Type}#-";
            }

            for (int i = 0; i < _n; i++)
            {
                foreach (Guid idElement in _elements[i].InElements)
                {
                    int j = _elements.ToList().IndexOf(_elements.First(c => c.Id == idElement));
                    _smejnost[i, j] = 1;
                }
            }

            for (int i = 0; i < _n; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    Console.Write(_smejnost[i,j]+" ");
                }
                Console.WriteLine();
            }
           
        }

        public static List<string> Calculate(List<string> inSignals)
        {
            for (int i = 0; i < _n; i++)
            {
                _type[i] = $"{_elements[i].Type}#-";
            }

            for (int i = 0; i < inSignals.Count; i++)
            {
                _type[i] = $"{_type[i].Split('#')[0]}#{inSignals[i]}";
            }

            while (_type.Count(s => s.Contains("-")) > 0)
            {
                for (int i = 0; i < _n; i++)
                {
                    if (_type[i].Contains("-"))
                    {
                        List<string> tempList = new List<string>();
                        for (int j = 0; j < _n; j++)
                        {
                            if (_smejnost[i, j] == 1) tempList.Add(_type[j]);
                        }

                        if (tempList.Count(s => s.Contains("-")) > 0)
                        {
                            continue;
                        }

                        List<int> list = tempList.Select(c => c.Split('#')[1]).Select(int.Parse).ToList();

                        switch (int.Parse(_type[i].Split('#')[0]))
                        {
                            case 0: //И
                                _type[i] = $"{_type[i].Split('#')[0]}#{list.Min()}";
                                break;
                            case 1: //ИЛИ
                                _type[i] = $"{_type[i].Split('#')[0]}#{list.Max()}";
                                break;
                            case 2://НЕ
                                _type[i] = $"{_type[i].Split('#')[0]}#{Convert.ToInt32(list[0]==0)}";
                                break;
                            case 3://И-НЕ
                                _type[i] = $"{_type[i].Split('#')[0]}#{Convert.ToInt32(list.Min()==0)}";
                                break;
                            case 4://ИЛИ-НЕ
                                _type[i] = $"{_type[i].Split('#')[0]}#{Convert.ToInt32(list.Max()==0)}";
                                break;
                            case 11://ВЫХОД
                                _type[i] = $"{_type[i].Split('#')[0]}#{list[0]}";
                                break;
                        }
                    }
                }
            }

            return _type.Skip(_type.Length - _type.Count(c => c.Split('#')[0].Equals("11"))).ToList();
        }
    }
}
