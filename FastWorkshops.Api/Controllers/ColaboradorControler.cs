using Microsoft.AspNetCore.Mvc;
using FastWorkshops.Api.Data;
using FastWorkshops.Api.Models;
using MySql.Data.MySqlClient;

namespace FastWorkshops.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ColaboradoresController : ControllerBase
{
    private readonly Database _db;

    public ColaboradoresController(Database db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var lista = new List<Colaborador>();
        using var conn = _db.GetConnection();
        conn.Open();

        var cmd = new MySqlCommand("SELECT Id, Nome FROM Colaboradores", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            lista.Add(new Colaborador
            {
                Id = reader.GetInt32("Id"),
                Nome = reader.GetString("Nome")
            });
        }

        return Ok(lista);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        var cmd = new MySqlCommand("SELECT Id, Nome FROM Colaboradores WHERE Id=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return NotFound();

        var colab = new Colaborador
        {
            Id = reader.GetInt32("Id"),
            Nome = reader.GetString("Nome")
        };

        return Ok(colab);
    }

    [HttpPost]
    public IActionResult Create(Colaborador novo)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        var cmd = new MySqlCommand("INSERT INTO Colaboradores (Nome) VALUES (@nome); SELECT LAST_INSERT_ID();", conn);
        cmd.Parameters.AddWithValue("@nome", novo.Nome);

        novo.Id = Convert.ToInt32(cmd.ExecuteScalar());
        return CreatedAtAction(nameof(GetById), new { id = novo.Id }, novo);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Colaborador atualizado)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        var cmd = new MySqlCommand("UPDATE Colaboradores SET Nome=@nome WHERE Id=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@nome", atualizado.Nome);

        var rows = cmd.ExecuteNonQuery();
        if (rows == 0) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        var cmd = new MySqlCommand("DELETE FROM Colaboradores WHERE Id=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        var rows = cmd.ExecuteNonQuery();
        if (rows == 0) return NotFound();

        return NoContent();
    }
}
