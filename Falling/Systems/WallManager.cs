using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Falling.Interfaces;
using Falling.Components;
using Microsoft.Xna.Framework;

namespace Falling.Systems
{
    public class WallManager : IUpdatedEntitySystem
    {
        static List<Type> subscribedComponentTypes = new List<Type>
        {
            typeof(PlayerControlled)
        };
        public List<Type> SubscribedComponentTypes { get { return subscribedComponentTypes; } }

        public Entity LeftWall { get; set; }
        public Entity RightWall { get; set; }

        public WallManager()
        {
            LeftWall = new Entity(new Transform { Position = new Vector2(-6, 0) },
                                  new Dimensions { Width = 2, Height = 50 },
                                  new Physics { Shape = PhysicsShape.Rectangle, Static = true });
            RightWall = new Entity(new Transform { Position = new Vector2(6, 0) },
                                  new Dimensions { Width = 2, Height = 50 },
                                  new Physics { Shape = PhysicsShape.Rectangle, Static = true });
        }

        public void Update(Entity entity)
        {
            var playerY = entity.GetComponent<Physics>().Body.Position.Y;
            var leftWallBody = LeftWall.GetComponent<Physics>().Body;
            var rightWallBody = RightWall.GetComponent<Physics>().Body;
            leftWallBody.Position = new Vector2(leftWallBody.Position.X, playerY);
            rightWallBody.Position = new Vector2(rightWallBody.Position.X, playerY);
        }
    }
}
