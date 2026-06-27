using HugeMazes.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HugeMazes.Tests.Generators;

[TestClass]
public class TheGenerateMethod
{
    [TestMethod]
    public void GeneratesAMaze()
    {
        //Arrange

        //Act
        var maze = HugeMazes.Generate(IStore.Create(false), 129, 129);

        //Assert
        Assert.IsFalse(maze[0, 0]);
        Assert.IsTrue(maze[1, 1]);
    }

    [TestMethod]
    public void GeneratesAPerfectMaze()
    {
        //Arrange
        //Act
        var maze = HugeMazes.Generate(IStore.Create(false), 129, 129);

        Assert.IsTrue(Verifier.IsPerfectMaze(maze));
    }
}
