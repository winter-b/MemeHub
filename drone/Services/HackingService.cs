using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebApi.Interfaces;
using WebApi.Repositories;

namespace WebApi.Services
{
    public class HackingService : IHackingService
    {
        private readonly IHackingRepository _hackingRepository;
        public HackingService(IHackingRepository hackingRepository) {
            _hackingRepository = hackingRepository;
        }
        public string GetHacks()
        {
            var response = "";
            var hacks = _hackingRepository.GetHackAttempts();
            if (hacks.Count >= 1)
            {
                int count = 0;
                foreach (var hack in hacks)
                {
                    count += hack.Count;
                }
                response = $"Hacking attempts total: {count}. Average: {count / hacks.Count} per day";
                var deserializeOptions = new JsonSerializerOptions();
                deserializeOptions.WriteIndented = true;
                response += "\n" + JsonSerializer.Serialize(hacks, deserializeOptions);
            }
            else {
                response = "No hacks";
            }
            return response;
        }
        public void UpsertHackEntry() {
            var date = DateTime.Now.ToShortDateString();
            if (_hackingRepository.GetHackByDate(date)) {
                _hackingRepository.Inc(date);
            }
            else
            {
                _hackingRepository.CreateHackEntry(date);
                _hackingRepository.Inc(date);
            }
        }
    }
}
