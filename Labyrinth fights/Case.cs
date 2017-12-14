using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinth_fights
{
    class Case
    {
        private bool libre; // true if libre, false if occupé
        private string element; // element à afficher
        private Object content;

        public Case(bool libre,string element)
        {
            this.libre = libre;
            this.element = element;// caractère ou type Object (Combattant ou objet)
        }




        public bool Libre
        {
            get { return this.libre; }
            set { this.libre = value; }
        }

        public string Element
        {
            get { return this.element; }
            set { this.element = value; }
        }
    }
}
