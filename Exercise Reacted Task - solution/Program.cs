using System;

namespace Exercise_Reacted_Task
{
    class Program
    {
        static void Main(string[] args)
        {
            IProducer p = new Producer();

            var d1 = new DeltaDetector(p, 3);
            var d2 = new DeltaDetector(p, 5);

            // TODO: when the each task complete, write the delta
            d1.WhenCrossDelta.ContinueWith(c => Console.WriteLine($"Complete {c.Result}"));
            d2.WhenCrossDelta.ContinueWith(c => Console.WriteLine($"Complete {c.Result}"));

            Console.ReadLine();
        }
    }
}
