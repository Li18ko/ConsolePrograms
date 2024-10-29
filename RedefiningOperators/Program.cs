namespace ConsolePrograms {
    public class Program {
        public static void Main(string[] args) {
            Point point1 = new Point(1, 2);
            Point point2 = new Point(3, 5);
            Point point3 = new Point(1, 2);
            
            Console.WriteLine(point1 + point2);
            Console.WriteLine(point1 - point2);
            Console.WriteLine(point1 == point3);
            Console.WriteLine(point1 != point3);
        }
    }
}