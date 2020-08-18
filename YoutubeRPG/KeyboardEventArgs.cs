using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace YoutubeRPG
{
    public class KeyboardEventArgs : EventArgs
    {
        public KeyboardEventArgs(Keys key, KeyboardState currentKeyboardState, KeyboardState prevKeyboardState)
        {
            CurrentState = currentKeyboardState;
            PrevState = prevKeyboardState;
            Key = key;
        }

        public readonly KeyboardState CurrentState;
        public readonly KeyboardState PrevState;
        public readonly Keys Key;
    }
}
