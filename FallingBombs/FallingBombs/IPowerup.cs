// Decompiled with JetBrains decompiler
// Type: _7Bomb.IPowerup
// Assembly: 7Bomb, Version=1.2.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 4B0225F3-2161-48B3-93D9-F5BBEE613954
// Assembly location: C:\Users\Daniel\Desktop\7bomb_v1.9.0.0-space.me\7Bomb.dll

using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FallingBombs
{
  public interface IPowerup
  {
    Color PowerUpColor { get; }

    Body PowerUpBody { get; }

    float Timer { get; set; }
  }
}
