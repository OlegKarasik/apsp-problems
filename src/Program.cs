using Spectre.Console.Cli;
using Code.Interface;

namespace Problems
{
  class Program
  {
    static int Main(string[] args)
    {
      var app = new CommandApp();

      app.Configure(config =>
      {
        config.AddBranch<RunSettings>("run", 
          run => 
          {
            run.AddBranch<RunAlgorithmSettings>("algorithm", 
              algorithm => 
              {
                algorithm.AddCommand<RunAlgorithmFloydWarshallCommand>("floyd-warshall");
                algorithm.AddCommand<RunAlgorithmBlockedFloydWarshallCommand>("blocked-floyd-warshall");
              });
            run.AddCommand<RunBenchmarkCommand>("benchmark");
          });
      });

      return app.Run(args);
    }
  }
}
