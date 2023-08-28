using System;
using System.Threading.Tasks;
using Hast.Layer;
using Hast.Transformer.SimpleMemory;

namespace Code.Hardware;

public class APSP04
{
  public virtual void ParallelOptimisation(SimpleMemory matrix, int sz)
  {
    for (var k = 0; k < sz; ++k)
    {
      Parallel.For(0, sz, i =>
      {
        for (var j = 0; j < sz; ++j)
        {
          var distance = matrix.ReadInt32(i * sz + k) + matrix.ReadInt32(k * sz + j);
          if (matrix.ReadInt32(i * sz + j) > distance)
          {
            matrix.WriteInt32(i * sz + j, distance);
          }
        }
      });
    }
  }

  public void Run(int[] matrix, int sz, IHastlayer hastlayer = null, IHardwareGenerationConfiguration configuration = null)
  {
    var memory = hastlayer is null
      ? SimpleMemory.CreateSoftwareMemory(matrix.Length)
      : hastlayer.CreateMemory(configuration, matrix.Length);

    for (var i = 0; i < matrix.Length; ++i)
      memory.WriteInt32(i, matrix[i]);

    ParallelOptimisation(memory, sz);

    for (var i = 0; i < matrix.Length; ++i)
      matrix[i] = memory.ReadInt32(i);
  }
}