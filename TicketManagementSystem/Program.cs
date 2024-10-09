using System;
using System.Collections.Generic;
using Spectre.Console; 
using TicketManagementSystem.Constants;
using TicketManagementSystem.Model;
using TicketManagementSystem.Repository;
using TicketManagementSystem.Service;

namespace TicketManagementSystem
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            TicketBookingSystem ticketBookingSystem = new TicketBookingSystem();
            EventRepo eventRepo = new EventRepo();
            List<Booking> bookings = new List<Booking>();
            BookingRepo bookingRepo = new BookingRepo(eventRepo);
            bool FLAG = true;
            Console.ForegroundColor = ConsoleColor.Cyan;
            CenterText("Ticket Booking System");
            Console.ResetColor();
            while (FLAG)
            {
                
                Console.WriteLine("\n[1] Create Event");
                Console.WriteLine("[2] Book Tickets");
                Console.WriteLine("[3] Cancel Tickets");
                Console.WriteLine("[4] Get Available Seats");
                Console.WriteLine("[5] Display Events");
                Console.WriteLine("[6] Booking Details");
                Console.WriteLine("[7] Exit");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("Enter your choice: ");
                Console.ResetColor();

                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        CreateEvent(eventRepo);
                        AskToContinue(ref FLAG);
                        break;
                    case 2:
                        BookTickets(ticketBookingSystem, eventRepo, bookings);
                        AskToContinue(ref FLAG);
                        break;
                    case 3:
                        CancelTickets(ticketBookingSystem, eventRepo);
                        AskToContinue(ref FLAG);
                        break;
                    case 4:
                        var eve = eventRepo.GetAllEvents();

                        foreach (var ev in eve)
                        {
                            Console.WriteLine($"Event: {ev.EventName}");
                        }

                        Console.Write("Enter event name: ");
                        string eventName = Console.ReadLine();

                        int availableSeats = eventRepo.GetAvailableNoOfTickets(eventName);
                        Console.WriteLine($"Available seats for event '{eventName}': {availableSeats}");
                        AskToContinue(ref FLAG);
                        break;
                    case 5:
                        DisplayEvents(eventRepo);
                        AskToContinue(ref FLAG);
                        break;
                    case 6:
                        DisplayBookingDetails(eventRepo);
                        AskToContinue(ref FLAG);
                        break;
                    case 7:
                        FLAG = false;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid choice. Please try again.");
                        Console.ResetColor();
                        break;
                }
            }
        }

        private static void DisplayEvents(EventRepo eventRepo)
        {
            var allEvents = eventRepo.GetAllEvents();

            
            var table = new Table();
            table.AddColumn("Event Name");
            table.AddColumn("Date");
            table.AddColumn("Time");
            table.AddColumn("Ticket Price");

            if (allEvents != null && allEvents.Count > 0)
            {
                foreach (var ev in allEvents)
                {
                    
                    table.AddRow(ev.EventName, ev.EventDate.ToString("yyyy-MM-dd"), ev.EventTime.ToString(@"hh\:mm"), ev.TicketPrice.ToString("F2"));
                }
                
                AnsiConsole.Render(table);
            }
            else
            {
                Console.WriteLine("No events to display.");
            }
        }

        private static void DisplayBookingDetails(EventRepo eventRepo)
        {
            Console.Write("Enter booking ID: ");
            int bookingId = int.Parse(Console.ReadLine());
            var bookingDetails = eventRepo.GetBookingDetails(bookingId);

            
            var table = new Table();
            //table.AddColumn("Booking ID");
            table.AddColumn("Event ID");
            table.AddColumn("Number of Tickets");
            table.AddColumn("Total Cost");
            table.AddColumn("Booking Date");

            if (bookingDetails != null)
            {
                
                table.AddRow( bookingDetails.EventId.ToString(),
                             bookingDetails.NumTickets.ToString(), bookingDetails.TotalCost.ToString("F2"),
                             bookingDetails.BookingDate.ToString("yyyy-MM-dd"));

          
                AnsiConsole.Render(table);
            }
            else
            {
                Console.WriteLine($"Booking with ID {bookingId} not found.");
            }
        }

        static void AskToContinue(ref bool flag)
        {
            Console.WriteLine("Want to continue? Yes/No");
            string cnn = Console.ReadLine();
            if (cnn.ToLower() == "no")
            {
                flag = false;
            }
        }
        static void CreateEvent(IEventServiceProvider eventService)
        {
     
            Console.WriteLine("Enter event details:");
            Console.Write("Event Name: ");
            string eventName = Console.ReadLine();
            Console.Write("Event Date (YYYY-MM-DD): ");
            DateTime eventDate = DateTime.Parse(Console.ReadLine());

            Console.Write("Event Time (HH:MM): ");
            TimeSpan eventTime = TimeSpan.Parse(Console.ReadLine());

            Console.Write("Venue Id: ");
            int venueId = int.Parse(Console.ReadLine());

            Console.Write("Venue Name: ");
            string venueName = Console.ReadLine(); 

            Console.Write("Venue Address: ");
            string venueAddress = Console.ReadLine();
            Venue venue = new Venue(venueId,venueName, venueAddress);

            Console.Write("Total Seats: ");
            int totalSeats = int.Parse(Console.ReadLine());

            Console.Write("Ticket Price: ");
            decimal ticketPrice = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Select Event Type:");
            foreach (EventTypes eventType in Enum.GetValues(typeof(EventTypes)))
            {
                Console.WriteLine($"{(int)eventType}. {eventType}");
            }
            EventTypes selectedEventType = (EventTypes)int.Parse(Console.ReadLine());
            eventService.CreateEvent(eventName, eventDate, eventTime, totalSeats, ticketPrice, selectedEventType, venue);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Event created successfully.");
            Console.ResetColor();   
        }

        static void BookTickets(TicketBookingSystem ticketBookingSystem, EventRepo eventRepo,List<Booking> bookings)
        {
            Event selectedEvent = DisplayEventsAndGetSelection(eventRepo);
            if (selectedEvent == null) return;
            int numTickets = GetNumberOfTicketsFromUser();
            Customer[] arrayOfCustomer = CustomerRepo.GetCustomers(numTickets);
            ticketBookingSystem.BookTickets(selectedEvent, eventRepo, numTickets, arrayOfCustomer, bookings);
        }
        static void CancelTickets(TicketBookingSystem ticketBookingSystem, EventRepo eventRepo)
        {
            Event selectedEvent = DisplayEventsAndGetSelection(eventRepo);
            int numTickets = GetNumberOfTicketsFromUser();
            ticketBookingSystem.CancelTickets(selectedEvent,eventRepo ,numTickets);
        }

        public static Event DisplayEventsAndGetSelection(EventRepo eventRepo)
        {
            Console.WriteLine("\nAvailable Events:");
            var events=eventRepo.GetAllEvents();
            if (events == null || events.Count == 0)
            {
                Console.WriteLine("No events available.");
                return null;
            }

            for (int i = 0; i < events.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {events[i].EventName}");
            }

            int selectedOption = -1;
            while (selectedOption < 1 || selectedOption > events.Count)
            {
                Console.Write("Enter the number of the event: ");
                if (int.TryParse(Console.ReadLine(), out selectedOption) && selectedOption > 0 && selectedOption <= events.Count)
                {
                    return events[selectedOption - 1];
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid selection. Please enter a valid number.");
                    Console.ResetColor();   
                }
            }

            return null;
        }

        private static int GetNumberOfTicketsFromUser()
        {
            Console.Write("Enter the number of tickets you want to book: ");
            int numTickets = int.Parse(Console.ReadLine());
            if (numTickets <= 0)
            {
                Console.WriteLine("Invalid input. Please enter a valid number greater than 0.");
            }

            return numTickets;
        }
        #region Task 1: Conditional Statements
        /*In a BookingSystem, you have been given the task is to create a program to book tickets. If available
        tickets more than noOfTicket to book then display the remaining tickets or ticket unavailable */

        //Tasks:
        // 1.Write a program that takes the availableTicket and noOfBookingTicket as input.
        // 2.Use conditional statements(if-else) to determine if the ticket is available or not.
        // 3.Display an appropriate message based on ticket availability.

        //Console.WriteLine("Enter the number of available tickets: ");
        //int availableTicket = Convert.ToInt32(Console.ReadLine());
        //Console.WriteLine("Enter the number of tickets to book: ");
        //int noOfBookingTicket = Convert.ToInt32(Console.ReadLine());

        //if (availableTicket >= noOfBookingTicket)
        //{

        //    Console.WriteLine($"{noOfBookingTicket} tickets booked successfully!");
        //}
        //else
        //{
        //    Console.WriteLine($"Tickets sold out!");
        //}
        #endregion

        #region Task 2: Nested Conditional Statements
        /*Create a program that simulates a Ticket booking and calculating cost of tickets. Display tickets options
         such as "Silver", "Gold", "Diamond".Based on ticket category fix the base ticket price and get the user input
         for ticket type and no of tickets need and calculate the total cost of tickets booked.*/

        //Console.WriteLine($"Tickets type available :\n'1:Silver'\n'2:Gold'\n'3:Diamond'");
        //Console.WriteLine($"Enter the type by choosing the options:");
        //int ticketType = Convert.ToInt32(Console.ReadLine());
        //Console.WriteLine($"Enter the number of tickets needed: ");
        //int ticketsNeeded = Convert.ToInt32(Console.ReadLine());
        //if (ticketType == 1)
        //{
        //    int silverTicket = ticketsNeeded * 3000;
        //    Console.WriteLine($"Total cost of tickets booked:{silverTicket}");
        //}
        //else if (ticketType == 2)
        //{
        //    int goldTicket = ticketsNeeded * 5000;
        //    Console.WriteLine($"Total cost of tickets booked:{goldTicket}");
        //}
        //else if (ticketType == 3)
        //{
        //    int diamondTicket = ticketsNeeded * 8000;
        //    Console.WriteLine($"Total cost of tickets booked:{diamondTicket}");
        //}
        //else
        //{
        //    Console.WriteLine("Ticket type not available");
        //}
        #endregion

        #region Task 3: Looping
        //From the above task book the tickets for repeatedly until user type "Exit"
        //    string input;
        //    do
        //    {
        //        Console.WriteLine("Book your Tickets!");
        //        string ticketType = "1:Silver\n2:Gold\n3:Diamond";
        //        Console.WriteLine($"Available ticket types:\n{ticketType}\n Choose a ticket type");
        //        int type = int.Parse(Console.ReadLine());
        //        Console.WriteLine("Enter the number of tickets: ");
        //        int ticketsneeded = Convert.ToInt32(Console.ReadLine());

        //        switch (type)
        //        {
        //            case 1:
        //                Console.WriteLine($"Total cost of tickets:{ticketsneeded * 3000}");
        //                break;
        //            case 2:
        //                Console.WriteLine($"Total cost of tickets:{ticketsneeded * 5000}");
        //                break;
        //            case 3:
        //                Console.WriteLine($"Total cost of tickets:{ticketsneeded * 8000}");
        //                break;

        //        }
        //    input = Console.ReadLine();

        //    } while (input != "Exit");
        //}

        #endregion

        #region Task 4: Class & Object
        ////Event @event = new Event("IPL Match",EventType.Sports, 40000, 2500, "Wankede", 3000);

        //Venue venue=new Venue("Wankede","Mumbai");
        //Customer customer=new Customer("Anu","anu345@gmail.com",9254638775);
        ////Booking booking = new Booking(5);
        #endregion

        #region Task 5: Inheritance and Polymorphism
        //Movie movie = new Movie("Avengers", EventType.Movie,450,125, "AMC Theatre",340,Movie.Genre.Action,"Chris Evans","Scarlett Johnsson");
        //Concert concert=new Concert("Eras Tour Concert",EventType.Concert,60000,1200,"Sofi Stadium",8000,Concert.Type.Rock,"Taylor Swift");
        //Sports sports=new Sports("IPL Match", EventType.Sports, 40000, 2500, "Wankede", 3000,"Cricket","CSK vs MI");


        //TicketBookingSystem ticketBookingSystem = new TicketBookingSystem("Football Match: Barcelona vs Real Madrid", "2024-05-20", "15:02:15", "Sport",500, "Stadium 1",25.00f);
        //while (true)
        //{

        //    Console.WriteLine("Welcome to Ticket Booking System!");
        //    Console.WriteLine("1. Book Tickets");
        //    Console.WriteLine("2. View Event Details");
        //    Console.WriteLine("3. Cancel Tickets");
        //    Console.WriteLine("4. Exit");
        //    Console.Write("Enter your choice: ");
        //    int choice = Convert.ToInt32(Console.ReadLine());

        //    switch(choice)
        //    {
        //        case 1:
        //            ticketBookingSystem.book_tickets();
        //            break;
        //        case 2:
        //            ticketBookingSystem.create_event();
        //            break;
        //        case 3:
        //            ticketBookingSystem.cancel_tickets();
        //            break;
        //        case 4:
        //            Environment.Exit(0);
        //            break;
        //         default:
        //            Console.WriteLine("Thank You for visiting!");
        //            break;

        //    }
        //}
        #endregion

        //#region Events
        //List<Event> events = new List<Event>()
        //    {
        //        new Event(){EventName= "Movie Night",EventDate= new DateOnly(2024, 5, 15),EventTime= new TimeOnly(18, 30, 0),VenueName="Cinema Hall 1", TotalSeats=300,AvailableSeats = 100,TicketPrice= 150.00M ,EventTypes = EventTypes.Movie },
        //        new Event(){EventName="Cricket Match", EventDate=new DateOnly(2024, 6, 20), EventTime=new TimeOnly(19, 0, 0),VenueName= "Stadium",TotalSeats= 5000, AvailableSeats = 2000,TicketPrice= 8000.00M ,EventTypes =EventTypes.Sports},
        //        new Event(){EventName="Concert", EventDate=new DateOnly(2024, 7, 10), EventTime= new TimeOnly(20, 0, 0), VenueName="Outdoor Arena",TotalSeats= 2000,AvailableSeats = 500, TicketPrice= 999.00M ,EventTypes =EventTypes.Concert}
        //    };
        //foreach (Event e in events)
        //{
        //    eventRepoObj.DisplayEventDetails();
        //}

        //Event eventToBook = events[2];

        ////Calculate_total_revenue()
        //decimal totalRevenue = 0;
        //foreach (Event e in events)
        //{
        //    decimal revenue = eventRepoObj.CalculateTotalRevenue(e, e.TotalSeats - e.AvailableSeats);
        //    totalRevenue += revenue;

        //}
        //Console.WriteLine($"Total Revenue: {totalRevenue}");

        ////getBookedNoOfTickets()
        //Console.WriteLine($"Total Number of tickets booked:{eventRepoObj.GetBookedNumberOfTickets(eventToBook)}");

        ////book_tickets(num_tickets)
        //int numTicketsToBook = 5;
        //eventRepoObj.BookTickets(eventToBook, numTicketsToBook);
        //int bookedTickets = numTicketsToBook;
        //Console.WriteLine($"Number of tickets Booked  for {eventToBook.EventName}: {bookedTickets}\n");

        ////display_event_details()
        //foreach (Event e in events)
        //{
        //    eventRepoObj.DisplayEventDetails();
        //}

        ////cancel_booking(num_tickets)
        //eventRepoObj.CancelBooking(eventToBook, numTicketsToBook);

        //bookedTickets -= numTicketsToBook;
        //Console.WriteLine($"Booked tickets after cancellation for {eventToBook.EventName}: {bookedTickets}");

        //#endregion
        //#region Venue
        //List<Venue> venues = new List<Venue>()
        //    {
        //        new Venue(){VenueName="PVR Cinemas",VenueAddress="Chennai"},
        //        new Venue(){VenueName="M.A.Chidambaram",VenueAddress = "Chennai"},
        //        new Venue(){VenueName="YMCA Nandhanam",VenueAddress = "Chennai"}
        //    };
        //foreach (Venue v in venues)
        //{
        //    VenueRepo.DisplayVenueDetails(v);
        //}
        //#endregion
        //#region Customer
        //List<Customer> customers = new List<Customer>()
        //    {
        //        new Customer(){CustomerName="Ganesh",CustomerMailId="ganesh@gmail.com",CustomerPhoneNumber=1234567890},
        //        new Customer(){CustomerName="Raaj",CustomerMailId="Raaj@gmail.com",CustomerPhoneNumber=9876543210},
        //        new Customer(){CustomerName="Raha",CustomerMailId="Raha@gmail.com",CustomerPhoneNumber=3546123782}
        //    };
        //foreach (Customer c in customers)
        //{
        //    CustomerRepo.DisplayCustomerDeatils(c);
        //}
        //#endregion
        //#region Booking

        //decimal totalCost = bookingRepoObj.CalculateBookingCost(numTicketsToBook, eventToBook.TicketPrice);
        //Console.WriteLine($"Total booking cost: {totalCost}");

        //// Book a specified number of tickets for an event
        //bookingRepoObj.BookTickets(numTicketsToBook, eventToBook, eventRepoObj);

        ////display_event_details()
        //foreach (Event e in events)
        //{
        //    eventRepoObj.DisplayEventDetails();
        //}

        //// Cancel the booking and update the available seats
        //bookingRepoObj.CancelBooking(numTicketsToBook, eventToBook, eventRepoObj);
        //#endregion

        public static void CenterText(string text)
        {
            int screenWidth = Console.WindowWidth;
            int textWidth = text.Length;
            int spaces = (screenWidth - textWidth) / 2;
            Console.WriteLine(new string(' ', spaces) + text);
        }
    }
}



