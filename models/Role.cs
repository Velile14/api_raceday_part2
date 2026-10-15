namespace api_raceday_part2.models
{//start of namespace

    public class Role
    {//start of Role class

        //role class properties
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;

        //navigation property for related users
        public ICollection<User> Users { get; set; } = new List<User>();

    }//end of Role class

}//end of namespace
