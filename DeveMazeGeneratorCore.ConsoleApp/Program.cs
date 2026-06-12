using DeveMazeGeneratorCore.ConsoleApp;
using DeveMazeGeneratorCore.IO;

CLI.SkipReuse = true;
IStore.LongOverride = true;

var cli = new CLI(new Options(args));
cli.Run();
