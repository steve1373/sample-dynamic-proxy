using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicProxySample1.Domain
{
    public interface IReservable
    {
        bool checkout(Student student);
    }
}
