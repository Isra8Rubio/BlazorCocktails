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
                    throw new Exception($@"
                        CocktailDB API error:
                        StatusCode: {response.StatusCode}
                        ErrorMessage: {response.ErrorMessage}
                        Content: {response.Content}
                    ");

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

        public async Task<List<CocktailItemDTO>?> GetByAlcoholTypeAsync(string type)
        {
            try
            {
                var request = new RestRequest("filter.php", Method.Get);
                request.AddQueryParameter("a", type);

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
                throw new Exception("CocktailClientService.GetByTypeAsync error", ex);
            }
        }

        public async Task<CocktailDetailDTO?> GetByIdAsync(string id)
        {
            try
            {
                // 1) Preparamos y enviamos la petición a la API
                var request = new RestRequest("lookup.php", Method.Get)
                    .AddQueryParameter("i", id);

                var response = await restClient.ExecuteAsync<CocktailLookupResponseDTO>(request);

                if (!response.IsSuccessful)
                    throw new Exception($@"
                        CocktailDB API error:
                        StatusCode: {response.StatusCode}
                        ErrorMessage: {response.ErrorMessage}
                        Content: {response.Content}
                    ");

                // 2) Extraemos la lista de bebidas cruda y nos quedamos con la primera
                var drinks = response.Data?.Drinks;
                var apiDrink = drinks?.FirstOrDefault();
                if (apiDrink == null)
                    return null;

                // 3) Mapeamos dinámicamente los ingredientes
                var mappedIngredients = new List<IngredientDTO>();
                for (int index = 1; index <= 15; index++)
                {
                    // Propiedades como StrIngredient1, StrIngredient2, … StrMeasure1, StrMeasure2, …
                    var ingredientName = apiDrink
                        .GetType()
                        .GetProperty($"StrIngredient{index}")
                        ?.GetValue(apiDrink) as string;

                    var measureText = apiDrink
                        .GetType()
                        .GetProperty($"StrMeasure{index}")
                        ?.GetValue(apiDrink) as string;

                    if (!string.IsNullOrWhiteSpace(ingredientName))
                    {
                        mappedIngredients.Add(new IngredientDTO
                        {
                            Name = ingredientName,
                            Measure = measureText ?? string.Empty
                        });
                    }
                }

                // 4) Construimos el DTO final
                var cocktailDetail = new CocktailDetailDTO
                {
                    IdDrink = apiDrink.IdDrink,
                    StrDrink = apiDrink.StrDrink,
                    StrCategory = apiDrink.StrCategory,
                    StrAlcoholic = apiDrink.StrAlcoholic,
                    StrGlass = apiDrink.StrGlass,
                    StrInstructions = apiDrink.StrInstructions,
                    StrDrinkThumb = apiDrink.StrDrinkThumb,
                    Ingredients = mappedIngredients
                };

                return cocktailDetail;
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

        // Obtiene la lista de todos los ingredientes.
        public async Task<List<IngredientSummaryDTO>?> GetIngredientsAsync()
        {
            try
            {
                var request = new RestRequest("list.php", Method.Get)
                    .AddQueryParameter("i", "list");

                var response = await restClient.ExecuteAsync<IngredientResponseDTO>(request);
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
                throw new Exception("CocktailClientService.GetIngredientsAsync error", ex);
            }
        }

        public async Task<IngredientDetailDTO?> GetIngredientByIdAsync(string id)
        {
            try
            {
                var request = new RestRequest("lookup.php", Method.Get)
                    .AddQueryParameter("iid", id);

                var response = await restClient
                    .ExecuteAsync<IngredientLookupResponseDTO>(request);

                if (!response.IsSuccessful)
                    throw new Exception($@"
                        CocktailDB API error:
                        StatusCode: {response.StatusCode}
                        ErrorMessage: {response.ErrorMessage}
                        Content: {response.Content}
                    ");

                // 1) Extraemos la lista de ingredientes del payload
                var ingredients = response.Data?.Ingredients;
                // 2) Tomamos el primero
                var apiIngredient = ingredients?.FirstOrDefault();
                if (apiIngredient == null)
                    return null;

                // 3) Mapeamos al DTO de detalle
                var detail = new IngredientDetailDTO
                {
                    IdIngredient = apiIngredient.IdIngredient,
                    Name = apiIngredient.Name,
                    Type = apiIngredient.Type,
                    Description = apiIngredient.Description
                };

                return detail;
            }
            catch (Exception ex)
            {
                throw new Exception("CocktailClientService.GetIngredientByIdAsync error", ex);
            }
        }

    }
}
