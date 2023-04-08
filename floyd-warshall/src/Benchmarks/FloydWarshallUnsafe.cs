using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Code.Utilz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Code.Benchmarks
{
  [HardwareCounters(HardwareCounter.CacheMisses, HardwareCounter.BranchMispredictions, HardwareCounter.LlcMisses)]
  public class FloydWarshallUnsafe
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
            ref int ij_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
            ij_ref = ref Unsafe.Add(ref ij_ref, i * sz + j);

            ref int ik_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
            ik_ref = ref Unsafe.Add(ref ik_ref, i * sz + k);

            ref int kj_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
            kj_ref = ref Unsafe.Add(ref kj_ref, k * sz + j);
       
            var distance = ik_ref + kj_ref;
            if (ij_ref > distance)
            {
              ij_ref = distance;
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
          ref int ik_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
          ik_ref = ref Unsafe.Add(ref ik_ref, i * sz + k);

          if (ik_ref == Constants.NO_EDGE)
          {
            continue;
          }
          for (var j = 0; j < sz; ++j)
          {
            ref int ij_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
            ij_ref = ref Unsafe.Add(ref ij_ref, i * sz + j);

            ref int kj_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
            kj_ref = ref Unsafe.Add(ref kj_ref, k * sz + j);
       
            var distance = ik_ref + kj_ref;
            if (ij_ref > distance)
            {
              ij_ref = distance;
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
          ref int ik_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
          ik_ref = ref Unsafe.Add(ref ik_ref, i * sz + k);

          if (ik_ref == Constants.NO_EDGE)
          {
            return;
          }
          for (var j = 0; j < sz; ++j)
          {
            ref int ij_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
            ij_ref = ref Unsafe.Add(ref ij_ref, i * sz + j);

            ref int kj_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
            kj_ref = ref Unsafe.Add(ref kj_ref, k * sz + j);
       
            var distance = ik_ref + kj_ref;
            if (ij_ref > distance)
            {
              ij_ref = distance;
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
          ref int ik_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
          ik_ref = ref Unsafe.Add(ref ik_ref, i * sz + k);

          if (ik_ref == Constants.NO_EDGE)
          {
            continue;
          }

          var ik_vec = new Vector<int>(ik_ref);

          var j = 0;
          for (; j < sz - Vector<int>.Count; j += Vector<int>.Count)
          {
            ref int ij_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
            ij_ref = ref Unsafe.Add(ref ij_ref, i * sz + j);

            ref int kj_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
            kj_ref = ref Unsafe.Add(ref kj_ref, k * sz + j);

            var ij_vec = new Vector<int>(MemoryMarshal.CreateSpan(ref ij_ref, Vector<int>.Count));
            var ikj_vec = new Vector<int>(MemoryMarshal.CreateSpan(ref kj_ref, Vector<int>.Count)) + ik_vec;

            var lt_vec = Vector.LessThan(ij_vec, ikj_vec);
            if (lt_vec == new Vector<int>(-1))
            {
              continue;
            }

            var r_vec = Vector.ConditionalSelect(lt_vec, ij_vec, ikj_vec);
            r_vec.CopyTo(MemoryMarshal.CreateSpan(ref ij_ref, Vector<int>.Count));
          }

          for (; j < sz; ++j)
          {
            ref int _ij_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
            _ij_ref = ref Unsafe.Add(ref _ij_ref, i * sz + j);

            ref int _ik_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
            _ik_ref = ref Unsafe.Add(ref _ik_ref, i * sz + k);

            ref int _kj_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
            _kj_ref = ref Unsafe.Add(ref _kj_ref, k * sz + j);
       
            var distance = _ik_ref + _kj_ref;
            if (_ij_ref > distance)
            {
              _ij_ref = distance;
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
          ref int ik_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
          ik_ref = ref Unsafe.Add(ref ik_ref, i * sz + k);

          if (ik_ref == Constants.NO_EDGE)
          {
            return;
          }

          var ik_vec = new Vector<int>(ik_ref);

          var j = 0;
          for (; j < sz - Vector<int>.Count; j += Vector<int>.Count)
          {
            ref int ij_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
            ij_ref = ref Unsafe.Add(ref ij_ref, i * sz + j);

            ref int kj_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
            kj_ref = ref Unsafe.Add(ref kj_ref, k * sz + j);

            var ij_vec = new Vector<int>(MemoryMarshal.CreateSpan(ref ij_ref, Vector<int>.Count));
            var ikj_vec = new Vector<int>(MemoryMarshal.CreateSpan(ref kj_ref, Vector<int>.Count)) + ik_vec;

            var lt_vec = Vector.LessThan(ij_vec, ikj_vec);
            if (lt_vec == new Vector<int>(-1))
            {
              continue;
            }

            var r_vec = Vector.ConditionalSelect(lt_vec, ij_vec, ikj_vec);
            r_vec.CopyTo(MemoryMarshal.CreateSpan(ref ij_ref, Vector<int>.Count));
          }

          for (; j < sz; ++j)
          {
            ref int _ij_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
            _ij_ref = ref Unsafe.Add(ref _ij_ref, i * sz + j);

            ref int _ik_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
            _ik_ref = ref Unsafe.Add(ref _ik_ref, i * sz + k);

            ref int _kj_ref = ref MemoryMarshal.GetArrayDataReference(matrix);
            _kj_ref = ref Unsafe.Add(ref _kj_ref, k * sz + j);
       
            var distance = _ik_ref + _kj_ref;
            if (_ij_ref > distance)
            {
              _ij_ref = distance;
            }
          }
        });
      }
    }
  }
}