using MonoGame.Extended.Sprites;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Strata.Interfaces;
using System;

namespace Strata.Example_Code
{
    class Player : Entity
    {
        private KeyboardState _playerKey;
        private AnimatedSprite _curlyAnimator;
        private bool _grounded = true;
        private Facing _face;

        public enum Facing
        {
            Left,
            Right,
            Up,
            Down
        }

        public override Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                _curlyAnimator.Position = _position;
            }
        }

        public Player()
        {
            _position = new Vector2(7 * 32, 11 * 32);
            _bounds = new Rectangle(10, 2, 12, 30);
            _face = Facing.Right;
            _velocity = Vector2.Zero;
        }

        private void HandleInput(float deltaTime)
        {
            KeyboardState prevState = _playerKey;
            _playerKey = Keyboard.GetState();

            if (_face == Facing.Right && Keyboard.GetState().GetPressedKeys().Length == 0)
            {
                _curlyAnimator.Play("idleRight");
            }
            if (_face == Facing.Left && Keyboard.GetState().GetPressedKeys().Length == 0)
            {
                _curlyAnimator.Play("idleLeft");
            }
            if (_playerKey.IsKeyDown(Keys.A))
            {
                _curlyAnimator.Play("walkLeft");
                _face = Facing.Left;
                _velocity.X -= 15.0f * deltaTime;
            }
            if (_playerKey.IsKeyDown(Keys.D))
            {
                _curlyAnimator.Play("walkRight");
                _face = Facing.Right;
                _velocity.X += 15.0f * deltaTime;
            }
            if (_playerKey.IsKeyDown(Keys.W))
            {
                _velocity.Y -= 60.0f * deltaTime;
            }
            if (_playerKey.IsKeyDown(Keys.S))
            {
                _velocity.Y += 60.0f * deltaTime;
            }
            if (_playerKey.IsKeyDown(Keys.F))
            {
                _velocity.X = 0;
                _velocity.Y = 0;
            }
        }

        public Vector2 PlayerFacingCameraPosition
        {
            get
            {
                Vector2 getPosition = Position;
                if (_face == Facing.Right)
                {
                    getPosition.X += 75;
                    return getPosition;
                }
                else if (_face == Facing.Left)
                {
                    getPosition.X -= 75;
                    return getPosition;
                }
                else if (_face == Facing.Up)
                {
                    getPosition.Y -= 75;
                    return getPosition;
                }
                else
                {
                    getPosition.Y += 75;
                    return getPosition;
                }
            }
        }

        public void LoadContent(ContentManager Content)
        {
            //Setup//
            var curlySheet = TextureAtlas.Create(Content.Load<Texture2D>("Sprite\\MyChar2"), 32, 32);
            var curlyFactory = new SpriteSheetAnimationFactory(curlySheet);
            /////////

            //Add animations here:
            curlyFactory.Add("walkRight", new SpriteSheetAnimationData(new[] { 274, 273, 275, 273 }));
            curlyFactory.Add("walkLeft", new SpriteSheetAnimationData(new[] { 261, 260, 262, 260 }));
            curlyFactory.Add("idleRight", new SpriteSheetAnimationData(new[] { 273 }));
            curlyFactory.Add("idleLeft", new SpriteSheetAnimationData(new[] { 260 }));

            //Final setup//
            _curlyAnimator = new AnimatedSprite(curlyFactory);
            _curlyAnimator.Scale = new Vector2(1.0f, 1.0f);
            _position += _curlyAnimator.Origin;
            _curlyAnimator.Position = _position;
            _curlyAnimator.Depth = 0.0f;
            _curlyAnimator.Play("idleRight");
            Width = (int)_curlyAnimator.GetBoundingRectangle().Width;
            Height = (int)_curlyAnimator.GetBoundingRectangle().Height;
            ///////////////
        }

        public override void OnCollision(ICollidable other, Direction dir)
        {

        }

        public override void ApplyVelocityX(float deltaTime)
        {
            _position.X += (float)Math.Round(_velocity.X);
            _curlyAnimator.Position = _position;
        }

        public override void ApplyVelocityY(float deltaTime)
        {
            _position.Y += (int)Math.Round(_velocity.Y);
            _curlyAnimator.Position = _position;
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleInput(deltaTime);
            #region commented code
            if (_grounded == true)
            {
                if (_velocity.X != 0.0f)
                {
                    //Friction statement
                    _velocity.X = (_velocity.X > 0) ? (_velocity.X -= _friction * deltaTime) : (_velocity.X += _friction * deltaTime);
                    if (_velocity.X < 0f && _velocity.X >= -1f * deltaTime) _velocity.X = 0.0f;

                }
            }
            #endregion
            _curlyAnimator.Update(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _curlyAnimator.Draw(spriteBatch);
        }
    }
}
