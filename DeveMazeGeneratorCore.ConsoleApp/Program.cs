using DeveMazeGeneratorCore.ConsoleApp;
using DeveMazeGeneratorCore.IO;

//generate 16384 16384 verify solve verify-path render render-path

CLI.SkipReuse = true;
IStore.LongOverride = true;

var cli = new CLI(new Options(args));
cli.Run();
