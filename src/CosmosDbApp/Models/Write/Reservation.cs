using System;

namespace CosmosDbApp.Models.Write
{
    internal class Reservation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int State { get; set; }
    }
}