using System;
using System.Reflection;

namespace ConsolePrograms {
    class Program {

        public static void Main(string[] args) {
            Type myType = typeof(Person);
            Console.WriteLine("Имя типа: " + myType.Name);
 
            var attributes = myType.GetCustomAttributes();
            foreach (var attribute in attributes) {
                if (attribute is DescriptionAttribute descAttr) {
                    Console.WriteLine("Описание: " + descAttr.Description);
                }
            }

            Console.WriteLine("\nРеализованные интерфейсы:");
            foreach (Type i in myType.GetInterfaces()) {
                Console.WriteLine(i.Name);
            }

            Console.WriteLine("\nМетоды класса:");
            MethodInfo[] methods = myType.GetMethods();
            foreach (var method in methods) {
                Console.WriteLine($"Метод: {method.Name}, Возвращаемый тип: {method.ReturnType}");
            }

            Console.WriteLine("\nСвойства класса:");
            PropertyInfo[] properties = myType.GetProperties();
            foreach (var property in properties) {
                Console.WriteLine($"Свойство: {property.Name}, Тип: {property.PropertyType}");
            }

            Person person = new Person("Alice", 30);
            person.Run();
            person.Speak();
            person.Eat();
            person.Sleep();
            person.DisplayInfo();
        }
        
    }
}