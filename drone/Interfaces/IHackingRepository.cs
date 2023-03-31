using System.Collections.Generic;
using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IHackingRepository
    {
        List<Hacks> GetHackAttempts();
        bool GetHackByDate(string date);
        void CreateHackEntry(string date);
        void Inc(string date);
    }
}