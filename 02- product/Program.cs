var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer(); // Services for Swagger
builder.Services.AddSwaggerGen();
var app = builder.Build();

if(app.Environment.IsDevelopment()){ // development --Middleware
    app.UseSwagger(); // Use--> hole middleware
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

// Rest API => Get, Post, Put, Delete
app.MapGet("/", ()=> "Api is working fine");
app.MapPost("/hello", ()=>
{
    var response = new { message = "This is a json object",
    success = true};
    return Results.Ok(response);// Response pass kortesi// 200
});

app.Run();