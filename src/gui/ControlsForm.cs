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
    public partial class ControlsForm : Form
    {
        public ControlsForm()
        {
            InitializeComponent();
            FormClosed += AppManager.OnSubFormClosed;
        }
    }
}