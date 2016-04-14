using Falling.Components;
using Falling.Interfaces;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Systems
{
    public class PlayerControlManager : IUpdatedEntitySystem
    {
        static List<Type> subscribedComponentTypes = new List<Type>
        {
            typeof(PlayerControlled)
        };
        public List<Type> SubscribedComponentTypes { get { return subscribedComponentTypes; } }

        public void Update(Entity entity)
        {
            var physics = entity.GetComponent<Physics>();
            var playerControlled = entity.GetComponent<PlayerControlled>();
            var keyboard = Keyboard.GetState();

            ApplyRotation(physics, playerControlled.RotationSpeed, keyboard);

            var world = Game.GetSystem<PhysicsManager>().World;
            var dimensions = entity.GetComponent<Dimensions>();

            if (OnGround(world, physics.Body, physics.Body.Position, dimensions.Width / 2))
            {
                if (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Up))
                {
                    var volume = dimensions.Width*dimensions.Height;
                    physics.Body.ApplyLinearImpulse(new Vector2(0, playerControlled.JumpForceRatio * volume));
                }
            }

            if (keyboard.IsKeyDown(Keys.X))
            {
                var destructableManager = Game.GetSystem<DestructableManager>();
                var verticesToCut = PolygonTools.CreateCircle(2, 20);
                verticesToCut.Translate(physics.Body.Position);
                destructableManager.VerticesToCut.Add(verticesToCut);
            }
        }

        private bool OnGround(World world, Body playerBody, Vector2 position, float radius)
        {
            var fixtureList = new List<Fixture>();
            var p1 = new Vector2(-radius / 2, -radius - 0.1f);
            var p2 = new Vector2(radius / 2, -radius - 0.1f);
            fixtureList.AddRange(world.RayCast(position + p1, position + p2));
            fixtureList.AddRange(world.RayCast(position + p2, position + p1));
            p1 = new Vector2(-radius / 2, -radius - 0.1f);
            p2 = new Vector2(-radius, 0);
            fixtureList.AddRange(world.RayCast(position + p1, position + p2));
            fixtureList.AddRange(world.RayCast(position + p2, position + p1));
            p1 = new Vector2(radius / 2, -radius - 0.1f);
            p2 = new Vector2(radius, 0);
            fixtureList.AddRange(world.RayCast(position + p1, position + p2));
            fixtureList.AddRange(world.RayCast(position + p2, position + p1));

            return fixtureList.Where((f) => f.Body != playerBody).Any();
        }

        private void ApplyRotation(Physics physics, float maxSpeed, KeyboardState keyboard)
        {
            var rotationSpeed = 0f;
            if (keyboard.IsKeyDown(Keys.Left))
            {
                rotationSpeed += maxSpeed;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                rotationSpeed -= maxSpeed;
            }
            if (rotationSpeed != 0)
            {
                physics.Body.AngularVelocity = rotationSpeed;
            }
        }
    }
}
