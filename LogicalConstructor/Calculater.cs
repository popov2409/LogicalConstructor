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
                    _smejnost[j, i] = 1;
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
            //for (int i = 0; i < _n; i++) // Ввод матрицы смежностей размера N*N
            //{
            //    int[] q = smej.ReadLine().Split().Where(x => x.Length > 0).Select(x => int.Parse(x)).ToArray();
            //    for (int j = 0; j < _n; j++)
            //    {
            //        _smejnost[i, j] = q[j];
            //    }
            //}
           
        }

        public static string Calculate(List<string> inSignals)
        {
            for (int i = 0; i < inSignals.Count; i++)
            {
                _type[i] = _type[i].Split('#')[0]+"#"+inSignals[i];
            }

            while (_type.Count(s => s.Contains("-")) > 0)
            {
                int yy = _type.Count(s => s.Contains("-"));
                for (int i = 0; i < _n; i++)
                {
                    if (_type[i].Contains("-"))
                    {
                        switch (int.Parse(_type[i].Split('#')[0]))
                        {
                            case 0:
                                try
                                {
                                    List<int> list = new List<int>();
                                    for (int j = 0; j < _n; j++)
                                    {
                                        if ((_smejnost[j, i] == 1) && (!_type[j].Contains("-"))
                                        ) //все 1 в таблице смежностей и у данного типа есть значение
                                            list.Add(int.Parse(_type[j].Split('#')[1]));
                                    }

                                    _type[i] = _type[i].Split('#')[0] + "#" + And(list);
                                    break;
                                }
                                catch
                                {
                                    break;
                                }
                            case 1:
                                try
                                {
                                    List<int> list = new List<int>();
                                    for (int j = 0; j < _n; j++)
                                    {
                                        if ((_smejnost[j, i] == 1) && (!_type[j].Contains("-"))
                                        ) //все 1 в таблице смежностей и у данного типа есть значение
                                            list.Add(int.Parse(_type[j].Split('#')[1]));
                                    }

                                    _type[i] = _type[i].Split('#')[0] + "#" + Or(list);
                                    break;
                                }
                                catch
                                {
                                    break;
                                }
                            case 2:
                                try
                                {
                                    for (int j = 0; j < _n; j++)
                                    {
                                        if (_smejnost[j, i] == 1
                                        ) // до первой 1, т.к. элемент "НЕ" работает только с одной переменной
                                        {
                                            _type[i] = _type[i].Split('#')[0] + "#" +
                                                       Not(int.Parse(_type[j].Split('#')[1]));
                                            break;
                                        }
                                    }

                                    break;
                                }
                                catch
                                {
                                    break;
                                }
                            case 3:
                                try
                                {
                                    List<int> list = new List<int>();
                                    for (int j = 0; j < _n; j++)
                                    {
                                        if ((_smejnost[j, i] == 1) && (!_type[j].Contains("-"))
                                        ) //все 1 в таблице смежностей и у данного типа есть значение
                                            list.Add(int.Parse(_type[j].Split('#')[1]));
                                    }

                                    _type[i] = _type[i].Split('#')[0] + "#" + AndNot(list);
                                    break;
                                }
                                catch
                                {
                                    break;
                                }
                            case 4:
                                try
                                {
                                    List<int> list = new List<int>();
                                    for (int j = 0; j < _n; j++)
                                    {
                                        if ((_smejnost[j, i] == 1) && (!_type[j].Contains("-"))
                                        ) //все 1 в таблице смежностей и у данного типа есть значение
                                            list.Add(int.Parse(_type[j].Split('#')[1]));
                                    }

                                    _type[i] = _type[i].Split('#')[0] + "#" + OrNot(list);
                                    break;
                                }
                                catch
                                {
                                    break;
                                }
                            case 11:
                                try
                                {
                                    for (int j = 0; j < _n; j++)
                                    {
                                        if (_smejnost[j, i] == 1
                                        ) // до первой 1, т.к. элемент "НЕ" работает только с одной переменной
                                        {
                                            _type[i] = _type[i].Split('#')[0] + "#" + int.Parse(_type[j].Split('#')[1]);
                                            break;
                                        }
                                    }

                                    break;
                                }
                                catch
                                {
                                    break;
                                }
                        }

                    }
                }
            }

            // int g=_type.
            //for (int i = 0; i < _n; i++) // по таблице типов до первого пустого в 3м столбике
            //{
            //    if (_type[i, 1] == "-") //2й столбик пустой
            //    {


            //        if (_type[i, 0] == "0") //логический элемент "И"
            //        {
            //            List<int> and = new List<int>();
            //            for (int j = 0; j < _n; j++)
            //            {
            //                if ((_smejnost[j, i] == 1) && (_type[j, 1] != "-")
            //                ) //все 1 в таблице смежностей и у данного типа есть значение
            //                    and.Add(int.Parse(_type[j, 1]));
            //            }

            //            _type[i, 1] = And(and);
            //        }

            //        if (_type[i, 0] == "1") //логический элемент "ИЛИ"
            //        {
            //            List<int> or = new List<int>();
            //            for (int j = 0; j < _n; j++)
            //            {
            //                if ((_smejnost[j, i] == 1) && (_type[j, 0] != "-")
            //                ) //все 1 в таблице смежностей и у данного типа есть значение
            //                    or.Add(int.Parse(_type[j, 1]));
            //            }

            //            _type[i, 1] = Or(or);
            //        }

            //        if (_type[i, 0] == "2") // логический элемент "НЕ"
            //            for (int j = 0; j < _n; j++)
            //            {
            //                if (_smejnost[j, i] == 1
            //                ) // до первой 1, т.к. элемент "НЕ" работает только с одной переменной
            //                {
            //                    _type[i, 1] = Not(int.Parse(_type[j, 1]));
            //                    break;
            //                }

            //            }

            //        if (_type[i, 0] == "3") //логический элемент "И-НЕ"
            //        {
            //            List<int> and = new List<int>();
            //            for (int j = 0; j < _n; j++)
            //            {
            //                if ((_smejnost[j, i] == 1) && (_type[j, 1] != "-")
            //                ) //все 1 в таблице смежностей и у данного типа есть значение
            //                    and.Add(int.Parse(_type[j, 1]));
            //            }

            //            _type[i, 1] = AndNot(and);
            //        }

            //        if (_type[i, 0] == "4") //логический элемент "ИЛИ-НЕ"
            //        {
            //            List<int> or = new List<int>();
            //            for (int j = 0; j < _n; j++)
            //            {
            //                if ((_smejnost[j, i] == 1) && (_type[j, 0] != "-"))//все 1 в таблице смежностей и у данного типа есть значение
            //                    or.Add(int.Parse(_type[j, 1]));
            //            }

            //            _type[i, 1] = OrNot(or);
            //        }

            //    }
            //}

            //return _type[_n - 1, 2]; // значение на выходе последнего логического блока
            return _type[_n-1].Split('#')[1];
        }
    }
}
