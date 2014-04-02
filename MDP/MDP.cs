using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace MDP
{
    class MDP
    {
        public Dictionary<string, double> StateDict;
        public Dictionary<string, double>[][,] transitionMatrix;
        public Dictionary<string, double>[] ActionProbability;
        public MDP(int numstates, int numactions)
        {
            StateDict = new Dictionary<string, double>();
            transitionMatrix = new Dictionary<string, double>[numactions][,];
            ActionProbability = new Dictionary<string, double>[numactions];
           
            for (int i = 0; i < numactions; i++)
            {
                transitionMatrix[i] = new Dictionary<string, double>[numstates, numstates];

            }
        }
        public void read_input(string file, int numstates, int numactions)
        {
            string input;
            string[] inputlines;
           
            
            var sr = new StreamReader(file);
            input = sr.ReadToEnd();
            inputlines = input.Split('\n');
            foreach (var item in inputlines)
            {
                IEnumerable<string> sequence = Regex.Split(item, @"\W+").Take(2);
                StateDict.Add(sequence.First(),Convert.ToDouble(sequence.ElementAt(1)));

                var next = Regex.Split(item, @"^[a-z][1-9]");
                MatchCollection mc = Regex.Matches(item, @"[a-z][1-9]|(\d*\.+\d+)");
                string[] matches = new string[mc.Count];
                int i=0;
                foreach (Match m in mc)
                {
                    matches[i++] = m.Value;
                }
                
                for (i = 1; i < matches.Length; i+=3)
                {
                    ActionProbability[(int)(matches[i][1]-'0')-1] = new Dictionary<string,double>(){{matches[i], Convert.ToDouble(matches[i + 2])}};
                    transitionMatrix[((int)(matches[i][1] - '0') - 1)][(int)(matches[0][1] - '0') - 1, (int)(matches[i + 1][1] - '0') - 1] = ActionProbability[(int)(matches[i][1] - '0') - 1];
                

                }                
            }
     


        }
        static void Main(string[] args)
        {
            string input = @"c:\users\ap\documents\visual studio 2013\Projects\MDP\MDP\test-win.in";
            MDP mdp = new MDP(4,2);
            mdp.read_input(input,4,2);
            Console.ReadKey();
        }
  
            
    }
}
