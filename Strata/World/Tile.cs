using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Strata.Interfaces;
using TiledSharp;
using System;

namespace Strata.World
{
    class Tile : StaticEntity, ICollidable
    {
        private int _layer;
        private bool _active;
        private TmxLayerTile _tile;
        private Type _tileType;

        #region Properties
        public enum Type
        {
            SOLID,
            BACKGROUND
        };

        public int LayerCount
        {
            get
            {
                return _layer;
            }
            set
            {
                _layer = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
            }
        }

        public int Gid
        {
            get
            {
                return TileRef.Gid;
            }
        }

        public Vector2 Velocity
        {
            get
            {
                return Vector2.Zero;
            }
            set
            {

            }
        }

        public TmxLayerTile TileRef
        {
            get
            {
                return _tile;
            }
            set
            {
                _tile = value;
                Position = new Vector2(_tile.X * _width, _tile.Y * _height);
            }
        }
        public Type TyleType
        {
            get
            {
                return _tileType;
            }
            set
            {
                _tileType = value;
            }
        }
        #endregion

        #region Methods
        public Tile()
        {
            Initialize();
        }

        private void Initialize()
        {
            _active = false; //Default active tile to false
        }

        private void ResolveCollisionsX(ICollidable currentEntity, Tile tile)
        {
            if (currentEntity.Velocity.X > 0) // Going right
            {
                int intersectDepth = tile.Bounds.Left - currentEntity.Bounds.Right;
                Vector2 intersectVec = Vector2.Zero;
                if (intersectDepth < -32) // We are moving away from the tile
                {
                    return;
                }

                //Correct the position
                intersectVec.X = intersectDepth;
                Vector2 currentPosition = currentEntity.Position;
                currentPosition += intersectVec;
                currentPosition.X = (float)Math.Round(currentPosition.X);
                currentEntity.Position = currentPosition;

                Vector2 newVel = currentEntity.Velocity;
                newVel.X = 0;
                currentEntity.Velocity = newVel;
            }
            else if (currentEntity.Velocity.X < 0) //Going down
            {
                int intersectDepth = tile.Bounds.Right - currentEntity.Bounds.Left;
                Vector2 intersectVec = Vector2.Zero;
                if (intersectDepth > 32) // We are moving away from the tile
                {
                    return;
                }

                //Correct the position
                intersectVec.X = intersectDepth;
                Vector2 currentPosition = currentEntity.Position;
                currentPosition += intersectVec;
                currentPosition.X = (float)Math.Round(currentPosition.X);
                currentEntity.Position = currentPosition;

                Vector2 newVel = currentEntity.Velocity;
                newVel.X = 0;
                currentEntity.Velocity = newVel;
            }

        }

        private void ResolveCollisionsY(ICollidable currentEntity, Tile tile)
        {
            if (currentEntity.Velocity.Y > 0) // Going down
            {
                int intersectDepth = tile.Bounds.Top - currentEntity.Bounds.Bottom;
                Vector2 intersectVec = Vector2.Zero;
                if (intersectDepth < -32) // We are moving away from the tile
                {
                    return;
                }

                //Correct the position
                intersectVec.Y = intersectDepth;
                Vector2 currentPosition = currentEntity.Position;
                currentPosition += intersectVec;
                currentPosition.Y = (float)Math.Round(currentPosition.Y);
                currentEntity.Position = currentPosition;

                Vector2 newVel = currentEntity.Velocity;
                newVel.Y = 0;
                currentEntity.Velocity = newVel;
            }
            else if (currentEntity.Velocity.Y < 0) //Going up
            {
                int intersectDepth = tile.Bounds.Bottom - currentEntity.Bounds.Top;
                Vector2 intersectVec = Vector2.Zero;
                if (intersectDepth > 32) // We are moving away from the tile
                {
                    return;
                }

                //Correct the position
                intersectVec.Y = intersectDepth;
                Vector2 currentPosition = currentEntity.Position;
                currentPosition += intersectVec;
                currentPosition.Y = (float)Math.Round(currentPosition.Y);
                currentEntity.Position = currentPosition;

                Vector2 newVel = currentEntity.Velocity;
                newVel.Y = 0;
                currentEntity.Velocity = newVel;
            }
        }

        public void OnCollision(ICollidable other, Direction dir)
        {
            if (dir == Direction.Horizontal)
            {
                ResolveCollisionsX(other, this);
            }
            else
            {
                ResolveCollisionsY(other, this);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            //Update logic for animated/destructible Tiles, or anything that can be updated in some way.
            if (_active)
            {

            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, Texture2D tileSet, Rectangle tileRect)
        {

            spriteBatch.Draw(tileSet, new Rectangle((int)_position.X, (int)_position.Y, _width, _height),
                tileRect, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.1f);
            //spriteBatch.Draw(tileSet, new Rectangle((int)_position.X, (int)_position.Y, _width, _height), tilesetRec, Color.White);
        }
        #endregion

    }
}
