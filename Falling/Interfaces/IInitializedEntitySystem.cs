using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Interfaces
{
    public interface IInitializedEntitySystem
    {
        List<Type> SubscribedComponentTypes { get; }
        void Initialize(Entity entity);
    }
}
