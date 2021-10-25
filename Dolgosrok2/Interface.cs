using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myInterface
{
    public class Interface
    {
        public Interface()
        {
            StartInterface();
        }

        public void StartInterface()
        {
            Console.WriteLine("Choose that you want to do:\n1 - Start program with parameters\n2 - Upload file\n3 - Download file\n4 - Delete file\nEsc - exit from program");
            this._key = Console.ReadKey();
            switch (this._key.Key)
            {
                case ConsoleKey.D1:
                    Action("launch programm");
                    return;
                case ConsoleKey.D2:
                    Action("upload file");
                    return;
                case ConsoleKey.D3:
                    Action("download file");
                    return;
                case ConsoleKey.D4:
                    Action("delete file");
                    return;
                case ConsoleKey.Escape:
                    System.Environment.Exit(0);
                    return;
            }
            Console.Clear();
            Console.WriteLine("Press one of this buttons");
            StartInterface();
        }

        public void CheckBackESC()
        {
            switch (this._key.Key)
            {
                case ConsoleKey.Backspace:
                    Console.Clear();
                    StartInterface();
                    break;
                case ConsoleKey.Escape:
                    System.Environment.Exit(0);
                    break;
            }
            if (this._key.Key != ConsoleKey.Enter)
            {
                Console.WriteLine("\nPress one of this button");
                this._key = Console.ReadKey();
                CheckBackESC();
            }
        }

        public void CreateResult()
        {
            Console.Clear();
            Console.WriteLine("Write directory of file");
            this._result = Convert.ToString(Console.ReadLine());
            Console.Clear();
        }

        public string GetResult()
        {
            return this._result;
        }

        public void Action(string toDo)
        {
            Console.Clear();
            Console.WriteLine("Enter - If you realy want to " + toDo + "\nESC - exit\nBackspace - Cumback to the start menu\n");
            this._key = Console.ReadKey();
            CheckBackESC();
            CreateResult();
        }

        private ConsoleKeyInfo _key;
        private string _result;
    }
}
