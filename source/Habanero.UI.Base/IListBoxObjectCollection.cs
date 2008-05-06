namespace Habanero.UI.Base
{
    public interface IListBoxObjectCollection
    {
        void Add(object item);

        int Count { get; }
        void Remove(object item);
        void Clear();
    }
}