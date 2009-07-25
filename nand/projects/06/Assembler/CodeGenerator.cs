using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler
{
    public static class CodeGenerator
    {
        public static string Dest(string mnemonic)
        {
            string binary = String.Empty;

            switch (mnemonic)
            {
                case "M":
                    {
                        binary = "001";
                        break;
                    }
                case "D":
                    {
                        binary = "010";
                        break;
                    }
                case "MD":
                    {
                        binary = "011";
                        break;
                    }
                case "A":
                    {
                        binary = "100";
                        break;
                    }
                case "AM":
                    {
                        binary = "101";
                        break;
                    }
                case "AD":
                    {
                        binary = "110";
                        break;
                    }
                case "AMD":
                    {
                        binary = "111";
                        break;
                    }
                default:
                    {
                        binary = "000";
                        break;
                    }
            }

            return binary;
        }

        public static string Jump(string mnemonic)
        {
            string binary = string.Empty;

            switch (mnemonic)
            {
                
                case ("JGT"):
                    {
                        binary = "001";
                        break;
                    }
                case ("JEQ"):
                    {
                        binary = "010";
                        break;
                    }
                case ("JGE"):
                    {
                        binary = "011";
                        break;
                    }
                case ("JLT"):
                    {
                        binary = "100";
                        break;
                    }
                case ("JNE"):
                    {
                        binary = "101";
                        break;
                    }
                case ("JLE"):
                    {
                        binary = "110";
                        break;
                    }
                case ("JMP"):
                    {
                        binary = "111";
                        break;
                    }
                default:
                    {
                        binary = "000";
                        break;
                    }
            }

            return binary;
        }

        public static string Comp(string mnemonic)
        {
            string binary = String.Empty; 

            switch (mnemonic)
            {
                case ("0"):
                    {
                        binary = "0101010";
                        break;
                    }
                case ("1"):
                    {
                        binary = "0111111";
                        break;
                    }
                case ("-1"):
                    {
                        binary = "0111010";
                        break;
                    }
                case ("D"):
                    {
                        binary = "0001100";
                        break;
                    }
                case ("A"):
                    {
                        binary = "0110000";
                        break;
                    }
                case ("!D"):
                    {
                        binary = "0001101";
                        break;
                    }
                case ("!A"):
                    {
                        binary = "0110001";
                        break;
                    }
                case ("-D"):
                    {
                        binary = "0001111";
                        break;
                    }
                case ("-A"):
                    {
                        binary = "0110011";
                        break;
                    }
                case ("D+1"):
                    {
                        binary = "0011111";
                        break;
                    }
                case ("A+1"):
                    {
                        binary = "0110111";
                        break;
                    }
                case ("D-1"):
                    {
                        binary = "0001110";
                        break;
                    }
                case ("A-1"):
                    {
                        binary = "0110010";
                        break;
                    }
                case ("D+A"):
                    {
                        binary = "0000010";
                        break;
                    }
                case ("D-A"):
                    {
                        binary = "0010011";
                        break;
                    }
                case ("A-D"):
                    {
                        binary = "0000111";
                        break;
                    }
                case ("D&A"):
                    {
                        binary = "0000000";
                        break;
                    }
                case ("D|A"):
                    {
                        binary = "0010101";
                        break;
                    }
                case ("M"):
                    {
                        binary = "1110000";
                        break;
                    }
                case ("!M"):
                    {
                        binary = "1110001";
                        break;
                    }
                case ("-M"):
                    {
                        binary = "1110011";
                        break;
                    }
                case ("M+1"):
                    {
                        binary = "1110111";
                        break;
                    }
                case ("M-1"):
                    {
                        binary = "1110010";
                        break;
                    }
                case ("D+M"):
                    {
                        binary = "1000010";
                        break;
                    }
                case ("D-M"):
                    {
                        binary = "1010011";
                        break;
                    }
                case ("M-D"):
                    {
                        binary = "1000111";
                        break;
                    }
                case ("D&M"):
                    {
                        binary = "1000000";
                        break;
                    }
                case ("D|M"):
                    {
                        binary = "1010101";
                        break;
                    }
            }
            return binary;
        }
        
    }
}
