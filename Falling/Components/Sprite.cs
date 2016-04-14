using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Components
{
    public class Sprite : Component
    {
        static List<Type> requiredComponents = new List<Type>
        {
            typeof(Transform),
            typeof(Textured),
            typeof(Dimensions)
        };

        public override List<Type> RequiredComponents { get { return requiredComponents; } }
    }
}
