/* using System;
using System.Collections.Generic;
using System.Data.SQLite;

const string connectionString = "Data Source=devices.db;";

public static class DeviceIds
{
    public static void StoreDeviceIds(List<string> deviceIds, string connectionString)
    {
        try
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                const string deleteAllSql = "DELETE FROM DeviceIds";
                using (var command = new SQLiteCommand(deleteAllSql, connection))
                {
                    command.ExecuteNonQuery();
                }

                const string insertSql = "INSERT INTO DeviceIds (DeviceId) VALUES (@deviceId)";
                using (var command = new SQLiteCommand(insertSql, connection))
                {
                    command.Parameters.AddWithValue("@deviceId", null); 

                    foreach (string deviceId in deviceIds)
                    {
                        command.Parameters["@deviceId"].Value = deviceId;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error storing device IDs: {ex.Message}");
        }
    }

    public static List<string> RetrieveDeviceIds(string connectionString)
    {
        var deviceIds = new List<string>();

        try
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                const string selectSql = "SELECT DeviceId FROM DeviceIds";
                using (var command = new SQLiteCommand(selectSql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        deviceIds.Add(reader.GetString(0));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving device IDs: {ex.Message}");
        }

        return deviceIds;
    }
} */