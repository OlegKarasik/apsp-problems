using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Code.Utilz;
using System;
using System.Collections.Generic;

namespace Code.Benchmarks
{
  [HardwareCounters(HardwareCounter.LlcMisses)]
  public class APSP02x1
  {
    public static IEnumerable<string> ValuesForGraph() 
      => new[] 
        { 
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

    [Benchmark(Baseline = true)]
    public void Baseline() 
      => Algorithms.FloydWarshall.Baseline(this.matrix, this.matrix_size);

    [Benchmark]
    public void VectorOptimisation() 
      => Algorithms.FloydWarshall.VectorOptimisation(this.matrix, this.matrix_size);

    [Benchmark]
    public void ParallelOptimisation() 
      => Algorithms.FloydWarshall.ParallelOptimisation(this.matrix, this.matrix_size);

    [Benchmark]
    public void ParallelVectorOptimisations() 
      => Algorithms.FloydWarshall.ParallelVectorOptimisations(this.matrix, this.matrix_size);
  }
}