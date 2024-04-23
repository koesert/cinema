namespace Cinema.Data
{
	public class Customer
	{
		public int Id { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public static List<Customer> AllCustomers = new List<Customer>();

		private static void RetrieveCustomers(CinemaContext db)
		{
			AllCustomers = db.Customers.ToList();
		}

		public static Customer FindCustomer(CinemaContext db, string username, string password)
		{
			RetrieveCustomers(db);

			foreach (Customer existingCustomer in AllCustomers)
			{
				if (existingCustomer.Username == username && existingCustomer.Password == password)
				{
					return existingCustomer;
				}
			}
			return null;
		}
	}
}