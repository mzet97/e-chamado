using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace EChamado.Client.Services
{
    public class ChamadoService
    {
        private readonly HttpClient _httpClient;

        public ChamadoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<DepartamentoDto>> GetDepartamentosAsync()
        {
            // A mensagem de HTTP incluirá automaticamente o access token
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<DepartamentoDto>>("v1/departments");
            return result;
        }
    }

    public class DepartamentoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
    }
}
