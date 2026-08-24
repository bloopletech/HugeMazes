using System.Runtime.CompilerServices;
using HugeMazes.Structures;

namespace HugeMazes.Extensions;

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
    //public static ISeed GetSeed(this Random random)
    //{
    //    object impl;
    //    try
    //    {
    //        impl = GetRandomXoshiroImpl(random);
    //        return Environment.Is64BitProcess ? GetXoshiro64Seed(impl) : GetXoshiro32Seed(impl);
    //    }
    //    catch(Exception ex)
    //    {
    //        try
    //        {
    //            impl = GetRandomCompatImpl(random);
    //            return GetCompatSeed(impl);
    //        }
    //        catch(Exception)
    //        {
    //            throw new InvalidOperationException("Random has unknown implementation");
    //        }
    //    }
    //}

    public static void SetSeed(this Random random, ISeed seed)
    {
        var randomType = random.GetRequiredType("System.Random, System.Private.CoreLib");
        var impl = randomType.GetRequiredFieldValue(random, "_impl");

        if(IsXoshiroImpl(impl))
        {
            if(Environment.Is64BitProcess) SetXoshiro64Seed(impl, (Xoshiro64Seed)seed);
            else SetXoshiro32Seed(impl, (Xoshiro32Seed)seed);
            return;
        }

        if(IsCompatSeedImpl(impl))
        {
            SetCompatSeed(impl, (CompatSeed)seed);
            return;
        }

        throw new InvalidOperationException("Random has unknown implementation");
    }
    //public static void SetSeed(this Random random, ISeed seed)
    //{
    //    object impl;
    //    try
    //    {
    //        impl = GetRandomXoshiroImpl(random);
    //        if(Environment.Is64BitProcess) SetXoshiro64Seed(impl, (Xoshiro64Seed)seed);
    //        else SetXoshiro32Seed(impl, (Xoshiro32Seed)seed);
    //    }
    //    catch(Exception ex)
    //    {
    //        try
    //        {
    //            impl = GetRandomCompatImpl(random);
    //            SetCompatSeed(impl, (CompatSeed)seed);
    //        }
    //        catch(Exception)
    //        {
    //            throw new InvalidOperationException("Random has unknown implementation");
    //        }
    //    }
    //}

    public static void SetSeed(this Random random, int seed)
    {
        var randomType = random.GetRequiredType("System.Random, System.Private.CoreLib");
        var impl = randomType.GetRequiredFieldValue(random, "_impl");

        if(IsXoshiroImpl(impl))
        {
            if(Environment.Is64BitProcess) SetXoshiro64Seed(impl, seed);
            else SetXoshiro32Seed(impl, seed);
            return;
        }

        if(IsCompatSeedImpl(impl))
        {
            SetCompatSeed(impl, seed);
            return;
        }

        throw new InvalidOperationException("Random has unknown implementation");
    }
    //public static void SetSeed(this Random random, int seed)
    //{
    //    object impl;
    //    try
    //    {
    //        impl = GetRandomXoshiroImpl(random);
    //        if(Environment.Is64BitProcess) SetXoshiro64Seed(impl, seed);
    //        else SetXoshiro32Seed(impl, seed);
    //    }
    //    catch(Exception ex)
    //    {
    //        try
    //        {
    //            impl = GetRandomCompatImpl(random);
    //            SetCompatSeed(impl, seed);
    //        }
    //        catch(Exception)
    //        {
    //            throw new InvalidOperationException("Random has unknown implementation");
    //        }
    //    }
    //}

    private static bool IsXoshiroImpl(object impl) => impl.GetType().FullName == "System.Random+XoshiroImpl";
    private static bool IsCompatSeedImpl(object impl) => impl.GetType().FullName == "System.Random+CompatSeedImpl";

    private static Xoshiro32Seed GetXoshiro32Seed(object impl)
    {
        var s0 = GetRandomXoshiro32S0(impl);
        var s1 = GetRandomXoshiro32S1(impl);
        var s2 = GetRandomXoshiro32S2(impl);
        var s3 = GetRandomXoshiro32S3(impl);
        return new Xoshiro32Seed(s0, s1, s2, s3);
    }

    private static void SetXoshiro32Seed(object impl, Xoshiro32Seed seed)
    {
        ref var s0 = ref GetRandomXoshiro32S0(impl);
        s0 = seed.S0;
        ref var s1 = ref GetRandomXoshiro32S1(impl);
        s1 = seed.S1;
        ref var s2 = ref GetRandomXoshiro32S2(impl);
        s2 = seed.S2;
        ref var s3 = ref GetRandomXoshiro32S3(impl);
        s3 = seed.S3;
    }

    private static void SetXoshiro32Seed(object impl, int seed) =>
        SetXoshiro32Seed(impl, new Xoshiro32Seed((uint)seed, 0, 0, 0));

    private static Xoshiro64Seed GetXoshiro64Seed(object impl)
    {
        var s0 = GetRandomXoshiro64S0(impl);
        var s1 = GetRandomXoshiro64S1(impl);
        var s2 = GetRandomXoshiro64S2(impl);
        var s3 = GetRandomXoshiro64S3(impl);
        return new Xoshiro64Seed(s0, s1, s2, s3);
    }

    private static void SetXoshiro64Seed(object impl, Xoshiro64Seed seed)
    {
        ref var s0 = ref GetRandomXoshiro64S0(impl);
        s0 = seed.S0;
        ref var s1 = ref GetRandomXoshiro64S1(impl);
        s1 = seed.S1;
        ref var s2 = ref GetRandomXoshiro64S2(impl);
        s2 = seed.S2;
        ref var s3 = ref GetRandomXoshiro64S3(impl);
        s3 = seed.S3;
    }

    private static void SetXoshiro64Seed(object impl, int seed) =>
        SetXoshiro64Seed(impl, new Xoshiro64Seed((ulong)seed, 0, 0, 0));

    //private static CompatSeed GetCompatSeed(object impl)
    //{
    //    var prng = GetRandomCompatImplPrng(impl);
    //    var seedArray = GetRandomCompatSeedArray(prng);
    //    var inext = GetRandomCompatINext(prng);
    //    var inextp = GetRandomCompatINextP(prng);
    //    return new CompatSeed(seedArray, inext, inextp);
    //}
    private static CompatSeed GetCompatSeed(object impl)
    {
        var implType = impl.GetRequiredType("System.Random+CompatSeedImpl, System.Private.CoreLib");
        var prng = implType.GetRequiredFieldValue(impl, "_prng");

        var seedArray = GetRandomCompatSeedArray(prng);
        var inext = GetRandomCompatINext(prng);
        var inextp = GetRandomCompatINextP(prng);
        return new CompatSeed(seedArray, inext, inextp);
    }

    //private static void SetCompatSeed(object impl, CompatSeed seed)
    //{
    //    var implType = impl.GetRequiredType("System.Random+CompatSeedImpl, System.Private.CoreLib");
    //    var prng = implType.GetRequiredFieldValue(impl, "_prng");

    //    var prngType = prng.GetRequiredType("System.Random+CompatPrng, System.Private.CoreLib");
    //    prngType.SetFieldValue(prng, "_seedArray", seed.SeedArray);
    //    prngType.SetFieldValue(prng, "_inext", seed.INext);
    //    prngType.SetFieldValue(prng, "_inextp", seed.INextP);
    //}
    //private static void SetCompatSeed(object impl, CompatSeed seed)
    //{
    //    var prng = GetRandomCompatImplPrng(impl);
    //    ref var seedArray = ref GetRandomCompatSeedArray(prng);
    //    seedArray = seed.SeedArray;
    //    ref var inext = ref GetRandomCompatINext(prng);
    //    inext = seed.INext;
    //    ref var inextp = ref GetRandomCompatINextP(prng);
    //    inextp = seed.INextP;
    //}
    private static void SetCompatSeed(object impl, CompatSeed seed)
    {
        var implType = impl.GetRequiredType("System.Random+CompatSeedImpl, System.Private.CoreLib");
        var prng = implType.GetRequiredFieldValue(impl, "_prng");

        ref var seedArray = ref GetRandomCompatSeedArray(prng);
        seedArray = seed.SeedArray;
        ref var inext = ref GetRandomCompatINext(prng);
        inext = seed.INext;
        ref var inextp = ref GetRandomCompatINextP(prng);
        inextp = seed.INextP;
    }

    //private static void SetCompatSeed(object impl, int seed)
    //{
    //    var implType = impl.GetRequiredType("System.Random+CompatSeedImpl, System.Private.CoreLib");
    //    var prng = implType.GetRequiredFieldValue(impl, "_prng");

    //    var prngType = prng.GetRequiredType("System.Random+CompatPrng, System.Private.CoreLib");
    //    var initMethod = prngType.GetRequiredMethod("Initialize");
    //    initMethod.Invoke(prng, [seed]);
    //}
    //private static void SetCompatSeed(object impl, int seed)
    //{
    //    var prng = GetRandomCompatImplPrng(impl);
    //    RandomCompatInitialize(prng, seed);
    //}
    private static void SetCompatSeed(object impl, int seed)
    {
        var implType = impl.GetRequiredType("System.Random+CompatSeedImpl, System.Private.CoreLib");
        var prng = implType.GetRequiredFieldValue(impl, "_prng");

        RandomCompatInitialize(prng, seed);
    }

    //[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_impl")]
    //[return: UnsafeAccessorType("System.Random+XoshiroImpl, System.Private.CoreLib")]
    //private static extern object GetRandomXoshiroImpl(Random @this);

    //[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_impl")]
    //[return: UnsafeAccessorType("System.Random+CompatSeedImpl, System.Private.CoreLib")]
    //private static extern object GetRandomCompatImpl(Random @this);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_s0")]
    private static extern ref uint GetRandomXoshiro32S0([UnsafeAccessorType("System.Random+XoshiroImpl, System.Private.CoreLib")] object @this);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_s1")]
    private static extern ref uint GetRandomXoshiro32S1([UnsafeAccessorType("System.Random+XoshiroImpl, System.Private.CoreLib")] object @this);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_s2")]
    private static extern ref uint GetRandomXoshiro32S2([UnsafeAccessorType("System.Random+XoshiroImpl, System.Private.CoreLib")] object @this);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_s3")]
    private static extern ref uint GetRandomXoshiro32S3([UnsafeAccessorType("System.Random+XoshiroImpl, System.Private.CoreLib")] object @this);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_s0")]
    private static extern ref ulong GetRandomXoshiro64S0([UnsafeAccessorType("System.Random+XoshiroImpl, System.Private.CoreLib")] object @this);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_s1")]
    private static extern ref ulong GetRandomXoshiro64S1([UnsafeAccessorType("System.Random+XoshiroImpl, System.Private.CoreLib")] object @this);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_s2")]
    private static extern ref ulong GetRandomXoshiro64S2([UnsafeAccessorType("System.Random+XoshiroImpl, System.Private.CoreLib")] object @this);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_s3")]
    private static extern ref ulong GetRandomXoshiro64S3([UnsafeAccessorType("System.Random+XoshiroImpl, System.Private.CoreLib")] object @this);

    //[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_prng")]
    //[return: UnsafeAccessorType("System.Random+CompatPrng, System.Private.CoreLib")]
    //private static extern object GetRandomCompatImplPrng([UnsafeAccessorType("System.Random+CompatSeedImpl, System.Private.CoreLib")] object @this);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_seedArray")]
    private static extern ref int[] GetRandomCompatSeedArray([UnsafeAccessorType("System.Random+CompatPrng, System.Private.CoreLib")] object @this);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_inext")]
    private static extern ref int GetRandomCompatINext([UnsafeAccessorType("System.Random+CompatPrng, System.Private.CoreLib")] object @this);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_inextp")]
    private static extern ref int GetRandomCompatINextP([UnsafeAccessorType("System.Random+CompatPrng, System.Private.CoreLib")] object @this);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Initialize")]
    private static extern void RandomCompatInitialize([UnsafeAccessorType("System.Random+CompatPrng, System.Private.CoreLib")] object @this, int seed);
}
