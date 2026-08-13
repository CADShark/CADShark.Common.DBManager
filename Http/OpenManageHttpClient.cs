using Newtonsoft.Json;
using OpenManage.Client.Contracts;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OpenManage.Client.Http
{
    internal sealed class OpenManageHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContentSerializer _serializer;

        public OpenManageHttpClient(
            HttpClient httpClient,
            IHttpContentSerializer serializer)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public Task<TResponse> GetAsync<TResponse>(
            string requestUri,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendAsync<TResponse>(
                new HttpRequestMessage(HttpMethod.Get, requestUri),
                cancellationToken);
        }

        public Task<TResponse> PostAsync<TRequest, TResponse>(
            string requestUri,
            TRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var message = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = _serializer.CreateContent(request)
            };

            return SendAsync<TResponse>(message, cancellationToken);
        }

        public Task<TResponse> PutAsync<TRequest, TResponse>(
            string requestUri,
            TRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var message = new HttpRequestMessage(HttpMethod.Put, requestUri)
            {
                Content = _serializer.CreateContent(request)
            };

            return SendAsync<TResponse>(message, cancellationToken);
        }

        public Task<TResponse> DeleteAsync<TResponse>(
            string requestUri,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendAsync<TResponse>(
                new HttpRequestMessage(HttpMethod.Delete, requestUri),
                cancellationToken);
        }

        private async Task<TResponse> SendAsync<TResponse>(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            using (request)
            {
                HttpResponseMessage response;

                try
                {
                    response = await _httpClient
                        .SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    throw new OpenManageApiException(
                        "The OpenManage API request timed out.",
                        OpenManageErrorKind.Network);
                }
                catch (HttpRequestException exception)
                {
                    throw new OpenManageApiException(
                        "The OpenManage API is unavailable.",
                        OpenManageErrorKind.Network,
                        innerException: exception);
                }

                using (response)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var responseBody = await response.Content
                        .ReadAsStringAsync()
                        .ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!response.IsSuccessStatusCode)
                    {
                        var apiError = TryReadError<TResponse>(responseBody);

                        throw new OpenManageApiException(
                            apiError?.Message ??
                            $"The OpenManage API returned HTTP {(int)response.StatusCode}.",
                            OpenManageErrorKind.Http,
                            response.StatusCode,
                            apiError?.Code,
                            responseBody);
                    }

                    if (string.IsNullOrWhiteSpace(responseBody))
                    {
                        throw InvalidResponse(
                            response.StatusCode,
                            responseBody,
                            "The OpenManage API returned an empty response.");
                    }

                    ApiResponse<TResponse> envelope;

                    try
                    {
                        envelope = _serializer.Deserialize<ApiResponse<TResponse>>(responseBody);
                    }
                    catch (JsonException exception)
                    {
                        throw InvalidResponse(
                            response.StatusCode,
                            responseBody,
                            "The OpenManage API returned invalid JSON.",
                            exception);
                    }

                    if (envelope == null)
                    {
                        throw InvalidResponse(
                            response.StatusCode,
                            responseBody,
                            "The OpenManage API response envelope is missing.");
                    }

                    if (!envelope.Success)
                    {
                        throw new OpenManageApiException(
                            envelope.Error?.Message ?? "The OpenManage API rejected the request.",
                            OpenManageErrorKind.Api,
                            response.StatusCode,
                            envelope.Error?.Code,
                            responseBody);
                    }

                    if (ReferenceEquals(envelope.Data, null))
                    {
                        throw InvalidResponse(
                            response.StatusCode,
                            responseBody,
                            "The OpenManage API returned a successful response without data.");
                    }

                    return envelope.Data;
                }
            }
        }

        private ApiError TryReadError<TResponse>(string responseBody)
        {
            if (string.IsNullOrWhiteSpace(responseBody))
                return null;

            try
            {
                return _serializer
                    .Deserialize<ApiResponse<TResponse>>(responseBody)
                    ?.Error;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static OpenManageApiException InvalidResponse(
            HttpStatusCode statusCode,
            string responseBody,
            string message,
            Exception innerException = null)
        {
            return new OpenManageApiException(
                message,
                OpenManageErrorKind.InvalidResponse,
                statusCode,
                responseBody: responseBody,
                innerException: innerException);
        }
    }
}
