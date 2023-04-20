using System;
using System.Collections.Generic;
using System.Linq;

namespace HttpServer
{
    public static class MyConsole
    {
        private static Dictionary<string, ConsoleColor> Colors = new Dictionary<string, ConsoleColor>
        {
            { "Black", ConsoleColor.Black },
            { "DarkBlue", ConsoleColor.DarkBlue },
            { "DarkGreen", ConsoleColor.DarkGreen },
            { "DarkCyan", ConsoleColor.DarkCyan },
            { "DarkRed", ConsoleColor.DarkRed },
            { "DarkMagenta", ConsoleColor.DarkMagenta },
            { "DarkYellow", ConsoleColor.DarkYellow },
            { "Gray", ConsoleColor.Gray },
            { "DarkGray", ConsoleColor.DarkGray },
            { "Blue", ConsoleColor.Blue },
            { "Green", ConsoleColor.Green },
            { "Cyan", ConsoleColor.Cyan },
            { "Red", ConsoleColor.Red },
            { "Magenta", ConsoleColor.Magenta },
            { "Yellow", ConsoleColor.Yellow },
            { "White", ConsoleColor.White },
        };

        private static Dictionary<string, string> MyConsoleCommands = new Dictionary<string, string>
        {
            { "Clear", "Очистить консоль"},
            { "Exit", "Выход из программы"},
            { "Console.Change.Title()", "Изменить заголовок консоли"},
            { "Console.Change.UPColor()", "Изменить цвет текста пользователя консоли (Admin#)"},
            { "Console.Change.TextColor()", "Изменить цвет текста"},
            { "Console.Change.CommandColor()", "Изменить цвет, которым подсвечиваются команды"},
            { "Console.Change.AccessCommandsColor()", "Изменить цвет текста доступных команд по нажатию TAB"},
            { "Console.Change.CommandsDescriptionColor()", "Изменить цвет текста описания команды"},
            { "Console.Change.ArgsCommandColor()", "Изменить цвет текста аргументов у различных команд"},
            { "Console.Change.User()", "Сменить пользователя"},
            { "Console.Change.UserPrefix()", "Изменить префикс пользователя"},
            { "ShowColors", "Показать доступные цвета"}
        };

        public static ConsoleColor TextColor = ConsoleColor.White;

        /// <summary>
        /// Кол-во последних введеных команд, которые будут хранится в истории
        /// </summary>
        public static int CommandsLimit = 30;
        public static List<string> CommandsHistory = new List<string>();
        private static int historyOffset = 0;

        static Dictionary<string, string> Commands = new Dictionary<string, string>
        {
            { "Clear", "Очистить консоль"},
            { "Exit", "Выход из программы"},
            { "SQL.Connect()", "Присоединиться к БД коннектору"},
            { "Console.Change.Title()", "Изменить заголовок консоли"},
            { "Console.Change.UPColor()", "Изменить цвет текста пользователя консоли (Admin#)"},
            { "Console.Change.TextColor()", "Изменить цвет текста"},
            { "Console.Change.CommandColor()", "Изменить цвет, которым подсвечиваются команды"},
            { "Console.Change.AccessCommandsColor()", "Изменить цвет текста доступных команд по нажатию TAB"},
            { "Console.Change.CommandsDescriptionColor()", "Изменить цвет текста описания команды"},
            { "Console.Change.ArgsCommandColor()", "Изменить цвет текста аргументов у различных команд"},
            { "ShowColors", "Показать доступные цвета"},
            { "Console.Change.User()", "Сменить пользователя"},
            { "Console.Change.UserPrefix()", "Изменить префикс пользователя"}
        };

        /// <summary>
        /// Цвет доступных команд, если несколько совпадений после нажатия TAB
        /// </summary>
        public static ConsoleColor AccessesCommandsColor = ConsoleColor.Magenta;
        public static ConsoleColor CommandsDescription = ConsoleColor.DarkGray;

        /// <summary>
        /// текущий введенный в консоль текст
        /// </summary>
        public static string CurrentCommand;

        /// <summary>
        /// Цвет текста, если нашлось совпадение со списком команд для автодополнения по нажатию клавиши TAB
        /// </summary>
        public static ConsoleColor CommandColor = ConsoleColor.Red;
        /// <summary>
        /// Цвет аргументов команды
        /// </summary>
        public static ConsoleColor ArgsCommandColor = ConsoleColor.Cyan;

        public static string User = "Admin";
        public static string Prefix = "#";
        public static string UP = User + Prefix;

        public static ConsoleColor UPColor = ConsoleColor.Green;

        public static bool PrintOnlySecret;
        public static string SecretSymbol = "*";

        /// <summary>
        ///Смещение относительно последнего символа (отрицательный либо 0)
        /// </summary>
        public static int RightOffset;

        /// <summary>
        /// Текущие координаты курсора консоли
        /// </summary>
        public static int Left, Top;

        /// <summary>
        /// Установить позицию курсора консоли
        /// </summary>
        /// <param name="left">Смещение влево относительно левой части окна консоли</param>
        /// <param name="top">Смещение вниз относительно верхней части окна консоли</param>
        public static void SetCursorPosition(int left, int top)
        {
            Left = left;
            Top = top;

            if (left / Console.WindowWidth == 0)
            {
                Console.SetCursorPosition(left, top);
            }
            else
            {
                int MaxTopOffet = Left / Console.WindowWidth;
                Console.SetCursorPosition(left % Console.WindowWidth, MaxTopOffet);
            }
        }

        /// <summary>
        /// Считывает ввод символов в консоль
        /// </summary>
        /// <returns>Введенную строку после нажатия клавишы ENTER</returns>
        public static string ReadLine()
        {
            CurrentCommand = "";
            BeginWriteCommand();
            while (true)
            {
                var key = Console.ReadKey(true);

                #region Keys
                if (key.Key == ConsoleKey.Enter)
                {
                    //если ранее была нажата TAB, стираем подсказки
                    ClearCommands();


                    SetCursorPosition(0, Console.CursorTop + 1);

                    //запоминаем команду
                    if (CurrentCommand.Length > 1)
                    {
                        CommandsHistory.Add(CurrentCommand);
                        historyOffset = 0;
                    }

                    if (CurrentCommand == "Clear")
                    {
                        Console.Clear();
                    }

                    if (CurrentCommand == "Exit")
                    {
                        Environment.Exit(0);
                    }

                    if (CurrentCommand == "ShowColors")
                    {
                        int currentLeft = Left;
                        int currentTop = Top;
                        foreach (var color in Colors)
                        {
                            SetCursorPosition(3, Top + 1);
                            Write(color.Key, AccessesCommandsColor, Left, Top);
                        }
                        SetCursorPosition(3, Top + 1);
                        Write("Нажмите Enter, чтобы продолжить...", AccessesCommandsColor, Left, Top);
                        var c = Console.ReadKey(true);
                        while (c.Key != ConsoleKey.Enter)
                        {
                            c = Console.ReadKey(true);
                        }
                        SetCursorPosition(currentLeft, currentTop);


                        int AddClear = 0;
                        string ClearString = "";
                        for (int i = 0; i < Console.WindowWidth; i++) ClearString += " ";


                        foreach (var color in Colors)
                        {
                            SetCursorPosition(3, Top + 1);
                            RePrintText("", ClearString, Left, Top);

                            int MaxTopOffset = color.Key.Length / Console.WindowWidth;
                            AddClear += MaxTopOffset;
                        }


                        for (int i = 0; i < AddClear + 1; i++)
                        {

                            SetCursorPosition(3, Top + 1);
                            RePrintText("", ClearString, Left, Top);
                        }
                        ClearString = "";

                        SetCursorPosition(currentLeft, currentTop);

                    }

                    if (CurrentCommand.Contains("Console.Change."))
                    {
                        if (CurrentCommand.Split(".")[2].Split("(")[0] == "Title")
                        {
                            string arg = CurrentCommand.Split("(")[1].Split(")")[0];
                            try
                            {
                                Console.Title = arg;
                            }
                            catch { }
                        }
                        if (CurrentCommand.Split(".")[2].Split("(")[0] == "TextColor")
                        {
                            string arg = CurrentCommand.Split("(")[1].Split(")")[0];
                            try
                            {
                                TextColor = Colors[arg];
                            }
                            catch { }

                        }
                        if (CurrentCommand.Split(".")[2].Split("(")[0] == "CommandColor")
                        {
                            string arg = CurrentCommand.Split("(")[1].Split(")")[0];
                            try
                            {
                                CommandColor = Colors[arg];
                            }
                            catch { }
                        }
                        if (CurrentCommand.Split(".")[2].Split("(")[0] == "AccessCommandsColor")
                        {
                            string arg = CurrentCommand.Split("(")[1].Split(")")[0];
                            try
                            {
                                AccessesCommandsColor = Colors[arg];
                            }
                            catch { }
                        }
                        if (CurrentCommand.Split(".")[2].Split("(")[0] == "CommandsDescriptionColor")
                        {
                            string arg = CurrentCommand.Split("(")[1].Split(")")[0];
                            try
                            {
                                CommandsDescription = Colors[arg];
                            }
                            catch { }
                        }
                        if (CurrentCommand.Split(".")[2].Split("(")[0] == "ArgsCommandColor")
                        {
                            string arg = CurrentCommand.Split("(")[1].Split(")")[0];
                            try
                            {
                                ArgsCommandColor = Colors[arg];
                            }
                            catch { }
                        }
                        if (CurrentCommand.Split(".")[2].Split("(")[0] == "UPColor")
                        {
                            string arg = CurrentCommand.Split("(")[1].Split(")")[0];
                            try
                            {
                                UPColor = Colors[arg];
                            }
                            catch { }
                        }
                        if (CurrentCommand.Split(".")[2].Split("(")[0] == "User")
                        {
                            string arg = CurrentCommand.Split("(")[1].Split(")")[0];
                            User = arg;
                            UP = User + Prefix;
                        }
                        if (CurrentCommand.Split(".")[2].Split("(")[0] == "UserPrefix")
                        {
                            string arg = CurrentCommand.Split("(")[1].Split(")")[0];
                            Prefix = arg;
                            UP = User + Prefix;
                        }
                    }

                    break;
                }

                if (key.Key == ConsoleKey.Backspace)
                {
                    string tempText2 = "";
                    int BeginCountRewriteIndex = CurrentCommand.Length;
                    for (int i = 0; i < CurrentCommand.Length; i++)
                    {
                        if (i != CurrentCommand.Length - 1 + RightOffset)
                        {
                            tempText2 += CurrentCommand[i];
                        }
                        else BeginCountRewriteIndex = i;
                    }
                    CurrentCommand = tempText2;

                    if (Left - 1 >= UP.Length)
                    {
                        PrintCurrentCommand(-1);
                    }
                    else PrintCurrentCommand(0);

                    CheckForCommands(CurrentCommand);
                    CurrentCommand = CheckForCustomCommand(CurrentCommand, new string[2] { "(", ")" }, ";");

                    continue;
                }

                if (key.Key == ConsoleKey.Tab)
                {
                    CurrentCommand = AutoCompleteText2(CurrentCommand);
                    CheckForCommands(CurrentCommand);
                    IsClear = false;
                    continue;
                }

                if (key.Key == ConsoleKey.UpArrow)
                {
                    historyOffset++;
                    try
                    {
                        PrintUP();
                        RePrintText(CommandsHistory[CommandsHistory.Count - historyOffset], CurrentCommand, UP.Length, Top);
                        CurrentCommand = CommandsHistory[CommandsHistory.Count - historyOffset];

                        CheckForCommands(CurrentCommand);
                        CurrentCommand = CheckForCustomCommand(CurrentCommand, new string[2] { "(", ")" }, ";");
                    }
                    catch
                    {
                        RePrintText(CurrentCommand, CurrentCommand, UP.Length, Top);
                        historyOffset--;
                    }
                    continue;
                }

                if (key.Key == ConsoleKey.DownArrow)
                {
                    if (historyOffset - 1 > -1) historyOffset--;
                    try
                    {
                        PrintUP();
                        RePrintText(CommandsHistory[CommandsHistory.Count - historyOffset], CurrentCommand, UP.Length, Top);
                        CurrentCommand = CommandsHistory[CommandsHistory.Count - historyOffset];

                        CheckForCommands(CurrentCommand);
                        CurrentCommand = CheckForCustomCommand(CurrentCommand, new string[2] { "(", ")" }, ";");
                    }
                    catch
                    {
                        RePrintText("", CurrentCommand, UP.Length, Top);
                        CurrentCommand = "";
                    }
                    continue;
                }

                if (key.Key == ConsoleKey.LeftArrow)
                {
                    //не сдвигаемся влево, например, за Admin#
                    if (Left - 1 >= UP.Length)
                    {
                        RightOffset--;
                        SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }

                    continue;
                }

                if (key.Key == ConsoleKey.RightArrow)
                {
                    //не сдвигаемся вправо, например, за Admin#123
                    if (Left + 1 <= UP.Length + CurrentCommand.Length)
                    {
                        RightOffset++;
                        SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                    }

                    continue;
                }
                #endregion

                string tempText = "";
                for (int i = 0; i < CurrentCommand.Length + 1; i++)
                {
                    if (i == CurrentCommand.Length + RightOffset)
                    {
                        tempText += key.KeyChar;
                    }

                    if (i <= CurrentCommand.Length - 1)
                        tempText += CurrentCommand[i];
                }

                CurrentCommand = tempText;

                ClearCommands();
                PrintCurrentCommand(1);

                if (CurrentCommand.Length / Console.WindowWidth == 0)
                {

                    CheckForCommands(CurrentCommand);
                    CurrentCommand = CheckForCustomCommand(CurrentCommand, new string[2] { "(", ")" }, ";");
                }

            }

            return CurrentCommand;
        }

        private static void BeginWriteCommand()
        {
            Console.ForegroundColor = TextColor;

            RightOffset = 0;

            PrintOnlySecret = false;
            IsClear = true;

            SetCursorPosition(0, Console.CursorTop);

            Console.ForegroundColor = UPColor;
            Console.Write(UP);
            Console.ForegroundColor = ConsoleColor.White;

            SetCursorPosition(Console.CursorLeft, Console.CursorTop);
        }

        /// <summary>
        /// Печатает текущую команду с учетом положения курсора и смещения для добавления или удаления символа(-ов)
        /// </summary>
        /// <param name="cursorLeftOffset">Кол-во добавленных или удаленных символов отностильно прошлой напечатанной команды</param>
        private static void PrintCurrentCommand(int cursorLeftOffset = 0)
        {
            int currentLeft = Left;
            string temp = "";

            int oldCmdLen = CurrentCommand.Length - cursorLeftOffset;

            if (cursorLeftOffset < 0)
            {
                SetCursorPosition(currentLeft + cursorLeftOffset, Top);
                for (int i = 0; i < oldCmdLen; i++)
                {
                    temp += " ";
                }
                Console.Write(temp);
                temp = "";
                SetCursorPosition(currentLeft + cursorLeftOffset, Top);
                for (int i = CurrentCommand.Length + RightOffset; i < CurrentCommand.Length; i++)
                {
                    temp += CurrentCommand[i];
                }
                Console.Write(temp);
                temp = "";
                SetCursorPosition(UP.Length + oldCmdLen + RightOffset + cursorLeftOffset, Top);
            }
            else
            {
                for (int i = CurrentCommand.Length + RightOffset - cursorLeftOffset; i < CurrentCommand.Length; i++)
                {
                    temp += CurrentCommand[i];
                }
                Console.Write(temp);
                temp = "";
                SetCursorPosition(UP.Length + oldCmdLen + RightOffset + cursorLeftOffset, Top);
            }
        }

        private static void AutoCompleteCommand(string newCommand, string oldCommand)
        {
            int currentLeft = Left;
            string temp = "";

            int oldCmdLen = newCommand.Length - oldCommand.Length;

            if (oldCmdLen < 0)
            {
                SetCursorPosition(currentLeft + oldCmdLen, Top);
                for (int i = 0; i < oldCmdLen; i++)
                {
                    temp += " ";
                }
                Console.Write(temp);
                temp = "";
                SetCursorPosition(currentLeft + oldCmdLen, Top);
                for (int i = newCommand.Length + RightOffset; i < newCommand.Length; i++)
                {
                    temp += newCommand[i];
                }
                Console.Write(temp);
                temp = "";
                SetCursorPosition(UP.Length + oldCmdLen + RightOffset + oldCmdLen, Top);
            }
            else
            {
                for (int i = newCommand.Length + RightOffset - oldCmdLen; i < newCommand.Length; i++)
                {
                    temp += newCommand[i];
                }
                Console.Write(temp);
                temp = "";
                SetCursorPosition(UP.Length + oldCmdLen + RightOffset + oldCmdLen, Top);
            }
        }

        public static void RePrintText(string newText, string oldText, int left, int top, bool saveCurrentCursorPosition = false)
        {
            int currentLeft = Left;
            int currentTop = Top;

            SetCursorPosition(left, top);
            string temp = "";
            for (int i = 0; i < oldText.Length; i++)
            {
                temp += " ";
            }
            Console.Write(temp);
            SetCursorPosition(left, top);

            Console.Write(newText);
            Left = Console.CursorLeft;
            Top = Console.CursorTop;

            if (!saveCurrentCursorPosition) RightOffset = 0;

            if (saveCurrentCursorPosition) SetCursorPosition(currentLeft, currentTop);
        }

        public static void RePrintText(string newText, string oldText, ConsoleColor rePrintColor, int left, int top, bool saveCurrentCursorPosition = false)
        {
            int currentLeft = Left;
            int currentTop = Top;

            SetCursorPosition(left, top);
            string temp = "";
            for (int i = 0; i < oldText.Length; i++)
            {
                temp += " ";
            }
            Console.Write(temp);
            SetCursorPosition(left, top);

            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = rePrintColor;

            Console.Write(newText);
            Left = Console.CursorLeft;
            Top = Console.CursorTop;

            Console.ForegroundColor = currentColor;

            if (!saveCurrentCursorPosition) RightOffset = 0;

            if (saveCurrentCursorPosition) SetCursorPosition(currentLeft, currentTop);
        }

        public static void ReWriteCurrent(string newText, ConsoleColor newColor, string oldText, ConsoleColor oldColor)
        {
            SetCursorPosition(0, Top);
            string temp = "";
            for (int i = 0; i < oldText.Length; i++)
            {
                temp += " ";
            }
            Console.Write(temp);

            ConsoleColor currentColor = Console.ForegroundColor;

            Console.ForegroundColor = newColor;
            SetCursorPosition(0, Console.CursorTop);
            Console.Write(newText);

            Console.ForegroundColor = oldColor;
            SetCursorPosition(Console.CursorLeft, Console.CursorTop + 1);
            Console.Write(oldText);

            Console.ForegroundColor = currentColor;
        }


        private static bool IsClear;
        private static void ClearCommands()
        {
            //отчиска нижнего текста
            if (!IsClear)
            {
                int currentLeft = Left;
                int currentTop = Top;

                int AddClear = 0;
                string ClearString = "";
                for (int i = 0; i < Console.WindowWidth; i++) ClearString += " ";


                foreach (var cmd in Commands)
                {
                    SetCursorPosition(3, Top + 1);
                    RePrintText("", ClearString, Left, Top);

                    int MaxTopOffset = (cmd.Key + "-" + cmd.Value).Length / Console.WindowWidth;
                    AddClear += MaxTopOffset;
                }


                for (int i = 0; i < AddClear; i++)
                {

                    SetCursorPosition(3, Top + 1);
                    RePrintText("", ClearString, Left, Top);
                }
                ClearString = "";

                SetCursorPosition(currentLeft, currentTop);
                IsClear = true;
            }
        }

        private static string AutoCompleteText2(string command)
        {
            ClearCommands();

            int currentLeft = Left;
            int currentTop = Top;

            int matches = 0;

            foreach (var cmd in Commands)
            {
                if (cmd.Key.ToLower().Contains(command.ToLower()))
                {
                    matches++;
                }
            }

            if (matches == 0)
            {
                SetCursorPosition(UP.Length, Top);

                for (int i = 0; i < command.Length; i++) Console.Write(" ");

                SetCursorPosition(UP.Length, Top);
                Console.Write(command);
                SetCursorPosition(Console.CursorLeft, Console.CursorTop);
                RightOffset = 0;
            }

            if (matches == 1)
            {
                foreach (var cmd in Commands)
                {
                    if (cmd.Key.ToLower().Contains(command.ToLower()))
                    {
                        command = cmd.Key;

                        SetCursorPosition(UP.Length, Top);

                        for (int i = 0; i < command.Length - 1; i++) Console.Write(" ");

                        SetCursorPosition(UP.Length, Console.CursorTop);
                        Console.Write(command);
                        SetCursorPosition(Console.CursorLeft, Top);
                        RightOffset = 0;
                    }
                }
            }

            if (matches > 1)
            {
                foreach (var cmd in Commands)
                {
                    if (cmd.Key.ToLower().Contains(command.ToLower()))
                    {
                        SetCursorPosition(3, Top + 1);
                        Write(cmd.Key, AccessesCommandsColor, Left, Top);
                        Write("-", TextColor, Left, Top);
                        Write(cmd.Value, CommandsDescription, Left, Top);
                    }
                }

                SetCursorPosition(currentLeft, currentTop);
            }
            return command;
        }

        public static string CheckForCustomCommand(string command, string[] commandSplitter, string argsSplitter)
        {
            if (Commands.ContainsKey(command.Split(commandSplitter[0])[0] + commandSplitter[0] + commandSplitter[1]))
            {
                string connection_args = "";
                try
                {
                    List<string> command_args = command.Split(commandSplitter[0])[1].Split(commandSplitter[1])[0].Split(argsSplitter).ToList();

                    ChangeColor(command.Split(commandSplitter[0])[0] + commandSplitter[0], CommandColor, UP.Length, Top, true);

                    for (int i = 0; i < command_args.Count; i++)
                    {
                        ChangeColor(command_args[i], ArgsCommandColor, Console.CursorLeft, Top, true);
                        if (i == command_args.Count - 1)
                        {
                            int count = command.ToCharArray().Where(c => c == argsSplitter[0]).Count();
                            if (count < command_args.Count) break;
                        }
                        ChangeColor(argsSplitter, TextColor, Console.CursorLeft, Top, true);
                    }

                    if (command.Contains(commandSplitter[1]))
                    {
                        ChangeColor(commandSplitter[1], CommandColor, Console.CursorLeft, Top, true);
                        SetCursorPosition(Left + 1 + RightOffset, Top);
                        command = command.Split(commandSplitter[1])[0] + commandSplitter[1];
                        return command;
                    }
                }
                catch { }
            }

            return command;
        }

        private static bool IsCommandColor;
        private static void CheckForCommands(string command)
        {
            bool otherCommandsCheck = false;

            int currentLeft = Left;
            int currentTop = Top;

            if (command.Contains("SQL.Connect("))
            {
                CurrentCommand = CheckForCustomCommand(CurrentCommand, new string[2] { "(", ")" }, ";");
            }
            if (command.Contains("Console.Change."))
            {
                CurrentCommand = CheckForCustomCommand(CurrentCommand, new string[2] { "(", ")" }, "_");
            }

            if (Commands.ContainsKey(command.Split(" ")[0]) && !otherCommandsCheck)
            {
                if (command.Split(" ").Length > 1)
                {
                    string args = " ";
                    for (int i = 1; i < command.Split(" ").Length; i++)
                    {
                        if (i == command.Split(" ").Length - 1) args += command.Split(" ")[i];
                        else args += command.Split(" ")[i] + " ";
                    }

                    ChangeColor(CurrentCommand.Split(" ")[0], CommandColor, UP.Length, Top);
                    ChangeColor(args, ArgsCommandColor, UP.Length + CurrentCommand.Split(" ")[0].Length, Top);
                    IsCommandColor = true;
                }
                else
                {
                    ChangeColor(CurrentCommand.Split(" ")[0], CommandColor, UP.Length, Top);
                    IsCommandColor = true;
                }
            }
            else
            {
                if (IsCommandColor)
                {
                    ChangeColor(CurrentCommand, TextColor, UP.Length, Top);
                }
                else
                {
                    ChangeColor(CurrentCommand, TextColor, UP.Length, Top /*- (CurrentCommand.Length / Console.WindowWidth)*/);
                    IsCommandColor = false;
                }
            }
        }

        public static void PrintUP()
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = UPColor;
            SetCursorPosition(0, Top);
            Console.Write(UP);
            Console.ForegroundColor = currentColor;
        }

        public static void Write(string text)
        {
            Console.Write(text);
            Left = Console.CursorLeft;
            Top = Console.CursorTop;
        }

        public static void Write(string text, bool saveCurrentCursorPosition = false)
        {
            int currentLeft = Left;
            int currentTop = Top;

            Console.Write(text);
            Left = Console.CursorLeft;
            Top = Console.CursorTop;

            if (saveCurrentCursorPosition) SetCursorPosition(currentLeft, currentTop);
        }

        public static void Write(string text, int left, int top, bool saveCurrentCursorPosition = false)
        {
            int currentLeft = Left;
            int currentTop = Top;

            SetCursorPosition(left, top);
            Console.Write(text);
            Left = Console.CursorLeft;
            Top = Console.CursorTop;

            if (saveCurrentCursorPosition) SetCursorPosition(currentLeft, currentTop);
        }
        public static void Write(string text, ConsoleColor textColor, bool saveCurrentCursorPosition = false)
        {
            int currentLeft = Left;
            int currentTop = Top;

            ConsoleColor currentColot = Console.ForegroundColor;
            Console.ForegroundColor = textColor;

            Console.Write(text);
            Left = Console.CursorLeft;
            Top = Console.CursorTop;

            Console.ForegroundColor = currentColot;

            if (saveCurrentCursorPosition) SetCursorPosition(currentLeft, currentTop);
        }
        public static void Write(string text, ConsoleColor textColor, int left, int top, bool saveCurrentCursorPosition = false)
        {
            int currentLeft = Left;
            int currentTop = Top;

            ConsoleColor currentColot = Console.ForegroundColor;
            Console.ForegroundColor = textColor;

            SetCursorPosition(left, top);
            Console.Write(text);
            Left = Console.CursorLeft;
            Top = Console.CursorTop;

            Console.ForegroundColor = currentColot;

            if (saveCurrentCursorPosition) SetCursorPosition(currentLeft, currentTop);
        }

        public static void WriteLine(string text, bool saveCurrentCursorPosition = false)
        {
            int currentLeft = Left;
            int currentTop = Top;

            Console.Write(text);
            Left = Console.CursorLeft;
            Top = Console.CursorTop + 1;
            SetCursorPosition(Left, Top);

            if (saveCurrentCursorPosition) SetCursorPosition(currentLeft, currentTop);
        }

        public static void WriteLine(string text, int left, int top, bool saveCurrentCursorPosition = false)
        {
            int currentLeft = Left;
            int currentTop = Top;

            SetCursorPosition(left, top);
            Console.Write(text);
            Left = Console.CursorLeft;
            Top = Console.CursorTop + 1;
            SetCursorPosition(Left, Top);

            if (saveCurrentCursorPosition) SetCursorPosition(currentLeft, currentTop);
        }

        public static void WriteLine(string text, ConsoleColor textColor, bool saveCurrentCursorPosition = false)
        {
            int currentLeft = Left;
            int currentTop = Top;

            ConsoleColor currentColot = Console.ForegroundColor;
            Console.ForegroundColor = textColor;

            Console.Write(text);
            Left = Console.CursorLeft;
            Top = Console.CursorTop + 1;
            SetCursorPosition(Left, Top);

            Console.ForegroundColor = currentColot;

            if (saveCurrentCursorPosition) SetCursorPosition(currentLeft, currentTop);
        }

        public static void WriteLine(string text, ConsoleColor textColor, int left, int top, bool saveCurrentCursorPosition = false)
        {
            int currentLeft = Left;
            int currentTop = Top;

            ConsoleColor currentColot = Console.ForegroundColor;
            Console.ForegroundColor = textColor;

            SetCursorPosition(left, top);
            Console.Write(text);
            Left = Console.CursorLeft;
            Top = Console.CursorTop + 1;
            SetCursorPosition(Left, Top);

            Console.ForegroundColor = currentColot;

            if (saveCurrentCursorPosition) SetCursorPosition(currentLeft, currentTop);
        }

        private static string DeleteSymbol(string text, int index)
        {
            string tempText = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (i != index)
                {
                    tempText += CurrentCommand[i];
                }
            }
            text = tempText;

            return text;
        }
        private static void ChangeColor(string text, ConsoleColor color, int left, int top, bool saveCursorAfterChangeColor = false)
        {
            int currentLeft = Left;
            int currentTop = Top;

            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = color;

            SetCursorPosition(left, top);
            Console.Write(text);

            Console.ForegroundColor = currentColor;

            if (!saveCursorAfterChangeColor)
                SetCursorPosition(currentLeft, currentTop);
        }

    }
}
