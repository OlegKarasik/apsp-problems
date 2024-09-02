using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Code.Utilz;
using System;
using System.Collections.Generic;
using System.IO;

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

    private Matrix matrix;
    private Matrix routes;

    [GlobalSetup]
    public void GlobalSetup()
    {
      using var inputStream = new FileStream(
        $@"{Environment.CurrentDirectory}/Data/{this.Graph}.input", FileMode.Open, FileAccess.Read, FileShare.Read);

      this.matrix = Matrix.Read(inputStream);
      this.routes = Matrix.Default(this.matrix.Size);
    }

    // aka FloydWarshallRoutes_00
    //
    [Benchmark(Baseline = true)]
    public void BaselineWithRoutes() 
      => Algorithms.FloydWarshall.BaselineWithRoutes(this.matrix, this.routes);

    // aka FloydWarshallRoutes_01
    //
    [Benchmark]
    public void SpartialVectorOptimisationsWithRoutes() 
      => Algorithms.FloydWarshall.SpartialVectorOptimisationsWithRoutes(this.matrix, this.routes);
  }
}