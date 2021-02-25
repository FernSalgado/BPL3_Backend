using BPL3_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPL3_Backend.Services
{
    public class PointsService
    {
        public PointsService()
        {

        }
        private static void GetTeams() 
        {
            
        }
        private static List<int> CalcPoints(List<Member> members)
        {
            List<int> points = new List<int>();
            points.Add(0);
            points.Add(0);
            foreach (var item in members)
            {
                points[0] += CalcTeamLevelPoints(item.Level);
                points[1] += CalcTeamDelvePoints(item.Delve / 10);
            }
            return points;
        }
        private static int CalcTeamLevelPoints(int level)
        {
            int points = 0;

            if (level <= 95)
            {
                points += level;
            }
            else 
            {
                points += (level + ((level - 95) * 5));
            }

            if (level >= 10 && level <= 80)
            {
                points += (level / 10) * 2;
            }
            else 
            {
                if (level >= 85) points += 5;
                if (level >= 90) points += 10;
                if (level >= 55) points += 15;
            }
            return points;
        }
        private static int CalcTeamDelvePoints(int delve)
        {
            int points = 0;
            int count = 100;
            if (delve >= 100) 
            {
                while (count <= delve && count <=500) 
                {
                    points += 4;
                    count += 25;
                }
                if (delve >= 300 && delve <= 500) 
                {
                    points += ((delve - 200)/100)*4;
                }
                if (delve > 600) 
                {
                    points += 12;
                    points += (delve - 500) / 10;
                }
            }
            return points;
        }
    }
}
