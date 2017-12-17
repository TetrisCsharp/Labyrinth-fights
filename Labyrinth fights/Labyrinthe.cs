using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Labyrinth_fights
{
    class Labyrinthe
    {
        private Case[,] board;
        private int dimX;
        private int dimY;
            
        private string[,] temp;
        private CaseFactory caseFactory;
        private CombattantFactory combattantFactory;

        public Labyrinthe()
        {
            // get the dimensions of the matrix before initialized the temp matrix
            StreamReader file1 = new StreamReader("board.txt");
            string line1;
            int n = 0;
            bool done = false;

            while ((line1 = file1.ReadLine()) != null)
            {
                if (!done)
                {
                    dimY = line1.Length;
                    done = true;
                }
                n++;
            }
            dimX = n;

            // fill the temp matrix with the data inside the file
            string line2;
            StreamReader file2 = new StreamReader("board.txt");
            temp = new string[dimX, dimY];

            while ((line2 = file2.ReadLine()) != null)
            {
                for (int i = 0; i < dimX; i++)
                {
                    string data = line2;
                    for (int j = 0; j < dimY; j++)
                    {
                        temp[i, j] = data[j].ToString();
                    }
                    line2 = file2.ReadLine();
                }
            }

            caseFactory = new CaseFactory();
            combattantFactory = new CombattantFactory();

            RemplissageBoard();
            //AffichageBoardTemp();
            //displayBoard();
        }

        //display the temp board (only used for the test)
        public void AffichageBoardTemp()
        {
            for (int i = 0; i < dimX; i++)
            {
                for (int j = 0; j < dimY; j++)
                {
                    Console.Write(temp[i, j]);
                    if (j == dimY - 1) Console.Write('\n');
                }
            }
        }
        // display the board in the console
        public void displayBoard()
        {
            while (true)
            {
                for (int i = 0; i < dimX; i++)
                {
                    for (int j = 0; j < dimY; j++)
                    {
                        // si mur ou sortie (classe à part) OU libre
                        if (board[i, j].GetType() == typeof(Mur) || board[i, j].GetType() == typeof(Sortie) || board[i, j].Element.ToString() == " ")
                        {
                            Console.Write(board[i, j].Element);
                            if (j == dimY - 1) Console.Write('\n');
                        }
                        else
                        {
                            OtherCase oc = (OtherCase)board[i, j];
                            Console.Write(oc.Element);
                        }
                    }
                }
                Thread.Sleep(200); 
                Console.SetCursorPosition(0, 0);
            }
        }

        public void RemplissageBoard()
        {
            board = new Case[dimX, dimY];

            for (int i = 0; i < dimX; i++)
            {
                for (int j = 0; j < dimY; j++)
                {
                    switch (temp[i, j])
                    {
                        case "0":
                            board[i, j] = caseFactory.returnCase("libre",dimX,dimY);
                            break;
                        case "1":
                            board[i, j] = caseFactory.returnCase("mur",dimX,dimY);
                            break;
                        case "2":
                            board[i, j] = caseFactory.returnCase("sortie",dimX,dimY);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        // Properties
        public int DimX
        {
            get{ return this.dimX; }
        }

        public int DimY
        {
            get { return this.dimY; }
        }

        public Case[,] Board
        {
            get { return this.board; }
            set { this.board = value; }
        }
    }
}
