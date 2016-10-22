using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Strata.Interfaces;
using TiledSharp;

namespace Strata.World
{
    class Level
    {
        //Constant Fields
        public const float GRAV_SCALE = 1.0f; //const for now
        public const float MAX_VEL_Y = 750.0f; //const for now
        private const string TILESET_PATH = "Maps\\Tilesets\\";

        #region Fields

        private TmxMap map;

        private List<string> tilesetName;

        private List<Entity> entities;

        private List<Texture2D> mapTileset;

        private List<List<Tile>> mapTiles;

        private Dictionary<int, Rectangle> textureAtlas;

        private Dictionary<ICollidable, List<Tile>> possibleCollisions;

        private Texture2D rect;

        private List<Tile> possibleCollisionsForEntity;

        #endregion

        #region Properties
        public int Width { get; set; }

        public int Height { get; set; }

        public int PixelWidth
        {
            get
            {
                return Width * this.map.TileWidth;
            }
        }

        public int PixelHeight
        {
            get
            {
                return Height * this.map.TileHeight;
            }
        }

        public int Layers
        {
            get
            {
                return this.map.Layers.Count;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle(0, 0, this.map.TileWidth * this.map.Width, this.map.TileHeight * this.map.Height);
            }
        }


        public List<Entity> LevelEntities
        {
            get
            {
                return this.entities;
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
            this.map = new TmxMap(pathToMap);
            //Reference content manager.
        }

        /// <summary>
        /// Picks a tileset based off the Gid of the tile passed into it.
        /// </summary>
        /// <param name="tile"> Tile to analyze. </param>
        /// <returns></returns>
        private Texture2D PickTileset(Tile tile)
        {
            for (int i = 0; i < this.map.Tilesets.Count; i++)
            {
                if (tile.Gid == 0)
                {
                    //do nothing
                    continue;
                }
                if (tile.Gid >= this.map.Tilesets[i].FirstGid && tile.Gid < this.map.Tilesets[i].FirstGid + this.map.Tilesets[i].TileCount)
                {
                    return this.mapTileset[i];
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
            foreach (TmxTileset tileset in this.map.Tilesets)
            {
                for (int i = 0; i < tileset.TileCount; i++)
                {
                    int tileFrame = i;
                    int? column = tileFrame % tileset.Columns;
                    int row = (int)Math.Floor(tileFrame / (double)tileset.Columns);

                    Rectangle tileRect = new Rectangle(tileset.TileWidth * (int)column, tileset.TileHeight * row, tileset.TileWidth, tileset.TileHeight);
                    this.textureAtlas.Add(count++, tileRect);
                }
            }
        }

        /// <summary>
        /// Initial setup for the level class. It must initialize all the tilesets and make the specifc tiles easy to access for animation and collision.
        /// </summary>
        public void Initialize()
        {
            //Init fields
            this.tilesetName = new List<string>();
            this.mapTileset = new List<Texture2D>();
            this.mapTiles = new List<List<Tile>>();
            this.entities = new List<Entity>();
            this.possibleCollisions = new Dictionary<ICollidable, List<Tile>>();
            this.textureAtlas = new Dictionary<int, Rectangle>();
            Width = this.map.Width;
            Height = this.map.Height;


            //Add tilesets to name list, we'll load them in a load content function
            foreach (TmxTileset tileSet in this.map.Tilesets)
            {
                this.tilesetName.Add(tileSet.Name);
            }

            //Create tile objects, luckily the layers contain only tiles and things like the player spawn object will not get confused here.
            for (int i = 0; i < this.map.Layers.Count; i++)
            {
                int count = 0;
                TmxLayer refLayer = this.map.Layers[i];
                for (int j = 0; j < this.map.Layers[i].Tiles.Count; j++)
                {
                    //Basic Tile Info
                    Tile newTile = new Tile();
                    newTile.Width = this.map.TileWidth;
                    newTile.Height = this.map.TileHeight;
                    newTile.TileRef = this.map.Layers[i].Tiles[j];
                    newTile.LayerCount = i;

                    //Determine the tile's type based off it's layer.
                    switch (this.map.Layers[i].Name.Contains("Solid"))
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
                        this.mapTiles.Add(new List<Tile>());
                        this.mapTiles[i].Add(newTile);
                        count++;
                    }
                    else
                    {
                        this.mapTiles[i].Add(newTile);
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
            foreach (string tilesetName in this.tilesetName)
            {
                try
                {
                    this.mapTileset.Add(Content.Load<Texture2D>(TILESET_PATH + tilesetName));
                }
                catch
                {
                    //Report error code to player
                    MessageBox.Show("Error 00: Failed to load tileset for level.", "Error");
                    Environment.Exit(1);
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
            int element = x + Width * y;

            //Need to make sure we don't add anything out side of the bounds of the list
            if (element >= layer.Count || element < 0)
            {
                return;
            }

            //Add the item if it's in bounds
            if (x <= Width && y <= Height)
            {
                if (layer[element].Gid != 0 && layer[element].TyleType == Tile.Type.SOLID)
                    refList.Add(layer[element]);
            }
        }

        /// <summary>
        /// Uses the AddIndividualTile() function to create a list of possible collisions for an entity.
        /// This list will be given to the CollisionManager to decide what is actually colliding or not.
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
            for (int i = 0; i < this.map.Layers.Count; i++)
            {
                if (this.map.Layers[i].Name.Contains("Solid"))
                {
                    layerList.Add(this.mapTiles[i]);
                }
            }

            //Loop through layerList and look around the entities.
            foreach (List<Tile> layer in layerList)
            {
                foreach (Entity entity in this.entities)
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
            this.possibleCollisions = GetPossibleTileCollisions();

            // Update, this will change the velocity of the entities.
            foreach (Entity e in this.entities)
            {
                e.Velocity += gravity;
                e.Update(gameTime);
            }

            // Check and react to collisions
            foreach (Entity e in this.entities)
            {
                e.ApplyVelocityX(deltaTime);
                CollisionManager.GetInstance().CheckTileCollisions(e, Direction.Horizontal, this.possibleCollisions);
            }
            // Check and react to collisions
            foreach (Entity e in this.entities)
            {
                e.ApplyVelocityY(deltaTime);
                CollisionManager.GetInstance().CheckTileCollisions(e, Direction.Vertical, this.possibleCollisions);
            }


        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            //Draw the level
            foreach (List<Tile> tileList in this.mapTiles)
            {
                spriteBatch.Begin(SpriteSortMode.Texture, null,
                SamplerState.PointClamp, null, null, null, camera.GetViewMatrix());
                foreach (Tile tile in tileList)
                {
                    Rectangle tilesetSourceRect;
                    this.textureAtlas.TryGetValue(tile.Gid, out tilesetSourceRect);
                    if (tile.Gid != 0)
                    {
                        tile.Draw(spriteBatch, PickTileset(tile), tilesetSourceRect);
                    }
                }
                spriteBatch.End();
            }
            spriteBatch.Begin(SpriteSortMode.Texture, null,
                SamplerState.PointClamp, null, null, null, camera.GetViewMatrix());
            //Draw debug
            if (GameManager.debugFlag)
            {
                this.rect = GameManager.debugRect;
                foreach (Tile tile in this.possibleCollisionsForEntity)
                {
                    if (tile != null)
                    {
                        spriteBatch.Draw(this.rect, tile.Bounds, Color.Red);

                    }
                }

            }
            spriteBatch.End();
        }

        #endregion
    }
}
