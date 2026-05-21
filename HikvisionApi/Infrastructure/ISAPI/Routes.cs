namespace Infrastructure.ISAPI;

public class Routes
{
	//Routes for Employee system
    public const string ListEmployees = "ISAPI/AccessControl/UserInfo/Search?format=json";
    
    public const string CreateEmployee = "ISAPI/AccessControl/UserInfo/Record?format=json";
    
    public const string DeleteEmployee = "ISAPI/AccessControl/UserInfoDetail/Delete?format=json";
    
    public const string UpdateEmployee = "ISAPI/AccessControl/UserInfo/Modify?format=json";
    
    public const string TotalEmployees = "ISAPI/AccessControl/UserInfo/Count?format=json";

    public const string CaptureFingerPrint = "ISAPI/AccessControl/CaptureFingerPrint";

    public const string SetFingerPrint = "ISAPI/AccessControl/FingerPrint/SetUp?format=json";

    public const string DeleteFingerPrint = "ISAPI/AccessControl/FingerPrint/Delete?format=json";
    
    

    //Routes for Users system
    public const string ListUsers = "ISAPI/Security/users";
    
    public const string CreateUser = "ISAPI/Security/users";
    
    public const string DeleteUser = "ISAPI/Security/users";
    
    public const string UpdateUser = "ISAPI/Security/users";
    
    public const string TotalUsers = "ISAPI/Security/users";

    //Routes for Hosts
    public const string ListHosts = "ISAPI/Event/notification/httpHosts";

    public const string SetHost = "ISAPI/Event/notification/httpHosts";
}