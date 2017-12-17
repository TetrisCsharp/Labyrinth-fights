using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinth_fights
{
    class CombattantFactory
    {
        public CombattantFactory() { }

        public Combattant returnCombattant(int x, int y)
        {
            Random random = new Random();
            int r = random.Next(0, 2);

            //offensif
            if (r == 1) return new Combattant(true,x, y);

            //defensif (r == 0)
            else return new Combattant(false, x, y);
        }
    }
}
