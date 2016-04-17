using Falling.Components;
using Falling.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Systems
{
    class ExplosionManager : IUpdatedSystem, IUpdatedEntitySystem
    {
        private DateTime nextBomb;

        public ExplosionManager()
        {
            nextBomb = DateTime.Now.AddSeconds(Game.random.NextDouble() * 1);
        }

        static List<Type> subscribedComponentTypes = new List<Type>
        {
            typeof(Explosive)
        };

        public List<Type> SubscribedComponentTypes { get { return subscribedComponentTypes; } }

        public void Update()
        {
            if(DateTime.Now >= nextBomb)
            {
                new Entity(
                    new Textured { Path = "WhitePixel" },
                    new ColorTint { Color = new Color(0, 255, 0) },
                    new Transform { Position = new Vector2((float)((Game.random.NextDouble() * 8) - 4), 30) },
                    new Dimensions { Width = 0.25f, Height = 0.25f },
                    new Physics { Shape = PhysicsShape.Circle, AngularDamping = 2f, Friction = 0.4f },
                    new TrimmedSprite(),
                    new Explosive() { DetonationTime = DateTime.Now.AddSeconds((Game.random.NextDouble() * 5) + 2) }
                );

                nextBomb = DateTime.Now.AddSeconds(Game.random.NextDouble() * 1);
            }
        }

        public void Update(Entity entity)
        {
            var explosive = entity.GetComponent<Explosive>();
            
            if(DateTime.Now >= explosive.DetonationTime.AddSeconds(-1))
                entity.GetComponent<ColorTint>().Color = new Color(255, 0, 0);

            if (DateTime.Now >= explosive.DetonationTime)
            {
                var destructableManager = Game.GetSystem<DestructableManager>();

                var verticesToCut = FarseerPhysics.Common.PolygonTools.CreateCircle(1, 20);
                verticesToCut.Translate(entity.GetComponent<Physics>().Body.Position);
                destructableManager.VerticesToCut.Add(verticesToCut);

                Game.RemoveEntity(entity);
            }
        }
    }
}
