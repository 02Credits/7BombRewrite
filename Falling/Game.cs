#region Using Statements
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Falling.Interfaces;
using Falling.Systems;
using Falling.Components;
#endregion

namespace Falling
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static void InitialEntities()
        {
            new Entity(
                new Textured { Path = "DebugCircle" },
                new ColorTint { Color = new Color(255, 0, 0) },
                new Transform { Position = new Vector2(0, 40) },
                new Dimensions { Width = 0.6f, Height = 0.6f },
                new Physics { Shape = PhysicsShape.Circle, AngularDamping = 5f, Friction = 1.5f },
                new PlayerControlled { JumpForceRatio = 5, RotationSpeed = 13, HorizontalMotion = 0.02f },
                new TrimmedSprite(),
                new CameraTarget()
            );
        }

        public static Entity Bomb(float size, Vector2 position)
        {
            return new Entity(
                new Textured { Path = "WhitePixel" },
                new ColorTint { Color = new Color(0, 255, 0) },
                new Transform { Position = position },
                new Dimensions { Width = 0.25f * size, Height = 0.1f * size },
                new Physics { Shape = PhysicsShape.Circle, AngularDamping = 2f, Friction = 0.4f },
                new Jumpable(),
                new TrimmedSprite(),
                new Explosive { FuseTime = 3, ExplosionRadius = size }
            );
        }

        public static Entity Panel(float depth)
        {
            return new Entity(
                new Textured { Path = "Debug" },
                new Transform { Position = new Vector2(0, depth - 5) },
                new Dimensions { Width = 10, Height = 10 },
                new Physics { Shape = PhysicsShape.Rectangle, Static = true },
                new GridPhysics { GridSize = 1f },
                new Jumpable(),
                new TrimmedSprite(),
                new Destructable(),
                new Trigger()
            );
        }

        #region BoilerPlate
        GraphicsDeviceManager graphics;

        public static Random Random { get; set; }
        public static float Time { get; set; }

        public static readonly List<Entity> Entities = new List<Entity>();
        public static readonly Dictionary<Type, object> Systems = new Dictionary<Type, object>();
        public static readonly List<IInitializedEntitySystem> InitializedEntitySystems = new List<IInitializedEntitySystem>();
        public static readonly List<ILoadedSystem> LoadedSystems = new List<ILoadedSystem>();
        public static readonly List<ILoadedEntitySystem> LoadedEntitySystems = new List<ILoadedEntitySystem>();
        public static readonly List<IUpdatedSystem> UpdatedSystems = new List<IUpdatedSystem>();
        public static readonly List<IUpdatedEntitySystem> UpdatedEntitySystems = new List<IUpdatedEntitySystem>();
        public static readonly List<IDrawnSystem> DrawnSystems = new List<IDrawnSystem>();
        public static readonly List<IDrawnEntitySystem> DrawnEntitySystems = new List<IDrawnEntitySystem>();
        public static readonly List<IUnloadedSystem> UnloadedSystems = new List<IUnloadedSystem>();
        public static readonly List<IUnloadedEntitySystem> UnloadedEntitySystems = new List<IUnloadedEntitySystem>();
        public static readonly List<IDeconstructedEntitySystem> DeconstructedEntitySystems = new List<IDeconstructedEntitySystem>();

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Random = new Random();
            Time = 0;
            Content.RootDirectory = @"Content/bin";
        }

        protected override void Initialize()
        {
            AddSystem(new TextureManager(Content, graphics.GraphicsDevice));
            AddSystem(new DestructableManager());
            AddSystem(new PanelManager());
            AddSystem(new PhysicsManager());
            AddSystem(new ParticleManager());
            AddSystem(new SpriteRenderer());
            AddSystem(new TrimmedSpriteRenderer());
            AddSystem(new CameraManager(graphics.GraphicsDevice));
            AddSystem(new PlayerControlManager());
            AddSystem(new DebugGraphicsManager());
            AddSystem(new VertexRenderer(graphics.GraphicsDevice));
            AddSystem(new VertexManager());
            AddSystem(new WallManager());
            AddSystem(new BombManager());

            InitialEntities();

            base.Initialize();
        }
        #endregion

        #region SubscriptionPumpers
        protected override void LoadContent()
        {
            foreach (var system in LoadedSystems)
            {
                system.Load();
            }

            foreach (var system in LoadedEntitySystems)
            {
                foreach (var gameObject in Entities.ToList())
                {
                    foreach (var subscribedComponent in system.SubscribedComponentTypes)
                    {
                        if (gameObject.Components.ContainsKey(subscribedComponent))
                        {
                            system.Load(gameObject);
                            break;
                        }
                    }
                }
            }

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            Time += (gameTime.ElapsedGameTime.Milliseconds / 1000f);
            foreach (var system in UpdatedSystems)
            {
                system.Update();
            }

            foreach (var system in UpdatedEntitySystems)
            {
                foreach (var gameObject in Entities.ToList())
                {
                    foreach (var subscribedComponent in system.SubscribedComponentTypes)
                    {
                        if (gameObject.Components.ContainsKey(subscribedComponent))
                        {
                            system.Update(gameObject);
                            break;
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            foreach (var system in DrawnSystems)
            {
                system.Draw();
            }

            foreach (var system in DrawnEntitySystems)
            {
                foreach (var gameObject in Entities.ToList())
                {
                    foreach (var subscribedComponent in system.SubscribedComponentTypes)
                    {
                        if (gameObject.Components.ContainsKey(subscribedComponent))
                        {
                            system.Draw(gameObject);
                            break;
                        }
                    }
                }
            }

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            foreach (var system in UnloadedSystems)
            {
                system.Unload();
            }

            foreach (var system in UnloadedEntitySystems)
            {
                foreach (var gameObject in Entities.ToList())
                {
                    foreach (var subscribedComponent in system.SubscribedComponentTypes)
                    {
                        if (gameObject.Components.ContainsKey(subscribedComponent))
                        {
                            system.Unload(gameObject);
                            break;
                        }
                    }
                }
            }

            base.UnloadContent();
        }
        #endregion

        #region SystemManagement
        public static void AddSystem(object system)
        {
            var initializedEntitySystem = system as IInitializedEntitySystem;
            if (initializedEntitySystem != null)
            {
                InitializedEntitySystems.Add(initializedEntitySystem);
            }

            var loadedSystem = system as ILoadedSystem;
            if (loadedSystem != null)
            {
                LoadedSystems.Add(loadedSystem);
            }
            var loadedEntitySystem = system as ILoadedEntitySystem;
            if (loadedEntitySystem != null)
            {
                LoadedEntitySystems.Add(loadedEntitySystem);
            }

            var updatedSystem = system as IUpdatedSystem;
            if (updatedSystem != null)
            {
                UpdatedSystems.Add(updatedSystem);
            }

            var updatedEntitySystem = system as IUpdatedEntitySystem;
            if (updatedEntitySystem != null)
            {
                UpdatedEntitySystems.Add(updatedEntitySystem);
            }

            var drawnSystem = system as IDrawnSystem;
            if (drawnSystem != null)
            {
                DrawnSystems.Add(drawnSystem);
            }

            var drawnEntitySystem = system as IDrawnEntitySystem;
            if (drawnEntitySystem != null)
            {
                DrawnEntitySystems.Add(drawnEntitySystem);
            }

            var unloadedSystem = system as IUnloadedSystem;
            if (unloadedSystem != null)
            {
                UnloadedSystems.Add(unloadedSystem);
            }

            var unloadedEntitySystem = system as IUnloadedEntitySystem;
            if (unloadedEntitySystem != null)
            {
                UnloadedEntitySystems.Add(unloadedEntitySystem);
            }

            var deconstructedEntitySystem = system as IDeconstructedEntitySystem;
            if (deconstructedEntitySystem != null)
            {
                DeconstructedEntitySystems.Add(deconstructedEntitySystem);
            }

            Systems[system.GetType()] = system;
        }

        public static T GetSystem<T>()
        {
            var type = typeof(T);
            if (Systems.ContainsKey(type))
            {
                return (T)Systems[type];
            }
            else
            {
                return default(T);
            }
        }
        #endregion

        #region EntityManagement
        public static void AddEntity(Entity entity)
        {
            Entities.Add(entity);

            foreach (var system in InitializedEntitySystems)
            {
                foreach (var subscribedComponent in system.SubscribedComponentTypes)
                {
                    if (entity.Components.ContainsKey(subscribedComponent))
                    {
                        system.Initialize(entity);
                        break;
                    }
                }
            }
        }

        public static void InitializeEntityComponent(Entity entity, Type componentType)
        {
            foreach (var system in InitializedEntitySystems)
            {
                foreach (var subscribedComponent in system.SubscribedComponentTypes)
                {
                    if (subscribedComponent == componentType)
                    {
                        system.Initialize(entity);
                        break;
                    }
                }
            }
        }

        public static void RemoveEntity(Entity entity)
        {
            Entities.Remove(entity);

            foreach (var system in DeconstructedEntitySystems)
            {
                foreach (var subscribedComponent in system.SubscribedComponentTypes)
                {
                    if (entity.Components.ContainsKey(subscribedComponent))
                    {
                        system.Deconstruct(entity);
                        break;
                    }
                }
            }
        }
        #endregion
    }
}
