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
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace SudokuSolver
{
    class PresentationLogic
    {
        private frmMain m_mainForm = null;
        private string m_puzzleFileName = "";
        private bool m_hasFilename = false;
        private string m_path = "";
        private BusinessLogic m_business = new BusinessLogic();

        internal PresentationLogic(frmMain main)
        {
            m_mainForm = main;
            m_business.ResultAvailable += new EventHandler(business_ResultAvailable);
        }

        internal void CommandSave()
        {
            if (!m_hasFilename)
            {
                CommandSaveAs();
            }
            else
            {
                Sudoku fromGui = GuiToPuzzle();
                m_business.saveFile(m_path, fromGui);
            }
        }

        internal void CommandSaveAs()
        {
            SaveFileDialog saveSudokuDialog = new SaveFileDialog();
            if (m_hasFilename)
                saveSudokuDialog.FileName = m_puzzleFileName;
            else
                saveSudokuDialog.FileName = "newpuzzle.txt";
            saveSudokuDialog.InitialDirectory = m_path;
            saveSudokuDialog.Filter = "Web friendly format (*.txt)|*.txt|VBForums Contest format (*.msk;*.sol)|*.msk;*.sol";
            DialogResult res = saveSudokuDialog.ShowDialog();
            if (res == DialogResult.Cancel)
                return;

            Sudoku fromGui = GuiToPuzzle();
            m_business.saveFile(saveSudokuDialog.FileName, fromGui);
            setPuzzleName(saveSudokuDialog.FileName);
        }

        internal void CommandOpen()
        {
            OpenFileDialog openSudokuDialog = new OpenFileDialog();
            if (m_hasFilename)
                openSudokuDialog.FileName = m_puzzleFileName;
            openSudokuDialog.InitialDirectory = m_path;
            openSudokuDialog.Filter = "Web friendly format (*.txt)|*.txt|VBForums Contest format (*.msk;*.sol)|*.msk;*.sol";

            DialogResult res = openSudokuDialog.ShowDialog();
            if (res == DialogResult.Cancel)
                return;

            Sudoku fromFile = m_business.openFile(openSudokuDialog.FileName);
            CommandNew();
            PuzzleToGui(fromFile);
            setPuzzleName(openSudokuDialog.FileName);
        }

        internal void CommandNew()
        {
            m_business.clear();
            for (int i = 0; i < Settings.SudokuDimension; i++)
                for (int j = 0; j < Settings.SudokuDimension; j++)
                    m_mainForm.m_boxes[i, j].Text = "";
            setPuzzleName("New puzzle");
            m_hasFilename = false; // prevent saving "New puzzle" files
        }

        internal void CommandSolve()
        {
            Sudoku formSudoku = GuiToPuzzle();
            m_business.startWith(formSudoku);
            m_business.solve();
        }

        private void business_ResultAvailable(object sender, EventArgs e)
        {
            Sudoku sudokuSolution = m_business.Statistics.BestSolution;
            ShowStatisticsMessage(m_business.Statistics);
            PuzzleToGui(sudokuSolution);
        }

        internal void CommandTest()
        {
            //top level plan:
            //get directory
                // for each file in directory
                    // read file as solved sudoku
                        // for i = 1 to 81
                            // for n attempts
                                // remove that many values from random fields
                                    // solve, gather statistics
            int possibleEmpty = 60;
            int repetitions = 5;
            string dir = @"C:\matej\sudokutest\solved";
            TestBusinessLogic business = new TestBusinessLogic(dir, possibleEmpty, repetitions);
            business.GO();
        }

        private void ShowStatisticsMessage(Solution statistics)
        {
            string message = "";
            string header = "";
            MessageBoxIcon icon;
            if( statistics.Error )
            {
                header += "Sudoku NOT SOLVED!";
                icon = MessageBoxIcon.Error;
                message += "Program has ran out of memory or the sudoku is unsolvable.\n\n";
            }
            else
            {
                header += "Sudoku SOLVED!";
                icon = MessageBoxIcon.Information;
            }
            message += "Iterations needed: " + statistics.IterationsNeeded.ToString() + "\n";
            message += "Sudokus generated: " + statistics.SudokusGenerated.ToString() + "\n";
            message += "Time needed: " + statistics.TimeNeeded.ToString() + "\n";
            int score = statistics.BestSolution.Score - statistics.BestSolution.PathLength;
            message += "Best score: " + score.ToString() + "\n";
            MessageBox.Show(message, header, MessageBoxButtons.OK, icon);
        }

        private Sudoku GuiToPuzzle()
        {
            Sudoku puzzle = new Sudoku();
            for (int i = 0; i < Settings.SudokuDimension; i++)
                for (int j = 0; j < Settings.SudokuDimension; j++)
                {
                    if (m_mainForm.m_boxes[i, j].Text.Length > 0)
                        puzzle.setBox(i, j, byte.Parse(m_mainForm.m_boxes[i, j].Text));
                    else
                        puzzle.setBox(i, j, 0);
                }
            return puzzle;
        }


        private void PuzzleToGui(Sudoku puzzle)
        {
            for (int i = 0; i < Settings.SudokuDimension; i++)
                for (int j = 0; j < Settings.SudokuDimension; j++)
                    if (puzzle.getBox(i, j) != 0)
                        m_mainForm.m_boxes[i, j].Text = puzzle.getBox(i, j).ToString();
        }

        private void setPuzzleName(string puzzlePath)
        {
            m_path = Path.GetFullPath(puzzlePath);
            m_puzzleFileName = Path.GetFileName(puzzlePath);
            m_hasFilename = true;
            m_mainForm.Text = m_puzzleFileName;
        }

        internal void CommandKeyDown(TextBox box, Keys keys)
        {
            Point coordinates = getBoxCoordinates(box);
            if (keys == Keys.Left || keys == Keys.Right ||
                keys == Keys.Up || keys == Keys.Down)
            {
                handleNavigation(coordinates, keys);
            }
            else if (keys == Keys.Back)
            {
                Point newCoor = handleNavigation(coordinates, Keys.Left);
                m_mainForm.m_boxes[newCoor.X, newCoor.Y].Focus();
            }
            else if (char.IsLetterOrDigit((char)keys)) // is digit
            {
                handleNavigation(coordinates, Keys.Right);
            }
            else
            {
                box.Text = "";
            }
        }

        private Point getBoxCoordinates(TextBox box)
        {
            Point coor = new Point();
            bool foundBox = false;
            int i = 0, j = 0;
            for (i = 0; i < Settings.SudokuDimension; i++)
            {
                for (j = 0; j < Settings.SudokuDimension; j++)
                    if (m_mainForm.m_boxes[i, j] == box)
                    {
                        foundBox = true;
                        break;
                    }
                if (foundBox)
                    break;
            }
            coor.X = i;
            coor.Y = j;
            return coor;
        }

        private Point handleNavigation(Point boxCoor, Keys key)
        {
            Point newCoor = new Point();
            newCoor = boxCoor;
            if (key == Keys.Left)
            {
                if (boxCoor.X > 0)
                {
                    m_mainForm.m_boxes[boxCoor.X - 1, boxCoor.Y].Focus();
                    --newCoor.X;
                }
                else if (boxCoor.Y > 0)
                {
                    m_mainForm.m_boxes[Settings.SudokuDimension - 1, boxCoor.Y - 1].Focus();
                    newCoor.X = Settings.SudokuDimension - 1;
                    --newCoor.Y;
                }
            }
            else if (key == Keys.Right)
            {
                if (boxCoor.X < (Settings.SudokuDimension - 1))
                {
                    m_mainForm.m_boxes[boxCoor.X + 1, boxCoor.Y].Focus();
                    ++newCoor.X;
                }
                else if (boxCoor.Y < (Settings.SudokuDimension - 1))
                {
                    m_mainForm.m_boxes[0, boxCoor.Y + 1].Focus();
                    newCoor.X = 0;
                    ++newCoor.Y;
                }
            }
            else if (key == Keys.Up && boxCoor.Y > 0)
            {
                m_mainForm.m_boxes[boxCoor.X, boxCoor.Y - 1].Focus();
                --newCoor.Y;
            }
            else if (key == Keys.Down && boxCoor.Y < (Settings.SudokuDimension - 1))
            {
                m_mainForm.m_boxes[boxCoor.X, boxCoor.Y + 1].Focus();
                ++newCoor.Y;
            }
            return newCoor;
        }
    }
}
