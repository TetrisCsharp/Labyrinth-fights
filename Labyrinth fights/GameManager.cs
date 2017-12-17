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

            //init board and others

            enregistrementCaseLibre();
            creerCombattants();
            ajoutCombattants();
            creerObjets();
            // ajoutObjets();
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
        public void enregistrementCaseLibre()
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
            double n = Math.Round((caseLibre.Count) * 0.01);

            for (int i = 0; i < n; i++)
            {
               // Combattant combattant = combattantFactory.returnCombattant();
                listeCombattants.Add((OtherCase)caseFactory.returnCase("combattant",labyrinthe.Board.GetLength(0),labyrinthe.Board.GetLength(1)));
            }
        }
        
        public void ajoutCombattants()
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
                enregistrementCaseLibre();
                r = 0;
            }
        }

        public void creerObjets()
        {
            //n => number of objets (10% of the free cases)
            double n = Math.Round((caseLibre.Count) * 0.1);

            for (int i = 0; i < n; i++)
            {
 
                listeObjets.Add((OtherCase)caseFactory.returnCase("objet",labyrinthe.Board.GetLength(0),labyrinthe.Board.GetLength(1)));
            }
        }

        public void ajoutObjets()
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

                //we place the combattant on the board
                labyrinthe.Board[n1, n2] = listeObjets[i];

                //update the free cases
                enregistrementCaseLibre();
                r = 0;
            }
        }

        public void functionCombattant(OtherCase othercase)
        {
            initCombattantListVisitesAndQueue(othercase);
            int x = othercase.PositionX;
            int y = othercase.PositionY;

            while (true)
            {
                int n = decisiondeplacement(othercase);
                
                int possibilities = compterCasePossible(othercase);
                int visites = compterCaseVisiteeAutour(othercase);

                // si totalement bloqué => 2 cas : soit impace => on ne peut que revenir en arriere, soit intersection dans laquelle toutes les possiblites ont été tester (donc visitées)
                if(n == 0 || n != 0 && possibilities == visites)
                {
                    //suppression de othercase (avec combattant dans board de labyrinthe)
                    supprimerCombattantSurBoard(othercase);
                    //the the first element of the stack (that corresponds to the last position of the Combattant)
                    Combattant comb = (Combattant)othercase.Content;
                    comb.Stack.Pop();
                    int [] positions = comb.Stack.Peek();
                    //ajouter 
                    ajouterCombattantSurBoard(othercase, positions[0], positions[1]);
                    

                }

                // si non bloqué
                else
                {
                    //suppression de othercase (avec combattant dans board de labyrinthe)
                    supprimerCombattantSurBoard(othercase);

                    switch (n)
                    {
                        case 1: // gauche
                            ajouterCombattantSurBoard(othercase, x, y - 1);
                            entierBloque = 3;
                            break;
                        case 2: // haut
                            ajouterCombattantSurBoard(othercase, x - 1 , y);
                            entierBloque = 4;
                            break;
                        case 3: // droite
                            ajouterCombattantSurBoard(othercase, x, y + 1);
                            entierBloque = 1;
                            break;
                        case 4: // bas
                            ajouterCombattantSurBoard(othercase, x + 1, y);
                            entierBloque = 2;
                            break;
                        default:
                            break;
                    }
                }
                Thread.Sleep(delay);
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

        public int compterCasePossible(OtherCase othercase)
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

        public void supprimerCombattantSurBoard(OtherCase othercase)
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

        public void initCombattantListVisitesAndQueue(OtherCase othercase)
        {
            //récupérer la position de othercase
            int x = othercase.PositionX;
            int y = othercase.PositionY;

            //cast du "content" de othercase
            Combattant comb = (Combattant)othercase.Content;
            //add true in the boardvisite (historic)
            comb.BoardVisite[x,y] = true;
            //add the array containing the position x an y to the queue
            int[] tab2 = { x, y };
            comb.Stack.Push(tab2);
        }

        private int decisiondeplacement(OtherCase othercase) // OK  
        {
            List<int> tab = deplacementsPossibles(othercase.PositionX, othercase.PositionY);
            int r = random.Next(0, tab.Count);
            return tab[r];
        }

        public List<int> deplacementsPossibles(int x, int y) // OK  
        {
            List<int> tab = new List<int>(); // gauche, haut, droite, bas (pour l'ordre)

            if(entierBloque != 3) if (verifDeplacementGauche(x, y)) tab.Add(1);
            if(entierBloque != 4) if (verifDeplacementHaut(x, y)) tab.Add(2);
            if(entierBloque != 1) if (verifDeplacementDroite(x, y)) tab.Add(3);
            if(entierBloque != 2) if (verifDeplacementBas(x, y)) tab.Add(4);

            return tab;
        }

        //Déplacements du combattants
        private bool verifDeplacementGauche(int x,int y) // OK
        {
            if (labyrinthe.Board[x, y - 1].Libre) return true;
            return false;
        }

        private bool verifDeplacementDroite(int x, int y)
        {
            if (labyrinthe.Board[x, y + 1].Libre) return true;
            return false;
        }

        private bool verifDeplacementHaut(int x, int y)
        {
            if (labyrinthe.Board[x - 1, y].Libre) return true;
            return false;
        }

        private bool verifDeplacementBas(int x, int y)
        {
            if (labyrinthe.Board[x + 1, y].Libre) return true;
            return false;
        }
    }
}
