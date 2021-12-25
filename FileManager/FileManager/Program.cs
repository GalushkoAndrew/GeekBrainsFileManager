using GeekBrains.Learn.FileManager.Interface;

namespace GeekBrains.Learn.FileManager.Main
{
    /// <summary>
    /// Start class
    /// </summary>
    public class Program
{
        /// <summary>
        /// Точка входа в программу
        /// </summary>
        public static void Main()
        {
            IUserInterface userInterface = new UserInterface();
            userInterface.Start();
        }
    }
}
