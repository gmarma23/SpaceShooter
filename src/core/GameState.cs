﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceShooter.core
{
    internal class GameState
    {
        private GameGrid grid;
        private HeroSpaceship hero;
        private EnemySpaceship enemy;
        private List<LaserBlast> activeLaserBlasts;

        public int GridXDimension { get => grid.DimensionX; }
        public int GridYDimension { get => grid.DimensionY; }
        public int Score { get; private set; }

        public GameState(int gridDimensionX = 1360, int gridDimensionY = 760)
        {
            grid = new GameGrid(gridDimensionX, gridDimensionY);
            hero = new HeroSpaceship(grid);
            activeLaserBlasts = new List<LaserBlast>();
        }
        
        public void RenewEnemySpaceship(EnemySpaceshipType enemyType)
        {
            enemy = enemyType switch
            {
                EnemySpaceshipType.Fighter => new EnemyFighterSpaceship(grid),
                EnemySpaceshipType.Teleporter => new EnemyTeleporterSpaceship(grid),
                EnemySpaceshipType.Boss => new EnemyBossSpaceship(grid, hero),
                _ => throw new Exception()
            };
        }

        public void HeroGoesUp(bool isInvoked) => hero.GoUp = isInvoked;

        public void HeroGoesDown(bool isInvoked) => hero.GoDown = isInvoked;

        public void HeroGoesLeft(bool isInvoked) => hero.GoLeft = isInvoked;

        public void HeroGoesRight(bool isInvoked) => hero.GoRight = isInvoked;

        public void MoveSpaceship(bool isEnemy) => getSpaceship(isEnemy).Move();

        public bool TeleportEnemySpaceship()
        {
            try
            {
                ((ITeleport)enemy).Teleport();
                return true;
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        public void SpaceshipGetLocation(bool isEnemy, ref int x, ref int y)
        {
            Spaceship spaceship = getSpaceship(isEnemy);
            x = spaceship.LocationX;
            y = spaceship.LocationY;
        }

        public void SpaceshipGetSize(bool isEnemy, ref int width, ref int height)
        {
            Spaceship spaceship = getSpaceship(isEnemy);
            width = spaceship.Width;
            height = spaceship.Height;
        }

        public void LaserBlastGetLocation(int numCode, ref int x, ref int y)
        {
            LaserBlast? laserBlast = getLaserBlastByNumCode(numCode);
            Debug.Assert(laserBlast != null);
            x = laserBlast.LocationX;
            y = laserBlast.LocationY;
        }

        public void LaserBlastGetSize(int numCode, ref int width, ref int height)
        {
            LaserBlast? laserBlast = getLaserBlastByNumCode(numCode);
            Debug.Assert(laserBlast != null);
            width = laserBlast.Width;
            height = laserBlast.Height;
        }

        public List<int> SpaceshipFireLaser(bool isEnemy)
        {
            List<int> newlaserBlastsNumCodes = new List<int>();
            Spaceship spaceship = getSpaceship(isEnemy);

            List<LaserBlast> firedLaserBlasts = spaceship.FireLaser();
            activeLaserBlasts.AddRange(firedLaserBlasts);

            foreach(LaserBlast laserBlast in firedLaserBlasts)
            {
                int newNumCode = laserBlast.NumCode;
                Debug.Assert(!newlaserBlastsNumCodes.Contains(newNumCode));
                newlaserBlastsNumCodes.Add(newNumCode);
            }

            return newlaserBlastsNumCodes;
        }

        public void MoveLaserBlast(int numCode)
        {
            LaserBlast? laserBlast = getLaserBlastByNumCode(numCode);
            Debug.Assert(laserBlast != null);

            laserBlast.Move();
        }

        public bool LaserBlastHitedTargetSpaceship(int numCode)
        {
            LaserBlast? laserBlast = getLaserBlastByNumCode(numCode);
            Debug.Assert(laserBlast != null);
            Spaceship targetSpaceship = getSpaceship(laserBlast.IsEnemy);

            if (!GameGrid.ItemsIntersect(laserBlast, targetSpaceship))
                return false;

            targetSpaceship.TakeDamage(laserBlast.Damage);
            return true;
        }

        public bool LaserBlastIsOutOfBounds(int numCode)
        {
            LaserBlast? laserBlast = getLaserBlastByNumCode(numCode);
            Debug.Assert(laserBlast != null);

            return laserBlast.LocationY < grid.GetItemMinPossibleY() ||
                laserBlast.LocationY > grid.GetItemMaxPossibleY(laserBlast);
        }

        public void DisposeLaserBlast(int numCode)
        {
            LaserBlast? laserBlast = getLaserBlastByNumCode(numCode);
            Debug.Assert(laserBlast != null);
            activeLaserBlasts.Remove(laserBlast);
        }

        public bool IsEnemyLaserBlast(int numCode)
        {
            LaserBlast? laserBlast = getLaserBlastByNumCode(numCode);
            Debug.Assert(laserBlast != null);
            return laserBlast.IsEnemy;
        }

        public bool IsEnemyDestroyed()
        {
            return enemy.IsDestroyed;
        }

        public bool IsGameOver()
        {
            return hero.IsDestroyed;
        }

        private LaserBlast? getLaserBlastByNumCode(int numCode)
        {
            return activeLaserBlasts.Find(laserBlast => laserBlast.NumCode == numCode);
        }

        private Spaceship getSpaceship(bool isEnemy)
        {
            return isEnemy ? enemy : hero;
        }
    }
}
