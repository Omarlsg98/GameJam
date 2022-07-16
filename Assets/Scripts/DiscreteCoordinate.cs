public class DiscreteCoordinate{
    public int x;
    public int y;

    public DiscreteCoordinate(int y, int x){
        this.x = x;
        this.y = y;
    }

    public bool isEquals(DiscreteCoordinate obj)
    {
        return obj.x == this.x && obj.y == this.y;
    }

    public override int GetHashCode()
    {
        return (x << 2) ^ y;
    }

    public override string ToString()
    {
        return System.String.Format("DiscreteCoordinate({0}, {1})", y, x);
    }
}