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
        World world; 
        //Camera camera;

        public override void LoadContent()
        {
            base.LoadContent();
            XmlManager<Player> playerLoader = new XmlManager<Player>();
            XmlManager<World> worldLoader = new XmlManager<World>();
            player = playerLoader.Load("Content/Load/Gameplay/Player.xml");
            player.LoadContent();
            world = worldLoader.Load("Content/Load/Gameplay/World/Intro.xml");
            world.LoadContent();

        }
        public override void UnloadContent()
        {
            base.UnloadContent();
            player.UnloadContent();
            world.UnloadContent();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            player.Update(gameTime, world.CurrentMap.Layer[1]); //collision layer
            world.Update(gameTime);

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            world.Draw(spriteBatch, "Underlay");
            player.Draw(spriteBatch);
            world.Draw(spriteBatch, "Overlay");

        }
    }
}
