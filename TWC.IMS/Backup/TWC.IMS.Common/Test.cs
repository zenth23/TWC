using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common
{
    public static class Test
    {
        /// <summary>
        /// Call this method to simulate CPU-intesive processing.
        /// Ex: long prime = Test.FindPrimeNumber(10000);
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static long FindPrimeNumber(int n)
        {
            int count = 0;
            long a = 2;
            while (count < n)
            {
                long b = 2;
                int prime = 1;  // to check if found a prime
                while (b * b <= a)
                {
                    if (a % b == 0)
                    {
                        prime = 0;
                        break;
                    }
                    b++;
                }
                if (prime > 0)
                {
                    count++;
                }
                a++;
            }
            return (--a);
        }
    }
}
