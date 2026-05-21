namespace Domain.Employee;

public class UserInfoSearchResponse
{
	public UserInfoSearch UserInfoSearch { get; set; } = new();
}

public class UserInfoSearch
{
	public string searchID { get; set; } = string.Empty;
	public string responseStatusStrg { get; set; } = string.Empty;
	public int numOfMatches { get; set; }
	public int totalMatches { get; set; }
	public List<UserInfo> UserInfo { get; set; } = [];
}

public class UserInfo
{
	public string employeeNo { get; set; } = string.Empty;
	public string name { get; set; } = string.Empty;
	public string userType { get; set; } = string.Empty;
	public int sortByNamePosition { get; set; }
	public string sortByNameFlag { get; set; } = string.Empty;
	public bool closeDelayEnabled { get; set; }
	public Valid Valid { get; set; } = new Valid();
	public string belongGroup { get; set; } = string.Empty;
	public string password { get; set; } = string.Empty;
	public string doorRight { get; set; } = string.Empty;
	public List<RightPlan> RightPlan { get; set; } = [];
	public int maxOpenDoorTime { get; set; }
	public int openDoorTime { get; set; }
	public bool localUIRight { get; set; }
	public string gender { get; set; } = string.Empty;
	public int numOfCard { get; set; }
	public int numOfFP { get; set; }
	public int numOfFace { get; set; }
	public int groupId { get; set; }
	public int localAtndPlanTemplateId { get; set; }
	public List<PersonInfoExtend> PersonInfoExtends { get; set; }	= [];
}

public class Valid
{
	public bool enable { get; set; }
	public string beginTime { get; set; } = string.Empty; // o DateTime, si quieres parsearlo automáticamente
	public string endTime { get; set; } = string.Empty;
	public string timeType { get; set; } = string.Empty;
}

public class RightPlan
{
	public int doorNo { get; set; }
	public string planTemplateNo { get; set; } = string.Empty;
}

public class PersonInfoExtend
{
	public string value { get; set; } = string.Empty;
}

