using SpaceShooter.core;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;

namespace SpaceShooter.gui
{
    public partial class GameForm : CustomForm
    {
        private const float statsPanelHeightRatio = 0.05f;

        public GameGridGui Grid { get; private init; }

        public StatsBar StatsBar { get; private init; }

        public GameForm(IGameStateUI gameState)
        {
            InitializeComponent();
            disposeBackgroundImage();

            int statsPanelWidth = gameState.Grid.DimensionX;
            int statsPanelHeight = (int)(statsPanelHeightRatio * gameState.Grid.DimensionY);

            int clientWidth = gameState.Grid.DimensionX;
            int clientHight = gameState.Grid.DimensionY + statsPanelHeight;
            ClientSize = new Size(clientWidth, clientHight);

            FormClosed += AppManager.OnSubFormClosed;

            Grid = new GameGridGui(this, gameState)
            {
                Top = statsPanelHeight
            };

            StatsBar = new StatsBar(this, statsPanelWidth, statsPanelHeight);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                // Double-buffer a window along with all of its child controls.
                // Everything gets rendered into an off-screen buffer.

                CreateParams cp = base.CreateParams;
                // Turn on WS_EX_COMPOSITED
                cp.ExStyle |= 0x02000000;  
                return cp;
            }
        }
    }
}
