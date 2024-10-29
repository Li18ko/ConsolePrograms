namespace ConsolePrograms;

public class Point {
    
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y) {
        X = x;
        Y = y;
    }

    public static Point operator +(Point p1, Point p2) {
        return new Point(p1.X + p2.X, p1.Y + p2.Y);
    }

    public static Point operator -(Point p1, Point p2) {
        return new Point(p2.X - p1.X, p2.Y - p1.Y);
    }

    public static bool operator ==(Point p1, Point p2) {
        return p1.X == p2.X && p1.Y == p2.Y;
    }

    public static bool operator !=(Point p1, Point p2) {
        return !(p1 == p2);
    }

    public override string ToString() {
        return "(" + X + ", " + Y + ")";
    }
}