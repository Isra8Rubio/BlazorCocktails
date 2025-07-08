using RestSharp;
using System;
using System.Threading.Tasks;
using Core.DTO;

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
            try
            {
                var request = new RestRequest("provincias", Method.Get);
                var response = await _restClient.ExecuteAsync<ProvinceResponseDTO>(request);
                if (!response.IsSuccessful)
                    throw new Exception($"ElTiempo API error ({response.StatusCode}): {response.ErrorMessage}");
                return response.Data;
            }
            catch (Exception ex)
            {
                throw new Exception("WeatherClient.GetProvinciasAsync error", ex);
            }
        }

        public async Task<ProvinceDetailResponseDTO?> GetProvinciaDetailAsync(string codProvincia)
        {
            try
            {
                var request = new RestRequest($"provincias/{codProvincia}", Method.Get);
                var response = await _restClient.ExecuteAsync<ProvinceDetailResponseDTO>(request);
                if (!response.IsSuccessful)
                    throw new Exception($"ElTiempo API error ({response.StatusCode}): {response.ErrorMessage}");
                return response.Data;
            }
            catch (Exception ex)
            {
                throw new Exception("WeatherClient.GetProvinciaDetailAsync error", ex);
            }
        }

        public async Task<HomeResponseDTO?> GetHomeAsync()
        {
            try
            {
                var request = new RestRequest("home", Method.Get);
                var response = await _restClient.ExecuteAsync<HomeResponseDTO>(request);
                if (!response.IsSuccessful)
                    throw new Exception($"ElTiempo API error ({response.StatusCode}): {response.ErrorMessage}");
                return response.Data;
            }
            catch (Exception ex)
            {
                throw new Exception("WeatherClient.GetHomeAsync error", ex);
            }
        }

        public async Task<MunicipioResponseDTO?> GetMunicipiosAsync(string codProvincia)
        {
            try
            {
                var request = new RestRequest($"provincias/{codProvincia}/municipios", Method.Get);
                var response = await _restClient.ExecuteAsync<MunicipioResponseDTO>(request);
                if (!response.IsSuccessful)
                    throw new Exception($"ElTiempo API error ({response.StatusCode}): {response.ErrorMessage}");
                return response.Data;
            }
            catch (Exception ex)
            {
                throw new Exception("WeatherClient.GetMunicipiosAsync error", ex);
            }
        }

        public async Task<MunicipioDetailResponseDTO?> GetMunicipioAsync(string codProvincia, string codMunicipio)
        {
            try
            {
                var request = new RestRequest($"provincias/{codProvincia}/municipios/{codMunicipio}", Method.Get);
                var response = await _restClient.ExecuteAsync<MunicipioDetailResponseDTO>(request);
                if (!response.IsSuccessful)
                    throw new Exception($"ElTiempo API error ({response.StatusCode}): {response.ErrorMessage}");
                return response.Data;
            }
            catch (Exception ex)
            {
                throw new Exception("WeatherClient.GetMunicipioAsync error", ex);
            }
        }
    }
}
