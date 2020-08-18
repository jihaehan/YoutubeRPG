using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{
    //base class (inheritance, polymorphism)
    public class GameScreen 
    {
        protected ContentManager content;
        [XmlIgnore]
        public Type Type;
        public string XmlPath;
        [XmlIgnore]
        public InputManager InputManager; 
        public GameScreen()
        {
            Type = this.GetType(); //whatever class we have, this is going to pass the type into it
            XmlPath = "Content/Load/" + Type.ToString().Replace("YoutubeRPG.", "") + ".xml";
            InputManager = new InputManager();
        }

        public virtual void LoadContent()
        {
            content = new ContentManager( //nab the screen manager instance of content
                ScreenManager.Instance.Content.ServiceProvider, "Content");
            InputManager.LoadContent();
        }
        public virtual void UnloadContent()
        {
            content.Unload();
            InputManager.UnloadContent();
        }
        public virtual void Update(GameTime gameTime)
        {
            InputManager.Update();
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

    }
}
