using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Collections {

    class Program {
        static void Main(string[] args) {
            //ArrayList();
            //List();
            //HashSet();
            //Dictionary();
            ValueTuple();
        }

        private static void ArrayList() {
            ArrayList arrayList = new ArrayList();
            Console.WriteLine("Количество элементов в динамическом массиве: {0}", arrayList.Count);
            Console.WriteLine("Емкость динамического массива: {0}", arrayList.Capacity);
            
            arrayList.Add(1);
            arrayList.Add("hello");
            arrayList.Add('c');
            Console.WriteLine("Количество элементов в динамическом массиве: {0}", arrayList.Count);
            Console.WriteLine("Емкость динамического массива: {0}", arrayList.Capacity);
            
            arrayList.Add(1);
            arrayList.Add(2);
            Console.WriteLine("Количество элементов в динамическом массиве: {0}", arrayList.Count);
            Console.WriteLine("Емкость динамического массива: {0}", arrayList.Capacity);

            foreach (object i in arrayList) {
                Console.Write(i + " ");
            }
            Console.WriteLine();

            arrayList.Remove(1);
            foreach (object i in arrayList) {
                Console.Write(i + " ");
            }
            Console.WriteLine();
            
            arrayList.RemoveAt(1);
            foreach (object i in arrayList) {
                Console.Write(i + " ");
            }
            Console.WriteLine();
            
            Console.WriteLine("Количество элементов в динамическом массиве: {0}", arrayList.Count);
            Console.WriteLine("Емкость динамического массива: {0}", arrayList.Capacity);
        }
        
        private static void List() {
            List<int> list = new List<int>();
            list.Add(101);
            list.Add(10);
            list.Add(5);
            foreach (int i in list) {
                Console.Write(i + " ");
            }
            Console.WriteLine();
            Console.WriteLine("Количество элементов в списке: {0}", list.Count);
            Console.WriteLine("Емкость списка: {0}", list.Capacity);
            
            list.Add(4);
            list.Add(5);
            foreach (int i in list) {
                Console.Write(i + " ");
            }
            Console.WriteLine();
            Console.WriteLine("Количество элементов в списке: {0}", list.Count);
            Console.WriteLine("Емкость списка: {0}", list.Capacity);
            
            List<int>.Enumerator it = list.GetEnumerator(); 
            while(it.MoveNext()) 
            {
                Console.WriteLine(it.Current); 
            }
            
            
            List<int> list2 = new List<int>(){1, 2, 3, 4, 824};
            list.AddRange(list2);
            foreach (int i in list) {
                Console.Write(i + " ");
            }
            Console.WriteLine();
            Console.WriteLine("Количество элементов в списке: {0}", list.Count);
            Console.WriteLine("Емкость списка: {0}", list.Capacity);
            
            list.Insert(0, 1212);
            foreach (int i in list) {
                Console.Write(i + " ");
            }
            Console.WriteLine();
            Console.WriteLine("Количество элементов в списке: {0}", list.Count);
            Console.WriteLine("Емкость списка: {0}", list.Capacity);

            list.InsertRange(1, list2);
            foreach (int i in list) {
                Console.Write(i + " ");
            }
            Console.WriteLine();
            Console.WriteLine("Количество элементов в списке: {0}", list.Count);
            Console.WriteLine("Емкость списка: {0}", list.Capacity);
            
            list.Remove(5);
            list.RemoveAt(3);
            foreach (int i in list) {
                Console.Write(i + " ");
            }
            Console.WriteLine();
            Console.WriteLine("Количество элементов в списке: {0}", list.Count);
            Console.WriteLine("Емкость списка: {0}", list.Capacity);
            
            list.Sort();
            foreach (int i in list) {
                Console.Write(i + " ");
            }
            Console.WriteLine();

            list.Reverse();
            foreach (int i in list) {
                Console.Write(i + " ");
            }
            Console.WriteLine();
        }

        private static void HashSet() {
            HashSet<int> hashSet = new HashSet<int> { 1, 2, 2, 3, 4, 4, 4, 5, 6, 6 };
            foreach (int i in hashSet) {
                Console.Write(i + " ");
            }
            Console.WriteLine();

            hashSet.Add(0);
            hashSet.Add(101);
            hashSet.Add(100);
            foreach (int i in hashSet) {
                Console.Write(i + " ");
            }
            Console.WriteLine();

            hashSet.Remove(2);
            foreach (int i in hashSet) {
                Console.Write(i + " ");
            }
            Console.WriteLine();
            
            if (hashSet.Contains(2)) {
                Console.WriteLine("2 is in hashSet");
            }
            else {
                Console.WriteLine("2 is not in hashSet");
            }

        }

        private static void Dictionary() {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            dictionary.Add(1, "one");
            dictionary.Add(2, "two");
            dictionary.Add(3, "three");
            dictionary.Add(4, "four");

            dictionary.TryAdd(1, "oneNew");

            foreach (KeyValuePair<int, string> kvp in dictionary) {
                Console.WriteLine(kvp.Key + " " + kvp.Value);
            }
            Console.WriteLine();
            
            dictionary.Remove(1);
            foreach (KeyValuePair<int, string> kvp in dictionary) {
                Console.WriteLine(kvp.Key + " " + kvp.Value);
            }
            Console.WriteLine();
            
            Console.WriteLine(dictionary[2]);

        }

        private static void ValueTuple() {
            (string, int) p1 = ("John", 21);
            (string Name, int Age) p2 = ("Mary", 23);
            Console.WriteLine($"p1: Name: {p1.Item1}, Age: {p1.Item2}");
            Console.WriteLine($"p2: Name: {p2.Name}, Age: {p2.Age}");
            
            
        }
    }
}

