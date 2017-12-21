using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinth_fights
{
    class CaseFactory
    {
        private ObjetFactory objetFactory;
        private CombattantFactory combattantFactory;
        private Labyrinthe labyrinthe;

        public CaseFactory()
        {
            objetFactory = new ObjetFactory();
            combattantFactory = new CombattantFactory();
        }
        public Case returnCase(string type,int x ,int y,Labyrinthe labyrinthe)
        {
            if (type is "mur")
            {
                return new Mur(false, "█");
            }
            else if (type is "sortie")
            {
                return new Sortie(false, " ");
            }
            else if (type is "libre")
            {
                return new OtherCase(true, " ", null);
            }
            else if (type == "objet")
            {
                return new OtherCase(false, "$", objetFactory.renvoieObjet());
            }
            else return new OtherCase(false, "O", combattantFactory.returnCombattant(x,y,labyrinthe));
        }
    }
    
}
