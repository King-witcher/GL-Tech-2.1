using System;
using System.Data.SQLite;
using System.Collections.Generic;
using System.IO;

using Engine.Data;
using Engine.Imaging;
using Engine.Scripting;
using Engine.Serialization;

namespace Engine.World
{
    public unsafe partial class Scene : IDisposable
    {
        internal unsafe void Save(string path)
        {
            (int index, Image image)[] ImageIndex = new (int, Image)[15];

            SQLiteConnection.CreateFile(path);

            SQLiteConnection connection = new SQLiteConnection($"Data Source={path};Version=3;");
            connection.Open();

            SQLiteCommand createScene = connection.CreateCommand();
            createScene.CommandText =
                "create table colliders (" +
                "   id integer not null primary key autoincrement," +
                "   name text(63)," +
                "   start_x float," +
                "   start_y float," +
                "   direction_x float," +
                "   direction_y float);";
            createScene.ExecuteNonQuery();

            foreach (var collider in colliders)
            {
                runSql($"insert into colliders (name, start_x, start_y, direction_x, direction_y) values (" +
                    $"\"{sqliFilter(collider.Name)}\", " +
                    $"{stringify(collider.unmanaged->start.x)}," +
                    $"{stringify(collider.unmanaged->start.y)}," +
                    $"{stringify(collider.unmanaged->direction.x)}," +
                    $"{stringify(collider.unmanaged->direction.y)});"
                );

                Console.WriteLine($"Saving collider {collider}.");
            }

            static string sqliFilter(string text)
            {
                return text.Replace("\"", "\"\"");
            }

            static string stringify(float number)
            {
                System.Globalization.NumberFormatInfo nfi = new();
                nfi.NumberDecimalSeparator = ".";
                return number.ToString(nfi);
            }

            void runSql(string commandString)
            {
                SQLiteCommand command = connection.CreateCommand();
                command.CommandText = commandString;
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }
}