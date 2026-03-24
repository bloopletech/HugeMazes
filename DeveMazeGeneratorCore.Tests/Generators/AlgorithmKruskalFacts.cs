using DeveMazeGeneratorCore.Generators;
using DeveMazeGeneratorCore.Generators.Helpers;
using DeveMazeGeneratorCore.Helpers;
using DeveMazeGeneratorCore.InnerMaps;
using Xunit;

namespace DeveMazeGeneratorCore.Tests.Generators
{
    namespace AlgorithmKruskalFacts
    {
        public class TheGenerateMethod
        {
            [Fact]
            public void GeneratesAMaze()
            {
                //Arrange
                var generator = new AlgorithmKruskal();

                //Act
                var maze = MazeGenerator.Generate<AlgorithmKruskal, BitArreintjeFastInnerMap, NetRandom>(128, 128);

                //Assert
                Assert.False(maze.InnerMap[0, 0]);
                Assert.True(maze.InnerMap[1, 1]);
            }

            [Fact]
            public void GeneratesAPerfectMaze()
            {
                //Arrange

                //Act
                var maze = MazeGenerator.Generate<AlgorithmKruskal, BitArreintjeFastInnerMap, NetRandom>(128, 128);

                Assert.True(MazeVerifier.IsPerfectMaze(maze.InnerMap));
            }
        }
    }
}
