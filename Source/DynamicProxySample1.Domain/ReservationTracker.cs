using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicProxySample1.Domain
{
    public class ReservationTracker
    {
        private static ReservationTracker instance = new ReservationTracker();
        public static ReservationTracker getInstance()
        {
            return instance;
        }

        private ReservationTracker()
        {
        }

        public virtual void reserve(IReservable resource, object reservationHolder)
        {
            Console.WriteLine("ReservationTracker: making a reservation.");
            reservationList.Add(resource, reservationHolder);
        }

        Dictionary<IReservable, object> reservationList = new Dictionary<IReservable, object>();
    }
}
