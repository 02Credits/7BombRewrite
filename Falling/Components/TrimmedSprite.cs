using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Components
{
    public class TrimmedSprite : Component
    {
        static List<Type> requiredComponents = new List<Type>
        {
            typeof(Physics),
            typeof(Textured),
            typeof(Dimensions)
        };

        public override List<Type> RequiredComponents { get { return requiredComponents; } }    
    }
}
