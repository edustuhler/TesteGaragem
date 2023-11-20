using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Adicionar código para sincronizar o banco
//builder.Services.<DbContext>(options =>
//{
//    options.UseNpgsql(connectionString, options =>
//    {
//        options.MigrationsAssembly(typeof(DbContext).Assembly.FullName);
//    });
//});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

//trecho para inicialização dos dados recebidos através dos jsons
//using (var scope = app.Services.CreateScope())
//{

//    var concreteContext = scope.ServiceProvider.GetService<DbContext>();

//    if (concreteContext != null)
//    {
//        try
//        {
//            Task.WaitAll(DatabaseInitializer.Initialize(concreteContext));
//        }
//        catch (Exception ex)
//        {
//            Log.Error(ex, "Erro ao inicializar os dados.");
//        }
//    }
//}


app.MapControllers();

app.Run();
