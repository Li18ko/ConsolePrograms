using System;

namespace DatabaseTests {
    class Program {
        public static void Main(String[] args) {
            
            Console.WriteLine("Введите имя пользователя:");
            string name = Console.ReadLine();
            Console.WriteLine("Введите возраст пользователя:");
            string ageInput = Console.ReadLine();
            int age;
            if (int.TryParse(ageInput, out age)) {
                DatabaseConnection.Instance.InsertUser(name, age);
                Console.WriteLine("Пользователь добавлен!");
            }
            else {
                Console.WriteLine("Некорректный возраст");
            }
            
            Console.WriteLine("\nСписок всех пользователей:");
            var users = DatabaseConnection.Instance.GetUsers();
            Console.WriteLine("\nСписок всех пользователей: {0}", users.Count());
            foreach (var user in users) {
                Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Age: {user.Age}");
            }
        }
    }
}
