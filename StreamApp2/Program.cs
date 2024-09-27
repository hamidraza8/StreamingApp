using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Application.Helper;
using Application.Services;
using Core.Interfaces;
using Infrastructure.AutoMapper;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;
        // Add services to the container.
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<UMSDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IVedioUploadRepository, VedioUploadRepository>();
        builder.Services.AddScoped<IVedioUploadService, VedioUploadService>();
        builder.Services.AddScoped<ICommonService, UserService>();
        builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
        builder.Services.AddScoped<IHelpers, Helpers>();
        builder.Services.AddAutoMapper(typeof(MappingProfile));
        builder.Services.AddAWSService<IAmazonS3>();


        var awsOptions = builder.Configuration.GetAWSOptions();
        builder.Services.AddDefaultAWSOptions(awsOptions);
        builder.Services.AddAWSService<IAmazonS3>();
        builder.Services.AddCors(o => o.AddPolicy("corspolicy", builder =>
        {
            builder.WithOrigins("*")
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .WithExposedHeaders("*");
        }));
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 1073741824;
        });
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.Limits.MaxRequestBodySize = 1073741824; 
        });
        var app = builder.Build();
        app.Use(async (context, next) =>
        {
            // Check for a specific route
            if (context.Request.Path.StartsWithSegments("/api/UploadVedios/StoreVideo"))
            {
                context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = 1073741824; // 1GB
            }

            await next.Invoke();
        });
        // Configure the HTTP request pipeline.
        // if (app.Environment.IsDevelopment())
        //{
        app.UseSwagger();
        app.UseSwaggerUI();
        //}
        app.UseCors("corspolicy");
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        try
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<UMSDbContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        app.Run();
    }
}