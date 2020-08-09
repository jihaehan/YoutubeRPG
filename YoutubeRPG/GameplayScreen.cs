using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace YoutubeRPG
{
    public class GameplayScreen : GameScreen
    {
        Player player;
        public override void LoadContent()
        {
            base.LoadContent();
            XmlManager<Player> playerLoader = new XmlManager<Player>();
            player = playerLoader.Load("Content/Load/Gameplay/Player.xml");
            player.LoadContent();
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
            player.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            player.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            player.Draw(spriteBatch);
        }
    }
}
