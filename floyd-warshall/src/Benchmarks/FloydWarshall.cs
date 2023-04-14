using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Code.Utilz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Code.Benchmarks
{
  [HardwareCounters(HardwareCounter.CacheMisses, HardwareCounter.LlcMisses)]
  public class FloydWarshall
  {
    public static IEnumerable<object[]> Arguments()
    {
      var inputs = new[]
      {
        $@"{Environment.CurrentDirectory}/Data/300-35880.input",
        $@"{Environment.CurrentDirectory}/Data/600-143760.input",
        $@"{Environment.CurrentDirectory}/Data/1200-575520.input",
        $@"{Environment.CurrentDirectory}/Data/2400-2303040.input",
        $@"{Environment.CurrentDirectory}/Data/4800-9214080.input"
      };

      return inputs
        .Select(i =>
          {
            var (matrix, size) = MatrixHelpers.FromInputFile(i);
            return new object[] { matrix, size };
          })
        .ToArray();
    }

    // baseline
    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Arguments))]
    public void FloydWarshall_00(int[] matrix, int sz)
    {
      for (var k = 0; k < sz; ++k)
      {
        for (var i = 0; i < sz; ++i)
        {
          for (var j = 0; j < sz; ++j)
          {
            var distance = matrix[i * sz + k] + matrix[k * sz + j];
            if (matrix[i * sz + j] > distance)
            {
              matrix[i * sz + j] = distance;
            }
          }
        }
      }
    }

    // + graph specific optimization
    [Benchmark]
    [ArgumentsSource(nameof(Arguments))]
    public void FloydWarshall_01(int[] matrix, int sz)
    {
      for (var k = 0; k < sz; ++k)
      {
        for (var i = 0; i < sz; ++i)
        {
          if (matrix[i * sz + k] == Constants.NO_EDGE)
          {
            continue;
          }
          for (var j = 0; j < sz; ++j)
          {
            var distance = matrix[i * sz + k] + matrix[k * sz + j];
            if (matrix[i * sz + j] > distance)
            {
              matrix[i * sz + j] = distance;
            }
          }
        }
      }
    }

    // + graph specific optimization
    // + parallel
    [Benchmark]
    [ArgumentsSource(nameof(Arguments))]
    public void FloydWarshall_02(int[] matrix, int sz)
    {
      for (var k = 0; k < sz; ++k)
      {
        Parallel.For(0, sz, i =>
        {
          if (matrix[i * sz + k] == Constants.NO_EDGE)
          {
            return;
          }
          for (var j = 0; j < sz; ++j)
          {
            var distance = matrix[i * sz + k] + matrix[k * sz + j];
            if (matrix[i * sz + j] > distance)
            {
              matrix[i * sz + j] = distance;
            }
          }
        });
      }
    }

    // + graph specific optimization
    // + vectorization
    [Benchmark]
    [ArgumentsSource(nameof(Arguments))]
    public void FloydWarshall_03(int[] matrix, int sz)
    {
      for (var k = 0; k < sz; ++k)
      {
        for (var i = 0; i < sz; ++i)
        {
          if (matrix[i * sz + k] == Constants.NO_EDGE)
          {
            continue;
          }

          var ik_vec = new Vector<int>(matrix[i * sz + k]);

          var j = 0;
          for (; j < sz - Vector<int>.Count; j += Vector<int>.Count)
          {
            var ij_vec = new Vector<int>(matrix, i * sz + j);
            var ikj_vec = new Vector<int>(matrix, k * sz + j) + ik_vec;

            var lt_vec = Vector.LessThan(ij_vec, ikj_vec);
            if (lt_vec == new Vector<int>(-1))
            {
              continue;
            }

            var r_vec = Vector.ConditionalSelect(lt_vec, ij_vec, ikj_vec);
            r_vec.CopyTo(matrix, i * sz + j);
          }

          for (; j < sz; ++j)
          {
            var distance = matrix[i * sz + k] + matrix[k * sz + j];
            if (matrix[i * sz + j] > distance)
            {
              matrix[i * sz + j] = distance;
            }
          }
        }
      }
    }

    // + graph specific optimization
    // + vectorization
    // + parallel
    [Benchmark]
    [ArgumentsSource(nameof(Arguments))]
    public void FloydWarshall_04(int[] matrix, int sz)
    {
      for (var k = 0; k < sz; ++k)
      {
        Parallel.For(0, sz, i =>
        {
          if (matrix[i * sz + k] == Constants.NO_EDGE)
          {
            return;
          }

          var ik_vec = new Vector<int>(matrix[i * sz + k]);

          var j = 0;
          for (; j < sz - Vector<int>.Count; j += Vector<int>.Count)
          {
            var ij_vec = new Vector<int>(matrix, i * sz + j);
            var ikj_vec = new Vector<int>(matrix, k * sz + j) + ik_vec;

            var lt_vec = Vector.LessThan(ij_vec, ikj_vec);
            if (lt_vec == new Vector<int>(-1))
            {
              continue;
            }

            var r_vec = Vector.ConditionalSelect(lt_vec, ij_vec, ikj_vec);
            r_vec.CopyTo(matrix, i * sz + j);
          }

          for (; j < sz; ++j)
          {
            var distance = matrix[i * sz + k] + matrix[k * sz + j];
            if (matrix[i * sz + j] > distance)
            {
              matrix[i * sz + j] = distance;
            }
          }
        });
      }
    }
  }
}