using Falling.Components;
using Falling.Interfaces;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Systems
{
    public class CollectableManager : IInitializedEntitySystem
    {
        static List<Type> subscribedComponentTypes = new List<Type>
        {
            typeof(Collectable)
        };
        public List<Type> SubscribedComponentTypes { get { return subscribedComponentTypes; } }

        public delegate void CollectedHandler(Entity collectable);
        public event CollectedHandler OnCollected;

        public void Initialize(Entity entity)
        {
            entity.GetComponent<Physics>().Body.OnCollision += Collision;
        }

        private bool Collision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            var collectedEntity = fixtureA.Body.UserData as Entity;
            var collectable = collectedEntity.GetComponent<Collectable>();

            var otherEntity = fixtureB.Body.UserData as Entity;
            if (otherEntity != null && !collectable.Collected)
            {
                if (otherEntity.HasComponent<PlayerControlled>())
                {
                    Game.RemoveEntity(collectedEntity);
                    collectable.Collected = true;
                    OnCollected(collectedEntity);
                    return false;
                }
            }

            return true;
        }
    }
}
