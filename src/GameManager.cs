﻿using SpaceShooter.core;
using SpaceShooter.gui;
using SpaceShooter.utils;
using System.Diagnostics;

namespace SpaceShooter
{
    public static class GameManager
    {
        private static GameState? gameState = null;
        private static GameForm? gameForm = null;
        private static TimeManager? timeManager = null;
        private static bool isEnemyBeingRenewed;

        public static void StartNewGame()
        {
            timeManager = new TimeManager(65);
            
            Size gameGridSize = GameForm.GetGameGridSize();
            gameState = new GameState(gameGridSize.Width, gameGridSize.Height);
            gameForm = new GameForm(gameState, (sender, e) => resumeGame());

            isEnemyBeingRenewed = false;

            gameForm.Deactivate += gameFormLostFocusActions;
            gameForm.FormClosed += gameFormClosedActions;
            gameForm.KeyDown += invokeHeroControls;
            gameForm.KeyUp += freeHeroControls;
            gameForm.KeyDown += toggleGameStatus;

            gameForm.Show();

            gameState.RenewEnemySpaceship();

            gameForm.Grid.RenderHeroSpaceship(gameState);
            gameForm.Grid.RenderEnemySpaceship(gameState);
            gameForm.StatsBar.WaveLabel.UpdateValue(gameState.Wave.ToString());
            gameForm.StatsBar.ScoreLabel.UpdateValue(gameState.Score.ToString());

            timeManager.AddMainRecurringAction(gameLoop);
            timeManager.EnableTime();
        }

        private static void gameLoop(object? sender, EventArgs e)
        {
            Debug.Assert(gameState != null);
            Debug.Assert(gameForm != null);
            Debug.Assert(timeManager != null);

            timeManager.UpdateDeltaTime();
            string elapsedTime = StringUtils.FormatSecondsToHMS(TimeManager.ElapsedGameTime);
            gameForm.StatsBar.ElapsedTimeLabel.UpdateValue(elapsedTime);

            if (gameState.IsGameOver())
            {
                gameOverActions();
                return;
            }

            renewEnemySpaceship();

            gameState.MoveGridItems();
            gameState.EnemyTeleport();
            gameState.DisposeInactiveCollidableItems();            

            gameForm.Grid.RelocateSpaceship(gameState, true);
            gameForm.Grid.RelocateSpaceship(gameState, false);
            gameForm.Grid.UpdateSpaceshipAvailableHealth(gameState, true);
            gameForm.Grid.UpdateSpaceshipAvailableHealth(gameState, false);

            gameState.SpaceshipFireLaser(false);
            gameState.EnemyLaunchMissile();

            gameForm.Grid.UpdateActiveCollidableItems(gameState);
        }

        private static async void renewEnemySpaceship()
        {
            Debug.Assert(gameState != null);
            Debug.Assert(gameForm != null);

            if (!gameState.IsEnemyDestroyed() || isEnemyBeingRenewed)
                return;
            
            isEnemyBeingRenewed = true;
            await gameForm.Grid.DestroySpaceship(false);
            gameState.RenewEnemySpaceship();
            gameForm.Grid.RenderEnemySpaceship(gameState);
            gameForm.StatsBar.WaveLabel.UpdateValue(gameState.Wave.ToString());
            gameForm.StatsBar.ScoreLabel.UpdateValue(gameState.Score.ToString());
            isEnemyBeingRenewed = false;
        }

        private static void invokeHeroControls(object? sender, KeyEventArgs e)
        {
            Debug.Assert(gameState != null);

            if (gameState.IsGameOver()) 
                return;

            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.E)
            {
                gameState.SpaceshipFireLaser(true);
                return;
            }
               
            toggleHeroMotionControls(e, true);
        }

        private static void freeHeroControls(object? sender, KeyEventArgs e)
        {
            Debug.Assert(gameState != null);

            if (gameState.IsGameOver()) 
                return;

            toggleHeroMotionControls(e, false);
        }

        private static void toggleHeroMotionControls(KeyEventArgs e, bool isInvoked)
        {
            Debug.Assert(gameState != null);

            IControls conrolableHero = gameState.GetControlableHero();
            switch (e.KeyCode)
            {
                case Keys.A:
                case Keys.Left:
                    conrolableHero.GoLeft = isInvoked;
                    break;
                case Keys.D:
                case Keys.Right:
                    conrolableHero.GoRight = isInvoked;
                    break;
                case Keys.W:
                case Keys.Up:
                    conrolableHero.GoUp = isInvoked;
                    break;
                case Keys.S:
                case Keys.Down:
                    conrolableHero.GoDown = isInvoked;
                    break;
                default:
                    break;
            };
        }

        private static void resumeGame()
        {
            Debug.Assert(gameState != null);
            Debug.Assert(timeManager != null);
            Debug.Assert(gameForm != null);

            if (gameState.IsGameOver())
                return;

            timeManager.EnableTime();
            gameForm.Grid.ResumeGame();
        }

        private static void pauseGame()
        {
            Debug.Assert(gameState != null);
            Debug.Assert(timeManager != null);
            Debug.Assert(gameForm != null);

            if (gameState.IsGameOver())
                return;

            timeManager.DisableTime();
            gameForm.Grid.PauseGame();
        }

        private static void toggleGameStatus(object? sender, KeyEventArgs e)
        {
            Debug.Assert(timeManager != null);

            if (e.KeyCode != Keys.Escape && e.KeyCode != Keys.P)
                return;
            
            if (timeManager.IsTimeActive)
                pauseGame();
            else
                resumeGame();    
        }

        private static async void gameOverActions()
        {
            Debug.Assert(gameState != null);
            Debug.Assert(gameForm != null);
            Debug.Assert(timeManager != null);

            timeManager.DisableTime();
            await gameForm.Grid.DestroySpaceship(true);
            gameForm.Grid.GameOverActions();

            string gameDuration = StringUtils.FormatSecondsToHMS(TimeManager.ElapsedGameTime);
            DatabaseManager.AddHighscoresEntry(gameState.Score, gameState.Wave, gameDuration);
        }

        private static void gameFormLostFocusActions(object? sender, EventArgs e)
        {
            Debug.Assert(gameState != null);

            IControls conrolableHero = gameState.GetControlableHero();
            conrolableHero.ResetControls();
        }

        private static void gameFormClosedActions(object? sender, EventArgs e)
        {
            Debug.Assert(timeManager != null);
            Debug.Assert(gameForm != null);

            timeManager.DisableTime();
            gameForm.Grid.DisposeBackgroundImage();

            gameState = null;
            gameForm = null;
            timeManager = null;
        }
    }
}
