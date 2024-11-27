using System.Text.Json;
using Questline.Models;

namespace Questline
{
	class Program
	{
		static void ParseDatesToDateTime(string[] dates, out DateTime startDate, out DateTime endDate)
		{
			DateTime.TryParseExact(dates[0], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None,
				out startDate);

			if (dates.Length == 1)
			{
				endDate = startDate.AddDays(1);
			}
			else
			{
				DateTime.TryParseExact(dates[1], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None,
					out endDate);
			}
		}

		static int GetRoomAvailability(List<Hotel> hotels, List<Booking> bookings, string hotelId, string[] dates, string roomType)
		{
			ParseDatesToDateTime(dates, out var startDate, out var endDate);

			var hotel = hotels.FirstOrDefault(h => h.Id == hotelId);
			if (hotel == null)
			{
				Console.WriteLine($"Hotel {hotelId} not found.");
				return 0;
			}

			var totalRooms = hotel.Rooms.Count(r => r.RoomType == roomType);
			if (totalRooms == 0)
			{
				Console.WriteLine($"Room type {roomType} not found in hotel {hotelId}.");
				return 0;
			}

			var overlappingBookings = bookings
				.Where(b => b.HotelId == hotelId && b.RoomType == roomType
				    && DateTime.ParseExact(b.Arrival, "yyyyMMdd", null)  < endDate
				    && DateTime.ParseExact(b.Departure, "yyyyMMdd", null) > startDate)
				.ToList();

			return totalRooms - overlappingBookings.Count;
		}


		static void Main(string[] args)
		{
			TestGetRoomAvailability_ShouldReturnCorrectAvailability();

			if (args.Length < 4)
			{
				Console.WriteLine("Usage: dotnet run --hotels hotels.json --bookings bookings.json");
				return;
			}

			var hotelsFile = args[1];
			var bookingsFile = args[3];

			if (!File.Exists(hotelsFile) || !File.Exists(bookingsFile))
			{
				Console.WriteLine("Error: One or more files not found.");
				return;
			}

			var hotels = JsonSerializer.Deserialize<List<Hotel>>(File.ReadAllText(hotelsFile));
			var bookings = JsonSerializer.Deserialize<List<Booking>>(File.ReadAllText(bookingsFile));


			while (true)
			{
				Console.WriteLine("\nEnter command (or blank line to exit)");
				Console.WriteLine("Use format: Availability(hotelId, dateRange, roomType)");
				var input = Console.ReadLine();

				if (string.IsNullOrWhiteSpace(input)) break;

				var parts = input.Split('(', ')', ',');
				if (parts.Length < 4 || !parts[0].Trim().Equals("Availability", StringComparison.OrdinalIgnoreCase))
				{
					Console.WriteLine("Invalid command. Use format: Availability(hotelId, dateRange, roomType)");
					continue;
				}

				var hotelId = parts[1].Trim();
				var dateRange = parts[2].Trim();
				var roomType = parts[3].Trim();
				var dates = dateRange.Split('-');

				var availability = GetRoomAvailability(hotels, bookings, hotelId, dates, roomType);

				Console.WriteLine($"For this requirements, the number of available rooms is: {availability}");
			}
		}

		static void TestGetRoomAvailability_ShouldReturnCorrectAvailability()
		{
			// Arrange
			var hotels = new List<Hotel>
			{
				new Hotel
				{
					Id = "H1",
					Name = "Hotel California",
					Rooms = new List<Room>
					{
						new Room { RoomType = "SGL", RoomId = "101" },
						new Room { RoomType = "SGL", RoomId = "102" },
						new Room { RoomType = "DBL", RoomId = "201" },
						new Room { RoomType = "DBL", RoomId = "202" }
					}
				}
			};

			var bookings = new List<Booking>
			{
				new Booking
				{
					HotelId = "H1",
					Arrival = "20240901",
					Departure = "20240903",
					RoomType = "DBL",
					RoomRate = "Prepaid"
				},
				new Booking
				{
					HotelId = "H1",
					Arrival = "20240902",
					Departure = "20240905",
					RoomType = "SGL",
					RoomRate = "Standard"
				}
			};

			string[] singleDate = ["20240901"];
			string[] rangeDates = ["20240901", "20240903"];

			// Act
			var singleRoomAvailability = Program.GetRoomAvailability(hotels, bookings, "H1", singleDate, "SGL");
			var doubleRoomAvailability = Program.GetRoomAvailability(hotels, bookings, "H1", rangeDates, "DBL");

			// Assert
			if (singleRoomAvailability == 2)
				Console.WriteLine("Test case 1. - passed");
			else
				Console.WriteLine("Test case 1. - NOT passed");

			if (doubleRoomAvailability == 1)
				Console.WriteLine("Test case 2. - passed");
			else
				Console.WriteLine("Test case 2. - NOT passed");
		}
	}
}
