using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Labyrinth_fights
{
    class Program
    {
        static void Main(string[] args)
        {
            GameManager game = new GameManager();
            game.start();
            Console.ReadKey();
        }
    }
}
