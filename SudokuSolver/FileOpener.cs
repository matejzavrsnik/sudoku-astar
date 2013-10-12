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
    public class SudokuParser
    {
        protected string m_fileContent = "";

        public SudokuParser(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            m_fileContent = sr.ReadToEnd();
            sr.Close();
        }

        public virtual Sudoku Parse()
        {
            throw new NotImplementedException();
        }
    }

    public class SudokuParserTxt : SudokuParser
    {
        public SudokuParserTxt(string filename)
            : base(filename)
        {
        }

        public override Sudoku Parse()
        {
            // Basically 81 characters (. and 1-9), 
            // multiple puzzles separated by CRLF, not handled
            Sudoku sudoku = new Sudoku();
            char[] chars = m_fileContent.ToCharArray();
            for(int k=0; k<chars.Length; k++)
            {
                int i = k % Settings.SudokuDimension;
                int j = k / Settings.SudokuDimension;
                byte val = 0;
                try
                {
                    byte.TryParse(chars[k].ToString(), out val);
                }
                catch (System.ArgumentException) { }
                sudoku.setBox(i, j, val);
            }
            return sudoku;
        }
    }

    public class SudokuParserMskSol : SudokuParser
    {
        public SudokuParserMskSol(string filename)
            : base(filename)
        {
        }

        public override Sudoku Parse()
        {
            // Each row in its own line, separated by CRLF. 
            Sudoku sudoku = new Sudoku();
            m_fileContent = m_fileContent.Replace("\r\n", "");
            char[] chars = m_fileContent.ToCharArray();
            for (int k = 0; k < chars.Length; k++)
            {
                int i = k % Settings.SudokuDimension;
                int j = k / Settings.SudokuDimension;
                byte val = 0;
                try
                {
                    byte.TryParse(chars[k].ToString(), out val);
                }
                catch (System.ArgumentException) { }
                sudoku.setBox(i, j, val);
            }
            return sudoku;
        }
    }

    public class FileOpener
    {
        private SudokuParser m_parser;

        public FileOpener(string filename)
        {
            if (filename.EndsWith(".txt"))
                m_parser = new SudokuParserTxt(filename);
            else if( filename.EndsWith(".msk") || filename.EndsWith(".sol"))
                m_parser = new SudokuParserMskSol(filename);
        }

        public Sudoku getSudoku()
        {
            Sudoku parsed = new Sudoku();
            parsed = m_parser.Parse();
            return parsed;
        }
    }
}
