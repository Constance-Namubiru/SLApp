using NationalInstruments.NetworkVariable;
using FGWriter.Classes;
using System;

namespace FGWriter
{
    class Program
    {

       
        static void Main(string[] args)
        {
            var subscriber = new Subscriber();
            subscriber.Connect();

            Console.ReadLine();
        }
    }
}
