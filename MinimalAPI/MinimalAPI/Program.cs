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

todoEndpoints
    .WithCreate(configure => configure
    .RegisterValidator<TodoCreateValidation>()
    .RegisterResponseModel<TodoCreateResponse>()
    .RegisterResponseModel<TodoCreateResponse>())
    ;

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

            try
            {
                dbContext.SaveChanges();
                return Results.Ok("Created!");
            }
            catch (Exception ex)
            {
                return Results.Problem("Something went wrong: " + ex.Message);
            }
        });
        return this;
    }

    public Endpoints<TEntity> WithUpdate(Action<CreateEndpointConfiguration<TEntity>> configure)
    {
        mapGroup.MapPut("/{id}", ([FromBody] TEntity entity,[FromRoute] int id, TodoDbContext dbContext) =>
        {
            var currentEntity = dbContext.Set<TEntity>().Find(id);
            if (currentEntity is null)
            {
                return Results.NotFound();
            }
            dbContext.Entry(currentEntity).CurrentValues.SetValues(entity);

            try
            {
                dbContext.SaveChanges();
                return Results.Ok("Updated!");
            }
            catch (Exception ex)
            {
                return Results.Problem("Something went wrong: " + ex.Message);
            }
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

#region TodoUpdate
class UpdateEndpointConfiguration<TEntity>
{
    public UpdateEndpointConfiguration<TEntity> RegisterValidator<TValidator>()
        where TValidator : class
    {
        return this;
    }

    public UpdateEndpointConfiguration<TEntity> RegisterRequestModel<TRequestModel>()
    {
        return this;
    }

    public UpdateEndpointConfiguration<TEntity> RegisterResponseModel<TResponseModel>()
    {
        return this;
    }
}
class TodoUpdateValidation : AbstractValidator<Todo>
{
    public TodoUpdateValidation()
    {
        RuleFor(p => p.Title)
            .NotEmpty().WithMessage("Title cannot be empty!")
            .NotNull().WithMessage("Please include title!");
    }
}

class TodoUpdateRequest
{
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}

class TodoUpdateResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
}
#endregion