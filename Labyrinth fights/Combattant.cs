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
        private bool[,] boardVisite;
        private Stack<int[]> stack;

        public Combattant(bool caractere,int x, int y)
        {
            boardVisite = new bool[x,y];
            initBoardVisites(x,y);
            pointsDeVie = 100;
            capacite = 10;
            this.caractere = caractere; //defensif
            objet = null;
            stack = new Stack<int[]>();
        }

        public void initBoardVisites(int x, int y)
        {
            for (int i = 0; i < boardVisite.GetLength(0); i++)
            {
                for(int j = 0; j < boardVisite.GetLength(1); j++)
                {
                    boardVisite[i, j] = false;
                }
            }
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
        public Stack<int[]> Stack
        {
            get { return this.stack; }
            set { this.stack = value; }
        }

        public bool[,] BoardVisite
        {
            get { return this.boardVisite; }
            set { this.boardVisite = value; }
        }
    }

}
