using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace YoutubeRPG
{
    public class Conversation
    {
        ConversationScene conversationScene;
        bool isTransitioning;

        void Transition(GameTime gameTime)
        {
            if (isTransitioning)
            {
                for (int i = 0; i < conversationScene.Items.Count; ++i)
                {
                    conversationScene.Items[i].Image.Update(gameTime);
                    float first = conversationScene.Items[0].Image.Alpha;
                    float last = conversationScene.Items[conversationScene.Items.Count - 1].Image.Alpha;
                    if (first == 0.0f && last == 0.0f)
                        conversationScene.ID = conversationScene.Items[conversationScene.ItemNumber].LinkID;
                    else if (first == 1.0f && last == 1.0f)
                    {
                        isTransitioning = false;
                        foreach (ConversationItem item in conversationScene.Items)
                            item.Image.RestoreEffects();
                    }
                }
            }
        }
        public Conversation()
        {
            conversationScene = new ConversationScene();
            conversationScene.OnConversationChanged += conversation_OnConversationChange;    
        }
        public void conversation_OnConversationChange(object sender, EventArgs e)
        {
            XmlManager<ConversationScene> XmlConversationManager = new XmlManager<ConversationScene>();
            conversationScene.UnloadContent();
            conversationScene = XmlConversationManager.Load(conversationScene.ID);
            conversationScene.LoadContent();
            conversationScene.OnConversationChanged += conversation_OnConversationChange;
            conversationScene.Transition(0.0f);

            foreach (ConversationItem item in conversationScene.Items)
            {
                item.Image.StoreEffects();
                item.Image.ActivateEffect("FadeEffect");
            }
        }
        public void LoadContent(string conversationPath)
        {
            if (conversationPath != String.Empty)
                conversationScene.ID = conversationPath;
        }
        public void UnloadContent()
        {
            conversationScene.UnloadContent();
        }
        public void Update(GameTime gameTime)
        {
            if (!isTransitioning)
                conversationScene.Update(gameTime);
            Transition(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            conversationScene.Draw(spriteBatch);
        }
        public void ConversationSelect(eButtonState buttonState)
        {
            if (buttonState == eButtonState.DOWN && !isTransitioning)
            {
                if (conversationScene.Items[conversationScene.ItemNumber].LinkType == "Screen")
                    ScreenManager.Instance.ChangeScreens(conversationScene.Items[conversationScene.ItemNumber].LinkID);
                else
                {
                    isTransitioning = true;
                    conversationScene.Transition(1.0f);
                    foreach (ConversationItem item in conversationScene.Items)
                    {
                        item.Image.StoreEffects();
                        item.Image.ActivateEffect("FadeEffect");
                    }
                }
            }
        }
        public void SelectRight(eButtonState buttonState)
        {
            if (conversationScene.Axis == "X" && buttonState == eButtonState.DOWN)
                conversationScene.ItemNumber++;
        }
        public void SelectLeft(eButtonState buttonState)
        {
            if (conversationScene.Axis == "X" && buttonState == eButtonState.DOWN)
                conversationScene.ItemNumber--;
        }
        public void SelectDown(eButtonState buttonState)
        {
            if (conversationScene.Axis == "Y" && buttonState == eButtonState.DOWN)
                conversationScene.ItemNumber++;
        }
        public void SelectUp(eButtonState buttonState)
        {
            if (conversationScene.Axis == "Y" && buttonState == eButtonState.DOWN)
                conversationScene.ItemNumber--;
        }
    }
}
