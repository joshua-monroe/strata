using Strata.World;
using Strata.Interfaces;
using System.Collections.Generic;

namespace Strata
{
    public enum Direction
    {
        Horizontal,
        Vertical
    }
    /// <summary>
    /// Singleton. This class manages collidables.
    /// </summary>
    sealed class CollisionManager
    {
        private static CollisionManager _instance = null;


        public static CollisionManager GetInstance()
        {
            if (CollisionManager._instance == null)
            {
                CollisionManager._instance = new CollisionManager();
            }

            return CollisionManager._instance;
        }

        public void CheckTileCollisions(ICollidable currentCollidable, Direction dir, Dictionary<ICollidable, List<Tile>> possibleCollisions)
        {
            List<Tile> possibleCollisionsForEntity;
            possibleCollisions.TryGetValue(currentCollidable, out possibleCollisionsForEntity);

            if (dir == Direction.Horizontal) //X
            {
                foreach (Tile tile in possibleCollisionsForEntity)
                {
                    if (tile.Bounds.Intersects(currentCollidable.Bounds))
                    {
                        currentCollidable.OnCollision(tile, dir);
                        tile.OnCollision(currentCollidable, dir);
                    }

                }
            }
            else
            {
                foreach (Tile tile in possibleCollisionsForEntity)
                {
                    if (tile.Bounds.Intersects(currentCollidable.Bounds))
                    {
                        currentCollidable.OnCollision(tile, dir);
                        tile.OnCollision(currentCollidable, dir);
                    }

                }
            }
        }

        private CollisionManager()
        { }

    }
}
