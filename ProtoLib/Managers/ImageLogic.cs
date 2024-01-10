using System;
using System.Linq;
using ProtoLib.Model;

namespace ProtoLib.Managers
{
    public class ImageLogic
    {
        public string? GetImagePath(Guid GUID)
        {
            using (BaseContext c = new BaseContext())
            {
                return c.Images.FirstOrDefault(x => x.Id == GUID)?.LocalPath;
            }
        }

        public string? GetImagePath(string GUID)
        {
            using (BaseContext c = new BaseContext())
            {
                return c.Images.FirstOrDefault(x => x.Id == Guid.Parse(GUID))?.LocalPath;
            }
        }

        public void StoreImage(byte[] file, string fileName)
        {
            
        }
        
    }
}