namespace ProtoLib2.Model;

public class StoredImage
{
    public Guid Id { get; set; }
    public string InitialFileName { get; set; }
    public string LocalPath { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
}