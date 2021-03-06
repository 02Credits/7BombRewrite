﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Components
{
    class Explosive : Component
    {
        static List<Type> requiredComponents = new List<Type>
        {
            typeof(ColorTint),
            typeof(Physics)
        };
        public override List<Type> RequiredComponents { get { return requiredComponents; } }

        public float FuseTime { get; set; }
        public float ExplosionRadius { get; set; }
        public bool Triggered { get; set; }
        public float TriggeredTime { get; set; }
    }
}
