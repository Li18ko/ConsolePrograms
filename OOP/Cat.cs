using System;

namespace OOP {
    public class Cat: Animal {
        public Cat(string name, int weight, Gender gender) : base(name, weight, gender) {
            
        }
        
        public override void PrintValues() {
            Console.WriteLine("I was born a cat, my name is {0}, I weigh {1} kg and my gender is {2}", this.Name, this.Weight, this.Gender);
        }
        
        public override void Speak() {
            Console.WriteLine("I'm meowing");
        }
    }
}