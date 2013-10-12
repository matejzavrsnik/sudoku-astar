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
using MzTools;
using System.ComponentModel;

namespace SudokuSolver
{
    public class BusinessLogic
    {
        private Sudoku m_fixedBasis;
        private MinHeap<Sudoku> minHeap;
        protected Solution m_solution;
        protected BackgroundWorker m_worker;
        public event EventHandler ResultAvailable;

        public BusinessLogic()
        {
            m_fixedBasis = null;
            minHeap = new MinHeap<Sudoku>();
            m_solution = new Solution();
            m_worker = new BackgroundWorker();
            m_worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            m_worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        public void startWith(Sudoku input)
        {
            clear();
            m_fixedBasis = input;
            Sudoku first = new Sudoku(m_fixedBasis);
            first.generateRandomSolution();
            minHeap.insertItem(first);
        }

        public void clear()
        {
            minHeap.clear();
            m_solution.Clear();
            m_fixedBasis = null;
        }

        public void solve()
        {
            m_worker.RunWorkerAsync();
        }

        protected void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Solution solution = new Solution();
            solution.FixedBasis = m_fixedBasis;
            solution.BestSolution = minHeap.getMin();
            solution.Start();
            int score = minHeap.getMin().Score;
            while (score != 0)
            {
                solution.IterationsNeeded += 1;
                //Sudoku bestThisRound = minHeap.extractMin();
                solution.BestSolution = minHeap.extractMin();
                // try to create subproblems. might throw out of memory
                try
                {
                    // for each field
                    for( int i = 0; i < Settings.SudokuDimension; i++ )
                    for (int j = 0; j < Settings.SudokuDimension; j++)
                        // for each another field
                        for( int k = i; k < Settings.SudokuDimension; k++ )
                        for (int l = j; l < Settings.SudokuDimension; l++)
                            // if none of the fields are fixed
                            if (((k!=i) || (l!=j)) &&
                                m_fixedBasis.getBox(i, j) == 0 &&
                                m_fixedBasis.getBox(k, l) == 0)
                                // switch them and create new subproblem sudoku
                                {

                                    Sudoku newSubproblem = new Sudoku(solution.BestSolution); // if out of memory, it will most likely throw here
                                    solution.SudokusGenerated += 1;
                                    newSubproblem.PathLength = solution.BestSolution.PathLength + 1;
                                    // switch two values
                                    byte temp = newSubproblem.getBox(i, j);
                                    newSubproblem.setBox(i, j, newSubproblem.getBox(k, l));
                                    newSubproblem.setBox(k, l, temp);
                                    // insert into minheap
                                    minHeap.insertItem(newSubproblem);
                                }
                    score = (minHeap.getMin().Score - minHeap.getMin().PathLength);
                    if (score < (solution.BestSolution.Score - solution.BestSolution.PathLength))
                    {
                        solution.BestSolution = minHeap.getMin();
                    }
                }
                catch
                {
                    // report puzzle unsolvable or out of memory
                    solution.Error = true;
                    break;
                }
            }
            solution.Stop();
            //solution.BestSolution = minHeap.extractMin();
            minHeap.clear();
            e.Result = (object)solution;
        }

        protected void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.m_solution = (Solution)e.Result;
            EventHandler handler = ResultAvailable;
            handler(this, EventArgs.Empty);
        }

        public Sudoku openFile( string filename )
        {
            Sudoku fromFile = new Sudoku();
            FileOpener opener = new FileOpener(filename);
            fromFile = opener.getSudoku();
            return fromFile;
        }

        public void saveFile(string filename, Sudoku puzzle)
        {
            FileSaver saver = new FileSaver(filename);
            saver.saveSudoku(puzzle);
        }

        public Solution Statistics
        {
            get { return m_solution; }
        }
    }
}
