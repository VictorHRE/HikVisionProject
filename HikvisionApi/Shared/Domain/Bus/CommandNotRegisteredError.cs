namespace Shared.Domain.Bus;

public class CommandNotRegisteredError(Command command)
    : Exception($"The command {command} has not a command handler associated")
{
}