using Falling.Components;
using Falling.Interfaces;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Common.TextureTools;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Systems
{
    public class PhysicsManager : IInitializedEntitySystem, IUpdatedSystem, IUpdatedEntitySystem, IDeconstructedEntitySystem
    {
        static List<Type> subscribedComponentTypes = new List<Type>
        {
            typeof(Physics),
            typeof(Destructable)
        };
        public List<Type> SubscribedComponentTypes { get{ return subscribedComponentTypes; } }

        public World World { get; set; }

        public PhysicsManager()
        {
            World = new World(new Vector2(0, -9.81f));
        }

        public void Initialize(Entity entity)
        {
            var physics = entity.GetComponent<Physics>();
            var dimensions = entity.GetComponent<Dimensions>();
            CreateBody(entity, physics, dimensions);

            var transform = entity.GetComponent<Components.Transform>();
            physics.Body.Position = transform.Position;
            physics.Body.Rotation = transform.Rotation;

            if (physics.Static)
            {
                physics.Body.BodyType = BodyType.Static;
            }
            else
            {
                physics.Body.BodyType = BodyType.Dynamic;
            }

            physics.Body.FixedRotation = physics.FixedAngle;
            physics.Body.AngularDamping = physics.AngularDamping;
            physics.Body.Friction = physics.Friction;

            if (entity.HasComponent<Destructable>())
            {
                entity.GetComponent<Destructable>().FullVerticesSet = physics.Body.FixtureList.Select(fixture => ((PolygonShape)fixture.Shape).Vertices).ToList();
            }

            physics.Body.UserData = entity;
        }

        public void Update()
        {
            World.Step(0.033333f);
        }

        public void Update(Entity entity)
        {
            var body = entity.GetComponent<Physics>().Body;
            var transform = entity.GetComponent<Components.Transform>();
            transform.Position = body.Position;
            transform.Rotation = body.Rotation;
        }

        public void Deconstruct(Entity entity)
        {
            var physics = entity.GetComponent<Physics>();
            World.RemoveBody(physics.Body);
        }

        private void CreateBody(Entity entity, Physics physics, Dimensions dimensions)
        {

            if (physics.Shape == PhysicsShape.Texture)
            {
                CreateTextureBasedFixture(entity, physics, dimensions);

            }
            else if (physics.Shape == PhysicsShape.Circle)
            {
                physics.Body = BodyFactory.CreateCircle(World, dimensions.Width / 2, 1);
            }
            else if (physics.Shape == PhysicsShape.Rectangle)
            {
                physics.Body = BodyFactory.CreateRectangle(World, dimensions.Width, dimensions.Height, 1);
            }

            if (entity.HasComponent<GridPhysics>())
            {
                Gridify(entity, physics, dimensions);
            }
        }

        private void CreateTextureBasedFixture(Entity entity, Physics physics, Dimensions dimensions)
        {
            var physicsTexture = Game.GetSystem<TextureManager>().Textures[entity.GetComponent<PhysicsSource>().Path];
            var data = new uint[physicsTexture.Width * physicsTexture.Height];
            physicsTexture.GetData(data);
            var textureVertices = PolygonTools.CreatePolygon(data, physicsTexture.Width, true);
            textureVertices.Scale(new Vector2(dimensions.Width / (float)physicsTexture.Width, dimensions.Height / (float)physicsTexture.Height));
            textureVertices.Translate(new Vector2(-dimensions.Width / 2, -dimensions.Height / 2));
            var verticesList = Triangulate.ConvexPartition(textureVertices, TriangulationAlgorithm.Bayazit);
            physics.Body = BodyFactory.CreateCompoundPolygon(World, verticesList, 1);
        }

        private static void Gridify(Entity entity, Physics physics, Dimensions dimensions)
        {
            var newVertices = new List<Vertices>();
            var fixturesToRemove = new List<Fixture>();
            var gridSize = entity.GetComponent<GridPhysics>().GridSize;
            for (float x = -dimensions.Width / 2; x < dimensions.Width / 2; x += gridSize)
            {
                for (float y = -dimensions.Height / 2; y < dimensions.Height / 2; y += gridSize)
                {
                    var gridSquare = PolygonTools.CreateRectangle(gridSize / 2, gridSize / 2, new Vector2(x + gridSize / 2, y + gridSize / 2), 0);
                    var squareAABB = gridSquare.GetAABB();
                    foreach (var fixture in physics.Body.FixtureList)
                    {
                        var physicsVertices = ((PolygonShape)fixture.Shape).Vertices;
                        var physicsAABB = physicsVertices.GetAABB();
                        PolyClipError error;
                        var results = YuPengClipper.Intersect(physicsVertices, gridSquare, out error);

                        if (error == PolyClipError.None)
                        {
                            newVertices.AddRange(results);
                            fixturesToRemove.Add(fixture);
                        }
                    }
                }
            }

            foreach (var fixture in fixturesToRemove)
            {
                if (physics.Body.FixtureList.Contains(fixture))
                {
                    physics.Body.DestroyFixture(fixture);
                }
            }
            FixtureFactory.AttachCompoundPolygon(newVertices, 1, physics.Body);
        }
    }
}
