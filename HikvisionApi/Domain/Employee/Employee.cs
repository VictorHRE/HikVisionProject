namespace Domain.Employee;

public class Employee
{
	public int Id { get; set; }

	public EmployeeId Identification { get; set; }

	public string Name { get; set; }

	public string LastName { get; set; }

	public string? Position { get; set; }

	public string? Phone { get; set; }

	public string? Email { get; set; }

	public int BranchId { get; set; }//idStore

	private EmployeeStatus? _status;

	public EmployeeStatus? Status
	{
		get  => _status; 
		set  => _status = value ?? EmployeeStatus.UNKNOWN;
	}

	private EmployeeType? _userType;

	public EmployeeType? UserType
	{
		get => _userType; 
		set => _userType = value ?? EmployeeType.normal;
	}

	private EmployeeGender? _gender;

	public EmployeeGender? Gender
	{
		get => _gender; 
		set => _gender = value ?? EmployeeGender.male;
	}

	public DateTime BeginTime { get; set; }

	public DateTime EndTime { get; set; }

	public DateTime BirthDate { get; set; }

	public DateTime CreatedAt { get; set; }

	public Employee(
		int id,
		EmployeeId identification,
		string name,
		string lastName,
		string? position,
		string? phone,
		string? email,
		int branchId,
		EmployeeStatus? status,
		EmployeeType? userType,
		EmployeeGender? gender,
		DateTime beginTime,
		DateTime endTime,
		DateTime birthDate,
		DateTime createdAt)
	{
		Id = id;
		Identification = identification;
		Name = name;
		LastName = lastName;
		Position = position;
		Phone = phone;
		Email = email;
		BranchId = branchId;
		Status = status;
		UserType = userType;
		Gender = gender;
		BeginTime = beginTime;
		EndTime = endTime;
		BirthDate = birthDate;
		CreatedAt = createdAt;
		
		_status = status;
		_userType = userType;
		_gender = gender;
	}

	public Employee()
	{
		Id = default;
		Identification = new EmployeeId(string.Empty);
		Name = string.Empty;
		LastName = string.Empty;
		Position = string.Empty;
		Phone = string.Empty;
		Email = string.Empty;
		BranchId = default;
		Status = EmployeeStatus.UNKNOWN;
		UserType = EmployeeType.unknown;
		Gender = EmployeeGender.unknown;
		BeginTime = DateTime.Now;
		EndTime = DateTime.Now;
		BirthDate = DateTime.Now;
		CreatedAt = DateTime.Now;
	}

	public static Employee Create(
		int id,
		EmployeeId identification,
		string name,
		string lastName,
		string? position,
		string? phone,
		string? email,
		int branchId,
		EmployeeStatus? status,
		EmployeeType? userType,
		EmployeeGender? gender,
		DateTime beginDate,
		DateTime endDate,
		DateTime birthDate,
		DateTime createdAt) => new(
			id: id,
			identification: identification,
			name: name,
			lastName: lastName,
			position: position,
			phone: phone,
			email: email,
			branchId: branchId,
			status: status,
			userType: userType,
			gender: gender,
			beginTime: beginDate,
			endTime: endDate,
			birthDate: birthDate,
			createdAt: createdAt
		);

}