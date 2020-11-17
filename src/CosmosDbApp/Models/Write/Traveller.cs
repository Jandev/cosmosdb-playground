namespace CosmosDbApp.Models.Write
{
    internal class Traveller
    {
        public string id { get; set; }
        public int ReservationId { get; set; }
        public string Name { get; set; }
        public string PassportNumber { get; set; }
        public string Type { get; set; }
    }
}