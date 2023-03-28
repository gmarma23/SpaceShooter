﻿using System;

namespace SpaceShooter.core
{
    public abstract class Missile : Weapon
    {
        public Missile(ILaunchMissile missileLauncher, GameGrid grid) 
        {
            defaultWidthRatio = 0.03f;
            defaultHeightRatio = 2;
            absMaxDisplacement = 360;

            setSize(grid);
            setBounds(grid);

            LocationX = missileLauncher.LocationX + (missileLauncher.Width / 2) - (Width / 2);
            damage = missileLauncher.MissileDamage;
        }

        public override void Move()
        {
            updateDisplacementX();
            base.Move();
        }

        protected void updateDisplacementX()
        {
            if (Target == null)
            {
                displacementX = 0;
                return;
            }

            int deltaMiddleX = Target.LocationX + (Target.Width / 2) - (LocationX + (Width / 2));

            if (deltaMiddleX == 0)
            {
                displacementX = 0;
                return;
            }
                
            int deltaMiddleXSign = Math.Sign(deltaMiddleX);
            int nexDisplacementX = deltaMiddleXSign * absMaxDisplacement;
            int newDeltaMiddleX = deltaMiddleX + nexDisplacementX;

            displacementX = deltaMiddleXSign == Math.Sign(newDeltaMiddleX) ? nexDisplacementX : deltaMiddleX;
        }
    }
}
