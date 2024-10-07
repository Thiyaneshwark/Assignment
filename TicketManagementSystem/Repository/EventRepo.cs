using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TicketManagementSystem.Constants;
using TicketManagementSystem.Model;
using TicketManagementSystem.Service;
using TicketManagementSystem.Util;

namespace TicketManagementSystem.Repository
{

    internal class EventRepo : IEventServiceProvider
    {
        List<Event> events;
        Booking bookings = new Booking();
        SqlConnection sqlConnection = null;
        SqlCommand cmd = null;

        public EventRepo()
        {
            sqlConnection = new SqlConnection(DbConnUtil.GetConnectionString());
            cmd = new SqlCommand();
            cmd.Connection = sqlConnection;
        }

        public Event CreateEvent(string eventName, DateTime date, TimeSpan time, int totalSeats, decimal ticketPrice, EventTypes eventType, Venue venue)
        {
            Event newEvent = null;

            try
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }

                int venueId = GetOrInsertVenueId(venue); // Ensure connection stays open
                cmd.CommandText = "INSERT INTO Event (event_name, event_date, event_time, venue_id, total_seats, available_seats, ticket_price, event_type) VALUES (@EventName, @EventDate, @EventTime, @VenueId, @TotalSeats, @AvailableSeats, @TicketPrice, @EventType)";
                cmd.Parameters.Clear(); // Clear previous parameters
                cmd.Parameters.AddWithValue("@EventName", eventName);
                cmd.Parameters.AddWithValue("@EventDate", date);
                cmd.Parameters.AddWithValue("@EventTime", time);
                cmd.Parameters.AddWithValue("@VenueId", venueId);
                cmd.Parameters.AddWithValue("@TotalSeats", totalSeats);
                cmd.Parameters.AddWithValue("@AvailableSeats", totalSeats);
                cmd.Parameters.AddWithValue("@TicketPrice", ticketPrice);
                cmd.Parameters.AddWithValue("@EventType", eventType.ToString());
                cmd.ExecuteNonQuery();

                // Event creation based on type
                switch (eventType)
                {
                    case EventTypes.Movie:
                        newEvent = new Movie(eventName, date, time, venue, totalSeats, totalSeats, ticketPrice, eventType, MovieGenre.Action, "ActorName", "ActressName");
                        break;
                    case EventTypes.Sports:
                        newEvent = new Sports(eventName, date, time, venue, totalSeats, totalSeats, ticketPrice, eventType, "sportsName", "teamName");
                        break;
                    case EventTypes.Concert:
                        newEvent = new Concert(eventName, date, time, venue, totalSeats, totalSeats, ticketPrice, eventType, "artist", ConcertType.Theatrical);
                        break;
                    default:
                        throw new ArgumentException($"Invalid event type: {eventType}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while creating event: {ex.Message}");
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
            return newEvent;
        }

        public int GetOrInsertVenueId(Venue venue)
        {
            int venueId = 0;
            try
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }

                cmd.CommandText = "SELECT venue_id FROM Venue WHERE venue_name = @VenueName AND address = @VenueAddress";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@VenueName", venue.VenueName);
                cmd.Parameters.AddWithValue("@VenueAddress", venue.VenueAddress);
                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    venueId = reader.GetInt32(0);
                }
                else
                {
                    reader.Close(); // Close the reader before inserting
                    cmd.CommandText = "INSERT INTO Venue (venue_name, address) OUTPUT INSERTED.venue_id VALUES (@VenueName, @VenueAddress)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@VenueName", venue.VenueName);
                    cmd.Parameters.AddWithValue("@VenueAddress", venue.VenueAddress);
                    venueId = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while getting or inserting venue: {ex.Message}");
            }
            return venueId;
        }

        public List<Event> GetEvents()
        {
            return events ?? new List<Event>();
        }
        public List<Event> GetAllEvents()
        {
            List<Event> events = new List<Event>();

            try
            {
                // Ensure connection is open only if it's not already open
                if (sqlConnection.State == ConnectionState.Closed)
                {
                    sqlConnection.Open();
                }

                cmd.CommandText = "SELECT * FROM Event";
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Event @event = null;
                    string eventTypeString = reader["event_type"].ToString();
                    EventTypes eventType = (EventTypes)Enum.Parse(typeof(EventTypes), eventTypeString);

                    switch (eventType)
                    {
                        case EventTypes.Movie:
                            @event = new Movie();
                            break;
                        case EventTypes.Sports:
                            @event = new Sports();
                            break;
                        case EventTypes.Concert:
                            @event = new Concert();
                            break;
                        default:
                            throw new ArgumentException($"Invalid event type: {eventTypeString}");
                    }

                    @event.EventName = reader["event_name"].ToString();
                    @event.EventDate = (DateTime)reader["event_date"];
                    @event.EventTime = (TimeSpan)reader["event_time"];
                    @event.Venue = new Venue { VenueId = (int)reader["venue_id"] };
                    @event.TotalSeats = (int)reader["total_seats"];
                    @event.AvailableSeats = (int)reader["available_seats"];
                    @event.TicketPrice = (decimal)reader["ticket_price"];
                    @event.EventTypes = eventType;
                    events.Add(@event);
                }

                reader.Close();  // Make sure to close the reader as well
            }
            finally
            {
                // Ensure the connection is always closed
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }

            return events;
        }

        public int GetAvailableNoOfTickets(string eventName)
        {
            int availableTickets = 0;
            try
            {
                sqlConnection.Open();
                cmd.CommandText = "SELECT available_seats FROM Event WHERE event_name = @EventName";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EventName", eventName);
                object result = cmd.ExecuteScalar();
                sqlConnection.Close();
                if (result != null)
                {
                    availableTickets = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while checking available tickets: {ex.Message}");
            }
            return availableTickets;
        }


        public decimal CalculateBookingCost(int numTickets, decimal ticketPrice)
        {
            return numTickets * ticketPrice;
        }

        public void BookTickets(string eventName, int numTickets, Customer[] arrayOfCustomer)
        {
            try
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();
                }

                // Retrieve event details
                int eventId = 0;
                decimal ticketPrice = 0;
                int availableSeats = 0;
                cmd.CommandText = "SELECT event_id, ticket_price, available_seats FROM Event WHERE event_name = @EventName";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EventName", eventName);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    eventId = reader.GetInt32(0);
                    ticketPrice = reader.GetDecimal(1);
                    availableSeats = reader.GetInt32(2);
                }
                reader.Close();

                if (availableSeats < numTickets)
                {
                    Console.WriteLine($"Sorry, only {availableSeats} tickets are available for '{eventName}'.");
                    return;
                }

                // Calculate total cost and book tickets
                decimal totalCost = CalculateBookingCost(numTickets, ticketPrice);
                cmd.CommandText = "INSERT INTO Booking (event_id, num_tickets, total_cost, booking_date) OUTPUT INSERTED.booking_id VALUES (@EventId, @NumTickets, @TotalCost, @BookingDate)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EventId", eventId);
                cmd.Parameters.AddWithValue("@NumTickets", numTickets);
                cmd.Parameters.AddWithValue("@TotalCost", totalCost);
                cmd.Parameters.AddWithValue("@BookingDate", DateTime.Now);
                int bookingId = (int)cmd.ExecuteScalar();

                // Insert customers and update seats
                foreach (var customer in arrayOfCustomer)
                {
                    cmd.CommandText = "INSERT INTO Customer VALUES (@CustomerName, @Email, @PhoneNumber, @BookingId)";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                    cmd.Parameters.AddWithValue("@Email", customer.CustomerMailId);
                    cmd.Parameters.AddWithValue("@PhoneNumber", customer.CustomerPhoneNumber);
                    cmd.Parameters.AddWithValue("@BookingId", bookingId);
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = "UPDATE Event SET available_seats = available_seats - @NumTickets WHERE event_id = @EventId";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@NumTickets", numTickets);
                cmd.Parameters.AddWithValue("@EventId", eventId);
                cmd.ExecuteNonQuery();

                Console.WriteLine($"{numTickets} tickets successfully booked for '{eventName}'. Total cost: {totalCost}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while booking tickets: {ex.Message}");
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        public void CancelBooking(int bookingId)
        {
            try
            {
                sqlConnection.Open();

                // Retrieve the eventId and number of tickets from the Booking table
                cmd.CommandText = "SELECT event_id, num_tickets FROM Booking WHERE booking_id = @BookingId";
                cmd.Parameters.AddWithValue("@BookingId", bookingId);
                var reader = cmd.ExecuteReader();

                int eventId = 0;
                int numTickets = 0;

                if (reader.Read())
                {
                    eventId = reader.GetInt32(0);
                    numTickets = reader.GetInt32(1);
                }
                reader.Close(); // Ensure the reader is closed before proceeding

                // Delete the booking
                cmd.CommandText = "DELETE FROM Booking WHERE booking_id = @BookingId";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@BookingId", bookingId);
                cmd.ExecuteNonQuery();

                // Update available seats in the Event table
                cmd.CommandText = "UPDATE Event SET available_seats = available_seats + @NumTickets WHERE event_id = @EventId";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@NumTickets", numTickets);
                cmd.Parameters.AddWithValue("@EventId", eventId);
                cmd.ExecuteNonQuery();

                Console.WriteLine($"Booking {bookingId} successfully canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while canceling booking: {ex.Message}");
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }


        public Booking GetBookingDetails(int bookingId)
        {
            Booking booking = null;

            try
            {
                cmd.Parameters.Clear();
                sqlConnection.Open();
                cmd.CommandText = "SELECT * FROM Booking WHERE booking_id = @BookingId";
                cmd.Parameters.AddWithValue("@BookingId", bookingId);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int numTickets = reader.GetInt32(3);
                    int totalPrice = reader.GetInt32(4);
                    DateTime bookingDate = reader.GetDateTime(5);
                    int eventId = reader.GetInt32(2);
                    Customer[] customers = new Customer[] { };
                    Event _event = GetEventDetailsById(eventId);
                    booking = new Booking(customers, _event, numTickets, totalPrice, bookingDate, eventId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while getting booking details: {ex.Message}");
            }
            sqlConnection.Close();
            return booking;
        }

        private Event GetEventDetailsById(int eventId)
        {
            Event _event = null;

            try
            {
                using (sqlConnection)
                {
                    sqlConnection.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Event WHERE event_id = @EventId", sqlConnection))
                    {
                        cmd.Parameters.AddWithValue("@EventId", eventId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string eventName = reader.GetString(1);
                                DateTime eventDate = reader.GetDateTime(2);
                                TimeSpan eventTime = reader.GetTimeSpan(3);
                                int totalSeats = reader.GetInt32(4);
                                int availableSeats = reader.GetInt32(5);
                                decimal ticketPrice = reader.GetDecimal(6);
                                EventTypes eventType = (EventTypes)Enum.Parse(typeof(EventTypes), reader.GetString(7));
                                int venueId = reader.GetInt32(8);

                                
                                Venue venue = GetVenueDetailsById(venueId); 

                                
                                switch (eventType)
                                {
                                    case EventTypes.Movie:
                                        _event = new Movie(eventName, eventDate, eventTime, venue, totalSeats, availableSeats, ticketPrice, eventType, MovieGenre.Action, "ActorName", "ActressName");
                                        break;
                                    case EventTypes.Sports:
                                        _event = new Sports(eventName, eventDate, eventTime, venue, totalSeats, availableSeats, ticketPrice, eventType, "sportsName", "teamName");
                                        break;
                                    case EventTypes.Concert:
                                        _event = new Concert(eventName, eventDate, eventTime, venue, totalSeats, availableSeats, ticketPrice, eventType, "artist", ConcertType.Theatrical);
                                        break;
                                    default:
                                        throw new ArgumentException($"Invalid event type: {eventType}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while getting event details by ID: {ex.Message}");
            }

            return _event;
        }

        
        private Venue GetVenueDetailsById(int venueId)
        {
            Venue venue = null;
            
            return venue;
        }
    }
}
