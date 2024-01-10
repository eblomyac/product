using System.IO;

namespace ProductLib
{
    public static class Constants
    {
        public static class Work
        {
            public const string ArticleWorkPrefix = "Артикул";
            public const string OrderWorkPrefix = "Заказ №";

            public static class Statuses
            {
                public const string Hidden = "Скрытый";
                public const string Income = "Вхоядщий";
                public const string Waiting = "Ожидание";
                public const string Running = "Выполнение";
                public const string Ended = "Завершено";
            }
        }

        public static class Role
        {
            public static string ShortNameSplitter=",";
        }

        public static class TableMapper
        {
            public static string TableReadErrorLine =
                "Ошибка в строке: {0}, не удалось задать значение {1}, ошибка: {2}";
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