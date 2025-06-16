namespace APIs_Graduation.Models
{
    public class Package_Images : BaseEntity
    {
        public string PictureUrl { get; set; }
        public int PackageId { get; set; }
        public Package Package { get; set; }
    }
}
