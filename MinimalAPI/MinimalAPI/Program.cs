using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDbContext>(config =>
{
    config.UseInMemoryDatabase("MinimalApiDb");
});

var app = builder.Build();

app.Run();

class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options)
    {

    }
    public DbSet<Todo> Todos { get; set; }
}

class Todo
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool IsCompleted { get; set; }
}
