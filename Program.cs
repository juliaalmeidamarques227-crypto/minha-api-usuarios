// aqui eu tô puxando a classe Usuario (que eu criei)
using MinhaApi.Models;
using System.Linq;

// isso aqui é tipo iniciar a API
var builder = WebApplication.CreateBuilder(args);

// ativa a parte de documentação (tipo um "painel" da API)
builder.Services.AddOpenApi();

// aqui a API é montada de fato
var app = builder.Build();

// essa lista aqui é tipo um "banco fake"
// tudo que eu criar vai ficar aqui (mas se desligar a API, some tudo)
var usuarios = new List<Usuario>();

// isso aqui só ativa a documentação quando estou desenvolvendo
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// força usar https (mais seguro, mas nem precisa esquentar muito com isso agora)
app.UseHttpsRedirection();


// =====================================
// isso aqui já vem pronto no projeto
// é só um exemplo de clima aleatório
// =====================================
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    return forecast;
});


// =====================================
// AGORA SIM: NOSSA API DE USUÁRIOS
// =====================================


// GET → ver todos os usuários
app.MapGet("/usuarios", () =>
{
    return usuarios; // só devolve a lista
});


// POST → criar usuário
app.MapPost("/usuarios", (Usuario usuario) =>
{
    // cria um id automático (tipo 1, 2, 3...)
    usuario.Id = usuarios.Count + 1;

    // adiciona na lista
    usuarios.Add(usuario);

    // retorna falando que criou
    return Results.Created($"/usuarios/{usuario.Id}", usuario);
});


// PUT → atualizar usuário
app.MapPut("/usuarios/{id}", (int id, Usuario usuarioAtualizado) =>
{
    // procura o usuário pelo id
    var usuario = usuarios.FirstOrDefault(u => u.Id == id);

    // se não achar, dá erro 404
    if (usuario == null)
    {
        return Results.NotFound();
    }

    // atualiza o nome
    usuario.Nome = usuarioAtualizado.Nome;

    return Results.Ok(usuario);
});


// DELETE → apagar usuário
app.MapDelete("/usuarios/{id}", (int id) =>
{
    var usuario = usuarios.FirstOrDefault(u => u.Id == id);

    if (usuario == null)
    {
        return Results.NotFound();
    }

    // remove da lista
    usuarios.Remove(usuario);

    return Results.Ok();
});


// aqui a API começa a rodar de verdade
app.Run();


// isso aqui é só do exemplo de clima (nao focar muito)
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Resumindo: aqui eu criei uma API simples de usuários.
// Usei uma lista em memória como "banco de dados" e fiz as rotas
// pra criar (POST), ver (GET), atualizar (PUT) e apagar (DELETE) usuários.
// Tudo funcionando localmente e testado com requisições HTTP.