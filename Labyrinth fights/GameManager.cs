using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Labyrinth_fights
{
    class GameManager
    {
        private Labyrinthe labyrinthe;
        private List<OtherCase> listeCombattants;
        private List<OtherCase> listeObjets;
        private CombattantFactory combattantFactory;
        private CaseFactory caseFactory;
        private Thread threadAffichage;
        private Random random;
        private int delay;
        private List<int[]> caseLibre;
        private Thread[] threadCombattant;

        private bool directionBloque;
        private int entierBloque;

        public GameManager()
        {
            delay = 1000;
            labyrinthe = new Labyrinthe();
            listeCombattants = new List<OtherCase>();
            listeObjets = new List<OtherCase>();

            combattantFactory = new CombattantFactory();
            caseFactory = new CaseFactory();

            random = new Random();
            threadAffichage = new Thread(labyrinthe.displayBoard);
            entierBloque = 0;
            directionBloque = false;
            
            enregistrementCaseLibre();
            creerCombattants();
            ajoutCombattants();
            creerObjets();
            ajoutObjets();

            //init position vistée (position initiale)

            /*
            //create thread for each combattant
            threadCombattant = new Thread[listeCombattants.Count];

            
            for(int i = 0; i != threadCombattant.Length; i++)
            {
                threadCombattant[i] = new Thread(new ThreadStart(()=>initCombattantListVisitesAndQueue(listeCombattants[i])));
                threadCombattant[i].Start();
            }
            */
            Console.WriteLine("Appuyer pour jouer");
            Console.ReadLine();
            Console.Clear();
            threadAffichage.Start();

            Thread t1 = new Thread(() => functionCombattant(listeCombattants[0]));
            Thread t2 = new Thread(() => functionCombattant(listeCombattants[1]));
            t1.Start();
            t2.Start(); 
        }


        // return the number of free cases
        public void enregistrementCaseLibre() // OK  
        {
            //containing the positions (x and y) of the free cases (empty) => if Libre = true
            caseLibre = new List<int[]>();

            for (int i = 0; i < labyrinthe.DimX; i++)
            {
                for (int j = 0; j < labyrinthe.DimY; j++)
                {
                    //si case Libre = true, on enregistre les coordonnées dans la Liste de case libre
                    if (labyrinthe.Board[i, j].Libre)
                    {
                        int[] tab = { i, j };
                        caseLibre.Add(tab);
                    }
                }
            }
        }
        
        public void creerCombattants() // OK  
        {
            //n => number of combattants (1% of the free cases)
            double n = Math.Round((caseLibre.Count) * 0.01);

            for (int i = 0; i < n; i++)
            {
                listeCombattants.Add((OtherCase)caseFactory.returnCase("combattant",labyrinthe.DimX,labyrinthe.DimY));
            }
        }
        
        public void ajoutCombattants() // OK  
        {
            // pour chaque combattants dans "listeCombattants"
            for(int i = 0; i < listeCombattants.Count; i++)
            {
                //nouveau random sur les cases libres
                int r = random.Next(0, caseLibre.Count);

                int n1 = caseLibre[r][0]; //x
                int n2 = caseLibre[r][1]; //y

                //change positions of the combattant (in the list)
                listeCombattants[i].PositionX = n1;
                listeCombattants[i].PositionY = n2;

                //we place the combattant on the board
                labyrinthe.Board[n1, n2] = listeCombattants[i];

                //update the list of "cases libres" ////////////////
                enregistrementCaseLibre();

            }
        }

        public void creerObjets() // OK  
        {
            //n => number of objets (10% of the free cases)
            double n = Math.Round((caseLibre.Count) * 0.1);

            for (int i = 0; i < n; i++)
            {
                // cast to Otherclase because objects from objet class inherits from the Othercase class
                listeObjets.Add((OtherCase)caseFactory.returnCase("objet",labyrinthe.Board.GetLength(0),labyrinthe.Board.GetLength(1)));
            }
        }

        public void ajoutObjets() // OK  
        {
            for (int i = 0; i < listeObjets.Count; i++)
            {
                //nouveau random sur les cases libres
                int r = random.Next(0, caseLibre.Count);

                int n1 = caseLibre[r][0]; //x
                int n2 = caseLibre[r][1]; //y

                //change the position of the combattant (in the list)
                listeObjets[i].PositionX = n1;
                listeObjets[i].PositionY = n2;

                //we place the Object "Objet" on the board
                labyrinthe.Board[n1, n2] = listeObjets[i];

                //update the free cases
                enregistrementCaseLibre();
            }
        }


        /// <summary>
        //déplacement du combattant
        /// </summary>

        public void functionCombattant(OtherCase othercase)
        {
            initCombattantListVisitesAndQueue(othercase);
            int x = othercase.PositionX;
            int y = othercase.PositionY;

            while (othercase.GetType().ToString() != "Sortie") // while case is not "Sortie" Case
            {
                int n = decisiondeplacement(othercase);
                
                // si totalement bloqué => 2 cas : soit impasse => on ne peut que revenir en arriere, soit intersection dans laquelle toutes les possiblites ont été tester (donc visitées)
                if(n == 4)
                {
                    directionBloque = false;
                    //suppression de othercase (avec combattant dans board de labyrinthe)
                    supprimerCombattantSurBoard(othercase);
                    //the the first element of the stack (that corresponds to the last position of the Combattant)
                    Combattant comb = (Combattant)othercase.Content;
                    comb.Stack.Pop();
                    int [] positions = comb.Stack.Peek();
                    //modify the positions of the othercase
                    othercase.PositionX = positions[0];
                    othercase.PositionY = positions[1];
                    //ajouter sur board
                    ajouterCombattantSurBoard(othercase, positions[0], positions[1]);
                }

                // si non bloqué => n = 0,1,2,3
                else
                {
                    //suppression de othercase (avec combattant dans board de labyrinthe)
                    supprimerCombattantSurBoard(othercase);
                    directionBloque = true;
                    switch (n)
                    {
                        case 0: // gauche
                            ajouterCombattantSurBoard(othercase, x, y - 1);
                            entierBloque = 2;
                            break;
                        case 1: // haut
                            ajouterCombattantSurBoard(othercase, x - 1 , y);
                            entierBloque = 3;
                            break;
                        case 2: // droite
                            ajouterCombattantSurBoard(othercase, x, y + 1);
                            entierBloque = 0;
                            break;
                        case 3: // bas
                            ajouterCombattantSurBoard(othercase, x + 1, y);
                            entierBloque = 1;
                            break;
                        default:
                            break;
                    }
                }
                Thread.Sleep(500);
            }
        }

        public int compterCaseVisiteeAutour(OtherCase othercase)
        {
            int n = 0;
            int x = othercase.PositionX;
            int y = othercase.PositionY;

            Combattant comb = (Combattant)othercase.Content;

             if (comb.BoardVisite[x, y - 1] == true) n++;
             if (comb.BoardVisite[x, y + 1] == true) n++;
             if (comb.BoardVisite[x - 1, y] == true) n++;
             if (comb.BoardVisite[x + 1, y] == true) n++;

            return n;
        }

        public int compterCasePossible(OtherCase othercase) // OK  
        {
            int n = 0;
            int x = othercase.PositionX;
            int y = othercase.PositionY;

            if (verifDeplacementBas(x, y)) n++;
            if (verifDeplacementDroite(x, y)) n++;
            if (verifDeplacementGauche(x, y)) n++;
            if (verifDeplacementHaut(x, y)) n++;

            return n;
        }

        public void ajouterCombattantSurBoard(OtherCase othercase,int newX,int newY)
        {
            //modifcation des positions de othercase
            modifierPositionOthercase(othercase, newX, newY);
            //ajouter nouvelle position dans queue
            ajoutPositionsSurQueue(othercase);
            //ajouter case visitée à true
            ajoutVisiteHistorique(othercase);
            //ajout sur le board du labyrinthe
            labyrinthe.Board[newX, newY] = othercase;

        }

        public void modifierPositionOthercase(OtherCase othercase,int newX,int newY)
        {
            othercase.PositionX = newX;
            othercase.PositionY = newY;
        }

        public void supprimerCombattantSurBoard(OtherCase othercase) // OK  
        {
            int x = othercase.PositionX;
            int y = othercase.PositionY;
            labyrinthe.Board[x, y] = caseFactory.returnCase("libre", x, y);
        }

        public void ajoutVisiteHistorique(OtherCase othercase)// après avoir bouger
        {
            int x = othercase.PositionX;
            int y = othercase.PositionY;

            //cast du "content" de othercase
            Combattant comb = (Combattant)othercase.Content;
            //add true in the boardvisite (historic)
            comb.BoardVisite[x,y] = true;
        }

        public void ajoutPositionsSurQueue(OtherCase othercase)// après avoir bouger
        {
            int x = othercase.PositionX;
            int y = othercase.PositionY;
            Combattant comb = (Combattant)othercase.Content;
            int[] tab = { x, y };
            comb.Stack.Push(tab);
        }

        //l'othercase est déjà placé => récupère la position de othercase, add true (visite), add positions to the stack
        public void initCombattantListVisitesAndQueue(OtherCase othercase) // OK  
        {
            //récupérer la position de othercase
            int x = othercase.PositionX;
            int y = othercase.PositionY;

            //cast du "content" de othercase
            Combattant comb = (Combattant)othercase.Content;
            //add true in the boardvisite (historic)
            comb.BoardVisite[x,y] = true;
            //add the array containing the position x an y to the stack
            int[] tab2 = { x, y };
            comb.Stack.Push(tab2);
        }





        //random choice on the possibilities, return one possiblities (the choice) return a number 0,1,2,or 3
        private int decisiondeplacement(OtherCase othercase) { // OK  

            bool [] tabPossible = deplacementsPossibles(othercase.PositionX, othercase.PositionY);
            bool [] tabVisite = caseDejaVisite(othercase);

            List<int> list = new List<int>();

            for(int i = 0; i < tabPossible.Length; i++)
            {
                if(entierBloque != i && tabPossible[i] && !tabVisite[i]) list.Add(i);////////////////////////////////////
            }
            int r = 4; // si la valeur reste à 4 alors bloqué !!!!
            if(list.Count != 0) r = random.Next(0, list.Count);
            return r;
        }

        public bool [] caseDejaVisite(OtherCase othercase)
        {
            bool[] tab = { false, false, false, false }; // gauche, haut, droite, bas (pour l'ordre)

            if (verifCaseVisiteGauche(othercase)) tab[0] = true;
            if (verifCaseVisiteHaut(othercase)) tab[1] = true;
            if (verifCaseVisiteDroite(othercase)) tab[2] = true;
            if (verifCaseVisiteBas(othercase)) tab[3]= true;

            return tab;
        }

        public bool verifCaseVisiteGauche(OtherCase othercase)
        {
            Combattant combattant = (Combattant)othercase.Content;
            if (combattant.BoardVisite[othercase.PositionX, othercase.PositionY - 1]) return true;
            return false;
        }

        public bool verifCaseVisiteHaut(OtherCase othercase)
        {
            Combattant combattant = (Combattant)othercase.Content;
            if (combattant.BoardVisite[othercase.PositionX - 1, othercase.PositionY]) return true;
            return false;
        }

        public bool verifCaseVisiteDroite(OtherCase othercase)
        {
            Combattant combattant = (Combattant)othercase.Content;
            if (combattant.BoardVisite[othercase.PositionX, othercase.PositionY + 1]) return true;
            return false;
        }

        public bool verifCaseVisiteBas(OtherCase othercase)
        {
            Combattant combattant = (Combattant)othercase.Content;
            if (combattant.BoardVisite[othercase.PositionX + 1, othercase.PositionY]) return true;
            return false;
        }

        //return an array containing all the possibilities for a case AND for a specific move (block the inversed move)
        public bool[] deplacementsPossibles(int x, int y) // OK  
        {
            bool[] tab = { false, false, false, false }; // gauche, haut, droite, bas (pour l'ordre => 0,1,2,3)

            if (verifDeplacementGauche(x, y)) tab[0] = true;
            if (verifDeplacementHaut(x, y)) tab[1] = true;
            if (verifDeplacementDroite(x, y)) tab[2] = true;
            if (verifDeplacementBas(x, y)) tab[3] = true;

            return tab;
        }

        //il ne faut pas que la case ai deja été visité !!!!!!!!!!!!!!!!!!!!!!

        /// <summary>
        /// Déplacements du combattants
        /// </summary>

        private bool verifDeplacementGauche(int x,int y) // OK  
        {
            if (labyrinthe.Board[x, y - 1].Libre) return true;
            return false;
        }

        private bool verifDeplacementDroite(int x, int y) // OK  
        {
            if (labyrinthe.Board[x, y + 1].Libre) return true;
            return false;
        }

        private bool verifDeplacementHaut(int x, int y) // OK  
        {
            if (labyrinthe.Board[x - 1, y].Libre) return true;
            return false;
        }

        private bool verifDeplacementBas(int x, int y) // OK  
        {
            if (labyrinthe.Board[x + 1, y].Libre) return true;
            return false;
        }
    }
}
