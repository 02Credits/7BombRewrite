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
    public class CoinManager : IInitializedSystem, IUpdatedSystem
    {
        public int CurrentScore { get; set; }

        static List<Type> subscribedComponentTypes = new List<Type>
        {
            typeof(Collectable)
        };
        public List<Type> SubscribedComponentTypes { get { return subscribedComponentTypes; } }

        public void Initialize()
        {
            CurrentScore = 0;
            Game.GetSystem<CollectableManager>().OnCollected += Collected;
        }

        private void Collected(Entity collectable)
        {
            if (collectable.HasComponent<Coin>())
            {
                CurrentScore++;
            }
        }

        public void Update()
        {
            if (Game.Random.Next(1000) <= 10)
            {
                Game.Coin(new Vector2((float)((Game.Random.NextDouble() * 8) - 4), 10));
            }
        }
    }
}