using System.Windows.Automation;
using System.Diagnostics;
using System;
using System.Drawing;

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
                string spaces = " ";
                Console.WriteLine("{0}{1} {3} ({2})",
                    spaces, processid, process.ProcessName, process.MainModule.FileName);

                nint current_handle = process.Handle;
                try
                {
                    while (current_handle != 0)
                    {
                        using (Process parent = ParentProcessUtilities.GetParentProcess(current_handle))
                        {
                            if (parent != null)
                            {
                                current_handle = parent.Handle;
                                spaces += " ";
                                Console.WriteLine("{0}{1} {3} ({2})",
                                    spaces, parent.Id, parent.ProcessName, parent.MainModule?.FileName ?? "{Unknown}");
                            }
                            else
                            {
                                current_handle = 0;
                            }
                        }
                    }
                }
                catch{}
            }
        }
    }
}
