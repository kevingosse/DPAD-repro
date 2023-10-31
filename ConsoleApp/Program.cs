using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ConsoleApp
{
    [StructLayout(LayoutKind.Explicit, Size = 1024)]
    public struct SomeStruct
    {

    }

    public class SomeClass
    {
        SomeStruct Value;
    }

    internal class Program
    {
        static SomeClass[] _array;
        public const int Count = 1024 * 4;


        static void Main(string[] args)
        {
            _array = new SomeClass[100_000];

            ClearArray();

            GC.Collect(0);

            InnerTest();

            GC.Collect(0);

            InspectArray();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void ClearArray()
        {
            for (int a = 0; a < _array.Length; a++)
            {
                _array[a] = null;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void InspectArray()
        {
            int gen0Count = 0;
            int gen1Count = 0;
            int gen2Count = 0;

            for (int i = 0; i < Count; i++)
            {
                var gen = GC.GetGeneration(_array[i]);

                if (gen == 0)
                {
                    gen0Count++;
                }
                else if (gen == 1)
                {
                    gen1Count++;
                }
                else if (gen == 2)
                {
                    gen2Count++;
                }
            }

            Console.WriteLine($"Summary: gen0: {gen0Count}, gen1: {gen1Count}, gen2: {gen2Count}");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void InnerTest()
        {
            int i = 0;

            var gen0Count = GC.CollectionCount(0);

            for (i = 0; i < Count; i++)
            {
                _array[i] = new SomeClass();
            }

            if (gen0Count != GC.CollectionCount(0))
            {
                Console.WriteLine($"***** A garbage collection happened *****");
            }
        }
    }
}
