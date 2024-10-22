using System;
using System.IO;

namespace СonsolePrograms {
    public class Program {
        static void Main(string[] args) {
            //Task1();
            //Task2();
            Task3();

        }

        private static void Task1() {
            Console.WriteLine("--Task 1--");
            /*Рассмотрим три числа a, b и c. Упорядочим их по возрастанию.
             Какое число будет стоять между двумя другими?*/
            
            int[] a = new int[3];
            Console.Write("Введите три числа в строку через пробел: ");
            string stroka = Console.ReadLine();
            string[] elements = stroka.Split(" ");

            if (elements.Length != 3) {
                Console.WriteLine("Введено неверное количесвто чисел");
                return;
            }
            
            for (int i = 0; i < a.Length; i++) {
                a[i] = Convert.ToInt32(elements[i]);
            }

            Array.Sort(a);
            Console.WriteLine(a[1]);
        }

        private static void Task2() {
            Console.WriteLine("--Task 2--");
            /*Задан массив a размера n. Необходимо посчитать количество уникальных
             элементов в данном массиве. Элемент называется уникальным, 
             если встречается в массиве ровно один раз.*/
            
            Console.WriteLine("Введите размер массива: ");
            int n = Convert.ToInt32(Console.ReadLine());
            int[] a = new int[n];
            
            Console.WriteLine("Введите элементы массива в строчку через пробел");
            string stroka = Console.ReadLine();
            string[] elements = stroka.Split(" ");

            if (elements.Length != n) {
                Console.WriteLine("Введено неверное количесвто чисел");
                return;
            }
            
            for (int i = 0; i < a.Length; i++) {
                a[i] = Convert.ToInt32(elements[i]);
            }

            Dictionary<int, int> dict = new Dictionary<int, int>();
            for (int i = 0; i < a.Length; i++) {
                if (dict.ContainsKey(a[i])) {
                    dict[a[i]] += 1;
                }
                else {
                    dict.Add(a[i], 1);
                }
            }

            int count = 0;

            foreach (var i in dict) {
                if (i.Value == 1) {
                    count++;
                }
            }
            
            Console.WriteLine(count);

        }

        private static void Task3() {
            Console.WriteLine("--Task3--");
            Console.WriteLine("Введите текст: ");
            string str = Console.ReadLine();
            using(FileStream file = new FileStream("info.txt", FileMode.OpenOrCreate)) {
                byte[] array = System.Text.Encoding.Default.GetBytes(str);
                file.Write(array, 0, array.Length);

            }

            using (FileStream file1 = File.OpenRead("info.txt")) {
                byte[] array = new byte[file1.Length];
                file1.Read(array, 0, array.Length);
                string strForFile = System.Text.Encoding.Default.GetString(array);
                Console.WriteLine(strForFile);
            }
        }
        
        
        
    }
}

