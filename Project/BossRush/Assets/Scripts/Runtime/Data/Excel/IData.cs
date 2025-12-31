namespace TeamSuneat
{
    public interface IData<TKey>
    {
        TKey GetKey();

        void Refresh();

        void OnLoadData();
    }
}