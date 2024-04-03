using System;
using System.Collections.Generic;
using Cinema.Data;

namespace Cinema.Services
{
  public static class ShowTimeGenerator
  {
    public static List<Showtime> GenerateShowtimes(DateTimeOffset startDate, int weeks, Movie movie)
    {
      var showtimes = new List<Showtime>();
      var roomIdRandom = new Random();

      // Loop through each week
      for (int i = 0; i < weeks; i++)
      {
        var currentDate = startDate.AddDays(i * 7); // Increase the date by a week

        // Loop through each day in the week
        for (int j = 0; j < 7; j++)
        {
          var currentDay = currentDate.AddDays(j);

          // Loop through hours from 8AM to 5PM
          for (int hour = 8; hour <= 17; hour += 3)
          {
            var startTime = new DateTimeOffset(currentDay.Year, currentDay.Month, currentDay.Day, hour, 0, 0, TimeSpan.Zero);
            var roomId = roomIdRandom.Next(1, 11).ToString(); // Random room id between 1 and 10

            var showtime = new Showtime
            {
              RoomId = roomId,
              StartTime = startTime,
              Movie = movie
            };

            showtimes.Add(showtime);
          }
        }
      }

      return showtimes;
    }

  }
}
