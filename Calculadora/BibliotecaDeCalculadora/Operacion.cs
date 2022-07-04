using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaDeCalculadora
{
    public enum Errores { Error_Matemático, Error_Conversión, Sin_Error };
    public enum Operador { Raíz };
    public class Operacion
    { 
        private static string operador;
        private static bool sePuedeOperar;
        private static bool terminoOperacion;
        private static bool errorMatematico;

        public static bool ErrorMatematico
        {
            get { return Operacion.errorMatematico; }
            set { Operacion.errorMatematico = value; }
        }

        public static bool TerminoOperacion
        {
            get { return Operacion.terminoOperacion; }
            set { Operacion.terminoOperacion = value; }
        }

        public static bool SePuedeOperar
        {
            get { return Operacion.sePuedeOperar; }
            set { Operacion.sePuedeOperar = value; }
        }

        public static string Operador
        {
            get { return Operacion.operador; }
            set
            {
                if (Operacion.EsOperadorValido(value))
                {
                    Operacion.operador = value;
                }
                else
                {
                    Operacion.operador = null;
                }
            }
        }

        static Operacion()
        {
            Operacion.Operador = null;
            Operacion.SePuedeOperar = false;
            Operacion.TerminoOperacion = false;
            Operacion.ErrorMatematico = false;
        }

        /// <summary>
        /// Evalua que un caracter sea ','.
        /// </summary>
        /// <param name="caracter">Caracter a evaluar.</param>
        /// <returns>True, si es ',' (coma). Caso contrario, false.</returns>
        internal static bool EsCaracterComa(char caracter)
        {
            return caracter == ',';
        }

        /// <summary>
        /// Evalua que un caracter sea '-'.
        /// </summary>
        /// <param name="caracter">Caracter a evaluar.</param>
        /// <returns>True, si es '-' (menos). Caso contrario, false.</returns>
        internal static bool EsCaracterMenos(char caracter)
        {
            return caracter == '-';
        }

        /// <summary>
        /// Evalua la cantidad de comas que tiene una cadena.
        /// </summary>
        /// <param name="cadena">Cadena que se evaluara.</param>
        /// <returns>True si tiene 0 o una coma, False si tiene mas de una coma.</returns>
        private static bool TieneCeroOUnaComa(string cadena)
        {
            int cantidadComas = 0;

            for (int i = 0; i < cadena.Length; i++)
            {
                if (Operacion.EsCaracterComa(cadena[i]))
                {
                    cantidadComas++;

                    if (cantidadComas > 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Evalua si la cantidad de digitos de una cadena es valida, para no desbordar una variabla de tipo LONG.
        /// De ser necesario, remueve los ultimos digitos de la cadena, para que no supere el limite.
        /// </summary>
        /// <param name="cadena">Cadena a evaluar.</param>
        /// <returns>Una cadena con una cantidad de digitos valida para una variable de tipo LONG.</returns>
        private static string ObtenerCadenaNumericaConCantidadMaximaPosibleDeDigitosParaDatoLong(string cadena)
        {
            while (cadena.Length > 18)
            {
                cadena = cadena.Remove(cadena.Length - 1);
            }
            return cadena;
        }

        /// <summary>
        /// Evalua la cantidad de digitos de una cadena, y la reduce de ser necesario. Hasta 48 digitos como maximo.
        /// </summary>
        /// <param name="cadena">Cadena a evaluar.</param>
        /// <returns>Una cadena con hasta 48 digitos como maximo.</returns>
        private static string ObtenerCadenaNumericaConLimiteParaMostrar(string cadena)
        {
            while (cadena.Length > 48)
            {
                cadena = cadena.Remove(cadena.Length - 1);
            }
            return cadena;
        }

        /// <summary>
        /// Convierte una cadena a numero, de ser posible.
        /// </summary>
        /// <param name="cadena">Cadena que se evaluara.</param>
        /// <returns>La cadena convertida en Double. En caso de error, retorna 0.</returns>
        internal static double ValidarNumero(string cadena)
        {
            _ = double.TryParse(cadena, out double numeroDecimal);

            return numeroDecimal;
        }
 
        /// <summary>
        /// Evalua si el operador recibido se corresponde a un operador aritmetico valido
        /// </summary>
        /// <param name="operador">Operador a evaluar.</param>
        /// <returns>True, si el operador es valido. Caso contrario, false.</returns>
        private static bool EsOperadorValido(string operador)
        {
            bool retorno = true;

            switch (operador)
            {
                case "+":
                case "-":
                case "*":
                case "/":
                case "Exp.":
                case "Raíz":
                    break;
                default:
                    retorno = false;
                    break;
            }
            return retorno;
        }

        /// <summary>
        /// Crea una cadena para el operando actualmente asignado a la clase.
        /// </summary>
        /// <returns>Cadena con el operando actualmente asignado a la clase.</returns>
        public static string CrearCadenaDeOperador()
        {
            string retorno = string.Empty;

            if (!string.IsNullOrWhiteSpace(Operacion.Operador))
            {
                switch (Operacion.Operador)
                {
                    case "+":
                        retorno = " + ";
                        break;
                    case "-":
                        retorno = " - ";
                        break;
                    case "*":
                        retorno = " * ";
                        break;
                    case "/":
                        retorno = " ÷ ";
                        break;
                    case "Exp.":
                        retorno = " ^ ";
                        break;
                    case "Raíz":
                        retorno = " √ ";
                        break;
                }
            }
            return retorno;
        }

        /// <summary>
        /// Realiza una operacion entre dos operandos
        /// </summary>
        /// <param name="numA">Operando A</param>
        /// <param name="numB">Operando B</param>
        /// <returns>Resultado de la operacion entre dos operandos.</returns>
        public static double Operar(Operando numA, Operando numB)
        {
            double retorno = 0;

            if (numA is not null && numB is not null && !string.IsNullOrWhiteSpace(Operacion.Operador))
            {
                switch (Operacion.Operador)
                {
                    case "+":
                        retorno = numA + numB;
                        break;
                    case "-":
                        retorno = numA - numB;
                        break;
                    case "*":
                        retorno = numA * numB;
                        break;
                    case "/":
                        retorno = numA / numB;
                        break;
                    case "Exp.":
                        retorno = Math.Pow(numA.Numero, numB.Numero);
                        break;
                    case "Raíz":
                        retorno = Math.Pow(numA.Numero, 1 / numB.Numero);
                        break;
                }
            }
            return retorno;
        }

        /// <summary>
        /// Elimina los ceros no validos a la izquiera de una cadena numerica.
        /// </summary>
        /// <param name="cadena">Cadena a evaluar.</param>
        /// <returns>Cadena con los 0 a la izquierda removidos. 
        /// Si la cadena recibida es nula o esta vacia, se retorna la misma cadena sin modificacion.</returns>
        public static string BorrarCerosALaIzquierda(string cadena)
        {
            if (!string.IsNullOrWhiteSpace(cadena))
            {
                while (cadena.Length > 1 && cadena[0] == '0' && !Operacion.EsCaracterComa(cadena[1]))
                {
                    cadena = cadena.Remove(0, 1);
                }
            }
            return cadena;
        }

        /// <summary>
        /// Elimina el ultimo caracter de una cadena.
        /// </summary>
        /// <param name="cadena">Cadena a evaluar</param>
        /// <returns>Cadena con el ultimo caracter borrado. Al eliminar completamente la cadena, se retornara '0'.
        /// Si la cadena recibida por parametro es nula o esta vacia, se retorna la misma cadena sin modificacion.</returns>
        public static string BorrarUnDigito(string cadena)
        {
            if (!string.IsNullOrWhiteSpace(cadena))
            {
                cadena = cadena.Remove(cadena.Length - 1);

                if (cadena.Length == 0 || (cadena.Length == 1 && Operacion.EsCaracterMenos(cadena[0])))
                {
                    cadena = "0";
                }
            }
            return cadena;
        }

        /// <summary>
        /// Le da un formato numerico correcto a una cadena.
        /// </summary>
        /// <param name="cadena">Cadena a evaluar</param>
        /// <returns>Una cadena con el formato correspondiente a un numero (con separacion de miles, y coma para decimales).
        /// Si la cadena es nula o esta vacia, la retorna sin cambios.</returns>
        public static string ObtenerFormato(string cadena)
        {
            string parteEnteraString;
            string parteDecimalString;
            string cadenaAuxiliar;

            if (!string.IsNullOrWhiteSpace(cadena))
            {
                cadenaAuxiliar = cadena.RemoverTodosLosPuntosDelString();

                while (!Operacion.TieneCeroOUnaComa(cadenaAuxiliar))
                {
                    cadenaAuxiliar = cadenaAuxiliar.Remove(cadenaAuxiliar.Length - 1);
                }

                if (cadenaAuxiliar.EsCadenaNumericaDeEnteroODecimal())
                {
                    int indiceDeLaComa = cadenaAuxiliar.IndexOf(",");

                    indiceDeLaComa = indiceDeLaComa <= 0 ? 0 : indiceDeLaComa;

                    parteEnteraString = cadenaAuxiliar.Substring(0, indiceDeLaComa == 0 ? cadenaAuxiliar.Length : indiceDeLaComa);

                    parteEnteraString = Operacion.ObtenerCadenaNumericaConCantidadMaximaPosibleDeDigitosParaDatoLong(parteEnteraString);

                    parteDecimalString = indiceDeLaComa != 0 ? cadenaAuxiliar.Substring(indiceDeLaComa) : string.Empty;

                    _ = long.TryParse(parteEnteraString, out long parteEnteraLong);

                    cadena = string.Format("{0:N0}{1}", parteEnteraLong, parteDecimalString);

                    cadena = Operacion.ObtenerCadenaNumericaConLimiteParaMostrar(cadena);
                }
            }
            return cadena;
        }
    }
}
