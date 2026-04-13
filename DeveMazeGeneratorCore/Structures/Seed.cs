namespace DeveMazeGeneratorCore.Structures;

public interface ISeed
{
}

public readonly record struct Xoshiro32Seed(uint S0, uint S1, uint S2, uint S3) : ISeed;
public readonly record struct Xoshiro64Seed(ulong S0, ulong S1, ulong S2, ulong S3) : ISeed;
public readonly record struct CompatSeed(int[] SeedArray, int INext, int INextP) : ISeed;
