// MIT License - Copyright (C) Shawn Rakowski
// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

using Microsoft.Xna.Framework.Input;

namespace LD42.MiningLogisticsGame.Engine
{
    public class GameInput
    {
        internal static KeyboardState PrevKBState { get; private set; }
        internal static KeyboardState CurrKBState { get; private set; }

        internal static MouseState PrevMouseState { get; private set; }
        internal static MouseState CurrMouseState { get; private set; }

        internal static GamePadState[] _prevGamePadStates = new GamePadState[4];
        internal static GamePadState[] _currGamePadStates = new GamePadState[4];

        internal static void Update()
        {
            PrevKBState = CurrKBState;
            CurrKBState = Keyboard.GetState();

            PrevMouseState = CurrMouseState;
            CurrMouseState = Mouse.GetState();

            for (int i = 0; i < 4; i++)
            {
                _prevGamePadStates[i] = _currGamePadStates[i];
                _currGamePadStates[i] = GamePad.GetState(i);
            }
        }
    }
}
