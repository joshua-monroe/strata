using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Strata.Screen_Entities
{
    public abstract class GameObject
    {
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}