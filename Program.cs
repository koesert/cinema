using Newtonsoft.Json.Linq;

class Program
{
    static void Main(string[] args)
    {
        string folderPath = "movies";

        if (Directory.Exists(folderPath))
        {
            string[] files = Directory.GetFiles(folderPath, "*.json");

            // Display the list of movies
            Console.WriteLine("Select a movie:");

            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileNameWithoutExtension(files[i]);
                Console.WriteLine($"{i + 1}. {fileName}");
            }

            // Prompt the user to select a movie
            Console.Write("Enter the number of the movie: ");
            int selectedIndex = int.Parse(Console.ReadLine()) - 1;

            // Check if the selected index is valid
            if (selectedIndex >= 0 && selectedIndex < files.Length)
            {
                string json = File.ReadAllText(files[selectedIndex]);
                JObject obj = JObject.Parse(json);

                // Display movie details
                Console.WriteLine("\nMovie Details:");
                Console.WriteLine($"Title: {obj["name"]}");
                Console.WriteLine($"Year: {obj["year"]}");
                Console.WriteLine($"Duration: {obj["runtime"]} minutes");

                // Handle the case where there's only one director
                JToken directorsToken = obj["director"];
                if (directorsToken.Type == JTokenType.String)
                {
                    Console.WriteLine("\nDirector:");
                    Console.WriteLine($"- {directorsToken}");
                }
                else if (directorsToken.Type == JTokenType.Array)
                {
                    Console.WriteLine("\nDirectors:");
                    foreach (var director in directorsToken)
                    {
                        Console.WriteLine($"- {director}");
                    }
                }

                Console.WriteLine("\nActors:");
                foreach (var actor in obj["actors"])
                {
                    Console.WriteLine($"- {actor}");
                }

                Console.WriteLine("\nStoryline:");
                Console.WriteLine(obj["storyline"]);
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Invalid movie selection.");
            }
        }
        else
        {
            Console.WriteLine("Folder does not exist");
        }
    }
}
