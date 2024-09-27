using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Application.Helper;
using Application.Services;
using Core.Interfaces;
using Infrastructure.AutoMapper;
using Infrastructure.Data;
using Infrastructure.Repositories;
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
        //builder.Services.AddSingleton<IAmazonS3>(sp =>
        //{
        //    var awsOptions = sp.GetRequiredService<IOptions<AWSOptions>>();
        //    return awsOptions.CreateServiceClient<IAmazonS3>();
        //});
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.Limits.MaxRequestBodySize = 5 * 1024 * 1024; // Set the limit to 5 MB
        });
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

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