using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Spectre.Console.Cli;

namespace Code.Interface;

public class RunBenchmarkCommand : Command<RunBenchmarkSettings>
{
  public override int Execute(
    CommandContext context,
    RunBenchmarkSettings settings)
  {
    var config = ManualConfig.Create(DefaultConfig.Instance)
      .WithOptions(ConfigOptions.JoinSummary);

    BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly())
      .Run(config: config);

    return 0;
  }
}
