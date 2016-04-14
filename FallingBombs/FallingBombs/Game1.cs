// Decompiled with JetBrains decompiler
// Type: _7Bomb.Game1
// Assembly: 7Bomb, Version=1.2.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 4B0225F3-2161-48B3-93D9-F5BBEE613954
// Assembly location: C:\Users\Daniel\Desktop\7bomb_v1.9.0.0-space.me\7Bomb.dll

using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using FarseerPhysics.Dynamics.Contacts;

namespace FallingBombs
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private Random rand;

        public const string versionNumber = "1.9";
        private const float worldScreenWidth = 66.6666f;
        private const float powerupFadeTime = 30f;
        private const float powerupFadeLength = 5f;
        private const float timeWaitedTillBotherBallComes = 45f;
        private const int ballStartFriction = 10;
        private const int speedTimerMax = 30;

        private static Color ballColor;
        private static Color floorColor;

        private List<Mine> mines;
        private List<Explosion> explosions;
        private List<IPowerup> weaponCharges;
        private List<Bomb> bombs;

        private World world;
        private Body ball;
        private Body floorBody;
        public Body highScoreLine;
        private SpriteFont spritefont;
        private Texture2D stars;
        private Texture2D chargeMarker;
        private double highScore;
        private double score;
        private Body botherBall;
        private bool shielded;
        private DateTime timeDead;
        private DateTime lastExploded;
        private DateTime lastJumped;
        private int lastUppedScore;
        private int timeSinceStart;
        private int? lastErasedPoint;
        private float speedTimer;
        private float currentFloorLength;
        private bool touchedGround = true;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = true,
                PreferredBackBufferWidth = 3840,
                PreferredBackBufferHeight = 2160
            };
            Content.RootDirectory = "Content";

            rand = new Random();

            ballColor = new Color((int)byte.MaxValue, 0, 0, (int)byte.MaxValue);
            floorColor = new Color(0, (int)byte.MaxValue, 0, (int)byte.MaxValue);

            world = new World(new Vector2(0.0f, -20f));
        }

        protected override void Initialize()
        {
            PrimitiveDrawing.SetUp(world);

            mines = new List<Mine>();
            explosions = new List<Explosion>();
            weaponCharges = new List<IPowerup>();
            bombs = new List<Bomb>();

            ResetWorld();

            base.Initialize();
        }

        private void ResetWorld()
        {
            world.Clear();

            mines.Clear();
            explosions.Clear();
            weaponCharges.Clear();
            bombs.Clear();

            shielded = false;
            botherBall = null;
            speedTimer = 0.0f;

            CreateWorld();
            lastUppedScore = timeSinceStart;
        }

        private void CreateWorld()
        {
            timeSinceStart = 0;
            touchedGround = true;
            score = 0.0;

            ball = BodyFactory.CreateCircle(world, 1.5f, 1f, new Vector2(0.0f, 3f), ballColor);
            ball.BodyType = BodyType.Dynamic;
            ball.Friction = 10f;

            floorBody = new Body(world)
            {
                BodyType = BodyType.Static,
                Position = new Vector2(0.0f, 0.0f),
                FixedRotation = true
            };

            currentFloorLength = 0.0f;

            BodyFactory.CreateRectangle(world, 4f, 1706665.0f / 512.0f, 1f, new Vector2(-35.4333f, ball.Position.Y), (object)new Color(0, 0, 0)).Friction = 0.0f;
            BodyFactory.CreateRectangle(world, 4f, 1706665.0f / 512.0f, 1f, new Vector2(35.3333f, ball.Position.Y), (object)new Color(0, 0, 0)).Friction = 0.0f;

            highScoreLine = BodyFactory.CreateRectangle(world, 66.6666f, (float)(66.6666030883789 / (double)GraphicsDevice.Viewport.AspectRatio / 40.0), 1f, new Vector2(0.0f, (float)(-highScore - 66.6666030883789 / (double)GraphicsDevice.Viewport.AspectRatio / 80.0)), (object)new Color(0, 0, (int)byte.MaxValue, 100));
            highScoreLine.IsStatic = true;
            highScoreLine.IsSensor = true;

            ball.OnCollision += new OnCollisionEventHandler(OnBallCollision);
            world.Step(1000f);
        }

        bool OnBallCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (fixtureB.Body == floorBody || bombs.Any(o => o.Body == fixtureB.Body))
                touchedGround = (double)contact.Manifold.LocalPoint.Y <= (double)ball.Position.Y;
            else if (fixtureB.Body.UserData is ShieldPowerup)
            {
                if (world.BodyList.Contains(fixtureB.Body))
                {
                    ShieldPowerup.Sound.Play();
                    world.RemoveBody(fixtureB.Body);
                    shielded = true;
                    return false;
                }
            }
            else if (fixtureB.Body.UserData is ExplosionPowerup)
            {
                if (world.BodyList.Contains(fixtureB.Body))
                {
                    ExplosionPowerup.Sound.Play();
                    if (weaponCharges.Count < 5)
                        weaponCharges.Add((IPowerup)fixtureB.Body.UserData);
                    world.RemoveBody(fixtureB.Body);
                    return false;
                }
            }
            else if (fixtureB.Body.UserData is SpeedPowerup)
            {
                if (world.BodyList.Contains(fixtureB.Body))
                {
                    SpeedPowerup.Sound.Play();
                    world.RemoveBody(fixtureB.Body);
                    if ((double)speedTimer <= 0.0)
                        speedTimer = 30f;
                    return false;
                }
            }
            else if (fixtureB.Body.UserData is TeleportPowerup)
            {
                if (world.BodyList.Contains(fixtureB.Body))
                {
                    TeleportPowerup.Sound.Play();
                    world.RemoveBody(fixtureB.Body);
                    if (weaponCharges.Count < 5)
                        weaponCharges.Add((IPowerup)fixtureB.Body.UserData);
                }
            }
            else if (fixtureB.Body.UserData is LazerPowerup && world.BodyList.Contains(fixtureB.Body))
            {
                LazerPowerup.Sound.Play();
                world.RemoveBody(fixtureB.Body);
                if (weaponCharges.Count < 5)
                    weaponCharges.Add((IPowerup)fixtureB.Body.UserData);
            }
            return true;
        }

        protected override void LoadContent()
        {
            spritefont = Content.Load<SpriteFont>("font");
            stars = Content.Load<Texture2D>("Stars");
            chargeMarker = Content.Load<Texture2D>("ChargeMarker");

            Explosion.SpriteSheet = Content.Load<Texture2D>("Explosion");
            Texture2D texture2D = Content.Load<Texture2D>("Capture" + (object)rand.Next(1, 8));
            Explosion.Explosion1 = Content.Load<SoundEffect>("Explosion1");
            Explosion.Explosion2 = Content.Load<SoundEffect>("Explosion2");
            Explosion.Explosion3 = Content.Load<SoundEffect>("Explosion3");
            Explosion.Explosion4 = Content.Load<SoundEffect>("Explosion4");
            Explosion.Explosion5 = Content.Load<SoundEffect>("Explosion5");
            Explosion.Explosion6 = Content.Load<SoundEffect>("Explosion6");
            ExplosionPowerup.Sound = Content.Load<SoundEffect>("GrenadeUp");
            ShieldPowerup.Sound = Content.Load<SoundEffect>("ShieldUp");
            SpeedPowerup.Sound = Content.Load<SoundEffect>("SpeedUp");
            TeleportPowerup.Sound = Content.Load<SoundEffect>("GravityUp");
            LazerPowerup.Sound = Content.Load<SoundEffect>("LazerUp");

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (ball.Position.X > 33.3333f)
                ball.Position = new Vector2(33.3333f, ball.Position.Y);
            else if (ball.Position.X < -33.3333f)
                ball.Position = new Vector2(-33.3333f, ball.Position.Y);

            HandleInput();

            if (gameTime.TotalGameTime.Seconds > 1 && (DateTime.Now - timeDead).Seconds > 2 && !world.BodyList.Contains(ball))
                ResetWorld();

            timeSinceStart += gameTime.ElapsedGameTime.Milliseconds;

            if (speedTimer > 0f)
            {
                speedTimer -= (float)gameTime.ElapsedGameTime.Milliseconds / 800f;
                if (speedTimer <= 0f)
                    CreateBombExplosion(ball.Position, 20f, false);
            }

            HandleBotherBall();
            UpdateExplosions();

            //Set the number only to 350 for complete MADNESS
            if (rand.Next(3500) <= timeSinceStart / 1000 + 30)
                MakeFallingItem();

            bombs.ForEach(b => b.Tick(this));

            if (ball.Position.Y - 66.6666f / base.GraphicsDevice.Viewport.AspectRatio / 2f < currentFloorLength)
                AddToFloorLength();

            UpdatePowerups(gameTime);

            FigureScore();

            world.Step(gameTime.ElapsedGameTime.Milliseconds * 0.001f);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            float scale = 1f;

            if (shielded)
                PrimitiveDrawing.DrawShape(PolygonTools.CreateCircle(2f, 20), new Color(0, (int)byte.MaxValue, (int)byte.MaxValue, (int)byte.MaxValue), ball.Position, 0.0f, false);

            GraphicsDevice.Clear(Color.Black);

            SpriteBatch spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteBatch.Begin();
            spriteBatch.Draw(stars, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.End();

            if (ball != null)
                PrimitiveDrawing.Draw(ball.Position.Y, 66.6666f, GraphicsDevice, this, gameTime, rand, GraphicsDevice.Viewport);

            spriteBatch.Begin();

            for (int index = 0; index < weaponCharges.Count; ++index)
                spriteBatch.Draw(chargeMarker, new Vector2((float)(index * (chargeMarker.Width * 2 + 5) + 5), 30f), new Rectangle?(), weaponCharges[index].PowerUpColor, 0.0f, Vector2.Zero, 2f, SpriteEffects.None, 0.0f);

            foreach (Explosion explosion in explosions)
                explosion.Draw(spriteBatch);

            spriteBatch.DrawString(spritefont, "Device HighScore:" + (object)(int)highScore, new Vector2(scale * 2f, scale * 2f), Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(spritefont, "Device HighScore:" + (object)(int)highScore, new Vector2(0.0f, 0.0f), Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(spritefont, "Score:" + (object)(int)score, new Vector2((float)(GraphicsDevice.Viewport.Width - (int)((double)spritefont.MeasureString("Score:" + (object)(int)score).X * (double)scale)) + scale * 2f, scale * 2f), Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(spritefont, "Score:" + (object)(int)score, new Vector2((float)(GraphicsDevice.Viewport.Width - (int)((double)spritefont.MeasureString("Score:" + (object)(int)score).X * (double)scale)), 0.0f), Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(spritefont, "Version:1.9", new Vector2(2f, (float)((double)GraphicsDevice.Viewport.Height - (double)spritefont.MeasureString("Version:1.9").Y * (double)scale + 2.0)), Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(spritefont, "Version:1.9", new Vector2(0.0f, (float)GraphicsDevice.Viewport.Height - spritefont.MeasureString("Version:1.9").Y * scale), Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);

            if (botherBall == null)
            {
                float num = 45f - (float)((int)((double)timeSinceStart / 1000.0) - (int)((double)lastUppedScore / 1000.0));
                Color color = Color.White;
                if ((double)num < 6.0)
                    color = Color.Red;
                spriteBatch.DrawString(spritefont, "TimeTillBother:" + (object)num, new Vector2((float)(GraphicsDevice.Viewport.Width - (int)((double)spritefont.MeasureString("TimeTillBother:" + (object)num).X * (double)scale)) + scale * 2f, (float)(22.0 + (double)scale * 2.0)), Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
                spriteBatch.DrawString(spritefont, "TimeTillBother:" + (object)num, new Vector2((float)(GraphicsDevice.Viewport.Width - (int)((double)spritefont.MeasureString("TimeTillBother:" + (object)num).X * (double)scale)), 22f), color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void UpdatePowerups(GameTime gameTime)
        {
            List<Body> list = new List<Body>();

            foreach (Body body in world.BodyList)
            {
                if (body.UserData is IPowerup)
                {
                    IPowerup powerup = body.UserData as IPowerup;

                    powerup.Timer += (float)gameTime.ElapsedGameTime.Milliseconds / 1000f;

                    if ((double)powerup.Timer > 35.0)
                        list.Add(body);
                    else if ((double)powerup.Timer > 30.0)
                    {
                        Color powerUpColor = powerup.PowerUpColor;
                        float num = (float)(1.0 - ((double)powerup.Timer - 30.0) / 5.0);
                        Color color = new Color(num * powerUpColor.ToVector3().X, num * powerUpColor.ToVector3().Y, num * powerUpColor.ToVector3().Z);
                        body.FixtureList[0].UserData = (object)color;
                    }
                }
            }

            list.ForEach(b => world.RemoveBody(b));
        }

        private void HandleBotherBall()
        {
            if (botherBall != null && world.BodyList.Contains(botherBall))
                botherBall.ApplyForce((ball.Position - botherBall.Position) * 100f);

            if (45.0 - (double)((int)((double)timeSinceStart / 1000.0) - (int)((double)lastUppedScore / 1000.0)) >= 0.0)
                return;

            bool flag = true;

            if (botherBall != null && world.BodyList.Contains(botherBall))
                flag = false;

            if (!flag)
                return;

            botherBall = BodyFactory.CreateCircle(world, 1.5f, 10f, new Vector2(0.0f, 20f));
            botherBall.IgnoreGravity = true;
            botherBall.IsStatic = false;
            botherBall.Friction = 40f;
        }

        private void UpdateExplosions()
        {
            List<Explosion> list = new List<Explosion>();

            foreach (Explosion explosion in explosions)
            {
                if (explosion.Done)
                    list.Add(explosion);
                else
                    explosion.Update(ConvertToScreenCords(explosion.firstPosition));
            }

            foreach (Explosion explosion in list)
                explosions.Remove(explosion);
        }

        private void HandleInput()
        {
            var ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Escape))
                Exit();

            if (ks.IsKeyDown(Keys.Space) && weaponCharges.Count > 0 && (DateTime.Now - lastExploded).Milliseconds > 500)
            {
                Explosion.CreateExplosionSound(rand);

                lastExploded = DateTime.Now;

                Vector2 position = ball.Position;

                var firstPowerup = weaponCharges.First();
                if (firstPowerup is ExplosionPowerup)
                {
                    Vertices circle = PolygonTools.CreateCircle(10f, 12);
                    circle.Translate(ref position);
                    TestMines(10f, ball.Position, new List<Vertices>() { circle }, false);
                    CutOutShapeFromFloor(circle);
                    CreateBombExplosion(ball.Position, 20f, false);
                }
                else if (firstPowerup is TeleportPowerup)
                    ball.Position = Vector2.Zero;
                else if (firstPowerup is LazerPowerup)
                {
                    Vertices rectangle = PolygonTools.CreateRectangle(66.6666f, 3f, new Vector2(0.0f, ball.Position.Y), 0.0f);
                    TestMines(10f, ball.Position, new List<Vertices>() { rectangle }, false);
                    CutOutShapeFromFloor(rectangle);
                    CreateBombExplosion(ball.Position, 20f, false);
                }
                else
                    throw new Exception("Unknown powerup used");

                weaponCharges.Remove(Enumerable.First<IPowerup>((IEnumerable<IPowerup>)weaponCharges));

                CreateBombExplosion(ball.Position, 20f, false);
            }

            if (ks.IsKeyDown(Keys.Up))
            {
                if ((DateTime.Now - lastJumped).Milliseconds <= 300)
                    return;
                lastJumped = DateTime.Now;
                Jump();
            }

            if (ks.IsKeyDown(Keys.Left) || ks.IsKeyDown(Keys.Right))
            {
                float X = ks.IsKeyDown(Keys.Left) ? 0 : GraphicsDevice.Viewport.Width;

                float num = 1f;

                if ((double)speedTimer > 0.0)
                    num = 30f / speedTimer;

                if (!world.BodyList.Contains(ball))
                    return;

                ball.AngularVelocity = (float)(((double)X - 400.0) / -20.0) * num;

                var firstFixture = ball.FixtureList.First();
                firstFixture.Friction = 10f * num;

                if ((double)speedTimer > 0.0)
                    firstFixture.UserData = new Color((int)byte.MaxValue, 0, (int)byte.MaxValue - (int)byte.MaxValue * (int)speedTimer / 30);
                else
                    firstFixture.UserData = new Color((int)byte.MaxValue, 0, 0);

                //Why this also? Angular Velocity is enough?
                //ball.ApplyForce(new Vector2((float)(((double)X - 400.0) / 8.0), 0.0f));
            }
            else
                ball.AngularVelocity = 0.0f;
        }

        private void Jump()
        {
            if (!touchedGround)
                return;

            ball.ApplyLinearImpulse(new Vector2(0.0f, 100f));

            touchedGround = false;
        }

        private void MakeFallingItem()
        {
            int num1 = rand.Next(10, 20);
            float num2 = (float)((double)num1 / 17.0 - 33.3333015441895);
            float num3 = (float)(33.3333015441895 - (double)num1 / 17.0);
            if (rand.Next(1, 20) == 1)
                num2 = num3 = ball.Position.X;
            int num4 = rand.Next(250);
            if (num4 == 0)
            {
                ShieldPowerup shieldPowerup = new ShieldPowerup(world, new Vector2((float)rand.Next((int)num2, (int)num3), 66.6666f / GraphicsDevice.Viewport.AspectRatio));
            }
            else if (num4 <= 3)
            {
                ExplosionPowerup explosionPowerup = new ExplosionPowerup(world, new Vector2((float)rand.Next((int)num2, (int)num3), 66.6666f / GraphicsDevice.Viewport.AspectRatio));
            }
            else if (num4 <= 5)
            {
                SpeedPowerup speedPowerup = new SpeedPowerup(world, new Vector2((float)rand.Next((int)num2, (int)num3), 66.6666f / GraphicsDevice.Viewport.AspectRatio));
            }
            else if (num4 <= 8)
            {
                TeleportPowerup teleportPowerup = new TeleportPowerup(world, new Vector2((float)rand.Next((int)num2, (int)num3), 66.6666f / GraphicsDevice.Viewport.AspectRatio));
            }
            else if (num4 <= 11)
            {
                LazerPowerup lazerPowerup = new LazerPowerup(world, new Vector2((float)rand.Next((int)num2, (int)num3), 66.6666f / GraphicsDevice.Viewport.AspectRatio));
            }
            else
                bombs.Add(new Bomb(new Vector2((float)rand.Next((int)num2, (int)num3), 66.6666f / GraphicsDevice.Viewport.AspectRatio), (float)num1, world, rand.Next(50, 120), (float)(rand.NextDouble() - 0.5) / 5f));
        }

        private void AddToFloorLength()
        {
            FixtureFactory.AttachPolygon(new Vertices((IList<Vector2>)new List<Vector2>()
      {
        new Vector2(-33.3333f, currentFloorLength - (float) (66.6666030883789 / (double) GraphicsDevice.Viewport.AspectRatio / 2.0)),
        new Vector2(33.3333f, currentFloorLength - (float) (66.6666030883789 / (double) GraphicsDevice.Viewport.AspectRatio / 2.0)),
        new Vector2(33.3333f, currentFloorLength),
        new Vector2(-33.3333f, currentFloorLength)
      }), 1f, floorBody, (object)Game1.floorColor);
            currentFloorLength = currentFloorLength - (float)(66.6666030883789 / (double)GraphicsDevice.Viewport.AspectRatio / 2.0);
            FixtureFactory.AttachRectangle(2f, 66.6666f, 1f, new Vector2(35.3333f, currentFloorLength), floorBody, (object)new Color(0, 0, 0)).Friction = 0.0f;
            FixtureFactory.AttachRectangle(2f, 66.6666f, 1f, new Vector2(-35.3333f, currentFloorLength), floorBody, (object)new Color(0, 0, 0)).Friction = 0.0f;
            for (int index = 0; (double)index < (score + 40.0) / 20.0 && mines.Count < 20; ++index)
            {
                if (rand.Next(3) == 1)
                    mines.Add(new Mine(new Vector2((float)(rand.Next(66) - 33), (float)rand.Next((int)currentFloorLength, (int)((double)currentFloorLength + 66.6666030883789 / (double)GraphicsDevice.Viewport.AspectRatio / 2.0 - 2.0))), world, rand));
            }
        }

        public void BlowUpBomb(Bomb bomb)
        {
            world.RemoveBody(bomb.Body);
            bombs.Remove(bomb);
            CreateBombExplosion(bomb.Body.Position, bomb.Size, true);
            if ((double)bomb.Body.Position.Y - (double)bomb.Size / 2.0 < (double)currentFloorLength)
                AddToFloorLength();
            List<Vertices> circleVertList = new List<Vertices>();
            Vertices circle = PolygonTools.CreateCircle(bomb.Size / 2f, 12);
            Vector2 position = bomb.Body.Position;
            circle.Translate(ref position);
            circleVertList.Add(circle);
            TestMines(bomb.Size, bomb.Body.Position, circleVertList, true);
            foreach (Vertices verts in circleVertList)
                CutOutShapeFromFloor(verts);
            ball.SleepingAllowed = false;
        }

        private void CutOutShapeFromFloor(Vertices verts)
        {
            AABB collisionBox = verts.GetAABB();
            List<Fixture> list1 = new List<Fixture>();
            List<Vertices> list2 = new List<Vertices>();
            List<Vertices> list3 = new List<Vertices>();
            foreach (Fixture floorFixture in floorBody.FixtureList)
            {
                AABB aabb;
                floorFixture.GetAABB(out aabb, 0);
                if (AABB.TestOverlap(ref collisionBox, ref aabb))
                {
                    list3.Clear();
                    DeconstructCompoundPolygon(Game1.CutOutPoly(verts, floorFixture), (ICollection<Vertices>)list3);
                    list3.ForEach(new Action<Vertices>(list2.Add));
                    list1.Add(floorFixture);
                }
            }
            list1.ForEach(new Action<Fixture>(floorBody.DestroyFixture));
            list2.ForEach((Action<Vertices>)(newverts =>
           {
               if (!newverts.IsSimple() || newverts.Count <= 2)
                   return;
               bool flag = true;
               foreach (Vector2 vector2_1 in (List<Vector2>)newverts)
               {
                   foreach (Vector2 vector2_2 in (List<Vector2>)newverts)
                   {
                       if (vector2_1 != vector2_2 && (double)Vector2.Distance(vector2_1, vector2_2) < 0.001)
                           flag = false;
                   }
               }
               if (!flag)
                   return;
               FixtureFactory.AttachPolygon(newverts, 1f, floorBody, (object)Game1.floorColor);
           }));
        }

        private void TestMines(float explosionSize, Vector2 explosionPosition, List<Vertices> circleVertList, bool effectPlayer)
        {
            foreach (Mine mine in Enumerable.ToList<Mine>((IEnumerable<Mine>)mines))
            {
                if (mines.Contains(mine) && (double)Vector2.Distance(explosionPosition, mine.Body.Position) < (double)explosionSize / 1.7 + (double)mine.Size / 10.0)
                {
                    CreateBombExplosion(mine.Body.Position, (float)mine.Size, effectPlayer);
                    Vertices circle = PolygonTools.CreateCircle((float)mine.Size / 1.5f, 12);
                    Vector2 position = mine.Body.Position;
                    circle.Translate(ref position);
                    mines.Remove(mine);
                    world.RemoveBody(mine.Body);
                    if ((double)mine.Body.Position.Y - (double)mine.Size < (double)currentFloorLength)
                        AddToFloorLength();
                    circleVertList.Add(circle);
                    TestMines((float)mine.Size, mine.Body.Position, circleVertList, effectPlayer);
                }
            }
        }

        private static IEnumerable<Vertices> CutOutPoly(Vertices circleVert, Fixture floorFixture)
        {
            PolyClipError error;
            return (IEnumerable<Vertices>)YuPengClipper.Difference(((PolygonShape)floorFixture.Shape).Vertices, circleVert, out error);
        }

        private void CreateBombExplosion(Vector2 position, float force, bool effectPlayer)
        {
            foreach (Body body in world.BodyList)
            {
                if ((body != ball || effectPlayer) && position != ball.Position)
                {
                    Vector2 vector2 = position - ball.Position;
                    float num = Math.Abs(20000f * force / vector2.LengthSquared());
                    Vector2 force1 = Vector2.Normalize(vector2) * -num;
                    body.ApplyForce(force1);
                }
            }
            explosions.Add(new Explosion(force, ConvertToScreenCords(position), position, rand));
            if (!effectPlayer)
                return;
            Vector2 vector2_1 = position - ball.Position;
            PrimitiveDrawing.Shake(force / 10f, 0.5f);
            //TODO: Vibrate
            //if (OptionScreen.VibrateEnabled)
            //    VibrateController.get_Default().Start(new TimeSpan(0, 0, 0, 0, 500 / (int)vector2_1.LengthSquared()));
            CheckIfBallDead(position, force);
        }

        private Vector2 ConvertToScreenCords(Vector2 point)
        {
            Vector2 vector2 = new Vector2(point.X + 33.3333f, (float)(-(double)point.Y + 66.6666030883789 / (double)GraphicsDevice.Viewport.AspectRatio / 2.0) + ball.Position.Y);
            vector2.X = (float)((double)vector2.X * (double)GraphicsDevice.Viewport.Width / 66.6666030883789);
            vector2.Y = (float)((double)vector2.Y * (double)GraphicsDevice.Viewport.Height / (66.6666030883789 / (double)GraphicsDevice.Viewport.AspectRatio));
            return vector2;
        }

        private void DeconstructCompoundPolygon(IEnumerable<Vertices> newPolys, ICollection<Vertices> newFloorVerts)
        {
            foreach (Vertices vertices in newPolys)
            {
                if (vertices.Count > 2)
                {
                    if (vertices.IsConvex())
                    {
                        if (vertices.Count >= FarseerPhysics.Settings.MaxPolygonVertices)
                        {
                            newFloorVerts.Add(vertices);
                            continue;
                        }
                    }

                    try
                    {
                        FarseerPhysics.Common.Decomposition.Triangulate.ConvexPartition(vertices, FarseerPhysics.Common.Decomposition.TriangulationAlgorithm.Bayazit).ForEach(new Action<Vertices>(newFloorVerts.Add));
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        newFloorVerts.Add(vertices);
                    }
                }
            }
        }

        private void CheckIfBallDead(Vector2 position, float force)
        {
            Vector2 vector2 = new Vector2(ball.Position.X, ball.Position.Y);
            if ((double)Vector2.Distance(position, vector2) >= (double)force / 3.0)
                return;

            //Uncomment following line for god mode
            //return;

            if (!shielded)
            {
                if (!world.BodyList.Contains(ball))
                    return;
                world.RemoveBody(ball);
                timeDead = DateTime.Now;
            }
            else
                shielded = false;
        }

        private void FigureScore()
        {
            if (ball.Position.Y < -score)
            {
                int num = (int)score;
                int? nullable = lastErasedPoint;
                if ((num != nullable.GetValueOrDefault() ? 1 : (!nullable.HasValue ? 1 : 0)) != 0)
                    lastErasedPoint = new int?();
                score = -(double)ball.Position.Y;
                lastUppedScore = timeSinceStart;
                if (!lastErasedPoint.HasValue && (int)score % 20 == 0)
                {
                    lastErasedPoint = new int?((int)score);
                    CutOutShapeFromFloor(PolygonTools.CreateRectangle(66.6666f, 20f, new Vector2(0.0f, (float)-score + 60f), 0.0f));
                }
            }

            if (score <= highScore)
                return;

            highScore = score;

            highScoreLine.Position = new Vector2(0.0f, (float)(-highScore - 66.6666030883789 / (double)GraphicsDevice.Viewport.AspectRatio / 80.0) - Enumerable.First<Fixture>((IEnumerable<Fixture>)ball.FixtureList).Shape.Radius);
        }
    }
}