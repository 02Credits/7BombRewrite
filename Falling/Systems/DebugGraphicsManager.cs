using Falling.Interfaces;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Systems
{
    public class DebugGraphicsManager : IDrawnSystem
    {
        public void Draw()
        {
            var physicsBodies = Game.GetSystem<PhysicsManager>().World.BodyList;
            var vertexManager = Game.GetSystem<VertexManager>();

            foreach (var body in physicsBodies)
            {
                var verticesList = Utils.GetVerticesFromBody(body);

                foreach (var vertices in verticesList)
                {
                    vertexManager.AddDebugVertices(vertices, Color.GreenYellow, body.Position, body.Rotation);
                }
            }
        }
    }
}
