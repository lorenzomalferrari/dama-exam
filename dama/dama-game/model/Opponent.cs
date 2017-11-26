using System;
using System.Collections.Generic;
using System.Text;

    //factory to create players based on difficulty
    public class Opponent
    {
     public IPlayer Opp(int difficulty)
    {

        string s;

        if (difficulty == 1)
            return new PlayerEasyComputer("CPUEasy");
        else if (difficulty == 2)
            return new PlayerMediumComputer("CPUMedium");
        else if (difficulty == 3)
            return new PlayerHardComputer("CPUHard");
        else if (difficulty == 0)
        {
            Console.WriteLine("\nInsert Player name: ");
            s = Console.ReadLine();
            return new Player(s);
        }

        return new PlayerMediumComputer("CPUMedium");

    }
 }

