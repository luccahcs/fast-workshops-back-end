using Microsoft.AspNetCore.Mvc;
using FastWorkshops.Api.Data;
using FastWorkshops.Api.Models;
using MySql.Data.MySqlClient;

namespace FastWorkshops.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkshopsController : ControllerBase
{
    private readonly Database _db;

    public WorkshopsController(Database db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var lista = new List<Workshop>();
        using var conn = _db.GetConnection();
        conn.Open();

        var cmd = new MySqlCommand("SELECT Id, Nome, DataRealizacao, Descricao FROM Workshops", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            lista.Add(new Workshop
            {
                Id = reader.GetInt32("Id"),
                Nome = reader.GetString("Nome"),
                DataRealizacao = reader.GetDateTime("DataRealizacao"),
                Descricao = reader.GetString("Descricao")
            });
        }

        return Ok(lista);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        var cmd = new MySqlCommand("SELECT Id, Nome, DataRealizacao, Descricao FROM Workshops WHERE Id=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return NotFound();

        var ws = new Workshop
        {
            Id = reader.GetInt32("Id"),
            Nome = reader.GetString("Nome"),
            DataRealizacao = reader.GetDateTime("DataRealizacao"),
            Descricao = reader.GetString("Descricao")
        };

        return Ok(ws);
    }

    [HttpPost]
    public IActionResult Create(Workshop novo)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        var cmd = new MySqlCommand("INSERT INTO Workshops (Nome, DataRealizacao, Descricao) VALUES (@nome, @data, @descricao); SELECT LAST_INSERT_ID();", conn);
        cmd.Parameters.AddWithValue("@nome", novo.Nome);
        cmd.Parameters.AddWithValue("@data", novo.DataRealizacao);
        cmd.Parameters.AddWithValue("@descricao", novo.Descricao);

        novo.Id = Convert.ToInt32(cmd.ExecuteScalar());
        return CreatedAtAction(nameof(GetById), new { id = novo.Id }, novo);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Workshop atualizado)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        var cmd = new MySqlCommand("UPDATE Workshops SET Nome=@nome, DataRealizacao=@data, Descricao=@descricao WHERE Id=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@nome", atualizado.Nome);
        cmd.Parameters.AddWithValue("@data", atualizado.DataRealizacao);
        cmd.Parameters.AddWithValue("@descricao", atualizado.Descricao);

        var rows = cmd.ExecuteNonQuery();
        if (rows == 0) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        var cmd = new MySqlCommand("DELETE FROM Workshops WHERE Id=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        var rows = cmd.ExecuteNonQuery();
        if (rows == 0) return NotFound();

        return NoContent();
    }
}
