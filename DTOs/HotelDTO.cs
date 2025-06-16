using System.ComponentModel.DataAnnotations;

namespace APIs_Graduation.DTOs
{
    public class HotelDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public double Rate { get; set; }
        public string Address { get; set; }
        public string Overview { get; set; }
        public double StartPrice { get; set; }
       // public List<string> PictureUrls { get; set; }
       // [Range(1, 5)]
      //  public int Rate{ get; set; }

       // public decimal StartPrice {  get; set; }

        public int CityId { get; set; }
    }
}
