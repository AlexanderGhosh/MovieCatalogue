namespace MovieApp.Web.Services.Abstractions
{
    public interface IDragDropService
    {
        void BeginDrag(object dragged);
        object EndDrag();
        T EndDrag<T>();
    }
}