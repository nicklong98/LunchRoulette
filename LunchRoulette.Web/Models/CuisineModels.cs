using System.ComponentModel.DataAnnotations;

namespace LunchRoulette.Web.Models
{
    public class CreateCuisineModel
    {
        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        public string CuisineName{ get; set; }
    }

    public class UpdateCuisineModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CuisineId{ get; set; }
        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        public string CuisineName{ get; set; }
    }

    public class GetCuisineModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CuisineId{ get; set; }
    }

    public class SearchCuisinesModel
    {
        [Required]
        public string CuisineName{ get; set; }
    }
}