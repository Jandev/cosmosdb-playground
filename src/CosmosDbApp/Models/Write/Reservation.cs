using System;

namespace CosmosDbApp.Models.Write
{
    internal class Reservation
    {
        public string id { get; set; }
        public int ReservationId { get; set; }
        public string Name { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int State { get; set; }
        public string Type { get; set; }
    }
}