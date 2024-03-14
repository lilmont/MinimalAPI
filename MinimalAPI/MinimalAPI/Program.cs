using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDbContext>(config =>
{
    config.UseInMemoryDatabase("MinimalApiDb");
});

var app = builder.Build();

var todoEndpoints = new Endpoints<Todo>(app.MapGroup("/todoItems"));

todoEndpoints.WithCreate(configure => configure
.RegisterValidator<TodoCreateValidation>()
.RegisterResponseModel<TodoCreateResponse>()
.RegisterResponseModel<TodoCreateResponse>()
);

app.Run();

#region Database
class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<Todo> Todos { get; set; }
}

class Todo
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public bool IsCompleted { get; set; }
}
#endregion

class Endpoints<TEntity>(RouteGroupBuilder mapGroup)
    where TEntity : class
{
    public Endpoints<TEntity> WithCreate(Action<CreateEndpointConfiguration<TEntity>> configure)
    {
        mapGroup.MapPost("/", ([FromBody] TEntity entity, TodoDbContext dbContext) =>
        {           
            dbContext.Set<TEntity>().Add(entity);
            dbContext.SaveChanges();
            return Results.Ok("Created.");
        });
        return this;
    }
}

#region TodoCreate
class CreateEndpointConfiguration<TEntity>
{
    public CreateEndpointConfiguration<TEntity> RegisterValidator<TValidator>()
        where TValidator : class
    {
        return this;
    }

    public CreateEndpointConfiguration<TEntity> RegisterRequestModel<TRequestModel>()
    {
        return this;
    }

    public CreateEndpointConfiguration<TEntity> RegisterResponseModel<TResponseModel>()
    {
        return this;
    }
}

class TodoCreateValidation : AbstractValidator<Todo>
{
    public TodoCreateValidation()
    {
        RuleFor(p => p.Title)
            .NotEmpty().WithMessage("Title cannot be empty!")
            .NotNull().WithMessage("Please include title!");
    }
}

class TodoCreateRequest
{
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}

class TodoCreateResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}
#endregion