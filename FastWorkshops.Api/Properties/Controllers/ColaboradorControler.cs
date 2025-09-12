using Microsoft.AspNetCore.Mvc;
using FastWorkshops.Api.Models;

namespace FastWorkshops.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ColaboradorController : ControllerBase
{
    private static List<Colaborador> colaboradores = new()
    {
        new Colaborador { Id = 1, Nome = "Lucas" },
        new Colaborador { Id = 2, Nome = "Maria" }
    };

    [HttpGet]
    public IActionResult GetAll() => Ok(colaboradores);
    
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var colab = colaboradores.FirstOrDefault(c => c.Id == id);
        return colab == null ? NotFound() : Ok(colab);
    }
    
    [HttpPost]
    public IActionResult Create(Colaborador novo)
    {
        novo.Id = colaboradores.Max(c => c.Id) + 1; // gera Id novo
        colaboradores.Add(novo);
        return CreatedAtAction(nameof(GetById), new { id = novo.Id }, novo);
    }
    
    [HttpPut("{id}")]
    public IActionResult Update(int id, Colaborador atualizado)
    {
        var colab = colaboradores.FirstOrDefault(c => c.Id == id);
        if (colab == null) return NotFound();

        colab.Nome = atualizado.Nome;
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var colab = colaboradores.FirstOrDefault(c => c.Id == id);
        if (colab == null) return NotFound();

        colaboradores.Remove(colab);
        return NoContent();
    }
}
