using Core.DTO;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructura.Services
{
    public class CocktailClientService
    {
        private readonly RestClient restClient;

        public CocktailClientService(RestClient restClient)
        {
            this.restClient = restClient;
        }

        public async Task<List<AlcoholTypeDTO>?> GetAlcoholTypesAsync()
        {
            try
            {
                // La ruta relativa que hemos expuesto en nuestro controller
                var request = new RestRequest("list.php", Method.Get);
                request.AddQueryParameter("a", "list");

                var response = await restClient.ExecuteAsync<AlcoholTypeResponseDTO>(request);
                if (!response.IsSuccessful)
                    throw new Exception($"CocktailDB API error ({response.StatusCode}): {response.ErrorMessage}");

                // CocktailTypeListResponse.Drinks será Map<string strAlcoholic>
                return response.Data?.Drinks
                             .Select(x => new AlcoholTypeDTO { StrAlcoholic = x.StrAlcoholic })
                             .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("CocktailClient.GetTypesAsync error", ex);
            }
        }

        public async Task<List<CocktailItemDTO>?> GetByTypeAsync(string type)
        {
            try
            {
                var request = new RestRequest("filter.php", Method.Get);
                request.AddQueryParameter("a", type);

                var response = await restClient.ExecuteAsync<CocktailItemResponseDTO>(request);
                if (!response.IsSuccessful)
                    throw new Exception($"CocktailDB API error ({response.StatusCode}): {response.ErrorMessage}");

                return response.Data?.Drinks;
            }
            catch (Exception ex)
            {
                throw new Exception("CocktailClientService.GetByTypeAsync error", ex);
            }
        }

        public async Task<CocktailDetailDTO?> GetByIdAsync(string id)
        {
            try
            {
                var request = new RestRequest("lookup.php", Method.Get);
                request.AddQueryParameter("i", id);

                var response = await restClient.ExecuteAsync<CocktailLookupResponseDTO>(request);
                if (!response.IsSuccessful)
                    throw new Exception($"CocktailDB API error ({response.StatusCode}): {response.ErrorMessage}");

                var raw = response.Data?.Drinks?.FirstOrDefault();
                if (raw == null) return null;

                // Mapear ingredientes dinámicamente
                var ingredients = new List<IngredientDTO>();
                for (int i = 1; i <= 15; i++)
                {
                    var propIng = raw.GetType().GetProperty($"StrIngredient{i}")?.GetValue(raw) as string;
                    var propMea = raw.GetType().GetProperty($"StrMeasure{i}")?.GetValue(raw) as string;
                    if (!string.IsNullOrWhiteSpace(propIng))
                        ingredients.Add(new IngredientDTO { Name = propIng!, Measure = propMea ?? "" });
                }

                return new CocktailDetailDTO
                {
                    IdDrink = raw.IdDrink,
                    StrDrink = raw.StrDrink,
                    StrCategory = raw.StrCategory,
                    StrAlcoholic = raw.StrAlcoholic,
                    StrGlass = raw.StrGlass,
                    StrInstructions = raw.StrInstructions,
                    StrDrinkThumb = raw.StrDrinkThumb,
                    Ingredients = ingredients
                };
            }
            catch (Exception ex)
            {
                throw new Exception("CocktailClientService.GetByIdAsync error", ex);
            }
        }

        public async Task<List<CategoryDTO>?> GetCategoriesAsync()
        {
            try
            {
                var request = new RestRequest("list.php", Method.Get)
                    .AddQueryParameter("c", "list");

                var response = await restClient.ExecuteAsync<CategoryResponseDTO>(request);
                if (!response.IsSuccessful)
                    throw new Exception($@"
                        CocktailDB API error:
                        StatusCode: {response.StatusCode}
                        ErrorMessage: {response.ErrorMessage}
                        Content: {response.Content}
                    ");

                return response.Data?.Drinks;
            }
            catch (Exception ex)
            {
                throw new Exception("CocktailClientService.GetCategoriesAsync error", ex);
            }
        }

        public async Task<List<CocktailItemDTO>?> GetByCategoryAsync(string category)
        {
            try
            {
                var request = new RestRequest("filter.php", Method.Get)
                    .AddQueryParameter("c", category);

                var response = await restClient.ExecuteAsync<CocktailItemResponseDTO>(request);
                if (!response.IsSuccessful)
                    throw new Exception($@"
                        CocktailDB API error:
                        StatusCode: {response.StatusCode}
                        ErrorMessage: {response.ErrorMessage}
                        Content: {response.Content}
                    ");

                return response.Data?.Drinks;
            }
            catch (Exception ex)
            {
                throw new Exception("CocktailClientService.GetByCategoryAsync error", ex);
            }
        }


        // Obtiene la lista de todos los tipos de vasos.
        public async Task<List<GlassDTO>?> GetGlassesAsync()
        {
            try
            {
                var request = new RestRequest("list.php", Method.Get)
                    .AddQueryParameter("g", "list");

                var response = await restClient.ExecuteAsync<GlassResponseDTO>(request);
                if (!response.IsSuccessful)
                    throw new Exception($@"
                        CocktailDB API error:
                        StatusCode: {response.StatusCode}
                        ErrorMessage: {response.ErrorMessage}
                        Content: {response.Content}
                    ");
                return response.Data?.Drinks;
            }
            catch (Exception ex)
            {
                throw new Exception("CocktailClientService.GetGlassesAsync error", ex);
            }
        }


        // Filtra cócteles por tipo de vaso.
        public async Task<List<CocktailItemDTO>?> GetByGlassAsync(string glass)
        {
            try
            {
                var request = new RestRequest("filter.php", Method.Get)
                    .AddQueryParameter("g", glass);

                var response = await restClient.ExecuteAsync<CocktailItemResponseDTO>(request);
                if (!response.IsSuccessful)
                    throw new Exception($@"
                        CocktailDB API error:
                        StatusCode: {response.StatusCode}
                        ErrorMessage: {response.ErrorMessage}
                        Content: {response.Content}
                    ");
                return response.Data?.Drinks;
            }
            catch (Exception ex)
            {
                throw new Exception("CocktailClientService.GetByGlassAsync error", ex);
            }
        }

    }
}
