using DataRepoCore;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AppDataAccessCore {
    public class KommissIOAPI : IKommissIOAPI {
        //The httpClient to use.
        private HttpClient _client;

        /// <summary>
        /// The token to use to authenticate the client.
        /// </summary>
        protected string? Token { get; set; } = null;

        ///<inheritdoc/>
        public Employee? CurrentEmployee { get; set; }

        public KommissIOAPI(string baseUri) {
            _client = new HttpClient();

            //Inspired by Microsoft (2023): https://learn.microsoft.com/de-de/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
            _client.BaseAddress = new Uri(baseUri);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <inheritdoc/>
        public async Task<Employee?> IdentifyAndAuthenticateAysnc(short personnelNumber, string password) {
            AuthenticationRequestInformation requestInformation = new AuthenticationRequestInformation();
            requestInformation.PersonnelNumber = personnelNumber;
            requestInformation.Password = password;

            HttpResponseMessage responseMessage = await _client.PostAsJsonAsync("token/aquire/", requestInformation);
            responseMessage.EnsureSuccessStatusCode();
            var strToken = await responseMessage.Content.ReadAsStringAsync();
            Token = strToken.Substring(1, strToken.Length - 2);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            CurrentEmployee = null;
            responseMessage = await _client.GetAsync($"identity/");
            if (responseMessage.IsSuccessStatusCode)
                CurrentEmployee = await responseMessage.Content.ReadFromJsonAsync<Employee>();

            return CurrentEmployee;
        }

        /// <inheritdoc/>
        public async Task<bool> AssignToPickingOrderAsync(PickingOrder order) {
            HttpResponseMessage responseMessage = await _client.GetAsync($"pickingorder/assign/{order.Id}");
            responseMessage.EnsureSuccessStatusCode();
            return bool.TryParse((await responseMessage.Content.ReadAsStringAsync()), out bool response) && response;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<DamageReport>> GetArticleDamageReportsAsync() {
            HttpResponseMessage responseMessage = await _client.GetAsync($"report/damage/all");
            responseMessage.EnsureSuccessStatusCode();
            return (await responseMessage.Content.ReadFromJsonAsync<IEnumerable<DamageReport>>())!;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PickingOrder>> GetFinishedPickingOrdersAsync() {
            HttpResponseMessage responseMessage = await _client.GetAsync($"pickingorder/finished");
            responseMessage.EnsureSuccessStatusCode();
            return (await responseMessage.Content.ReadFromJsonAsync<IEnumerable<PickingOrder>>())!;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PickingOrder>> GetInProgressAssignedPickingOrdersAsync() {
            HttpResponseMessage responseMessage = await _client.GetAsync($"pickingorder/assigned/progress");
            responseMessage.EnsureSuccessStatusCode();
            return (await responseMessage.Content.ReadFromJsonAsync<IEnumerable<PickingOrder>>())!;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PickingOrder>> GetInProgressPickingOrdersAsync() {
            HttpResponseMessage responseMessage = await _client.GetAsync($"pickingorder/progress");
            responseMessage.EnsureSuccessStatusCode();
            return (await responseMessage.Content.ReadFromJsonAsync<IEnumerable<PickingOrder>>())!;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PickingOrder>> GetOpenPickingOrdersAsync() {
            HttpResponseMessage responseMessage = await _client.GetAsync($"pickingorder/open");
            responseMessage.EnsureSuccessStatusCode();
            return (await responseMessage.Content.ReadFromJsonAsync<IEnumerable<PickingOrder>>())!;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<PickingOrder>> GetPickingOrdersAsync() {
            HttpResponseMessage responseMessage = await _client.GetAsync($"pickingorder/all");
            responseMessage.EnsureSuccessStatusCode();
            return (await responseMessage.Content.ReadFromJsonAsync<IEnumerable<PickingOrder>>())!;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<StockPosition>> GetStockPositionsForArticleAsync(Article article) {
            HttpResponseMessage responseMessage = await _client.GetAsync($"stockposition/{article.ArticleNumber}");
            responseMessage.EnsureSuccessStatusCode();
            return (await responseMessage.Content.ReadFromJsonAsync<IEnumerable<StockPosition>>())!;
        }

        /// <inheritdoc/>
        public async Task<bool> PickAsync(PickingOrderPosition orderPosition, StockPosition position, int? amount = null) {
            HttpResponseMessage responseMessage = await _client.GetAsync($"pick/{orderPosition.Id}/{position.Id}/{amount ?? orderPosition.DesiredAmount - orderPosition.PickedAmount}");
            responseMessage.EnsureSuccessStatusCode();
            return bool.TryParse((await responseMessage.Content.ReadAsStringAsync()), out bool response) && response;
        }

        /// <inheritdoc/>
        public async Task<bool> ReportDamagedArticleAsync(Article article, string message) {
            DamageReportFileRequest report = new() { ArticleNumber = article.ArticleNumber, Message = message };

            HttpResponseMessage responseMessage = await _client.PostAsJsonAsync<DamageReportFileRequest>($"report/damage/fileReport/", report);
            responseMessage.EnsureSuccessStatusCode();
            return bool.TryParse((await responseMessage.Content.ReadAsStringAsync()), out bool response) && response;
        }

        /// <inheritdoc/>
        public async Task<bool> ResetToDefaultAsync() {
            HttpResponseMessage responseMessage = await _client.GetAsync($"reset");
            responseMessage.EnsureSuccessStatusCode();
            return bool.TryParse((await responseMessage.Content.ReadAsStringAsync()), out bool response) && response;
        }

        /// <inheritdoc/>
        public void Dispose() {
            _client.Dispose();
            Token = null;
        }
    }
}
