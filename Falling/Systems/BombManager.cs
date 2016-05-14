using Falling.Components;
using Falling.Interfaces;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Systems
{
    class BombManager : IInitializedEntitySystem, IUpdatedSystem, IUpdatedEntitySystem
    {
        static List<Type> subscribedComponentTypes = new List<Type>
        {
            typeof(Explosive)
        };
        public List<Type> SubscribedComponentTypes { get { return subscribedComponentTypes; } }

        public void Initialize(Entity entity)
        {
            entity.GetComponent<Physics>().Body.OnCollision += Collision;
        }

        private bool Collision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            var myEntity = fixtureA.Body.UserData as Entity;
            var explosive = myEntity.GetComponent<Explosive>();
            if (!explosive.Triggered)
            {
                var otherEntity = fixtureB.Body.UserData as Entity;
                if (otherEntity != null)
                {
                    if (otherEntity.HasComponent<Trigger>())
                    {
                        explosive.Triggered = true;
                        explosive.TriggeredTime = Game.Time;
                    }
                }
            }
            return true;
        }

        public void Update()
        {
            if (Game.Random.Next(1000) <= 10)
            {
                Game.Bomb((float)Game.Random.NextDouble() * 1.5f + 0.75f,
                    new Vector2((float)((Game.Random.NextDouble() * 8) - 4), 10));
            }
        }

        public void Update(Entity entity)
        {
            var explosive = entity.GetComponent<Explosive>();

            if (explosive.Triggered)
            {
                if (Game.Time - explosive.TriggeredTime > explosive.FuseTime)
                {
                    var destructableManager = Game.GetSystem<DestructableManager>();

                    var verticesToCut = FarseerPhysics.Common.PolygonTools.CreateCircle(explosive.ExplosionRadius, 20);
                    verticesToCut.Translate(entity.GetComponent<Physics>().Body.Position);
                    destructableManager.VerticesToCut.Add(verticesToCut);

                    var entitiesInRange = new List<Entity>();
                    var myPosition = entity.GetComponent<Transform>().Position;
                    foreach (var otherEntity in Game.Entities)
                    {
                        if (otherEntity.HasComponent<Collectable>())
                        {
                            var otherPosition = otherEntity.GetComponent<Transform>().Position;
                            var distance = Vector2.Distance(myPosition, otherPosition);
                            if (distance <= explosive.ExplosionRadius)
                                entitiesInRange.Add(otherEntity);
                        }
                    }

                    entitiesInRange.ForEach(o => Game.RemoveEntity(o));

                    Game.RemoveEntity(entity);
                }
                else
                {
                    var timeLeftPercentage = (Game.Time - explosive.TriggeredTime) / explosive.FuseTime;
                    entity.GetComponent<ColorTint>().Color = new Color(255, (int)((1.0f - timeLeftPercentage) * 255.0f), (int)((1.0f - timeLeftPercentage) * 255.0f));
                }
            }
        }
    }
}