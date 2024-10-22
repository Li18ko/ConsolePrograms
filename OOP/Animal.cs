using System;

namespace OOP {
    public enum Gender {
        female, male
    }
    
    public abstract class Animal {

        private string name;
        private int weight;
        private Gender gender;
        private static int count = 0;

        public Animal(string name, int weight, Gender gender) {
            this.name = name;
            this.weight = weight;
            this.gender = gender;
            count++;
        }

        public string Name {
            get {
                return this.name;
            }
            set {
                this.name = value;
            }
        }
        
        public int Weight {
            get {
                return this.weight;
            }
            set {
                if (weight >= 60)
                    this.weight = 60;
                else
                    this.weight = value;
            }
        }

        public Gender Gender {
            get {
                return this.gender;
            }
            set {
                this.gender = value;
            }
        }


        public abstract void PrintValues();

        public abstract void Speak();

        public void Run() {
            Console.WriteLine("I'm running at the moment");
        }
    
        public void Eat() {
            Console.WriteLine("I'm eating at the moment");
        }

        public void Sleep() {
            Console.WriteLine("I'm sleeping at the moment.");
        }

        public static void Print() {
            Console.WriteLine("Count animal: {0}", count);
        }
        

    }
}