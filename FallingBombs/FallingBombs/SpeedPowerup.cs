// Decompiled with JetBrains decompiler
// Type: _7Bomb.SpeedPowerup
// Assembly: 7Bomb, Version=1.2.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 4B0225F3-2161-48B3-93D9-F5BBEE613954
// Assembly location: C:\Users\Daniel\Desktop\7bomb_v1.9.0.0-space.me\7Bomb.dll

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace FallingBombs
{
  internal class SpeedPowerup : IPowerup
  {
    private static Color powerUpColor = new Color(205, 0, 205);
    private Body powerUpBody;

    public Color PowerUpColor
    {
      get
      {
        return SpeedPowerup.powerUpColor;
      }
    }

    public Body PowerUpBody
    {
      get
      {
        return this.powerUpBody;
      }
    }

    public float Timer { get; set; }

    public static SoundEffect Sound { get; set; }

    public SpeedPowerup(World world, Vector2 startPosition)
    {
      this.powerUpBody = new Body(world)
      {
        Position = startPosition,
        IsStatic = false,
        UserData = this
      };
      Vertices circle = PolygonTools.CreateCircle(1.5f, 3);
      circle.Rotate(-1.570796f);
      FixtureFactory.AttachPolygon(circle, 1f, this.PowerUpBody, (object) SpeedPowerup.powerUpColor);
      this.Timer = 0.0f;
    }
  }
}
