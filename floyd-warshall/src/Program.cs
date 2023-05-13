using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

using System.Reflection;

namespace Problems
{
  class Program
  {
    static void Main(string[] args)
    {
      var config = ManualConfig.Create(DefaultConfig.Instance)
        .WithOptions(ConfigOptions.JoinSummary);

      BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly())
        .Run(args, config);
    }
  }
}
