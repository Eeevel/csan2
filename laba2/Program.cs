using System;

namespace laba2
{
    class Program
    {
        private const string DNS_PARAMETER = "-d";

        static void Main(string[] args)
        {
            if (args.Length == 1)
                Tracert.Perform(args[0], false);
            else
                Tracert.Perform(args[0], args[1] == DNS_PARAMETER);
        }
    }
}
