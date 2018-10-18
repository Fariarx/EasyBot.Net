using System.Reflection;

namespace EasyBot
{
    /// <summary>
    /// </summary>
    public static class Library
    {
        /// <summary>
        ///     Returns the Author of this Library
        /// </summary>
        public static string Author => "michel-pi";

        /// <summary>
        ///     Returns the Library Name
        /// </summary>
        public static string Name => "EasyBot.Net";

        /// <summary>
        ///     Returns the URL of the Github Repository
        /// </summary>
        public static string ProjectUrl => "https://github.com/michel-pi/EasyBot.Net";

        /// <summary>
        ///     Returns the <c>AssemblyVersion</c> of this Library
        /// </summary>
        public static string Version
        {
            get
            {
                try
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var assemblyName = assembly.GetName();

                    return assemblyName.Version.ToString();
                }
                catch
                {
                    return "1.0.0.0";
                }
            }
        }
    }
}
