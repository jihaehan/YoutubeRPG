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
    public class CharacterManager
    {
        [XmlElement("CharacterSource")]
        public List<string> CharacterSource;
        public string CurrentCharacterName;

        Dictionary<string, Character> characters;
        public List<string> characterName;

        public CharacterManager()
        {
            CharacterSource = new List<string>();
            CurrentCharacterName = String.Empty;
            characters = new Dictionary<string, Character>();
            characterName = new List<string>();
        }

        public Character CurrentCharacter
        {
            get { return characters[CurrentCharacterName]; }
        }
        public void LoadContent()
        {
            XmlManager<Character> characterLoader = new XmlManager<Character>();
            foreach (string characterSource in CharacterSource)
            {
                string[] split = characterSource.Split('/');
                string s = (split[split.Length - 1]).Replace(".xml", String.Empty);
                Character character = characterLoader.Load(characterSource);

                characterName.Add(s);
                character.QuestDescription = "Characters/Description/" + s;
                character.LoadContent();
                characters.Add(s, character);
            }
            if (characterName.Count() > 0)
                CurrentCharacterName = characterName[0];
        }
        public void UnloadContent()
        {
            foreach (string name in characterName)
            {
                characters[name].UnloadContent();
            }
        }
        public void Update(GameTime gameTime)
        {
            foreach (string name in characterName)
            {
                characters[name].Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (string name in characterName)
            {
                characters[name].Draw(spriteBatch);
            }
        }
        public Character GetCharacter(string characterName)
        {
            if (characters.ContainsKey(characterName))
                return characters[characterName];
            return null;
        }
        public void ChangeCharacter(string characterName)
        {
            if (characters.ContainsKey(characterName))
            {
                CurrentCharacterName = characterName;
                return;
            }
            throw new Exception("Character not found.");
        }

    }
}
