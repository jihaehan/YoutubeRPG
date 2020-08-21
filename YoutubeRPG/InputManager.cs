using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Input;

namespace YoutubeRPG
{
    public delegate void GameAction(eButtonState buttonState);

    public class InputManager //singleton class
    {
        private InputListener m_Input;
        private Dictionary<Keys, GameAction> m_KeyBindings; 

        public InputManager()
        {
            m_Input = new InputListener();

            m_Input.OnKeyDown += this.OnKeyDown;
            m_Input.OnKeyPressed += this.OnKeyPressed;
            m_Input.OnKeyUp += this.OnKeyUp;
        }

        public void LoadContent()
        {
            m_KeyBindings = new Dictionary<Keys, GameAction>();
        }
        public void UnloadContent()
        {
            m_KeyBindings.Clear();
        }

        public void Update()
        {
            m_Input.Update();
        }

        public void OnKeyDown(object sender, KeyboardEventArgs e)
        {
            GameAction action = m_KeyBindings[e.Key];

            if (action != null)
            {
                action(eButtonState.DOWN);
            }
        }

        public void OnKeyUp(object sender, KeyboardEventArgs e)
        {
            GameAction action = m_KeyBindings[e.Key];

            if (action != null)
            {
                action(eButtonState.UP);
            }
        }

        public void OnKeyPressed(object sender, KeyboardEventArgs e)
        {
            GameAction action = m_KeyBindings[e.Key];

            if (action != null)
            {
                action(eButtonState.PRESSED);
            }
        }

        public void AddKeyboardBinding(Keys key, GameAction action)
        {
            if (!m_KeyBindings.ContainsKey(key))
            {
                m_Input.AddKey(key);
                m_KeyBindings.Add(key, action);
            }
            else
            {
                m_KeyBindings.Remove(key);
                m_KeyBindings.Add(key, action);
            }
        }
    }
}
