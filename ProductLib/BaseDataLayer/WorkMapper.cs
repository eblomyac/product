using ProductLib.Work;

namespace ProductLib.BaseDataLayer
{
    public class WorkMapper<T> where T:Work.Work, new ()
    {
        public T MapFrom(WorkRecord record)
        {
            T result = new T();
            return result;
        }
    }
    
}