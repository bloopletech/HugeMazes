using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Extensions;

public static class RandomExtensions
{
    public static ISeed GetSeed(this Random random)
    {
        var impl = random.GetRequiredFieldValue("_impl");

        if(IsXoshiroImpl(impl)) return GetXoshiroSeed(impl);
        if(IsCompatSeedImpl(impl)) return GetCompatSeed(impl);
        throw new InvalidOperationException("Random has unknown implementation");
    }

    private static bool IsXoshiroImpl(object impl) => impl.GetType().FullName == "System.Random+XoshiroImpl";
    private static bool IsCompatSeedImpl(object impl) => impl.GetType().FullName == "System.Random+CompatSeedImpl";

    private static XoshiroSeed GetXoshiroSeed(object impl)
    {
        if(!IsXoshiroImpl(impl))throw new InvalidOperationException("Random does not have a XoshiroImpl");

        var s0 = impl.GetRequiredFieldValue<ulong>("_s0");
        var s1 = impl.GetRequiredFieldValue<ulong>("_s1");
        var s2 = impl.GetRequiredFieldValue<ulong>("_s2");
        var s3 = impl.GetRequiredFieldValue<ulong>("_s3");
        return new XoshiroSeed(s0, s1, s2, s3);
    }

    private static CompatSeed GetCompatSeed(object impl)
    {
        if(!IsCompatSeedImpl(impl)) throw new InvalidOperationException("Random does not have a CompatSeedImpl");

        var prng = impl.GetRequiredFieldValue("_prng");
        if(prng.GetType().FullName != "System.Random+CompatPrng")
        {
            throw new InvalidOperationException("CompatSeedImpl does not have a CompatPrng");
        }

        var seedArray = prng.GetRequiredFieldValue<int[]>("_seedArray");
        var inext = prng.GetRequiredFieldValue<int>("_inext");
        var inextp = prng.GetRequiredFieldValue<int>("_inextp");
        return new CompatSeed(seedArray, inext, inextp);
    }
}
