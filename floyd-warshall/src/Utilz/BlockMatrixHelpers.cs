using System;

namespace Code.Utilz
{
  public static class BlockMatrixHelpers
  {
    public static (long[] matrix, int block_count, int block_size, int orign_size) ConvertMatrixToBlockMatrix(
      long[] matrix, int size, int block_size)
    {
      var orign_size = size;
      var block_count = size / block_size;
      if (size % block_size != 0)
      {
        block_count++;
        size = block_count * block_size;
      }

      var sz_block = block_size * block_size;
      var sz_block_row = block_count * sz_block;

      var result = new long[size * size];
      Array.Fill(result, Constants.NO_EDGE);

      for (var i = 0; i < orign_size; i++)
      {
        for (var j = 0; j < orign_size; j++)
        {
          var b_i = i / block_size; var b_j = j / block_size;
          var b_x = i % block_size; var b_y = j % block_size;

          result[b_i * sz_block_row + b_j * sz_block + b_x * block_size + b_y] = matrix[i * orign_size + j];
        }
      }

      return (result, block_count, block_size, orign_size);
    }

    public static (long[] matrix, int size) ConvertBlockMatrixToMatrix(
      long[] matrix, int block_count, int block_size, int orign_size)
    {
      var sz_block = block_size * block_size;
      var sz_block_row = block_count * sz_block;

      var result = new long[orign_size * orign_size];
      for (var i = 0; i < orign_size; i++)
      {
        for (var j = 0; j < orign_size; j++)
        {
          var b_i = i / block_size; var b_j = j / block_size;
          var b_x = i % block_size; var b_y = j % block_size;

          result[i * orign_size + j] = matrix[b_i * sz_block_row + b_j * sz_block + b_x * block_size + b_y];
        }
      }

      return (result, orign_size);
    }
  }
}