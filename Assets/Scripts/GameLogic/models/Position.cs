namespace Iterum.models
{
    public class Position
    {
        public Position() { 
            X = 0; 
            Y = 0;
            Z = 0;
        }

        public Position(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
}
