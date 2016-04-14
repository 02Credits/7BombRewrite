using Falling.Components;
using Falling.Interfaces;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Systems
{
    public class DestructableManager : IUpdatedSystem, IUpdatedEntitySystem
    {
        static List<Type> subscribedComponentTypes = new List<Type>
        {
            typeof(Destructable)
        };
        public List<Type> SubscribedComponentTypes { get { return subscribedComponentTypes; } }

        // This is to handle the fact that update occurs before entity update.
        // Eventually I might change the system management to deal with this issue
        // but for now Im just doing a simple list swap until it becomes an issue again
        List<Vertices> polyList1 = new List<Vertices>();
        List<Vertices> polyList2 = new List<Vertices>();
        bool firstList;

        public List<Vertices> VerticesToCut
        {
            get
            {
                if (firstList)
                {
                    return polyList1;
                }
                else
                {
                    return polyList2;
                }
            }
        }
        private List<Vertices> CurrentVertices
        {
            get
            {
                if (!firstList)
                {
                    return polyList1;
                }
                else
                {
                    return polyList2;
                }
            }
        }

        public void Update()
        {
            firstList = !firstList;
            VerticesToCut.Clear();
        }

        public void Update(Entity entity)
        {
            if (CurrentVertices.Count > 0)
            {
                var physics = entity.GetComponent<Physics>();
                var fullVertices = entity.GetComponent<Destructable>().FullVerticesSet;
                bool cutSuccess = false;
                foreach (var polygon in CurrentVertices)
                {
                    List<Vertices> verticesToAdd = new List<Vertices>();
                    List<Vertices> verticesToRemove = new List<Vertices>();
                    foreach (var vertices in fullVertices)
                    {
                        polygon.Translate(-physics.Body.Position);
                        polygon.Rotate(-physics.Body.Rotation);
                        var polyAABB = polygon.GetAABB();
                        var physicsAABB = vertices.GetAABB();
                        if (AABB.TestOverlap(ref polyAABB, ref physicsAABB))
                        {
                            PolyClipError error;
                            var results = YuPengClipper.Difference(vertices, polygon, out error);
                            if (error == PolyClipError.None)
                            {
                                cutSuccess = true;
                                verticesToAdd.AddRange(results);
                                verticesToRemove.Add(vertices);
                            }
                        }

                        polygon.Rotate(physics.Body.Rotation);
                        polygon.Translate(physics.Body.Position);
                    }

                    foreach (var vertices in verticesToRemove)
                    {
                        fullVertices.Remove(vertices);
                    }

                    fullVertices.AddRange(verticesToAdd);
                }

                if (cutSuccess)
                {
                    foreach (var fixture in physics.Body.FixtureList.ToList())
                    {
                        physics.Body.DestroyFixture(fixture);
                    }

                    var concaveVertices = new List<Vertices>();
                    foreach (var vertices in fullVertices)
                    {
                        var decomposedVertices = Triangulate.ConvexPartition(vertices, TriangulationAlgorithm.Earclip);
                        concaveVertices.AddRange(decomposedVertices);
                    }
                    var verticesBigEnough = new List<Vertices>();
                    foreach (var vertices in concaveVertices)
                    {
                        if (vertices.GetSignedArea() > 0.001f)
                        {
                            verticesBigEnough.Add(vertices);
                        }
                    }
                    FixtureFactory.AttachCompoundPolygon(verticesBigEnough, 1, physics.Body);

                    if (physics.Body.FixtureList.Count == 0)
                    {
                        Game.RemoveEntity(entity);
                    }
                }
            }
        }
    }
}
