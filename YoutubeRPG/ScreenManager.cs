using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{
    public class ScreenManager
    {
        //singleton class: single instance of the class, alike a static class but more efficient
        private static ScreenManager instance;
        [XmlIgnore]
        public Vector2 Dimensions { private set; get; }
        [XmlIgnore]
        public ContentManager Content { private set; get; }
        XmlManager<GameScreen> xmlGameScreenManager;

        GameScreen currentScreen, newScreen;
        [XmlIgnore]
        public GraphicsDevice GraphicsDevice;
        [XmlIgnore]
        public SpriteBatch SpriteBatch;

        public Image Image;
        [XmlIgnore]
        public bool IsTransitioning { get; private set; }
        [XmlIgnore]
        public bool IsFadeEffect { get; set; }

        public static ScreenManager Instance
        {
            get
            {
                if (instance == null)
                {
                    XmlManager<ScreenManager> xml = new XmlManager<ScreenManager>();
                    instance = xml.Load("Content/Load/ScreenManager.xml"); 
                }
                return instance; 
            }
        }

        public ScreenManager()
        {
            Dimensions = new Vector2(1280, 720);
            currentScreen = new BattleScreen();
            //currentScreen = new GameplayScreen_Blue();
            //currentScreen = new SplashScreen();
            xmlGameScreenManager = new XmlManager<GameScreen>();
            xmlGameScreenManager.Type = currentScreen.Type;
            //currentScreen = xmlGameScreenManager.Load("Content/Load/SplashScreen.xml");

        }
        public void ChangeScreens(string screenName)
        {
            newScreen = (GameScreen)Activator.CreateInstance(Type.GetType("YoutubeRPG." + screenName));
            Image.IsActive = true;
            Image.FadeEffect.Increase = true;
            Image.Alpha = 0.0f;
            IsTransitioning = true; 

        }
        public void FadeScreen()
        {
            Image.IsActive = true;
            Image.FadeEffect.Increase = true;
            Image.Alpha = 0.0f;
            IsFadeEffect = true;
        }
        private void Transition(GameTime gameTime)
        {
            if (IsTransitioning)
            { 
                Image.Update(gameTime);
                if (Image.Alpha == 1.0f)
                {
                    currentScreen.UnloadContent();
                    currentScreen = newScreen;
                    xmlGameScreenManager.Type = currentScreen.Type;
                    if (File.Exists(currentScreen.XmlPath))
                        currentScreen = xmlGameScreenManager.Load(currentScreen.XmlPath);
                    currentScreen.LoadContent();
                }
                else if (Image.Alpha == 0.0f)
                {
                    Image.IsActive = false;
                    IsTransitioning = false; 
                }
            }
        }
        public void LoadContent(ContentManager Content)
        {
            this.Content = new ContentManager(Content.ServiceProvider, "Content");
            currentScreen.LoadContent();
            Image.LoadContent();
        }
        public void UnloadContent() //unload if you don't need assets, release assets stored up in memory
        {
            currentScreen.UnloadContent();
            Image.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
            Transition(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            currentScreen.Draw(spriteBatch);
            spriteBatch.Begin();
            if (IsTransitioning || IsFadeEffect)
                Image.Draw(spriteBatch);
            spriteBatch.End();
        }
    }
}
