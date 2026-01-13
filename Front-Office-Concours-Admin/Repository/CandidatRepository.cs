using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Front_Office_Concours_Admin.Models;

public class CandidatRepository
{
    private readonly string _connectionString;

    public CandidatRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // CREATE
    public void AddCandidat(Candidat candidat)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(@"
            INSERT INTO Candidat
            (Prenom, Nom, DateNaissance, Adresse, Telephone, Email, MotDePasse, Genre, DateCreation)
            VALUES
            (@Prenom, @Nom, @DateNaissance, @Adresse, @Telephone, @Email, @MotDePasse, @Genre, @DateCreation)
        ", connection);

        command.Parameters.AddWithValue("@Prenom", candidat.Prenom);
        command.Parameters.AddWithValue("@Nom", candidat.Nom);
        command.Parameters.AddWithValue("@DateNaissance", candidat.DateNaissance.ToDateTime(TimeOnly.MinValue));
        command.Parameters.AddWithValue("@Adresse", candidat.Adresse);
        command.Parameters.AddWithValue("@Telephone", candidat.Telephone);
        command.Parameters.AddWithValue("@Email", candidat.Email);
        command.Parameters.AddWithValue("@MotDePasse", candidat.MotDePasse);
        command.Parameters.AddWithValue("@Genre", candidat.Genre);
        command.Parameters.AddWithValue("@DateCreation", DateTime.Now);

        connection.Open();
        command.ExecuteNonQuery();
    }

    // READ BY ID
    public Candidat GetById(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            "SELECT * FROM Candidat WHERE Id = @Id AND DateSuppression IS NULL",
            connection);

        command.Parameters.AddWithValue("@Id", id);

        connection.Open();
        using var reader = command.ExecuteReader();

        return reader.Read() ? Map(reader) : null;
    }

    // READ ALL
    public List<Candidat> GetAll()
    {
        var list = new List<Candidat>();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            "SELECT * FROM Candidat WHERE DateSuppression IS NULL",
            connection);

        connection.Open();
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            list.Add(Map(reader));
        }

        return list;
    }

    // UPDATE
    public void updateCandidat(Candidat candidat)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(@"
            UPDATE Candidat SET
                Prenom = @Prenom,
                Nom = @Nom,
                Adresse = @Adresse,
                Telephone = @Telephone,
                Email = @Email,
                Genre = @Genre,
                DateModification = @DateModification
            WHERE Id = @Id
        ", connection);

        command.Parameters.AddWithValue("@Id", candidat.Id);
        command.Parameters.AddWithValue("@Prenom", candidat.Prenom);
        command.Parameters.AddWithValue("@Nom", candidat.Nom);
        command.Parameters.AddWithValue("@Adresse", candidat.Adresse);
        command.Parameters.AddWithValue("@Telephone", candidat.Telephone);
        command.Parameters.AddWithValue("@Email", candidat.Email);
        command.Parameters.AddWithValue("@Genre", candidat.Genre);
        command.Parameters.AddWithValue("@DateModification", DateTime.Now);

        connection.Open();
        command.ExecuteNonQuery();
    }

    // SOFT DELETE
    public void deleteCandidat(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(
            "UPDATE Candidat SET DateSuppression = @DateSuppression WHERE Id = @Id",
            connection);

        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@DateSuppression", DateTime.Now);

        connection.Open();
        command.ExecuteNonQuery();
    }

    // Mapper SqlDataReader → Candidat
    private Candidat Map(SqlDataReader reader)
    {
        return new Candidat
        {
            Id = (int)reader["Id"],
            Prenom = reader["Prenom"].ToString(),
            Nom = reader["Nom"].ToString(),
            DateNaissance = DateOnly.FromDateTime((DateTime)reader["DateNaissance"]),
            Adresse = reader["Adresse"].ToString(),
            Telephone = reader["Telephone"].ToString(),
            Email = reader["Email"].ToString(),
            MotDePasse = reader["MotDePasse"].ToString(),
            Genre = reader["Genre"].ToString(),
            DateCreation = (DateTime)reader["DateCreation"],
            DateModification = reader["DateModification"] as DateTime?,
            DateSuppression = reader["DateSuppression"] as DateTime?
        };
    }
    
    public bool login(string email, string motDePasse)
    {
        using SqlConnection cn = new SqlConnection(_connectionString);
        cn.Open();

        string query = @"SELECT motDePasse FROM Candidat WHERE Email = @Email";
        using SqlCommand cmd = new SqlCommand(query, cn);
        cmd.Parameters.AddWithValue("@Email", email);

        var result = cmd.ExecuteScalar();
        if (result != null)
        {
            string motDePasseStocke = result.ToString();

            // Comparer le mot de passe (simple, peut ajouter hash)
            return motDePasse == motDePasseStocke;
        }

        return false;
    }
    public Candidat GetCandidatByEmail(string email)
    {
        using SqlConnection cn = new SqlConnection(_connectionString);
        cn.Open();

        string query = @"SELECT * FROM Candidat WHERE Email = @Email";
        using SqlCommand cmd = new SqlCommand(query, cn);
        cmd.Parameters.AddWithValue("@Email", email);

        using SqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Candidat
            {
                Id = (int)reader["Id"],
                Nom = reader["Nom"].ToString(),
                Prenom = reader["Prenom"].ToString(),
                Email = reader["Email"].ToString(),
                Telephone = reader["Telephone"].ToString(),
                Adresse = reader["Adresse"].ToString(),
                Genre = reader["Genre"].ToString(),
                MotDePasse = reader["motDePasse"].ToString()
            };
        }

        return null;
    }
}
