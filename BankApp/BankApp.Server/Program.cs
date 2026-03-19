var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Allow the WinUI client to connect
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

// Global exception for all endpoints
app.UseExceptionHandler(a => a.Run(async context =>
{
    context.Response.StatusCode = 500;
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsJsonAsync(new { error = "Something went wrong." });
}));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.MapControllers();
app.Run();