using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicProxySample1.Domain
{
    public class Course // Note: not reservable
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual int Number { get; set; }

        public Book Book { get; set; }
        public Instructor Instructor { get ; set;}
    }
}
