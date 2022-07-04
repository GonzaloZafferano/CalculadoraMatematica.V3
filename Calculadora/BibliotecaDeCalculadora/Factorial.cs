using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaDeCalculadora
{
    public static class Factorial
    {
        /// <summary>
        /// Obtiene el Factorial de un numero Entero Positivo.
        /// </summary>
        /// <param name="numero">Cadena numerica que se evaluara.</param>
        /// <returns>Factorial del numero entero positivo recibido, caso contrario, retorna el double.MinValue.</returns>
        public static double FactorialDeUnNumero(string numero)
        {
            double retorno = double.MinValue;

            if(!string.IsNullOrWhiteSpace(numero) && numero.EsCadenaNumericaDeEntero() && 
                double.TryParse(numero, out double numeroParseado) && numeroParseado >= 0)
            {
                if(numeroParseado < 2)
                {
                    retorno = 1;
                }
                else
                {
                    retorno = Factorial.CalcularFactorial((int)numeroParseado);
                }
            }
            return retorno;
        }

        /// <summary>
        /// Calcula el factorial de un numero Entero.
        /// </summary>
        /// <param name="numero">Numero del cual se calculara el Factorial.</param>
        /// <returns>Factorial del numero recibido.</returns>
        private static double CalcularFactorial(int numero)
        {
            double retorno = numero;

            for(int i = numero - 1; i > 0 ;i--)
            {
                retorno *= i;
            }
            return retorno;
        }       
    }
}
