#region includes
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using cevEngine2D.source.engine;
using cevEngine2D.source.engine.input;
using cevEngine2D.source.engine.tilemap;
using cevEngine2D.source.world;
using cevEngine2D.source.world.projectiles;
using cevEngine2D.source.world.units;
using SharpDX.MediaFoundation.DirectX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
#endregion
//TODO: make _flip functional in player state machine
namespace cevEngine2D.source.engine.animate {
    internal class Animation {
        internal Texture2D _spriteSheet;
        internal int _numFrames, _frameSize, _countAnimations;
        internal bool _isHorizontal;
        internal Vector2 _startingPosition, _position;
        internal Rectangle _sRect;
        internal SpriteEffects _flip;
        private GameTimer _timer;

        public Rectangle SRect { get { return _sRect; } }

        public Animation(Texture2D spriteSheet, int numFrames, int frameSize, bool isHorizontal, bool flip, Vector2 startingPos) {
            _flip = flip ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            _spriteSheet = spriteSheet;
            _numFrames = numFrames;
            _frameSize = frameSize;
            _countAnimations = 0;
            _isHorizontal = isHorizontal;
            _startingPosition = new Vector2(startingPos.X * _frameSize, startingPos.Y * _frameSize);
            _position = _startingPosition;
            _sRect = new Rectangle(
                (int)_startingPosition.X * _frameSize,
                (int)_startingPosition.Y * _frameSize,
                _frameSize,
                _frameSize
            );
            _timer = new GameTimer(100);
        }

        private void Animate() {
            Rectangle sRect;

            if (_countAnimations == _numFrames) {
                _countAnimations = 0;
                _position = _startingPosition;
            }

            _position.X += _isHorizontal && _countAnimations > 0 ? _frameSize : 0;
            _position.Y += !_isHorizontal && _countAnimations > 0 ? _frameSize : 0;

            sRect = new Rectangle(
                (int)_position.X,
                (int)_position.Y,
                _frameSize,
                _frameSize
            );

            _countAnimations++;
            _sRect = sRect;
        }

        public Rectangle Update() {
            _timer.UpdateTimer();
            if (_timer.Test()) {
                Animate();
                _timer.ResetToZero();
            }

            return _sRect;
        }
    }
}
