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
        public int numstates { get; set; }
        public int numactions { get; set; }
        public double gamma { get; set; }
        public Dictionary<string, double> StateDict;
        public Dictionary<string, double>[][,] transitionMatrix;
        public Dictionary<string, double>[] ActionProbability;
        public double[] Jest;
        public Dictionary<string, string> policy;
        public MDP(int Numstates, int Numactions,double Gamma)
        {
            numstates = Numstates;
            numactions = Numactions;
            gamma = Gamma;
            StateDict = new Dictionary<string, double>();
            transitionMatrix = new Dictionary<string, double>[numactions][,];
            ActionProbability = new Dictionary<string, double>[numactions];
            policy = new Dictionary<string, string>();
            Jest = new double[numstates];
            for (int i = 0; i < numactions; i++)
            {
                transitionMatrix[i] = new Dictionary<string, double>[numstates, numstates];

            }
        }
        public void read_input(string file)
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
                
                }                          }

        }
        public void value_iter()
        {
            for (int i = 0; i < numstates; i++)
            {
                
                    Jest[i] = 0.0;
            }
            for (int i = 0; i < numstates; i++)
            {
                Jest[i] = StateDict["s" + (i+1)];
            }
            

        }
        public void Bellman()
        {
            int statenum;
            double[] estimate =new double[numactions];
            double[] maxestimate = new double[numstates];
            int[] maxestindex=new int[numstates];
            for (int counter = 0; counter < 20; counter++)
            {
                Console.WriteLine("After iteration " + counter+1);
                foreach (var item in StateDict)
                {
                    Match state = Regex.Match(item.Key, @"\d");
                    statenum = Convert.ToInt32(state.Value);
                    for (int j = 0; j < numactions; j++)
                    {
                        for (int i = 0; i < numstates; i++)
                        {
                            if (transitionMatrix[j][statenum - 1, i] != null)
                                estimate[j] += transitionMatrix[j][statenum - 1, i]["a" + (j + 1)] * Jest[i];
                        }
                    }
                    
                    maxestimate[statenum - 1] = item.Value + gamma*estimate.Max();
                    maxestindex[statenum - 1] = estimate.ToList().IndexOf(estimate.Max()) + 1;
                    Console.Write("(" + item.Key + " " + "a" + maxestindex[statenum - 1] + " " + maxestimate[statenum - 1] + ") ");
                }
                Console.WriteLine();
                for (int i = 0; i < Jest.Length; i++)
                {
                    Jest[i] = maxestimate[i];
                }
            }
        }
        static void Main(string[] args)
        {
            string input = @"c:\users\ap\documents\visual studio 2013\Projects\MDP\MDP\test-win.in";
            MDP mdp = new MDP(4,2,0.5);
            mdp.read_input(input);
            mdp.value_iter();
            mdp.Bellman();
            Console.ReadKey();
        }
  
            
    }
}
