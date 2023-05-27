using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Code.Utilz;
using System;
using System.Collections.Generic;

namespace Code.Benchmarks
{
  [HardwareCounters(HardwareCounter.LlcMisses)]
  public class BlockFloydWarshallBenchmarks
  {
    public static IEnumerable<string> ValuesForGraph() 
      => new[] 
        { 
          "1200-575520",
          "2400-2303040",
          "4800-9214080"
        };

    public static IEnumerable<int> ValuesForBlockSize() 
      => new[] 
        { 
          50
        };

    [ParamsSource(nameof(ValuesForGraph))]
    public string Graph;

    [ParamsSource(nameof(ValuesForBlockSize))]
    public int BlockSize;

    private long[] block_matrix;
    private int block_size;
    private int block_count;

    [GlobalSetup]
    public void GlobalSetup()
    {
      var (matrix, matrix_size) = MatrixHelpers.FromInputFile(
        $@"{Environment.CurrentDirectory}/Data/{this.Graph}.input");

      MatrixHelpers.SplitInPlace(matrix, matrix_size, this.BlockSize, out var block_count);

      this.block_matrix = matrix;
      this.block_size = this.BlockSize;
      this.block_count = block_count;
    }

    [Benchmark(Baseline = true)]
    public void Baseline() 
      => Algorithms.BlockFloydWarshall.Baseline(this.block_matrix, this.block_count, this.block_size);

    [Benchmark]
    public void ParallelOptimisation() 
      => Algorithms.BlockFloydWarshall.ParallelOptimisation(this.block_matrix, this.block_count, this.block_size);

    [Benchmark]
    public void VectorOptimisation() 
      => Algorithms.BlockFloydWarshall.VectorOptimisation(this.block_matrix, this.block_count, this.block_size);
    
    [Benchmark]
    public void ParallelVectorOptimisations() 
      => Algorithms.BlockFloydWarshall.ParallelVectorOptimisations(this.block_matrix, this.block_count, this.block_size);
 }
}