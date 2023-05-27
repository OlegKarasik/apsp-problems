using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Code.Utilz;
using System;
using System.Collections.Generic;

namespace Code.Benchmarks
{
  [HardwareCounters(HardwareCounter.LlcMisses)]
  public class FloydWarshallWithRoutesBenchmarks
  {
    public static IEnumerable<string> ValuesForGraph() 
      => new[] 
        { 
          "1200-575520"
        };

    [ParamsSource(nameof(ValuesForGraph))]
    public string Graph;

    private long[] matrix;
    private long[] matrix_routes;
    private int matrix_size;

    [GlobalSetup]
    public void GlobalSetup()
    {
      var (matrix, matrix_size) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{this.Graph}.input");

      var (matrix_routes, _) = MatrixHelpers.Initialize(matrix_size);

      this.matrix = matrix;
      this.matrix_size = matrix_size;
      this.matrix_routes = matrix_routes;
    }

    [Benchmark(Baseline = true)]
    public void BaselineWithRoutes() 
      => Algorithms.FloydWarshall.BaselineWithRoutes(this.matrix, this.matrix_routes, this.matrix_size);

    [Benchmark]
    public void SpartialVectorOptimisationsWithRoutes() 
      => Algorithms.FloydWarshall.SpartialVectorOptimisationsWithRoutes(this.matrix, this.matrix_routes, this.matrix_size);
  }
}