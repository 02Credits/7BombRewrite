using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Components
{
    public class Jumpable : Component
    {
        static List<Type> requiredComponents = new List<Type>
        {
            typeof(Physics)
        };
        public override List<Type> RequiredComponents { get { return requiredComponents; } }
    }
}
