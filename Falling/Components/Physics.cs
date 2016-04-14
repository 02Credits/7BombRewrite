using FarseerPhysics.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Components
{
    public enum PhysicsShape
    {
        Circle,
        Rectangle,
        Texture
    }

    public class Physics : Component
    {
        public Body Body { get; set; }

        public PhysicsShape Shape { get; set; }
        public bool Static { get; set; }
        public bool FixedAngle { get; set; }
        public float AngularDamping { get; set; }
        public float Friction { get; set; }

        public Physics()
        {
            Friction = 0.5f;
        }
    }
}
