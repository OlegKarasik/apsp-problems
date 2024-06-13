﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Code.Utilz;
using System;
using System.Collections.Generic;
using System.IO;

namespace Code.Benchmarks
{
  [HardwareCounters(HardwareCounter.LlcMisses)]
  public class APSP03x2
  {
    public static IEnumerable<string> ValuesForGraph() 
      => new[] 
        { 
          //"1200-575520",
          //"2400-2303040",
          "4800-9214080"
        };

    public static IEnumerable<int> ValuesForBlockSize() 
      => new[] 
        { 
          120
        };

    [ParamsSource(nameof(ValuesForGraph))]
    public string Graph;

    [ParamsSource(nameof(ValuesForBlockSize))]
    public int BlockSize;

    private Matrix.Blocks blocks;

    [GlobalSetup]
    public void GlobalSetup()
    {
      using var inputStream = new FileStream(
        $@"{Environment.CurrentDirectory}/Data/{this.Graph}.input", FileMode.Open, FileAccess.Read, FileShare.Read);

      this.blocks = Matrix.Read(inputStream).SplitInBlocks(this.BlockSize);
    }

    //[Benchmark(Baseline = true)]
    public void Baseline() 
      => Algorithms.BlockedFloydWarshall.Baseline(this.blocks);

    [Benchmark]
    public void ParallelOptimisation() 
      => Algorithms.BlockedFloydWarshall.ParallelOptimisation(this.blocks);
    [Benchmark]
    public void ParallelOptimisation1() 
      => Algorithms.BlockedFloydWarshall.ParallelOptimisation(this.blocks);
    [Benchmark]
    public void ParallelOptimisation2() 
      => Algorithms.BlockedFloydWarshall.ParallelOptimisation(this.blocks);

    //[Benchmark]
    public void VectorOptimisation() 
      => Algorithms.BlockedFloydWarshall.VectorOptimisation(this.blocks);
    
    [Benchmark]
    public void ParallelVectorOptimisations() 
      => Algorithms.BlockedFloydWarshall.ParallelVectorOptimisations(this.blocks);
 }
}