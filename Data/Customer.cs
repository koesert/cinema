namespace Cinema.Data
{
	public class Customer
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
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

		public static Customer FindCustomer(CinemaContext db, string email, string password)
		{
			RetrieveCustomers(db);

			foreach (Customer existingCustomer in AllCustomers)
			{
				if (existingCustomer.Email == email && existingCustomer.Password == password)
				{
					return existingCustomer;
				}
			}
			return null;
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
			Customer newCustomer = new Customer(username, password, email);
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

		public override string ToString()
		{
			return $"Username: {Username}, Email: {Email}";
		}
	}
}