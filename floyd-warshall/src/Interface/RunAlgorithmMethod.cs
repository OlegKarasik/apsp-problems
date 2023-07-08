using System;

namespace Code.Interface;

[Flags]
public enum RunAlgorithmMethod
{
  None = 0,
  Vector = 1,
  Parallel = 1 << 1,
  Spartial = 1 << 2,
  Routes = 1 << 3
}
