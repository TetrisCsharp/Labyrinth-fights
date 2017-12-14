using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinth_fights
{
    class Combattant
    {
        private int pointsDeVie;
        private int capacite;
        private Objet objet;
        private bool caractere;
        private Object symbol;

        public Combattant(bool caractere)
        {
            pointsDeVie = 100;
            capacite = 10;
            this.caractere = caractere; //defensif
            objet = null;
            
        }

        //déplacements
        private void deplacementGauche()
        {
           // if(position[0] -= 1;
        }

        private void deplacementDroite()
        {
            //position[0] += 1;
        }

        private void deplacementHaut()
        {
            //position[1] -= 1;
        }

        private void deplacementBas()
        {
           // position[1] += 1;
        }

        private void rammasseObjet()
        {
            
        }

        public void changerCapaciteSuivantObjet()
        {
            //capacite += objet.valeur;
        }

        public void comportement()
        {
            Random random = new Random();
            // int n = random.Next()
            return;
        }

        //propriétés
        public int PointDeVie
        {
            get { return this.pointsDeVie;  }
            set { this.pointsDeVie = value; }
        }
        public int Capacite
        {
            get { return this.capacite; }
            set { this.capacite = value; }
        }

    }

}
