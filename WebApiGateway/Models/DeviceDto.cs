namespace WebApiGateway.Models
{
    public record DeviceCreateDto
    {
        public string Name { get; set; }
        public bool? Online { get; set; }
        public int? LocationID { get; set; }
    }
}