using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfFifteen
{


    public class AutoSolveClass
    {
        private int deepness, minPrevIter;
        private string map;

        public AutoSolveClass()
        {
            deepness = 0;
            minPrevIter = 0;
        }

        public string GetPathIDAStar(BoardState start)
        {;       
            if (IDAStar(start))
                return new string(map.Reverse().ToArray());
            return "";
        }

        // Calculate Manhatten distance for current board state
        private int ManhattenDistance(int[] b)
        {
            int res = 0;
            for (int i = 0; i < 16; i++)
            {
                if (b[i] == 0)
                    continue;
                for (int j = 0; j < 16; j++)
                {
                    if (j == b[i] - 1)
                    {
                        res += Math.Abs(i % 4 - j % 4) + Math.Abs(i / 4 - j / 4);
                    }
                }
            }

            return res;
        }

        // Find current states`s neighbors
        private List<BoardState> FindNeighbors(BoardState parent)
        {
            List<BoardState> l = new List<BoardState>();
            int[] tempArr;
            int i, zeroIndex = 0;

            for (i = 0; i < 16; i++)
            {
                if (parent.boardNumArray[i] == 0)
                {
                    zeroIndex = i;
                    break;
                }
            }
             
            for (i = 0; i < 16; i++)
            {
                if (Math.Abs(zeroIndex - i) == 1 || Math.Abs(zeroIndex - i) == 4)
                {
                    if (zeroIndex - i == 1 && zeroIndex % 4 == 0)
                        continue;
                    if (zeroIndex - i == -1 && zeroIndex % 4 == 3)
                        continue;
                    else
                    {
                        tempArr = (int[])parent.boardNumArray.Clone();
                        tempArr[zeroIndex] = tempArr[i];
                        tempArr[i] = 0;
                        switch (zeroIndex - i)
                        {
                            case -1:
                                l.Add(new BoardState("R", tempArr, parent));
                                break;
                            case -4:
                                l.Add(new BoardState("D", tempArr, parent));
                                break;
                            case 1:
                                l.Add(new BoardState("L", tempArr, parent));
                                break;
                            case 4:
                                l.Add(new BoardState("U", tempArr, parent));
                                break;
                        }
                    }
                }
            }

            return l;
        }

        // IDA star algorithm
        private bool IDAStar(BoardState start)
        {
            bool res = false;
            deepness = ManhattenDistance(start.boardNumArray);
            do
            {
                minPrevIter = 2147483647;
                res = recSearch(0, start);
                deepness = minPrevIter;
            } while ((!res) && (deepness <= 50));

            return res;
        }

        // Recursive method wich call in IDAStar method
        private bool recSearch(int g, BoardState prevState)
        {
            int h = ManhattenDistance(prevState.boardNumArray);
            bool res;
            List<BoardState> Neighbors;

            if (h == 0)
                return true;

            int f = g + h;

            if (f > deepness)
            {
                if (minPrevIter > f)
                    minPrevIter = f;
                return false;
            }

            Neighbors = FindNeighbors(prevState);

            foreach (var y in Neighbors)
            {
                if (prevState.move != "") //If state is not a start position
                {
                    if (!y.boardNumArray.SequenceEqual(prevState.parent.boardNumArray))
                    {
                        res = recSearch(g + 1, y);
                        if (res)
                        {
                           // myDelegate(y.move);
                            map += y.move;
                            return true;
                        }
                    }
                }
                else
                {
                    res = recSearch(g + 1, y);
                    if (res)
                    {
                       // myDelegate(y.move);
                        map += y.move;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
