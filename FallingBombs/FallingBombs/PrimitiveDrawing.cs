// Decompiled with JetBrains decompiler
// Type: _7Bomb.PrimitiveDrawing
// Assembly: 7Bomb, Version=1.2.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 4B0225F3-2161-48B3-93D9-F5BBEE613954
// Assembly location: C:\Users\Daniel\Desktop\7bomb_v1.9.0.0-space.me\7Bomb.dll

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FallingBombs
{
  internal class PrimitiveDrawing
  {
    private static Color defaultColor = new Color(0, 0, (int) byte.MaxValue);
    private static List<VertexPositionColor> TriangleVerts = new List<VertexPositionColor>();
    private static List<VertexPositionColor> LineVerts = new List<VertexPositionColor>();
    private const int circleSideAmounts = 20;
    private static BasicEffect basicEffect;
    private static World world;
    private static VertexPositionColor firstVPC;
    private static VertexPositionColor secondVPC;
    private static VertexPositionColor thirdVPC;
    private static bool shaking;
    private static float shakeMagnitude;
    private static float shakeDuration;
    private static float shakeTimer;

    public static void SetUp(World startWorld)
    {
      world = startWorld;
    }

    public static void Shake(float magnitude, float duration)
    {
      if (!shaking)
      {
        shaking = true;
        shakeTimer = 0.0f;
        shakeDuration = duration;
        shakeMagnitude = magnitude;
      }
      else
        shakeMagnitude += magnitude;
    }

    public static void Draw(float yPos, float worldScreenWidth, GraphicsDevice gd, Game1 game, GameTime gameTime, Random rand, Viewport viewport)
    {
      float xPosition = 0.0f;
      float yPosition = -yPos;
      float zPosition = 0.0f;
      if (shaking)
      {
        shakeTimer += (float) gameTime.ElapsedGameTime.TotalSeconds;
        if ((double) shakeTimer >= (double) shakeDuration)
        {
          shaking = false;
          shakeTimer = shakeDuration;
        }
        float num1 = shakeTimer / shakeDuration;
        float num2 = shakeMagnitude * (float) (1.0 - (double) num1 * (double) num1);
        xPosition = (float) (rand.NextDouble() * 2.0 - 1.0) * num2;
        yPosition += (float) (rand.NextDouble() * 2.0 - 1.0) * num2;
      }
      basicEffect = new BasicEffect(gd)
      {
        Projection = Matrix.CreateOrthographic(worldScreenWidth, worldScreenWidth / gd.Viewport.AspectRatio, 0.0f, 1f),
        View = Matrix.CreateTranslation(xPosition, yPosition, zPosition),
        VertexColorEnabled = true
      };
      foreach (Body body in world.BodyList)
      {
        if (body != game.highScoreLine || body.UserData == (ValueType) Mine.MineColor)
          DrawBody(body);
      }
      DrawBody(game.highScoreLine);
      gd.Viewport = viewport;
      foreach (EffectPass effectPass in basicEffect.CurrentTechnique.Passes)
      {
        effectPass.Apply();
        if (TriangleVerts.Count > 0)
          gd.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, TriangleVerts.ToArray(), 0, TriangleVerts.Count / 3);
        if (LineVerts.Count > 0)
          gd.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, LineVerts.ToArray(), 0, LineVerts.Count / 2);
      }
      LineVerts.Clear();
      TriangleVerts.Clear();
    }

    private static void DrawBody(Body body)
    {
      foreach (Fixture fixture in body.FixtureList)
      {
        Vertices vertsFromFixture = GetVertsFromFixture(fixture);
        if (fixture.UserData is Color)
          DrawShape(vertsFromFixture, (Color) fixture.UserData, fixture.Body.Position, fixture.Body.Rotation, fixture.Shape.ShapeType == ShapeType.Circle);
        else if (!(fixture.UserData is Texture))
          DrawShape(vertsFromFixture, defaultColor, fixture.Body.Position, fixture.Body.Rotation, fixture.Shape.ShapeType == ShapeType.Circle);
      }
    }

    public static void DrawShape(Vertices fixtureVerts, Color color, Vector2 position, float rotation, bool drawRadius)
    {
      bool flag1 = false;
      Vector2 vector1 = Vector2.Zero;
      bool flag2 = false;
      Vector2 vector2 = Vector2.Zero;
      Matrix translation = Matrix.CreateTranslation(MakeVector3(position));
      Matrix rotationZ = Matrix.CreateRotationZ(rotation);
      Matrix matrix = rotationZ * translation;
      foreach (Vector2 vector3 in (List<Vector2>) fixtureVerts)
      {
        if (!flag1)
        {
          flag1 = true;
          vector1 = vector3;
        }
        else if (!flag2)
        {
          flag2 = true;
          vector2 = vector3;
        }
        else
        {
          Vector3 vector3_1 = Vector3.Transform(MakeVector3(vector1), matrix);
          Vector3 vector3_2 = Vector3.Transform(MakeVector3(vector2), matrix);
          Vector3 vector3_3 = Vector3.Transform(MakeVector3(vector3), matrix);
          firstVPC.Position = vector3_1;
          firstVPC.Color = color;
          secondVPC.Position = vector3_2;
          secondVPC.Color = color;
          thirdVPC.Position = vector3_3;
          thirdVPC.Color = color;
          TriangleVerts.Add(thirdVPC);
          TriangleVerts.Add(secondVPC);
          TriangleVerts.Add(firstVPC);
          vector2 = vector3;
        }
      }
      if (!drawRadius)
        return;
      Vector3 position1 = Vector3.Transform(Vector3.Transform(MakeVector3(fixtureVerts[0]), rotationZ), translation);
      LineVerts.Add(new VertexPositionColor(position1, Color.White));
      LineVerts.Add(new VertexPositionColor(MakeVector3(position), Color.White));
    }

    private static Vector3 MakeVector3(Vector2 vector)
    {
      return new Vector3(vector.X, vector.Y, 0.0f);
    }

    private static Vertices GetVertsFromFixture(Fixture fixture)
    {
      if (fixture.Shape.ShapeType == ShapeType.Circle)
        return PolygonTools.CreateCircle(fixture.Shape.Radius, 20);
      return ((PolygonShape) fixture.Shape).Vertices;
    }
  }
}
