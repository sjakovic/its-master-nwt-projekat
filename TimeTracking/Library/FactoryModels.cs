using TimeTracking.Models;

namespace TimeTracking.Library
{
    public class FactoryModels
    {
        public static AspNetUser CreateAspNetUserByEmail (string email)
        {
            AspNetUser user = new AspNetUser();
            user.LoadByEmail(email);
            return user;
        }
    }
}
