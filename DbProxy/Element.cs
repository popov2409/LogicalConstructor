using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbProxy
{
    class Element
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 0-and, 1-or, 2-invertion, 10-in, 11-out
        /// </summary>
        public int Type { get; set; }
        public bool Inversion { get; set; }
        public List<Guid> InElement { get; set; }
        public Point Location { get; set; }
        


        public Element()
        {
            Id=Guid.NewGuid();
            InElement=new List<Guid>();
            Inversion = false;
        }
    }
}
