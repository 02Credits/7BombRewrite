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
    class CoinManager : IInitializedEntitySystem, IUpdatedSystem, IUpdatedEntitySystem
    {
        static List<Type> subscribedComponentTypes = new List<Type>
        {
            typeof(Collectable)
        };
        public List<Type> SubscribedComponentTypes { get { return subscribedComponentTypes; } }

        public void Initialize(Entity entity)
        {
            entity.GetComponent<Physics>().Body.OnCollision += Collision;
        }

        private bool Collision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            var myEntity = fixtureA.Body.UserData as Entity;
            var collectable = myEntity.GetComponent<Collectable>();

            var otherEntity = fixtureB.Body.UserData as Entity;
            if (otherEntity != null)
            {
                if (otherEntity.HasComponent<PlayerControlled>())
                {
                    Game.RemoveEntity(myEntity);
                }
            }

            return true;
        }

        public void Update()
        {
            if (Game.Random.Next(1000) <= 10)
            {
                Game.Coin(new Vector2((float)((Game.Random.NextDouble() * 8) - 4), 10));
            }
        }

        public void Update(Entity entity)
        {
        }
    }
}