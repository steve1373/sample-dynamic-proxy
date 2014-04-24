using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicProxySample1.Domain
{
    public class Instructor
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string OfficeNumber { get; set; }
    }
}
