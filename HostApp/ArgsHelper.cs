using System.Linq;

namespace HostApp
{
    public class ArgsHelper
    {
        public static string GetOrderFileName(string[] args)
        {
            return args.First(p => p.Contains("заявка"));
        }
    }
}
