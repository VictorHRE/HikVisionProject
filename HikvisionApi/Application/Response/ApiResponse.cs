using Microsoft.AspNetCore.Mvc;

namespace Application.Response;

public class ApiResponse
{
	public string Message { get; set; }

	public object? Data { get; set; }

	public bool Success { get; set; }

	public int StatusCode { get; set; }


	protected ApiResponse(string message, object? data, bool success, int statusCode)
	{
		Message = message;
		Data = data;
		Success = success;
		StatusCode = statusCode;

	}

	protected ApiResponse()
	{
		Message = string.Empty;
		Data = null;
		Success = false;
		StatusCode = 0;
	}

	public static OkObjectResult OkResult(ApiResponse response)
	{
		return new OkObjectResult(response.Data);
	}

	public static BadRequestObjectResult BadRequestResult(ApiResponse response)
	{
		return new BadRequestObjectResult(response.Data);
	}

	public static NotFoundObjectResult NotFoundResult(ApiResponse response)
	{
		return new NotFoundObjectResult(response.Data);
	}

	public static StatusCodeResult StatusCodeResult(ApiResponse response)
	{
		return new StatusCodeResult(response.StatusCode);
	}

	public object SerializableResponse()
	{
		return new
		{
			Data,
			StatusCode,
			Message
		};
	}
}

public class ApiResponse<T> : ApiResponse
{
	public ApiResponse(string message, T? data, bool success, int statusCode) : base(message, data, success, statusCode)
	{
	}

	public ApiResponse() : base()
	{
	}
}
