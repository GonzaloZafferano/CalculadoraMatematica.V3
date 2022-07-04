using System;
using System.Drawing;

namespace BibliotecaDeColores
{
    public class Tema
    {
        public enum Temas { Rosa, Verde, Azul };

        private Color colorDeLetra;
        private Color colorDeFondoAplicacion;
        private Color colorDeFondoLabelYListBox;
        private Color colorDeBordeDeBoton;
        private Color colorMouseOver;
        private Color colorMouseDown;
        private Temas tema;

        public Color ColorDeLetra { get => this.colorDeLetra; }
        public Color ColorDeFondoAplicacion { get => this.colorDeFondoAplicacion; }
        public Color ColorDeFondoLabelYListBox { get => this.colorDeFondoLabelYListBox; }
        public Color ColorDeBordeDeBoton { get => this.colorDeBordeDeBoton; }
        public Color ColorMouseOver { get => this.colorMouseOver; }
        public Color ColorMouseDown { get => this.colorMouseDown; }
        public Temas TemaAplicado { get => this.tema; }

        public Tema(Temas tema)
        {
            this.tema = tema;
            this.CargarTema();
        }

        private void CargarTema()
        {
            switch (this.tema)
            {
                case Temas.Azul:
                    this.colorDeBordeDeBoton = Color.LightSteelBlue;
                    this.colorDeFondoAplicacion = Color.LightSteelBlue;
                    this.colorDeFondoLabelYListBox = Color.CornflowerBlue;
                    this.colorDeLetra = Color.DarkBlue;
                    this.colorMouseDown = Color.Aqua;
                    this.colorMouseOver = Color.Aquamarine;
                    break;
                case Temas.Rosa:
                    this.colorDeBordeDeBoton = Color.LightPink;
                    this.colorDeFondoAplicacion = Color.LightPink;
                    this.colorDeFondoLabelYListBox = Color.HotPink;
                    this.colorDeLetra = Color.MediumVioletRed;
                    this.colorMouseDown = Color.HotPink;
                    this.colorMouseOver = Color.LavenderBlush;
                    break;
                case Temas.Verde:
                    this.colorDeBordeDeBoton = Color.YellowGreen;
                    this.colorDeFondoAplicacion = Color.YellowGreen;
                    this.colorDeFondoLabelYListBox = Color.OliveDrab;
                    this.colorDeLetra = Color.DarkGreen;
                    this.colorMouseDown = Color.LawnGreen;
                    this.colorMouseOver = Color.GreenYellow;
                    break;
            }
        }
    }
}
