using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Code.Utilz;
using System;
using System.Collections.Generic;

namespace Code.Benchmarks
{
  public class APSP02
  {
    public static IEnumerable<string> ValuesForGraph() 
      => new[] 
        { 
          "300-35880",
          "600-143760",
          "1200-575520",
          "2400-2303040",
          "4800-9214080"
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

    // aka FloydWarshallRoutes_00
    //
    [Benchmark(Baseline = true)]
    public void BaselineWithRoutes() 
      => Algorithms.FloydWarshall.BaselineWithRoutes(this.matrix, this.matrix_routes, this.matrix_size);

    // aka FloydWarshallRoutes_01
    //
    [Benchmark]
    public void SpartialVectorOptimisationsWithRoutes() 
      => Algorithms.FloydWarshall.SpartialVectorOptimisationsWithRoutes(this.matrix, this.matrix_routes, this.matrix_size);
  }
}