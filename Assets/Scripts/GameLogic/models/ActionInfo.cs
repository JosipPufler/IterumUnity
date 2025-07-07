using Iterum.models.interfaces;
using System.Collections;
using System.Collections.Generic;

namespace Iterum.models
{
    public class ActionInfo
    {
        public ICreature OriginCreature { get; set; }
        public IList<ITargetable> Targets { get; set; }
    }
}
