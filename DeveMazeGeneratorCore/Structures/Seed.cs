namespace DeveMazeGeneratorCore.Structures;

public interface ISeed
{
}

public readonly record struct XoshiroSeed(ulong S0, ulong S1, ulong S2, ulong S3) : ISeed;
public readonly record struct CompatSeed(int[] SeedArray, int INext, int INextP) : ISeed;
