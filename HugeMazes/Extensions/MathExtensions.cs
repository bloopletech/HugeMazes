using System.Numerics;

namespace HugeMazes.Extensions;

public static class MathExtensions
{
    extension(int number)
    {
        public int RoundDownOdd() => int.IsEvenInteger(number) ? number - 1 : number;
        public int RoundUpEven() => int.IsOddInteger(number) ? number + 1 : number;

        public int RoundDownToPowerOf2()
        {
            return (int)(BitOperations.RoundUpToPowerOf2((uint)number) >> 1);
        }

        public int DivCeil(int other) => (number + other - 1) / other;
    }

    extension(long number)
    {
        public long DivCeil(long other) => (number + other - 1) / other;
    }
}
