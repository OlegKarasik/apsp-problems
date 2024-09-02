using System.Numerics;
using System.Threading.Tasks;
using Code.Utilz;

namespace Code.Algorithms;

public static class FloydWarshall
{
  public static void Baseline(Matrix matrix)
    => Baseline(matrix.Data, matrix.Size);

  public static void SpartialOptimisation(Matrix matrix)
    => SpartialOptimisation(matrix.Data, matrix.Size);

  public static void SpartialParallelOptimisations(Matrix matrix)
    => SpartialParallelOptimisations(matrix.Data, matrix.Size);

  public static void SpartialVectorOptimisations(Matrix matrix)
    => SpartialVectorOptimisations(matrix.Data, matrix.Size);

  public static void SpartialParallelVectorOptimisations(Matrix matrix)
    => SpartialParallelVectorOptimisations(matrix.Data, matrix.Size);

  public static void ParallelOptimisation(Matrix matrix)
    => ParallelOptimisation(matrix.Data, matrix.Size);

  public static void VectorOptimisation(Matrix matrix)
    => VectorOptimisation(matrix.Data, matrix.Size);

  public static void ParallelVectorOptimisations(Matrix matrix)
    => ParallelVectorOptimisations(matrix.Data, matrix.Size);

  public static void BaselineWithRoutes(Matrix matrix, Matrix routes)
    => BaselineWithRoutes(matrix.Data, routes.Data, matrix.Size);

  public static void SpartialVectorOptimisationsWithRoutes(Matrix matrix, Matrix routes)
    => SpartialVectorOptimisationsWithRoutes(matrix.Data, routes.Data, matrix.Size);

  private static void Baseline(int[] matrix, int sz)
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

  private static void SpartialOptimisation(int[] matrix, int sz)
  {
    for (var k = 0; k < sz; ++k)
    {
      for (var i = 0; i < sz; ++i)
      {
        if (matrix[i * sz + k] == Matrix.NO_EDGE)
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

  private static void SpartialParallelOptimisations(int[] matrix, int sz)
  {
    for (var k = 0; k < sz; ++k)
    {
      Parallel.For(0, sz, i =>
      {
        if (matrix[i * sz + k] == Matrix.NO_EDGE)
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

  private static void SpartialVectorOptimisations(int[] matrix, int sz)
  {
    for (var k = 0; k < sz; ++k)
    {
      for (var i = 0; i < sz; ++i)
      {
        if (matrix[i * sz + k] == Matrix.NO_EDGE)
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

  private static void SpartialParallelVectorOptimisations(int[] matrix, int sz)
  {
    for (var k = 0; k < sz; ++k)
    {
      Parallel.For(0, sz, i =>
      {
        if (matrix[i * sz + k] == Matrix.NO_EDGE)
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

  private static void ParallelOptimisation(int[] matrix, int sz)
  {
    for (var k = 0; k < sz; ++k)
    {
      Parallel.For(0, sz, i =>
      {
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

  private static void VectorOptimisation(int[] matrix, int sz)
  {
    for (var k = 0; k < sz; ++k)
    {
      for (var i = 0; i < sz; ++i)
      {
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

  private static void ParallelVectorOptimisations(int[] matrix, int sz)
  {
    for (var k = 0; k < sz; ++k)
    {
      Parallel.For(0, sz, i =>
      {
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

  private static void BaselineWithRoutes(int[] matrix, int[] routes, int sz)
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

  private static void SpartialVectorOptimisationsWithRoutes(int[] matrix, int[] routes, int sz)
  {
    for (var k = 0; k < sz; ++k)
    {
      var k_vec = new Vector<int>(k);

      for (var i = 0; i < sz; ++i)
      {
        if (matrix[i * sz + k] == Matrix.NO_EDGE)
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

          var rk_vec = Vector.ConditionalSelect(lt_vec, new Vector<int>(routes, i * sz + j), k_vec);
          rk_vec.CopyTo(routes, i * sz + j);
        }

        for (; j < sz; ++j)
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
}