using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace API.APIService
{
    public partial class APIClient
    {
        // --- HOOKS ASINCRONOS QUE FALTAN ---
        protected virtual Task PrepareRequestAsync(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder, CancellationToken cancellationToken)
            => Task.CompletedTask;

        protected virtual Task PrepareRequestAsync(HttpClient client, HttpRequestMessage request, string url, CancellationToken cancellationToken)
            => Task.CompletedTask;

        protected virtual Task ProcessResponseAsync(HttpClient client, HttpResponseMessage response, CancellationToken cancellationToken)
            => Task.CompletedTask;

        // --- (Opcional) versiones síncronas, por si el cliente también las llama ---
        protected virtual void PrepareRequest(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder) { }
        protected virtual void PrepareRequest(HttpClient client, HttpRequestMessage request, string url) { }
        protected virtual void ProcessResponse(HttpClient client, HttpResponseMessage response) { }

        // EJEMPLO: añadir un Bearer token antes de cada request
        // protected override Task PrepareRequestAsync(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder, CancellationToken ct)
        // {
        //     request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "<token>");
        //     return Task.CompletedTask;
        // }
    }
}
