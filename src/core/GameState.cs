﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter.core
{
    internal class GameState
    {
        private HeroSpaceship hero;
        private List<EnemySpaceship> enemies;
        private List<LaserBlast> activeLaserBlasts;

        public int GridXDimension { get; private init; }
        public int GridYDimension { get; private init; }
        public int Score { get; private set; }

        public GameState(int gridXDimension, int gridYDimension)
        {
            GridXDimension = gridXDimension;
            GridYDimension = gridYDimension;
            //hero = new HeroSpaceship();
            enemies = new List<EnemySpaceship>();
            activeLaserBlasts = new List<LaserBlast>();
        }

        public double GetHeroHealthPercentage()
        {
            return hero.AvailableHP / hero.TotalHP;
        }

        public (int, int) GetHeroLocation()
        {
            return (hero.XLocation, hero.YLocation);
        }

        public void FireHeroLaser()
        {
            activeLaserBlasts.Add(new LaserBlast());
        }

        public double GetEmemyHealthPercentage(int enemyIndex)
        {
            EnemySpaceship enemy = getEnemyByIndex(enemyIndex);
            return enemy.AvailableHP / enemy.TotalHP;
        }

        public (int, int) GetEnemyLocation(int enemyIndex)
        {
            EnemySpaceship enemy = getEnemyByIndex(enemyIndex);
            return (enemy.XLocation, enemy.YLocation);
        }

        public void FireEnemyLaser(int enemyIndex)
        {
            EnemySpaceship enemy = getEnemyByIndex(enemyIndex);
            activeLaserBlasts.Add(new LaserBlast());
        }

        public void DestroyEnemy(int enemyIndex)
        {
            EnemySpaceship enemy = getEnemyByIndex(enemyIndex);
            enemies.Remove(enemy);
        }

        public bool IsGameOver()
        {
            return hero.IsDestroyed;
        }

        private EnemySpaceship getEnemyByIndex(int enemyIndex)
        {
            EnemySpaceship? enemy = enemies.Find(enemy => enemy.Index == enemyIndex);
            if (enemy == null)
                throw new Exception();
            else
                return enemy;
        }
    }
}
