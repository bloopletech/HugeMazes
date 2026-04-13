using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Extensions;

public static class RandomExtensions
{
    public static ISeed GetSeed(this Random random)
    {
        var randomType = random.GetRequiredType("System.Random, System.Private.CoreLib");
        var impl = randomType.GetRequiredFieldValue(random, "_impl");

        if(IsXoshiroImpl(impl))
        {
            return Environment.Is64BitProcess ? GetXoshiro64Seed(impl) : GetXoshiro32Seed(impl);
        }
        if(IsCompatSeedImpl(impl)) return GetCompatSeed(impl);
        throw new InvalidOperationException("Random has unknown implementation");
    }

    private static bool IsXoshiroImpl(object impl) => impl.GetType().FullName == "System.Random+XoshiroImpl";
    private static bool IsCompatSeedImpl(object impl) => impl.GetType().FullName == "System.Random+CompatSeedImpl";

    private static Xoshiro32Seed GetXoshiro32Seed(object impl)
    {
        var implType = impl.GetRequiredType("System.Random+XoshiroImpl, System.Private.CoreLib");
        var s0 = implType.GetRequiredFieldValue<uint>(impl, "_s0");
        var s1 = implType.GetRequiredFieldValue<uint>(impl, "_s1");
        var s2 = implType.GetRequiredFieldValue<uint>(impl, "_s2");
        var s3 = implType.GetRequiredFieldValue<uint>(impl, "_s3");
        return new Xoshiro32Seed(s0, s1, s2, s3);
    }

    private static Xoshiro64Seed GetXoshiro64Seed(object impl)
    {
        var implType = impl.GetRequiredType("System.Random+XoshiroImpl, System.Private.CoreLib");
        var s0 = implType.GetRequiredFieldValue<ulong>(impl, "_s0");
        var s1 = implType.GetRequiredFieldValue<ulong>(impl, "_s1");
        var s2 = implType.GetRequiredFieldValue<ulong>(impl, "_s2");
        var s3 = implType.GetRequiredFieldValue<ulong>(impl, "_s3");
        return new Xoshiro64Seed(s0, s1, s2, s3);
    }

    private static CompatSeed GetCompatSeed(object impl)
    {
        var implType = impl.GetRequiredType("System.Random+CompatSeedImpl, System.Private.CoreLib");
        var prng = implType.GetRequiredFieldValue(impl, "_prng");

        var prngType = prng.GetRequiredType("System.Random+CompatPrng, System.Private.CoreLib");
        var seedArray = prngType.GetRequiredFieldValue<int[]>(prng, "_seedArray");
        var inext = prngType.GetRequiredFieldValue<int>(prng, "_inext");
        var inextp = prngType.GetRequiredFieldValue<int>(prng, "_inextp");
        return new CompatSeed(seedArray, inext, inextp);
    }
}
