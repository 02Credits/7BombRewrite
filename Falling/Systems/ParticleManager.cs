using Falling.Components;
using Falling.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Systems
{
    public class ParticleManager : IUpdatedEntitySystem
    {
        static List<Type> subscribedComponentTypes = new List<Type>
        {
            typeof(Particle)
        };
        public List<Type> SubscribedComponentTypes { get { return subscribedComponentTypes; } }

        public void Update(Entity entity)
        {
            var particle = entity.GetComponent<Particle>();
            if (entity.HasComponent<Transform>())
            {
                var transform = entity.GetComponent<Transform>();
                transform.Position += particle.Velocity;
                transform.Rotation += particle.RotationRate;
            }

            if (entity.HasComponent<ColorTint>())
            {
                var colorTint = entity.GetComponent<ColorTint>();

                if (colorTint.Color.A > particle.FadeRate)
                {
                    colorTint.Color = new Color(colorTint.Color, (int)(colorTint.Color.A - particle.FadeRate));
                }
                else
                {
                    Game.RemoveEntity(entity);
                }
            }
        }
    }
}
