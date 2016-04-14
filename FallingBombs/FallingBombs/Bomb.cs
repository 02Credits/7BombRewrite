// Decompiled with JetBrains decompiler
// Type: _7Bomb.Bomb
// Assembly: 7Bomb, Version=1.2.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 4B0225F3-2161-48B3-93D9-F5BBEE613954
// Assembly location: C:\Users\Daniel\Desktop\7bomb_v1.9.0.0-space.me\7Bomb.dll

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace FallingBombs
{
  public class Bomb
  {
    public float Size;
    public Body Body;
    private Fixture tank;
    private Fixture head;
    private bool armed;
    private int timer;
    private Color bombColor;

    public Bomb(Vector2 startPosition, float startSize, World world, int startTime, float startingRotation)
    {
      this.Size = startSize;
      this.Body = BodyFactory.CreateBody(world, startPosition);
      this.Body.BodyType = BodyType.Dynamic;
      this.timer = startTime;
      this.UpdateColor();
      this.tank = FixtureFactory.AttachRectangle(this.Size / 10f, this.Size / 7f, 1f, new Vector2(0.0f, this.Size / 14f), this.Body, (object) this.bombColor);
      this.head = FixtureFactory.AttachPolygon(PolygonTools.CreateCircle(this.Size / 20f, 8), 1f, this.Body, (object) this.bombColor);
      this.Body.AngularVelocity = startingRotation;
      this.head.OnCollision = (OnCollisionEventHandler) ((a, b, c) =>
      {
        if (!this.armed)
          this.armed = true;
        return true;
      });
      this.tank.OnCollision = (OnCollisionEventHandler) ((a, b, c) =>
      {
        if (!this.armed)
          this.armed = true;
        return true;
      });
    }

    public void Tick(Game1 game)
    {
      if (!this.armed)
        return;
      this.UpdateColor();
      this.Body.FixtureList[0].UserData = (object) this.bombColor;
      this.Body.FixtureList[1].UserData = (object) this.bombColor;
      --this.timer;
      if (this.timer != 0)
        return;
      game.BlowUpBomb(this);
    }

    public void UpdateColor()
    {
      int num = (int) byte.MaxValue - this.timer * 5;
      if (num < 100)
        num = 100;
      this.bombColor = new Color(num, num, num);
    }
  }
}
