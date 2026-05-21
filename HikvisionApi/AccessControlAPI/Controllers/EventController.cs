using AMPMAccesControlAPI.Request.Event;
using Application.Events.CreateEvent;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Bus;
using System.Text.Json;

namespace AMPMAccessControlAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EventController : ControllerBase
	{
		private readonly ICommandBus _commandBus;

		public EventController(ICommandBus commandBus)
		{
			_commandBus = commandBus;
		}


		[HttpPost]
		public async Task<IActionResult> HandleEvent()
		{

			// Verificar que sea multipart/form-data
			if (!Request.HasFormContentType)
				return BadRequest("No es multipart/form-data");

			var form = await Request.ReadFormAsync();

			// Hikvision manda el JSON en el campo "event_log"
			var eventJson = form["event_log"].ToString();

			if (string.IsNullOrEmpty(eventJson))
				return BadRequest("No se encontró el campo event_log");

			Console.WriteLine("\n");
			Console.WriteLine("************************************ DETALLE DEL EVENTO ************************************");
			Console.WriteLine("Evento recibido:");
			Console.WriteLine(eventJson);

			try
			{
				var evento = JsonSerializer
					.Deserialize<HikvisionEventRequest>(eventJson, new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true,
						NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
					}
					);

				var eventType = (evento?.EventType ?? evento?.AccessControllerEvent?.CurrentVerifyMode) ?? "Desconocido";

				if (eventType == "fp")
				{
					Console.Clear();
					Console.WriteLine($"Evento: {evento?.EventType} - {evento?.AccessControllerEvent?.CurrentVerifyMode}");
				}
				else
				{
					Console.WriteLine($"Evento: {evento?.EventType} - {evento?.AccessControllerEvent?.CurrentVerifyMode}");
				}
				if (evento?.AccessControllerEvent?.SubEventType is not null)
				{
					var _ = await _commandBus.DispatchAsync<bool>(new CreateEventCommand()
					{
						Data = eventJson,
						SubeEventType = int.Parse(evento.AccessControllerEvent.SubEventType.ToString() ?? "-1"),
						EmployeeId = evento.AccessControllerEvent.EmployeeNoString!
					});
				}

				Console.WriteLine("\n");
				Console.WriteLine("************************************ FIN DETALLE DEL EVENTO ************************************");

			}
			catch (Exception ex)
			{
				Console.WriteLine("Error al parsear JSON: " + ex.Message);
				return BadRequest("Error al parsear el JSON");
			}

			return Ok("Recibido");
		}
	}
}
