﻿using System;
using System.Numerics;
using System.Threading;
using Code.Utilz;

namespace Code.Algorithms;

public static class BlockedFloydWarshall
{
  private readonly struct ParallelMessage
  {
    public readonly CountdownEvent Event;
    public readonly int[] Matrix;
    public readonly int OffsetX;
    public readonly int OffsetY;
    public readonly int OffsetZ;
    public readonly int BlockSize;
    public readonly int SpanLength;

    public ParallelMessage(
      CountdownEvent Event,
      int[] Matrix,
      int OffsetX,
      int OffsetY,
      int OffsetZ,
      int BlockSize,
      int SpanLength)
    {
      this.Event = Event;
      this.Matrix = Matrix;
      this.OffsetX = OffsetX;
      this.OffsetY = OffsetY;
      this.OffsetZ = OffsetZ;
      this.BlockSize = BlockSize;
      this.SpanLength = SpanLength;
    }
  }

  public static void Baseline(Matrix.Blocks blocks)
    => Baseline(blocks.Data, blocks.Count, blocks.Size);

  public static void ParallelOptimisation(Matrix.Blocks blocks)
    => ParallelOptimisation(blocks.Data, blocks.Count, blocks.Size);

  public static void VectorOptimisation(Matrix.Blocks blocks)
    => VectorOptimisation(blocks.Data, blocks.Count, blocks.Size);

  public static void ParallelVectorOptimisations(Matrix.Blocks blocks)
    => ParallelVectorOptimisations(blocks.Data, blocks.Count, blocks.Size);

  private static void Baseline(int[] matrix, int block_count, int block_size)
  {
    var lineral_block_size = block_size * block_size;
    var lineral_block_row_size = block_count * lineral_block_size;

    for (var m = 0; m < block_count; ++m) 
    {
      var offset_mm = m * lineral_block_row_size + m * lineral_block_size;

      var mm = new Span<int>(matrix, offset_mm, lineral_block_size);

      Procedure(mm, mm, mm, block_size);

      for (var i = 0; i < block_count; ++i) 
      {
        if (i != m) 
        {
          var offset_im = i * lineral_block_row_size + m * lineral_block_size;
          var offset_mi = m * lineral_block_row_size + i * lineral_block_size;
      
          var im = new Span<int>(matrix, offset_im, lineral_block_size);
          var mi = new Span<int>(matrix, offset_mi, lineral_block_size);

          Procedure(im, im, mm, block_size);
          Procedure(mi, mm, mi, block_size);
        }
      }
      for (var i = 0; i < block_count; ++i) 
      {
        if (i != m) 
        {
          var offset_im = i * lineral_block_row_size + m * lineral_block_size;

          var im = new Span<int>(matrix, offset_im, lineral_block_size);

          for (var j = 0; j < block_count; ++j) 
          {
            if (j != m) 
            {
              var offset_ij = i * lineral_block_row_size + j * lineral_block_size;
              var offset_mj = m * lineral_block_row_size + j * lineral_block_size;
      
              var ij = new Span<int>(matrix, offset_ij, lineral_block_size);
              var mj = new Span<int>(matrix, offset_mj, lineral_block_size);

              Procedure(ij, im, mj, block_size);
            }
          }
        }
      }
    }

    static void Procedure(
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
  }

  private static void ParallelOptimisation(int[] matrix, int block_count, int block_size)
  {
    var iteration_sync = new CountdownEvent(0);

    var lineral_block_size = block_size * block_size;
    var lineral_block_row_size = block_count * lineral_block_size;

    for (var m = 0; m < block_count; ++m) 
    {
      var offset_mm = m * lineral_block_row_size + m * lineral_block_size;

      var mm = new Span<int>(matrix, offset_mm, lineral_block_size);

      Procedure(mm, mm, mm, block_size);

      iteration_sync.Reset(2 * (block_count - 1));
      for (var i = 0; i < block_count; ++i) 
      {
        if (i != m) 
        {
          var offset_im = i * lineral_block_row_size + m * lineral_block_size;
          var offset_mi = m * lineral_block_row_size + i * lineral_block_size;

          var message_x = new ParallelMessage(
            iteration_sync, matrix, offset_im, offset_im, offset_mm, block_size, lineral_block_size);

          var message_y = new ParallelMessage(
            iteration_sync, matrix, offset_mi, offset_mm, offset_mi, block_size, lineral_block_size);

          ThreadPool.QueueUserWorkItem(ParallelProcedure, message_x, false);
          ThreadPool.QueueUserWorkItem(ParallelProcedure, message_y, false);
        }
      }
      iteration_sync.Wait();

      iteration_sync.Reset((block_count - 1) * (block_count - 1));
      for (var i = 0; i < block_count; ++i) 
      {
        if (i != m) 
        {
          var offset_im = i * lineral_block_row_size + m * lineral_block_size;
          for (var j = 0; j < block_count; ++j) 
          {
            if (j != m) 
            {
              var offset_ij = i * lineral_block_row_size + j * lineral_block_size;
              var offset_mj = m * lineral_block_row_size + j * lineral_block_size;

              var message = new ParallelMessage(
                iteration_sync, matrix, offset_ij, offset_im, offset_mj, block_size, lineral_block_size);

              ThreadPool.QueueUserWorkItem(ParallelProcedure, message, false);
            }
          }
        }
      }
      iteration_sync.Wait();
    }
    
    static void ParallelProcedure(
      ParallelMessage message)
    {
      var x = new Span<int>(message.Matrix, message.OffsetX, message.SpanLength);
      var y = new Span<int>(message.Matrix, message.OffsetY, message.SpanLength);
      var z = new Span<int>(message.Matrix, message.OffsetZ, message.SpanLength);

      Procedure(x, y, z, message.BlockSize);

      message.Event.Signal();
    }
    static void Procedure(
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
  }

  private static void VectorOptimisation(int[] matrix, int block_count, int block_size)
  {
    var lineral_block_size = block_size * block_size;
    var lineral_block_row_size = block_count * lineral_block_size;

    for (var m = 0; m < block_count; ++m) 
    {
      var offset_mm = m * lineral_block_row_size + m * lineral_block_size;

      var mm = new Span<int>(matrix, offset_mm, lineral_block_size);

      Procedure(mm, mm, mm, block_size);

      for (var i = 0; i < block_count; ++i) 
      {
        if (i != m) 
        {
          var offset_im = i * lineral_block_row_size + m * lineral_block_size;
          var offset_mi = m * lineral_block_row_size + i * lineral_block_size;
      
          var im = new Span<int>(matrix, offset_im, lineral_block_size);
          var mi = new Span<int>(matrix, offset_mi, lineral_block_size);

          Procedure(im, im, mm, block_size);
          Procedure(mi, mm, mi, block_size);
        }
      }
      for (var i = 0; i < block_count; ++i) 
      {
        if (i != m) 
        {
          var offset_im = i * lineral_block_row_size + m * lineral_block_size;

          var im = new Span<int>(matrix, offset_im, lineral_block_size);

          for (var j = 0; j < block_count; ++j) 
          {
            if (j != m) 
            {
              var offset_ij = i * lineral_block_row_size + j * lineral_block_size;
              var offset_mj = m * lineral_block_row_size + j * lineral_block_size;
      
              var ij = new Span<int>(matrix, offset_ij, lineral_block_size);
              var mj = new Span<int>(matrix, offset_mj, lineral_block_size);

              Procedure(ij, im, mj, block_size);
            }
          }
        }
      }
    }
    static void Procedure(
      Span<int> ij, Span<int> ik, Span<int> kj, int block_size)
    {
      for (var k = 0; k < block_size; ++k)
      {
        for (var i = 0; i < block_size; ++i)
        {
          var ik_vec = new Vector<int>(ik[i * block_size + k]);

          var j = 0;
          for (; j < block_size - Vector<int>.Count; j += Vector<int>.Count)
          {
            var ij_vec = new Vector<int>(ij.Slice(i * block_size + j, Vector<int>.Count));
            var ikj_vec = new Vector<int>(kj.Slice(k * block_size + j, Vector<int>.Count)) + ik_vec;

            var lt_vec = Vector.LessThan(ij_vec, ikj_vec);
            if (lt_vec == new Vector<int>(-1))
            {
              continue;
            }

            var r_vec = Vector.ConditionalSelect(lt_vec, ij_vec, ikj_vec);
            r_vec.CopyTo(ij.Slice(i * block_size + j, Vector<int>.Count));
          }

          for (; j < block_size; ++j)
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
  }

  private static void ParallelVectorOptimisations(int[] matrix, int block_count, int block_size)
  {
    var iteration_sync = new CountdownEvent(0);

    var lineral_block_size = block_size * block_size;
    var lineral_block_row_size = block_count * lineral_block_size;

    for (var m = 0; m < block_count; ++m) 
    {
      var offset_mm = m * lineral_block_row_size + m * lineral_block_size;

      var mm = new Span<int>(matrix, offset_mm, lineral_block_size);

      Procedure(mm, mm, mm, block_size);

      iteration_sync.Reset(2 * (block_count - 1));
      for (var i = 0; i < block_count; ++i) 
      {
        if (i != m) 
        {
          var offset_im = i * lineral_block_row_size + m * lineral_block_size;
          var offset_mi = m * lineral_block_row_size + i * lineral_block_size;

          var message_x = new ParallelMessage(
            iteration_sync, matrix, offset_im, offset_im, offset_mm, block_size, lineral_block_size);

          var message_y = new ParallelMessage(
            iteration_sync, matrix, offset_mi, offset_mm, offset_mi, block_size, lineral_block_size);

          ThreadPool.QueueUserWorkItem(ParallelProcedure, message_x, false);
          ThreadPool.QueueUserWorkItem(ParallelProcedure, message_y, false);
        }
      }
      iteration_sync.Wait();

      iteration_sync.Reset((block_count - 1) * (block_count - 1));
      for (var i = 0; i < block_count; ++i) 
      {
        if (i != m) 
        {
          var offset_im = i * lineral_block_row_size + m * lineral_block_size;
          for (var j = 0; j < block_count; ++j) 
          {
            if (j != m) 
            {
              var offset_ij = i * lineral_block_row_size + j * lineral_block_size;
              var offset_mj = m * lineral_block_row_size + j * lineral_block_size;

              var message = new ParallelMessage(
                iteration_sync, matrix, offset_ij, offset_im, offset_mj, block_size, lineral_block_size);

              ThreadPool.QueueUserWorkItem(ParallelProcedure, message, false);
            }
          }
        }
      }
      iteration_sync.Wait();
    }
    
    static void ParallelProcedure(
      ParallelMessage message)
    {
      var x = new Span<int>(message.Matrix, message.OffsetX, message.SpanLength);
      var y = new Span<int>(message.Matrix, message.OffsetY, message.SpanLength);
      var z = new Span<int>(message.Matrix, message.OffsetZ, message.SpanLength);

      Procedure(x, y, z, message.BlockSize);

      message.Event.Signal();
    }
    static void Procedure(
      Span<int> ij, Span<int> ik, Span<int> kj, int block_size)
    {
      for (var k = 0; k < block_size; ++k)
      {
        for (var i = 0; i < block_size; ++i)
        {
          var ik_vec = new Vector<int>(ik[i * block_size + k]);

          var j = 0;
          for (; j < block_size - Vector<int>.Count; j += Vector<int>.Count)
          {
            var ij_vec = new Vector<int>(ij.Slice(i * block_size + j, Vector<int>.Count));
            var ikj_vec = new Vector<int>(kj.Slice(k * block_size + j, Vector<int>.Count)) + ik_vec;

            var lt_vec = Vector.LessThan(ij_vec, ikj_vec);
            if (lt_vec == new Vector<int>(-1))
            {
              continue;
            }

            var r_vec = Vector.ConditionalSelect(lt_vec, ij_vec, ikj_vec);
            r_vec.CopyTo(ij.Slice(i * block_size + j, Vector<int>.Count));
          }

          for (; j < block_size; ++j)
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
  }
}