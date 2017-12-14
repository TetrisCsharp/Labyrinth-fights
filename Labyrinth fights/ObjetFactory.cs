using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinth_fights
{
    class ObjetFactory
    {
        public Objet renvoieObjet()
        {
            Random random = new Random();
            int r = random.Next(1, 11);

            return new Objet(r);
        }
    }
}
