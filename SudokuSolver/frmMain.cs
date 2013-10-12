/*
Copyright (C) 2013 Matej Zavrsnik <matejzavrsnik@gmail.com> (matejzavrsnik.com)

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
associated documentation files (the "Software"), to deal in the Software without restriction,
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SudokuSolver
{
    public partial class frmMain : Form
    {
        public TextBox[,] m_boxes = new TextBox[Settings.SudokuDimension, Settings.SudokuDimension];
        private Button btnSolve = new Button();
        PresentationLogic m_presentation;

        public frmMain()
        {
            InitializeComponent();
            m_presentation = new PresentationLogic(this);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            for (int j = 0; j < Settings.SudokuDimension; j++)
                for (int i = 0; i < Settings.SudokuDimension; i++)
                {
                    TextBox box = new TextBox();
                    box.Size = new Size(Settings.BoxSize, Settings.BoxSize);
                    box.Location = new Point(Settings.ControlSpacing + Settings.BoxSize * i,
                        menuStrip.Height + Settings.ControlSpacing + Settings.BoxSize * j);
                    box.TextAlign = HorizontalAlignment.Center;
                    box.MaxLength = 1;
                    box.KeyDown += new System.Windows.Forms.KeyEventHandler(this.box_KeyDown);
                    this.Controls.Add(box);
                    m_boxes[i, j] = box;
                }
            btnSolve.Size = new Size(Settings.ButtonWidth,Settings.ButtonHeight);
            btnSolve.Text = "Solve";
            this.btnSolve.Click += new System.EventHandler(this.btnSolve_Click);
            btnSolve.Location = new Point(2*Settings.ControlSpacing + Settings.BoxSize * Settings.SudokuDimension, 
                menuStrip.Height + Settings.ControlSpacing);
            this.Controls.Add(btnSolve);
            this.Size = new Size(5 + 3 * Settings.ControlSpacing + Settings.BoxSize * Settings.SudokuDimension + Settings.ButtonWidth,
                menuStrip.Height + 27 + 2 * Settings.ControlSpacing + Settings.BoxSize * Settings.SudokuDimension);
            m_presentation.CommandNew();
        }

        private void box_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox box = (TextBox)sender;
            m_presentation.CommandKeyDown( box, e.KeyCode );
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            m_presentation.CommandSolve();
            //m_presentation.CommandTest();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_presentation.CommandNew();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_presentation.CommandOpen();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_presentation.CommandSaveAs();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_presentation.CommandSave();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }

    }
}
