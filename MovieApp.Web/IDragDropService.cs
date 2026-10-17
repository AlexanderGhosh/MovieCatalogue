namespace MovieApp.Web
{
    public interface IDragDropService
    {
        void BeginDrag(object dragged);
        object EndDrag();
        T EndDrag<T>();
    }
}