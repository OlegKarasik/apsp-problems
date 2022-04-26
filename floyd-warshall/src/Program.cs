using BenchmarkDotNet.Running;

using System.Reflection;

namespace Problems
{
  class Program
  {
    static void Main(string[] args)
    {
      BenchmarkSwitcher.FromAssembly(Assembly.GetExecutingAssembly()).Run(args);
    }
  }
}
