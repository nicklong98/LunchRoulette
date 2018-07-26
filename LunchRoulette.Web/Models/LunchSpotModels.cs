using System.ComponentModel.DataAnnotations;

namespace LunchRoulette.Web.Models
{
    public class CreateLunchSpotModel
    {
        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        public string CuisineName { get; set; }
        [Required]
        [StringLength(maximumLength: 200, MinimumLength = 2)]
        public string LunchSpotName { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

    public class GetLunchSpotModel
    {
        [Required]
        [Range(minimum: 1, maximum: int.MaxValue)]
        public int LunchSpotId { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

    public class UpdateLunchSpotModel
    {
        [Required]
        public int LunchSpotId { get; set; }
        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        public string LunchSpotName { get; set; }
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        public string CuisineName { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }

    public class SearchLunchSpotModel
    {
        public string LunchSpotName { get; set; }
        public string CuisineName { get; set; }

        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}