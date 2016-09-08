using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Strata.Screen_Entities
{
    public abstract class GameObject
    {
        //This 
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}