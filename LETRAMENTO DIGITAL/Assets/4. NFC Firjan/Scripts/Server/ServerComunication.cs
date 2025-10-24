using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace _4._NFC_Firjan.Scripts.Server
{
	public class ServerComunication : MonoBehaviour
	{
		/// <summary>
		/// Precisa ser colocado para receber do Json em cada aplicação.
		/// </summary>
		public string Ip;
		
		/// <summary>
		/// Precisa ser colocado para receber do Json em cada aplicação.
		/// </summary>
		public int Port;

		private HttpClient _client;

		private void Awake()
		{
			_client = new HttpClient();
			_client.Timeout = TimeSpan.FromSeconds(3);
		}

		private string GetFullEndGameUrl(string nfcId)
		{
			return $"http://{Ip}:{Port}/users/{nfcId}/endgame";
		}

		private string GetFullNfcUrl(string nfcId)
		{
			return $"http://{Ip}:{Port}/users/{nfcId}";
		}

		/// <summary>
		/// Informação deve ser enviada após o jogo para atualizar a pontuação do jogador, aconselho colocar o id do jogo por Json
		/// </summary>
		/// <param name=">gameInfo"><see cref="GameModel"/> Pontuação de cada jogo funciona diferente, olhar no <see href="https://docs.google.com/document/d/14COKL4PcHkT3_J_TiCc79gAZNwbT6pKFmMdV3G9mY0Q/edit?usp=sharing">documento</see></param>
		/// <returns>Codigo de status do update ao server <see cref="HttpStatusCode"/></returns>
		public async Task<HttpStatusCode> UpdateNfcInfoFromGame(GameModel gameInfo)
		{
			try
			{
				var url = GetFullNfcUrl(gameInfo.nfcId);
				var request = new HttpRequestMessage(HttpMethod.Post, url);
				var content = new StringContent(gameInfo.ToString());
				content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
				request.Content = content;
				
				Debug.Log($"[ServerComunication] Enviando para {url}");
				
				var response = await _client.SendAsync(request);
				
				Debug.Log($"[ServerComunication] Resposta: {response.StatusCode}");
				return response.StatusCode;
			}
			catch (HttpRequestException ex)
			{
				Debug.LogWarning($"[ServerComunication] Servidor offline ou inacessível: {ex.Message}");
				return HttpStatusCode.ServiceUnavailable;
			}
			catch (TaskCanceledException ex)
			{
				Debug.LogWarning($"[ServerComunication] Timeout (3s) - Servidor não respondeu: {ex.Message}");
				return HttpStatusCode.RequestTimeout;
			}
			catch (Exception ex)
			{
				Debug.LogWarning($"[ServerComunication] Erro inesperado: {ex.Message}");
				return HttpStatusCode.InternalServerError;
			}
		}

		/// <summary>
		/// Usado para pegar as informações atuais do nfc
		/// </summary>
		/// <param name="nfcId">Nome enviado pelo <see cref="NFC.NFCReceiver"/></param>
		/// <returns><see cref="EndGameResponseModel"></see></returns>
		public async Task<EndGameResponseModel> GetNfcInfo(string nfcId)
		{
			try
			{
				var url = GetFullNfcUrl(nfcId);
				var request = new HttpRequestMessage(HttpMethod.Get, url);
				Debug.Log($"[ServerComunication] GET {url}");
				
				var response = await _client.SendAsync(request);
				Debug.Log($"[ServerComunication] Resposta: {response.StatusCode}");
				
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var content = await response.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<EndGameResponseModel>(content);
				}
				else
				{
					Debug.LogWarning($"[ServerComunication] Falha ao obter info. Status: {response.StatusCode}");
					return null;
				}
			}
			catch (HttpRequestException ex)
			{
				Debug.LogWarning($"[ServerComunication] Servidor offline: {ex.Message}");
				return null;
			}
			catch (TaskCanceledException ex)
			{
				Debug.LogWarning($"[ServerComunication] Timeout: {ex.Message}");
				return null;
			}
			catch (Exception ex)
			{
				Debug.LogWarning($"[ServerComunication] Erro: {ex.Message}");
				return null;
			}
		}
		
		/// <summary>
		/// Usado para enviar o nome do usuario para o Banco de dados e avisar que já passou pela última experiência
		/// </summary>
		/// <param name="endGameRequestModel"></param>
		/// <param name="nfcId"></param>
		/// <returns></returns>
		public async Task<EndGameResponseModel> PostNameForEndGameInfo(EndGameRequestModel endGameRequestModel, string nfcId)
		{
			try
			{
				var url = GetFullEndGameUrl(nfcId);
				var request = new HttpRequestMessage(HttpMethod.Post, url);
				var content = new StringContent(endGameRequestModel.ToString());
				content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
				request.Content = content;
				
				Debug.Log($"[ServerComunication] POST {url}");
				
				var response = await _client.SendAsync(request);
				Debug.Log($"[ServerComunication] Resposta: {response.StatusCode}");
				
				if (response.StatusCode == HttpStatusCode.OK)
				{
					var responseContent = await response.Content.ReadAsStringAsync();
					return JsonConvert.DeserializeObject<EndGameResponseModel>(responseContent);
				}
				else
				{
					Debug.LogWarning($"[ServerComunication] Falha. Status: {response.StatusCode}");
					return null;
				}
			}
			catch (HttpRequestException ex)
			{
				Debug.LogWarning($"[ServerComunication] Servidor offline: {ex.Message}");
				return null;
			}
			catch (TaskCanceledException ex)
			{
				Debug.LogWarning($"[ServerComunication] Timeout: {ex.Message}");
				return null;
			}
			catch (Exception ex)
			{
				Debug.LogWarning($"[ServerComunication] Erro: {ex.Message}");
				return null;
			}
		}
		
		private void OnDestroy()
		{
			_client?.Dispose();
		}
	}
}
