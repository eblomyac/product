using System.IO;

namespace ProtoLib
{
    public static class Constants
    {
        public static class Work
        {
            public static class Statuses
            {
                public const string Hidden = "Прогноз";
                public const string Income = "Получение";
                public const string Waiting = "Ожидание";
                public const string Running = "Выполнение";
                public const string Ended = "Завершено";
                public const string Sended = "Передача";
            }

            public static class EndPosts
            {
                public const string JustEnd = "[завершить]";
                public const string TotalEnd = "[артикул сдан]";
            }
        }

      
        
        public static class Database
        {
            private static string connectionStringFile ="database.connection-string";

            public static string CrpConnectionString =
                "Server=sdata3.kck2.ksk.ru;Database=crp;User=sa;Password=-c2h5oh-";
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