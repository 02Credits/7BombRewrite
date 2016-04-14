using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Components
{
    public class Particle : Component
    {
        public Vector2 Velocity { get; set; }
        public float RotationRate { get; set; }
        public float FadeRate { get; set; }
    }
}
