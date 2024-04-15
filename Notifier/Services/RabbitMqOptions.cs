namespace Notifier.Services
{
    public class RabbitMqOptions : IRabbitMqOptions
    {
        public string? Host { get; set; }

        public string? Admin_direct { get; set; }
        public string? Homework_direct { get; set; }
        public List<string> Admin_RoutingKeys { get; set; }

        public List<string> Homework_RoutingKeys { get; set; }
    }
}
