using System;

public static class SeedController
{
    static string Alphanumeric = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static char newChar(Random Prng)
    {
        return Alphanumeric[Prng.Next(0, Alphanumeric.Length)];
    }
    public static string FormatedSeed(string _Seed)
    {
        if (isSeedValid(_Seed))
        {
            string seed = _Seed;
            if (seed.Length == 8)
                seed = seed.Insert(4, " ");

            return seed;
        }
        else
            return "Seed Not Valid";
    }
    public static bool isSeedValid(string _Seed)
    {
        string Seed = _Seed.Replace(" ", string.Empty);

        if (Seed.Length != 8)
            return false;

        Seed = Seed.ToUpper();
        for (int i = 0; i < 8; i++)
        {
            if (!Alphanumeric.Contains(Seed[i].ToString()))
                return false;
        }

        return true;
    }
}