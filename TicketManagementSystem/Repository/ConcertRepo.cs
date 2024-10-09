using TicketManagementSystem.Model;

namespace TicketManagementSystem.Repository
{
    internal class ConcertRepo
    {
        public void DisplayEventDetails(EventRepo eventRepo,Concert concert)
        {
            
            Console.WriteLine($"Artist: {concert.Artist}");
            Console.WriteLine($"Type: {concert.Type}");
        }
    }
}
