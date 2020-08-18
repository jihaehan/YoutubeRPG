using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace YoutubeRPG
{
    public class InputListener
    {
        private KeyboardState PrevKeyboardState { get; set; }
        private KeyboardState CurrentKeyboardState { get; set; }

        public HashSet<Keys> KeyList;

        public event EventHandler<KeyboardEventArgs> OnKeyDown = delegate { };
        public event EventHandler<KeyboardEventArgs> OnKeyPressed = delegate { };
        public event EventHandler<KeyboardEventArgs> OnKeyUp = delegate { };

        public InputListener()
        {
            CurrentKeyboardState = Keyboard.GetState();
            PrevKeyboardState = CurrentKeyboardState;
            KeyList = new HashSet<Keys>();
        }

        public void AddKey(Keys key)
        {
            KeyList.Add(key);
        }

        public void Update()
        {
            PrevKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
            FireKeyboardEvents();
        }

        private void FireKeyboardEvents()
        {
            foreach (Keys key in KeyList)
            {
                if (PrevKeyboardState.IsKeyUp(key) && CurrentKeyboardState.IsKeyDown(key))
                {
                    if (OnKeyDown != null)
                        OnKeyDown(this, new KeyboardEventArgs(key, CurrentKeyboardState, PrevKeyboardState));
                }

                if (PrevKeyboardState.IsKeyDown(key) && CurrentKeyboardState.IsKeyUp(key))
                {
                    if (OnKeyUp != null)
                        OnKeyUp(this, new KeyboardEventArgs(key, CurrentKeyboardState, PrevKeyboardState));
                }


                if (PrevKeyboardState.IsKeyDown(key) && CurrentKeyboardState.IsKeyDown(key))
                {
                    if (OnKeyPressed != null)
                        OnKeyPressed(this, new KeyboardEventArgs(key, CurrentKeyboardState, PrevKeyboardState));
                }
            }
        }
    }
}
