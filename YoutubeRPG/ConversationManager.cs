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

        public ConversationManager()
        {

        }
    }
}
