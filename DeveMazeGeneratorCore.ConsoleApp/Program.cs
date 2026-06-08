using DeveMazeGeneratorCore.ConsoleApp;
using DeveMazeGeneratorCore.IO;

CLI.SkipReuse = false;
IStore.BigOverride = true;

var cli = new CLI(new Options(args));
await cli.Run();
