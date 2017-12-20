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
            delay = 200;
            labyrinthe = new Labyrinthe();
            listeCombattants = new List<OtherCase>();
            listeObjets = new List<OtherCase>();

            combattantFactory = new CombattantFactory();
            caseFactory = new CaseFactory();

            random = new Random();
            threadAffichage = new Thread(labyrinthe.displayBoard);
            entierBloque = 0;
            directionBloque = false;
            
            EnregistrementCaseLibre();
            CreerCombattants();
            AjoutCombattants();
            CreerObjets();
            AjoutObjets();

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

            Thread t1 = new Thread(() => FunctionCombattant(listeCombattants[0]));
            Thread t2 = new Thread(() => FunctionCombattant(listeCombattants[1]));
            t1.Start();
            t2.Start(); 
        }


        // return the number of free cases
        private void EnregistrementCaseLibre() // OK  
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
        
        private void CreerCombattants() // OK  
        {
            //n => number of combattants (1% of the free cases)
            double n = Math.Round((caseLibre.Count) * 0.01);

            for (int i = 0; i < n; i++)
            {
                listeCombattants.Add((OtherCase)caseFactory.returnCase("combattant",labyrinthe.DimX,labyrinthe.DimY));
            }
        }
        
        private void AjoutCombattants() // OK  
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
                EnregistrementCaseLibre();

            }
        }

        private void CreerObjets() // OK  
        {
            //n => number of objets (10% of the free cases)
            double n = Math.Round((caseLibre.Count) * 0.1);

            for (int i = 0; i < n; i++)
            {
                // cast to Otherclase because objects from objet class inherits from the Othercase class
                listeObjets.Add((OtherCase)caseFactory.returnCase("objet",labyrinthe.Board.GetLength(0),labyrinthe.Board.GetLength(1)));
            }
        }

        private void AjoutObjets() // OK  
        {
            for(int i = 0; i < listeObjets.Count; i++)
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
                EnregistrementCaseLibre();
            }
        }

        private void FunctionCombattant(OtherCase othercase)
        {
            //1 seul fois
            InitCombattantListVisitesAndQueue(othercase);
            //les positions que l'on va incrémenter à chaque fois
            int x = othercase.PositionX;
            int y = othercase.PositionY;
            entierBloque = 4;

            while (othercase.GetType().ToString() != "Sortie") // while case is not "Sortie" Case
            {
                int n = Decisiondeplacement(othercase);
                
                // si totalement bloqué => 2 cas : soit impasse => on ne peut que revenir en arriere, soit intersection dans laquelle toutes les possiblites ont été tester (donc visitées)
                if(n == 4)
                {
                    entierBloque = 4;
                    // revenir sur nos pas tant qu'il n'y a pas de case (non visité, et libre)

                    //suppression de othercase (avec combattant dans board de labyrinthe)
                    SupprimerCombattantSurBoard(othercase);
                    //delete the last position in the stack (the current position)
                    Combattant comb = (Combattant)othercase.Content;
                    comb.Stack.Pop();
                    int[] positions = comb.Stack.Peek();
                    //modify the positions of the othercase
                    othercase.PositionX = positions[0];
                    othercase.PositionY = positions[1];
                    x = positions[0];
                    y = positions[1];
                    //réajouter sur board sur positions précédentes
                    labyrinthe.Board[othercase.PositionX, othercase.PositionY] = othercase;                   
                    
                }

                // si non bloqué => n = 0,1,2,3
                else
                {
                    //suppression de othercase (avec combattant dans board de labyrinthe)
                    SupprimerCombattantSurBoard(othercase);
                   

                    switch (n)
                    {
                        case 0: // gauche
                            if (VerifCaseContientObjet(x, y - 1))
                            {
                                AjouterObjetAuCombattant(othercase, x, y - 1);
                            }
                            AjouterCombattantSurBoard(othercase, x, y - 1);
                            entierBloque = 2;
                            y -= 1;
                            break;

                        case 1: // haut
                            if (VerifCaseContientObjet(x - 1, y))
                            {
                                AjouterObjetAuCombattant(othercase, x - 1, y);
                            }
                            AjouterCombattantSurBoard(othercase, x - 1 , y);
                            entierBloque = 3;
                            x -= 1;
                            break;

                        case 2: // droite
                            if (VerifCaseContientObjet(x, y + 1))
                            {
                                AjouterObjetAuCombattant(othercase, x, y + 1);
                            }
                            AjouterCombattantSurBoard(othercase, x, y + 1);
                            entierBloque = 0;
                            y += 1;
                            break;

                        case 3: // bas
                            if (VerifCaseContientObjet(x + 1, y))
                            {
                                AjouterObjetAuCombattant(othercase,x +1,y);
                            }
                            AjouterCombattantSurBoard(othercase, x + 1, y);
                            entierBloque = 1;
                            x += 1;
                            break;

                        default:
                            break;
                    }
                }
                Thread.Sleep(delay);
            }
        }

        private void AjouterCombattantSurBoard(OtherCase othercase,int newX,int newY)
        {
            //modifcation des positions de othercase
            ModifierPositionOthercase(othercase, newX, newY);
            //ajouter nouvelle position dans queue
            AjoutPositionsSurQueue(othercase);
            //ajouter case visitée à true
            AjoutVisiteHistorique(othercase);
            //ajout sur le board du labyrinthe
            labyrinthe.Board[newX, newY] = othercase;

        }

        private void ModifierPositionOthercase(OtherCase othercase,int newX,int newY)
        {
            othercase.PositionX = newX;
            othercase.PositionY = newY;
        }

        private void SupprimerCombattantSurBoard(OtherCase othercase) // OK  
        {
            int x = othercase.PositionX;
            int y = othercase.PositionY;
            labyrinthe.Board[x, y] = caseFactory.returnCase("libre", x, y);
        }

        private void AjoutVisiteHistorique(OtherCase othercase)// après avoir bouger
        {
            int x = othercase.PositionX;
            int y = othercase.PositionY;

            //cast du "content" de othercase
            Combattant comb = (Combattant)othercase.Content;
            //add true in the boardvisite (historic)
            comb.BoardVisite[x,y] = true;
        }

        private void AjoutPositionsSurQueue(OtherCase othercase)// après avoir bouger
        {
            int x = othercase.PositionX;
            int y = othercase.PositionY;
            Combattant comb = (Combattant)othercase.Content;
            int[] tab = { x, y };
            comb.Stack.Push(tab);
        }

        //l'othercase est déjà placé => récupère la position de othercase, add true (visite), add positions to the stack
        private void InitCombattantListVisitesAndQueue(OtherCase othercase) // OK  
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
        private int Decisiondeplacement(OtherCase othercase) { // OK  

            bool [] tabPossible = DeplacementsPossibles(othercase.PositionX, othercase.PositionY);
            bool [] tabVisite = CaseDejaVisite(othercase);

            List<int> list = new List<int>();

            for(int i = 0; i < tabPossible.Length; i++)
            {
                if (entierBloque != i && tabPossible[i] && !tabVisite[i])
                {
                    list.Add(i);
                }
            }
            int r = 4; // si la valeur reste à 4 alors bloqué !!!!
            if (list.Count != 0)
            {
                r = random.Next(0, list.Count);
                return list[r];
            }
            return r;
        }

        private bool [] CaseDejaVisite(OtherCase othercase)
        {
            bool[] tab = { false, false, false, false }; // gauche, haut, droite, bas (pour l'ordre)

            if (VerifCaseVisiteGauche(othercase)) tab[0] = true;
            if (VerifCaseVisiteHaut(othercase)) tab[1] = true;
            if (VerifCaseVisiteDroite(othercase)) tab[2] = true;
            if (VerifCaseVisiteBas(othercase)) tab[3]= true;

            return tab;
        }

        private bool VerifCaseVisiteGauche(OtherCase othercase)
        {
            Combattant combattant = (Combattant)othercase.Content;
            if (combattant.BoardVisite[othercase.PositionX, othercase.PositionY - 1]) return true;
            return false;
        }

        private bool VerifCaseVisiteHaut(OtherCase othercase)
        {
            Combattant combattant = (Combattant)othercase.Content;
            if (combattant.BoardVisite[othercase.PositionX - 1, othercase.PositionY]) return true;
            return false;
        }

        private bool VerifCaseVisiteDroite(OtherCase othercase)
        {
            Combattant combattant = (Combattant)othercase.Content;
            if (combattant.BoardVisite[othercase.PositionX, othercase.PositionY + 1]) return true;
            return false;
        }

        private bool VerifCaseVisiteBas(OtherCase othercase)
        {
            Combattant combattant = (Combattant)othercase.Content;
            if (combattant.BoardVisite[othercase.PositionX + 1, othercase.PositionY]) return true;
            return false;
        }

        //return an array containing all the possibilities for a case AND for a specific move (block the inversed move)
        private bool[] DeplacementsPossibles(int x, int y) // OK  
        {
            bool[] tab = { false, false, false, false }; // gauche, haut, droite, bas (pour l'ordre => 0,1,2,3)

            if (VerifDeplacementGauche(x, y)) tab[0] = true;
            if (VerifDeplacementHaut(x, y)) tab[1] = true;
            if (VerifDeplacementDroite(x, y)) tab[2] = true;
            if (VerifDeplacementBas(x, y)) tab[3] = true;

            return tab;
        }


        private bool VerifDeplacementGauche(int x,int y) // OK  
        {
            if (labyrinthe.Board[x, y - 1].Libre || VerifCaseContientObjet(x, y - 1)) return true;
            return false;
        }

        private bool VerifDeplacementDroite(int x, int y) // OK  
        {
            if (labyrinthe.Board[x, y + 1].Libre || VerifCaseContientObjet(x, y + 1)) return true;
            return false;
        }

        private bool VerifDeplacementHaut(int x, int y) // OK  
        {
            if (labyrinthe.Board[x - 1, y].Libre || VerifCaseContientObjet(x - 1, y)) return true;
            return false;
        }

        private bool VerifDeplacementBas(int x, int y) // OK  
        {
            if (labyrinthe.Board[x + 1, y].Libre || VerifCaseContientObjet(x + 1, y)) return true;
            return false;
        }

        private bool VerifCaseContientObjet(int x, int y) // case à coté du combattant
        {
            if (labyrinthe.Board[x,y] is OtherCase)
            {
                OtherCase othr = (OtherCase)labyrinthe.Board[x, y];
                if (!labyrinthe.Board[x, y].Libre && othr.Content is Objet)
                {
                    return true;
                }
            }
            return false;
        }

        private void AjouterObjetAuCombattant(OtherCase othercase, int x, int y)
        {
            OtherCase other = (OtherCase)labyrinthe.Board[x, y];
            Objet objet = (Objet)other.Content;
            Combattant combattant = (Combattant)othercase.Content;
            int val = objet.Valeur + combattant.Objet.Valeur;

            combattant.Objet.Valeur = val;
        }

        public bool VerifCaseContientEnnemi(OtherCase othercase,int x, int y)
        {

            OtherCase othr = (OtherCase) labyrinthe.Board[x, y];
            if (!labyrinthe.Board[x, y].Libre && othr.Content is Combattant)
            {
                return true;
            }

            return false;
        }

        public void CombattreEnnemi(OtherCase othercase, int x, int y)
        {

        }
    }
}
