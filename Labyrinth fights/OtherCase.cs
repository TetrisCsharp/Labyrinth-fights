using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labyrinth_fights
{
    class OtherCase : Case
    {
        private int positionX;
        private int positionY;
        private Object content;

        public OtherCase(bool libre, string element,Object content) : base(libre,element)
        {
            this.content = content;
        }
        public int  PositionX
        {
            get { return this.positionX; }
            set { this.positionX = value; }
        }

        public int PositionY
        {
            get { return this.positionY; }
            set { this.positionY = value; }
        }

        public Object Content
        {
            get { return this.content; }
            set { this.content = value; }
        }
    }
}
