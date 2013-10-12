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
using MzTools;

namespace SudokuSolver
{
    public class Sudoku : IComparable<Sudoku>
    {
        protected byte[,] m_boxes = new byte[Settings.SudokuDimension, Settings.SudokuDimension];
        private int m_score;
        private int m_pathLength;
        private bool m_scoreCalculated;

        public Sudoku()
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    m_boxes[i, j] = 0;
            m_score = -1;
            //m_pathLength = 0;
            m_scoreCalculated = false;
        }

        public Sudoku(Sudoku basedOn)
        {
            copyBoxesFromAnother(basedOn);
            m_score = -1;
            m_pathLength = 0;
            m_scoreCalculated = false;
        }

        public int Score
        {
            get 
            {
                if (m_scoreCalculated == false) // not yet calculated
                    calculateScore();
                return m_score; 
            }
        }

        public int PathLength
        {
            get { return m_pathLength; }
            set { m_pathLength = value; }
        }

        public void setBox(int i, int j, byte value)
        {
            if (i >= Settings.SudokuDimension || j >= Settings.SudokuDimension || i < 0 || j < 0)
                throw new System.InvalidOperationException("Invalid indices.");
            m_boxes[i, j] = value;
            m_scoreCalculated = false; // any score is invalid now
        }

        public byte getBox(int i, int j)
        {
            if (i >= Settings.SudokuDimension || j >= Settings.SudokuDimension || i < 0 || j < 0)
                throw new System.InvalidOperationException("Invalid indices.");
            return m_boxes[i, j];
        }

        public void copyBoxesFromAnother(Sudoku fromSudoku)
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    m_boxes[i, j] = fromSudoku.m_boxes[i, j];
            m_scoreCalculated = false; // any score is invalid now
        }

        public void generateRandomSolution()
        {
            // all possible numbers instances
            List<byte> numbers = new List<byte>((int)Math.Pow(Settings.SudokuDimension, 2));
            for (byte i = 1; i <= 9; i++)
                for (byte j = 1; j <= 9; j++)
                {
                    numbers.Add(j);
                }
            // remove numbers, that are fixed in basedOn sudoku
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (m_boxes[i, j] != 0)
                    {
                        numbers.Remove(m_boxes[i, j]);
                    }
            // the rest of numbers fill have to fit into solution somehow.
            // shuffle them in-place, then fill sudoku with them in order.
            FisherYates<byte>.Shuffle(ref numbers);
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (m_boxes[i, j] == 0)
                    {
                        m_boxes[i, j] = numbers[0];
                        numbers.RemoveAt(0);
                    }
            m_scoreCalculated = false; // invalidate score
            // done
        }

        private int getFieldScore(int i, int j)
        {
            int fieldScore = 0;
            for( int k = 0; k < 9; k++ )
            {
                if( ( k != i && m_boxes[k,j] == m_boxes[i,j] ) ||
                    ( k != j && m_boxes[i,k] == m_boxes[i,j] ) )
                    ++fieldScore;
            }
            int sq_dim = Settings.SudokuDimension / 3;
            int row = i / sq_dim;
            int col = j / sq_dim;
            int row_from, row_to, col_from, col_to;

            row_from = row*sq_dim;
            row_to = row_from + sq_dim - 1;
            col_from = col*sq_dim;
            col_to = col_from + sq_dim - 1;
            for( int k = row_from; k <= row_to; k++ )
                for ( int l = col_from; l <= col_to; l++ )
                    if( k != i && l != j && m_boxes[k, l] == m_boxes[i, j])
                        ++fieldScore;
            return fieldScore;
        }

        private void calculateScore()
        {
            // score consists of the sum of how wrong a number on each field is.
            // "Wrongness" is calculated by how many times the number on the field
            // appears in each line, column, square
            m_score = m_pathLength;
            for( int i = 0; i < 9; i++ )
                for (int j = 0; j < 9; j++)
                {
                    m_score += getFieldScore(i, j);
                }
            m_scoreCalculated = true;
        }

        public int CompareTo(Sudoku other)
        {
            if (other == null || other == this)
                throw new System.InvalidOperationException("Can't compare to null.");

            return (this.Score - other.Score);
        }
    }
}
