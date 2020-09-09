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
    public class ConversationScene
    {
        public event EventHandler OnConversationChanged; //type of delegate, which calls an 'event'

        public string Axis, Effects;
        public Vector2 Alignment, Spacing; 
        [XmlElement("Item")]
        public List<ConversationItem> Items;
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
                OnConversationChanged(this, null);
            }
        }
        public void Transition(float alpha)
        {
            foreach (ConversationItem item in Items)
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
            foreach (ConversationItem item in Items)
                dimensions += new Vector2(item.Image.SourceRect.Width,
                    item.Image.SourceRect.Height);

            dimensions = new Vector2((ScreenManager.Instance.Dimensions.X -
                dimensions.X) / 2, (ScreenManager.Instance.Dimensions.Y - dimensions.Y) / 2);

            foreach (ConversationItem item in Items)
            {
                if (Axis == "X")
                    item.Image.Position = new Vector2(dimensions.X,
                        (ScreenManager.Instance.Dimensions.Y - item.Image.SourceRect.Height) / 2) + Alignment;
                else if (Axis == "Y")
                    item.Image.Position = new Vector2((ScreenManager.Instance.Dimensions.X - item.Image.SourceRect.Width) / 2,
                        dimensions.Y) + Alignment;
                dimensions += new Vector2(item.Image.SourceRect.Width,
                    item.Image.SourceRect.Height) + Spacing;
            }
        }

        public ConversationScene()
        {
            id = String.Empty;
            itemNumber = 0;
            Effects = String.Empty;
            Axis = "Y";
            Alignment = Vector2.Zero;
            Spacing = Vector2.Zero;
            Items = new List<ConversationItem>();
        }

        public void LoadContent()
        {
            string[] split = Effects.Split(':');
            foreach (ConversationItem item in Items)
            {
                item.Image.LoadContent();
                foreach (string s in split)
                    item.Image.ActivateEffect(s);
            }
            AlignMenuItems();
        }
        public void UnloadContent()
        {
            foreach (ConversationItem item in Items)
                item.Image.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
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
            foreach (ConversationItem item in Items)
                item.Image.Draw(spriteBatch);
        }
    }
}
