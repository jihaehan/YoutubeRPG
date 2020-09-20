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
        public Image Image;
        public bool FadeOut; 
        public int FadeCount;
        Vector2 targetPosition;
        float moveSpeed;
        float pause;

        #region Main Methods
        public SPX(string xmlPath)
        {
            //Set MoveSpeed
            moveSpeed = 180f;
            FadeCount = 0;
            FadeOut = false;
            pause = 0;
            //Load Image
            XmlManager<Image> xmlManager = new XmlManager<Image>();
            Image = xmlManager.Load(xmlPath);
            LoadContent();
            targetPosition = Image.Position;
            if (Image.Path.Contains("enemy"))
                Image.Position = new Vector2(1280, targetPosition.Y);
            else if (Image.Path.Contains("player"))
                Image.Position = new Vector2(-Image.SourceRect.Width, targetPosition.Y);
        }
        public SPX(string xmlPath, Vector2 position)
        {
            XmlManager<Image> xmlManager = new XmlManager<Image>();
            Image = xmlManager.Load(xmlPath);
            Image.Position = position;
            pause = 0;
            FadeCount = 0;
            FadeOut = false;
            LoadContent();
        }
        public void LoadContent()
        {
            if (!Image.Path.Contains("player") &&  !Image.Path.Contains("enemy"))
            {
                Image.Alpha = 0.0f;
                Image.IsActive = true;
                Image.Effects = "FadeEffect";
            }
            else
                Image.IsActive = false;
            Image.LoadContent();
            //Image.FadeEffect.FadeSpeed = 0.5f;
        }
        public void UnloadContent()
        {
            Image.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            if (FadeOut)
            {
                if (Image.Alpha > 0)
                    Image.Alpha -= (float)gameTime.ElapsedGameTime.TotalSeconds * 0.5f;
            }
            else
            {
                Image.Update(gameTime);

                if (Image.Path.Contains("target") && pause < 5)
                {
                    if (Image.Alpha >= 1.0f && pause < 1)
                    {
                        Image.IsActive = false;
                        pause += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else if (pause > 0)
                        FadeOut = true;
                }
                else if (Image.IsActive && Image.Path.Contains("white"))
                {
                    if (Image.Alpha >= 1.0f)
                        FadeCount++;
                    else if (FadeCount > 2 && Image.Alpha <= 0.0f)
                    {
                        Image.IsActive = false;
                        Image.IsVisible = false;
                    }
                }
                else if (Image.IsActive && Image.Path.Contains("red"))
                {
                    if (Image.Alpha >= 1.0f)
                        FadeCount++;
                    else if (FadeCount > 2 && Image.Alpha <= 0.0f)
                    {
                        Image.IsActive = false;
                        Image.IsVisible = false;
                    }
                }
                else if (Image.IsActive && Image.Path.Contains("black"))
                {
                    if (Image.Alpha >= 1.0f)
                        FadeCount++;
                    else if (FadeCount > 2 && Image.Alpha <= 0.0f)
                    {
                        Image.IsActive = false;
                        Image.IsVisible = false;
                    }
                }
                

                if (Image.Path.Contains("Extinguisher") && Image.IsActive)
                {
                    if (Image.Alpha >= 1.0f)
                        Image.IsActive = false;
                }
                else if (Image.Path.Contains("UV") && Image.Position.Y < -Image.SourceRect.Height && !Image.IsActive)
                {
                    Image.Position.Y -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else if (Image.Path.Contains("enemy"))
                {
                    if (Image.Position.X > targetPosition.X)
                        Image.Position.X -= moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds; 
                    else
                        FadeOut = true;
                }
                else if (Image.Path.Contains("player"))
                {
                    if (Image.Position.X < targetPosition.X)
                        Image.Position.X += moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    else
                        FadeOut = true;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }
        #endregion
    }
    public class SPXManager
    {
        public Dictionary<string, bool> EnvironmentEffects;
        public SPXManager()
        {
            EnvironmentEffects = new Dictionary<string, bool>();
            EnvironmentEffects.Add("UV", false);
            EnvironmentEffects.Add("Extinguisher", false);
        }

        public string EnvironmentXml(string spxName)
        {   //draws out environment SPX: UV, Extinguisher
            if (EnvironmentEffects.ContainsKey(spxName))
                EnvironmentEffects[spxName] = true;
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
        public string BranchingXml()
        {
            Random rnd = new Random();
            string[] randomTarget = { "white1", "white2" }; //"black1", "black2", "white1", "white2",
            int randomIndex = rnd.Next(randomTarget.Length);
            string xmlPath = randomTarget[randomIndex];
            return "Content/Load/SPX/" + xmlPath + ".xml";
        }
        public string TargetXml()
        {
            //Randomizes SPX effect to load
            Random rnd = new Random();
            string[] randomTarget = { "target1", "target2", "target3" }; //"black1", "black2", "white1", "white2",
            int randomIndex = rnd.Next(randomTarget.Length);
            string xmlPath = randomTarget[randomIndex];            
            return "Content/Load/SPX/" + xmlPath + ".xml";
        }
        
    }
}
