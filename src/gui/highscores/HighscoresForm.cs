﻿namespace SpaceShooter.gui
{
    public partial class HighscoresForm : CustomForm
    {
        private const int topEntriesCount = 10;
        private const float okBtnHeightRatio = 0.05f;
        private const float okBtnWidthRatio = 0.135f;
        private const float okBtnMarginRatio = 0.05f;

        public HighscoresForm()
        {
            InitializeComponent();
            setBackgroundImage();

            new FormTitleLabel(this, new string(' ', 5) + "HIGHSCORES" + new string(' ', 5));

            List<(int, int, string)> highscores = DatabaseManager.GetTopHighscoresEntries(topEntriesCount);

            while (highscores.Count < topEntriesCount)
                highscores.Add((0, 0, "00:00:00"));

            List<string> nums = Enumerable.Range(1, topEntriesCount).Select(n => n.ToString()).ToList();
            List<string> scores = highscores.Select(item => item.Item1.ToString()).ToList();
            List<string> waves = highscores.Select(item => item.Item2.ToString()).ToList();
            List<string> duration = highscores.Select(item => item.Item3.ToString()).ToList();

            LabelGroup numsGroup = new LabelGroup(this, "No", nums)
            {
                Left = 0
            };

            LabelGroup scoresGroup = new LabelGroup(this, "Score", scores)
            {
                Left = numsGroup.Width
            };

            LabelGroup wavesGroup = new LabelGroup(this, "Wave", waves)
            {
                Left = numsGroup.Width + scoresGroup.Width
            };

            LabelGroup durationGroup = new LabelGroup(this, "Duration", duration)
            {
                Left = numsGroup.Width + scoresGroup.Width + wavesGroup.Width
            };

            OKButton okBtn = new OKButton(this, okBtnWidthRatio, okBtnHeightRatio)
            {
                Top = numsGroup.Top + numsGroup.Height + (int)(Height * okBtnMarginRatio),
            };
        }
    }
}
