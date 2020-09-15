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
        public SPX(string xmlPath)
        {
            XmlManager<Image> xmlManager = new XmlManager<Image>();
            Image = xmlManager.Load(xmlPath);
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
                Vector2 Velocity = new Vector2(0, -1) * 300f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Image.Position += Velocity;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }
    }

    public class SPXManager
    {
        Dictionary<string, string> Target;
        public SPXManager()
        {
            Target = new Dictionary<string, string>();
            initializeSPX();
        }
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
            string xmlPath = Target[randomTarget[randomIndex]];            
            return "Content/Load/SPX/" + xmlPath + ".xml";
        }
        void initializeSPX()
        {
            //Target SPX
            Target.Add("black1", "hit_black");
            Target.Add("black2", "cloud_black");
            Target.Add("white1", "hit_white_fill");
            Target.Add("white2", "cloud_white_fill");
            Target.Add("red1", "hit_red");
            Target.Add("red2", "cloud_red");
        }
    }
}
