using GrpcCustomersService;
using GrpcCustomersService.Services;
using Microsoft.EntityFrameworkCore;
using Sirbu_Iulia_Lab2.Data;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddDbContext<LibraryContext>(options =>

options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddGrpc();
var app = builder.Build();
// Configure the HTTP request pipeline.
app.MapGrpcService<GrpcCrudService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPCclient");
app.Run();