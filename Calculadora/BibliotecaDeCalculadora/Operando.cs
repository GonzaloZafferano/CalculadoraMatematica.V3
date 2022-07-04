using System;

namespace BibliotecaDeCalculadora
{
    public class Operando
    {
        private double numero;   

        public double Numero
        {
            get { return this.numero; }
        }

        private string SetNumero
        {
            set { this.numero = Operacion.ValidarNumero(value); }
        }       

        /// <summary>
        /// Constructor de la clase Operando. Genera un operando previa validacion.
        /// </summary>
        /// <param name="cadenaNumerica">Cadena que representa un valor numerico, el cual se asignara al objeto Operando</param>
        public Operando(string cadenaNumerica)
        {
            this.SetNumero = cadenaNumerica;
        }

        #region Sobrecarga de operadores

        public static double operator +(Operando numA, Operando numB)
        {
            double retorno = 0;
            if(numA is not null && numB is not null)
            {
                retorno = numA.Numero + numB.Numero;
            }
            return retorno;
        }

        public static double operator -(Operando numA, Operando numB)
        {
            double retorno = 0;
            if (numA is not null && numB is not null)
            {
                retorno = numA.Numero - numB.Numero;
            }
            return retorno;
        }

        public static double operator *(Operando numA, Operando numB)
        {
            double retorno = 0;
            if (numA is not null && numB is not null)
            {
                retorno = numA.Numero * numB.Numero;
            }
            return retorno;
        }        

        public static double operator /(Operando numA, Operando numB)
        {
            double retorno = double.MinValue;
            if (numA is not null && numB is not null && numB.Numero != 0)
            {
                retorno = numA.Numero / numB.Numero;
            }
            return retorno;
        }

        #endregion
    }
}
