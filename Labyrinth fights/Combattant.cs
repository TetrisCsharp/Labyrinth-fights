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
        private bool offensif;
        private Object symbol;
        private bool[,] boardVisite;
        private Stack<int[]> stack;
        private ObjetFactory objetFactory;

        public Combattant(bool offensif,int x, int y,Labyrinthe lab)
        {
            boardVisite = new bool[x,y];
            initBoardVisites(x,y,lab);
            pointsDeVie = 100;
            capacite = 10;
            this.offensif = offensif; //defensif
            this.objetFactory = new ObjetFactory();
            objet = objetFactory.renvoieObjet();
            objet.Valeur = 0;
            stack = new Stack<int[]>();
        }

        public void initBoardVisites(int x, int y,Labyrinthe lab)
        {
            for (int i = 0; i < boardVisite.GetLength(0); i++)
            {
                for(int j = 0; j < boardVisite.GetLength(1); j++)
                {
                    if (lab.Board[i, j] is Mur) boardVisite[i, j] = true;
                    else boardVisite[i, j] = false;
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

        public Objet Objet
        {
            get { return this.objet; }
            set { this.objet = value; }
        }

        public bool Offensif
        {
            get => offensif;
            set => offensif = value;
        }
    }

}
