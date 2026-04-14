using DeveMazeGeneratorCore.ConsoleApp;
using DeveMazeGeneratorCore.IO;

CLI.SkipReuse = true;
IStore.BigOverride = true;

var cli = new CLI(new Options(args));
await cli.Run();
