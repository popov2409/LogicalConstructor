using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace LogicalConstructor.DbProxy
{
    public class ElementClass
    {
        public Guid Id { get; set; }
        public Point Location { get; set; }
        /// <summary>
        /// Тип элемента 0-И, 1-ИЛИ, 2-НЕ, 3-И-НЕ, 4-ИЛИ-НЕ
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// С кем связан(входы)
        /// </summary>
        public List<Guid> InElements { get; set; }
        /// <summary>
        /// Колличество входов
        /// </summary>
        public int InCount { get; set; }
        

        public ElementClass()
        {
            Id=Guid.NewGuid();
            InElements=new List<Guid>();
            Type = 2;
            InCount = 1;

        }

    }

    public struct Connection
    {
        public Guid In { get; set; }
        public Guid Out { get; set; }
        //public Point StartPoint { get; set; }
        //public Point EndPoint { get; set; }
        public Polyline Line { get; set; }

    }
}
