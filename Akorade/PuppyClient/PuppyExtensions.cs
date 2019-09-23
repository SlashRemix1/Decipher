using System.Collections.Generic;

namespace Doumi.PuppyServer
{
    internal static class PuppyExtensions
    {
        internal static void Write<T>(this List<T> thisList, int startIndex, T[] sourceArray, int sourceIndex, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int dstIndex = startIndex + i;
                int srcIndex = sourceIndex + i;

                if (dstIndex >= thisList.Count)
                    thisList.Add(sourceArray[srcIndex]);
                else
                    thisList[dstIndex] = sourceArray[srcIndex];
            }
        }
    }
}
