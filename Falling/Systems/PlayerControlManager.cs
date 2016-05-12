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
        
        private KeyboardState previousState = Keyboard.GetState();
        public void Update(Entity entity)
        {
            var physics = entity.GetComponent<Physics>();
            var playerControlled = entity.GetComponent<PlayerControlled>();
            var keyboard = Keyboard.GetState();

            ApplyRotation(physics, playerControlled.RotationSpeed, keyboard);
            ApplyHorizontalMotion(physics, playerControlled.HorizontalMotion, keyboard);

            var world = Game.GetSystem<PhysicsManager>().World;
            var dimensions = entity.GetComponent<Dimensions>();

            if (OnGround(world, physics.Body, physics.Body.Position, dimensions.Width / 2))
            {
                if (keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Up))
                {
                    var volume = dimensions.Width * dimensions.Height;
                    physics.Body.ApplyLinearImpulse(new Vector2(0, playerControlled.JumpForceRatio * volume));
                }
            }

            if (keyboard.IsKeyDown(Keys.X) && !previousState.IsKeyDown(Keys.X))
            {
                var destructableManager = Game.GetSystem<DestructableManager>();
                var verticesToCut = PolygonTools.CreateCircle(2, 20);
                verticesToCut.Translate(physics.Body.Position);
                destructableManager.VerticesToCut.Add(verticesToCut);
            }

            previousState = keyboard;
        }

        private bool OnGround(World world, Body playerBody, Vector2 position, float radius)
        {
            var vertexManager = Game.GetSystem<VertexManager>();
            var fixtureList = new List<Fixture>();
            var p1 = new Vector2(-radius / 2, -radius - 0.05f);
            var p2 = new Vector2(radius / 2, -radius - 0.05f);
            vertexManager.AddDebugLine(position + p1, position + p2, Color.Purple);
            fixtureList.AddRange(world.RayCast(position + p1, position + p2));
            fixtureList.AddRange(world.RayCast(position + p2, position + p1));
            p1 = new Vector2(-radius / 2, -radius - 0.05f);
            p2 = new Vector2(-radius - 0.01f, 0);
            vertexManager.AddDebugLine(position + p1, position + p2, Color.Purple);
            fixtureList.AddRange(world.RayCast(position + p1, position + p2));
            fixtureList.AddRange(world.RayCast(position + p2, position + p1));
            p1 = new Vector2(radius / 2, -radius - 0.05f);
            p2 = new Vector2(radius + 0.01f, 0);
            vertexManager.AddDebugLine(position + p1, position + p2, Color.Purple);
            fixtureList.AddRange(world.RayCast(position + p1, position + p2));
            fixtureList.AddRange(world.RayCast(position + p2, position + p1));

            return fixtureList
                .Where((f) => f.Body != playerBody)
                .Where((f) => f.Body.UserData is Entity)
                .Where((f) => ((Entity)f.Body.UserData).HasComponent<Jumpable>())
                .Any();
        }

        private void ApplyHorizontalMotion(Physics physics, float horizontalMotion, KeyboardState keyboard)
        {
            var linearMultiplier = 0f;
            if (keyboard.IsKeyDown(Keys.Left))
            {
                linearMultiplier -= horizontalMotion;
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                linearMultiplier += horizontalMotion;
            }
            if (linearMultiplier != 0)
            {
                physics.Body.ApplyLinearImpulse(new Vector2(linearMultiplier, 0));
            }
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
