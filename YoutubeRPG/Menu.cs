using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{
    public class Menu
    {
        public event EventHandler OnMenuChanged; //type of delegate, which calls an 'event'

        public string Axis, Effects;
        public Vector2 Alignment, Spacing, Grid;
        public Image Image;
        [XmlElement("Item")]
        public List<MenuItem> Items;
        public bool Active;
        int itemNumber;
        string id;

        
        public int ItemNumber
        {
            get { return itemNumber; }
            set { itemNumber = value; }
        }

        public string ID
        { 
            get { return id; }
            set
            {
                id = value;
                OnMenuChanged(this, null);
            }
        }
        public void Transition(float alpha)
        {
            foreach(MenuItem item in Items)
            {
                item.Image.IsActive = true;
                item.Image.Alpha = alpha;
                if (alpha == 0.0f)
                    item.Image.FadeEffect.Increase = true;
                else
                    item.Image.FadeEffect.Increase = false;
            }
        }

        void AlignMenuItems()
        {
            Vector2 dimensions = Vector2.Zero;
            foreach (MenuItem item in Items)
                dimensions += new Vector2(item.Image.SourceRect.Width,
                    item.Image.SourceRect.Height);

            dimensions = new Vector2((ScreenManager.Instance.Dimensions.X -
                dimensions.X) / 2, (ScreenManager.Instance.Dimensions.Y - dimensions.Y) / 2);

            if (Grid == Vector2.One)
            {
                if (Axis == "X")
                    Grid.X = Items.Count();
                else if (Axis == "Y")
                    Grid.Y = Items.Count();
            }

            foreach (MenuItem item in Items)
            {
                if (Axis == "X" && Grid.X > 0)
                {
                    item.Image.Position = new Vector2(dimensions.X,
                        (ScreenManager.Instance.Dimensions.Y - item.Image.SourceRect.Height) / 2) + Alignment;
                    Grid.X--; 
                }
                else if (Axis == "Y" && Grid.Y > 0)
                {
                    item.Image.Position = new Vector2((ScreenManager.Instance.Dimensions.X - item.Image.SourceRect.Width) / 2, dimensions.Y) + Alignment;
                    Grid.Y--;
                }
                dimensions += new Vector2(item.Image.SourceRect.Width,
                    item.Image.SourceRect.Height) + Spacing;

            }
        }

        public Menu()
        {
            id = String.Empty;
            itemNumber = 0;
            Effects = String.Empty;
            Axis = "Y";
            Items = new List<MenuItem>();
            Alignment = Spacing = Vector2.Zero;
            Grid = Vector2.One;
            Active = true;
        }

        public void LoadContent()
        {
            if (Image != null)
                Image.LoadContent();
            string[] split = Effects.Split(':');
            foreach(MenuItem item in Items)
            {
                item.Image.LoadContent();
                foreach (string s in split)
                    item.Image.ActivateEffect(s);
            }
            AlignMenuItems();
        }
        public void UnloadContent()
        {
            if (Image != null)
                Image.UnloadContent();
            foreach (MenuItem item in Items)
                item.Image.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            if (Image != null)
                Image.Update(gameTime);

            if (itemNumber < 0) 
                itemNumber = 0; 
            else if (itemNumber > Items.Count - 1) 
                itemNumber = Items.Count - 1; 

            for (int i = 0; i < Items.Count; i++)
            {
                if (i == itemNumber) 
                    Items[i].Image.IsActive = true; 
                else 
                    Items[i].Image.IsActive = false;

                Items[i].Image.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Image != null)
                Image.Draw(spriteBatch);
            foreach (MenuItem item in Items)
                item.Image.Draw(spriteBatch);
        }
    }
}
