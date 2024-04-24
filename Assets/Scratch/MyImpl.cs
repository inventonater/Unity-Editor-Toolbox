public class MyImpl : IMyInterface
{
    private int _hi = 7;
    public int _yo = 8;

    public int Bye { get; } = 10;
    public void Update()
    {
        _yo++;
    }
}