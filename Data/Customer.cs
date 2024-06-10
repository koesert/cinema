namespace Cinema.Data
{
	public class Customer
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public bool Subscribed { get; set; }
		public DateTimeOffset? SubscribedSince { get; set; }
		public static List<Customer> AllCustomers = new List<Customer>();

		private Customer(string username, string password, string email)
		{
			Username = username;
			Password = password;
			Email = email;
		}

		public Customer()
		{
		}

		private static void RetrieveCustomers(CinemaContext db)
		{
			AllCustomers = db.Customers.ToList();
		}

		public static int FindCustomer(CinemaContext db, string email, string password)
		{
			RetrieveCustomers(db);

			foreach (Customer existingCustomer in AllCustomers)
			{
				if (existingCustomer.Email == email.ToLower())
				{
					if (existingCustomer.Password == password)
					{
						// Credentials matched
						return 1;
					}
					else
					{
						// Email found but password didn't match
						return 2;
					}
				}
			}
			// No account found with the provided email
			return 0;
		}

		public static bool CheckEmailCustomer(CinemaContext db, string email)
		{
			RetrieveCustomers(db);

			foreach (Customer existingCustomer in AllCustomers)
			{
				if (existingCustomer.Email == email)
				{
					return true;
				}
			}
			return false;
		}
		public static Customer CreateCustomer(CinemaContext db, string username, string password, string email)
		{
			Customer newCustomer = new Customer(username, password, email.ToLower());
			db.Customers.Add(newCustomer);
			db.SaveChanges();

			return newCustomer;
		}

		public static void UpdateCustomer(CinemaContext db, Customer customer)
		{
			db.Customers.Update(customer);
			db.SaveChanges();
		}

		public static void DeleteCustomer(CinemaContext db, Customer customer)
		{
			db.Customers.Remove(customer);
			db.SaveChanges();
		}

		public static bool UpdatePreference(Customer customer, CinemaContext db)
		{
			RetrieveCustomers(db);
			if (customer.Subscribed == false)
			{
				customer.Subscribed = true;
				DateTime newDateTime = DateTime.UtcNow.AddHours(2);
				DateTimeOffset dateTimeOffset = new DateTimeOffset(newDateTime);
				DateTimeOffset dateOnly = new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, 0, 0, 0, dateTimeOffset.Offset);
				customer.SubscribedSince = dateOnly;
				db.Customers.Update(customer);
				db.SaveChanges();
				return true;
			}
			else
			{
				customer.Subscribed = false;
				customer.SubscribedSince = null;
				db.Customers.Update(customer);
				db.SaveChanges();
				return false;
			}
		}

		public override string ToString()
		{
			return $"Username: {Username}, Email: {Email}";
		}
	}
}