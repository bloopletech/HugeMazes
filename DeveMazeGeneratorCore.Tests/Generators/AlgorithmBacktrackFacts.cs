using Xunit;

namespace DeveMazeGeneratorCore.Tests.Generators
{
    namespace AlgorithmBacktrackFacts
    {
        public class TheGenerateMethod
        {
            [Fact]
            public void GeneratesAMaze()
            {
                //Arrange

                //Act
                var maze = DeveMazeGeneratorCore.Generate(128, 128);

                //Assert
                Assert.False(maze[0, 0]);
                Assert.True(maze[1, 1]);
            }

            [Fact]
            public void GeneratesAPerfectMaze()
            {
                //Arrange
                //Act
                var maze = DeveMazeGeneratorCore.Generate(128, 128);

                Assert.True(Verifier.IsPerfectMaze(maze));
            }
        }
    }
}
