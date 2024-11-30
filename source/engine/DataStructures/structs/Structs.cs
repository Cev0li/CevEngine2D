using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cevEngine2D.source.engine.DataStructures.structs {

        public struct DirectionSwitch {
            public bool NW = true;
            public bool W = true;
            public bool SW = true;
            public bool N = true;
            public bool X = true;
            public bool S = true;
            public bool NE = true;
            public bool E = true;
            public bool SE = true;

            bool[] directionSwitch;

            public DirectionSwitch() {
                bool[] constructorArr = { NW, W, SW, N, X, S, NE, E, SE };
                directionSwitch = constructorArr;
            }

            public void markCollision(int i) {
                directionSwitch[i] = !directionSwitch[i];
            }
        }

}
