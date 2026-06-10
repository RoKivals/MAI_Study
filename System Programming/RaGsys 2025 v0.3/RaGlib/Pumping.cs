using System;
using System.Collections;
using System.Linq;

namespace RaGlib {
    public class Pumping
    {
        public void Dialog()
        {
            var key = "1";
            while (key == "1" || key == "2")
            {
                Console.WriteLine("Введите:");
                Console.WriteLine("1 - Показать пример.");
                Console.WriteLine("2 - Проверить цепочку.");
                Console.WriteLine("3 - Завершить.");
                key = Console.ReadLine();
                if (key == "1")
                {
                    Console.WriteLine();
                    Example();
                    Console.WriteLine();
                }
                if (key == "2")
                {
                    Console.Write("\nВведите строчку для проверки: ");
                    Check(new ArrayList(Console.ReadLine().ToCharArray().Select(c => c.ToString()).ToArray()));
                }
            }
        }
        private bool Check_Repetition(ArrayList y)
        {
            int k = 0;
            ArrayList pattern = new ArrayList() { y[k] };
            while (k != y.Count - 1)
            {
                if (y.Count % pattern.Count != 0)
                {
                    k++;
                    pattern.Add(y[k]);
                    continue;
                }
                bool fl = true;
                for (int i = 0; i < y.Count; i++)
                {
                    if (!y[i].Equals(pattern[i % pattern.Count]))
                    {
                        fl = false;
                        break;
                    }
                }
                if (fl) return true;
                k++;
                pattern.Add(y[k]);
            }
            return false;
        }//Check_Repetition

        private string Find_Repetition(ArrayList y)
        {
            if (y.Count == 0 || y.Count == 1) return "";
            int k = 0;
            ArrayList pattern = new ArrayList() { y[k] };
            while (k != y.Count - 1)
            {
                if (y.Count % pattern.Count != 0)
                {
                    k++;
                    pattern.Add(y[k]);
                    continue;
                }
                bool fl = true;
                for (int i = 0; i < y.Count; i++)
                {
                    if (!y[i].Equals(pattern[i % pattern.Count]))
                    {
                        fl = false;
                        break;
                    }
                }
                if (fl) return String.Join("", pattern.ToArray());
                k++;
                pattern.Add(y[k]);
            }
            return "";
        }//end Find_Repetition

        public void Check(ArrayList w)
        {
            Console.WriteLine("Проверить цепочку:");
            Console.WriteLine("1 - на принадлежность регулярному языку");
            Console.WriteLine("2 - на принадлежность кс-языку");
            Console.WriteLine("3 - найти все повторения");
            Console.Write("Введите 1 или 2 или 3: ");
            var key = Console.ReadLine();
            Console.WriteLine();
            if (!(key == "1" || key == "2" || key == "3"))
            {
                Console.WriteLine("Неверно введенные данные.");
                return;
            }

            Console.WriteLine("Для цепочки: " + String.Join("", w.ToArray()));
            ArrayList L = new ArrayList();
            ArrayList L_All = new ArrayList();
            for (int p = 1; p < w.Count; p++)
            {
                for (int ind1 = 0; ind1 < w.Count + 1; ind1++)
                {
                    for (int ind2 = ind1; ind2 < w.Count + 1; ind2++)
                    {
                        for (int ind3 = ind2; ind3 < w.Count + 1; ind3++)
                        {
                            for (int ind4 = ind3; ind4 < w.Count + 1; ind4++)
                            {

                                ArrayList u = new ArrayList();
                                ArrayList x = new ArrayList();
                                ArrayList v = new ArrayList();  // 
                                ArrayList y = new ArrayList();
                                ArrayList z = new ArrayList();

                                if (ind2 - ind1 == 0 && ind4 - ind3 == 0) continue;
                                for (int k = 0; k < w.Count; k++)
                                {
                                    if (k < ind1) u.Add(w[k]);
                                    if (k >= ind1 && k < ind2) x.Add(w[k]); 
                                    if (k >= ind2 && k < ind3) v.Add(w[k]); // 
                                    if (k >= ind3 && k < ind4) y.Add(w[k]); 
                                    if (k >= ind4) z.Add(w[k]);
                                }
                                if (key == "1" && x.Count > 1 && Find_Repetition(x) != "" && Find_Repetition(x).Length == 1)
                                {
                                    if (!Contains1(L, new ArrayList() { u, x, v, y, z }))
                                        L.Add(new ArrayList() { u, x, v, y, z });
                                }
                                if (key == "2" && x.Count > 1 && y.Count > 1 &&
                                    Check_Repetition(x) && Check_Repetition(y))
                                {
                                    if (!Contains2(L, new ArrayList() { u, x, v, y, z }))
                                        L.Add(new ArrayList() { u, x, v, y, z });
                                }
                                if ((u.Count > 1 && Check_Repetition(u)) ||
                                    (x.Count > 1 && Check_Repetition(x)) ||
                                    (v.Count > 1 && Check_Repetition(v)) ||
                                    (y.Count > 1 && Check_Repetition(y)) ||
                                    (z.Count > 1 && Check_Repetition(z)))
                                {
                                    if (!Contains3(L_All, new ArrayList() { u, x, v, y, z }))
                                        L_All.Add(new ArrayList() { u, x, v, y, z });
                                }

                            }
                        }
                    }
                }
            }
            if (L.Count != 0 && (key == "1" || key == "2"))
            {
                if (key == "1") Print_Regular((ArrayList)L[0]);
                if (key == "2") Print_Ks((ArrayList)L[0]);

                if (key == "1") Console.WriteLine("Цепочка принадлежит регулярному языку");
                if (key == "2") Console.WriteLine("Цепочка принадлежит кс-языку");

                Console.Write("Вывести все повторения? (1 - да, 0 - нет): ");
                var key2 = Console.ReadLine();
                Console.WriteLine();
                if (key2 == "1")
                {
                    Print(L_All);
                }
            }
            else
            {
                if (key == "1") Console.WriteLine("Цепочка не принадлежит регулярному языку.");
                if (key == "2") Console.WriteLine("Цепочка не принадлежит кс-языку.");

            }
            if (key == "3" && L_All.Count != 0)
            {
                Print(L_All);
            }
            else
            {
                if (key == "3") Console.WriteLine("Нет повторений.");
            }
        }//end Check

        private void Print_Regular(ArrayList res)
        {
            Console.Write(String.Join("", ((ArrayList)res[0]).ToArray()) + " ");
            string repetition = Find_Repetition((ArrayList)res[1]);
            Console.Write(repetition + "^" + ((ArrayList)res[1]).Count / repetition.Length + " ");
            Console.Write(String.Join("", ((ArrayList)res[2]).ToArray()));
            Console.Write(String.Join("", ((ArrayList)res[3]).ToArray()));
            Console.Write(String.Join("", ((ArrayList)res[4]).ToArray()));
            Console.WriteLine();
        }//end Print_Regular

        private void Print_Ks(ArrayList res)
        {
            Console.Write(String.Join("", ((ArrayList)res[0]).ToArray()) + " ");
            string repetition = Find_Repetition((ArrayList)res[1]);
            Console.Write(repetition + "^" + ((ArrayList)res[1]).Count / repetition.Length + " ");
            Console.Write(String.Join("", ((ArrayList)res[2]).ToArray()) + " ");
            string repetition1 = Find_Repetition((ArrayList)res[3]);
            Console.Write(repetition1 + "^" + ((ArrayList)res[3]).Count / repetition1.Length + " ");
            Console.Write(String.Join("", ((ArrayList)res[4]).ToArray()));
            Console.WriteLine();
        }//end Print_Ks

        private void Print(ArrayList L)
        {
            ArrayList F = new ArrayList();
            foreach (ArrayList l in L)
            {
                string res = "";
                foreach (ArrayList r in l)
                {
                    string rep = Find_Repetition(r);
                    if (rep == "") res += String.Join("", r.ToArray());
                    else res += " " + rep + "^" + r.Count / rep.Length + " ";

                }
                if (!F.Contains(res)) Console.WriteLine(res);
                F.Add(res);
            }

        }//end Print

        private bool Contains1(ArrayList L, ArrayList w)
        {
            foreach (ArrayList l in L)
            {
                if (String.Join("", ((ArrayList)l[1]).ToArray()) == String.Join("", ((ArrayList)w[1]).ToArray()))
                {
                    return true;
                }
            }
            return false;
        }//end Contains1

        private bool Contains2(ArrayList L, ArrayList w)
        {
            if (Find_Repetition((ArrayList)w[1]) == Find_Repetition((ArrayList)w[3]))
            {
                if (((ArrayList)w[2]).Count == 0) return true;
                if (Find_Repetition((ArrayList)w[1]) == Find_Repetition((ArrayList)w[2])) return true;
                if (((ArrayList)w[2]).Count == 1 && Find_Repetition((ArrayList)w[1]).Equals(((ArrayList)w[2])[0])) return true;
                if (Find_Repetition((ArrayList)w[1]).Equals(((ArrayList)w[2]).ToString())) return true;
            }
            foreach (ArrayList l in L)
            {
                if (Find_Repetition((ArrayList)l[1]) == Find_Repetition((ArrayList)l[1]) &&
                    Find_Repetition((ArrayList)l[3]) == Find_Repetition((ArrayList)l[3]))
                {
                    return true;
                }
            }
            return false;
        } //end Contains2

        private bool Contains3(ArrayList L, ArrayList w)
        {
            foreach (ArrayList l in L)
            {
                if (Find_Repetition((ArrayList)l[0]) == Find_Repetition((ArrayList)w[0]) &&
                    Find_Repetition((ArrayList)l[1]) == Find_Repetition((ArrayList)w[1]) &&
                    Find_Repetition((ArrayList)l[2]) == Find_Repetition((ArrayList)w[2]) &&
                    Find_Repetition((ArrayList)l[3]) == Find_Repetition((ArrayList)w[3]) &&
                    Find_Repetition((ArrayList)l[4]) == Find_Repetition((ArrayList)w[4]))
                {
                    return true;
                }
            }
            return false;
        }//end Contains3

        public void Example()
        {
            ArrayList w = new ArrayList() { "d", "h", "d", "h", "d", "h", "d", "h", "g", "d", "d", "d", "d", "g", "h", "d", "g", "h", "d" };
            int key = 2;
            Console.WriteLine("Для цепочки: " + String.Join("", w.ToArray()));
            ArrayList L = new ArrayList();
            ArrayList L_All = new ArrayList();
            for (int p = 1; p < w.Count; p++)
            {
                for (int ind1 = 0; ind1 < w.Count + 1; ind1++)
                {
                    for (int ind2 = ind1; ind2 < w.Count + 1; ind2++)
                    {
                        for (int ind3 = ind2; ind3 < w.Count + 1; ind3++)
                        {
                            for (int ind4 = ind3; ind4 < w.Count + 1; ind4++)
                            {

                                ArrayList u = new ArrayList();
                                ArrayList x = new ArrayList();
                                ArrayList v = new ArrayList();
                                ArrayList y = new ArrayList();
                                ArrayList z = new ArrayList();

                                if (ind2 - ind1 == 0 && ind4 - ind3 == 0) continue;
                                for (int k = 0; k < w.Count; k++)
                                {
                                    if (k < ind1) u.Add(w[k]);
                                    if (k >= ind1 && k < ind2) x.Add(w[k]);
                                    if (k >= ind2 && k < ind3) v.Add(w[k]);
                                    if (k >= ind3 && k < ind4) y.Add(w[k]);
                                    if (k >= ind4) z.Add(w[k]);
                                }
                                if (key == 1 && x.Count > 1 && Find_Repetition(x) != "")
                                {
                                    if (!Contains1(L, new ArrayList() { u, x, v, y, z }))
                                        L.Add(new ArrayList() { u, x, v, y, z });
                                }
                                if (key == 2 && x.Count > 1 && y.Count > 1 &&
                                    Check_Repetition(x) && Check_Repetition(y))
                                {
                                    if (!Contains2(L, new ArrayList() { u, x, v, y, z }))
                                        L.Add(new ArrayList() { u, x, v, y, z });
                                }
                                if ((u.Count > 1 && Check_Repetition(u)) ||
                                    (x.Count > 1 && Check_Repetition(x)) ||
                                    (v.Count > 1 && Check_Repetition(v)) ||
                                    (y.Count > 1 && Check_Repetition(y)) ||
                                    (z.Count > 1 && Check_Repetition(z)))
                                {
                                    if (!Contains3(L_All, new ArrayList() { u, x, v, y, z }))
                                        L_All.Add(new ArrayList() { u, x, v, y, z });
                                }

                            }
                        }
                    }
                }
            }

            if (L.Count != 0 && (key == 1 || key == 2))
            {
                if (key == 1) Print_Regular((ArrayList)L[0]);
                if (key == 2) Print_Ks((ArrayList)L[0]);

                if (key == 1) Console.WriteLine("Цепочка принадлежит регулярному языку");
                if (key == 2) Console.WriteLine("Цепочка принадлежит кс-языку");

                Console.WriteLine("Все повторения:");
                Print(L_All);
            }
        }

    }//end Pumping
}//end namespace MPTranslator