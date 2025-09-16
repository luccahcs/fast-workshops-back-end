using Microsoft.AspNetCore.Mvc;
using FastWorkshops.Api.Data;
using FastWorkshops.Api.Models;
using MySql.Data.MySqlClient;
using System.Linq;

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

        var cmd = new MySqlCommand("SELECT Id, Nome, DataRealizacao, Descricao, Participantes FROM Workshops", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            int idxPart = reader.GetOrdinal("Participantes");
            string participantesStr = reader.IsDBNull(idxPart) ? string.Empty : reader.GetString(idxPart);

            var participantesList = participantesStr
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p))
                .ToList();

            lista.Add(new Workshop
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Nome = reader.GetString(reader.GetOrdinal("Nome")),
                DataRealizacao = reader.GetDateTime(reader.GetOrdinal("DataRealizacao")),
                Descricao = reader.GetString(reader.GetOrdinal("Descricao")),
                Participantes = participantesList
            });
        }

        return Ok(lista);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        var cmd = new MySqlCommand("SELECT Id, Nome, DataRealizacao, Descricao, Participantes FROM Workshops WHERE Id=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return NotFound();

        int idxPart = reader.GetOrdinal("Participantes");
        string participantesStr = reader.IsDBNull(idxPart) ? string.Empty : reader.GetString(idxPart);

        var participantesList = participantesStr
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .Where(p => !string.IsNullOrEmpty(p))
            .ToList();

        var ws = new Workshop
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            Nome = reader.GetString(reader.GetOrdinal("Nome")),
            DataRealizacao = reader.GetDateTime(reader.GetOrdinal("DataRealizacao")),
            Descricao = reader.GetString(reader.GetOrdinal("Descricao")),
            Participantes = participantesList
        };

        return Ok(ws);
    }

    [HttpPost]
    public IActionResult Create(Workshop novo)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        using var cmd = new MySqlCommand("INSERT INTO Workshops (Nome, DataRealizacao, Descricao) VALUES (@nome, @data, @descricao);", conn);
        cmd.Parameters.AddWithValue("@nome", novo.Nome);
        cmd.Parameters.AddWithValue("@data", novo.DataRealizacao);
        cmd.Parameters.AddWithValue("@descricao", novo.Descricao);

        cmd.ExecuteNonQuery();

        using var getId = new MySqlCommand("SELECT LAST_INSERT_ID();", conn);
        novo.Id = Convert.ToInt32(getId.ExecuteScalar());

        return CreatedAtAction(nameof(GetById), new { id = novo.Id }, novo);
    }

    [HttpPost("{id}/participant-register")]
    public IActionResult RegisterParticipant(int id, Colaborador participant)
    {
        if (participant == null || string.IsNullOrWhiteSpace(participant.Nome))
            return BadRequest(new { message = "Nome do participante é obrigatório." });

        string nome = participant.Nome.Trim();

        using var conn = _db.GetConnection();
        conn.Open();

        using var tran = conn.BeginTransaction();

        try
        {
            string participantesStr;
            using (var sel = new MySqlCommand("SELECT participantes FROM workshops WHERE id = @workshopId FOR UPDATE;", conn, tran))
            {
                sel.Parameters.AddWithValue("@workshopId", id);
                object? obj = sel.ExecuteScalar();
                participantesStr = obj == null || obj == DBNull.Value ? string.Empty : obj.ToString()!;
            }

            var participantesList = participantesStr
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p))
                .ToList();

            if (participantesList.Count >= 30)
            {
                tran.Rollback();
                return BadRequest(new { message = "Limite de 30 participantes atingido para este workshop." });
            }
            if (participantesList.Any(x => string.Equals(x, nome, StringComparison.OrdinalIgnoreCase)))
            {
                tran.Rollback();
                return Conflict(new { message = "Participante já registrado neste workshop." });
            }

            // ✅ Inserir colaborador (se não existir)
            int colaboradorId;
            using (var chk = new MySqlCommand("SELECT Id FROM colaboradores WHERE nome = @nome LIMIT 1;", conn, tran))
            {
                chk.Parameters.AddWithValue("@nome", nome);
                object? idObj = chk.ExecuteScalar();
                if (idObj != null && idObj != DBNull.Value)
                {
                    colaboradorId = Convert.ToInt32(idObj);
                }
                else
                {
                    using var ins = new MySqlCommand("INSERT INTO colaboradores (nome) VALUES (@nome);", conn, tran);
                    ins.Parameters.AddWithValue("@nome", nome);
                    ins.ExecuteNonQuery();
                    colaboradorId = (int)ins.LastInsertedId;
                }
            }

            participantesList.Add(nome);
            string newParticipantesStr = string.Join(",", participantesList);

            using (var upd = new MySqlCommand("UPDATE workshops SET participantes = @new WHERE id = @workshopId;", conn, tran))
            {
                upd.Parameters.AddWithValue("@new", newParticipantesStr);
                upd.Parameters.AddWithValue("@workshopId", id);

                var rowsAffected = upd.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    tran.Rollback();
                    return NotFound();
                }
            }

            Console.WriteLine($"[LOG] Colaborador - ({nome}) registrado no workshop {id}.");

            tran.Commit();

            var payloadResponse = new
            {
                workshopId = id,
                participantes = participantesList.ToArray()
            };

            return CreatedAtAction(nameof(GetById), new { id }, payloadResponse);
        }
        catch (Exception ex)
        {
            try { tran.Rollback(); } catch { /* ignorar rollback falho */ }
            Console.Error.WriteLine(ex);
            return StatusCode(500, new { message = "Erro interno." });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Workshop atualizado)
    {
        using var conn = _db.GetConnection();
        conn.Open();

        string participantesStr = atualizado.Participantes != null
            ? string.Join(",", atualizado.Participantes)
            : string.Empty;

        var cmd = new MySqlCommand("UPDATE Workshops SET Nome=@nome, DataRealizacao=@data, Descricao=@descricao, Participantes=@participantes WHERE Id=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@nome", atualizado.Nome);
        cmd.Parameters.AddWithValue("@data", atualizado.DataRealizacao);
        cmd.Parameters.AddWithValue("@descricao", atualizado.Descricao);
        cmd.Parameters.AddWithValue("@participantes", participantesStr);

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
