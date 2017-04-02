namespace LibSassGen
{
    /// <summary>
    ///     Generates a C# file from api.h
    ///     Runs only on Windows
    /// </summary>
    internal class Program
    {
        private static void Main(string[] args)
        {
            var codeGen = new CodeGenerator();
            // codeGen.OutputToConsole = true;
            codeGen.Run();
        }
    }
}
