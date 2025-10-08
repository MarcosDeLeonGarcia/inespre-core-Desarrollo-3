// EventsApi
namespace INESPRE.Desktop.Api
{
    public sealed class EventsApi : ApiClient
    {
        private readonly string _endpoint;
        public EventsApi(string endpoint = "/api/events") => _endpoint = endpoint;

        public async Task<List<EventDto>> GetAllAsync(string? name = null)
        {
            var url = BuildUrl(_endpoint, new Dictionary<string, string?> { ["name"] = name });
            return await GetMaybeAsync<List<EventDto>>(url) ?? new List<EventDto>();
        }

        public Task<EventDto?> CreateAsync(EventCreateRequest dto)
            => PostMaybeAsync<EventCreateRequest, EventDto>(_endpoint, dto);

        public async Task UpdateAsync(int id, EventUpdateRequest dto)
        {
            dto.EventId = id;
            await PutAsync($"{_endpoint}/{id}", dto);
        }

        public Task DeleteAsync(int id)
            => DeleteAsync($"{_endpoint}/{id}");
    }
}
