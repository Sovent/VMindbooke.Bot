﻿namespace Usage
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var compositionRoot = CompositionRoot.Create("appsettings.json");
            compositionRoot.Service.Start();
        }
    }
}