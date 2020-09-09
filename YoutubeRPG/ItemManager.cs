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
    public class ItemManager
    {
        [XmlElement("ItemSource")]
        public List<string> ItemSource;
        public int CurrentItemNumber;

        public List<Item> Items;
        public List<string> itemName;

        public ItemManager()
        {
            ItemSource = new List<string>();
            CurrentItemNumber = 0;
            Items = new List<Item>();
            itemName = new List<string>();
        }
        public Item CurrentItem
        {
            get { return Items[CurrentItemNumber]; }
        }
        public void LoadContent()
        {
            XmlManager<Item> itemLoader = new XmlManager<Item>();

            foreach (string itemSource in ItemSource)
            {
                string[] split = itemSource.Split('/');
                string s = (split[split.Length - 1]).Replace(".xml", String.Empty);
                Item item = itemLoader.Load(itemSource);
                item.LoadContent();
                item.Name = s;
                Items.Add(item);
            }
            if (itemName.Count() > 0)
                CurrentItemNumber = 0;
        }
        public void UnloadContent()
        {
            foreach (Item i in Items)
                i.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            foreach (Item i in Items)
                i.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Item i in Items)
                i.Draw(spriteBatch);
        }
        public void AddItem(Item item)
        {
            Items.Add(item);
        }
        public void AddItem(string itemSource)
        {
            XmlManager<Item> itemLoader = new XmlManager<Item>();
            string[] split = itemSource.Split('/');
            string s = (split[split.Length - 1]).Replace(".xml", String.Empty);
            Item item = itemLoader.Load(itemSource);
            item.LoadContent();
            item.Name = s;
            Items.Add(item);
        }

    }
}
