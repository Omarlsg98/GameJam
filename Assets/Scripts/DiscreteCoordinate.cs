public class DiscreteCoordinate{
    public int x;
    public int y;

    public DiscreteCoordinate(int y, int x){
        this.x = x;
        this.y = y;
    }

    public override bool Equals(System.Object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || ! this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else {
            DiscreteCoordinate p = (DiscreteCoordinate) obj;
            return (x == p.x) && (y == p.y);
        }
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