using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Code.Utilz;
using System;
using System.Collections.Generic;
using System.IO;

namespace Code.Benchmarks
{
  public class APSP01
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

    [GlobalSetup]
    public void GlobalSetup()
    {
      using var inputStream = new FileStream(
        $@"{Environment.CurrentDirectory}/Data/{this.Graph}.input", FileMode.Open, FileAccess.Read, FileShare.Read);

      this.matrix = Matrix.Read(inputStream);
    }

    // aka FloydWarshall_00
    //
    [Benchmark(Baseline = true)]
    public void Baseline() 
      => Algorithms.FloydWarshall.Baseline(this.matrix);

    // aka FloydWarshall_01
    //
    [Benchmark]
    public void SpartialOptimisation() 
      => Algorithms.FloydWarshall.SpartialOptimisation(this.matrix);

    // aka FloydWarshall_02
    //
    [Benchmark]
    public void SpartialParallelOptimisations() 
      => Algorithms.FloydWarshall.SpartialParallelOptimisations(this.matrix);

    // aka FloydWarshall_03
    //
    [Benchmark]
    public void SpartialVectorOptimisations() 
      => Algorithms.FloydWarshall.SpartialVectorOptimisations(this.matrix);

    // aka FloydWarshall_04
    //
    [Benchmark]
    public void SpartialParallelVectorOptimisations() 
      => Algorithms.FloydWarshall.SpartialParallelVectorOptimisations(this.matrix);
  }
}