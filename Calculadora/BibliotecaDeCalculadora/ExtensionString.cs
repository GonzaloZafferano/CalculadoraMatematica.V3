using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BibliotecaDeCalculadora
{
    public static class ExtensionString
    {
        /// <summary>
        /// Borra todos los puntos de una cadena.
        /// </summary>
        /// <param name="cadena">Cadena de la cual se borraran todos los puntos.</param>
        /// <returns>Una cadena sin puntos.</returns>
        public static string RemoverTodosLosPuntosDelString(this string cadena)
        {
            return cadena.Replace(".", "");
        }

        /// <summary>
        /// Evalua si una cadena es valida, acorde a la expresion recibida por parametro.
        /// </summary>
        /// <param name="cadena">Cadena que se evaluara.</param>
        /// <param name="expresion">Expresion cuyo cumplimiento se evaluara.</param>
        /// <returns>True si la cadena cumple con la expresion, caso contrario false.</returns>
        private static bool EsCadenaValida(this string cadena, string expresion)
        {
            if(!string.IsNullOrWhiteSpace(expresion))
            {
                Regex expresionRegular = new Regex(expresion);

                return expresionRegular.IsMatch(cadena);
            }
            return false;
        }

        /// <summary>
        /// Evalua si una cadena es numerica (Entero o Decimal). 
        /// La cadena decimal puede terminar con un numero o con coma.
        /// </summary>
        /// <param name="cadena">Cadena a evaluar.</param>
        /// <returns>True si la cadena es un numero entero o decimal, caso contrario false.</returns>
        public static bool EsCadenaNumericaDeEnteroODecimal(this string cadena)
        {
            string cadenaAuxiliar = cadena.RemoverTodosLosPuntosDelString();

            return cadenaAuxiliar.EsCadenaValida("^-?[0-9]+(,?([0-9]+)?)$");
        }

        /// <summary>
        /// Evalua si una cadena es numerica (Entero).
        /// </summary>
        /// <param name="cadena">Cadena a evaluar.</param>
        /// <returns>True si la cadena es un numero entero, caso contrario false.</returns>
        public static bool EsCadenaNumericaDeEntero(this string cadena)
        {
            string cadenaAuxiliar = cadena.RemoverTodosLosPuntosDelString();

            return cadenaAuxiliar.EsCadenaValida("^-?[0-9]+$");
        }

        /// <summary>
        /// Evalua si una cadena es un numero binario (1 y 0).
        /// </summary>
        /// <param name="cadena">Cadena a evaluar.</param>
        /// <returns>True si la cadena es un numero binario, caso contrario false.</returns>
        public static bool EsCadenaDeNumeroBinario(this string cadena)
        {
            return cadena.EsCadenaValida("^-?([10]+,)?[10]+?$");
        }
    }
}
