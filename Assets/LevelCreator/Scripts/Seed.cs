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
            return "";

    }
    public static bool isSeedValid(string _Seed)
    {
        bool isValid = true;

        if (_Seed.Length != 8)
            isValid = false;

        if (isValid == true)
            for(int i = 0; i < 8; i++)
            {
                if (!Alphanumeric.Contains(_Seed[i].ToString()))
                    isValid = false;
            }

        return isValid;
    }
}