using Cinema.Data;
using Cinema.Models.Choices;
using Cinema.Services;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

public class UserExperienceService : ManagementExperienceService
{

  private static readonly Dictionary<UserExperienceChoice, string> UserExperienceChoiceDescriptions = new Dictionary<UserExperienceChoice, string>
  {
      { UserExperienceChoice.ListMovies, "Blader door films & vertoningen" },
      { UserExperienceChoice.ViewTickets, "Bekijk uw tickets" },
      { UserExperienceChoice.Logout, "Uitloggen" }
  };
  public void ManageUser(Customer customer, CinemaContext db, IConfiguration configuration)
  {
    // Initialize the current user choice
    UserExperienceChoice currentUserChoice = UserExperienceChoice.ListMovies;

    // Loop until the user chooses to logout
    while (currentUserChoice != UserExperienceChoice.Logout)
    {
      Console.Clear();

      // Prompt the user for their choice
      var choice = AnsiConsole.Prompt(
          new SelectionPrompt<string>()
              .Title($"Welcome {customer.Username}! What would you like to do?")
              .PageSize(10)
              .AddChoices(UserExperienceChoiceDescriptions.Select(kv => kv.Value))
      );

      // Retrieve the enum value based on the selected description
      currentUserChoice = UserExperienceChoiceDescriptions.FirstOrDefault(kv => kv.Value == choice).Key;

      // Execute the corresponding action based on the user's choice
      switch (currentUserChoice)
      {
        case UserExperienceChoice.ListMovies:
          // Implement logic to list movies for the user
          ListMoviesWithShowtimes(db);
          break;
        case UserExperienceChoice.ViewTickets:
          // Implement logic to view user's tickets
          ViewTickets(db);
          break;
        default:
          break;
      }
    }
  }

  private void ListMovies(CinemaContext db)
  {
    // Implement logic to list movies for the user
  }

  private void ViewTickets(CinemaContext db)
  {
    // Implement logic to view user's tickets
  }
}
