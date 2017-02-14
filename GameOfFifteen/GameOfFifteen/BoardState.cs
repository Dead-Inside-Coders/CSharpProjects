using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfFifteen
{
    public class BoardState:IComparable<BoardState>
    {
        public int g, h, f;
        public int[] boardNumArray = new int[16];
        public string move;
        public BoardState parent;

        public BoardState()
        {
            g = 0;
            h = 0;
            f = 0;
            move = "";
        }

        public BoardState(string m, int[] ar, BoardState p)
        {
            move = m;
            boardNumArray = (int[])ar.Clone();
            parent = p;
        }

        int IComparable<BoardState>.CompareTo(BoardState other)
        {
            if (other.f > this.f)
                return -1;
            else if (other.f == this.f)
                return 0;
            else
                return 1;
        }
    }
}
