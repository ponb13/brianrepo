using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembler
{
    public class DecimalToBinaryConverter
    {
        private static int[] binaryValues = new int[15]
            {
                16384,
                8192,
                4096,
                2048,
                1024,
                512,
                256,
                128,
                64,
                32,
                16,
                8,
                4,
                2,
                1,
            };

        public static int[] Convert(int valueToConvert)
        {
            int[] binaryRep = new int[15];

            for (int i = 0; i <= 14; i++)
            {
                if ((valueToConvert - binaryValues[i]) > 0)
                {
                    binaryRep[i] = 1;
                    valueToConvert = valueToConvert - binaryValues[i];
                }
                else if (valueToConvert - binaryValues[i] == 0)
                {
                    binaryRep[i] = 1;
                    valueToConvert = valueToConvert - binaryValues[i];
                    // this is the last value to set becuase we are at zero so break out of loop
                    break; 
                }
            }

            return binaryRep;
        }

        public static string GetStringRep(int valToConvert)
        {
            int[] binaryRep = DecimalToBinaryConverter.Convert(valToConvert);
            string[] binaryRepString = new string[15];

            for (int i = binaryRepString.Length - 1; i >= 0; i--)
            {
                binaryRepString[i] = binaryRep[i].ToString();
            }

            return String.Join("",binaryRepString);
        }
    }
}
