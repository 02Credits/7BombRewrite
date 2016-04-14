// Decompiled with JetBrains decompiler
// Type: _7Bomb.Mine
// Assembly: 7Bomb, Version=1.2.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 4B0225F3-2161-48B3-93D9-F5BBEE613954
// Assembly location: C:\Users\Daniel\Desktop\7bomb_v1.9.0.0-space.me\7Bomb.dll

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;

namespace FallingBombs
{
  internal class Mine
  {
    public static Color MineColor = new Color(34, 177, 76);
    public Body Body;
    public int Size;

    public Mine(Vector2 position, World world, Random rand)
    {
      this.Size = rand.Next(10, 20);
      this.Body = BodyFactory.CreatePolygon(world, PolygonTools.CreateCircle((float) this.Size / 10f, 8), 1f, position, (object) Mine.MineColor);
    }
  }
}
