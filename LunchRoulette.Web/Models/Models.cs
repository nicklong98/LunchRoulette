using System.Collections.Generic;

namespace LunchRoulette.Web.Models
{
    public class ErrorModel
    {
        public string Message{ get; set; }
        public List<string> ErrorMessages{ get; set; }

        public ErrorModel()
        {
            ErrorMessages = new List<string>();
        }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}