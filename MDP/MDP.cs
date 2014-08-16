/*------------------------------------------------------------------------
# MACHINE LEARNING - IMPLEMENTATION OF MARKOV DECISION PROCESS
#-------------------------------------------------------------------------------
# START DATE: 3/31/2014
# END DATE: 4/07/2014
#-------------------------------------------------------------------------------
# NAME : Pradeep Anatharaman(pxa130130)
#-------------------------------------------------------------------------------
#----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
#  Functions-               | 																																					         |
# intput and output         |                                     Description                                                                                                	         |
#----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
#| read_input        :      |    Splits the input files line by line. Stores the State and rewards as a dictionary ( mapping from states to rewards).                                    |
#| input             :      |    file: the input file containing the states, its reward and sequence of states for each action.                                                          |
#| output            :      |    no value returned. It reads the input file and sets the Transition probability using jagged array.                                                      |
#|                          |                                                                                                                                                            |
#| init              :      |    Sets up the J*() array to initial values to 0.                                                                                                          |
#| input             :      |    NA                                                                                                                                                      |
#| output            :      |    no value returned. sets up J* array for policy computation using Bellman equation                                                                       |
#|                          |                                                                                                                                                            |
#| value_iter        :      |    Uses Bellman eqauation and prints out the optimal policy.                                                                                               |
#| Storage structure :		|	 StateDict :  Dictionary that maps states to its rewards.                                                                                                |
#|              			|	 transitionMatrix: Jagged array of Dictionaries. The first dimension specifies the action and the 2d array stores                                        |
#|                          |      each element as array of dictionaries of mapping from action to its probability. The 2d array can be interpreted as                                   |
#|                          |      states to states mapping. i.e for the 1st row, s1 is the current state and the columns 0,1,2.. represent states 1,2,3..                               |
#|                          |       and each element specifies the probability for transition from s1 to the other states for a specified action.                                        |
#|							|	 ActionProbability: Array of dictionaries with maximum dimensions as the number of actions and maps actions to its probability                           |
#|                          |        and is stored as an element in the transitionMatrix.                                                                                                |
#|							|	 Jest: Single dimension array of double values for storing the J*() values to compute optimal policy.                                                    |
#|							|	 Q: Hash map that stores and maps time t to each state.                                                                                                  |
#|							|	 O: List to maintain current observation sequence																										 |
#|							|	 argmax: stores greatest Viterbi probabilities from earlier states                                                                                       |
#| **********************************Constructor initialization******************************                                                                                            | 
#| input             :      |    Numstates: number of states that was provided by the user. Numactions: number of actions provided by the user. Gamma: The gamma value specified by user |
#| Initialization    :      |    the user inputs are initialized to the corresponding class fields.Statedict, transitionMatrix and ActionProbability dictionary/arrays are initialized   |    											                                             |
#----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MDP
{
    class MDP
    {
        #region Global fields
        public int numstates { get; set; }
        public int numactions { get; set; }
        public double gamma { get; set; }
        public Dictionary<string, double> StateDict;
        public Dictionary<string, double>[][,] transitionMatrix;
        public Dictionary<string, double>[] ActionProbability;
        public double[] Jest;
        #endregion


        #region Constructor initialization
        public MDP(int numstates, int numactions, double gamma)
        {
            this.numstates = numstates;
            this.numactions = numactions;
            this.gamma = gamma;
            StateDict = new Dictionary<string, double>();
            transitionMatrix = new Dictionary<string, double>[numactions][,];
            ActionProbability = new Dictionary<string, double>[numactions];
            Jest = new double[numstates];
            for (int i = 0; i < numactions; i++)
            {
                transitionMatrix[i] = new Dictionary<string, double>[numstates, numstates];

            }
        }

        #endregion

        public void read_input(string file)
        {
            string input;
            string[] inputlines;

            try
            {
                var sr = new StreamReader(file);
                input = sr.ReadToEnd();
                inputlines = input.Split('\n');
                foreach (var item in inputlines)
                {
                    IEnumerable<string> sequence = Regex.Split(item, @" ").Take(2);
                    StateDict.Add(sequence.First(), Convert.ToDouble(sequence.ElementAt(1)));

                    //get the states, actions and the transition probabilities

                    MatchCollection mc = Regex.Matches(item, @"[a-z][1-9]([0-9]?)*|(\d*\.+\d+)|\d+");
                    string[] matches = new string[mc.Count];
                    int i = 0;
                    foreach (Match m in mc)
                    {
                        matches[i++] = m.Value;
                        
                    }

                    //store each mapping of actions to its probability in the corresponding state transition element 
                    for (i = 2; i < matches.Length; i += 3)
                    {

                        ActionProbability[Convert.ToInt32(matches[i].Substring(1)) - 1] = new Dictionary<string, double>() { { matches[i], Convert.ToDouble(matches[i + 2]) } };

                        transitionMatrix[Convert.ToInt32(matches[i].Substring(1)) - 1][Convert.ToInt32(matches[0].Substring(1)) - 1, Convert.ToInt32(matches[i + 1].Substring(1)) - 1] = ActionProbability[Convert.ToInt32(matches[i].Substring(1)) - 1];

                    }

                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Error: " + ex + "\n\nInput file not found: Check if the file name is correct and if the file is present in the same folder as the executable file. \nexitting..");
                throw;
            }
            catch (FormatException ex)
            {

                Console.WriteLine("Error: " + ex + "\n\nThe input file has not been provided in the expected format. \nexitting..");
                throw;
            }
            catch(IndexOutOfRangeException ex)
            {
                Console.WriteLine("Error: " + ex + "\n\n Out of range exception error: Check if the number of states/actions and other parameters match the ones given as parameters. \nexitting..");
                throw;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex + "\n\nA general exception has been caught. Try rerunning the program . \n exitting");
                throw;
            }

        }



        public void init()
        {
            for (int i = 0; i < numstates; i++)
            {

                Jest[i] = 0.0;
            }



        }



        public void value_iter()
        {
            int statenum;
            double zero = 0;
            bool actionexist = false;
            double[] estimate = new double[numactions];
            double[] maxestimate = new double[numstates];
            int maxestindex;

            //start iteration
            for (int counter = 0; counter < 20; counter++)
            {
                Console.WriteLine("After iteration " + (counter + 1));

                /* For each of the given states, calculate the J value for each action at every transition
                 * to another state. Find the maximum of the estimates among the actions taken and also 
                 * the action corresponding to the maximum estimate. This gives the optimum policy for 
                 * each state.
                */
                foreach (var item in StateDict)
                {
                    Match state = Regex.Match(item.Key, @"\d+");
                    statenum = Convert.ToInt32(state.Value);
                    //find estimates for each action at every transition
                    for (int j = 0; j < numactions; j++)
                    {
                        for (int i = 0; i < numstates; i++)
                        {
                            if (transitionMatrix[j][statenum - 1, i] != null)
                            {
                                actionexist = true;
                                estimate[j] += transitionMatrix[j][statenum - 1, i]["a" + (j + 1)] * Jest[i];   //maximizing term of the Bellman's equation (max over k actions( P(i,j,k) * Jest(Sj)))
                            }
                        }
                        if (actionexist == false)
                            estimate[j] = -1 / zero;
                        actionexist = false;
                    }
                    //get the maximum of the estimate among the actions. reward and discount factor(gamma) is 
                    //used after calculating estimates as they are independent in finding the maximum estimate
                    maxestimate[statenum - 1] = item.Value + gamma * estimate.Max();
                    maxestindex = estimate.ToList().IndexOf(estimate.Max()) + 1;
                    Console.Write("(" + item.Key + " " + "a" + maxestindex + " " + maxestimate[statenum - 1] + ") ");
                    for (int i = 0; i < estimate.Length; i++)
                    {
                        estimate[i] = 0.0;
                    }
                }
                Console.WriteLine();
                if (counter == 0)
                {
                    //initialize J* array
                    for (int i = 0; i < numstates; i++)
                    {
                        Jest[i] = StateDict["s" + (i + 1)];

                    }

                }
                else
                {
                    for (int i = 0; i < Jest.Length; i++)
                    {
                        Jest[i] = maxestimate[i];
                    }
                }
            }
        }


        static void Main(string[] args)
        {

            Console.WriteLine("Enter number of states[space] number of actions[space] input file[space] and discount factor(gamma)");
            string[] arguments = Regex.Split(Console.ReadLine(), @" ");
            try
            {
                string input = arguments[2];
                MDP mdp = new MDP(Convert.ToInt32(arguments[0]), Convert.ToInt32(arguments[1]), Convert.ToDouble(arguments[3]));

                //string input = @"c:\users\ap\documents\visual studio 2013\Projects\MDP\MDP\test2-wi.in";
                //MDP mdp = new MDP(10,4, .9);
                mdp.read_input(input);
                mdp.init();
                mdp.value_iter();
            }
            catch(IndexOutOfRangeException ex)
            {
                Console.WriteLine("Error: "  + ex+ "\n\n Check if all the 4 parameters were given. Rerun and try giving the 4 parameters.\n exitting...");
                throw;
            }
            catch (FormatException ex)
            {

                Console.WriteLine("Error: " + ex + "\n\n The input arguments have not been provided in the expected format,Rerun and try giving the 4 parameters in correct format.\n exitting... ");
                throw;
            }
            
            Console.WriteLine("\n Press any key to continue...");
            Console.ReadKey();
        }


    }
}
