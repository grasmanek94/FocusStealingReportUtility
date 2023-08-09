using System.Windows.Automation;
using System.Diagnostics;

namespace FuckFocusStealingSharp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SubscribeToFocusChange();
            Console.WriteLine("monitoring... hit enter to end.");
            Console.ReadLine();
        }

        static void SubscribeToFocusChange()
        {
            var focusHandler = new AutomationFocusChangedEventHandler(OnFocusChange);
            Automation.AddAutomationFocusChangedEventHandler(focusHandler);
        }

        private static void PrintProcess(Process process, string prepend)
        {
            if(process == null)
            {
                return;
            }

            Console.WriteLine("{0}{1} {3} ({2})",
                 prepend, process.Id, process.ProcessName, 
                 process.MainModule?.FileName ?? "{Unknown}");

            using(Process parent = ParentProcessUtilities.GetParentProcess(process.Handle))
            {
                PrintProcess(parent, prepend + " ");
            }
        }

        private static void OnFocusChange(object src, AutomationFocusChangedEventArgs args)
        {
            Console.WriteLine("-- Focus changed!");
            AutomationElement? element = src as AutomationElement;

            if (element == null)
            {
                return;
            }

            int processid = element.Current.ProcessId;
            using (Process process = Process.GetProcessById(processid))
            {
                PrintProcess(process, " ");
            }
        }
    }
}
