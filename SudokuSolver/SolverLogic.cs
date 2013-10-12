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
using System.Collections.Generic;
using System.Text;
using MzTools;

namespace SudokuSolver
{
    class SolverLogic
    {
        //private AStar astar = new AStar();
        private Sudoku fixedBasis = null;
        MinHeap<Sudoku> minHeap = new MinHeap<Sudoku>();

        public void startWith(Sudoku input)
        {
            clear();
            fixedBasis = input;
            Sudoku first = new Sudoku(fixedBasis);
            first.generateRandomSolution();
            minHeap.insertItem(first);
        }

        public void clear()
        {
            minHeap.clear();
            fixedBasis = null;
        }

        public void createSubproblems( Sudoku bestSoFar )
        {
            // for each field
            for( int i = 0; i < Settings.SudokuDimension; i++ )
            for (int j = 0; j < Settings.SudokuDimension; j++)
                // for each another field
                for( int k = i+1; k < Settings.SudokuDimension; k++ )
                for (int l = j+1; l < Settings.SudokuDimension; l++)
                    // if none of the fields are fixed
                    if (fixedBasis.getBox(i, j) == 0 &&
                        fixedBasis.getBox(k, l) == 0)
                        // switch them and create new subproblem sudoku
                        {
                            Sudoku newSubproblem = new Sudoku(bestSoFar);
                            newSubproblem.PathLength = bestSoFar.PathLength + 1;
                            // switch two values
                            short temp = newSubproblem.getBox(i, j);
                            newSubproblem.setBox(i, j, newSubproblem.getBox(k, l));
                            newSubproblem.setBox(k, l, temp);
                            // insert into minheap
                            minHeap.insertItem(newSubproblem);
                        }
        }

        public Sudoku solve()
        {
            while ( (minHeap.getMin().Score - minHeap.getMin().PathLength) != 0)
            {
                int score = (minHeap.getMin().Score - minHeap.getMin().PathLength);
                Sudoku bestSoFar = minHeap.extractMin();
                createSubproblems(bestSoFar);
            }
            return minHeap.getMin();
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
    }
}
