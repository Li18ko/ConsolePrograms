using System;
using System.Collections;
using System.Collections.Generic;

namespace Collections {

    class Program {
        static void Main(string[] args) {
            /*System.Collections.Generic типизированные коллекции*/
            //Dictionary();
            //List();
            //QueueGeneric();
            //SortedList();   
            //StackGeneric();
            //HashSet();
            //LinkedList();


            /*System.Collections нетипизированные коллекции*/
            //ArrayList();
            //Hashtable();
            //Stack();
            //Queue();
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

            if (dictionary.ContainsKey(2)) {
                Console.WriteLine("Ключ 2 есть");
            }
            else {
                Console.WriteLine("Ключа 2 нет");
            }

            if (dictionary.ContainsValue("two")) {
                Console.WriteLine("Значения two есть");
            }
            else {
                Console.WriteLine("Значения two нет");
            }
            
            
            dictionary.Remove(1);
            foreach (KeyValuePair<int, string> kvp in dictionary) {
                Console.WriteLine(kvp.Key + " " + kvp.Value);
            }
            
            Console.WriteLine(dictionary[2]);

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

        private static void QueueGeneric() {
            Queue<string> numbers = new Queue<string>();
            numbers.Enqueue("one");
            numbers.Enqueue("two");
            numbers.Enqueue("three");
            numbers.Enqueue("four");
            numbers.Enqueue("five");

            foreach (string str in numbers) {
                Console.WriteLine(str);
            }

            if (numbers.Contains("two")) {
                Console.WriteLine("two есть");
            }

            string one = numbers.Dequeue();
            string oneNew = numbers.Peek();
            Console.WriteLine("Первым элементом был {0}, сейчас уже {1}", one, oneNew);
            Console.WriteLine(one.GetType());
        }

        private static void SortedList() {
            SortedList<int, string> sortedList = new SortedList<int, string>();
            sortedList.Add(3, "Three");
            sortedList.Add(1, "One");
            sortedList.Add(2, "Two");

            foreach (KeyValuePair<int, string> kv in sortedList)
            {
                Console.WriteLine(kv.Key + " : " + kv.Value);
            }
        }

        private static void StackGeneric() {
            Stack<int> stack = new Stack<int>();
            stack.Push(1);
            stack.Push(2);
            Console.WriteLine(stack.Pop());
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

        private static void LinkedList() {
            LinkedList<int> linkedList = new LinkedList<int>();
            linkedList.AddLast(1);
            linkedList.AddLast(2);
            Console.WriteLine(linkedList.First.Value);
            linkedList.AddFirst(3);
            Console.WriteLine(linkedList.First.Value);
            Console.WriteLine(linkedList.Last.Value);
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

        private static void Hashtable() {
            Hashtable hashtable = new Hashtable();
            hashtable.Add("apple", 1);
            hashtable.Add("banana", 2);
            hashtable["cherry"] = 3;

            foreach (DictionaryEntry entry in hashtable)
            {
                Console.WriteLine(entry.Key + " : " + entry.Value);
            }
        }

        private static void Stack() {
            Stack stack = new Stack();
            stack.Push("Hello");
            stack.Push(123);
            Console.WriteLine(stack.Pop()); 
            Console.WriteLine(stack.Pop()); 
        }
        
        private static void Queue() {
            Queue queue = new Queue();
            queue.Enqueue("First");
            queue.Enqueue(2);
            queue.Enqueue(3.5);
            Console.WriteLine(queue.Dequeue()); 
            Console.WriteLine(queue.Dequeue()); 
            Console.WriteLine(queue.Dequeue()); 
        }
        
    }
}

