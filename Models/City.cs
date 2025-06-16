namespace APIs_Graduation.Models
{
    public class City : BaseEntity
    {
        public string Name { get; set; }
        //public  string Description { get; set; }
         public  string PictureUrl { get; set; }
        public  string type { get; set; } 
        public double? Price { get; set; }
        public string? Airport { get; set; }
    }
}
