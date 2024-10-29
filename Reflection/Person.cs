namespace ConsolePrograms {
   
    [Description("Класс, представляющий человека")]
    public class Person: IRun, ISpeak {
        public string Name { get; }
        public int Age { get; set; }

        public Person(string name, int age) {
            Name = name;
            Age = age;
        }
        
        public void Run() {
            Console.WriteLine($"{Name} runs");
        }

        public void Speak() {
            Console.WriteLine($"{Name} speaks");
        }

        public void Eat() {
            Console.WriteLine($"{Name} is eating");
        }

        public void Sleep() {
            Console.WriteLine($"{Name} is sleeping");
        }

        public void DisplayInfo() {
            Console.WriteLine($"Name: {Name}, Age: {Age}");
        }
    }
    
}