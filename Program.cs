using System;
using System.Data;

class Program
{
    static void Main()
    {
        Superuser.CreateSuperuser("jojo@jo.com", "kaasje");
        PresentLogin.Start();
    }
}