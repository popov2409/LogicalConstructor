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

        /// <summary>
        /// Метод расчета логического элемента "НЕ"
        /// </summary>
        /// <param name="A"></param>
        /// <returns></returns>
        static string Not(int A) 
        {
            return (A == 1 ? 0 : 1).ToString();
        }
        
        /// <summary>
        /// Метод расчета логического элемента "ИЛИ"
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        static string Or(List<int> B)  
        {
            return B.Max().ToString();
        }
        /// <summary>
        /// Метод расчета логического элемента "ИЛИ-НЕ"
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        static string OrNot(List<int> B)
        {
            return (B.Max()==1?0:1).ToString();
        }
        /// <summary>
        /// Метод расчета логического элемента "И-НЕ"
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        static string AndNot(List<int> B)
        {
            return (B.Min() == 0 ? 1 : 0).ToString();
        }

        /// <summary>
        /// Метод расчета логического элемента "И"
        /// </summary>
        /// <param name="B"></param>
        /// <returns></returns>
        static string And(List<int> B)
        {
            return B.Min().ToString();
        }



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
            _elements[k] = SaverClass.Elements.First(c => c.Type > 10);

            for (int i = 0; i < _n; i++) // Ввод матрицы типов размера N*3
            {
                _type[i] = $"{_elements[i].Type.ToString()}#-";
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

        public static string Calculate(List<string> inSignals)
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
                            case 0:
                                _type[i] = $"{_type[i].Split('#')[0]}#{And(list)}";
                                break;
                            case 1:
                                _type[i] = $"{_type[i].Split('#')[0]}#{Or(list)}";
                                break;
                            case 2:
                                _type[i] = $"{_type[i].Split('#')[0]}#{Not(list[0])}";
                                break;
                            case 3:
                                _type[i] = $"{_type[i].Split('#')[0]}#{AndNot(list)}";
                                break;
                            case 4:
                                _type[i] = $"{_type[i].Split('#')[0]}#{OrNot(list)}";
                                break;
                            case 11:
                                _type[i] = $"{_type[i].Split('#')[0]}#{list[0]}";
                                break;
                        }
                    }
                }
            }
            return _type[_n - 1].Split('#')[1];
        }
    }
}
