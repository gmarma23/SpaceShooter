﻿namespace SpaceShooter.src.gui.game.grid
{
    public class PauseMenuButton : CustomButton
    {
        private const float gameOverButtonWidthRatio = 0.1f;
        private const float gameOverButtonHeightRatio = 0.05f;

        public PauseMenuButton(Control parent, string text) : base(parent)
        {
            Width = (int)(Parent.Width * gameOverButtonWidthRatio);
            Height = (int)(Parent.Height * gameOverButtonHeightRatio);
            Left = Parent.ClientRectangle.Width / 2 - Width / 2;
            Text = text;
        }
    }
}