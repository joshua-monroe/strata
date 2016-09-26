using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Strata.Interfaces;
using TiledSharp;

#if WINDOWS
using System.Windows.Forms;
#endif

namespace Strata.World
{
    class Level
    {
        //Constant Fields
        public const float GRAV_SCALE = 1.0f; //const for now
        public const float MAX_VEL_Y = 750.0f; //const for now
        private const string TILESET_PATH = "Maps\\Tilesets\\";

        #region Fields

        private TmxMap _map;

        private List<string> _tilesetName;

        private List<Entity> _entities;

        private List<Texture2D> _mapTileset;

        private List<List<Tile>> _mapTiles;

        private Dictionary<int, Rectangle> _textureAtlas;

        private Dictionary<ICollidable, List<Tile>> _possibleCollisions;

        private int _width;

        private int _height;

        private int _mapLayers;

        private Texture2D _rect;

        private List<Tile> _possibleCollisionsForEntity;

        #endregion

        #region Properties
        public int Width
        {
            get
            {
                return _width;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public int PixelWidth
        {
            get
            {
                return _width * _map.TileWidth;
            }
        }

        public int PixelHeight
        {
            get
            {
                return _height * _map.TileHeight;
            }
        }

        public int Layers
        {
            get
            {
                return _map.Layers.Count;
            }
        }

        public TmxMap Map
        {
            get
            {
                return _map;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(0, 0, _map.TileWidth * _map.Width, _map.TileHeight * _map.Height);
            }
        }

        public List<List<Tile>> MapTiles
        {
            get
            {
                return _mapTiles;
            }
        }

        public List<Entity> LevelEntities
        {
            get
            {
                return _entities;
            }
        }
        #endregion




        #region Methods

        /// <summary>
        /// Constructor for the Level class.
        /// </summary>
        /// <param name="pathToMap">Path to the [mapName].tmx file.</param>
        public Level(string pathToMap)
        {
            //Load map.
            _map = new TmxMap(pathToMap);
            //Reference content manager.
        }

        /// <summary>
        /// Picks a tileset based off the Gid of the tile passed into it.
        /// </summary>
        /// <param name="tile"> Tile to analyze. </param>
        /// <returns></returns>
        private Texture2D PickTileset(Tile tile)
        {
            for (int i = 0; i < _map.Tilesets.Count; i++)
            {
                if (tile.Gid == 0)
                {
                    //do nothing
                    continue;
                }
                if (tile.Gid >= _map.Tilesets[i].FirstGid && tile.Gid < _map.Tilesets[i].FirstGid + _map.Tilesets[i].TileCount)
                {
                    return _mapTileset[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Sets up the dictionary that will be used as a sort of texture atlas.
        /// </summary>
        private void SetUpDicitonary()
        {
            int count = 1;
            foreach (TmxTileset tileset in _map.Tilesets)
            {
                for (int i = 0; i < tileset.TileCount; i++)
                {
                    int tileFrame = i;
                    int? column = tileFrame % tileset.Columns;
                    int row = (int)Math.Floor(tileFrame / (double)tileset.Columns);

                    Rectangle tileRect = new Rectangle(tileset.TileWidth * (int)column, tileset.TileHeight * row, tileset.TileWidth, tileset.TileHeight);
                    _textureAtlas.Add(count++, tileRect);
                }
            }
        }

        /// <summary>
        /// Initial setup for the level class. It must initialize all the tilesets and make the specifc tiles easy to access for animation and collision.
        /// </summary>
        public void Initialize()
        {
            //Init fields
            _tilesetName = new List<string>();
            _mapTileset = new List<Texture2D>();
            _mapTiles = new List<List<Tile>>();
            _entities = new List<Entity>();
            _possibleCollisions = new Dictionary<ICollidable, List<Tile>>();
            _textureAtlas = new Dictionary<int, Rectangle>();
            _width = _map.Width;
            _height = _map.Height;
            _mapLayers = _map.Layers.Count;



            //Add tilesets to name list, we'll load them in a load content function
            foreach (TmxTileset tileSet in _map.Tilesets)
            {
                _tilesetName.Add(tileSet.Name);
            }

            //Create tile objects, luckily the layers contain only tiles and things like the player spawn object will not get confused here.
            for (int i = 0; i < _map.Layers.Count; i++)
            {
                int count = 0;
                TmxLayer refLayer = _map.Layers[i];
                for (int j = 0; j < _map.Layers[i].Tiles.Count; j++)
                {
                    //Basic Tile Info
                    Tile newTile = new Tile();
                    newTile.Width = _map.TileWidth;
                    newTile.Height = _map.TileHeight;
                    newTile.TileRef = _map.Layers[i].Tiles[j];
                    newTile.LayerCount = i;

                    //Determine the tile's type based off it's layer.
                    switch (_map.Layers[i].Name.Contains("Solid"))
                    {
                        case true:
                            newTile.TyleType = Tile.Type.SOLID;
                            break;
                        default:
                            newTile.TyleType = Tile.Type.BACKGROUND;
                            break;
                    }

                    //Add the tile to our list.
                    if (count == 0)
                    {
                        _mapTiles.Add(new List<Tile>());
                        _mapTiles[i].Add(newTile);
                        count++;
                    }
                    else
                    {
                        _mapTiles[i].Add(newTile);
                        count++;
                    }

                }
            }

            //Load the content needed for the level.
        }

        /// <summary>
        /// This method will load the main content needed to draw the stage tiles, probably enemies too.
        /// </summary>
        public void LoadContent(ContentManager Content)
        {
            //Load tilesets
            foreach (string tilesetName in _tilesetName)
            {
                try
                {
                    _mapTileset.Add(Content.Load<Texture2D>(TILESET_PATH + tilesetName));
                }
                catch
                {
                    //Report error code to player
#if WINDOWS
                    MessageBox.Show("Error 00: Failed to load tileset for level.", "Error");
                    Environment.Exit(1);
#endif
                }
            }
            //Set up dictionary from Texture.

            SetUpDicitonary();
        }

        /// <summary>
        /// Helper class that adds a tile to the passed in list.
        /// </summary>
        /// <param name="x">x-coordinate of tile</param>
        /// <param name="y">y-coordinate of tile</param>
        /// <param name="refList"></param>
        /// <param name="layer"></param>
        private void AddIndividualTile(int x, int y, List<Tile> refList, List<Tile> layer)
        {
            int element = x + _width * y;

            //Need to make sure we don't add anything out side of the bounds of the list
            if (element >= layer.Count || element < 0)
            {
                return;
            }

            //Add the item if it's in bounds
            if (x <= _width && y <= _height)
            {
                if (layer[element].Gid != 0 && layer[element].TyleType == Tile.Type.SOLID)
                    refList.Add(layer[element]);
            }
        }

        /// <summary>
        /// Uses the AddIndividualTile() function to create a list of possible collisions for an entity.
        /// </summary>
        /// <param name="layer">Layer of solid tiles.</param>
        /// <param name="currentEntity">The entity to consider.</param>
        /// <returns></returns>
        private List<Tile> AddTilesForEntity(List<Tile> layer, Entity currentEntity)
        {
            int x = (int)currentEntity.TileCoordinates.X;
            int y = (int)currentEntity.TileCoordinates.Y;
            List<Tile> resultList = new List<Tile>();

            #region Look Around Entity
            AddIndividualTile(x, y, resultList, layer);         // Entity
            AddIndividualTile(x - 1, y, resultList, layer);     // Left of Entity
            AddIndividualTile(x + 1, y, resultList, layer);     // Right of Entity
            AddIndividualTile(x, y - 1, resultList, layer);     // Top of Entity
            AddIndividualTile(x - 1, y - 1, resultList, layer); // Top Left of Entity
            AddIndividualTile(x + 1, y - 1, resultList, layer); // Top Right of Entity
            AddIndividualTile(x, y + 1, resultList, layer);     // Bottom of Entity
            AddIndividualTile(x - 1, y + 1, resultList, layer); // Bottom Left of Entity
            AddIndividualTile(x + 1, y + 1, resultList, layer); // Bottom Right of Entity
            #endregion

            return resultList;
        }

        /// <summary>
        /// Procedure that gets a list of possible tile collisions.
        /// </summary>
        /// <returns>Returns a list of the tiles we need to look at for each movable entity.</returns>
        private Dictionary<ICollidable, List<Tile>> GetPossibleTileCollisions()
        {
            Dictionary<ICollidable, List<Tile>> resultDict = new Dictionary<ICollidable, List<Tile>>();
            List<List<Tile>> layerList = new List<List<Tile>>();
            List<Tile> resultList = null;
            Entity currentEntity = null;

            //Find out which layers we need to consider
            for (int i = 0; i < _map.Layers.Count; i++)
            {
                if (_map.Layers[i].Name.Contains("Solid"))
                {
                    layerList.Add(_mapTiles[i]);
                }
            }

            //Loop through layerList and look around the entities.
            foreach (List<Tile> layer in layerList)
            {
                foreach (Entity entity in _entities)
                {

                    currentEntity = entity;
                    resultList = AddTilesForEntity(layer, entity);
                }

                resultDict.Add(currentEntity, resultList);
            }
            return resultDict;
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 gravity = new Vector2(0, 30f * deltaTime);
            // Populate Collision entities

            // Get possible collisions around entities.
            _possibleCollisions = GetPossibleTileCollisions();

            // Update, this will change the velocity of the entities.
            foreach (Entity e in LevelEntities)
            {
                e.Velocity += gravity;
                e.Update(gameTime);
            }

            // Check and react to collisions
            foreach (Entity e in LevelEntities)
            {
                e.ApplyVelocityX(deltaTime);
                CollisionManager.GetInstance().CheckTileCollisions(e, Direction.Horizontal, _possibleCollisions);
            }
            // Check and react to collisions
            foreach (Entity e in LevelEntities)
            {
                e.ApplyVelocityY(deltaTime);
                CollisionManager.GetInstance().CheckTileCollisions(e, Direction.Vertical, _possibleCollisions);
            }


        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            //Draw the level
            foreach (List<Tile> tileList in _mapTiles)
            {
                spriteBatch.Begin(SpriteSortMode.Texture, null,
                SamplerState.PointClamp, null, null, null, camera.GetViewMatrix());
                foreach (Tile tile in tileList)
                {
                    Rectangle rectRef;
                    _textureAtlas.TryGetValue(tile.Gid, out rectRef);
                    if (tile.Gid != 0)
                    {
                        tile.Draw(spriteBatch, PickTileset(tile), rectRef);
                    }
                }
                spriteBatch.End();
            }
            spriteBatch.Begin(SpriteSortMode.Texture, null,
                SamplerState.PointClamp, null, null, null, camera.GetViewMatrix());
            //Draw debug
            if (GameManager.debugFlag)
            {
                _rect = GameManager.debugRect;
                foreach (Tile tile in _possibleCollisionsForEntity)
                {
                    if (tile != null)
                    {
                        spriteBatch.Draw(_rect, tile.Bounds, Color.Red);

                    }
                }

            }
            spriteBatch.End();
        }

        #endregion
    }
}
