using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{
    public class SPX
    {
        Image Image;
        Vector2 targetPosition;
        float moveSpeed;

        public SPX(string xmlPath)
        {
            //Set MoveSpeed
            moveSpeed = 300f;
            //Load Image
            XmlManager<Image> xmlManager = new XmlManager<Image>();
            Image = xmlManager.Load(xmlPath);
            LoadContent();
            targetPosition = Image.Position;
            if (Image.Path.Contains("enemy"))
                Image.Position = new Vector2(1280 + Image.SourceRect.Width, targetPosition.Y);
            else if (Image.Path.Contains("player"))
                Image.Position = new Vector2(-Image.SourceRect.Width, targetPosition.Y);
        }
        public void LoadContent()
        {
            Image.Alpha = 0.0f;
            Image.IsActive = true;
            Image.Effects = "FadeEffect";
            Image.LoadContent();
        }
        public void UnloadContent()
        {
            Image.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            Image.Update(gameTime);
            if (Image.Alpha >= 1.0f)
                Image.IsActive = false;
            
            if (Image.Path.Contains("UV") && Image.Position.Y < -Image.SourceRect.Height && !Image.IsActive)
            {
                Image.Position.Y -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (Image.Path.Contains("enemy") && Image.Position.X > targetPosition.X)
            {
                Image.Position.X -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (Image.Path.Contains("player") && Image.Position.X < targetPosition.X)
            {
                Image.Position.X += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }
    }

    public class SPXManager
    {
        public SPXManager()
        { }

        public string EnvironmentXml(string spxName)
        {   //draws out environment SPX: UV, Extinguisher
            return "Content/Load/SPX/" + spxName + ".xml";
        }
        public string AOEXml(string spxName, bool isPlayer)
        {
            //draws out SPX effect depending on isPlayer
            string xmlPath = String.Empty;
            if (isPlayer)
                xmlPath = "Content/Load/SPX/player_";
            else
                xmlPath = "Content/Load/SPX/enemy_";
            return xmlPath + spxName + ".xml";
        }
        public string TargetXml()
        {
            //Randomizes SPX effect to load
            Random rnd = new Random();
            string[] randomTarget = { "black1", "black2", "white1", "white2", "red1", "red2" };
            int randomIndex = rnd.Next(randomTarget.Length);
            string xmlPath = randomTarget[randomIndex];            
            return "Content/Load/SPX/" + xmlPath + ".xml";
        }
    }
}
