using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.cgimin.helpers
{
    public class TimeMeasure
    {

        private static long startTime;

        public static void Start()
        {
            startTime = Stopwatch.GetTimestamp();
        }

        public static void Show(string message)
        {
            Console.WriteLine(message + ": " + ((float)(Stopwatch.GetTimestamp() - startTime) / 10000000.0).ToString());
            startTime = Stopwatch.GetTimestamp();
        }


    }
}
