using System.Data;
using Microsoft.Data.SqlClient;

namespace HospitalManagementSystem.Data;

public static class DbManager
{
    // Ndrysho emrin e serverit sipas instalimit tuaj.
    private const string ConnectionString = "Server=.;Database=HospitalManagementDB;Trusted_Connection=True;TrustServerCertificate=True;";

    public static DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
    {
        using SqlConnection connection = new(ConnectionString);
        using SqlCommand command = new(query, connection);

        if (parameters.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }

        using SqlDataAdapter adapter = new(command);
        DataTable table = new();
        adapter.Fill(table);
        return table;
    }

    public static int ExecuteNonQuery(string query, params SqlParameter[] parameters)
    {
        using SqlConnection connection = new(ConnectionString);
        using SqlCommand command = new(query, connection);

        if (parameters.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }

        connection.Open();
        return command.ExecuteNonQuery();
    }

    public static object? ExecuteScalar(string query, params SqlParameter[] parameters)
    {
        using SqlConnection connection = new(ConnectionString);
        using SqlCommand command = new(query, connection);

        if (parameters.Length > 0)
        {
            command.Parameters.AddRange(parameters);
        }

        connection.Open();
        return command.ExecuteScalar();
    }
}
