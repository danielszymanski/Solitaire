using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Solitaire
{
    class Program
    {
        static private void SaveResult(string result)
        {
            if (!File.Exists("Results.txt"))
            {
                StreamWriter sw = File.CreateText("Results.txt");

                sw.WriteLine("Time|Moves|Searched states|Created states\n");
                                        
                sw.WriteLine(result);
                sw.Close();
            }
            else
            {
                StreamWriter sw = File.AppendText("Results.txt");

                sw.WriteLine(result);
                sw.Close();
            }
        }
        
        static void Main(string[] args)
        {
            int atempts = 1;

            Console.Write("How many attempts: ");
            try
            {atempts = Int32.Parse(Console.ReadLine());}
            catch { Console.WriteLine("Wrong input format - one attempt will be executed"); Console.ReadLine(); }

            int numberOfHeuristics = 2;

            for (int iterations = 0; iterations < atempts; iterations++)
            {

                System.TimeSpan[] timey = new System.TimeSpan[numberOfHeuristics];
                int[] moves = new int[numberOfHeuristics]; ;
                int[] searchedStates = new int[numberOfHeuristics];
                int[] createdStates = new int[numberOfHeuristics];

                int solved = 0;

                Board p = new Board();

                for (int h = 0; h < numberOfHeuristics; h++)
                {
                    List<Board> Open = new List<Board>();
                    HashSet<string> Closed = new HashSet<string>();


                    Console.WriteLine("Heuristic {0}\n", h + 1);
                    Console.WriteLine("Starting state: \n");
                    p.Draw();
                    Console.WriteLine("\n\nPress Enter to begin searching...");
                    //Console.ReadLine();

                    Stopwatch sw = new Stopwatch();             //Włączenie stopera
                    sw.Start();


                    Open.Add(p);

                    while (Open.Any())
                    {
                        Parallel.ForEach<Board>(Open, x =>
                        {
                            if (h == 0)
                                x.CountHeuristic1();
                            if (h == 1)
                                x.CountHeuristic2();
                        });

                        var OpenSorted = Open.OrderBy(o => o.heuristic);            // Ustawienie wg heurystyki

                        p = OpenSorted.First();
                        p.availableMoves.Clear();

                        // Console.ReadLine();
                        //System.Threading.Thread.Sleep(400);

                        for (int i = 0; i < 8; i++)
                        {
                            Console.Clear();
                            int index_time = 1;
                            int index_moves = 1;
                            int index_states1 = 1;
                            int index_states2 = 1;

                            foreach (System.TimeSpan time in timey)
                            {
                                Console.WriteLine("Time {0}: {1}", index_time, time);
                                index_time++;
                            }

                            Console.WriteLine();

                            foreach (int m in moves)
                            {
                                Console.WriteLine("Moves {0}: {1}", index_moves, m);
                                index_moves++;
                            }

                            Console.WriteLine();

                            foreach (int states in searchedStates)
                            {
                                Console.WriteLine("Searched states {0}: {1}", index_states1, states);
                                index_states1++;
                            }

                            Console.WriteLine();

                            foreach (int states in createdStates)
                            {
                                Console.WriteLine("Created states {0}: {1}", index_states2, states);
                                index_states2++;
                            }

                            Console.WriteLine("===========================================================================\n");
                            p.selection = i;
                            p.Draw();
                        }

                        Board[] childs = new Board[p.availableMoves.Count()];
                        bool[] toAdd = new bool[p.availableMoves.Count()];

                        Closed.Add(p.ToString());
                        searchedStates[h]++;

                        for (int i = 0; i < p.availableMoves.Count(); i++)
                        {
                            Board copy = new Board(ref p);
                            Board.Move(ref copy, p.availableMoves[i][0], p.availableMoves[i][1], p.availableMoves[i][2]);

                            bool add = true;

                            Parallel.ForEach<Board>(Open, x =>
                            {
                                if (x.hash.Equals(copy.hash))
                                    add = false;
                            });

                            if (Closed.Contains(copy.hash))
                                add = false;

                            toAdd[i] = add;
                            childs[i] = copy;
                        }

                        for (int i = 0; i < p.availableMoves.Count(); i++)
                        {
                            if (toAdd[i])
                                Open.Add(childs[i]);
                            createdStates[h]++;
                        }

                        if (p.heuristic == 0)
                        {
                            Console.WriteLine("Solution found!\n");
                            solved = 1;
                            break;
                        }

                        Closed.Add(p.ToString());
                        Open.Remove(p);
                    }


                    if (solved == 1)
                    {
                        sw.Stop();                              //Wyłączenie stopera
                        var Path = new List<Board>();

                        while (p.parent != null)
                        {
                            Path.Add(p);
                            p = p.parent;
                        }


                        Console.Write("Searching time: ");
                        Console.WriteLine("{0}", timey[h] = sw.Elapsed);

                        Console.Write("Number of moves: ");
                        Console.WriteLine(moves[h] = Path.Count());

                        Console.Write("States searched: ");
                        Console.WriteLine(searchedStates[h]);

                        Console.Write("States created: ");
                        Console.WriteLine(createdStates[h]);

                        
                        Console.WriteLine("Show solution...");
                        Console.ReadLine();

                        for (int i = Path.Count() - 1; i >= 0; i--)
                        {
                            Console.Clear();

                            Path[i].Draw();
                            Console.ReadLine();
                        }
                        
                    }

                    Console.WriteLine("The End");
                    Console.ReadLine();
                    Console.Clear();

                }

                Console.WriteLine("FINAL RESULT:\n\n");

                System.TimeSpan fastestTime = System.TimeSpan.MaxValue;
                int lowest = int.MaxValue, iterationsMin = int.MaxValue, createdMin = int.MaxValue;

                int fastestHeuristic = 0, shortest = 0, iterationsBest = 0, createdBest = 0;

                if (solved == 1)
                {
                    StringBuilder myStringBuilder = new StringBuilder();

                    for (int i = 0; i < numberOfHeuristics; i++)
                    {
                        Console.WriteLine("Heuristic {0}\n", i + 1);

                        Console.WriteLine("Time: {0}", timey[i]);
                        Console.WriteLine("Moves: {0}", moves[i]);
                        Console.WriteLine("Searched states: {0}", searchedStates[i]);
                        Console.WriteLine("Created states: {0}\n\n", createdStates[i]);


                        myStringBuilder.Append(timey[i]);
                        myStringBuilder.Append(" ");
                        myStringBuilder.Append(moves[i]);
                        myStringBuilder.Append(" ");
                        myStringBuilder.Append(searchedStates[i]);
                        myStringBuilder.Append(" ");
                        myStringBuilder.Append(createdStates[i]);
                        myStringBuilder.Append(" ");
                        myStringBuilder.Append("\t");

                        if (timey[i] < fastestTime)
                        {
                            fastestTime = timey[i];
                            fastestHeuristic = i;
                        }

                        if (moves[i] < lowest)
                        {
                            lowest = moves[i];
                            shortest = i;
                        }


                        if (searchedStates[i] < iterationsMin)
                        {
                            iterationsMin = searchedStates[i];
                            iterationsBest = i;
                        }

                        if (createdStates[i] < createdMin)
                        {
                            createdMin = createdStates[i];
                            createdBest = i;
                        }
                    }

                    Console.WriteLine("Fastest heuristic (time): Heuristic {0} - {1}", fastestHeuristic + 1, fastestTime);
                    Console.WriteLine("Lowest amount of searched states: Heuristic {0} - {1} moves", shortest + 1, lowest);
                    Console.WriteLine("Lowest amount of created states: Heuristic {0} - {1} states", createdBest + 1, createdMin);
                    Console.WriteLine("Best solution: Heurisitc {0} - {1} moves", iterationsBest + 1, iterationsMin);

                    SaveResult(myStringBuilder.ToString());
                }

                else
                {
                    Console.WriteLine("Unable to find a solution");
                    SaveResult("Unable to find a solution");
                }
            }
            Console.ReadLine();
        }
    }
}
