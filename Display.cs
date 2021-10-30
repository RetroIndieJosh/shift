using System;

namespace csif
{
    // A wrapper for the C# console to allow easy modification for other interfaces
    // (also potential speedup by limiting console writes, but needs profiling to verify)
    public class Display
    {
        static private string text = "";

        static public void Flush()
        {
            Console.Write(text);
            text = "";
        }

        static public ConsoleKeyInfo ReadKey(bool capture = false)
        {
            return Console.ReadKey(capture);
        }

        static public string ReadLine()
        {
            return Console.ReadLine();
        }

        static public void Write(string s, params object[] args)
        {
            text += String.Format(s, args);
        }

        static public void WriteLine(string s = "", params object[] args)
        {
            if (!string.IsNullOrEmpty(s))
                Write(s, args);
            Write("\n");
        }
    }
}