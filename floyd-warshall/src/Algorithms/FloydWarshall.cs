using System.Numerics;
using System.Threading.Tasks;
using Code.Utilz;

namespace Code.Algorithms
{
  public static class FloydWarshall
  {
    public static void Baseline(long[] matrix, int sz)
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

    public static void SpartialOptimisation(long[] matrix, int sz)
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
  
    public static void SpartialParallelOptimisations(long[] matrix, int sz)
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

    public static void SpartialVectorOptimisations(long[] matrix, int sz)
    {
      for (var k = 0; k < sz; ++k)
      {
        for (var i = 0; i < sz; ++i)
        {
          if (matrix[i * sz + k] == Constants.NO_EDGE)
          {
            continue;
          }

          var ik_vec = new Vector<long>(matrix[i * sz + k]);

          var j = 0;
          for (; j < sz - Vector<long>.Count; j += Vector<long>.Count)
          {
            var ij_vec = new Vector<long>(matrix, i * sz + j);
            var ikj_vec = new Vector<long>(matrix, k * sz + j) + ik_vec;

            var lt_vec = Vector.LessThan(ij_vec, ikj_vec);
            if (lt_vec == new Vector<long>(-1))
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

    public static void SpartialParallelVectorOptimisations(long[] matrix, int sz)
    {
      for (var k = 0; k < sz; ++k)
      {
        Parallel.For(0, sz, i =>
        {
          if (matrix[i * sz + k] == Constants.NO_EDGE)
          {
            return;
          }

          var ik_vec = new Vector<long>(matrix[i * sz + k]);

          var j = 0;
          for (; j < sz - Vector<long>.Count; j += Vector<long>.Count)
          {
            var ij_vec = new Vector<long>(matrix, i * sz + j);
            var ikj_vec = new Vector<long>(matrix, k * sz + j) + ik_vec;

            var lt_vec = Vector.LessThan(ij_vec, ikj_vec);
            if (lt_vec == new Vector<long>(-1))
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

    public static void ParallelOptimisation(long[] matrix, int sz)
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
  
    public static void VectorOptimisation(long[] matrix, int sz)
    {
      for (var k = 0; k < sz; ++k)
      {
        for (var i = 0; i < sz; ++i)
        {
          var ik_vec = new Vector<long>(matrix[i * sz + k]);

          var j = 0;
          for (; j < sz - Vector<long>.Count; j += Vector<long>.Count)
          {
            var ij_vec = new Vector<long>(matrix, i * sz + j);
            var ikj_vec = new Vector<long>(matrix, k * sz + j) + ik_vec;

            var lt_vec = Vector.LessThan(ij_vec, ikj_vec);
            if (lt_vec == new Vector<long>(-1))
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

    public static void ParallelVectorOptimisations(long[] matrix, int sz)
    {
      for (var k = 0; k < sz; ++k)
      {
        Parallel.For(0, sz, i =>
        {
          var ik_vec = new Vector<long>(matrix[i * sz + k]);

          var j = 0;
          for (; j < sz - Vector<long>.Count; j += Vector<long>.Count)
          {
            var ij_vec = new Vector<long>(matrix, i * sz + j);
            var ikj_vec = new Vector<long>(matrix, k * sz + j) + ik_vec;

            var lt_vec = Vector.LessThan(ij_vec, ikj_vec);
            if (lt_vec == new Vector<long>(-1))
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

    public static void BaselineWithRoutes(long[] matrix, long[] routes, int sz)
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
  
    public static void SpartialVectorOptimisationsWithRoutes(long[] matrix, long[] routes, int sz)
    {
      for (var k = 0; k < sz; ++k)
      {
        var k_vec = new Vector<long>(k);

        for (var i = 0; i < sz; ++i)
        {
          if (matrix[i * sz + k] == Constants.NO_EDGE)
          {
            continue;
          }

          var ik_vec = new Vector<long>(matrix[i * sz + k]);

          var j = 0;
          for (; j < sz - Vector<long>.Count; j += Vector<long>.Count)
          {
            var ij_vec = new Vector<long>(matrix, i * sz + j);
            var ikj_vec = new Vector<long>(matrix, k * sz + j) + ik_vec;

            var lt_vec = Vector.LessThan(ij_vec, ikj_vec);
            if (lt_vec == new Vector<long>(-1))
            {
              continue;
            }

            var r_vec = Vector.ConditionalSelect(lt_vec, ij_vec, ikj_vec);
            r_vec.CopyTo(matrix, i * sz + j);

            var rk_vec = Vector.ConditionalSelect(lt_vec, new Vector<long>(routes, i * sz + j), k_vec);
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
}