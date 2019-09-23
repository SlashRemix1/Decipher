using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decipher_Server
{
    internal static class Extensions
    {
        internal static void Write<T>(this List<T> thisList, int startIndex, T[] sourceArray, int sourceIndex, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int num = startIndex + i;
                int num2 = sourceIndex + i;
                if (num >= thisList.Count)
                {
                    thisList.Add(sourceArray[num2]);
                }
                else
                {
                    thisList[num] = sourceArray[num2];
                }
            }
        }
    }
}
