﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Code.Utilz;
using System;
using System.Collections.Generic;
using System.IO;

namespace Code.Benchmarks
{
  [HardwareCounters(HardwareCounter.LlcMisses)]
  public class APSP03x1
  {
    public static IEnumerable<string> ValuesForGraph() 
      =>
        [
          "4800"
        ];

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

    [Benchmark(Baseline = true)]
    public void Baseline() 
      => Algorithms.FloydWarshall.Baseline(this.matrix);

    [Benchmark]
    public void VectorOptimisation() 
      => Algorithms.FloydWarshall.VectorOptimisation(this.matrix);

    [Benchmark]
    public void ParallelOptimisation() 
      => Algorithms.FloydWarshall.ParallelOptimisation(this.matrix);

    [Benchmark]
    public void ParallelVectorOptimisations() 
      => Algorithms.FloydWarshall.ParallelVectorOptimisations(this.matrix);
  }
}