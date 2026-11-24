using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Dalil_Academy.Core.DTOs.Payments;
using Dalil_Academy.Core.Service;
using Microsoft.Extensions.Configuration;

namespace Dalil_Academy.Infra.Service;

public class PayPalPaymentGateway(
    HttpClient httpClient,
    IConfiguration configuration)
    : IPaymentGateway
{
    private readonly string clientId =
        configuration["PayPal:ClientId"]
        ?? throw new InvalidOperationException(
            "PayPal ClientId is missing.");

    private readonly string clientSecret =
        configuration["PayPal:ClientSecret"]
        ?? throw new InvalidOperationException(
            "PayPal ClientSecret is missing.");

    private async Task<string> GetAccessTokenAsync()
    {
        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes(
                $"{clientId}:{clientSecret}"));

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/v1/oauth2/token");

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Basic",
                credentials);

        request.Content = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            });

        using var response =
            await httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        return json.GetProperty("access_token").GetString()
            ?? throw new HttpRequestException(
                "PayPal did not return an access token.");
    }

    public async Task<PayPalOrderResult> CreateOrderAsync(
        decimal amount)
    {
        var token = await GetAccessTokenAsync();

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "/v2/checkout/orders");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        request.Content = JsonContent.Create(new
        {
            intent = "CAPTURE",

            purchase_units = new[]
            {
                new
                {
                    amount = new
                    {
                        currency_code = "USD",
                        value = amount.ToString(
                            "F2",
                            System.Globalization.CultureInfo.InvariantCulture)
                    }
                }
            },

            payment_source = new
            {
                paypal = new
                {
                    experience_context = new
                    {
                        return_url =
                            configuration["PayPal:ReturnUrl"],

                        cancel_url =
                            configuration["PayPal:CancelUrl"],

                        user_action = "PAY_NOW"
                    }
                }
            }
        });

        using var response =
            await httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        var orderId = json.GetProperty("id").GetString()
            ?? throw new HttpRequestException(
                "PayPal did not return an order ID.");

        var links = json.GetProperty("links");

        var approvalUrl = links.EnumerateArray()
            .FirstOrDefault(link =>
            {
                var rel = link.GetProperty("rel").GetString();

                return rel == "approve" ||
                       rel == "payer-action";
            });

        if (approvalUrl.ValueKind == JsonValueKind.Undefined)
        {
            throw new HttpRequestException(
                "PayPal did not return an approval URL.");
        }

        var url = approvalUrl.GetProperty("href").GetString()
            ?? throw new HttpRequestException(
                "PayPal approval URL is missing.");

        return new PayPalOrderResult(orderId, url);
    }

    public async Task<bool> CaptureOrderAsync(string orderId)
    {
        var token = await GetAccessTokenAsync();

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"/v2/checkout/orders/{Uri.EscapeDataString(orderId)}/capture");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        request.Content = JsonContent.Create(new { });

        using var response =
            await httpClient.SendAsync(request);

        if ((int)response.StatusCode >= 500 ||
     response.StatusCode ==
         System.Net.HttpStatusCode.TooManyRequests)
        {
            response.EnsureSuccessStatusCode();
        }

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        return json.TryGetProperty("status", out var status)
            && status.GetString() == "COMPLETED";
    }
}