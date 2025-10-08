namespace INESPRE.Desktop.Api
{
    public class RolesApi : ApiClient
    {
        public Task<List<RoleDto>> GetAllAsync() => GetAsync<List<RoleDto>>("/api/roles");
        public Task CreateAsync(RoleCreateRequest dto) => PostAsync("/api/roles", dto);
        public Task UpdateAsync(int id, RoleUpdateRequest dto) { dto.RoleId = id; return PutAsync($"/api/roles/{id}", dto); }
        public Task DeleteAsync(int id) => DeleteAsync($"/api/roles/{id}");
    }
}
