using System.Diagnostics;
using System.Reflection;

namespace HugeMazes.Web.Status;

public static class StatusObtainer
{
    public static StatusModel GetStatus()
    {
        var assemblyName = Assembly.GetExecutingAssembly().GetName();
        return new(
            assemblyName.Name!,
            assemblyName.Version!.ToString(),
            (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString());
    }
}
