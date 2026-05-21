using System.Text.Json;
using Domain.Device;
using Infrastructure.HttpClients.CentralHubClient;
using Infrastructure.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Shared.Domain.Bus;

namespace Application.Events.CreateEvent;

public class CreateEventCommandHandler : ICommandHandler<CreateEventCommand, bool>
{
	private readonly IDeviceRepository _deviceRepository;
	private readonly IUnitOfWork _unitOfWork;
	private readonly ICentralApiHttpClient _centralApiHttpClient;
	private readonly int _counter;

	public CreateEventCommandHandler(
		IDeviceRepository deviceRepository,
		IUnitOfWork unitOfWork,
		ICentralApiHttpClient centralApiHttpClient,
		IConfiguration configuration)
	{
		_deviceRepository = deviceRepository;
		_unitOfWork = unitOfWork;
		_centralApiHttpClient = centralApiHttpClient;

		_counter = configuration["AttendanceTime"] is not null
			? int.Parse(configuration["AttendanceTime"]!)
			: throw new Exception("Agregar AttendanceTime en appsettings.json");
	}

	public async Task<bool> Handle(CreateEventCommand command)
	{
		try
		{
			var isSubEvent = Enum.IsDefined(typeof(SubEventType), command.SubeEventType);

			if (!isSubEvent) return true;

			await _unitOfWork.DeviceRepository
				.InsertEventLogAsync(
					command.Data,
					command.SubeEventType,
					_centralApiHttpClient.GetIdStore(),
					CancellationToken.None
				);

			await _unitOfWork.CommitAsync(CancellationToken.None);

			/*var res = await _centralApiHttpClient.SendPostAsync(
				"eventlog/add-eventlog", JsonSerializer.Serialize(
					new CreateEventLogRequest()
					{
						Id = 0,
						Data = command.Data,
						EventType = command.SubeEventType.ToString(),
						CreatedAt = DateTime.Now,
						IdStoreHQ = _centralApiHttpClient.GetIdStore(),
						EmployeeIdentification = "-"
					}
				));

			var content = res.Content.ReadAsStringAsync();*/

			if ((SubEventType)command.SubeEventType == SubEventType.FINGER_PRINT_ACCESS_GRANTED)
			{
				var employee = await _unitOfWork.EmployeeRepository.GetEmployeeAsync(command.EmployeeId)
							   ?? throw new Exception("Employee not found");

				var hasAttendance = await _unitOfWork.AttendanceRepository.GetEmployeeAttendanceAsync(employee);

				if (hasAttendance is null)
				{
					var countAttendace = await _unitOfWork.AttendanceRepository.GetDayEmployeeAttendancesAsync(employee, Domain.Attendance.AttendanceType.CheckIn);

					if (countAttendace.Count == 0)
					{
						await _unitOfWork.AttendanceRepository.InsertAttendanceAsync(
							employee,
							Domain.Attendance.AttendanceType.CheckIn
						);

						var body = JsonSerializer.Serialize(new CreateEventLogRequest()
						{
							Id = 0,
							Data = JsonSerializer.Serialize(employee),
							EventType = Enum.GetName(typeof(SubEventType), command.SubeEventType)!,
							CreatedAt = DateTime.Now,
							IdStoreHQ = _centralApiHttpClient.GetIdStore(),
							EmployeeIdentification = employee.Email //employee.Identification.Value
						});

						_ = await _centralApiHttpClient.SendPostAsync("eventlog/add-eventlog-clockin", body);
					}
				}

				if (hasAttendance is not null &&
					hasAttendance.AttendanceType == Domain.Attendance.AttendanceType.CheckIn)
				{
					var currentTime = DateTime.Now;
					var diffTime = (currentTime - hasAttendance.Time);

					var countAttendace = await _unitOfWork.AttendanceRepository.GetDayEmployeeAttendancesAsync(employee, Domain.Attendance.AttendanceType.CheckOut);

					var timer = diffTime >= TimeSpan.FromMinutes(_counter);

					if (timer && countAttendace.Count == 0)
					{
						await _unitOfWork.AttendanceRepository.InsertAttendanceAsync(
							employee,
							Domain.Attendance.AttendanceType.CheckOut
						);

						var body = JsonSerializer.Serialize(
								new CreateEventLogRequest()
								{
									Id = 0,
									Data = JsonSerializer.Serialize(employee),
									EventType = Enum.GetName(typeof(SubEventType), command.SubeEventType)!,
									CreatedAt = DateTime.Now,
									IdStoreHQ = _centralApiHttpClient.GetIdStore(),
									EmployeeIdentification = employee.Email
								}
						);

						_ = await _centralApiHttpClient.SendPostAsync("eventlog/add-eventlog-clockout", body);
					}
				}
			}

			return true;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return false;
		}
	}
}