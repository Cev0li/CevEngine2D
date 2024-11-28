using cevEngine2D.source.world.units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cevEngine2D.source.engine.interfaces {
    internal interface ICollisionManager {
        public void CheckCollisions(Unit unit) { }
        public event Action<String> CollisionEvent;

    }
}
