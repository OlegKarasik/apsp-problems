using System;
using System.IO;
using Code.Utilz;
using Xunit;

namespace Tests.Utilz
{
  public class MatrixTests
  {
    [Theory]
    [InlineData("18-14", 2)]
    [InlineData("18-14", 3)]
    [InlineData("18-14", 6)]
    public void SplitJoin(string input, int blockSize)
    {
      // Arrange
      using var inputStream = new FileStream(
        $@"{Environment.CurrentDirectory}/Data/{input}.input", FileMode.Open, FileAccess.Read, FileShare.Read);

      var original = Matrix.Read(inputStream);
      var clone    = Matrix.Copy(original);

      // Act: convert matrix to blocks and back
      clone.JoinFromBlocks(
        clone.SplitInBlocks(blockSize));

      // Assert
      Assert.Equal(original.Data, clone.Data);
    }
  }
}
