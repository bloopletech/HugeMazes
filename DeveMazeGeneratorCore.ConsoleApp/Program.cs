using DeveMazeGeneratorCore.ConsoleApp;

var options = new Options(args);
var context = new JobContext();

IJob ParseJob(string job)
{
    switch(job)
    {
        case "generate": return GenerateJob.Parse(options, context);
        case "verify": return VerifyJob.Parse(options, context);
        case "solve": return SolveJob.Parse(options, context);
        case "render": return RenderJob.Parse(options, context);
        case "render-path": return RenderPathJob.Parse(options, context);
        case "benchmark": return BenchmarkJob.Parse(options, context);
        default: throw new InvalidOperationException($"Unknown task: {job}");
    }
}

var jobs = new List<IJob>();
while(options.HasNext()) jobs.Add(ParseJob(options.Next()));
foreach(var job in jobs) await job.Run(context);
