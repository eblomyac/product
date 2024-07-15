namespace ProtoLib2.Model;

public class ImageSet
{
    public Guid Id { get; set; }
    public Guid ImageId { get; set; }
    public List<StoredImage> Images { get; set; }
}