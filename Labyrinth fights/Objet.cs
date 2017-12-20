using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinth_fights
{
    class Objet
    {
        private int valeur;

        public Objet(int valeur)
        {
            this.valeur = valeur;
        }

        public int Valeur
        {
            get { return this.valeur; }
            set { this.valeur = value; }
        }
    }
}
