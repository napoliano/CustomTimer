using System.Data;

namespace CustomTimer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string message = "Timer expired";

            Timer.Instance.AddTimerTask(5000, (arg) => {
                Console.WriteLine($"{message} - {DateTime.Now}");
            }, message);
        }
    }
}