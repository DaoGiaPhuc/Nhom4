using System.Text;
using System.IO;
namespace DinosaurGame
{
    class Program
    {
        public static int maxjump = 9;
        public static int[] BasePositionOfTheTrex = new int[2] { 0, 0 };
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Display.NameGame();
            Display.Print((Console.BufferWidth - "Nhấn phím bất kì để tiếp tục...".Length) / 2, Console.BufferHeight / 2 + 7, "Nhấn phím bất kì để tiếp tục...", "", "DarkMagenta");
            Console.ReadKey(true);
        Return:;
            Console.Clear();
            Display.Reset(ref BasePositionOfTheTrex);

            // Nhận phím từ người dùng
            ConsoleKeyInfo key = new ConsoleKeyInfo();
            int[] PreviousPlayersName = new int[0];

            int[,] Level = new int[7, 2] { { 600, 35 }, { 610, 34 }, { 620, 33 }, { 630, 32 }, { 640, 31 }, { 650, 30 }, { 660, 30 } };
            int NumOfLvl = 0;

            int leftorright = 0;

            int heightjumping = 0,                // Độ cao khi nhảy
                speed = Level[NumOfLvl, 0],       // Tốc độ dịch chuyển phong cảnh
                score = 0;                        // Điểm số

            bool Exit = false;

            // Lưu các Bitmap của T-rex, Xương rồng, Thằn lằn bay -> kiểm tra va chạm
            KeyValuePair<int[], bool[,]> ShapOfTrex
                = new KeyValuePair<int[], bool[,]>(new int[] { 0, 0 }, new bool[,] { { true } });

            KeyValuePair<int[][], bool[][,]> ShapOfCacti
                = new KeyValuePair<int[][], bool[][,]>(new int[][] { new int[] { 0, 0 } }, new bool[][,] { new bool[,] { { true } } });

            KeyValuePair<int[][], bool[][,]> ShapOfPteroes
                = new KeyValuePair<int[][], bool[][,]>(new int[][] { new int[] { 0, 0 } }, new bool[][,] { new bool[,] { { true } } });

            int[] IndexOfSaveBox = new int[2];
            Display.Print(0, 0, "<Tab> để xem bảng xếp hạng", "MessageTab", "");
            Display.InputNameBox(ref IndexOfSaveBox);
            string YourName = "";

            InputKey(ref key, Display.MaximumLengthOfPlayersName, ref YourName, IndexOfSaveBox, new ConsoleKey[] { ConsoleKey.Enter, ConsoleKey.Tab });
            if (key.Key == ConsoleKey.Tab)
            {
                key = new ConsoleKeyInfo();
                while (true)
                {
                    if (key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.Tab) goto Return;
                    Display.Bangxephang();
                    Display.Clear(0, 0, 0, 0, "MessageTab");
                    Display.Print(0, 0, "Nhấn <Tab> để trở lại", "", "");
                    Rank(Display.PositionName, Display.PositionScore, Display.PositionDateTime, Display.LengthOfTheColumn);
                    key = Console.ReadKey(true);
                }
            }

            Console.Clear();
            Display.Print(2, 0, "<Esc> thoát", "", "");
            Display.Print(60, 0, "Sử dụng các phím A, W, D, mũi tên, phím cách ", "", "");

            // Khủng long chạy
            Thread Running = new Thread(() =>
            {
                while (key.Key != ConsoleKey.Escape && !Exit)
                {
                    if (heightjumping == 0)
                        for (int i = 0; i < Display.TheNumberOfTrexImages; i++)
                        {
                            Display.Trex(BasePositionOfTheTrex[0] + leftorright, BasePositionOfTheTrex[1], i, ref ShapOfTrex);
                            if (heightjumping != 0)
                            {
                                Display.Clear(0, 0, 0, 0, String.Format("Trex,{0},{1}", Display.PreviousPositionOfTrex[0], Display.PreviousPositionOfTrex[1]));
                                break;
                            }
                            Display.Clear(0, 0, 0, 0, "Name");
                            Display.Print(ShapOfTrex.Key[0] + (Display.WidthOfTheTrex - YourName.Length) / 2 + 3, ShapOfTrex.Key[1] - 2, YourName, "Name", "Blue");
                            Thread.Sleep(Level[NumOfLvl, 1] * 2);
                        }
                }
            });
            Running.Start();
            Display.Trex(BasePositionOfTheTrex[0] + leftorright, BasePositionOfTheTrex[1], 0, ref ShapOfTrex);
            Display.Print(ShapOfTrex.Key[0] + (Display.WidthOfTheTrex - YourName.Length) / 2 + 3,
                ShapOfTrex.Key[1] - 2 - heightjumping,
                YourName,
                String.Format("Name,{0},{1}", ShapOfTrex.Key[0] + (Display.WidthOfTheTrex - YourName.Length) / 2 + 3,
                ShapOfTrex.Key[1] - 2 - heightjumping),
                "Blue");

            bool IsCactusOrPteroSpawned = false;

            // Thằn lằn bay
            Thread Pterodactyls = new Thread(() =>
            {
                int move = 0,
                    spawnptero = 0,
                    ratio = 5,
                    stackofpteroes = 0;
                while (key.Key != ConsoleKey.Escape && !Exit)
                {
                    if (move >= Display.LenghtOfTheRoad - 1) move = 0;

                    // Lệnh sinh ra thằn lằn bay, tương tự như của xương rồng
                    if ((new Random()).Next(1, ratio) == 1 && spawnptero > 8 && score > 30 && stackofpteroes < 1 && IsCactusOrPteroSpawned)
                    {
                        spawnptero = 0;
                        Display.Ptero(Console.BufferWidth - 5, true, ref ShapOfPteroes);
                        stackofpteroes++;
                    }
                    else
                        Display.Ptero(Console.BufferWidth - 5, false, ref ShapOfPteroes);

                    if (OstacleChecking(ShapOfTrex, ShapOfPteroes, ShapOfCacti)) Exit = true;

                    if (spawnptero >= 30 && (new Random()).Next(1, 2) == 2)
                        IsCactusOrPteroSpawned = !IsCactusOrPteroSpawned;

                    if (spawnptero >= 100) stackofpteroes = 0;

                    Thread.Sleep(5);

                    move++;
                    spawnptero++;

                    if (score == 50) ratio = 3;
                }
            });
            Pterodactyls.Start();

            // Phong cảnh dịch chuyển
            Thread Scenery = new Thread(() =>
            {
                int move = 0,
                    spawncactus = 0,
                    ratio = 40,
                    stackofcacti = 0;

                while (key.Key != ConsoleKey.Escape && !Exit)
                {
                    if (move >= Display.LenghtOfTheRoad - 1) move = 0;

                    Display.Road(move, ref BasePositionOfTheTrex);

                    // Lệnh sinh cây xương rồng, với (new Random()).Next(a, b), tỉ lệ sinh ra là 1/(b-a+1)
                    // (spawncactus > 40 || (spawncactus < 6 && spawncactus > 3)) không để xương rồng quá nhiều và gần nhau
                    if ((new Random()).Next(1, ratio) == 1 && spawncactus > 3 && !(spawncactus > 6 && spawncactus < 40) && stackofcacti < 3 && !IsCactusOrPteroSpawned)
                    {
                        spawncactus = 0;
                        Display.Cactus(Console.BufferWidth - 1, 0, true, ref ShapOfCacti);
                        stackofcacti++;
                    }
                    else
                        Display.Cactus(0, 0, false, ref ShapOfCacti);

                    if (OstacleChecking(ShapOfTrex, ShapOfPteroes, ShapOfCacti)) Exit = true;

                    if (spawncactus >= 70 && (new Random()).Next(1, 2) == 1)
                        IsCactusOrPteroSpawned = !IsCactusOrPteroSpawned;

                    if (spawncactus >= 30

                    ) stackofcacti = 0;

                    Thread.Sleep((int)Math.Floor((double)(10000 / speed)));

                    move++;
                    spawncactus++;

                    if (score == 50) ratio = 20;
                }
            });
            Scenery.Start();

            // In ra điểm số
            Thread Achievement = new Thread(() =>
            {
                while (key.Key != ConsoleKey.Escape && !Exit)
                {
                    Display.Print(30, 0, "Điểm: " + score, "", "");
                    Thread.Sleep(200000 / speed);
                    score++;
                    switch (score)
                    {
                        case 10: NumOfLvl = 1; break;
                        case 30: NumOfLvl = 2; break;
                        case 50: NumOfLvl = 3; break;
                        case 80: NumOfLvl = 4; break;
                        case 120: NumOfLvl = 5; break;
                        case 150: NumOfLvl = 6; break;
                    }
                    speed = Level[NumOfLvl, 0];
                }
            });
            Achievement.Start();

            Thread Quit = new Thread(() =>
            {
                while (true)
                {
                    if (Exit)
                    {
                        Display.GameOver();
                        if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Escape)
                            break;
                    }
                }
            });
            Quit.Start();

            // Xử lý phím từ người dùng
            do
            {
                if (!Exit) key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape && !Exit) Environment.Exit(0);
                if ((key.Key == ConsoleKey.A || key.Key == ConsoleKey.LeftArrow) && leftorright >= 0)
                {
                    if (heightjumping == 0) leftorright -= 5;
                    else leftorright--;
                    Display.Clear(0, 0, 0, 0, "Name");
                    Display.Print(ShapOfTrex.Key[0] + (Display.WidthOfTheTrex - YourName.Length) / 2 + 3, ShapOfTrex.Key[1] - 2, YourName, "Name", "Blue");
                    if (OstacleChecking(ShapOfTrex, ShapOfPteroes, ShapOfCacti)) Exit = true;
                }

                if ((key.Key == ConsoleKey.D || key.Key == ConsoleKey.RightArrow) && leftorright <= 40)
                {
                    if (heightjumping == 0) leftorright += 5;
                    else leftorright++;
                    Display.Clear(0, 0, 0, 0, "Name");
                    Display.Print(ShapOfTrex.Key[0] + (Display.WidthOfTheTrex - YourName.Length) / 2 + 3, ShapOfTrex.Key[1] - 2, YourName, "Name", "Blue");
                    if (OstacleChecking(ShapOfTrex, ShapOfPteroes, ShapOfCacti)) Exit = true;
                }

                if (key.Key == ConsoleKey.Spacebar || key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.W)
                {
                    Thread jump = new Thread(() =>
                    {
                        // Nhảy lên
                        for (int j = 0; j < maxjump; j++)
                        {
                            heightjumping = j;
                            Display.Trex(BasePositionOfTheTrex[0] + leftorright, BasePositionOfTheTrex[1] - j, 0, ref ShapOfTrex);
                            Display.Clear(0, 0, 0, 0, "Name");
                            Display.Print(ShapOfTrex.Key[0] + (Display.WidthOfTheTrex - YourName.Length) / 2 + 3, ShapOfTrex.Key[1] - 2, YourName, "Name", "Blue");
                            if (OstacleChecking(ShapOfTrex, ShapOfPteroes, ShapOfCacti)) { Exit = true; goto Quit; }

                            Thread.Sleep(Level[NumOfLvl, 1] / 2 * 3 + (int)Math.Pow(heightjumping, 19 / 10));
                        }
                        for (int j = 0; j < 1; j++)
                        {
                            heightjumping = maxjump;
                            if (OstacleChecking(ShapOfTrex, ShapOfPteroes, ShapOfCacti)) { Exit = true; goto Quit; }

                            Thread.Sleep(Level[NumOfLvl, 1]);
                        }
                        // Rớt xuống
                        for (int j = maxjump; j >= 0; j--)
                        {
                            heightjumping = j;
                            Display.Trex(BasePositionOfTheTrex[0] + leftorright, BasePositionOfTheTrex[1] - j, 0, ref ShapOfTrex);
                            Display.Clear(0, 0, 0, 0, "Name");
                            Display.Print(ShapOfTrex.Key[0] + (Display.WidthOfTheTrex - YourName.Length) / 2 + 3, ShapOfTrex.Key[1] - 2, YourName, "Name", "Blue");
                            if (OstacleChecking(ShapOfTrex, ShapOfPteroes, ShapOfCacti)) { Exit = true; goto Quit; }

                            Thread.Sleep(Level[NumOfLvl, 1] / 2 * 3 + (int)Math.Pow(heightjumping, 19 / 10));
                        }
                    Quit:;
                    });

                    if (heightjumping == 0)
                    {
                        jump.Start();
                        if (Exit) goto Exit;
                    }
                }
            } while (!Exit);
        Exit:;
            while (true)
            {
                Display.GameOver();
                if (key.Key == ConsoleKey.Escape) goto Return;
                if (key.Key == ConsoleKey.Enter) { Console.Clear(); break; }
                key = Console.ReadKey(true);
            }
            Display.SaveBox(ref IndexOfSaveBox);
            InputKey(ref key, Display.MaximumLengthOfPlayersName, ref YourName, IndexOfSaveBox, new ConsoleKey[] { ConsoleKey.Enter });
            SaveAchievement(YourName, score);
            goto Return;
        }
        // Khoá để chỉ cho một luồng truy cập vào một thời điểm
        static readonly object Lock = new object();

        // Kiểm tra va chạm
        static bool OstacleChecking(KeyValuePair<int[], bool[,]> ShapOfTrex, KeyValuePair<int[][], bool[][,]> ShapOfPteroes, KeyValuePair<int[][], bool[][,]> ShapOfCacti)
        {
            KeyValuePair<int[][], bool[][,]> UseForComparision = new KeyValuePair<int[][], bool[][,]>();

            lock (Lock)
            {
                bool Crash = false;

                for (int h = 0; h < 2; h++)
                {
                    if (h == 0)
                        UseForComparision = ShapOfPteroes;

                    if (h == 1)
                        UseForComparision = ShapOfCacti;

                    for (int i = 0; i < UseForComparision.Key.Length; i++)
                        for (int j = 0; j < ShapOfTrex.Value.GetLength(0); j++)
                            for (int k = 0; k < ShapOfTrex.Value.GetLength(1); k++)
                            {
                                int Index1 = k + UseForComparision.Key[i][0] - ShapOfTrex.Key[0];
                                int Index2 = j + UseForComparision.Key[i][1] - ShapOfTrex.Key[1];

                                if (Index1 + 1 < UseForComparision.Value[i].GetLength(0) && Index2 < UseForComparision.Value[i].GetLength(1) && Index1 + 1 >= 0 && Index2 >= 0)
                                    if (ShapOfTrex.Value[j, k] && UseForComparision.Value[i][Index1 + 1, Index2])
                                        Crash = true;

                                if (Index1 - 1 < UseForComparision.Value[i].GetLength(0) && Index2 < UseForComparision.Value[i].GetLength(1) && Index1 - 1 >= 0 && Index2 >= 0)
                                    if (ShapOfTrex.Value[j, k] && UseForComparision.Value[i][Index1 - 1, Index2])
                                        Crash = true;

                                if (Index1 < UseForComparision.Value[i].GetLength(0) && Index2 + 1 < UseForComparision.Value[i].GetLength(1) && Index1 >= 0 && Index2 + 1 >= 0)
                                    if (ShapOfTrex.Value[j, k] && UseForComparision.Value[i][Index1, Index2 + 1])
                                        Crash = true;

                                if (Index1 < UseForComparision.Value[i].GetLength(0) && Index2 - 1 < UseForComparision.Value[i].GetLength(1) && Index1 >= 0 && Index2 - 1 >= 0)
                                    if (ShapOfTrex.Value[j, k] && UseForComparision.Value[i][Index1, Index2 - 1])
                                        Crash = true;
                            }
                }
                return Crash;
            }
        }
        static void InputKey(ref ConsoleKeyInfo key, int MaximumLengthOfString, ref string Text, int[] PositionOfText, ConsoleKey[] KeyToStopTying)
        {
            Console.CursorVisible = true;
            int movecursor = Text.Length;
            Console.SetCursorPosition(PositionOfText[0], PositionOfText[1]);
            while (true)
            {
                Display.Clear(PositionOfText[0], PositionOfText[1], Display.MaximumLengthOfPlayersName, 1, "");
                if ((Char.IsLetterOrDigit(key.KeyChar) || key.Key == ConsoleKey.Spacebar) && Text.Length < MaximumLengthOfString)
                {
                    Text += key.KeyChar;
                    movecursor = Text.Length;
                }
                Console.SetCursorPosition(PositionOfText[0], PositionOfText[1]);
                Console.Write(Text);
                key = Console.ReadKey(true);
                switch (key.Key)
                {
                    //case ConsoleKey.LeftArrow: if (movecursor > 0) movecursor--; break;
                    //case ConsoleKey.RightArrow: if (movecursor < Text.Length) movecursor++; break;
                    case ConsoleKey.Backspace:
                        if (movecursor > 0)
                        {
                            if (movecursor != Text.Length)
                                Text = string.Concat(Text.Substring(0, movecursor).Remove(Text.Substring(0, movecursor).Length - 1), (Text.Substring(movecursor)));
                            else Text = Text.Remove(Text.Length - 1);
                            movecursor--;
                        }
                        break;
                    case ConsoleKey.Escape: Environment.Exit(0); break;
                }
                for (int i = 0; i < KeyToStopTying.Length; i++)
                    if (key.Key == KeyToStopTying[i]) goto Exit;
            }
        Exit:;
            Console.CursorVisible = false;
        }
        static void SaveAchievement(string Name, int score)
        {
            if (Name != "" && !String.IsNullOrWhiteSpace(Name))
            {
                bool ContainName = false;
                File.AppendAllText("DinosaurResult.txt", "");
                string[] OpenSaveFile = File.ReadAllLines("DinosaurResult.txt");

                for (int i = 0; i < OpenSaveFile.Length / 3; i += 3)
                {
                    if (OpenSaveFile[i * 3].Contains(Name))
                    {
                        if (score > int.Parse(OpenSaveFile[i + 1]))
                        {

                            OpenSaveFile[i * 3 + 1] = score.ToString();
                            OpenSaveFile[i * 3 + 2] = DateTime.Now.ToString("HH:mm dd/MM/yyyy");
                        }
                        ContainName = true;
                    }
                    if (ContainName) break;
                }
                if (!ContainName)
                    OpenSaveFile = OpenSaveFile.Concat(new string[] { Name, score.ToString(), DateTime.Now.ToString("HH:mm dd/MM/yyyy") }).ToArray();

                File.WriteAllLines("DinosaurResult.txt", OpenSaveFile);
            }
        }
        static void Rank(int[] PositionName, int[] PositionScore, int[] PositionDateTime, int LenghtOfColumn)
        {
            File.AppendAllText("DinosaurResult.txt", "");
            string[] OpenSaveFile = File.ReadAllLines("DinosaurResult.txt");
            string[] temp = new string[3];
            for (int h = 0; h < Math.Pow(OpenSaveFile.Length / 3, 2); h++)
                for (int i = 0; i < OpenSaveFile.Length / 3 - 1; i++)
                {
                    for (int j = 0; j < 3; j++)
                        temp[j] = OpenSaveFile[i * 3 + j];

                    if (int.Parse(OpenSaveFile[i * 3 + 1]) < int.Parse(OpenSaveFile[i * 3 + 4]))
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            OpenSaveFile[i * 3 + j] = OpenSaveFile[i * 3 + j + 3];
                            OpenSaveFile[i * 3 + j + 3] = temp[j];
                        }
                    }
                }
            if (OpenSaveFile.Length / 3 > LenghtOfColumn)
            {
                string[] temparray = new string[LenghtOfColumn * 3];
                for (int i = 0; i < temparray.Length; i++)
                    temparray[i] = OpenSaveFile[i];

                OpenSaveFile = temparray;
            }

            Display.Clear(PositionName[0], PositionName[1], 30, Display.LengthOfTheColumn, "");
            for (int i = 0; i < OpenSaveFile.Length / 3; i++)
            {
                Display.Print(PositionName[0] + 3 - (i + 1).ToString().Length, PositionName[1] + i, (i + 1) + "  " + OpenSaveFile[i * 3], "", "");
                Display.Print(PositionScore[0], PositionScore[1] + i, OpenSaveFile[i * 3 + 1], "", "");
                Display.Print(PositionDateTime[0], PositionName[1] + i, OpenSaveFile[i * 3 + 2], "", "");
            }
        }
    }
    class Display
    {
        public static int
            TheNumberOfTrexImages = 0,      // Số lượng image của cho T-rex
            LenghtOfTheRoad = 0,            // Độ dài image con đường
            WidthOfTheTrex = 0,             // Chiều rộng
            BufferWidth = Console.BufferWidth,
            BufferHeigth = Console.BufferHeight;

        public static int[] PreviousPositionOfTrex = new int[2] { 0, 0 };     // Vị trí trước đó của T-re

        private static List<int> PreviousPositionOfCacti = new List<int>(); // Vị trí trước đó của các cây xương rồng mọc trên đường, dùng để xoá vết in khi các cây xương rồng dịch chuyển
        private static List<int> PreviousPositionOfPteroes = new List<int>(); // Vị trí trước đó của thằn lằn bay, tương tự xương rồng

        public static int[] BasePositionOfObjects = { 1, Console.BufferHeight / 2 + 3 }; // Vị trí gốc cho toàn bộ đối tượng

        // Cho một luồng truy cập vào một đoạn code khi sử dụng với từ khoá 'lock'
        // Khi luồng này truy cập vào thì các luồng khác sẽ đợi khi luồng đó truy cập xong rồi mới được truy cập
        private static readonly object locker = new object();

        public static void Reset(ref int[] BasePositionOfTheTrex)
        {
            PreviousPositionOfCacti = new List<int>();
            PreviousPositionOfPteroes = new List<int>();

            BasePositionOfTheTrex = new int[] { BasePositionOfObjects[0] + 15, BasePositionOfObjects[1] - 1 };
        }
        public static void NameGame()
        {
            string[] Name = new string[]
            {

                @"  ▁▁▁▁▁▁   ▁▁                                                          ",
                @" /      \ /  │                                                         ",
                @"/$$$$$$  \$$/▁  ▁▁▁▁▁▁▁    ▁▁▁▁▁    ▁▁▁▁▁▁   ▁▁▁▁▁   ▁▁    ▁▁  ▁▁▁▁▁▁▁ ",
                @"$$ │  $$  │/  │/       \  /     \  /      │ /     \ /  │  /  │/     / \",
                @"$$ │   $$ │$$ │$$$$$$$  │ $$$$$  │ $$$$$$/  $$$$   │$$ │  $$ │$$ $$$$ │",
                @"$$ │   $$ │$$ │$$ │  $$ │$$ │ $$ │$$ │   \  /  $$  │$$ │  $$ │$$$\ ▁▁/ ",
                @"$$ │   $$ │$$ │$$ │  $$ │$$ │ $$ │ $$$$$  │/$$$$$  │$$ \▁▁$$ │$$$ /    ",
                @"$$╱   $$ / $$ │$$ │  $$ │$$ │ $$ │     $$/ $$   $$ │$$    $$/ $$ │     ",
                @" $$$$$$ /  $$/ $$ /  $$ / $$$$$ / $$$$$$/  $$$$$$ /  $$$$$$/  $$▁/     "
            };
            Print((Console.BufferWidth - Name[0].Length) / 2, Console.BufferHeight / 2 - 6, Name, "", "DarkYellow");
        }
        public static void Trex(int PositionX, int PositionY, int run, ref KeyValuePair<int[], bool[,]> ShapOfTrex)
        {
            string[][] ImagesOfTrex = new string[3][];
            ImagesOfTrex[0] = new string[]
            {
                "        ▄▄▄▄ ",
                "        █▄███",
                "█▄    ▄████  ",
                " ██▄▄▄████▄  ",
                "  ▀██████▀ ▀ ",
                "     █ █▄    "
            };
            ImagesOfTrex[1] = new string[]
            {
                "        ▄▄▄▄ ",
                "        █▄███",
                "█▄    ▄████  ",
                " ██▄▄▄████▄  ",
                "  ▀██████▀ ▀ ",
                "     ▀ █▄    "
            };
            ImagesOfTrex[2] = new string[]
            {
                "        ▄▄▄▄ ",
                "        █▄███",
                "█▄    ▄████  ",
                " ██▄▄▄████▄  ",
                "  ▀██████▀ ▀ ",
                "     █ ▀▀    "
            };

            WidthOfTheTrex = ImagesOfTrex[0][0].Length;
            TheNumberOfTrexImages = ImagesOfTrex.Length;

            // Set bitmap cho khủng long
            bool[,] Bitmap = new bool[ImagesOfTrex[0].Length, ImagesOfTrex[0][0].Length];

            for (int i = 0; i < Bitmap.GetLength(0); i++)
                for (int j = 0; j < Bitmap.GetLength(1); j++)
                    if (ImagesOfTrex[0][i][j] != ' ') Bitmap[i, j] = true;

            int[] Position = { PositionX, PositionY, };
            ShapOfTrex = new KeyValuePair<int[], bool[,]>(Position, Bitmap);

            // Xoá vết in của khủng long
            Clear(0, 0, 0, 0, String.Format("Trex,{0},{1}", PreviousPositionOfTrex[0], PreviousPositionOfTrex[1]));

            // In ra khủng long
            if (PositionY == BasePositionOfObjects[1] - 1)
                Print(PositionX, PositionY, ImagesOfTrex[run], String.Format("Trex,{0},{1}", PositionX, PositionY), "");
            else
                Print(PositionX, PositionY, ImagesOfTrex[0], String.Format("Trex,{0},{1}", PositionX, PositionY), "");

            PreviousPositionOfTrex = new int[] { PositionX, PositionY };
        }
        public static void Ptero(int initialization, bool spawn, ref KeyValuePair<int[][], bool[][,]> ShapOfPteroes)
        {
            string[][] Ptero = new string[2][];
            Ptero[0] = new string[]
            {
                "             ",
                "     █▄      ",
                "   ▄ ███     ",
                "▄███▄ ███    ",
                "   ▀██████▄▄ ",
                "     ▀▀▀▀▄▄ ▀",
                "             ",
                "             ",
            };
            Ptero[1] = new string[]
            {
                "             ",
                "             ",
                "   ▄         ",
                "▄███▄ ▄▄▄    ",
                "   ▀██████▄▄ ",
                "     ███▀▄▄ ▀",
                "    ██▀      ",
                "    ▀        "
            };

            int usepicture;

            // Set bitmap cho thằn lằn bay
            int[][] Position = new int[PreviousPositionOfPteroes.Count][];
            bool[][,] Bitmap = new bool[PreviousPositionOfPteroes.Count][,];

            usepicture = 0; // Sử dụng pic quạt cánh lên hay xuống, tạo hoạt ảnh cho thằn lằn bay

            for (int i = 0; i < PreviousPositionOfPteroes.Count; i++)
            {
                Position[i] = new int[] { BasePositionOfObjects[0] + PreviousPositionOfPteroes[i] - 18, BasePositionOfObjects[1] - Program.maxjump - 2 };
                if (PreviousPositionOfPteroes[i] % 12 < 4) usepicture = 1;

                Bitmap[i] = new bool[Ptero[usepicture].Length, Ptero[usepicture][0].Length];

                for (int j = 0; j < Ptero[usepicture].Length; j++)
                    for (int k = 0; k < Ptero[usepicture][0].Length; k++)
                        if (Ptero[usepicture][j][k] != ' ')
                            Bitmap[i][j, k] = true;
            }
            ShapOfPteroes = new KeyValuePair<int[][], bool[][,]>(Position, Bitmap);

            // Lệnh sinh ra thằn lằn bay
            if (spawn) PreviousPositionOfPteroes.Add(initialization);

            // In ra thằn lằn bay
            for (int i = 0; i < PreviousPositionOfPteroes.Count; i++)
            {
                Clear(0, 0, 0, 0, "Ptero" + i);

                Print(BasePositionOfObjects[0] - 12 + PreviousPositionOfPteroes[i], BasePositionOfObjects[1] - Program.maxjump - 2, Ptero[usepicture], "Ptero" + i, "DarkMagenta");

                PreviousPositionOfPteroes[i]--;
            }
            // Xoá thằn lằn bay đã bay hết đường
            for (int i = 0; i < PreviousPositionOfPteroes.Count; i++)
                if (PreviousPositionOfPteroes[i] <= 1)
                {
                    PreviousPositionOfPteroes.RemoveAt(i);
                    ShapOfPteroes.Key[i] = new int[2] { 0, 0 };
                    ShapOfPteroes.Value[i] = new bool[1, 1] { { false } };
                    Clear(0, 0, 0, 0, "Ptero" + PreviousPositionOfPteroes.Count);
                }

        }
        public static void Cactus(int initialization, byte type, bool spawn, ref KeyValuePair<int[][], bool[][,]> ShapOfCacti)
        {
            string[][] Cactus = new string[1][];
            Cactus[0] = new string[]
            {
                "  ▄▄  ",
                "█ ██  ",
                "█▄██ █",
                "  ██▀▀",
                "  ██  "
            };

            // Set bitmap cho xương rồng
            int[][] Position = new int[PreviousPositionOfCacti.Count][];
            bool[][,] Bitmap = new bool[PreviousPositionOfCacti.Count][,];

            for (int i = 0; i < PreviousPositionOfCacti.Count; i++)
            {
                Position[i] = new int[] { BasePositionOfObjects[0] - 15 + PreviousPositionOfCacti[i], BasePositionOfObjects[1] };
                Bitmap[i] = new bool[Cactus[type].Length, Cactus[type][0].Length];

                for (int j = 0; j < Cactus[type].Length; j++)
                    for (int k = 3; k < Cactus[type][0].Length; k++)
                        if (Cactus[type][j][k] != ' ')
                            Bitmap[i][j, k] = true;
            }
            ShapOfCacti = new KeyValuePair<int[][], bool[][,]>(Position, Bitmap);

            // Lệnh sinh ra xương rồng
            if (spawn) PreviousPositionOfCacti.Add(initialization);

            // In các cây xương rồng
            for (int i = 0; i < PreviousPositionOfCacti.Count; i++)
            {
                Clear(0, 0, 0, 0, "Cactus" + i);

                Print(BasePositionOfObjects[0] - 6 + PreviousPositionOfCacti[i], BasePositionOfObjects[1], Cactus[type], "Cactus" + i, "Green");

                PreviousPositionOfCacti[i]--;
            }

            // Xoá cây xương rồng đã chạy đến cuối đường
            for (int i = 0; i < PreviousPositionOfCacti.Count; i++)
                if (PreviousPositionOfCacti[i] <= 1)
                {
                    PreviousPositionOfCacti.RemoveAt(i);
                    ShapOfCacti.Key[i] = new int[2] { 0, 0 };
                    ShapOfCacti.Value[i] = new bool[1, 1] { { false } };
                    Clear(0, 0, 0, 0, "Cactus" + PreviousPositionOfCacti.Count);
                }
        }
        public static void Road(int move, ref int[] BasePositionOfTheTrex)
        {
            string[] road = new string[]
            {
                "▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄   ▄▄▄▄▄▄▄▄▄▀▀▀█▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄",
                "     ▀  ▄▄       ▄▄           ▀▀▀                  ▄▄               ",
                "▀▀                                   █                              "
            };
            LenghtOfTheRoad = road[0].Length;

            // In ra con đường
            string[] DisplayRoad = new string[] { "", "", "" };
            int LengthOfDisplayRoad = 0;

            while (LengthOfDisplayRoad < Console.BufferWidth - 2)
            {
                for (int i = 0; i < DisplayRoad.Length; i++)
                {
                    if (LengthOfDisplayRoad == 0)
                        DisplayRoad[i] += road[i].Substring(move);
                    else
                        DisplayRoad[i] += road[i];
                }
                LengthOfDisplayRoad = DisplayRoad[0].Length;
            }
            for (int i = 0; i < DisplayRoad.Length; i++)
                DisplayRoad[i] = DisplayRoad[i].Remove(Console.BufferWidth - 2);

            Print(BasePositionOfObjects[0], 5 + BasePositionOfObjects[1], DisplayRoad, "", "DarkYellow");

            BasePositionOfTheTrex = new int[] { BasePositionOfObjects[0] + 15, BasePositionOfObjects[1] - 1 };
        }
        public static void GameOver()
        {
            string[] gameover = new string[]
            {
                "G A M E  O V E R !",
                "    ┏━━━━━━━┓     ",
                "    ┃ Enter ┃     ",
                "    ┗━━━━━━━┛     ",
                "   Lưu kết quả    ",
                " <Esc> Không lưu  "
            };
            Print((Console.BufferWidth - gameover[0].Length) / 2, (Console.BufferHeight - gameover.Length) / 2 - 3, gameover, "", "");
        }
        public static int MaximumLengthOfPlayersName = 0;
        public static void InputNameBox(ref int[] IndexOfSaveBox)
        {
            string[] savebox = new string[]
            {
                "   Thông tin người chơi  ",
                "┏━━━━━━━━━━━━━━━━━━━━━━━┓",
                "┃ Tên:                  ┃",
                "┗━━━━━━━━━━━━━━━━━━━━━━━┛",
                "  Nhấn <Enter> hoàn tất  "
            };

            int x = (Console.WindowWidth - savebox[0].Length) / 2,
                y = (Console.WindowHeight - savebox.Length) / 2;

            MaximumLengthOfPlayersName = savebox[0].Length - 9;
            Print(x, y, savebox, "SaveBox", "");
            Console.SetCursorPosition(x + 7, y + 2);
            IndexOfSaveBox[0] = x + 7;
            IndexOfSaveBox[1] = y + 2;
        }
        public static void SaveBox(ref int[] IndexOfSaveBox)
        {
            string[] savebox = new string[]
            {
                "      Hộp thoại lưu      ",
                "┏━━━━━━━━━━━━━━━━━━━━━━━┓",
                "┃ Tên:                  ┃",
                "┗━━━━━━━━━━━━━━━━━━━━━━━┛",
                "  Nhấn <Enter> hoàn tất  "
            };

            int x = (Console.WindowWidth - savebox[0].Length) / 2,
                y = (Console.WindowHeight - savebox.Length) / 2;

            MaximumLengthOfPlayersName = savebox[0].Length - 9;
            Print(x, y, savebox, "SaveBox", "");
            Console.SetCursorPosition(x + 7, y + 2);
            IndexOfSaveBox[0] = x + 7;
            IndexOfSaveBox[1] = y + 2;
        }
        public static int[] PositionName = new int[2];
        public static int[] PositionScore = new int[2];
        public static int[] PositionDateTime = new int[2];
        public static int LengthOfTheColumn = 0;
        public static void Bangxephang()
        {
            string[] bangxephang = new string[]
            {
                @"                                   BẢNG XẾP HẠNG                                    ",
                @"  \━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━  ",
                @"   \\   / /                                                               / /   |   ",
                @" ==>/  / /                                                               / /    <== ",
                @"   /  / /                                                               / /    /    ",
                @" ==>   /\                                                                 \   /<==  ",
                @"    \/   \                                                                 \ / /    ",
                @" ==>/     \                                                                /  /<==  ",
                @"   / /   /                                                                /  /      ",
                @" ==> \  /                                                                /  / \ <== ",
                @"    \ \/                                                                  \ \  \    ",
                @"  ==>\                                                                    /\ \  \<==",
                @"     /  \                                                                /  \ \/ \  ",
                @" ==>/  \ \                                                              /    \/<==  ",
                @"    \   \ \                                                           \\ \   /\     ",
                @"  ==>\   \ \                                                            \ \ /<==    ",
                @"     ━━━━\━━\━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━/━━━━━━  ",
            };
            int vt1 = (Console.BufferWidth - bangxephang[0].Length) / 2,
                vt2 = (Console.BufferHeight - bangxephang.Length) / 2;
            PositionName[0] = vt1 + 16;
            PositionName[1] = vt2 + 2;
            PositionScore[0] = vt1 + 45;
            PositionScore[1] = vt2 + 2;
            PositionDateTime[0] = vt1 + 52;
            PositionDateTime[1] = vt2 + 2;
            LengthOfTheColumn = bangxephang.Length - 3;

            Print(vt1, vt2, bangxephang, "", "DarkYellow");

        }

        // Dùng để lưu một hình đã in, sau đó hàm Clear() sử dụng lại để xoá nhanh vết in đó.
        private static Dictionary<string, int[,]> SavePrints = new Dictionary<string, int[,]>();

        // In ra một hình với vị trí (x, y), nếu cần lưu lại trong 'SavePrint' thì đặt tên cho 'NameImage', nếu không thì đặt trống: ""
        public static void Print<T>(int x, int y, T print, string NameImage, string color)
        {
        Return:;
            try
            {
                lock (locker)
                {
                    ConsoleColor ColorOfImage = new ConsoleColor();
                    if (Enum.TryParse(color, out ColorOfImage))
                        Console.ForegroundColor = ColorOfImage;

                    int[,] MarkOfPrint = new int[0, 0];
                    if (print is string)
                    {
                        if (x >= 0 && y >= 0 && x + ((dynamic)print).Length < Console.BufferWidth && y < Console.BufferHeight)
                        {
                            Console.SetCursorPosition(x, y);
                            Console.Write(print);

                            MarkOfPrint = new int[1, 3];
                            MarkOfPrint[0, 0] = x;
                            MarkOfPrint[0, 1] = y;
                            MarkOfPrint[0, 2] = ((dynamic)print).Length;
                        }
                    }
                    if (print is string[])
                    {
                        MarkOfPrint = new int[((dynamic)print).Length, 3];
                        for (int i = 0; i < ((dynamic)print).Length; i++)
                        {
                            if (x >= 0 && y >= 0 && x + ((dynamic)print)[i].Length < Console.BufferWidth && y + ((dynamic)print).Length < Console.BufferHeight)
                            {
                                Console.SetCursorPosition(x, y + i);
                                Console.Write(((dynamic)print)[i]);

                                MarkOfPrint[i, 0] = x;
                                MarkOfPrint[i, 1] = y + i;
                                MarkOfPrint[i, 2] = ((dynamic)print)[i].Length;
                            }
                        }
                    }
                    if (!(print is string) && !(print is string[]))
                        throw new Exception("Kiểu dữ liệu không hợp lệ, phải là kiểu string hoặc string[].");

                    if (NameImage != "" && MarkOfPrint != null)
                    {
                        if (!SavePrints.ContainsKey(NameImage))
                            SavePrints.Add(NameImage, MarkOfPrint);
                    }
                    Console.ResetColor();
                }
                if (BufferWidth != Console.BufferWidth || BufferHeigth != Console.BufferHeight)
                    throw new Exception();
            }
            catch
            {
                BufferWidth = Console.BufferWidth;
                BufferHeigth = Console.BufferHeight;
                Console.Clear();
                if (Console.BufferWidth < 120 || Console.BufferHeight < 30)
                {
                    string message = "Kích thước cửa sổ quá nhỏ!";
                    if (Console.BufferWidth > message.Length) Print((Console.BufferWidth - message.Length) / 2, Console.BufferHeight / 2, message, "", "Red");
                }
                while (Console.BufferWidth < 120 || Console.BufferHeight < 30) { };
                goto Return;
            }
        }
        // Xoá một khoảng trên màn hình ở vị trí (x, y), độ dài chiều ngang, dọc (w, h); hoặc sử dụng 'NameImage' đã lưu để xoá nhanh một hình
        public static void Clear(int x, int y, int w, int h, string NameImage)
        {
        Return:;
            try
            {
                lock (locker)
                {
                    string c = "";

                    for (int i = 0; i < w; i++)
                        c += " ";
                    for (int i = 0; i < h; i++)
                        if (x + w < Console.BufferWidth && y + i < Console.BufferHeight)
                        {
                            Console.SetCursorPosition(x, y + i);
                            Console.Write(c);
                        }

                    if (SavePrints.ContainsKey(NameImage))
                    {
                        for (int i = 0; i < SavePrints[NameImage].GetLength(0); i++)
                            Clear(SavePrints[NameImage][i, 0], SavePrints[NameImage][i, 1], SavePrints[NameImage][i, 2], 1, "");
                        SavePrints.Remove(NameImage);
                    }
                }
            }
            catch
            {
                while (Console.BufferWidth < 120 || Console.BufferHeight < 30) { };
                goto Return;
            }
        }
    }
}
