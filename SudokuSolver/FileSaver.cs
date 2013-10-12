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
using System.IO;

namespace SudokuSolver
{
    public class SudokuSaver
    {
        protected string m_filename = "";

        public SudokuSaver(string filename)
        {
            m_filename = filename;
        }

        public virtual void Save(Sudoku puzzle)
        {
            throw new NotImplementedException();
        }
    }

    public class SudokuSaverTxt : SudokuSaver
    {
        public SudokuSaverTxt(string filename)
            : base(filename)
        {
        }

        public override void Save(Sudoku puzzle)
        {
            // Basically 81 characters (. and 1-9), 
            // multiple puzzles separated by CRLF, not handled
            StreamWriter sw = new StreamWriter(m_filename);
            for( int j=0; j<Settings.SudokuDimension; j++ )
                for( int i = 0; i < Settings.SudokuDimension; i++)
                {
                    string number = ".";
                    if (puzzle.getBox(i, j) != 0)
                        number = puzzle.getBox(i, j).ToString();
                    sw.Write(number);
                }
            sw.Flush();
            sw.Close();
        }
    }

    public class SudokuSaverMskSol : SudokuSaver
    {
        public SudokuSaverMskSol(string filename)
            : base(filename)
        {
        }

        public override void Save(Sudoku puzzle)
        {
            // Each row in its own line, separated by CRLF. 
            StreamWriter sw = new StreamWriter(m_filename);
            for (int j = 0; j < Settings.SudokuDimension; j++)
            {
                for (int i = 0; i < Settings.SudokuDimension; i++)
                {
                    string number = ".";
                    if( puzzle.getBox(i, j) != 0)
                        number = puzzle.getBox(i, j).ToString();
                    sw.Write(number);
                }
                sw.Write("\r\n");
            }
            sw.Flush();
            sw.Close();
        }
    }

    class FileSaver
    {
        private SudokuSaver m_saver = null;

        public FileSaver(string filename)
        {
            if (filename.EndsWith(".txt"))
                m_saver = new SudokuSaverTxt(filename);
            else if( filename.EndsWith(".msk") || filename.EndsWith(".sol"))
                m_saver = new SudokuSaverMskSol(filename);
        }

        public void saveSudoku( Sudoku puzzle )
        {
            m_saver.Save(puzzle);
        }
    }
}
