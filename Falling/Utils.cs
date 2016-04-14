using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling
{
    public static class Utils
    {
        public static List<Vertices> GetVerticesFromBody(Body body)
        {
            List<Vertices> returnList = new List<Vertices>();

            foreach (var fixture in body.FixtureList)
            {
                if (fixture.Shape is PolygonShape)
                {
                    returnList.Add(((PolygonShape)fixture.Shape).Vertices);
                }
                else if (fixture.Shape is CircleShape)
                {
                    returnList.Add(PolygonTools.CreateCircle(((CircleShape)fixture.Shape).Radius, 20));
                }
                else if (fixture.Shape is EdgeShape)
                {
                    var edge = (EdgeShape)fixture.Shape;
                    var vertices = new Vertices();
                    vertices.Add(edge.Vertex1);
                    vertices.Add(edge.Vertex2);
                    returnList.Add(vertices);
                }
            }

            return returnList;
        }
    }
}
