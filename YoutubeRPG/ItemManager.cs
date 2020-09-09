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

        List<Item> items;
        public List<string> itemName;

        public ItemManager()
        {
            ItemSource = new List<string>();
            CurrentItemNumber = 0;
            items = new List<Item>();
            itemName = new List<string>();
        }
        public Item CurrentItem
        {
            get { return items[CurrentItemNumber]; }
        }
        public void LoadContent()
        {
            XmlManager<Item> itemLoader = new XmlManager<Item>();

            foreach (string itemSource in ItemSource)
            {
                string[] split = itemSource.Split('/');
                string s = (split[split.Length - 1]).Replace(".xml", String.Empty);
                Item item = itemLoader.Load(itemSource);
                item.Name = s;
                item.LoadContent();
                items.Add(item);
            }
            if (itemName.Count() > 0)
                CurrentItemNumber = 0;
        }
        public void UnloadContent()
        {
            foreach (Item i in items)
                i.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            foreach (Item i in items)
                i.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Item i in items)
                i.Draw(spriteBatch);
        }
        public void AddItem(Item item)
        {
            items.Add(item);
        }
        public void AddItem(string itemSource)
        {
            XmlManager<Item> itemLoader = new XmlManager<Item>();
            string[] split = itemSource.Split('/');
            string s = (split[split.Length - 1]).Replace(".xml", String.Empty);
            Item item = itemLoader.Load(itemSource);
            item.Name = s;
            item.LoadContent();
            items.Add(item);
        }

    }
}
