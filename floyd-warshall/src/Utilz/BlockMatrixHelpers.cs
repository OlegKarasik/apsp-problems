namespace Code.Utilz
{
  public static class BlockMatrixHelpers
  {
    public static (int[] matrix, int block_count, int block_size) ConvertMatrixToBlockMatrix(
      int[] matrix, int size, int block_size)
    {
      var block_count = size / block_size;

      var sz_block = block_size * block_size;
      var sz_block_row = block_count * sz_block;

      var result = new int[size * size];
      for (int i = 0; i < size; i++)
      {
        for (int j = 0; j < size; j++)
        {
          var b_i = i / block_size; var b_j = j / block_size;
          var b_x = i % block_size; var b_y = j % block_size;

          result[b_i * sz_block_row + b_j * sz_block + b_x * block_size + b_y] = matrix[i * size + j];
        }
      }

      return (result, block_count, block_size);
    }

    public static (int[] matrix, int size) ConvertBlockMatrixToMatrix(
      int[] matrix, int block_count, int block_size)
    {
      var size = block_count * block_size;

      var sz_block = block_size * block_size;
      var sz_block_row = block_count * sz_block;

      var result = new int[size * size];
      for (int i = 0; i < size; i++)
      {
        for (int j = 0; j < size; j++)
        {
          var b_i = i / block_size; var b_j = j / block_size;
          var b_x = i % block_size; var b_y = j % block_size;

          result[i * size + j] = matrix[b_i * sz_block_row + b_j * sz_block + b_x * block_size + b_y];
        }
      }

      return (result, size);
    }
  }
}