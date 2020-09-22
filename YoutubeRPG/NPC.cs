using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{
    public class NPC
    {
        public string Name;
        public string NpcXml;
        public string PartyXml;
        public bool TalkOnly;
        Character character;

        public NPC()
        {
            NpcXml = String.Empty;
            PartyXml = String.Empty;
            character = new Character();
            TalkOnly = false;
        }

        public void LoadContent()
        {
            XmlManager<Character> characterLoader = new XmlManager<Character>();
            character = characterLoader.Load(NpcXml);
            character.LoadContent(PartyXml);
        }
        public void UnloadContent()
        {
            character.UnloadContent();
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            character.Draw(spriteBatch);
        }
        public void Update(GameTime gameTime)
        {
            character.Update(gameTime);
        }
        public Character GetCharacter()
        {
            return character;
        }
    }
}
