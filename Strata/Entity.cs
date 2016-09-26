using System;
using Strata.Interfaces;
using Microsoft.Xna.Framework;

namespace Strata
{
    abstract class Entity : ICollidable
    {
        protected int _width;
        protected int _height;
        protected Vector2 _velocity;
        protected Vector2 _position;
        protected float _friction = 10f;
        protected Rectangle _bounds;

        public int Width
        {
            get
            {
                return _width;
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
        public abstract Vector2 Position
        {
            get;
            set;
        }
        public Vector2 ZeroPosition
        {
            get
            {
                return Position - Origin;
            }
        }
        public Vector2 Velocity
        {
            get
            {
                return _velocity;
            }
            set
            {
                _velocity = value;
            }
        }
        public Vector2 Origin
        {
            get
            {
                return new Vector2(Width / 2, Height / 2);
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
                int x = (int)Math.Round(ZeroPosition.X);
                int y = (int)Math.Round(ZeroPosition.Y);
                return new Rectangle(x + _bounds.X, y + _bounds.Y, _bounds.Width, _bounds.Height);
            }
        }

        public abstract void OnCollision(ICollidable other, Direction dir);
        public abstract void ApplyVelocityX(float deltaTime);
        public abstract void ApplyVelocityY(float deltaTime);
        public abstract void Update(GameTime gameTime);
    }
}
