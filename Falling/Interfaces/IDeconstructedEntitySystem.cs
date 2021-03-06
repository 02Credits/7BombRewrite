﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Falling.Interfaces
{
    public interface IDeconstructedEntitySystem
    {
        List<Type> SubscribedComponentTypes { get; }
        void Deconstruct(Entity entity);
    }
}
