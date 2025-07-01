using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Weather.core.DTO;
using static System.Net.WebRequestMethods;

namespace Weather.infra.ExternalClients
{
    public class WeatherClient
    {
        private readonly HttpClient httpClient;

        public WeatherClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            httpClient.BaseAddress = new Uri("https://www.el-tiempo.net/api/json/v2/");
        }

        public async Task<ProvinceResponseDTO?> GetProvinciasAsync()
        {
            var response = await httpClient.GetAsync("provincias");
            return await response.Content.ReadFromJsonAsync<ProvinceResponseDTO>();
        }
        public async Task<ProvinceDetailResponseDTO?> GetProvinciaDetailAsync(string codProvincia)
        {
            var response = await httpClient.GetAsync($"provincias/{codProvincia}");
            return await response.Content.ReadFromJsonAsync<ProvinceDetailResponseDTO>();
        }
        public async Task<HomeResponseDTO?> GetHomeAsync()
        {
            var response = await httpClient.GetAsync("home");
            return await response.Content.ReadFromJsonAsync<HomeResponseDTO>();
        }
        public async Task<MunicipioResponseDTO?> GetMunicipiosAsync(string codProvincia)
        {
            var response = await httpClient.GetAsync($"provincias/{codProvincia}/municipios");
            return await response.Content.ReadFromJsonAsync<MunicipioResponseDTO>();
        }
        public async Task<MunicipioDetailResponseDTO?> GetMunicipioAsync(string codProvincia, string codMunicipio)
        {
            // Example: GET https://www.el-tiempo.net/api/json/v2/provincias/28/municipios/28140
            var response = await httpClient.GetAsync($"provincias/{codProvincia}/municipios/{codMunicipio}");
            return await response.Content.ReadFromJsonAsync<MunicipioDetailResponseDTO>();
        }
        // restsharp
    }
}
