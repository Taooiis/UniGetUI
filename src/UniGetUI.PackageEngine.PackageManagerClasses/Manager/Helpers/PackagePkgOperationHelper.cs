using UniGetUI.Core.Logging;
using UniGetUI.PackageEngine.Enums;
using UniGetUI.PackageEngine.Interfaces;
using UniGetUI.PackageEngine.Interfaces.ManagerProviders;

namespace UniGetUI.PackageEngine.Classes.Manager.BaseProviders;

public abstract class PackagePkgOperationHelper : IPackageOperationHelper
{
    protected IPackageManager Manager;

    public PackagePkgOperationHelper(IPackageManager manager)
    {
        Manager = manager;
    }

    protected abstract IEnumerable<string> _getOperationParameters(
        IPackage package,
        IInstallationOptions options,
        OperationType operation);

    protected abstract OperationVeredict _getOperationResult(
        IPackage package,
        OperationType operation,
        IEnumerable<string> processOutput,
        int returnCode);

    public IEnumerable<string> GetParameters(
        IPackage package,
        IInstallationOptions options,
        OperationType operation)
    {
        var parameters = _getOperationParameters(package, options, operation);
        Logger.Info(
            $"Loaded operation parameters for package id={package.Id} on manager {Manager.Name} and operation {operation}: " +
            string.Join(' ', parameters));
        return parameters;

    }

    public OperationVeredict GetResult(
        IPackage package,
        OperationType operation,
        IEnumerable<string> processOutput,
        int returnCode)
    {

        if (returnCode is 999 && (!processOutput.Any() || processOutput.Last() == "Error: The operation was canceled by the user."))
        {
            Logger.Warn("Elevator [or GSudo] UAC prompt was canceled, not showing error message...");
            return OperationVeredict.Canceled;
        }

        return _getOperationResult(package, operation, processOutput, returnCode);
    }
}