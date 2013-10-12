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

namespace SudokuSolver
{
    public class Solution
    {
        // properties
        private uint m_iterationsNeeded;
        private uint m_sudokusGenerated;
        private TimeSpan m_timeNeeded;
        private Sudoku m_fixedBasis;
        private bool m_error;
        private string m_errorString;
        private Sudoku m_bestSolution;

        // help
        private DateTime m_startTime;

        public void Clear()
        {
            m_iterationsNeeded = 0;
            m_sudokusGenerated = 0;
            m_timeNeeded = new TimeSpan();
            m_fixedBasis = new Sudoku();
            m_error = false;
            m_errorString = "";
            m_bestSolution = null;
        }

        public Solution()
        {
            Clear();
        }

        public Solution(Solution other)
        {
            m_iterationsNeeded = other.m_iterationsNeeded;
            m_sudokusGenerated = other.m_sudokusGenerated;
            m_timeNeeded = other.m_timeNeeded;
            m_fixedBasis = other.m_fixedBasis;
            m_error = other.m_error;
            m_errorString = other.m_errorString;
            m_bestSolution = other.m_bestSolution;
        }

        public uint IterationsNeeded
        {
            get { return m_iterationsNeeded; }
            set { m_iterationsNeeded = value; }
        }

        public uint SudokusGenerated
        {
            get { return m_sudokusGenerated; }
            set { m_sudokusGenerated = value; }
        }

        public void Start()
        {
            m_startTime = DateTime.Now;
        }

        public void Stop()
        {
            DateTime stopTime = DateTime.Now;
            m_timeNeeded = stopTime - m_startTime;
        }

        public TimeSpan TimeNeeded
        {
            get { return m_timeNeeded; }
        }

        public Sudoku FixedBasis
        {
            get { return m_fixedBasis; }
            set { m_fixedBasis = value; }
        }

        public bool Error
        {
            get { return m_error; }
            set { m_error = value; }
        }

        public string ErrorString
        {
            get { return m_errorString; }
            set { m_errorString = value; }
        }

        public int BestScore
        {
            get { return m_bestSolution.Score; }
        }

        public Sudoku BestSolution
        {
            get { return m_bestSolution; }
            set { m_bestSolution = value; }
        }

        
    }
}
