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
    public class ConversationManager
    {
        [XmlElement("ConversationSource")]
        public List<string> ConversationSource;
        public string CurrentConversationName;

        Dictionary<string, Conversation> conversations;
        List<string> conversationName;
        float speed;
        Vector2 position;
        bool isVisible;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public float Speed
        {
            get { return speed; }
            set { speed = (float)MathHelper.Clamp(speed, 4f, 16f); }        //clamp the speed
        }
        public Matrix Transformation
        {
            get { return Matrix.CreateTranslation(new Vector3(-Position, 0f)); }
        }

        public ConversationManager()
        {
            speed = 4f;
            isVisible = false;
        }
        public void LoadContent()
        {

        }
        public void UnLoadContent()
        {

        }
        public void Update(GameTime gameTime)
        {

        }
        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
