using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Interfaces
{
    public interface IDrawnEntitySystem
    {
        List<Type> SubscribedComponentTypes { get; }
        void Draw(Entity entity);
    }
}
