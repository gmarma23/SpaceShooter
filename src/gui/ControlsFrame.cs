﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceShooter.gui
{
    public partial class ControlsFrame : Form
    {
        public ControlsFrame()
        {
            InitializeComponent();
            FormClosed += AppManager.OnSubFrameClosed;
        }
    }
}
