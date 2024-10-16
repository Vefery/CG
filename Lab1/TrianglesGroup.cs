﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab1
{
    public partial class TrianglesGroupUC : UserControl
    {
        TriangleGroup assignedGroup;
        public delegate void OnStartEdit(TriangleGroup assignedGroup);
        public OnStartEdit onStartEdit;

        public TrianglesGroupUC(TriangleGroup assignedGroup, string name)
        {
            InitializeComponent();
            this.assignedGroup = assignedGroup;
            assignedGroup.onDeleted += () => this.Dispose();
            groupName.Text = name;
        }

        public void SwitchButton(bool enable)
        {
            editButton.Enabled = enable;
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            onStartEdit?.Invoke(assignedGroup);
        }
    }
}
