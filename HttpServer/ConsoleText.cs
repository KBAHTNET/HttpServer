using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpServer
{
    public static class ConsoleTextSettings
    {
        public static ConsoleColor UserColor = ConsoleColor.Blue;
        public static ConsoleColor CommandColor = ConsoleColor.Green;
        public static ConsoleColor CommandArgsColor = ConsoleColor.Cyan;
        public static ConsoleColor ListAccesableCommands = ConsoleColor.DarkMagenta;
        public static ConsoleColor RequestColor = ConsoleColor.DarkYellow;

        private static string UserName = "Admin";
        private static string Prefix = "#";
        public static string UsNamPref = UserName + Prefix;

        public static int CommandsLimit = 30;
        public static List<string> commandsHistory = new List<string>();
        public static int historyOffset = 0;
        public static int offset = 0;

        public static int ConsoleLeft, ConsoleTop;
    }
    public class ConsoleTextColor
    {
        public string Text;
        public ConsoleColor Color;
        public ConsoleTextColor(string text, ConsoleColor color)
        {
            Text = text;
            Color = color;
        }
    }
    public static class ConsoleText
    {
        public static string CurrentCommand;
        public static string WaitForInput()
        {
            CurrentCommand = "";

            while (true)
            {
                ConsoleTextSettings.ConsoleTop = Console.CursorTop;
                ConsoleTextSettings.ConsoleLeft = Console.CursorLeft;
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.SetCursorPosition(0, Console.CursorTop + 1);
                    for (int i = 0; i < HTTPServerCommands.Commands.Count; i++)
                    {
                        Console.Write("                              ");
                        Console.SetCursorPosition(0, Console.CursorTop + 1);
                    }

                    Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);

                    Console.SetCursorPosition(0, Console.CursorTop + 1);
                    if (CurrentCommand.Length > 1) ConsoleTextSettings.commandsHistory.Add(CurrentCommand);
                    if (ConsoleTextSettings.commandsHistory.Count > ConsoleTextSettings.CommandsLimit) ConsoleTextSettings.commandsHistory.RemoveAt(0);
                    ConsoleTextSettings.historyOffset = 0;
                    ConsoleTextSettings.offset = 0;
                    break;
                }

                if (key.Key == ConsoleKey.UpArrow)
                {
                    Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);

                    ConsoleTextSettings.historyOffset++;
                    try
                    {
                        CurrentCommand = ConsoleTextSettings.commandsHistory[ConsoleTextSettings.commandsHistory.Count - ConsoleTextSettings.historyOffset];
                        WriteFromStart(new ConsoleTextColor("                                                                                                                                                           ", ConsoleColor.White));
                        Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);
                        WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor), new ConsoleTextColor(CurrentCommand, ConsoleColor.White));
                    }
                    catch
                    {
                        ConsoleTextSettings.historyOffset--;
                    }
                    CheckForCommands(CurrentCommand);
                    continue;
                }

                if (key.Key == ConsoleKey.DownArrow)
                {
                    Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);
                    if (ConsoleTextSettings.historyOffset - 1 > -1) ConsoleTextSettings.historyOffset--;
                    try
                    {
                        CurrentCommand = ConsoleTextSettings.commandsHistory[ConsoleTextSettings.commandsHistory.Count - ConsoleTextSettings.historyOffset];
                        WriteFromStart(new ConsoleTextColor("                                                                                                                                                           ", ConsoleColor.White));
                        Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);
                        WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor), new ConsoleTextColor(CurrentCommand, ConsoleColor.White));
                    }
                    catch
                    {
                        WriteFromStart(new ConsoleTextColor("                                                                                                                                                           ", ConsoleColor.White));
                        Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);
                        WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor));
                        CurrentCommand = "";
                    }
                    CheckForCommands(CurrentCommand);
                    continue;
                }

                if (key.Key == ConsoleKey.LeftArrow)
                {
                    if (Math.Abs(ConsoleTextSettings.offset) < CurrentCommand.Length)
                    {
                        ConsoleTextSettings.offset--;
                        ConsoleTextSettings.ConsoleLeft--;

                        Console.SetCursorPosition(0, ConsoleTextSettings.ConsoleTop);
                        Console.ForegroundColor = ConsoleTextSettings.UserColor;
                        Console.Write(ConsoleTextSettings.UsNamPref);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(CurrentCommand);

                        Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);

                        //int temp_left = ConsoleTextSettings.ConsoleLeft - 1;

                        //WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor),
                        //    new ConsoleTextColor(CurrentCommand, ConsoleTextSettings.CommandColor));

                        //ConsoleTextSettings.ConsoleLeft = temp_left;
                        //Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);

                        //Console.SetCursorPosition(0, ConsoleTextSettings.ConsoleTop);
                        //Write(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor);
                        //Write(CurrentCommand, ConsoleColor.White);
                        //ConsoleTextSettings.ConsoleLeft -= Math.Abs(ConsoleTextSettings.offset);
                        //ConsoleTextSettings.ConsoleTop++;
                        //Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);

                        //WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor), new ConsoleTextColor(CurrentCommand, ConsoleColor.White));
                        //ConsoleTextSettings.ConsoleLeft-=Math.Abs(ConsoleTextSettings.offset);
                        //ConsoleTextSettings.ConsoleTop++;
                        //ConsoleTextSettings.offset--;
                        //Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);

                        //Console.SetCursorPosition(Console.CursorLeft - Math.Abs(ConsoleTextSettings.offset), Console.CursorTop);

                    }
                    else
                    {
                        //WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor), new ConsoleTextColor(CurrentCommand, ConsoleColor.White));
                        Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);
                    }

                    continue;
                }

                if (key.Key == ConsoleKey.RightArrow)
                {
                    if (ConsoleTextSettings.offset + 1 <= 0)
                    {
                        ConsoleTextSettings.offset++;
                        Console.SetCursorPosition(0, ConsoleTextSettings.ConsoleTop);
                        Write(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor);
                        Write(CurrentCommand, ConsoleColor.White);
                        ConsoleTextSettings.ConsoleLeft = Math.Abs(ConsoleTextSettings.UsNamPref.Length + CurrentCommand.Length - Math.Abs(ConsoleTextSettings.offset));
                        ConsoleTextSettings.ConsoleTop++;
                        Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);
                        //WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor), new ConsoleTextColor(CurrentCommand, ConsoleColor.White));
                        //ConsoleTextSettings.ConsoleLeft += ConsoleTextSettings.UsNamPref.Length + ConsoleText.CurrentCommand.Length  - Math.Abs(ConsoleTextSettings.offset);
                        //ConsoleTextSettings.ConsoleTop++;
                        //Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);

                        //Console.SetCursorPosition(Console.CursorLeft + Math.Abs(ConsoleTextSettings.offset), Console.CursorTop);
                        //ConsoleTextSettings.offset++;
                    }
                    else
                    {
                        //WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor), new ConsoleTextColor(CurrentCommand, ConsoleColor.White));
                        Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);
                    }
                    continue;
                }

                if (key.Key == ConsoleKey.Tab)
                {
                    CurrentCommand = AutoCompleteText(CurrentCommand);
                    CheckForCommands(CurrentCommand);
                    continue;
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    ConsoleTextSettings.ConsoleLeft = Console.CursorLeft;
                    Console.Write(" ");
                    Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);
                    //Console.Write(" ");
                    //Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft-1, ConsoleTextSettings.ConsoleTop);

                    if (Console.CursorLeft < ConsoleTextSettings.UsNamPref.Length)
                    {
                        WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor));
                        continue;
                    }

                    string temp = "";
                    for (int i = 0; i < CurrentCommand.Length; i++)
                    {
                        if (i != CurrentCommand.Length - 1 + ConsoleTextSettings.offset)
                            temp += CurrentCommand[i];
                    }
                    CurrentCommand = temp;
                    WriteFromStart(new ConsoleTextColor("                                                                                                                                                           ", ConsoleColor.White));
                    Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);
                    WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor), new ConsoleTextColor(CurrentCommand, ConsoleColor.White));
                    //Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft - 1, ConsoleTextSettings.ConsoleTop);

                    CheckForCommands(CurrentCommand);

                    continue;
                }


                string t = "";

                if (ConsoleTextSettings.offset < 0)
                {
                    for (int i = 0; i < CurrentCommand.Length + ConsoleTextSettings.offset; i++)
                    {
                        t += CurrentCommand[i];
                    }
                    t += key.KeyChar;
                    for (int i = CurrentCommand.Length + ConsoleTextSettings.offset; i < CurrentCommand.Length; i++)
                    {
                        t += CurrentCommand[i];
                    }
                    CurrentCommand = t;
                    WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor), new ConsoleTextColor(CurrentCommand, ConsoleColor.White));
                    Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft + 1, ConsoleTextSettings.ConsoleTop);

                    CheckForCommands(CurrentCommand);
                }


                CurrentCommand += key.KeyChar;

                CheckForCommands(CurrentCommand);
            }
            return CurrentCommand;
        }

        private static void CheckForCommands(string command)
        {
            bool otherCommandsCheck = false;

            if (command.Contains("SQL.Connect("))
            {
                string connection_args = "";
                try
                {
                    connection_args = command.Split("SQL.Connect(")[1].Split(")")[0];

                    List<string> command_args = command.Split("(")[1].Split(")")[0].Split(";").ToList();
                    WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor),
                            new ConsoleTextColor(command.Split("(")[0] + "(", ConsoleTextSettings.CommandColor));

                    for (int i = 0; i < command_args.Count; i++)
                    {
                        Write(command_args[i], ConsoleColor.Cyan);
                        if (i == command_args.Count - 1)
                        {
                            int count = command.ToCharArray().Where(c => c == ';').Count();
                            if (count < command_args.Count) break;
                        }
                        Write(";", ConsoleColor.White);
                    }

                    if (command.Contains(")"))
                        Write(")", ConsoleTextSettings.CommandColor);

                    if (command.Split(")").Count() > 1)
                    {
                        Write(command.Split(")")[1], ConsoleColor.White);
                    }
                }
                catch { }
                return;
            }

            if (HTTPServerCommands.Commands.Contains(command.Split(" ")[0]) && !otherCommandsCheck)
            {
                if (command.Split(" ").Length > 1)
                {
                    string args = " ";
                    for (int i = 1; i < command.Split(" ").Length; i++)
                    {
                        if (i == command.Split(" ").Length - 1) args += command.Split(" ")[i];
                        else args += command.Split(" ")[i] + " ";
                    }
                    WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor),
                        new ConsoleTextColor(command.Split(" ")[0], ConsoleTextSettings.CommandColor),
                        new ConsoleTextColor(args, ConsoleColor.Cyan));

                }
                else
                {
                    ConsoleText.WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor),
                        new ConsoleTextColor(command.Split(" ")[0], ConsoleTextSettings.CommandColor));
                }
            }
        }

        private static string AutoCompleteText(string command)
        {
            Console.SetCursorPosition(0, Console.CursorTop + 1);
            for (int i = 0; i < HTTPServerCommands.Commands.Count; i++)
            {
                Console.Write("                              ");
                Console.SetCursorPosition(0, Console.CursorTop + 1);
            }

            Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);

            int pair = 0;
            foreach (var cmd in HTTPServerCommands.Commands)
            {
                if (cmd.ToLower().Contains(command.ToLower()))
                {
                    pair++;
                }
            }
            if (pair == 1)
            {
                foreach (var cmd in HTTPServerCommands.Commands)
                {
                    if (cmd.ToLower().Contains(command.ToLower()))
                    {
                        command = cmd;
                        WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor),
                            new ConsoleTextColor(command, ConsoleTextSettings.CommandColor));

                        Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        continue;
                    }
                }
            }
            if (pair > 1)
            {
                foreach (var cmd in HTTPServerCommands.Commands)
                {
                    if (cmd.ToLower().Contains(command.ToLower()))
                    {


                        Console.SetCursorPosition(3, Console.CursorTop + 1);

                        Write(cmd, ConsoleTextSettings.ListAccesableCommands);

                        Console.SetCursorPosition(Console.CursorLeft + 3, Console.CursorTop);

                        continue;
                    }
                }
                Console.SetCursorPosition(ConsoleTextSettings.ConsoleLeft, ConsoleTextSettings.ConsoleTop);
            }

            return command;
        }

        public static void Write(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.White;
            ConsoleTextSettings.ConsoleLeft = Console.CursorLeft;
            ConsoleTextSettings.ConsoleTop = Console.CursorTop - 1;
        }

        public static void Write(params ConsoleTextColor[] texts)
        {
            foreach (var textcolor in texts)
            {
                Console.ForegroundColor = textcolor.Color;
                Console.Write(textcolor.Text);
                Console.ForegroundColor = ConsoleColor.White;
            }
            ConsoleTextSettings.ConsoleLeft = Console.CursorLeft;
            ConsoleTextSettings.ConsoleTop = Console.CursorTop - 1;
        }

        public static void WriteLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n");
            ConsoleTextSettings.ConsoleLeft = Console.CursorLeft;
            ConsoleTextSettings.ConsoleTop = Console.CursorTop;
        }

        public static void WriteLine(params ConsoleTextColor[] texts)
        {
            foreach (var textcolor in texts)
            {
                Console.ForegroundColor = textcolor.Color;
                Console.Write(textcolor.Text);
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.Write("\n");
            ConsoleTextSettings.ConsoleLeft = Console.CursorLeft;
            ConsoleTextSettings.ConsoleTop = Console.CursorTop;
        }

        public static void ReWrite(string text, string rewritableText, ConsoleColor color, ConsoleColor rewriteTextColor)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            for (int i = 0; i < rewritableText.Length; i++) Console.Write(" ");
            Console.SetCursorPosition(0, Console.CursorTop);

            Console.ForegroundColor = color;
            Console.Write(text);

            Console.WriteLine();
            Console.ForegroundColor = rewriteTextColor;
            Console.Write(rewritableText);
            Console.ForegroundColor = ConsoleColor.White;
            ConsoleTextSettings.ConsoleLeft = Console.CursorLeft;
            ConsoleTextSettings.ConsoleTop = Console.CursorTop;
        }

        public static void WriteFromStart(params ConsoleTextColor[] textscolor)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine("");
            ConsoleTextSettings.ConsoleTop = Console.CursorTop-1;
            Console.SetCursorPosition(0, ConsoleTextSettings.ConsoleTop);
            foreach (var textcolor in textscolor)
            {
                Console.ForegroundColor = textcolor.Color;
                Console.Write(textcolor.Text);
                Console.ForegroundColor = ConsoleColor.White;
            }
            ConsoleTextSettings.ConsoleLeft = Console.CursorLeft;
            //ConsoleTextSettings.ConsoleTop = Console.CursorTop-1;
        }

        public static void RewriteCurrenCommandtString(string rewriteText, ConsoleColor textColor)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            for (int i = 0; i < CurrentCommand.Length + ConsoleTextSettings.UsNamPref.Length; i++) Console.Write(" ");
            Console.SetCursorPosition(0, Console.CursorTop);

            ConsoleText.WriteFromStart(new ConsoleTextColor(rewriteText, textColor));
            Console.WriteLine(" ");

            ConsoleTextSettings.ConsoleLeft = Console.CursorLeft;
            ConsoleTextSettings.ConsoleTop = Console.CursorTop;

            ConsoleText.WriteFromStart(
                new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor),
                new ConsoleTextColor(ConsoleText.CurrentCommand, ConsoleColor.White));
            ConsoleTextSettings.ConsoleLeft = Console.CursorLeft;
            ConsoleTextSettings.ConsoleTop = Console.CursorTop;
        }
    }
}
