using FarseerPhysics.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Components
{
    public class Destructable : Component
    {
        static List<Type> requiredComponents = new List<Type>
        {
            typeof(TrimmedSprite)
        };
        public override List<Type> RequiredComponents { get { return requiredComponents; } }

        public List<Vertices> FullVerticesSet { get; set; }
    }
}
