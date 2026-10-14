namespace api_raceday_part2.models
{//start of namespace

    public class User
    {//start of User class

        //user class properties
        public int UserID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; //note: never return a user's password or stored password hash in an API response
        public int RoleID { get; set; } // Foreign key to Role

    }//end of User class

}//end of namespace
