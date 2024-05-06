namespace Cinema.Data
{
    public class Administrator
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public static List<Administrator> AllAdministrators = new List<Administrator>();

        private static void RetrieveAdministrators(CinemaContext db)
        {
            AllAdministrators = db.Administrators.ToList();
        }

        public static bool FindAdministrator(CinemaContext db, string username, string password)
        {
            RetrieveAdministrators(db);

            foreach (Administrator existingAdmin in AllAdministrators)
            {
                if (existingAdmin.Username == username && existingAdmin.Password == password)
                {
                    return true;
                }
            }
            return false;
        }
    }
}