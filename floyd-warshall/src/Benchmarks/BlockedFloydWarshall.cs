using BenchmarkDotNet.Attributes;
using Code.Utilz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Code.Benchmarks
{
  [MemoryDiagnoser]
  public class BlockFloydWarshall
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
      var blocks = new[]
      {
        50
      };

      return inputs
        .SelectMany(i =>
          {
            var (matrix, size) = MatrixHelpers.FromInputFile(i);
            return blocks.Select(j => 
            {
              var (block_matrix, block_count, block_size) = BlockMatrixHelpers.ConvertMatrixToBlockMatrix(matrix, size, j);
              return new object[] { block_matrix, block_count, block_size };
            });
          })
        .ToArray();
    }

    private static void BlockFloydWarshall_00_Procedure(
      Span<int> ij, Span<int> ik, Span<int> kj, int block_size)
    {
      for (var k = 0; k < block_size; ++k)
      {
        for (var i = 0; i < block_size; ++i)
        {
          for (var j = 0; j < block_size; ++j)
          {
            var distance = ik[i * block_size + k] + kj[k * block_size + j];
            if (ij[i * block_size + j] > distance)
            {
              ij[i * block_size + j] = distance;
            }
          }
        }
      }
    }

    // baseline
    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(Arguments))]
    public void BlockFloydWarshall_00(int[] matrix, int block_count, int block_size)
    {
      var sz_block = block_size * block_size;
      var sz_block_row = block_count * sz_block;

      for (var m = 0; m < block_count; ++m) 
      {
        var offset_mm = m * sz_block_row + m * sz_block;

        var mm = new Span<int>(matrix, offset_mm, sz_block);

        BlockFloydWarshall_00_Procedure(mm, mm, mm, block_size);

        for (var i = 0; i < block_count; ++i) 
        {
          if (i != m) 
          {
            var offset_im = i * sz_block_row + m * sz_block;
            var offset_mi = m * sz_block_row + i * sz_block;
        
            var im = new Span<int>(matrix, offset_im, sz_block);
            var mi = new Span<int>(matrix, offset_mi, sz_block);

            BlockFloydWarshall_00_Procedure(im, im, mm, block_size);
            BlockFloydWarshall_00_Procedure(mi, mm, mi, block_size);
          }
        }
        for (var i = 0; i < block_count; ++i) 
        {
          if (i != m) 
          {
            var offset_im = i * sz_block_row + m * sz_block;

            var im = new Span<int>(matrix, offset_im, sz_block);

            for (var j = 0; j < block_count; ++j) 
            {
              if (j != m) 
              {
                var offset_ij = i * sz_block_row + j * sz_block;
                var offset_mj = m * sz_block_row + j * sz_block;
        
                var ij = new Span<int>(matrix, offset_ij, sz_block);
                var mj = new Span<int>(matrix, offset_mj, sz_block);

                BlockFloydWarshall_00_Procedure(ij, im, mj, block_size);
              }
            }
          }
        }
      }
    }

    private static void BlockFloydWarshall_01_Procedure(
      Span<int> ij, Span<int> ik, Span<int> kj, int block_size)
    {
      for (var k = 0; k < block_size; ++k)
      {
        for (var i = 0; i < block_size; ++i)
        {
          if (ik[i * block_size + k] == Constants.NO_EDGE)
          {
            continue;
          }
          for (var j = 0; j < block_size; ++j)
          {
            var distance = ik[i * block_size + k] + kj[k * block_size + j];
            if (ij[i * block_size + j] > distance)
            {
              ij[i * block_size + j] = distance;
            }
          }
        }
      }
    }

    // + graph specific optimization
    [Benchmark]
    [ArgumentsSource(nameof(Arguments))]
    public void BlockFloydWarshall_01(int[] matrix, int block_count, int block_size)
    {
      var sz_block = block_size * block_size;
      var sz_block_row = block_count * sz_block;

      for (var m = 0; m < block_count; ++m) 
      {
        var offset_mm = m * sz_block_row + m * sz_block;

        var mm = new Span<int>(matrix, offset_mm, sz_block);

        BlockFloydWarshall_01_Procedure(mm, mm, mm, block_size);

        for (var i = 0; i < block_count; ++i) 
        {
          if (i != m) 
          {
            var offset_im = i * sz_block_row + m * sz_block;
            var offset_mi = m * sz_block_row + i * sz_block;
        
            var im = new Span<int>(matrix, offset_im, sz_block);
            var mi = new Span<int>(matrix, offset_mi, sz_block);

            BlockFloydWarshall_01_Procedure(im, im, mm, block_size);
            BlockFloydWarshall_01_Procedure(mi, mm, mi, block_size);
          }
        }
        for (var i = 0; i < block_count; ++i) 
        {
          if (i != m) 
          {
            var offset_im = i * sz_block_row + m * sz_block;

            var im = new Span<int>(matrix, offset_im, sz_block);

            for (var j = 0; j < block_count; ++j) 
            {
              if (j != m) 
              {
                var offset_ij = i * sz_block_row + j * sz_block;
                var offset_mj = m * sz_block_row + j * sz_block;
        
                var ij = new Span<int>(matrix, offset_ij, sz_block);
                var mj = new Span<int>(matrix, offset_mj, sz_block);

                BlockFloydWarshall_01_Procedure(ij, im, mj, block_size);
              }
            }
          }
        }
      }
    }

    // + graph specific optimization
    // + parallel
    [Benchmark]
    [ArgumentsSource(nameof(Arguments))]
    public void BlockFloydWarshall_02(int[] matrix, int block_count, int block_size)
    {
      var sz_block = block_size * block_size;
      var sz_block_row = block_count * sz_block;

      for (var m = 0; m < block_count; ++m) 
      {
        var offset_mm = m * sz_block_row + m * sz_block;

        var mm = new Span<int>(matrix, offset_mm, sz_block);

        BlockFloydWarshall_01_Procedure(mm, mm, mm, block_size);

        Task.Run(() => 
        {
          for (var i = 0; i < block_count; ++i) 
          {
            if (i != m) 
            {
              var offset_im = i * sz_block_row + m * sz_block;
              var offset_mi = m * sz_block_row + i * sz_block;
          
              new Task(() => {
                var mm = new Span<int>(matrix, offset_mm, sz_block);
                var im = new Span<int>(matrix, offset_im, sz_block);
                var mi = new Span<int>(matrix, offset_mi, sz_block);

                BlockFloydWarshall_01_Procedure(im, im, mm, block_size);
              }, 
              TaskCreationOptions.AttachedToParent).Start();

              new Task(() => {
                var mm = new Span<int>(matrix, offset_mm, sz_block);
                var im = new Span<int>(matrix, offset_im, sz_block);
                var mi = new Span<int>(matrix, offset_mi, sz_block);

                BlockFloydWarshall_01_Procedure(mi, mm, mi, block_size);
              }, 
              TaskCreationOptions.AttachedToParent).Start();
            }
          }
        })
        .Wait();

        Task.Run(() => 
        {
          for (var i = 0; i < block_count; ++i) 
          {
            if (i != m) 
            {
              var offset_im = i * sz_block_row + m * sz_block;
              for (var j = 0; j < block_count; ++j) 
              {
                if (j != m) 
                {
                  var offset_ij = i * sz_block_row + j * sz_block;
                  var offset_mj = m * sz_block_row + j * sz_block;

                  new Task(() => {
                    var im = new Span<int>(matrix, offset_im, sz_block);
                    var ij = new Span<int>(matrix, offset_ij, sz_block);
                    var mj = new Span<int>(matrix, offset_mj, sz_block);

                    BlockFloydWarshall_01_Procedure(ij, im, mj, block_size);
                  }, 
                  TaskCreationOptions.AttachedToParent).Start();
                }
              }
            }
          }
        })
        .Wait();
      }
    }

    // // + graph specific optimization
    // // + parallel
    // [Benchmark]
    // [ArgumentsSource(nameof(Arguments))]
    // public void FloydWarshall_02(int[] matrix, int sz)
    // {
    //   for (var k = 0; k < sz; ++k)
    //   {
    //     Parallel.For(0, sz, i =>
    //     {
    //       if (matrix[i * sz + k] == Constants.NO_EDGE)
    //       {
    //         return;
    //       }
    //       for (var j = 0; j < sz; ++j)
    //       {
    //         var distance = matrix[i * sz + k] + matrix[k * sz + j];
    //         if (matrix[i * sz + j] > distance)
    //         {
    //           matrix[i * sz + j] = distance;
    //         }
    //       }
    //     });
    //   }
    // }

    // // + graph specific optimization
    // // + vectorization
    // [Benchmark]
    // [ArgumentsSource(nameof(Arguments))]
    // public void FloydWarshall_03(int[] matrix, int sz)
    // {
    //   for (var k = 0; k < sz; ++k)
    //   {
    //     for (var i = 0; i < sz; ++i)
    //     {
    //       if (matrix[i * sz + k] == Constants.NO_EDGE)
    //       {
    //         continue;
    //       }

    //       var ik_vec = new Vector<int>(matrix[i * sz + k]);

    //       var j = 0;
    //       for (; j < sz - Vector<int>.Count; j += Vector<int>.Count)
    //       {
    //         var ij_vec = new Vector<int>(matrix, i * sz + j);
    //         var ikj_vec = new Vector<int>(matrix, k * sz + j) + ik_vec;

    //         var lt_vec = Vector.LessThan(ij_vec, ikj_vec);
    //         if (lt_vec == new Vector<int>(-1))
    //         {
    //           continue;
    //         }

    //         var r_vec = Vector.ConditionalSelect(lt_vec, ij_vec, ikj_vec);
    //         r_vec.CopyTo(matrix, i * sz + j);
    //       }

    //       for (; j < sz; ++j)
    //       {
    //         var distance = matrix[i * sz + k] + matrix[k * sz + j];
    //         if (matrix[i * sz + j] > distance)
    //         {
    //           matrix[i * sz + j] = distance;
    //         }
    //       }
    //     }
    //   }
    // }

    // // + graph specific optimization
    // // + vectorization
    // // + parallel
    // [Benchmark]
    // [ArgumentsSource(nameof(Arguments))]
    // public void FloydWarshall_04(int[] matrix, int sz)
    // {
    //   for (var k = 0; k < sz; ++k)
    //   {
    //     Parallel.For(0, sz, i =>
    //     {
    //       if (matrix[i * sz + k] == Constants.NO_EDGE)
    //       {
    //         return;
    //       }

    //       var ik_vec = new Vector<int>(matrix[i * sz + k]);

    //       var j = 0;
    //       for (; j < sz - Vector<int>.Count; j += Vector<int>.Count)
    //       {
    //         var ij_vec = new Vector<int>(matrix, i * sz + j);
    //         var ikj_vec = new Vector<int>(matrix, k * sz + j) + ik_vec;

    //         var lt_vec = Vector.LessThan(ij_vec, ikj_vec);
    //         if (lt_vec == new Vector<int>(-1))
    //         {
    //           continue;
    //         }

    //         var r_vec = Vector.ConditionalSelect(lt_vec, ij_vec, ikj_vec);
    //         r_vec.CopyTo(matrix, i * sz + j);
    //       }

    //       for (; j < sz; ++j)
    //       {
    //         var distance = matrix[i * sz + k] + matrix[k * sz + j];
    //         if (matrix[i * sz + j] > distance)
    //         {
    //           matrix[i * sz + j] = distance;
    //         }
    //       }
    //     });
    //   }
    // }
  }
}