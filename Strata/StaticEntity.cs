using System;
using Microsoft.Xna.Framework;

namespace Strata
{
    abstract class StaticEntity
    {
        protected int _width;
        protected int _height;
        protected Vector2 _position;
        protected Rectangle _bounds;

        public int Width
        {
            get
            {
                return _height;
            }
            set
            {
                _width = value;
            }
        }
        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }
        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }
        public Vector2 TileCoordinates
        {
            get
            {
                float x = Position.X / 32;
                float y = Position.Y / 32;
                return new Vector2((float)Math.Floor(x), (float)Math.Floor(y));
            }
        }
        public Rectangle Bounds
        {
            get
            {
                int x = (int)Position.X;
                int y = (int)Position.Y;
                return new Rectangle(x, y, _width, _height);
            }
        }
    }
}
