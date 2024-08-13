var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // N?u mu?n Swagger UI ???c hi?n th? ? ???ng d?n root "/"
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

        // N?u mu?n Swagger UI t? ??ng load khi truy c?p root "/"
        c.RoutePrefix = string.Empty; 
    });
}

// Lo?i b? HttpsRedirection ?? không t? ??ng chuy?n ??i HTTP sang HTTPS
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();