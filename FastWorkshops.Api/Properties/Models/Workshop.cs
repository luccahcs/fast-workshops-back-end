namespace FastWorkshops.Api.Models;

public class Workshop
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataRealizacao { get; set; }
    public string descricao { get; set; } = string.Empty;
}
