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

            // TODO: write when the each task complete the delta


            Console.ReadKey();
        }
    }
}
