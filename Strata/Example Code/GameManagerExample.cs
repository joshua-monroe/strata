using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.TextureAtlases;
using Strata;

namespace CaveStoryLearningGL
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameManager : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Camera _camera;
        private Strata.World.Level _testlevel;
        private Strata.Example_Code.Player player;
        private SpriteFont debug;

        public static bool debugFlag = false;
        public static Texture2D rect;

        public GameManager()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _camera = new Camera(GraphicsDevice);
            _testlevel = new Strata.World.Level("Content/Maps/map.tmx");
            _testlevel.Initialize();
            _camera.Limits = _testlevel.Bounds;
            _camera.ResetCamera();
            _camera.Zoom += 0.8f;
            rect = new Texture2D(GraphicsDevice, 32, 32);

            Color[] data = new Color[80 * 30];

            for (int i = 0; i < data.Length; ++i)
                data[i] = Color.White;

            rect.SetData(data);


            var curlySheet = TextureAtlas.Create(Content.Load<Texture2D>("Sprite\\MyChar2"), 32, 32);
            player = new Strata.Example_Code.Player();

            _testlevel.LevelEntities.Add(player);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            player.LoadContent(Content);
            _testlevel.LoadContent(Content);
            debug = Content.Load<SpriteFont>("debug");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                _camera.Zoom += 5.0f * deltaTime;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                _camera.Zoom -= 5.0f * deltaTime;
            }
            //Update Level
            _testlevel.Update(gameTime);
            //_camera.LookAt(new Vector2(player.Position.X, player.Position.Y));
            _camera.LerpLookAt(player.PlayerFacingCameraPosition, 0.05f);
            _camera.ValidatePosition();


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            // TODO: Add your drawing code here



            //Draw level

            _testlevel.Draw(spriteBatch, _camera);


            //Draw debug w/ camera
            if (debugFlag)
            {
                spriteBatch.Begin(SpriteSortMode.Texture, null,
                    SamplerState.PointClamp, null, null, null, _camera.GetViewMatrix());
                spriteBatch.Draw(rect, player.Bounds, Color.Red);
                spriteBatch.End();
            }

            //Draw character(s)

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
                SamplerState.PointClamp, null, null, null, _camera.GetViewMatrix());
            player.Draw(spriteBatch);
            spriteBatch.End();

            //Draw debug w/o camera
            if (debugFlag)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(debug, "Player coordinates: " + player.TileCoordinates.X + "," + player.TileCoordinates.Y,
                    Vector2.Zero, Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}