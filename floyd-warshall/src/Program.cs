using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Code.Utilz;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

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
