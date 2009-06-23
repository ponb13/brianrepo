using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Assembler
{
    public class Parser: IDisposable
    {
        private StreamReader streamReader;
        

        public string CurrentCommand
        {
            get;
            set;
        }
        
        public Parser(string filePath)
        {
            streamReader = new StreamReader(filePath);
        }

        public bool HasMoreCommands()
        {
            return !streamReader.EndOfStream;
        }

        public void Advance()
        {
            if (this.HasMoreCommands())
            {
                this.CurrentCommand = streamReader.ReadLine();
                
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (streamReader != null)
            {
                streamReader.Close();
            }
        }

        #endregion
    }
}
