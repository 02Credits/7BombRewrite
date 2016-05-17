using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Components
{
    public enum CollectableType
    {
        Coin,
        Godmode // light green triangle
    }

    public class Collectable : Component
    {
        static List<Type> requiredComponents = new List<Type>
        {
            typeof(Physics)
        };
        public override List<Type> RequiredComponents { get { return requiredComponents; } }

        public CollectableType Type { get; set; }
        public bool Collected = false;
    }
}
