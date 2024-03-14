public class PresentLogin
{
    public static void Start() // Main method to initiate login process
    {
        string? userChoice = ""; // Stores user's choice (nullable for potential null input)
        bool loginSuccessful; // Flag to indicate successful login

        while (userChoice.ToLower() != "quit") // Loop until user quits
        {
            // Prompt user for action
            Console.WriteLine("Do you wish to login or continue as guest? '+' to create new superuser. 'quit' to quit.");
            userChoice = Console.ReadLine(); // Get user's choice

            if (userChoice.ToLower() == "login") // Login option selected
            {
                bool validEmail = false; // Flag to indicate valid email
                bool validPassword = false; // Flag to indicate valid password

                while (!validEmail) // Loop until both email and password are valid
                {
                    // Prompt user for email
                    Console.WriteLine("Enter your email:");
                    string email = Console.ReadLine() ?? ""; // Get email (null check for potential empty input)

                    validEmail = Login.IsValidEmail(email); // Validate email format

                    if (validEmail) // If email is valid
                    {
                        while (!validPassword)
                        {
                            // Prompt user for password
                            Console.WriteLine("Enter your password:");
                            string password = Console.ReadLine() ?? ""; // Get password (null check)

                            validPassword = Login.IsValidPassword(password); // Validate password complexity
                            if (!validPassword) // If password is valid
                            {
                                try // Try block for potential exceptions during login
                                {
                                    loginSuccessful = Login.TryLogin(email, password);

                                    if (loginSuccessful) // Login successful
                                    {
                                        Console.WriteLine("Login successful!");
                                        // PresentMovies.Start(); // Call to present movies (implementation not shown)
                                        break; // Exit the loop
                                    }
                                    else // Login failed
                                    {
                                        Console.WriteLine("Invalid email or password. Or your account does not exist.");
                                    }
                                }
                                catch (Exception ex) // Catch general exceptions for login errors
                                {
                                    Console.WriteLine("Error during login: {0}", ex.Message);
                                }
                            }
                            else // Password invalid
                            {
                                Console.WriteLine("Invalid password. Please check password complexity requirements.");
                            }
                        }
                        
                    }
                    else // Email invalid
                    {
                        Console.WriteLine("Invalid email format. Please enter a valid email address.");
                    }
                }
            }
            else if (userChoice == "+") // Create superuser option selected
            {
                bool validEmail = false;
                bool validPassword = false;

                while (!validEmail) // Loop until both email and password are valid
                {
                    // Prompt user for email
                    Console.WriteLine("Enter your email:");
                    string email = Console.ReadLine() ?? ""; // Get email (null check)

                    validEmail = Login.IsValidEmail(email); // Validate email format

                    if (validEmail) // If email is valid
                    {
                        while(!validPassword)
                        {
                            // Prompt user for password
                            Console.WriteLine("Enter your password:");
                            string password = Console.ReadLine() ?? ""; // Get password (null check)

                            validPassword = Login.IsValidPassword(password); // Validate password complexity

                            if (!validPassword) // If password is valid
                            {
                                try // Try block for potential exceptions during superuser creation
                                {
                                    Superuser.CreateSuperuser(email, password); // Create superuser
                                    Console.WriteLine("Superuser created successfully!");
                                    break;
                                }
                                catch (Exception ex) // Catch general exceptions for superuser creation errors
                                {
                                    Console.WriteLine("Error creating superuser: {0}", ex.Message);
                                }
                            }
                            else // Password invalid
                            {
                                Console.WriteLine("Invalid password. Please check password complexity requirements.");
                            }
                        }
                    }
                    else // Email invalid
                    {
                        Console.WriteLine("Invalid email format. Please enter a valid email address.");
                    }
                }
            }
            else if (userChoice == "guest") // Continue as guest option selected
            {
                Console.WriteLine("Continuing as a guest.");
                // PresentMovies.Start(); // Call to present movies (implementation not shown)
                break;
            }
            else if (userChoice == "quit") // Quit option selected
            {
                Console.WriteLine("Sad to see you go, have a nice day!");
                break;
            }
            else // Invalid option entered
            {
                Console.WriteLine("Not a valid option. Try again.");
            }
        }
    }
}