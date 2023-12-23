using System.Text;
using static ConsoleTetris.Figure;

namespace ConsoleTetris
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Figure figure = new(Figures.T, 5, 0);
            Field field = new(figure);
            Renderer renderer = new(field);
            renderer.Draw();
            Console.Title = "TETRIS";
            Task.Run(renderer.PlaySong);
            var autoEvent = new AutoResetEvent(true);
            var fallTimer = new Timer(renderer.TimeToFall,
                                   autoEvent, 1000, 1000);
            while (true)
            {
                renderer.RenderNextFrame();
            }
        }
    }
    class Cube(int x, int y)
    {
        int x = x, y = y;

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        public int Y
        {
            get { return y; }
            set { y = value; }
        }
    }
    class Figure : ICloneable
    {
        List<Cube> cubes;
        Figures figure;
        RotatedTo rotatedTo;
        int mainX;
        int mainY;
        public enum Figures : int//https://dzen.ru/a/XfdkoN3-9gCwAT89 названия фигур взяты отсюда
        {
            T = 1,
            I = 2,
            O = 3,
            L = 4,
            Z = 5,
            J = 6,
            S = 7,
            Unknown
        }
        public enum RotateDirection
        {
            Left,
            Right
        }
        public enum MoveDirection
        {
            ToLeft,
            ToRight
        }
        public enum RotatedTo : int
        {
            Default = 1,
            ToRight = 2,
            Upside_Down = 3,
            ToLeft = 4,
        }
        public List<Cube> Cubes
        {
            get { return cubes; }
            set { cubes = value; }
        }
        public Figure()
        {
            cubes = [];
            figure = Figures.Unknown;
            rotatedTo = RotatedTo.Default;
        }
        public Figure(Figures figure, int mainX, int mainY, RotatedTo rotated = RotatedTo.Default)
        {
            this.mainX = mainX;
            this.mainY = mainY;
            this.figure = figure;
            Rotated = rotated;
            cubes = GetStandardFigurePlacementByName(figure, mainX, mainY);
        }
        public Figures FiguresName
        {
            get { return figure; }
            set { figure = value; }
        }
        public RotatedTo Rotated
        {
            get
            { return rotatedTo; }
            set
            { rotatedTo = value; }
        }
        public object Clone()
        {
            return MemberwiseClone();
        }
        public int MainX
        {
            get
            {
                return mainX;
            }
            set
            {
                mainX = value;
            }
        }
        public int MainY
        {
            get
            {
                return mainY;
            }
            set
            {
                mainY = value;
            }
        }
        public void Fall(Field field, int tiles = 1)
        {

            if (field.IsFigureFalled(cubes))
            {
                Random random = new();
                field.FallenFigures = [.. field.FallenFigures, .. cubes];
                field.FallingFigure = new Figure((Figures)random.Next(1, 4), field.StartX, field.StartY);
                Console.WriteLine(field.FallingFigure.FiguresName.ToString());
                return;
            }
            for (int i = 0; i < cubes.Count; i++)
            {
                cubes[i].Y += tiles;
            }
            MainY += tiles;
        }
        public void MoveToLeft(Field field, int tiles = 1)
        {
            if (field.IsFigureCrashed(cubes, MoveDirection.ToLeft) || field.IsFigureNearTheLeftBorder(cubes))
            {
                return;
            }
            for (int i = 0; i < cubes.Count; i++)
            {
                cubes[i].X -= tiles;

            }
            MainX -= tiles;
        }
        public void MoveToRight(Field field, int tiles = 1)
        {
            if (field.IsFigureCrashed(cubes, MoveDirection.ToRight) || field.IsFigureNearTheRightBorder(cubes))
            {
                return;
            }
            for (int i = 0; i < cubes.Count; i++)
            {
                cubes[i].X += tiles;

            }
            mainX += tiles;
        }
        public List<Cube> GetStandardFigurePlacementByName(Figures figuresName, int mainX, int mainY)
        {
            int x = mainX;
            int y = mainY;
            List<Cube> cubes = [];
            cubes.Add(new Cube(x, y));
            switch (figuresName)
            {
                case Figures.T:
                    {
                        cubes.Add(new Cube(x, y + 1));
                        cubes.Add(new Cube(x - 1, y));
                        cubes.Add(new Cube(x + 1, y));
                        return cubes;
                    }
                case Figures.I:
                    {
                        cubes.Add(new Cube(x, y + 1));
                        cubes.Add(new Cube(x, y - 1));
                        cubes.Add(new Cube(x, y - 2));
                        return cubes;
                    }
                case Figures.L:
                    {
                        cubes.Add(new Cube(x, y + 1));
                        cubes.Add(new Cube(x, y - 1));
                        cubes.Add(new Cube(x + 1, y - 1));
                        return cubes;
                    }
                case Figures.O:
                    {
                        cubes.Add(new Cube(x, y - 1));
                        cubes.Add(new Cube(x - 1, y));
                        cubes.Add(new Cube(x - 1, y - 1));
                        return cubes;
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }

            }
        }

    }
    class TFigure : Figure
    {
        List<Cube> cubes = [];
        int mainX;
        int mainY;
        public TFigure(int x, int y)
        {
            mainX = x;
            mainY = y;
            cubes = GetStandardFigurePlacementByName(Figures.T, x, y);
            Cubes = cubes;
            FiguresName = Figures.T;
            Rotated = RotatedTo.Default;
        }
        public TFigure(List<Cube> cubes, int mainX, int mainY, RotatedTo rotated)
        {
            Rotated = rotated;
            FiguresName = Figures.T;
            this.cubes = cubes;
            this.mainX = mainX;
            this.mainY = mainY;
            MainX = mainX;
            MainY = mainY;
        }
        public TFigure(Figure figure)
            : this(figure.Cubes, figure.MainX, figure.MainY, figure.Rotated)
        {

        }
        List<Cube> GetCubesDependingOnRotating(RotatedTo rotatedTo)
        {
            List<Cube> rotatedFigure = [];
            rotatedFigure.Add(new Cube(mainX, mainY));//Главная точка остаётся постоянной при всех поворотах
            switch (rotatedTo)
            {

                case RotatedTo.Default:
                    {

                        rotatedFigure.Add(new Cube(mainX, mainY + 1));
                        rotatedFigure.Add(new Cube(mainX - 1, mainY));
                        rotatedFigure.Add(new Cube(mainX + 1, mainY));
                        return rotatedFigure;
                    }
                case RotatedTo.ToRight:
                    {
                        rotatedFigure.Add(new Cube(mainX, mainY + 1));
                        rotatedFigure.Add(new Cube(mainX + 1, mainY));
                        rotatedFigure.Add(new Cube(mainX, mainY - 1));
                        return rotatedFigure;
                    }
                case RotatedTo.Upside_Down:
                    {
                        rotatedFigure.Add(new Cube(mainX - 1, mainY));
                        rotatedFigure.Add(new Cube(mainX + 1, mainY));
                        rotatedFigure.Add(new Cube(mainX, mainY - 1));
                        return rotatedFigure;
                    }
                case RotatedTo.ToLeft:
                    {
                        rotatedFigure.Add(new Cube(mainX, mainY + 1));
                        rotatedFigure.Add(new Cube(mainX, mainY - 1));
                        rotatedFigure.Add(new Cube(mainX - 1, mainY));
                        return rotatedFigure;
                    }
                default:
                    {
                        throw new Exception();
                    }
            }
        }
        public void Rotate(RotateDirection direction)
        {
            if (direction == RotateDirection.Left)
            {
                int rotatingId = (int)Rotated - 1;
                if (rotatingId == 0)
                    rotatingId = 4;
                Cubes = GetCubesDependingOnRotating((RotatedTo)rotatingId);
                Rotated = (RotatedTo)rotatingId;
            }
            else
            {
                int rotatingId = (int)Rotated + 1;
                if (rotatingId == 5)
                    rotatingId = 1;
                Cubes = GetCubesDependingOnRotating((RotatedTo)rotatingId); ;
                Rotated = (RotatedTo)rotatingId;
            }
        }
        public new int MainX
        {
            get { return base.MainX; }
            set { base.MainX = value; }
        }
        public new int MainY
        {
            get { return base.MainY; }
            set { base.MainY = value; }
        }
    }
    class IFigure : Figure
    {
        List<Cube> cubes;
        int mainX;
        int mainY;

        public IFigure(int mainX, int mainY)
        {
            this.mainX = mainX;
            this.mainY = mainY;
            cubes = GetStandardFigurePlacementByName(Figures.I, mainX, mainY);
            Cubes = cubes;
            FiguresName = Figures.I;
            Rotated = RotatedTo.Default;
        }
        public IFigure(List<Cube> cubes, int mainX, int mainY, RotatedTo rotated)
        {
            Rotated = rotated;
            FiguresName = Figures.I;
            Cubes = Cubes;
            this.cubes = cubes;
            this.mainX = mainX;
            this.mainY = mainY;
        }
        public IFigure(Figure figure)
            : this(figure.Cubes, figure.MainX, figure.MainY, figure.Rotated)
        {

        }
        public void Rotate(RotateDirection direction)
        {
            if (direction == RotateDirection.Left)
            {
                int rotatingId = (int)Rotated - 1;
                if (rotatingId == 0)
                    rotatingId = 4;
                cubes = GetCubesDependingOnRotating((RotatedTo)rotatingId);
                Cubes = cubes;
                Rotated = (RotatedTo)rotatingId;
            }
            else
            {
                int rotatingId = (int)Rotated + 1;
                if (rotatingId == 5)
                    rotatingId = 1;
                cubes = GetCubesDependingOnRotating((RotatedTo)rotatingId);
                Cubes = cubes;
                Rotated = (RotatedTo)rotatingId;
            }
        }
        public new int MainX
        {
            get { return base.MainX; }
            set { base.MainX = value; }
        }
        public new int MainY
        {
            get { return base.MainY; }
            set { base.MainY = value; }
        }
        List<Cube> GetCubesDependingOnRotating(RotatedTo rotatedTo)
        {
            List<Cube> cubes = [];
            cubes.Add(new Cube(mainX, mainY));//главная точка всегда остаётся на месте при повороте
            switch (rotatedTo)
            {
                case RotatedTo.Default:
                    {
                        return GetStandardFigurePlacementByName(Figures.I, mainX, mainY);
                    }
                case RotatedTo.ToLeft:
                    {
                        cubes.Add(new Cube(mainX - 1, mainY));
                        cubes.Add(new Cube(mainX + 1, mainY));
                        cubes.Add(new Cube(mainX + 2, mainY));
                        return cubes;
                    }
                case RotatedTo.Upside_Down:
                    {
                        cubes.Add(new Cube(mainX, mainY + 1));
                        cubes.Add(new Cube(mainX, mainY + 2));
                        cubes.Add(new Cube(mainX, mainY - 1));
                        return cubes;
                    }
                case RotatedTo.ToRight:
                    {
                        cubes.Add(new Cube(mainX + 1, mainY));
                        cubes.Add(new Cube(mainX - 1, mainY));
                        cubes.Add(new Cube(mainX - 2, mainY));
                        return cubes;
                    }
                default:
                    {
                        throw new Exception();
                    }
            }
        }
    }
    class OFigure : Figure
    {
        List<Cube> cubes;
        int mainX;
        int mainY;
        RotatedTo rotatedTo;
        public OFigure(int mainX, int mainY, RotatedTo rotatedTo = RotatedTo.Default)
        {
            this.mainX = mainX;
            this.mainY = mainY;
            this.rotatedTo = rotatedTo;
            cubes = GetStandardFigurePlacementByName(Figures.O, mainX, mainY);
            Cubes = cubes;
            Rotated = rotatedTo;
        }
        public OFigure(List<Cube> cubes, int mainX, int mainY, RotatedTo rotatedTo = RotatedTo.Default)
        {
            this.mainY = mainY;
            this.mainX = mainX;
            this.cubes = cubes;
            Cubes = cubes;
            this.rotatedTo = RotatedTo.Default;
            Rotated = rotatedTo;
        }
        public OFigure(Figure figure) :
            this(figure.Cubes, figure.MainX, figure.MainY, figure.Rotated)
        {

        }
        public void Rotate()
        {
            ;//квадрат не  вращается
        }

    }
    class Field
    {
        Figure fallingFigure;
        List<Cube> fallenFigures;
        (int, int) size;
        public Field(Figure fallingFigure, string size = "10x20")
        {
            this.fallingFigure = fallingFigure;
            int sizeInX = int.Parse(size.Split("x").First());
            int sizeInY = int.Parse(size.Split("x").Last());
            this.size = (sizeInX, sizeInY);
            fallenFigures = [];
        }
        public bool IsFigureNearTheLeftBorder(List<Cube> figure)
        {
            foreach (var cube in figure)
            {
                if (cube.X == 0)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsFigureNearTheRightBorder(List<Cube> figure)
        {
            foreach (var cube in figure)
            {
                if (cube.X == size.Item1 - 1)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsFigureCrashed(List<Cube> figure, Figure.MoveDirection moveDirection)
        {
            int xOffset;
            if (moveDirection == Figure.MoveDirection.ToRight)
            {
                xOffset = 1;
            }
            else
            {
                xOffset = -1;
            }
            foreach (var movingCube in figure)
            {
                foreach (var fallenCube in fallenFigures)
                {
                    if ((movingCube.X + xOffset) == fallenCube.X && movingCube.Y == fallenCube.Y)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsFigureFalled(Figure figure)
        {
            return IsFigureFalled(figure.Cubes);
        }
        public bool IsFigureFalled(List<Cube> figure)
        {
            foreach (var fallingCube in figure)
            {
                if (fallingCube.Y == size.Item2 - 1)
                    return true;
                foreach (var fallenCube in fallenFigures)
                {
                    if (fallingCube.Y + 1 == fallenCube.Y && fallingCube.X == fallenCube.X)
                        return true;
                }
            }
            return false;
        }
        public bool IsGameOver()
        {
            return fallenFigures.Exists(cube => cube.X == StartX && cube.Y == StartY);
        }
        public void ClearRows()
        {
            for (int y = size.Item2; y >= 0; y--)//Item2 - Y, Item1 - X
            {
                int cubesInRow = 0;
                for (int x = 0; x < size.Item1; x++)
                {
                    if (fallenFigures.Exists(cube => cube.X == x && cube.Y == y))
                    {

                        cubesInRow++;
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                if (cubesInRow == size.Item1)
                {
                    fallenFigures.RemoveAll(cubes => cubes.Y == y);
                    foreach (var cube in fallenFigures)
                    {
                        if (cube.Y < y)
                        {
                            cube.Y++;
                        }
                    }
                }

            }
        }
        public bool IsFigureBehindTheBorder(List<Cube> figure)
        {
            foreach (Cube cube in figure)
            {
                if (cube.X < 0 || cube.X > size.Item1 - 1)
                    return true;
            }
            return false;
        }
        public int StartX => size.Item1 / 2;
        public int StartY => 0;
        public List<Cube> FallenFigures
        {
            get { return fallenFigures; }
            set
            {
                fallenFigures = value;
                if (IsGameOver())
                {
                    Console.Clear();
                    Console.WriteLine("Вы проиграли... Чтобы попробовать снова, перезапустите программу");
                    Environment.Exit(993);
                }
                ClearRows();
            }
        }
        public Figure FallingFigure
        {
            get { return fallingFigure; }
            set { fallingFigure = value; }
        }
        public (int, int) Size => size;

    }
    class Renderer(Field field, string symbolForEmpty = "░░", string symbolForNonEmpty = "██")
    {
        Field field = field;
        int sizeX = field.Size.Item1;
        int sizeY = field.Size.Item2;
        string symbolForEmpty = symbolForEmpty;
        string symbolForNonEmpty = symbolForNonEmpty;
        string[,] stringField = new string[field.Size.Item1, field.Size.Item2];
        public void Draw()
        {
            var fallenFigures = field.FallenFigures;
            var fallingFigure = field.FallingFigure;
            StringBuilder nextFrame = new(string.Empty);
            var figures = fallenFigures.Concat(fallingFigure.Cubes).ToList();
            Console.SetCursorPosition(0, 0);
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    if (figures.Find(cube => cube.X == x && cube.Y == y) != null)
                    {
                        stringField[x, y] = symbolForEmpty;
                    }
                    else
                    {
                        stringField[x, y] = symbolForNonEmpty;
                    }
                    nextFrame.Append(stringField[x, y]);
                }
                nextFrame.AppendLine();
            }
            Console.Write(nextFrame.ToString());
            field.FallingFigure = fallingFigure;
        }
        public void RenderNextFrame()
        {
            if (Console.KeyAvailable)
            {
                field.FallingFigure = ProcessKeyPressing(Console.ReadKey(true), field.FallingFigure);
                Draw();
            }
        }
        public void TimeToFall(object? stateInfo)
        {
            field.FallingFigure.Fall(field);
            Draw();
        }
        public void PlaySong()
        {
            Beep(1320, 500); Beep(990, 250); Beep(1056, 250); Beep(1188, 250); Beep(1320, 125); Beep(1188, 125); Beep(1056, 250); Beep(990, 250); Beep(880, 500); Beep(880, 250); Beep(1056, 250); Beep(1320, 500); Beep(1188, 250); Beep(1056, 250); Beep(990, 750); Beep(1056, 250); Beep(1188, 500); Beep(1320, 500); Beep(1056, 500); Beep(880, 500); Beep(880, 500); System.Threading.Thread.Sleep(250); Beep(1188, 500); Beep(1408, 250); Beep(1760, 500); Beep(1584, 250); Beep(1408, 250); Beep(1320, 750); Beep(1056, 250); Beep(1320, 500); Beep(1188, 250); Beep(1056, 250); Beep(990, 500); Beep(990, 250); Beep(1056, 250); Beep(1188, 500); Beep(1320, 500); Beep(1056, 500); Beep(880, 500); Beep(880, 500); System.Threading.Thread.Sleep(500); PlaySong();
        }
        void Beep(int x, int y)
        {
            Console.Beep(x, y);
        }
        public Figure ProcessKeyPressing(ConsoleKeyInfo pressedKey, Figure fallingFigure)
        {

            switch (pressedKey.Key)
            {
                case ConsoleKey.Q:
                    {
                        switch (fallingFigure.FiguresName)
                        {
                            case Figures.T:
                                {
                                    var tFigure = new TFigure(fallingFigure);
                                    tFigure.Rotate(RotateDirection.Left);
                                    if (field.IsFigureBehindTheBorder(tFigure.Cubes))
                                        return fallingFigure;
                                    fallingFigure.Cubes = tFigure.Cubes;
                                    fallingFigure.Rotated = tFigure.Rotated;
                                    return fallingFigure;
                                }
                            case Figures.I:
                                {
                                    var iFigure = new IFigure(fallingFigure);
                                    iFigure.Rotate(RotateDirection.Left);
                                    if (field.IsFigureBehindTheBorder(iFigure.Cubes))
                                        return fallingFigure;
                                    fallingFigure.Cubes = iFigure.Cubes;
                                    fallingFigure.Rotated = iFigure.Rotated;
                                    return fallingFigure;
                                }
                            case Figures.O:
                                {
                                    return fallingFigure;
                                }
                            default:
                                {
                                    return fallingFigure;
                                }
                        }
                    }
                case ConsoleKey.E:
                    {
                        switch (fallingFigure.FiguresName)
                        {
                            case Figures.T:
                                {
                                    var tFigure = new TFigure(fallingFigure);
                                    tFigure.Rotate(RotateDirection.Right);
                                    if (field.IsFigureBehindTheBorder(tFigure.Cubes))
                                        return fallingFigure;
                                    fallingFigure.Cubes = tFigure.Cubes;
                                    fallingFigure.Rotated = tFigure.Rotated;
                                    return fallingFigure;
                                }
                            case Figures.I:
                                {
                                    var iFigure = new IFigure(fallingFigure);
                                    iFigure.Rotate(RotateDirection.Right);
                                    if (field.IsFigureBehindTheBorder(iFigure.Cubes))
                                        return fallingFigure;
                                    fallingFigure.Cubes = iFigure.Cubes;
                                    fallingFigure.Rotated = iFigure.Rotated;
                                    return fallingFigure;
                                }
                            case Figures.O:
                                {
                                    return fallingFigure;
                                }
                            default:
                                {
                                    return fallingFigure;
                                }
                        }
                    }
                case ConsoleKey.S:
                    {
                        fallingFigure.Fall(field);
                        return fallingFigure;
                    }
                case ConsoleKey.A:
                    {
                        fallingFigure.MoveToLeft(field);
                        return fallingFigure;
                    }
                case ConsoleKey.D:
                    {
                        fallingFigure.MoveToRight(field);
                        return fallingFigure;
                    }
                case ConsoleKey.R:
                    {
                        Console.Clear();
                        return fallingFigure;
                    }
                case ConsoleKey.Enter:
                    {
                        Console.Clear();
                        Console.WriteLine("Игра окончена, перазапустите программу,чтобы начать заново");
                        Environment.Exit(0);
                        return fallingFigure;
                    }
            }
            return fallingFigure;
        }
    }
}
