using System;
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

        public DateTime DetonationTime { get; set; }

        public Explosive()
        {
        }
    }
}
