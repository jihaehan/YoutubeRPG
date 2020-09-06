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
    public class Image
    {
        public float Alpha;
        public string Text, FontName, Path;
        public Vector2 Position, Scale;
        public Rectangle SourceRect;
        public bool IsActive, IsVisible;
        public Color TextColor;
        public bool IsCorner;

        Texture2D texture;
        Vector2 origin;
        ContentManager content;
        RenderTarget2D renderTarget;
        SpriteFont font;

        Dictionary<string, ImageEffect> effectList;
        public String Effects;

        public FadeEffect FadeEffect;
        public SpriteSheetEffect SpriteSheetEffect;
        public HighlightEffect HighlightEffect;

        public Texture2D Texture
        {
            get { return texture; }
        }
        public SpriteFont Font
        {
            get { return font; }
        }

        void SetEffect<T>(ref T effect)
        {
            if (effect == null)
                effect = (T)Activator.CreateInstance(typeof(T));
            else
            {
                (effect as ImageEffect).IsActive = true; 
                var obj = this;
                (effect as ImageEffect).LoadContent(ref obj); 
            }
            effectList.Add(effect.GetType().ToString().Replace("YoutubeRPG.", ""), (effect as ImageEffect));
        }
        public void ActivateEffect(string effect)
        {
            if (effectList.ContainsKey(effect))
            {
                effectList[effect].IsActive = true;
                var obj = this;
                effectList[effect].LoadContent(ref obj);
            }
        }
        public void DeactivateEffect(string effect)
        {
            if (effectList.ContainsKey(effect))
            {
                effectList[effect].IsActive = false;
                effectList[effect].UnloadContent();
            }
        }
        public void StoreEffects()
        {
            Effects = String.Empty;
            foreach (var effect in effectList)
                if (effect.Value.IsActive)
                    Effects += effect.Key + ":";
            if (Effects != String.Empty)
                Effects.Remove(Effects.Length - 1);
        }
        public void RestoreEffects()
        {
            foreach (var effect in effectList)
                DeactivateEffect(effect.Key);

            string[] split = Effects.Split(':');
            foreach (string s in split)
                ActivateEffect(s);
        }

        public Image()
        {
            Path = Text = Effects = String.Empty;
            FontName = "Fonts/AgencyFB";
            Position = Vector2.Zero;
            Scale = Vector2.One;
            Alpha = 1.0f;
            SourceRect = Rectangle.Empty;
            effectList = new Dictionary<string, ImageEffect>();
            TextColor = Color.White;
            IsCorner = false;
            IsVisible = true;
        }

        public void LoadContent()
        {
            content = new ContentManager(
                ScreenManager.Instance.Content.ServiceProvider, "Content");

            if (Path != String.Empty)
                texture = content.Load<Texture2D>(Path);

            font = content.Load<SpriteFont>(FontName);
            Vector2 dimensions = Vector2.Zero;

            if (Texture != null)
                dimensions.X += Texture.Width;
            else
                dimensions.X += font.MeasureString(Text).X;

            if (Texture != null)
                dimensions.Y = Math.Max(Texture.Height, font.MeasureString(Text).Y);
            else
                dimensions.Y = font.MeasureString(Text).Y;

            if (SourceRect == Rectangle.Empty)
                SourceRect = new Rectangle(0, 0, (int)dimensions.X, (int)dimensions.Y);

            renderTarget = new RenderTarget2D(ScreenManager.Instance.GraphicsDevice,
                (int)dimensions.X, (int)dimensions.Y);
            ScreenManager.Instance.GraphicsDevice.SetRenderTarget(renderTarget); //the screen is the render target
            ScreenManager.Instance.GraphicsDevice.Clear(Color.Transparent);
            ScreenManager.Instance.SpriteBatch.Begin();
            if (Texture != null)
                ScreenManager.Instance.SpriteBatch.Draw(Texture, Vector2.Zero, Color.White);
            ScreenManager.Instance.SpriteBatch.DrawString(font, Text, dimensions / 2 - font.MeasureString(Text) / 2, TextColor);
            ScreenManager.Instance.SpriteBatch.End();
            
            texture = renderTarget;

            ScreenManager.Instance.GraphicsDevice.SetRenderTarget(null);

            //Set Image Effects
            SetEffect<FadeEffect>(ref FadeEffect);
            SetEffect<SpriteSheetEffect>(ref SpriteSheetEffect);
            SetEffect<HighlightEffect>(ref HighlightEffect);

            if (Effects != String.Empty)
            {
                string[] split = Effects.Split(':');
                foreach (string item in split)
                    ActivateEffect(item);
            }
        }
        public void UnloadContent()
        {
            content.Unload();
            foreach (var effect in effectList)
                DeactivateEffect(effect.Key);
            
        }
        public void Update(GameTime gameTime)
        {
            foreach (var effect in effectList)
            {
                if (effect.Value.IsActive)
                    effect.Value.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);

            if (IsVisible)
            spriteBatch.Draw(
                Texture, Position + origin, SourceRect, Color.White * Alpha,
                0.0f, origin, Scale, SpriteEffects.None, 0.0f); 
        }

        //overloading the Draw method for world movement
        public void Draw(SpriteBatch spriteBatch, Camera camera) 
        {
            Vector2 position = new Vector2(camera.Position.X, camera.Position.Y);
            origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);
            spriteBatch.Draw(
                Texture, position + origin, SourceRect, Color.White * Alpha,
                0.0f, origin, Scale, SpriteEffects.None, 0.0f);
        }

    }
}
