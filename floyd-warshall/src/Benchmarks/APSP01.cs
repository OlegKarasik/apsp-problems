using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Code.Utilz;
using System;
using System.Collections.Generic;

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

    private long[] matrix;
    private int matrix_size;

    [GlobalSetup]
    public void GlobalSetup()
    {
      var (matrix, matrix_size) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{this.Graph}.input");

      this.matrix = matrix;
      this.matrix_size = matrix_size;
    }

    // aka FloydWarshall_00
    //
    [Benchmark(Baseline = true)]
    public void Baseline() 
      => Algorithms.FloydWarshall.Baseline(this.matrix, this.matrix_size);

    // aka FloydWarshall_01
    //
    [Benchmark]
    public void SpartialOptimisation() 
      => Algorithms.FloydWarshall.SpartialOptimisation(this.matrix, this.matrix_size);

    // aka FloydWarshall_02
    //
    [Benchmark]
    public void SpartialParallelOptimisations() 
      => Algorithms.FloydWarshall.SpartialParallelOptimisations(this.matrix, this.matrix_size);

    // aka FloydWarshall_03
    //
    [Benchmark]
    public void SpartialVectorOptimisations() 
      => Algorithms.FloydWarshall.SpartialVectorOptimisations(this.matrix, this.matrix_size);

    // aka FloydWarshall_04
    //
    [Benchmark]
    public void SpartialParallelVectorOptimisations() 
      => Algorithms.FloydWarshall.SpartialParallelVectorOptimisations(this.matrix, this.matrix_size);
  }
}