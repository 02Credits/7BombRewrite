// Decompiled with JetBrains decompiler
// Type: _7Bomb.Explosion
// Assembly: 7Bomb, Version=1.2.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 4B0225F3-2161-48B3-93D9-F5BBEE613954
// Assembly location: C:\Users\Daniel\Desktop\7bomb_v1.9.0.0-space.me\7Bomb.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FallingBombs
{
  public class Explosion
  {
    private static Point frameSize = new Point(64, 64);
    private static Point sheetSize = new Point(5, 5);
    private Point currentFrame = new Point(0, 0);
    public static Texture2D SpriteSheet;
    public static SoundEffect Explosion1;
    public static SoundEffect Explosion2;
    public static SoundEffect Explosion3;
    public static SoundEffect Explosion4;
    public static SoundEffect Explosion5;
    public static SoundEffect Explosion6;
    private float scale;
    public Vector2 firstPosition;
    private Vector2 position;
    public bool Done;

    public Explosion(float size, Vector2 startPosition, Vector2 realPosition, Random rand)
    {
      this.scale = size / 5f;
      this.position = startPosition;
      this.firstPosition = realPosition;
      Explosion.CreateExplosionSound(rand);
    }

    public static void CreateExplosionSound(Random rand)
    {
      switch (rand.Next(6))
      {
        case 0:
          Explosion.Explosion1.Play(1f, 0.0f, 0.0f);
          break;
        case 1:
          Explosion.Explosion2.Play(1f, 0.0f, 0.0f);
          break;
        case 2:
          Explosion.Explosion3.Play(1f, 0.0f, 0.0f);
          break;
        case 3:
          Explosion.Explosion4.Play(1f, 0.0f, 0.0f);
          break;
        case 4:
          Explosion.Explosion5.Play(1f, 0.0f, 0.0f);
          break;
        case 5:
          Explosion.Explosion6.Play(1f, 0.0f, 0.0f);
          break;
      }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      if (this.Done)
        return;
      spriteBatch.Draw(Explosion.SpriteSheet, new Vector2(this.position.X - (float) ((double) Explosion.frameSize.X * (double) this.scale / 2.0), this.position.Y - (float) ((double) Explosion.frameSize.Y * (double) this.scale / 2.0)), new Rectangle?(new Rectangle(this.currentFrame.X * Explosion.frameSize.X, this.currentFrame.Y * Explosion.frameSize.Y, Explosion.frameSize.X, Explosion.frameSize.Y)), Color.White, 0.0f, Vector2.Zero, this.scale, SpriteEffects.None, 0.0f);
    }

    public void Update(Vector2 newPosition)
    {
      this.position = newPosition;
      ++this.currentFrame.X;
      if (this.currentFrame.X < Explosion.sheetSize.X)
        return;
      this.currentFrame.X = 0;
      ++this.currentFrame.Y;
      if (this.currentFrame.Y < Explosion.sheetSize.Y)
        return;
      this.Done = true;
    }
  }
}
