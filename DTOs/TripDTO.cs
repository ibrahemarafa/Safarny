using System.ComponentModel.DataAnnotations;

namespace APIs_Graduation.DTOs
{
    public class TripDTO
    {

        
        
            public string UserId { get; set; }  
            public int CityId { get; set; }
        //[MinLength(1, ErrorMessage = "At least one tourist place must be selected.")]
        public List<int> TouristPlaceIds { get; set; }
            public int HotelId { get; set; } 
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }

        [Required]
        public decimal MinPrice { get; set; }

        [Required]
        public decimal MaxPrice { get; set; }
       // public decimal Price { get; set; }
        public int StarRating {  get; set; }

    }
}
