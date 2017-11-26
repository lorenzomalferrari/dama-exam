using System;
using System.Text;
using System.Threading;

public sealed class Chessboard
{
    //Interfaces
    private readonly IPlayer player1;
    private readonly IPlayer player2;
    //Statistics
    public IPlayer Winner { get; private set; }
    public IPlayer Loser { get; private set; }
    //Scores
    private int player1Score = 0;
    private int player2Score = 0;
    //Matrix representing the chessboard
    private Box[,] boxes;

    public Chessboard(IPlayer p1, IPlayer p2)
    {
        this.player1 = p1;
        this.player2 = p2;

        this.boxes = new Box[8, 8];
        IPlayer pBox = null;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (x < 3 && FillablePosition(x, y))
                    pBox = p1;
                else if (x > 4 && FillablePosition(x, y))
                    pBox = p2;
                else
                    pBox = null;

                boxes[x, y] = new Box(x, y, pBox);
            }
        }
    }

   /*public void SimulatePlay()
    {
        Random rnd = new Random();
        int winnerScore = 0;
        if (rnd.Next(0, 2) == 0)
        {
            Winner = player1;
            Loser = player2;

            winnerScore = player1Score = 12;
        }
        else
        {
            Winner = player2;
            Loser = player1;

            winnerScore = player2Score = 12;
        }

        Winner.Winner();
        Loser.Loser();

        Winner.AddScore(winnerScore, Loser);
    }*/

    public override string ToString()
    {
        var stb = new StringBuilder();
        stb.AppendLine("  0 1 2 3 4 5 6 7");
        for (int x = 0; x < 8; x++)
        {
            stb.Append(x);

            for (int y = 0; y < 8; y++)
            {
                if(boxes[x,y].Owner==player1)
                    //assing W (for White) if Player1 box
                    stb.Append("|" + "W");
                else if (boxes[x, y].Owner == player2)
                    //assing B (for Black) if Player2 box
                    stb.Append("|" + "B");
                    //assing blank if empty
                else
                    stb.Append("|"+" ");
            }
            stb.AppendLine("|");
        }

        return stb.ToString();
    }

    bool FillablePosition(int x, int y)
    {
        return ((x % 2 == 0 && y % 2 == 0) || (x % 2 != 0 && y % 2 != 0));
    }

    public void MovePawn(IPlayer p, int xStart, int yStart, int xEnd, int yEnd)
    {
        Box jumpedBox = null;
        if(xEnd < 0 || xEnd > 7 || yEnd < 0 || yEnd > 7)
        {
            throw new MoveException("Illegal move outside of the board.");
        }

        var startBox = boxes[xStart, yStart];
        if(startBox.IsFree() || !startBox.Owner.Equals(p))
            throw new MoveException("Empty box or it is not yours.");

        var arrivalBox = boxes[xEnd, yEnd];

        //win condition (reaching the other side of the board)
        if (p == player1 && xEnd == 7 || p == player1 && xEnd == 0)
            Winner = p;

        if (Math.Abs(xStart - xEnd) == 2 && Math.Abs(yStart - yEnd) == 2)
        {
            //It is a jump
            int xInter, yInter;
            if (yStart > yEnd)
            {
                yInter = yEnd + 1;
            }
            else
            {
                yInter = yEnd - 1;
            }

            if (xStart > xEnd)
            {
                xInter = xEnd + 1;
            }
            else
            {
                xInter = xEnd - 1;
            }

            //check if you can move by two boxes (to make sure you can't eat your own pawns)
            if (boxes[xInter, yInter].Owner == p || boxes[xInter, yInter] == null)
                throw new MoveException("Illegal move.");
            else
            {
                jumpedBox = boxes[xInter, yInter];
                //win condition (reaching the other side of the board)
                if (p == player1 && xEnd == 7 || p == player2 && xEnd == 0)
                    Winner = p;
            }


        }
        else if (Math.Abs(xStart - xEnd) > 2 || Math.Abs(yStart - yEnd) > 2 ||
               Math.Abs(xStart - xEnd) == 0 || Math.Abs(yStart - yEnd) == 0)
        {
            throw new MoveException("Illegal move.");
        }


        if (arrivalBox.IsFree())
        {
            arrivalBox.Owner = p;
            startBox.Free();

            if (jumpedBox != null)
            {
                jumpedBox.Free();
                if (p == player1)
                {
                    player1Score++;
                }
                else
                {
                    player2Score++;
                }
            }

            //win condition (eating all the enemy pawns)
            if (player1Score == 12 || player2Score == 12)
                Winner = p;

        }
        else if (arrivalBox.Owner.Equals(p))
        {
            throw new MoveException("You already own this box.");
        }
        else
        {
            throw new MoveException("This box is owned by the enemy.");
        }
    }

    sealed class Box
    {
        public Box(int x, int y, IPlayer p = null)
        {
            horizontal = y;
            vertical = x;
            Owner = p;
        }
        public IPlayer Owner { get; set; }
        public int vertical;//1-8
        public int horizontal;//A-H

        public bool IsFree()
        {
            return Owner == null;
        }

        public void Free()
        {
            Owner = null;
        }

        public override string ToString()
        {
            if (IsFree())
            {
                return " ";
            }
            else return Owner.ToString();
        }
    }
}
