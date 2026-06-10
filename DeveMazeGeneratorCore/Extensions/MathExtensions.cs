using System.Numerics;

namespace DeveMazeGeneratorCore.Extensions;

public static class MathExtensions
{
    extension(int number)
    {
        /// <summary>
        /// Makes a number uneven. E.g. if 32 is inputted it will return 31.
        /// If 31 is inputted it will return 31.
        /// </summary>
        /// <param name="number">The input number</param>
        /// <returns>The first uneven number lower then this</returns>
        public int MakeUneven() => int.IsEvenInteger(number) ? number - 1 : number;

        public int RoundDownToPowerOf2()
        {
            return (int)(BitOperations.RoundUpToPowerOf2((uint)number) >> 1);
        }
    }
}
