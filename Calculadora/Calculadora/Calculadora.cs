using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BibliotecaDeCalculadora;
using BibliotecaDeColores;

namespace Calculadora
{
    public partial class Calculadora : Form
    {
        private Operando operandoUno;
        private Operando operandoDos;
        CancellationTokenSource tokenDeCancelacion;
        CancellationToken token;
        Task tarea;
        Color color;

        public Calculadora()
        {
            this.InitializeComponent();
        }

        #region Logica

        private CancellationToken ObtenerToken
        {
            get
            {
                if(this.tokenDeCancelacion == null)
                {
                    this.tokenDeCancelacion = new CancellationTokenSource();
                }

                return this.tokenDeCancelacion.Token;
            }
        }

        private string LblResultado
        {
            get { return this.lblResultado.Text; }

            set
            {
                if (double.TryParse(value, out double resultado))
                {
                    if (resultado == double.MinValue || double.IsNaN(resultado))
                    {
                        this.lblResultado.Text = Errores.Error_Matemático.ToString();
                        Operacion.ErrorMatematico = true;
                    }
                    else
                    {
                        this.lblResultado.Text = Operacion.ObtenerFormato(Math.Round(resultado, 10).ToString());
                    }
                }
                else if (value == Errores.Error_Conversión.ToString())
                {
                    this.lblResultado.Text = value;
                    Operacion.ErrorMatematico = true;
                }
            }
        }

        private string LblResultadoABinario
        {
            set
            {       
                if(!string.IsNullOrWhiteSpace(value))
                {
                    Operacion.ErrorMatematico = !value.EsCadenaDeNumeroBinario();
                    this.lblResultado.Text = value;
                }
            }
        }

        private string LblResultadoAgregarUnDigito
        {
            set
            {
                this.lblResultado.Text = Operacion.ObtenerFormato(this.LblResultado + value);
            }
        }

        private string LblResultadoBorrarUnDigito
        {
            set
            {
                this.lblResultado.Text = Operacion.ObtenerFormato(value);
            }
        }

        private string LblOperacion
        {
            get
            {
                return this.lblOperacion.Text;
            }
            set
            { 
                this.lblOperacion.Text = value;                
            }
        }

        /// <summary>
        /// Evento que se dispara al cargar el Formulario.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Objeto que contiene informacion del evento.</param>
        private void Calculadora_Load(object sender, EventArgs e)
        {
            this.lstOperaciones.Width = 404;
            this.btnLimpiarHistorial.Location = new Point(this.lstOperaciones.Location.X + this.lstOperaciones.Width - this.btnLimpiarHistorial.Width, this.btnLimpiarHistorial.Location.Y);
            _ = Enum.TryParse(Properties.Settings.Default.tema, out Tema.Temas tema);
            this.CambiarTema(tema);

            this.Limpiar();
            this.tsBtnHistorial.Tag = false;
        }

        /// <summary>
        /// Evento que se dispara al presionar el boton btnLimpiar. Este evento limpia la informacion cargada en el formulario.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Objeto que contiene informacion del evento.</param>
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.Limpiar();
        }

        /// <summary>
        /// Realiza la limpieza de datos del formulario, reseteando los valores de operandos y operaciones..
        /// </summary>
        private void Limpiar()
        {
            this.operandoUno = null;
            this.operandoDos = null;
            this.LblResultado = "0";
            this.LblOperacion = "0";
            this.btnBorrarUnDigito.Tag = true;
            this.btnNegativo.Tag = false;
            this.btnConvertirABinario.Tag = true;
            this.btnConvertirADecimal.Tag = false;
            Operacion.Operador = null;
            Operacion.TerminoOperacion = false;
            Operacion.ErrorMatematico = false;
            ConversionBinarioDecimal.EsNumeroBinario = false;   
            Operacion.SePuedeOperar = false;
        }

        /// <summary>
        /// Evento que se dispara al presionar el boton btnBorrarUnDigito. Este evento elimina el ultimo digito de una cadena.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Objeto que contiene informacion del evento.</param>
        private void btnBorrarUnDigito_Click(object sender, EventArgs e)
        {
            if ((bool)this.btnBorrarUnDigito.Tag)
            {
                this.LblResultadoBorrarUnDigito = Operacion.BorrarUnDigito(this.LblResultado);
            }
        }

        /// <summary>
        /// Evento que se dispara al presionar el boton btnNumero. Este evento carga un digito en el operando.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Objeto que contiene informacion del evento.</param> 
        private void btnNumero_Click(object sender, EventArgs e)
        {
            if (Operacion.TerminoOperacion || Operacion.ErrorMatematico || 
                ConversionBinarioDecimal.EsNumeroBinario)
            {
                this.Limpiar();
            }

            btnNegativo.Tag = true;

            if (this.operandoUno is not null && !Operacion.SePuedeOperar)
            {
                this.LblResultado = "0";
                Operacion.SePuedeOperar = true;
            }
            this.LblResultadoAgregarUnDigito = ((Button)sender).Text;
            this.btnBorrarUnDigito.Tag = true;
        }

        /// <summary>
        /// Evento que se dispara al presionar el boton btnOperar. Este realiza una carga el operador aritmetico.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Objeto que contiene informacion del evento.</param>
        private void btnOperar_Click(object sender, EventArgs e)
        {
            this.CancelarTask();

            if (Operacion.ErrorMatematico || ConversionBinarioDecimal.EsNumeroBinario)
            {
                this.Limpiar();
            }           

            if (Operacion.SePuedeOperar)
            {
                this.Operar();
            }
            else
            {
                this.operandoUno = new Operando(this.LblResultado);
                this.btnNegativo.Tag = false;
            }
            this.LblOperacion = string.Empty;
            Operacion.Operador = ((Button)sender).Text;

            this.MostrarPrimeraParteEnLabelOperaciones();

            Operacion.TerminoOperacion = false;
            this.btnBorrarUnDigito.Tag = false;
        }

        /// <summary>
        /// Evento que se dispara al presionar el boton btnIgual. Este evento realiza la operacion aritmetica.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Objeto que contiene informacion del evento.</param>
        private void btnIgual_Click(object sender, EventArgs e)
        {
            this.CancelarTask();

            if (Operacion.SePuedeOperar)
            {
                this.Operar();
            }
        }

        /// <summary>
        /// Evento que se dispara al presionar el boton btnNegativo. Este evento invierte el signo del operando.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Objeto que contiene informacion del evento.</param>
        private void btnNegativo_Click(object sender, EventArgs e)
        {
            if ((bool)btnNegativo.Tag && double.TryParse(this.LblResultado, out double numeroParseado) && numeroParseado != 0)
            {
                this.LblResultado = (numeroParseado * -1).ToString();                
            }
        }

        /// <summary>
        /// Metodo que se encarga de realizar la operacion aritmetica en base a los datos seleccionados por el usuario.
        /// </summary>
        private void Operar()
        {
            if(this.operandoUno is not null && Operacion.Operador is not null && Operacion.SePuedeOperar)
            {
                this.operandoDos = new Operando(this.LblResultado);

                this.MostrarSegundaParteEnLabelOperaciones();

                this.LblResultado = Operacion.Operar(this.operandoUno, this.operandoDos).ToString();

                if (Operacion.Operador != Operador.Raíz.ToString())
                {
                    this.lstOperaciones.Items.Add($"({this.operandoUno.Numero}) {Operacion.CrearCadenaDeOperador()} ({operandoDos.Numero}) = {this.LblResultado}");
                }
                else
                {
                    this.lstOperaciones.Items.Add($"({this.operandoDos.Numero}) {Operacion.CrearCadenaDeOperador()} ({operandoUno.Numero}) = {this.LblResultado}");
                }

                this.operandoUno = new Operando(this.LblResultado);
                Operacion.SePuedeOperar = false;
                Operacion.TerminoOperacion = true;
                this.btnBorrarUnDigito.Tag = false;
            }            
        }

        /// <summary>
        /// Este metodo carga el Label de operaciones con el operando y operador seleccionado por el usuario.
        /// </summary>
        private void MostrarPrimeraParteEnLabelOperaciones()
        {
            if(!Operacion.ErrorMatematico && (double.TryParse(this.LblResultado, out double _)))
            {
                if (Operacion.Operador != Operador.Raíz.ToString())
                {
                    this.LblOperacion += $"({this.LblResultado})";
                    this.LblOperacion += Operacion.CrearCadenaDeOperador();
                }
                else
                {
                    if(this.tarea == null)
                    {
                        this.token = this.ObtenerToken;

                        this.tarea = Task.Run(() =>
                        {
                            while(!this.token.IsCancellationRequested)
                            {
                                this.ResaltarLabelOperacion();
                                Thread.Sleep(250);
                            }

                            this.lblOperacion.Invoke(new Action(()=>
                            {
                                this.lblOperacion.ForeColor = this.color;
                            }));

                            this.tarea = null;
                        });                        
                    }

                    this.LblOperacion += "(Ingrese raíz... ) ";
                    this.LblOperacion += Operacion.CrearCadenaDeOperador().TrimStart();
                    this.LblOperacion += $"({this.LblResultado})";
                }
            }
        }

        /// <summary>
        /// Resalta el 'lblOperacion', alternando su color.
        /// </summary>
        private void ResaltarLabelOperacion()
        {
            if(this.lblOperacion.InvokeRequired)
            {
                this.lblOperacion.Invoke(new Action(ResaltarLabelOperacion));
            }
            else
            {
                this.lblOperacion.ForeColor = this.lblOperacion.ForeColor == this.color ? Color.Transparent : this.color;
            }
        }

        /// <summary>
        /// Este metodo carga el Label de operaciones con el segundo operando seleccionado por el usuario.
        /// </summary>
        private void MostrarSegundaParteEnLabelOperaciones()
        {
            if (!Operacion.ErrorMatematico && (double.TryParse(this.LblResultado, out double _)))
            {
                if (Operacion.Operador != Operador.Raíz.ToString())
                {
                    this.LblOperacion += $"({this.LblResultado}) = ";
                }
                else
                {
                    this.LblOperacion = this.LblOperacion.Replace("(Ingrese raíz... ) ", "");
                    this.LblOperacion = this.LblOperacion.Insert(0, $"({this.LblResultado}) ");
                    this.LblOperacion += " =";
                }
            }
        }

        /// <summary>
        /// Evento que se activa al cambiar el texto del label lblResultado. Adecua el tamaño del texto.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Objeto que contiene informacion del evento.</param>
        private void lblResultado_TextChanged(object sender, EventArgs e)
        {
            if (this.LblResultado.Length >= 44)
            {
                this.lblResultado.Font = new Font(this.lblResultado.Font.FontFamily, 10, FontStyle.Bold);
            }
            else if (this.LblResultado.Length >= 37)
            {
                this.lblResultado.Font = new Font(this.lblResultado.Font.FontFamily, 12, FontStyle.Bold);
            }
            else if(this.LblResultado.Length >= 30)
            {
                this.lblResultado.Font = new Font(this.lblResultado.Font.FontFamily, 14, FontStyle.Bold);
            }
            else if (LblResultado.Length >= 23)
            {
                this.lblResultado.Font = new Font(this.lblResultado.Font.FontFamily, 18, FontStyle.Bold);
            }
            else
            {
                this.lblResultado.Font = new Font(this.lblResultado.Font.FontFamily, 22, FontStyle.Bold);
            }
        }

        /// <summary>
        /// Evento que se activa al cambiar el texto del label lblOperacion. Adecua el tamaño del texto.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Objeto que contiene informacion del evento.</param>
        private void lblOperacion_TextChanged(object sender, EventArgs e)
        {
            if (this.LblOperacion.Length >= 44)
            {
                this.lblOperacion.Font = new Font(this.lblOperacion.Font.FontFamily, 10, FontStyle.Bold);
            }
            else if (this.LblOperacion.Length >= 37)
            {
                this.lblOperacion.Font = new Font(this.lblOperacion.Font.FontFamily, 12, FontStyle.Bold);
            }
            else
            {
                this.lblOperacion.Font = new Font(this.lblOperacion.Font.FontFamily, 14, FontStyle.Bold);
            }
        }

        /// <summary>
        /// Evento que se activa al intentar cerrar el formulario. Consulta al usuario si esta seguro de cerrar.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Objeto que contiene informacion del evento.</param>
        private void Calculadora_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(MessageBox.Show("¿Esta seguro de cerrar la aplicacion?", "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                this.CancelarTask();
            }
        }

        /// <summary>
        /// Cancela la Task.
        /// </summary>
        private void CancelarTask()
        {
            if (this.tokenDeCancelacion != null)
            {
                this.tokenDeCancelacion.Cancel();
                this.tokenDeCancelacion = null;
            }
        }

        /// <summary>
        /// Evento que se activa al presionar el btnConvertirABinario. Convierte el numero, de ser posible, a Binario.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Objeto que contiene informacion del evento.</param>
        private void btnConvertirABinario_Click(object sender, EventArgs e)
        {
            string resultadoDecimal = this.LblResultado;

            if ((bool)this.btnConvertirABinario.Tag && !Operacion.ErrorMatematico)
            {
                this.LblResultadoABinario = ConversionBinarioDecimal.DecimalBinario(this.LblResultado);
                this.btnConvertirABinario.Tag = false;
                this.btnConvertirADecimal.Tag = true;
                ConversionBinarioDecimal.EsNumeroBinario = true;
                Operacion.SePuedeOperar = false;

                this.LblOperacion = $"({resultadoDecimal})d =";

                this.lstOperaciones.Items.Add($"({resultadoDecimal})d = ({this.LblResultado})b");
            }
        }

        /// <summary>
        /// Evento que se activa al presionar el btnConvertirADecimal. Convierte el numero, de ser posible, a decimal.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Objeto que contiene informacion del evento.</param>
        private void btnConvertirADecimal_Click(object sender, EventArgs e)
        {
            string resultadoBinario = this.LblResultado;

            if ((bool)this.btnConvertirADecimal.Tag && !Operacion.ErrorMatematico)
            {
                this.LblResultado = ConversionBinarioDecimal.BinarioDecimal(this.LblResultado);
                this.btnConvertirADecimal.Tag = false;
                this.btnConvertirABinario.Tag = true;
                ConversionBinarioDecimal.EsNumeroBinario = false;

                this.LblOperacion = $"({resultadoBinario})b =";

                this.lstOperaciones.Items.Add($"({resultadoBinario})b = ({this.LblResultado})d");
            }
        }

        /// <summary>
        /// Evento que se activa al presionar el btnFactorial. Obtiene el factorial, de ser posible, del numero en pantalla.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Objeto que contiene informacion del evento.</param>
        private void btnFactorial_Click(object sender, EventArgs e)
        {
            if(!ConversionBinarioDecimal.EsNumeroBinario && !Operacion.ErrorMatematico)
            {
                string resultado = this.LblResultado;

                this.LblOperacion = $"{resultado}! =";

                this.LblResultado = Factorial.FactorialDeUnNumero(this.LblResultado).ToString();

                Operacion.TerminoOperacion = true;

                this.btnBorrarUnDigito.Tag = false;

                this.lstOperaciones.Items.Add($"({resultado})! = {this.LblResultado}");
            }
        }         

        /// <summary>
        /// Evento que se activa al presionar el btnLimpiarHistorial. Limpia el historial de operaciones realizadas.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Objeto que contiene informacion del evento.</param>
        private void btnLimpiarHistorial_Click(object sender, EventArgs e)
        {
            this.lstOperaciones.Items.Clear();
        }

        /// <summary>
        /// Evento que se activa al presionar el tsBtnHistorial. Muestra el historial de operaciones realizadas.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Objeto que contiene informacion del evento.</param>
        private void tsBtnHistorial_Click(object sender, EventArgs e)
        {
            if (!(bool)tsBtnHistorial.Tag)
            {
                this.Width += 422;
                this.lstOperaciones.Visible = true;
                this.btnLimpiarHistorial.Visible = true;
                this.tsBtnHistorial.Tag = true;
                this.tsBtnHistorial.Text = "Historial <<";
            }
            else
            {
                this.Width -= 422;
                this.lstOperaciones.Visible = false;
                this.btnLimpiarHistorial.Visible = false;
                this.tsBtnHistorial.Tag = false;
                this.tsBtnHistorial.Text = "Historial >>";
            }
        }

        #endregion

        #region Cambiar Tema

        /// <summary>
        /// Evento que se activa al presionar el uno de los botones para cambiar el Tema. Cambia el tema de la aplicacion.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Objeto que contiene informacion del evento.</param>
        private void tsBtnTema_Click(object sender, EventArgs e)
        {
            if(Object.ReferenceEquals(sender, tsBtnAzul))
            {
                this.CambiarTema(Tema.Temas.Azul);
            }
            else if (Object.ReferenceEquals(sender, tsBtnRosa))
            {
                this.CambiarTema(Tema.Temas.Rosa);
            }
            else
            {
                this.CambiarTema(Tema.Temas.Verde);
            }
        }

        /// <summary>
        /// Cambia el tema de la aplicacion
        /// </summary>
        /// <param name="tema">Objeto con la informacion del tema que se cargara.</param>
        private void CambiarTema(Tema.Temas color)
        {
            Tema tema = new Tema(color);

            foreach (Control control in this.Controls)
            {
                if (control is Button)
                {
                    control.BackColor = tema.ColorDeFondoAplicacion;
                    ((Button)control).FlatAppearance.MouseDownBackColor = tema.ColorMouseDown;
                    ((Button)control).FlatAppearance.MouseOverBackColor = tema.ColorMouseOver;
                    ((Button)control).FlatAppearance.BorderColor = tema.ColorDeBordeDeBoton;
                }
                else if (control is Label || control is ListBox)
                {
                    control.BackColor = tema.ColorDeFondoLabelYListBox;
                }
                control.ForeColor = tema.ColorDeLetra;
            }
            this.BackColor = tema.ColorDeFondoAplicacion;
            this.tsMenu.BackColor = tema.ColorDeFondoAplicacion;
            this.tsBtnHistorial.ForeColor = tema.ColorDeLetra;
            this.tsBtnColores.ForeColor = tema.ColorDeLetra;
            this.color = tema.ColorDeLetra;

            Properties.Settings.Default["tema"] = tema.TemaAplicado.ToString();
            Properties.Settings.Default.Save();
        }

        #endregion

        /// <summary>
        /// Permite el manejo de la aplicacion con botones.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Objeto con informacion del evento.</param>
        private void Calculadora_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.OemBackslash:
                    this.tsBtnHistorial_Click(this.tsBtnHistorial, e);
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
                case Keys.Space:
                    this.btnLimpiarHistorial_Click(this.btnLimpiarHistorial, e);
                    break;
                case Keys.Return:
                    this.btnIgual_Click(this.btnIgual, e);
                    break;
                case Keys.Delete:
                    this.btnLimpiar_Click(this.btnLimpiar, e);
                    break;
                case Keys.Back:
                    this.btnBorrarUnDigito_Click(this.btnBorrarUnDigito, e);
                    break;
                case Keys.Oemcomma:
                case Keys.Decimal:
                case Keys.OemPeriod:
                    this.btnNumero_Click(this.btnPunto, e);
                    break;
                case Keys.D0:
                case Keys.NumPad0:
                    this.btnNumero_Click(this.btnCero, e);
                    break;
                case Keys.D1:
                case Keys.NumPad1:
                    this.btnNumero_Click(this.btnUno, e);
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    this.btnNumero_Click(this.btnDos, e);
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    this.btnNumero_Click(this.btnTres, e);
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    this.btnNumero_Click(this.btnCuatro, e);
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    this.btnNumero_Click(this.btnCinco, e);
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    this.btnNumero_Click(this.btnSeis, e);
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    this.btnNumero_Click(this.btnSiete, e);
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                    this.btnNumero_Click(this.btnOcho, e);
                    break;
                case Keys.D9:
                case Keys.NumPad9:
                    this.btnNumero_Click(this.btnNueve, e);
                    break;
                case Keys.Add:
                    this.btnOperar_Click(this.btnSumar, e);
                    break;
                case Keys.Subtract:
                    this.btnOperar_Click(this.btnRestar, e);
                    break;
                case Keys.Divide:
                    this.btnOperar_Click(this.btnDividir, e);
                    break;
                case Keys.Multiply:
                    this.btnOperar_Click(this.btnMultiplicar, e);
                    break;
            }
        }

        /// <summary>
        /// Evalua si la tecla presionada es "Enter" o "Space", para indicar que su uso ya ha sido consumido, 
        /// evitando que estas teclas realicen otras acciones (como RE-disparar el evento de un boton que quedo seleccionado).
        /// </summary>
        /// <param name="msg">-</param>
        /// <param name="keyData">Tecla presionada.</param>
        /// <returns>True si la pulsacion de la tecla ha sido consumida y no requiere mas acciones,
        /// caso contrario False.</returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter || keyData == Keys.Space)
            {
                return true;
            }
            return false;
        }
    }
}
