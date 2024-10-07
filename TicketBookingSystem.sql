-- TASKS 1: DATABASE DESIGN:

-- Task 1.1: Create the database named "TicketBookingSystem"
CREATE DATABASE TicketOrderingSystems
USE TicketOrderingSystems

-- Task 1.2: Write SQL scripts to create the mentioned tables with appropriate data types, constraints, and relationships
-- Venu Table
CREATE TABLE Venue (
venue_id INT PRIMARY KEY IDENTITY(1,1),
venue_name VARCHAR(50),
address VARCHAR(50)
);

-- Event Table
CREATE TABLE Event (
event_id INT PRIMARY KEY IDENTITY(1,1),
event_name VARCHAR(50),
event_date DATE,
event_time TIME,
venue_id INT,
total_seats INT,
available_seats INT,
ticket_price DECIMAL(10, 2),
event_type CHAR(10),
booking_id INT,
FOREIGN KEY (venue_id) REFERENCES Venue(venue_id) ON DELETE CASCADE
);

-- Customer Table
CREATE TABLE Customer (
customer_id INT PRIMARY KEY IDENTITY(1,1),
customer_name VARCHAR(50),
email VARCHAR(50),
phone_number BIGINT,
booking_id INT
);

-- Booking Table
CREATE TABLE Booking (
booking_id INT PRIMARY KEY IDENTITY(1,1),
customer_id INT,
event_id INT,
num_tickets INT,
total_cost INT,
booking_date DATE,
FOREIGN KEY (customer_id) REFERENCES Customer(customer_id) ON DELETE CASCADE,
FOREIGN KEY (event_id) REFERENCES Event(event_id) ON DELETE CASCADE
);

-- Task 1.3: Create an ERD (Entity Relationship Diagram) for the database

-- Task 1.4: Create appropriate Primary Key and Foreign Key constraints for referential integrity 

-- Adding Venue Foreign Key in Event
ALTER TABLE Event 
ADD CONSTRAINT fk_venue FOREIGN KEY (venue_id) REFERENCES Venu(venue_id) ON DELETE CASCADE ON UPDATE CASCADE;

-- Adding Customer foreign key constraints in Booking table
ALTER TABLE Booking
ADD CONSTRAINT fk_customer_booking FOREIGN KEY (customer_id) REFERENCES Customers(customer_id) ON DELETE CASCADE ON UPDATE CASCADE;

-- Adding Event foreign key constraints in Booking table
ALTER TABLE Booking
ADD CONSTRAINT fk_event_booking FOREIGN KEY (event_id) REFERENCES Event(event_id) ON DELETE CASCADE ON UPDATE CASCADE;

-- Adding Booking Foreign Key in Event and Customer
ALTER TABLE Event
ADD CONSTRAINT fk_booking_event FOREIGN KEY (booking_id) REFERENCES Booking(booking_id);

ALTER TABLE Customers
ADD CONSTRAINT fk_booking_customers FOREIGN KEY (booking_id) REFERENCES Booking(booking_id);

--Tasks 2: Select, Where, Between, AND, LIKE:

-- Task 2.1: Insert at least 10 sample records into each tables:
  INSERT INTO Venue VALUES('M.A.Chidambaram Stadium','Chekpauk'),
('Luxe Cinemas','Chennai'),
('Jawaharlal Nehru Stadium','Chennai'),
('Wankhede Stadium','Mumbai'),
('EVP Cinemas','Chembarambakkam'),
('National Centre for Performing Arts (NCPA)','Mumbai'),
('M. Chinnaswamy Stadium','Bangalore'),
('Eros Cinemas','Mumbai'),
('Barabati stadium','Odisha'),
('Rajiv Gandhi International Cricket Stadium','Hyderabad')

SELECT * FROM Venue
 
INSERT INTO Event VALUES ('CSKvRCB','2024-04-25','06:30:00',1,10000,100,12000.00,'Sports',Null),
('Movie Night: Avengers Endgame', '2024-05-03', '19:00:00', 2,450, 200, 350.00, 'Movie', Null),
('U1 Concert', '2024-02-03', '11:00:00', 3, 5000, 180, 10000.00, 'Concert', Null),
('India vs Australia', '2024-05-01', '14:00:00', 4, 15000, 150, 8000.00, 'Sports', Null),
('KongVGodzilla', '2024-05-05', '20:30:00', 5, 300, 19, 700.00, 'Movie', Null),
('Music Concert: Rock Fest', '2024-05-05', '20:30:00', 6, 2000, 180, 200.00, 'Concert',Null),
('NZ vs PAK', '2024-04-10', '16:00:00', 7,20000, 150, 35000.00, 'Sports',Null),
('Avengers', '2024-05-07', '20:00:00', 8, 300, 19, 1789.53, 'Movie', Null),
('AR Concert', '2024-05-05', '17:30:00', 9, 6000, 180, 20000.00, 'Concert',Null),
('ZIMW vs VANW', '2024-08-1', '12:00:00', 10, 8000, 190, 15000.00, 'Sports', Null)

SELECT * FROM Event

INSERT INTO Customer VALUES ('John Doe', 'john.doe@example.com', 1234567000, NULL),
('Jane Smith', 'jane.smith@example.com', 9876543210, NULL),
('Alice Johnson', 'alice.johnson@example.com', 5551234567, NULL),
('Bob Williams', 'bob.williams@example.com', 7779876543, NULL),
('Emily Brown', 'emily.brown@example.com', 9998887776, NULL),
('Michael Davis', 'michael.davis@example.com', 1112223334, NULL),
('Sarah Wilson', 'sarah.wilson@example.com', 4445556667, NULL),
('David Martinez', 'david.martinez@example.com', 6667778889, NULL),
('Jessica Taylor', 'jessica.taylor@example.com', 2223334445, NULL),
('Christopher Anderson', 'christopher.anderson@example.com', 8889990001, NULL)

SELECT* FROM Customer

INSERT INTO Booking VALUES(1,1,4,12000,'2024-04-20'),
(2,2,2,350,'2024-04-30'),
(3,3,12,10000,'2024-02-01'),
(4,4,1,8000,'2024-04-02'),
(5,5,5,700,'2024-05-04'),
(6,6,180,200,'2024-05-03'),
(7,7,4,35000,'2024-04-09'),
(8,8,15,1790,'2024-05-07'),
(9,9,12,20000,'2024-03-01'),
(10,10,4,15000,'2024-07-01')

SELECT * FROM Booking

-- Task 2.2: List all Events
SELECT * FROM Event;

-- Task 2.3: Select events with available tickets
SELECT * FROM Event WHERE available_seats > 0;

-- Task 2.4: Select events name partial match with 'cup'
SELECT * FROM Event WHERE event_name LIKE '%cup%';

-- Task 2.5: Select events with ticket price between 1000 to 2500
SELECT * FROM Event WHERE ticket_price BETWEEN 1000 AND 2500;

-- Task 2.6: Retrieve events with dates in specific range
SELECT * FROM Event WHERE event_date BETWEEN '2024-09-01' AND '2024-12-31';

-- Task 2.7: Retrieve events with available tickets and "concert" in their name
SELECT * FROM Event WHERE available_seats > 0 AND event_type = 'Concert';

-- Task 2.8: Retrieve users in batches of 5 starting from the 6th user
SELECT * FROM Customers ORDER BY customer_id OFFSET 5 ROWS FETCH NEXT 5 ROWS ONLY;

-- Task 2.9: Retrieve bookings with more than 4 tickets
SELECT * FROM Booking WHERE num_tickets > 4;

-- Task 2.10: Retrieve customer info with phone number ending in '000'
SELECT * FROM Customers WHERE phone_number LIKE '%000';

-- Task 2.11: Retrieve events with seat capacity more than 15000
SELECT * FROM Event WHERE total_seats > 15000 ORDER BY total_seats;

-- Task 2.12: Select events where name doesn't start with 'x', 'y', or 'z'
SELECT * FROM Event WHERE event_name NOT LIKE 'x%' AND event_name NOT LIKE 'y%' AND event_name NOT LIKE 'z%';


-- TASKS 3: AGGREGATE FUNCTIONS, HAVING, ORDER BY, GROUP BY, AND JOINS:

-- Task 3.1: List events and their average ticket prices
SELECT event_name AS [Event Name], AVG(ticket_price) AS [Avg Ticket Price]
FROM Event 
GROUP BY event_name;

-- Task 3.2: Calculate the total revenue generated by events
SELECT e.event_name AS [Event Name], SUM(b.total_cost) AS [Total Revenue]
FROM Booking b 
JOIN Event e ON b.event_id = e.event_id
GROUP BY e.event_name;

-- Task 3.3: Find the event with the highest ticket sales
SELECT TOP 1 e.event_name AS [Event Name], SUM(b.num_tickets) AS [Total Tickets Sold]
FROM Booking b 
JOIN Event e ON b.event_id = e.event_id
GROUP BY e.event_name
ORDER BY [Total Tickets Sold] DESC;

-- Task 3.4: Total number of tickets sold for each event
SELECT e.event_name AS [Event Name], SUM(b.num_tickets) AS [Total Tickets Sold]
FROM Booking b 
JOIN Event e ON b.event_id = e.event_id
GROUP BY e.event_name;

-- Task 3.5: Find events with no ticket sales
SELECT e.event_name AS [Event Name]
FROM Event e 
LEFT JOIN Booking b ON e.event_id = b.event_id
WHERE b.event_id IS NULL;

-- Task 3.6: Find the user who has booked the most tickets
SELECT TOP 1 c.customer_name AS [Customer Name], SUM(b.num_tickets) AS [Total Tickets]
FROM Booking b 
JOIN Customers c ON b.customer_id = c.customer_id
GROUP BY c.customer_name
ORDER BY [Total Tickets] DESC;

-- Task 3.7: List events and total tickets sold per month
SELECT e.event_name AS [Event Name], MONTH(b.booking_date) AS Month, SUM(b.num_tickets) AS [Total Tickets]
FROM Booking b 
JOIN Event e ON b.event_id = e.event_id
GROUP BY e.event_name, MONTH(b.booking_date);

-- Task 3.8: Average ticket price for events in each venue
SELECT v.venue_name AS [Venue Name], AVG(e.ticket_price) AS [Avg Ticket Price]
FROM Event e 
JOIN Venu v ON e.venue_id = v.venue_id
GROUP BY v.venue_name;

-- Task 3.9: Total number of tickets sold for each event type
SELECT e.event_type AS [Event Type], SUM(b.num_tickets) AS [Total Tickets]
FROM Booking b 
JOIN Event e ON b.event_id = e.event_id
GROUP BY e.event_type;

-- Task 3.10: Total revenue generated by events in each year
SELECT YEAR(b.booking_date) AS Year, SUM(b.total_cost) AS [Total Revenue]
FROM Booking b
GROUP BY YEAR(b.booking_date);

-- Task 3.11: List users who have booked tickets for multiple events
SELECT c.customer_name AS [Customer Name], COUNT(DISTINCT b.event_id) AS [Events Booked]
FROM Booking b 
JOIN Customers c ON b.customer_id = c.customer_id
GROUP BY c.customer_name
HAVING COUNT(DISTINCT b.event_id) > 1;

-- Task 3.12: Total revenue generated by events for each user
SELECT c.customer_name AS [Customer Name], SUM(b.total_cost) AS [Total Revenue]
FROM Booking b 
JOIN Customers c ON b.customer_id = c.customer_id
GROUP BY c.customer_name;

-- Task 3.13: Average ticket price for events in each category and venue
SELECT e.event_type AS [Event Type], v.venue_name AS [Venue Name], AVG(e.ticket_price) AS [Avg Ticket Price]
FROM Event e 
JOIN Venu v ON e.venue_id = v.venue_id
GROUP BY e.event_type, v.venue_name;

-- Task 3.14: Users and total tickets purchased in the last 30 days
SELECT c.customer_name AS [Customer Name], SUM(b.num_tickets) AS [Total Tickets]
FROM Booking b 
JOIN Customers c ON b.customer_id = c.customer_id
WHERE DATEDIFF(day, b.booking_date, GETDATE()) <= 30
GROUP BY c.customer_name;


-- TASKS 4: SUBQUERIES AND THEIR TYPES:

-- Task 4.1: Calculate the Average Ticket Price for Events in Each Venue Using a Subquery
SELECT Venu.venue_name AS [Venue Name],
(SELECT AVG(Event.ticket_price) FROM Event WHERE Event.venue_id = Venu.venue_id) AS [Avg Ticket Price]
FROM Venu;

-- Task 4.2: Find Events with More Than 50% of Tickets Sold using subquery
SELECT Event.event_name AS [Event Name]
FROM Event
WHERE Event.available_seats < 
(SELECT 0.5 * Event.total_seats FROM Event AS e WHERE e.event_id = Event.event_id);

-- Task 4.3: Calculate the Total Number of Tickets Sold for Each Event
SELECT Event.event_name AS [Event Name],
(SELECT SUM(Booking.num_tickets) FROM Booking WHERE Booking.event_id = Event.event_id) AS [Total Tickets Sold]
FROM Event;

-- Task 4.4: Find users who have not booked any tickets using a NOT EXISTS subquery
SELECT Customers.customer_name AS [Customer Name]
FROM Customers
WHERE NOT EXISTS (SELECT 1 FROM Booking WHERE Booking.customer_id = Customers.customer_id);

-- Task 4.5: List events with no ticket sales using a NOT IN subquery
SELECT Event.event_name AS [Event Name]
FROM Event
WHERE Event.event_id NOT IN (SELECT Booking.event_id FROM Booking);

-- Task 4.6: Calculate the total number of tickets sold for each event type using a subquery in the FROM clause
SELECT event_type AS [Event Type], 
(SELECT SUM(b.num_tickets) 
FROM Booking b 
WHERE b.event_id IN (SELECT e.event_id FROM Event e WHERE e.event_type = Event.event_type)) AS [Total Tickets]
FROM Event
GROUP BY event_type;

-- Task 4.7: Find events with ticket prices higher than the average ticket price using a subquery in the WHERE clause
SELECT Event.event_name AS [Event Name]
FROM Event
WHERE Event.ticket_price > (SELECT AVG(ticket_price) FROM Event);

-- Task 4.8: Calculate the total revenue generated by events for each user using a correlated subquery
SELECT Customers.customer_name AS [Customer Name],
(SELECT SUM(Booking.total_cost) FROM Booking WHERE Booking.customer_id = Customers.customer_id) AS [Total Revenue]
FROM Customers;

-- Task 4.9: List users who have booked tickets for events in a given venue using a subquery in the WHERE clause
SELECT DISTINCT customer_name AS [Customer Name]
FROM Customers
WHERE customer_id IN (SELECT Booking.customer_id
FROM Booking
WHERE Booking.event_id IN (SELECT Event.event_id 
FROM Event
WHERE Event.venue_id = 3));

-- Task 4.10: Calculate the total number of tickets sold for each event category using a subquery with GROUP BY
SELECT event_type AS [Event Type], 
(SELECT SUM(b.num_tickets) 
FROM Booking b 
WHERE b.event_id IN (SELECT e.event_id 
FROM Event e 
WHERE e.event_type = Event.event_type)) AS [Total Tickets Sold]
FROM Event
GROUP BY event_type;

-- Task 4.11: Find users who have booked tickets for events in each month using a subquery with DATE_FORMAT
SET DATEFORMAT ymd;  -- Set the date format to year-month-day
SELECT c.customer_name AS [Customer Name], 
(SELECT COUNT(b.booking_id)
FROM Booking b
WHERE b.customer_id = c.customer_id
AND MONTH(b.booking_date) = 9) AS [Tickets Booked]
FROM Customers c;

-- Task 4.12: Calculate the average ticket price for events in each venue using a subquery
SELECT Venu.venue_name AS [Venue Name],
(SELECT AVG(Event.ticket_price)
FROM Event
WHERE Event.venue_id = Venu.venue_id) AS [Avg Ticket Price]
FROM Venu;

