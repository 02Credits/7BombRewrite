using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Interfaces
{
    public interface IUnloadedEntitySystem
    {
        List<Type> SubscribedComponentTypes { get; }
        void Unload(Entity entity);
    }
}
