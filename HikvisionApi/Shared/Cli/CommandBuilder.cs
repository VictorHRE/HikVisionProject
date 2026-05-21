using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;



namespace Shared.Cli;

public abstract class CommandBuilder<T>(string[] args, Dictionary<string, Type> commands, ServiceProvider provider)
{
    private readonly string[] _args = args;
    private readonly Dictionary<string, Type> _commands = commands;

    private ServiceProvider Provider { get; set; } = provider;

    public abstract T Build(IConfigurationRoot config);

    public virtual void Run()
    {
        var command = GetCommand();

        using var scope = Provider.CreateScope();

        var service = scope.ServiceProvider.GetService(command);
        ((Command)service!)?.Execute(_args);
    }

    private Type GetCommand()
    {
        var command = _commands.FirstOrDefault(cmd => _args.Contains(cmd.Key));
        
        return command.Value ?? throw new SystemException("arguments does not match with any command");
    }
}