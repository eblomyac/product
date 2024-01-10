using System.IO;

namespace ProductLibPrototype
{
    public static class Constants
    {
        public static class Work
        {
            public static class Statuses
            {
                public const string Hidden = "Скрытый";
                public const string Income = "Вхоядщий";
                public const string Waiting = "Ожидание";
                public const string Running = "Выполнение";
                public const string Ended = "Завершено";
            }
        }

      
        
        public static class Database
        {
            private static string connectionStringFile ="database.connection-string"; 
            public static string ConnectionString
            {
                get
                {
                    if (File.Exists(connectionStringFile))
                    {
                        return File.ReadAllText(connectionStringFile);
                    }

                    return "";

                }
            }
        }
    }
}