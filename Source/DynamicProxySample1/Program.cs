using DynamicProxySample1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicProxySample1
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up!");

            Book book = createRegularBook("Design Patterns (non-proxy)");
            Book proxyBook = createProxyBook("More Design Patterns (proxy)");

            testReservable(book);       // not reservable
            testReservable(proxyBook);  // proxy is reservable

            book.PublicationYear = 2012;
            proxyBook.PublicationYear = 2013;

            Console.ReadLine();
        }

        private static void testReservable(Book book)
        {
            if (book is IReservable)
            {
                Console.WriteLine("Checking out a book:" + book.Name);
                IReservable reservable = (IReservable)book;
                reservable.checkout(new Student { ID = 1, Name = "Stephan" });
            }
            else
            {
                Console.WriteLine("Could not check out book:" + book.Name);
            }
        }

        private static Book createProxyBook(string name)
        {
            Book result = DynamicProxyCreator.MakeProxy<Book>();
            result.Name = name;
            return result;
        }

        private static Book createRegularBook(string name)
        {
            Book result = new Book();
            result.Name = name;
            return result;
        }
    }
}
