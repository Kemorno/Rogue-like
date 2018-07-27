using System;

public static class Seed
{

    static string Alphanumeric = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string GenerateSeed(Random Prng)
    {
        string seed = "";

        for (int i = 0; i < 8; i++)
        {
            seed += newChar(Prng);
        }
        
        return seed;
    }
    static char newChar(Random Prng)
    {
        return Alphanumeric[Prng.Next(0, Alphanumeric.Length)];
    }
}