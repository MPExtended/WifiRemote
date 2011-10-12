using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace WifiRemote
{
    /// <summary>
    /// Extends some classes with custom methods
    /// </summary>
    public static class WifiRemoteExtensions
    {
        private static Random random = new Random(Environment.TickCount);

        /// <summary>
        /// Shuffle a list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Get a random element from a list
        /// 
        /// Source:
        /// http://nickstips.wordpress.com/2010/08/28/c-optimized-extension-method-get-a-random-element-from-a-collection/
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T GetRandomElement<T>(this IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            // Get the number of elements in the collection
            int count = list.Count();

            // If there are no elements in the collection, return the default value of T
            if (count == 0)
                return default(T);

            // Get a random index
            int index = random.Next(list.Count());

            // When the collection has 100 elements or less, get the random element
            // by traversing the collection one element at a time.
            if (count <= 100)
            {
                using (IEnumerator<T> enumerator = list.GetEnumerator())
                {
                    // Move down the collection one element at a time.
                    // When index is -1 we are at the random element location
                    while (index >= 0 && enumerator.MoveNext())
                        index--;

                    // Return the current element
                    return enumerator.Current;
                }
            }

            // Get an element using LINQ which casts the collection
            // to an IList and indexes into it.
            return list.ElementAt(index);
        }
    }
}
