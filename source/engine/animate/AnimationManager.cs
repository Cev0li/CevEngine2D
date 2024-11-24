#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using cevEngine2D.source.engine;
using cevEngine2D.source.engine.animate;
using cevEngine2D.source.engine.input;
using cevEngine2D.source.engine.tilemap;
using cevEngine2D.source.world.projectiles;
using cevEngine2D.source.world.units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
#endregion

namespace cevEngine2D.source.engine.animate
{
    internal class AnimationManager {
        internal Dictionary<String, Animation> _animations = new();
        private Unit _owner;

        public AnimationManager(Unit owner) {
            _owner = owner;
        }

        public void AddAnimation(String keyBind, int numFrames, int frameSize, bool isHorizontal, bool flip, Vector2 startingPos) {
            _animations.Add(keyBind, new Animation(
                _owner.SpriteTexture, 
                numFrames,
                frameSize, 
                isHorizontal, 
                flip,
                startingPos)
            );
        }
        
        public void Update(String key) {
            _owner.SRect = _animations[key].Update();
        }
    }
}
