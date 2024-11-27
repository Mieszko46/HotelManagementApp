<h3>Instruction</h3>

To prepare the data files for the project go to the bin folder then \Debug\.net8.0\ and put there hotels.json and bookings.json. I will post my bin folder with some data, so you can easier test the application.

If somehow the bin folder will not be present, use command 'dotnet build' in the terminal, in the project folder. Then you will need to add the hotels.json and bookings.json files.

After preparing project like that, open terminal in the project folder and type 
'dotnet run --hotels hotels.json --bookings bookings.json'

After that program should run.

Below I give some example input cases for prepared data files:

Availability(H1, 20240901, SGL)
Availability(H1, 20240901-20240903, DBL)
Availability(H2, 20240901, SGL)
Availability(H2, 20240903-20240905, DBL)


<h3>Assumptions</h3>

1. I assume that if we check the availability of a room on one specific day, it means that 
arrival will take place on that day and departure on the next day, i.e.

Availability(H1, 20240901, DBL), is equivalent to Availability(H1, 20240901-20240902, DBL)

Basing on real life examples, the rooms are occupied from the afternoon until the morning of the next day.
Therefore, if the departure time of one booking is on the same day as the arrival time of another booking, I assume they don't overlap.

2. In order to keep the solution simple, which was meant to be on of the key aspects of this task. (information from the email and blog post: https://medium.com/guestline-labs/hints-for-our-interview-process-and-code-test-ae647325f400)

I store the dates in the booking model as string types.

I would in general convert the input when reading from a file and store it in the appropriate DateTime format, which is certainly a better solution in terms of future implementation.

And I also added a test method, for which I would normally use the XUnit library, but I didn't want to add additional dependencies to the project just for one method, so I made it simpler.
