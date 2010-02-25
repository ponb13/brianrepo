using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using System.IO;
using System.Text.RegularExpressions;

namespace States
{
    public static class Extensions
    {
        /// <summary>
        /// Creates the token object.
        /// </summary>
        /// <param name="previousState">previous state i.e. the state before token complete is the token name!</param>
        /// <returns></returns>
        public static Pair<string, string> CreateTokenObject(this IState state)
        {
            string stateName = state.ToString().Split('.').Last();

            return new Pair<string, string>
            {
                Value1 = stateName,
                Value2 = state.TokenCharacters.ToString()
            };
        }

    }
}
