using System;
using System.Reflection.Metadata;

namespace ProductLib.Work
{
    public class ArticleWork : Work, IEquatable<ArticleWork>
    {
        public override string FullName
        {
            get
            {
                return string.Join(' ', Prefix, Article);
            }
        }

        public virtual string Article { get;  set; }
        public virtual string Post { get; set; }
        public ArticleWork(string article, string post):base()
        {
            this.Article = article;
            this.Post = post;
            this.Prefix = Constants.Work.ArticleWorkPrefix;
        }

        public ArticleWork():base()
        {
            this.Article = "";
            this.Post = "";
            this.Prefix = Constants.Work.ArticleWorkPrefix;
        }

        public bool Equals(ArticleWork? other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Article == other.Article && this.Post == other.Post;
        }
    }
}