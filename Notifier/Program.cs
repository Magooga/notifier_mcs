using Notifier.Services;

namespace Notifier
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.Configure<RabbitMqOptions>(
                builder.Configuration.GetSection("RabbitMq"));

            // Listen RabbitMq queue
            //builder.Services.AddHostedService<RabbitMqListener>(x => new RabbitMqListener(rabbitMqOptions));  // https://www.rabbitmq.com/tutorials/tutorial-six-dotnet.html
            builder.Services.AddHostedService<RabbitMqListener>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // DI
            builder.Services.AddSingleton<IEmailService, EmailService>();

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

            app.Run("https://localhost:7211");
        }
    }
}