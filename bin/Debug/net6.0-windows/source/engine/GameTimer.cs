using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace cevEngine2D.source.engine {
    public class GameTimer {
        private int _mSec;
        public int MilliSeconds { get { return _mSec; } }
        private TimeSpan timer = new TimeSpan();
        public int Timer { get { return (int)timer.TotalMilliseconds; } }

        public GameTimer(int mSec) {
            _mSec = mSec;
        }

        public void UpdateTimer() {
            timer += Globals.gameTime.ElapsedGameTime;
        }

        public bool Test() {
            if (timer.TotalMilliseconds >= _mSec) {
                return true;
            } else {
                return false;
            }
        }

        public void ResetToZero() {
            timer = TimeSpan.Zero;
        }

        public void SetTimer(TimeSpan time) {
            timer = time;
        }
    }
}
