namespace WebApi.Interfaces
{
    public interface IHackingService
    {
        string GetHacks();
        void UpsertHackEntry();
    }
}