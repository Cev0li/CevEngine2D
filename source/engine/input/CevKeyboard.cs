using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameTutorial.source.engine.input {
    public class CevKeyboard {
        public KeyboardState newKeyboard, oldKeyboard;

        public List<CevKey> pressedKeys = new List<CevKey>(), previousPressedKeys = new List<CevKey>();

        public CevKeyboard() { }

        public virtual void Update() {
            newKeyboard = Keyboard.GetState();

            GetPressedKeys();
        }

        public void UpdateOld() {
            oldKeyboard = newKeyboard;

            previousPressedKeys = new List<CevKey>();
            for (int i = 0; i < pressedKeys.Count; i++) {
                previousPressedKeys.Add(pressedKeys[i]);
            }
        }


        public bool GetPress(string KEY) {

            for (int i = 0; i < pressedKeys.Count; i++) {

                if (pressedKeys[i].key == KEY) {
                    return true;
                }

            }
            return false;
        }


        public virtual void GetPressedKeys() {
            bool found = false;

            pressedKeys.Clear();
            for (int i = 0; i < newKeyboard.GetPressedKeys().Length; i++) {
                pressedKeys.Add(new CevKey(newKeyboard.GetPressedKeys()[i].ToString(), 1));
            }
        }
    }
}
