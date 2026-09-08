namespace RabbitMQ.SharedClassForHeaderExchange;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal Stock { get; set; }
}