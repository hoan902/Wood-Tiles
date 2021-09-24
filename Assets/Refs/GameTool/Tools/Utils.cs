using System.Collections;
using System.Text;
using System;
using UnityEngine;
using System.Collections.Generic;

// A set of useful functions
namespace Berry.Utils
{
    public static class Utils
    {

        public static T GetRandom<T>(this ICollection<T> collection)
        {
            if (collection == null)
                return default(T);
            int t = UnityEngine.Random.Range(0, collection.Count);
            foreach (T element in collection)
            {
                if (t == 0)
                    return element;
                t--;
            }
            return default(T);
        }

    }
}

   