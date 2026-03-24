using DeveMazeGeneratorCore.InnerMaps;

namespace DeveMazeGeneratorCore.Factories;

public class InnerMapFactory<T> : IInnerMapFactory<T> where T : InnerMap
{
    public InnerMapFactory()
    {
    }

    public T Create(int desiredWidth, int desiredHeight)
    {
        var typeSwitcher = new TypeSwitch<InnerMap>()
          .Case(() => new BitArreintjeFastInnerMap(desiredWidth, desiredHeight))
          .Case(() => new BitArreintjeFastChunkedInnerMap(desiredWidth, desiredHeight));

        var createdObject = typeSwitcher.Switch(typeof(T));
        return (T)createdObject;
    }
}
