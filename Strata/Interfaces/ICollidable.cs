using Microsoft.Xna.Framework;

namespace Strata.Interfaces
{
    interface ICollidable
    {
        Vector2 Position { get; set; }
        Vector2 Velocity { get; set; }
        Rectangle Bounds { get; }

        void OnCollision(ICollidable other, Direction dir);
    }
}
