﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using cevEngine2D.source.engine.input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace cevEngine2D {
    public delegate void PassObject(object i);
    public delegate object PassObjectAndReturn(object i);

    public class Globals {
        public static int screenHeight, screenWidth;

        public static ContentManager content;
        public static SpriteBatch spriteBatch; //object for draw method

        public static Viewport viewport;
        public static CevKeyboard keyboard;
        public static CevMouseControl mouse;

        public static GameTime gameTime;

        public static Texture2D rectangleTexture; //debug variable for rectHollow method

        public static float GetDistance(Vector2 pos, Vector2 target) {
            return (float)Math.Sqrt(Math.Pow(pos.X - target.X, 2) + Math.Pow(pos.Y - target.Y, 2));
        }

        public static Vector2 RadialMovement(Vector2 focus, Vector2 pos, float speed) {
            float dist = Globals.GetDistance(pos, focus);

            if (dist <= speed) {
                return focus - pos;
            } else {
                return (focus - pos) * speed / dist;
            }
        }

        //Debug method for outlining rectangles
        public static void DrawRectHollow(Rectangle rect, int thickness) {
            Globals.spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Y,
                    rect.Width,
                    thickness
                ),
                Color.White
            );
            Globals.spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Bottom - thickness,
                    rect.Width,
                    thickness
                ),
                Color.White
            );
            Globals.spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.X,
                    rect.Y,
                    thickness,
                    rect.Height
                ),
                Color.White
            );
            Globals.spriteBatch.Draw(
                rectangleTexture,
                new Rectangle(
                    rect.Right - thickness,
                    rect.Y,
                    thickness,
                    rect.Height
                ),
                Color.White
            );
        }

        //public static float RotateTowards(Vector2 Pos, Vector2 focus) {
        //    float h, sineTheta, angle;
        //    if (Pos.Y - focus.Y != 0) {
        //        h = (float)Math.Sqrt(Math.Pow(Pos.X - focus.X, 2) + Math.Pow(Pos.Y - focus.Y, 2));
        //        sineTheta = (float)(Math.Abs(Pos.Y - focus.Y) / h); //* ((item.Pos.Y-focus.Y)/(Math.Abs(item.Pos.Y-focus.Y))));
        //    } else {
        //        h = Pos.X - focus.X;
        //        sineTheta = 0;
        //    }

        //    angle = (float)(Math.Asin(sineTheta));

        //    // Drawing diagonial lines here.
        //    //Quadrant 2
        //    if (Pos.X - focus.X > 0 && Pos.Y - focus.Y > 0) {
        //        angle = (float)(Math.PI * 3 / 2 + angle);
        //    }
        //    //Quadrant 3
        //    else if (Pos.X - focus.X > 0 && Pos.Y - focus.Y < 0) {
        //        angle = (float)(Math.PI * 3 / 2 - angle);
        //    }
        //    //Quadrant 1
        //    else if (Pos.X - focus.X < 0 && Pos.Y - focus.Y > 0) {
        //        angle = (float)(Math.PI / 2 - angle);
        //    } else if (Pos.X - focus.X < 0 && Pos.Y - focus.Y < 0) {
        //        angle = (float)(Math.PI / 2 + angle);
        //    } else if (Pos.X - focus.X > 0 && Pos.Y - focus.Y == 0) {
        //        angle = (float)Math.PI * 3 / 2;
        //    } else if (Pos.X - focus.X < 0 && Pos.Y - focus.Y == 0) {
        //        angle = (float)Math.PI / 2;
        //    } else if (Pos.X - focus.X == 0 && Pos.Y - focus.Y > 0) {
        //        angle = (float)0;
        //    } else if (Pos.X - focus.X == 0 && Pos.Y - focus.Y < 0) {
        //        angle = (float)Math.PI;
        //    }

        //    return angle;
        //}
    }
}
