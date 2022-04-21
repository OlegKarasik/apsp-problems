using BenchmarkDotNet.Attributes;
using Code.Utilz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Code.Benchmarks
{
  public class FloydWarshallRoutes
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
            var routes = new int[size * size];
            return new object[] { matrix, routes, size };
          })
        .ToArray();
    }

    // baseline
    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Arguments))]
    public void FloydWarshallRoutes_00(int[] matrix, int[] routes, int sz)
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
              routes[i * sz + j] = k;
            }
          }
        }
      }
    }

    // + vectorization
    [Benchmark]
    [ArgumentsSource(nameof(Arguments))]
    public void FloydWarshallRoutes_01(int[] matrix, int[] routes, int sz)
    {
      for (var k = 0; k < sz; ++k)
      {
        var k_vec = new Vector<int>(k);

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

            var rk_vec = Vector.ConditionalSelect(lt_vec, ij_vec, k_vec);
            rk_vec.CopyTo(routes, i * sz + j);
          }

          for (; j < sz; ++j)
          {
            var distance = matrix[i * sz + k] + matrix[k * sz + j];
            if (matrix[i * sz + j] > distance)
            {
              matrix[i * sz + j] = distance;
              routes[i * sz + j] = distance;
            }
          }
        }
      }
    }
  }
}