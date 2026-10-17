namespace MovieApp.Web
{
    public class DragDropService : IDragDropService
    {
        private object? _dragged;

        public void BeginDrag(object dragged)
        {
            ArgumentNullException.ThrowIfNull(dragged);
            _dragged = dragged;
        }

        public object EndDrag()
        {
            if (_dragged is null)
                throw new InvalidOperationException("Cannot end drag before starting one.");
            var res = _dragged;
            _dragged = null;
            return res;
        }

        public T EndDrag<T>()
        {
            if (_dragged is null)
                throw new InvalidOperationException("Cannot end drag before starting one.");
            var res = (T)_dragged;
            _dragged = null;
            return res;
        }
    }
}
