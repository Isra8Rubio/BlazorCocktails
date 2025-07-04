using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Core.DTO;
using static System.Net.WebRequestMethods;

namespace Infraestructura.Services
{
    public class WeatherClient
    {
        private readonly RestClient _restClient;

        public WeatherClient(RestClient restClient)
        {
            _restClient = restClient;
        }
        public async Task<ProvinceResponseDTO?> GetProvinciasAsync()
        {
            var request = new RestRequest("provincias", Method.Get);
            var response = await _restClient.ExecuteAsync<ProvinceResponseDTO>(request);
            if (!response.IsSuccessful)
                throw new Exception($"ElTiempo API error ({response.StatusCode}): {response.ErrorMessage}");
            return response.Data;
        }

        public async Task<ProvinceDetailResponseDTO?> GetProvinciaDetailAsync(string codProvincia)
        {
            var request = new RestRequest($"provincias/{codProvincia}", Method.Get);
            var response = await _restClient.ExecuteAsync<ProvinceDetailResponseDTO>(request);
            if (!response.IsSuccessful)
                throw new Exception($"ElTiempo API error ({response.StatusCode}): {response.ErrorMessage}");
            return response.Data;
        }

        public async Task<HomeResponseDTO?> GetHomeAsync()
        {
            var request = new RestRequest("home", Method.Get);
            var response = await _restClient.ExecuteAsync<HomeResponseDTO>(request);
            if (!response.IsSuccessful)
                throw new Exception($"ElTiempo API error ({response.StatusCode}): {response.ErrorMessage}");
            return response.Data;
        }

        public async Task<MunicipioResponseDTO?> GetMunicipiosAsync(string codProvincia)
        {
            var request = new RestRequest($"provincias/{codProvincia}/municipios", Method.Get);
            var response = await _restClient.ExecuteAsync<MunicipioResponseDTO>(request);
            if (!response.IsSuccessful)
                throw new Exception($"ElTiempo API error ({response.StatusCode}): {response.ErrorMessage}");
            return response.Data;
        }

        public async Task<MunicipioDetailResponseDTO?> GetMunicipioAsync(string codProvincia, string codMunicipio)
        {
            var request = new RestRequest($"provincias/{codProvincia}/municipios/{codMunicipio}", Method.Get);
            var response = await _restClient.ExecuteAsync<MunicipioDetailResponseDTO>(request);
            if (!response.IsSuccessful)
                throw new Exception($"ElTiempo API error ({response.StatusCode}): {response.ErrorMessage}");
            return response.Data;
        }
    }
}