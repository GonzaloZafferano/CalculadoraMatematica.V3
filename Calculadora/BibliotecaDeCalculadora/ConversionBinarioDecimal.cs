using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaDeCalculadora
{
    public static class ConversionBinarioDecimal
    {
        private static bool esNumeroBinario;

        public static bool EsNumeroBinario
        {
            get { return ConversionBinarioDecimal.esNumeroBinario; }
            set { ConversionBinarioDecimal.esNumeroBinario = value; }        
        }

        static ConversionBinarioDecimal()
        {
            ConversionBinarioDecimal.EsNumeroBinario = false;
        }

        /// <summary>
        /// Evalua si una cadena tiene coma, y en caso de encontrarla retorna el indice de la misma.
        /// </summary>
        /// <param name="cadena">Cadena a evaluar.</param>
        /// <param name="indiceDeLaComa">Variable donde se almacenara el indice de la coma, en caso de encontrarlo. Caso contrario almacenara 0.</param>
        /// <returns>True, si la cadena tiene coma. Caso contrario false.</returns>
        private static bool CadenaTieneComa(string cadena, out int indiceDeLaComa)
        {
            bool retorno = false;
            indiceDeLaComa = 0;

            if (!string.IsNullOrWhiteSpace(cadena))
            {
                for (int i = 0; i < cadena.Length; i++)
                {
                    if (Operacion.EsCaracterComa(cadena[i]))
                    {
                        retorno = true;
                        indiceDeLaComa = i;
                        break;
                    }
                }
            }
            return retorno;
        }      

        /// <summary>
        /// Obtiene un numero binario a partir de un numero entero positivo.
        /// </summary>
        /// <param name="cadena">Cadena que se evaluara</param>
        /// <returns>El numero entero positivo convertido en numero binario.</returns>
        private static string ObtenerBinarioDeUnNumeroEntero(string cadena)
        {
            StringBuilder cadenaBinaria = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(cadena) && int.TryParse(cadena, out int numeroEntero))
            {
                do
                {
                    cadenaBinaria.Append(numeroEntero % 2);
                } while ((numeroEntero /= 2) > 1);

                cadenaBinaria.Append(numeroEntero);

                while (cadenaBinaria.Length % 4 != 0)
                {
                    cadenaBinaria.Append(0);
                }
            }
            return new string(cadenaBinaria.ToString().Reverse().ToArray());
        }

        /// <summary>
        /// Convierte un numero fraccionario positivo en fraccion binaria.
        /// </summary>
        /// <param name="cadena">cadena a evaluar</param>
        /// <returns>Numero binario fraccionario.</returns>
        private static double CalcularDecimalesBinarios(string cadena)
        {
            StringBuilder cadenaDecimales = new StringBuilder();
            double retorno = 0;

            if (!string.IsNullOrWhiteSpace(cadena) && double.TryParse(cadena, out double decimales))
            {
                cadenaDecimales.Append("0,");

                for (int i = 0; i < 8 && decimales != 1; i++)
                {
                    decimales *= 2;

                    cadenaDecimales.Append((int)decimales);

                    decimales -= (int)decimales;
                }

                _ = double.TryParse(cadenaDecimales.ToString(), out retorno);
            }
            return retorno;
        }

        /// <summary>
        /// Convierte un numero decimal (recibido en formato string) en Binario.
        /// </summary>
        /// <param name="cadenaNumeroDecimal">Cadena que representa un numero decimal.</param>
        /// <returns>Cadena con el numero Binario, o "Valor invalido" en caso de no poder realizar la conversion.</returns>
        public static string DecimalBinario(string cadenaNumeroDecimal)
        {
            StringBuilder cadenaNumericaDecimalesSB = new StringBuilder();
            string retorno = Errores.Error_Conversión.ToString();
            string cadenaNumericaDecimales;
            string signo = string.Empty;
            double decimalesBinarios = 0;
            string binarioDeUnNumeroEntero;
            string cadenaNumericaEnteros; 

            if (!string.IsNullOrWhiteSpace(cadenaNumeroDecimal) && !ConversionBinarioDecimal.EsNumeroMuyGrande(cadenaNumeroDecimal))
            {
                cadenaNumeroDecimal = cadenaNumeroDecimal.RemoverTodosLosPuntosDelString();

                if (Operacion.EsCaracterMenos(cadenaNumeroDecimal[0]))
                {
                    cadenaNumeroDecimal = cadenaNumeroDecimal.Remove(0, 1);
                    signo = "-";
                }

                if (ConversionBinarioDecimal.CadenaTieneComa(cadenaNumeroDecimal, out int indice))
                {
                    cadenaNumericaEnteros = cadenaNumeroDecimal.Substring(0, indice);

                    //cadenaNumericaDecimales = cadenaNumeroDecimal.Substring(indice).Insert(0, "0");

                    cadenaNumericaDecimales = cadenaNumeroDecimal[indice..].Insert(0, "0");

                    decimalesBinarios = ConversionBinarioDecimal.CalcularDecimalesBinarios(cadenaNumericaDecimales);                    
                }
                else
                {
                    cadenaNumericaEnteros = cadenaNumeroDecimal;
                }

                binarioDeUnNumeroEntero = ConversionBinarioDecimal.ObtenerBinarioDeUnNumeroEntero(cadenaNumericaEnteros);
             
                if (binarioDeUnNumeroEntero.EsCadenaDeNumeroBinario() && decimalesBinarios.ToString().EsCadenaDeNumeroBinario())
                {                           
                    if(decimalesBinarios != 0)
                    {
                        cadenaNumericaDecimalesSB.Append(decimalesBinarios.ToString().Remove(0, 1));

                        //Quito la coma del Lenght.
                        while ((cadenaNumericaDecimalesSB.Length -1) % 4 != 0)
                        {
                            cadenaNumericaDecimalesSB.Append('0');
                        }
                    }
                    retorno = binarioDeUnNumeroEntero.Insert(0, signo);
                    retorno += cadenaNumericaDecimalesSB.ToString();
                }                
            }      
            return retorno;
        }

        /// <summary>
        /// Evalua si un numero es demasiado grande, porque contiene la notacion 'E'
        /// </summary>
        /// <param name="cadena"></param>
        /// <returns></returns>
        private static bool EsNumeroMuyGrande(string cadena)
        {
            bool retorno = false;
            if(!string.IsNullOrWhiteSpace(cadena))
            {
                for(int i = 0; i < cadena.Length; i++)
                {
                    if(cadena[i] == 'E')
                    {
                        retorno = true;
                        break;
                    }
                }
            }
            return retorno;
        }

        /// <summary>
        /// Obtiene un numero positivo a partir de un numero Binario.
        /// </summary>
        /// <param name="cadena">Cadena en formato binario.</param>
        /// <returns>Un numero positivo decimal</returns>
        private static double ObtenerEnteroDeUnBinario(string cadena)
        {
            double acumulador = 0;

            if(!string.IsNullOrWhiteSpace(cadena))
            {
                for (int i = 0; i < cadena.Length; i++)
                {
                    if (cadena[i] != '0')
                    {
                        acumulador += Math.Pow(2, i);
                    }
                }
            }
            return acumulador;
        }

        /// <summary>
        /// Convierte la parte fraccionaria de un numero binario en Decimal.
        /// </summary>
        /// <param name="cadena">Cadena a evaluar</param>
        /// <returns>Un numero fraccionario positivo.</returns>
        private static double CalcularBinariosDecimales(string cadena)
        {
            double retorno = 0;

            if (!string.IsNullOrWhiteSpace(cadena) && cadena.EsCadenaDeNumeroBinario())
            {
                for (int i = 2; i < cadena.Length; i++)
                {
                    if (cadena[i] != '0')
                    {
                        retorno += Math.Pow(2, (i - 1) * -1);
                    }
                }
            }
            return retorno;
        }

        /// <summary>
        /// Convierte un numero Binario (recibido en formato string) en numero Decimal.
        /// </summary>
        /// <param name="cadena">Cadena que representa un numero Binario.</param>
        /// <returns>Cadena con el numero Decimal ya convertido, o "Valor invalido" en caso de no poder realizar la conversion.</returns>
        public static string BinarioDecimal(string cadena)
        {
            string retorno = Errores.Error_Conversión.ToString();
            string signo = string.Empty;
            double binariosDecimales = 0;
            double acumulador = 0;
            string cadenaNumericaEnteros;
            string cadenaNumericaDecimales;

            if (!string.IsNullOrWhiteSpace(cadena) && cadena.EsCadenaDeNumeroBinario())
            {
                if (Operacion.EsCaracterMenos(cadena[0]))
                {
                    cadena = cadena.Remove(0, 1);
                    signo = "-";
                }

                if (ConversionBinarioDecimal.CadenaTieneComa(cadena, out int indice))
                {
                    cadenaNumericaEnteros = cadena.Substring(0, indice);
                    cadenaNumericaEnteros = new string(cadenaNumericaEnteros.Reverse().ToArray());

                    //cadenaNumericaDecimales = cadenaAuxiliar.Substring(indice).Insert(0, "0");

                    cadenaNumericaDecimales = cadena[indice..].Insert(0, "0");

                    binariosDecimales = ConversionBinarioDecimal.CalcularBinariosDecimales(cadenaNumericaDecimales);
                }
                else
                {
                    cadenaNumericaEnteros = new string(cadena.Reverse().ToArray());
                }

                acumulador = ConversionBinarioDecimal.ObtenerEnteroDeUnBinario(cadenaNumericaEnteros);

                acumulador += binariosDecimales;

                retorno = acumulador.ToString().Insert(0, signo);                
            }
            return retorno;
        }
    }
}
