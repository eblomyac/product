using System.Collections.Generic;

namespace ProductLib.Post
{
    public abstract class Post
    {
        public string Name { get; private protected set; }
    }

    public class ProductPost : Post
    {
        public ICollection<string> CustomShortNames { get; private set; }

        public string CustomShortNamesString
        {
            get
            {
                if (this.CustomShortNames != null)
                {
                    return string.Join(Constants.Role.ShortNameSplitter, this.CustomShortNames);    
                }

                return "";
            }
            set
            {
                this.CustomShortNames = value.Split(Constants.Role.ShortNameSplitter);
            }
        }

    }
    
}