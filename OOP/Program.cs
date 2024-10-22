using System;

namespace OOP {

    class Program {
        static void Main(string[] args) {
            Dog dog = new Dog("Bobi", 23, Gender.female);
            Cat cat = new Cat("Orange", 16, Gender.female);
            
            dog.PrintValues();
            dog.Run();
            dog.Speak();
            
            cat.PrintValues();
            cat.Run();
            cat.Speak();
            
            Animal.Print();

            Dog dog1 = new Dog("Raf", 12, Gender.male);
            dog1.PrintValues();
            dog1.Sleep();
            
            Animal.Print();

        }
    }
    
}