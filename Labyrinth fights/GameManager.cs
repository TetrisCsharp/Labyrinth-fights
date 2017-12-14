using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinth_fights
{
    class GameManager
    {
        private Labyrinthe labyrinthe;
        private List<OtherCase> listeCombattants;

        private CombattantFactory combattantFactory;
        private CaseFactory caseFactory;

        private Random random;

        private List<int[]> caseLibre;

        public GameManager()
        {
            labyrinthe = new Labyrinthe();
            listeCombattants = new List<OtherCase>();
            combattantFactory = new CombattantFactory();
            caseFactory = new CaseFactory();
            random = new Random();

        }

        public void start()
        {
            labyrinthe.displayBoard();
            EnregistrementCaseLibre();
            creerCombattants();
            AjoutCombattants();
            labyrinthe.displayBoard();
            AjoutObjets();

        }

        // return the number of free cases
        public void EnregistrementCaseLibre()
        {
            caseLibre = new List<int[]>();
            int n = 0;
            for (int i = 0; i < labyrinthe.DimX; i++)
            {
                for (int j = 0; j < labyrinthe.DimY; j++)
                {
                    //si case libre on enregistre les coordonnées dans la Liste de case libre
                    if (labyrinthe.Board[i, j].Libre)
                    {
                        int[] tab = { i, j };
                        caseLibre.Add(tab);
                    }
                }
            }
        }
        
        public void creerCombattants()
        {
            //n => number of combattants (1% of the free cases)
            double n = Math.Round((caseLibre.Capacity) * 0.01);

            for (int i = 0; i < n; i++)
            {
                Combattant combattant = combattantFactory.returnCombattant();
                listeCombattants.Add((OtherCase)caseFactory.returnCase("combattant"));
            }
        }
        
        public void AjoutCombattants()
        {
            // pour chaque combattants dans la liste
            for(int i = 0; i < listeCombattants.Count; i++)
            {
                //nouveau random sur les cases libres
                int r = random.Next(0, caseLibre.Count);

                int n1 = caseLibre[r][0]; //x
                int n2 = caseLibre[r][1]; //y

                //change the position of the combattant (in the list)
                listeCombattants[i].PositionX= n1;
                listeCombattants[i].PositionY = n2;

                //we place the combattant on the board
                labyrinthe.Board[n1, n2] = listeCombattants[i];

                //update the free cases
                EnregistrementCaseLibre();
                r = 0;
            }
        }

        public void creerObjets()
        {
            //n => number of objets (10% of the free cases)
            double n = Math.Round((caseLibre.Capacity) * 0.1);

            for (int i = 0; i < n; i++)
            {
                OtherCase objet = combattantFactory.returnCombattant();
                listeCombattants.Add((OtherCase)caseFactory.returnCase("combattant"));
            }
        }
        public void AjoutObjets()
        {
            int r = random.Next(0, caseLibre.Count);
        }
    }
}
