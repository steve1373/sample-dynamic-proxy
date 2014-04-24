using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicProxySample1.Domain
{
    public class Book  // Note: not reservable
    {
        public virtual string Name { get; set; }
        public virtual string ISBN { get; set; }
        public virtual string Barcode { get; set; }
        public virtual int PublicationYear { get; set; }
        public virtual bool CheckedOut { get; set; }
    }
}
